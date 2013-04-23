' Note: this midi handler was designed only to read the midi files used in frets on fire type games. 
' As such, not all types of midi events are supported by this handler. For example, channel after 
' touch events and patch change events could cause this handler to fail. As these events are not 
' used in guitar hero / frets on fire files, this should not be a problem.
Imports System.IO
Imports System.Text
Imports System.Math
Public Class MidiReader

    Private midiPath As String
    Private midiFile As FileStream
    Private fileFormat As Integer
    Private numTracks As Integer
    Private currentTrack As Integer
    Private buff() As Byte
    Private deltaTime As Double   ' number of ticks per beat (quarter note)
    Private totalNoteCount As Integer

    Private tempos(1, 1) As Integer ' array containing tempo changes and when they come into effect (in abolute time in ticks) - tempos are in microseconds per beat
    Private notes(1, 2) As Integer  ' array containing note numbers (in decimal) their absolute start time, and their absolute stop times (in ticks).
    Private noteOnTrueTimes() As Double
    Private noteOffTrueTimes() As Double

    Private nextTrackInd As Integer = 0 ' index dictating when the next track should start

    Public dataLoaded As Boolean = False

    Public Sub New(ByVal path As String)
        midiPath = path

        readHeader()
        Console.Write("number of tracks: " & numTracks & vbNewLine)
        If numTracks = 2 Then
            currentTrack = 1
            readTrack(nextTrackInd) ' start at byte 14 - the first track will allways start at 14
            currentTrack = 2
            readTrack(nextTrackInd)
            If notes.Length > 0 Then dataLoaded = True
        Else
            Console.Write("your midi file must have two tracks: one for tempo information and one for actual note events. Fix it")
        End If

        ' get the true note times - (in milisecinds, not ticks)
        getTrueNoteTimes()

    End Sub



    '--------------------------------------------------------------------------------'
    '-------------------------------- Read midi header ------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub readHeader()
        Dim length As Integer = 13
        Dim Data(length) As Byte
        Dim headerTag(3) As Byte
        Dim datLength(3) As Byte
        Dim format(1) As Byte
        Dim nTracks(1) As Byte
        Dim deltT(1) As Byte

        Try
            midiFile = New FileStream(midiPath, FileMode.Open, FileAccess.Read)
        Catch ex As Exception
            Console.Write("there is somthing wrong with either your midi file or it's path")
        End Try

        For i = 0 To length Step 1
            Data(i) = midiFile.ReadByte()
        Next

        Array.Copy(Data, 0, headerTag, 0, 4)
        Array.Copy(Data, 4, datLength, 0, 4) : Array.Reverse(datLength)
        Array.Copy(Data, 8, format, 0, 2) : Array.Reverse(format)
        Array.Copy(Data, 10, nTracks, 0, 2) : Array.Reverse(nTracks)
        Array.Copy(Data, 12, deltT, 0, 2) : Array.Reverse(deltT)

        If Not (Encoding.ASCII.GetString(headerTag) = "MThd") Then
            Console.Write("your midi file is not properly formatted")
        Else

            fileFormat = BitConverter.ToInt16(format, 0)
            numTracks = BitConverter.ToInt16(nTracks, 0)
            deltaTime = BitConverter.ToInt16(deltT, 0)
            nextTrackInd = 14
        End If

        midiFile.Close()
        midiFile.Dispose()
    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------- Read a midi track chunk --------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub readTrack(ByVal startIndex As Integer)
        Dim trackHeader(3) As Byte
        Dim numBytes(3) As Byte
        Dim Data(7) As Byte
        Dim trackLength As Integer
        Dim trackBytes() As Byte

        Try
            midiFile = New FileStream(midiPath, FileMode.Open, FileAccess.Read)
        Catch ex As Exception
            Console.Write("there is somthing wrong with either your midi file or it's path")
        End Try

        midiFile.Seek(startIndex, System.IO.SeekOrigin.Begin)

        For i = 0 To 7 Step 1
            Data(i) = midiFile.ReadByte()
        Next

        Array.Copy(Data, 0, trackHeader, 0, 4)
        Array.Copy(Data, 4, numBytes, 0, 4) : Array.Reverse(numBytes)

        If Not (Encoding.ASCII.GetString(trackHeader) = "MTrk") Then
            Console.Write("you midi file constins tracks that are not properly formatted")
        Else
            ' read the track
            trackLength = BitConverter.ToInt32(numBytes, 0)
        End If

        Console.Write("track length: " & trackLength & vbNewLine)

        ReDim trackBytes(trackLength - 1)

        For i = 0 To (trackLength - 1) Step 1
            trackBytes(i) = midiFile.ReadByte()
        Next

        'Console.Write(BitConverter.ToString(trackBytes) & vbNewLine)

        'Console.Write(midiFile.Position & vbNewLine)

        'Console.Write(BitConverter.ToString(trackBytes) & vbNewLine)
        If currentTrack = 1 Then readTempos(trackBytes)
        If currentTrack = 2 Then readTiming(trackBytes)

        nextTrackInd = midiFile.Position

        midiFile.Close()
        midiFile.Dispose()

    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------- Read a tempo information -------------------------'
    '--------------------------------------------------------------------------------'
    ' Most frets on fire songs contain two tracks. The first track just contains meta
    ' information (tempo, time signiture, etc). The only pice of meta information that
    ' I care about is the tempo. Unfortunately, the tempo is not allways constant 
    ' throughout the entire song. As such, to get the correct note times I have to keep
    ' track of the freaking tempo. This function reads an array of bytes taken from the
    ' first track and creates an nx2 array of tempos. The first col in the array 
    ' contains the tickNum at which the tempo was set, and the second col contains the
    ' new tempo
    '
    ' note that rather than soting the absolute time for every event, midi files store
    ' the relative timing since the last event. Although this saves memory, It's a 
    ' royal pain for me because I have to pay attention to the timing of events I don't
    ' care about. A such, I will be storing absolute time. 
    '--------------------------------------------------------------------------------'
    Private Sub readTempos(ByRef trackDat() As Byte)

        Dim tempoSets = 0
        For i = 0 To (trackDat.Length - 1) Step 1
            If (trackDat(i) = 255) Then             ' signals the start of a meta event
                If (trackDat(i + 1) = 81) Then      ' indicates that the meta event is a tempo change
                    tempoSets = tempoSets + 1
                End If
            End If
        Next i

        ReDim tempos(tempoSets - 1, 1)
        Dim tempoCount = 0

        Dim State = 0
        Dim absTick As Integer = 0  ' absolute timing
        Dim relTick As Integer = 0  ' relative timing since last event

        Dim msgL As Integer = 0
        Dim pauseCount As Integer = 0

        ' unfortunately, I can't just ignore the events that I'm not interested in because the only way to know 
        ' which bytes contain timing information is to keep track of all events. Midi is lame.

        For i = 0 To (trackDat.Length - 1) Step 1
            Select Case State
                Case 0 ' look for time
                    If (trackDat(i) > 128) Then
                        relTick = relTick * 2 ^ 7 + (trackDat(i) - 128) * 2 ^ 7
                    Else
                        relTick = relTick + trackDat(i)
                        absTick += relTick
                        State = 1
                    End If
                    Exit Select
                Case 1 ' look for the start of a meta event
                    If (trackDat(i) = 255) Then State = 2
                    Exit Select
                Case 2 ' get the new tempo if the event is a tempo change otherwise, stall
                    If (trackDat(i) = 81) Then ' tempo change event
                        tempos(tempoCount, 0) = absTick
                        tempos(tempoCount, 1) = CInt(trackDat(i + 2) * 16 ^ 4 + trackDat(i + 3) * 16 ^ 2 + trackDat(i + 4))
                        tempoCount += 1
                        'If tempoCount >= 2 Then
                        '    Console.Write("new tempo: " & (tempos(tempoCount - 1, 0) * CDbl(tempos(tempoCount - 2, 1))) / (CDbl(deltaTime) * 10 ^ 6) & vbTab & (60 * 10 ^ 6) / tempos(tempoCount - 1, 1) & vbNewLine)
                        'Else
                        '    Console.Write("new tempo: " & (tempos(tempoCount - 1, 0) * CDbl(tempos(tempoCount - 1, 1))) / (CDbl(deltaTime) * 10 ^ 6) & vbTab & (60 * 10 ^ 6) / tempos(tempoCount - 1, 1) & vbNewLine)
                        'End If

                    ElseIf trackDat(i) = 47 Then
                        Exit For : Exit Select
                    End If
                    State = 3
                    msgL = CInt(trackDat(i + 1)) + 1
                    Exit Select
                Case 3 ' stall until the message is over
                    pauseCount += 1
                    If (pauseCount >= msgL) Then
                        State = 0
                        pauseCount = 0
                        relTick = 0
                        msgL = 0
                    End If
                    Exit Select
            End Select
        Next i

    End Sub


    '--------------------------------------------------------------------------------'
    '----------------------------- Read a Note timing -------------------------------'
    '--------------------------------------------------------------------------------'
    ' Assuming that we are delaing with a 2 track song, the note timing information 
    ' will be contained within the second track. the second track will contain both
    ' meta events and command events. I only care about the command events.
    ' spcifically, I only care about note on and note off events. The purpose of this 
    ' function is to propogate an n*3 array with note number, start time, and stop 
    ' time data.
    '--------------------------------------------------------------------------------'
    Private Sub readTiming(ByRef trackDat() As Byte)
        '--------------------------------------------------------------------------------'
        ' ok, the first thing that I need to do is find out how many notes I have
        ' the code for a note on event is 144 and the code for a note off event is 128
        ' However, I can't just assume that all bytes that = 144 or 128 are note events
        ' they could just as well be carrying timing info. However, all notes events will
        ' be followed by at least two bytes with MSBs of 0 (e.g. they will be < 128 in 
        ' decimal). This will necer be true for timing data.
        '
        ' note that I am assuiming that we are using channel 0 for all note events 
        ' ( 144 and 128 are the codes for note on/off for channel 0 only)
        '
        ' note on/off events can affect more than one note at a time :(
        ' When we see a note event, we need to check how many notes it is affecting.
        '
        ' again, the timing recorded in the midi files is relative timing. I want to store 
        ' absolute timing (in ticks)
        '--------------------------------------------------------------------------------'
        Dim numNoteOns As Integer = 0
        Dim numNoteOffs As Integer = 0

        Dim noteOn As Boolean = False
        Dim noteOff As Boolean = False


        Dim oxCount As Integer = 0

        ' this loop counts the number of freaking notes we have
        For i = 0 To (trackDat.Length - 1) Step 1
            ' note on and off events will never be the last bytes in the track -  there has to be a track off event
            If ((i + 2) < trackDat.Length - 1) Then
                If ((trackDat(i) = 144) And (trackDat(i + 1) < 128) And (trackDat(i + 2) < 128)) Then
                    noteOn = True
                ElseIf ((trackDat(i) = 128) And (trackDat(i + 1) < 128) And (trackDat(i + 2) < 128)) Then
                    noteOff = True
                End If
            End If

            ' now count the number of 0X bits we see before the next 1X
            ' except that there's one more tiny catch - we can't assume that all messages will start with a 1X
            ' If the deltaT value for the message is small, the we could end up with 1 extra 0X byte in our count
            If (noteOn And trackDat(i) <> 144) Then
                If (trackDat(i) < 128) Then
                    oxCount += 1
                Else
                    If (oxCount > 2) Then
                        If Not (oxCount Mod 2) Then oxCount -= 1
                        numNoteOns += (oxCount + 1) / 3  ' this will give the number of notes within the note on message
                    Else
                        numNoteOns += 1
                    End If
                    oxCount = 0
                    noteOn = False
                End If

                If (trackDat(i + 1) = 144 And noteOn) Then
                    If (oxCount > 2) Then
                        If Not (oxCount Mod 2) Then oxCount -= 1
                        numNoteOns += (oxCount + 1) / 3  ' this will give the number of notes within the note on message
                    Else
                        numNoteOns += 1
                    End If
                    oxCount = 0
                    noteOn = False
                End If

            End If

            ' now count the number of 0X bits we see before the next 1X
            If (noteOff And trackDat(i) <> 128) Then
                If (trackDat(i) < 128) Then
                    oxCount += 1
                Else
                    If (oxCount > 2) Then
                        If Not (oxCount Mod 2) Then oxCount -= 1
                        numNoteOffs += (oxCount + 1) / 3  ' this will give the number of notes within the note on message
                    Else
                        numNoteOffs += 1
                    End If
                    oxCount = 0
                    noteOff = False
                End If

                If (trackDat(i + 1) = 128 And noteOff) Then
                    If (oxCount > 2) Then
                        If Not (oxCount Mod 2) Then oxCount -= 1
                        numNoteOffs += (oxCount + 1) / 3  ' this will give the number of notes within the note on message
                    Else
                        numNoteOffs += 1
                    End If
                    oxCount = 0
                    noteOff = False
                End If

            End If

        Next i

        If (numNoteOns <> numNoteOffs) Then Console.Write("we have experienced note count failage. The number of note ons is : " & numNoteOns & " and the number of note offs is" & numNoteOffs & vbNewLine)

        totalNoteCount = numNoteOns

        ReDim notes(numNoteOns - 1, 2)

        ' Ok, I now know hoaw many notes to expect. Now I can actually read the file
        ' I know that this is a mess, but I don't see much that I can do about it. 
        ' It is what it is - a mess.

        Dim State = 0
        Dim relTick = 0
        Dim absTick = 0
        Dim msgL = 0
        Dim pauseCount As Integer = 0
        Dim noteOnCount = 0
        Dim noteOffCount = 0

        For i = 0 To (trackDat.Length - 1) Step 1
            If ((i + 1) <= (trackDat.Length - 1)) Then   ' It's ok to do this because the last bit is worthless junk from ther end track meta event
                Select Case State
                    Case 0  'timing
                        If (trackDat(i) >= 128) Then
                            relTick = relTick * 2 ^ 7 + (trackDat(i) - 128) * 2 ^ 7
                        Else
                            relTick = relTick + trackDat(i)
                            absTick += relTick
                            If trackDat(i + 1) = 255 Then : State = 6 : Else : State = 2 : End If
                        End If
                        Exit Select
                    Case 2  'control event
                        If trackDat(i) = 144 Then ' it's a note on event
                            'Console.Write("note On at Time: " & absTick & vbNewLine)
                            State = 3
                        ElseIf trackDat(i) = 128 Then  ' it's a note off event
                            'Console.Write("note Off at Time: " & absTick & vbNewLine)
                            State = 4
                        Else ' I don't really care what it is - stall
                            State = 5
                        End If
                        Exit Select
                    Case 3  'not on response
                        If (trackDat(i - 1) = 144) Or (trackDat(i - 1) = 0) Then ' this is the first value
                            notes(noteOnCount, 0) = CInt(trackDat(i)) ' the noteNum
                            notes(noteOnCount, 1) = absTick ' the absolute time (in ticks when the note is to be turned on)
                            'Console.Write("new note: " & notes(noteOnCount, 0) & vbTab & notes(noteOnCount, 1) & vbNewLine)
                            noteOnCount += 1
                        End If

                        If (trackDat(i) < 128) And (trackDat(i + 1) < 128) Then
                            ' stall
                            ' note: this is safe, because there is not case in which we can be in state 4 and i+4 will be beyond our bounds - that I can think of
                            If (trackDat(i + 2) >= 128) And (trackDat(i + 3) < 128) And (trackDat(i + 4) < 128) Then
                                relTick = 0
                                State = 0
                            End If
                        Else
                            State = 0 ' go back to looking for a timing event
                            relTick = 0
                        End If

                        Exit Select
                    Case 4  'note off resposne
                        If (trackDat(i - 1) = 128) Or (trackDat(i - 1) = 0) Then ' this is the first value
                            If (notes(noteOffCount, 0) <> CInt(trackDat(i))) Then Console.Write("note on - note off missalignment " & vbNewLine) ' the noteNum
                            notes(noteOffCount, 2) = absTick ' the absolute time (in ticks when the note is to be turned off)
                            noteOffCount += 1
                        End If

                        If (trackDat(i) < 128) And (trackDat(i + 1) < 128) Then
                            ' stall
                            ' note: this is safe, because there is not case in which we can be in state 4 and i+4 will be beyond our bounds - that I can think of
                            If (trackDat(i + 2) >= 128) And (trackDat(i + 3) < 128) And (trackDat(i + 4) < 128) Then
                                relTick = 0
                                State = 0
                            End If
                        Else
                            State = 0 ' go back to looking for a timing event
                            relTick = 0
                        End If

                        Exit Select
                    Case 5  'control stall
                        If (trackDat(i) < 128) And (trackDat(i + 1) < 128) Then
                            ' stall
                            ' note: this is safe, because there is not case in which we can be in state 4 and i+4 will be beyond our bounds - that I can think of
                            If (trackDat(i + 2) >= 128) And (trackDat(i + 3) < 128) And (trackDat(i + 4) < 128) Then
                                relTick = 0
                                State = 0
                            End If
                        Else
                            State = 0 ' go back to looking for a timing event
                            relTick = 0
                        End If
                        Exit Select
                    Case 6  'meta event
                        State = 7
                        Exit Select
                    Case 7  'type of meta event
                        If (trackDat(i) = 47) Then Exit For ' the only meta event that I really care about is the exit track event
                        msgL = CInt(trackDat(i + 1)) + 1    ' for other events I'll just wait them out -  thi tells me how long to wait
                        State = 8
                        Exit Select
                    Case 8  'meta stall
                        pauseCount += 1
                        If (pauseCount >= msgL) Then
                            State = 0
                            pauseCount = 0
                            relTick = 0
                            msgL = 0
                        End If
                        Exit Select
                End Select
            End If
        Next i

        ' It is finally over - how lame is that mess ^ ?
        ' quick check
        Console.Write("the total number of notes is: " & totalNoteCount & " and the number of noteOns was: " & noteOnCount & vbNewLine)

    End Sub


    '--------------------------------------------------------------------------------'
    '--------------------------- sort notes into frets ------------------------------'
    '--------------------------------------------------------------------------------'
    ' C coresponds to the first fret, c# to the second, D to the third, D# to the 
    ' fourth, and E to the 5th. SuperEasy notes are contained in the 5th octave, 
    ' easy notes are stored in the 6th octave, and so on. This function sorts notes
    ' a vector based on the note val passed into the function. It also calculates the
    ' actual time at which the note should be played (note the time in ticks) time is in miliseconds
    Public Sub sortNotes(ByVal note As Integer, ByRef fretDat As Fret)
        ' first count the number of times that the note comes up
        Dim noteCount As Integer = 0

        For i = 0 To (notes.GetLength(0) - 1)
            If notes(i, 0) = note Then noteCount += 1 ' gets the number of notes on each string
        Next

        ' now sort and convert ticks to time
        fretDat.noteCount = noteCount

        ' this loop calculates the position of each note in absolute time
        If noteCount > 0 Then
            ReDim fretDat.onTimes(noteCount - 1)
            ReDim fretDat.offTimes(noteCount - 1)
            noteCount = 0

            ' this loop singles out only the notes on the specified string 
            For i = 0 To (notes.GetLength(0) - 1) Step 1
                If notes(i, 0) = note Then
                    fretDat.onTimes(noteCount) = noteOnTrueTimes(i)
                    fretDat.offTimes(noteCount) = noteOffTrueTimes(i)
                    'Console.Write("note: " & vbTab & noteCount & vbTab & fretDat.onTimes(noteCount) & vbNewLine)
                    'Console.Write("note: " & vbTab & noteCount & vbTab & fretDat.onTimes(noteCount) / 1000 & vbTab & (60 * 10 ^ 6) / CDbl(tempos(currentTempo, 1)) & vbNewLine)
                    'Console.Write("note: " & vbTab & noteCount & vbTab & notes(i, 1) & vbTab & fretDat.onTimes(noteCount) & vbNewLine)
                    noteCount += 1
                End If
            Next i

        End If


    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------- sort notes by difficulty -----------------------------'
    '--------------------------------------------------------------------------------'
    ' group together all notes of the specified difficulty.
    Public Sub sortByDifficulty(ByVal note As Integer, ByRef noteTimes(,) As Double)
        ' first count the number of times that the note comes up
        Dim noteCount As Integer = 0

        For i = 0 To (notes.GetLength(0) - 1)
            If ((notes(i, 0) >= note) And ((notes(i, 0) <= (note + 4)))) Then noteCount += 1 ' gets the number of notes in the specified difficulty level
        Next

        ' this loop calculates the position of each note in absolute time
        If noteCount > 0 Then
            ReDim noteTimes(noteCount - 1, 2)
            noteCount = 0

            ' this loop singles out only the notes on the specified string 
            For i = 0 To (notes.GetLength(0) - 1) Step 1
                If ((notes(i, 0) >= note) And ((notes(i, 0) <= (note + 4)))) Then
                    noteTimes(noteCount, 0) = noteOnTrueTimes(i)
                    noteTimes(noteCount, 1) = noteOffTrueTimes(i)
                    noteTimes(noteCount, 2) = notes(i, 0) - note
                    noteCount += 1
                End If
            Next i
        End If


    End Sub

    '--------------------------------------------------------------------------------'
    '-------------------------- calculate true note times ---------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub getTrueNoteTimes()
        Dim tempo As Double
        ReDim noteOffTrueTimes(notes.GetLength(0) - 1)
        ReDim noteOnTrueTimes(notes.GetLength(0) - 1)

        ' the next two loops calcluate the true on and off times for each note in miliseconds        
        noteOnTrueTimes(0) = 0
        For ii = 0 To (notes(0, 1) - 1) Step 1
            'find the correct tempo for our current tick
            For iii = 0 To (tempos.GetLength(0) - 1) Step 1
                If ii >= tempos(iii, 0) Then tempo = CDbl(tempos(iii, 1))
            Next iii
            noteOnTrueTimes(0) += tempo / (CDbl(deltaTime) * 10 ^ 3)
        Next ii
        'Console.Write("note true time: " & noteOnTrueTimes(0) / 10 ^ 3 & vbNewLine)
        For i = 1 To (notes.GetLength(0) - 1) Step 1
            noteOnTrueTimes(i) = 0
            For ii = notes(i - 1, 1) To (notes(i, 1) - 1) Step 1
                'find the correct tempo for our current tick
                For iii = 0 To (tempos.GetLength(0) - 1) Step 1
                    If ii >= tempos(iii, 0) Then tempo = CDbl(tempos(iii, 1))
                Next iii
                noteOnTrueTimes(i) += tempo / (CDbl(deltaTime) * 10 ^ 3)
            Next ii
            noteOnTrueTimes(i) += noteOnTrueTimes(i - 1)
            'Console.Write("note true time: " & noteOnTrueTimes(i) / 10 ^ 3 & vbNewLine)
        Next

        ' now do the same thing for the note offs

        noteOffTrueTimes(0) = 0
        For ii = 0 To (notes(0, 1) - 1) Step 1
            'find the correct tempo for our current tick
            For iii = 0 To (tempos.GetLength(0) - 1) Step 1
                If ii >= tempos(iii, 0) Then tempo = CDbl(tempos(iii, 1))
            Next iii
            noteOffTrueTimes(0) += tempo / (CDbl(deltaTime) * 10 ^ 3)
        Next ii

        For i = 1 To (notes.GetLength(0) - 1) Step 1
            noteOffTrueTimes(i) = 0
            For ii = notes(i - 1, 1) To (notes(i, 1) - 1) Step 1
                'find the correct tempo for our current tick
                For iii = 0 To (tempos.GetLength(0) - 1) Step 1
                    If ii >= tempos(iii, 0) Then tempo = CDbl(tempos(iii, 1))
                Next iii
                noteOffTrueTimes(i) += tempo / (CDbl(deltaTime) * 10 ^ 3)
            Next ii
            noteOffTrueTimes(i) += noteOnTrueTimes(i - 1)
            'Console.Write("note true time: " & noteOffTrueTimes(i) / 10 ^ 3 & vbNewLine)
        Next i

    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------------- free it up ---------------------------------'
    '--------------------------------------------------------------------------------'
    Protected Overrides Sub Finalize()
        Console.Write("death to MIDI! " & vbNewLine)
        midiFile.Dispose()
        MyBase.Finalize()
    End Sub


End Class

