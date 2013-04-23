Imports OpenTK
Imports OpenTK.Audio
Imports System.IO
Imports OpenTK.Audio.OpenAL

Imports irrklang

Public Class Song
    '--------------------------------------------------------------------------------'
    '--------------------------- variable declarations ------------------------------'
    '--------------------------------------------------------------------------------'
    Public superEasy(4) As Fret
    Public easy(4) As Fret
    Public medium(4) As Fret
    Public amazing(4) As Fret

    Public superEasyAll(0, 2) As Double
    Public easyAll(0, 2) As Double
    Public mediumAll(0, 2) As Double
    Public amazingAll(0, 2) As Double

    Public diffsExist(3) As Boolean

    Public songPath As String
    Public name As String

    Public songFile As StreamWriter

    'Public player As OggPlayer
    'Public audFile As OggFile

    Private irrKlangEngine As New ISoundEngine()
    Public player As ISound
    Private vol As Single = 0.75

    Public noteReader As MidiReader

    '--------------------------------------------------------------------------------'
    '---------------------------------- constructor ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New(ByVal fileName As String)
        songPath = GAMEPATH & "songs\" & fileName & "\"
        name = fileName
        'songPath = filePath

        noteReader = New MidiReader(songPath & "notes.mid")
        loadNoteData()
        songFile = New StreamWriter(GAMEPATH & "songs\noteFiles\" & name & ".txt")
        writeSongFile()
        songFile.Close()

        'noteReader.Finalize()

    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------  create audio player ------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub createAudioPlayer()
        If System.IO.File.Exists(songPath & "guitar.ogg") Then
            If player IsNot Nothing Then
                player.Stop()
                player.Dispose()
            End If

            player = irrKlangEngine.Play2D(songPath & "guitar.ogg", False, False, StreamMode.AutoDetect, True)
            player.Paused = True
            player.Volume = vol
        End If
    End Sub
    'Public Sub createAudioPlayer()
    '    If System.IO.File.Exists(songPath & "guitar.ogg") Then
    '        player = New OggPlayerVBN()
    '        audFile = New OggFile(songPath & "guitar.ogg")
    '        player.SetCurrentFile(audFile)
    '    End If
    'End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------------- pause song ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub pauseSong()
        If player IsNot Nothing Then
            player.Paused = Not player.Paused
        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------------------ stop song ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub stopSong()
        If player IsNot Nothing Then
            player.Stop()
        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------  load note data from midi  ----------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub loadNoteData()
        If noteReader.dataLoaded Then
            Dim noteNum As Integer = 72
            noteReader.sortByDifficulty(noteNum, superEasyAll)
            For i = 0 To (superEasy.Length - 1) Step 1
                superEasy(i) = New Fret()
                noteReader.sortNotes(noteNum, superEasy(i))
                noteNum += 1
            Next i

            noteNum = 84
            noteReader.sortByDifficulty(noteNum, easyAll)
            For i = 0 To (easy.Length - 1) Step 1
                easy(i) = New Fret()
                noteReader.sortNotes(noteNum, easy(i))
                noteNum += 1
            Next i

            noteNum = 96
            noteReader.sortByDifficulty(noteNum, mediumAll)
            For i = 0 To (medium.Length - 1) Step 1
                medium(i) = New Fret()
                noteReader.sortNotes(noteNum, medium(i))
                noteNum += 1
            Next i

            noteNum = 108
            noteReader.sortByDifficulty(noteNum, amazingAll)
            For i = 0 To (amazing.Length - 1) Step 1
                amazing(i) = New Fret()
                noteReader.sortNotes(noteNum, amazing(i))
                noteNum += 1
            Next i

        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------------  write song file  -------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub writeSongFile()
        songFile.WriteLine("superEasy")
        For i = 0 To (superEasyAll.GetLength(0) - 1) Step 1
            songFile.WriteLine(CStr(superEasyAll(i, 0)) & vbTab & CStr(superEasyAll(i, 1)) & vbTab & CStr(superEasyAll(i, 2)))
        Next i

        songFile.WriteLine("easy")
        For i = 0 To (easyAll.GetLength(0) - 1) Step 1
            songFile.WriteLine(CStr(easyAll(i, 0)) & vbTab & CStr(easyAll(i, 1)) & vbTab & CStr(easyAll(i, 2)))
        Next i

        songFile.WriteLine("medium")
        For i = 0 To (mediumAll.GetLength(0) - 1) Step 1
            songFile.WriteLine(CStr(mediumAll(i, 0)) & vbTab & CStr(mediumAll(i, 1)) & vbTab & CStr(mediumAll(i, 2)))
        Next i

        songFile.WriteLine("amazing")
        For i = 0 To (amazingAll.GetLength(0) - 1) Step 1
            songFile.WriteLine(CStr(amazingAll(i, 0)) & vbTab & CStr(amazingAll(i, 1)) & vbTab & CStr(amazingAll(i, 2)))
        Next i

    End Sub

    '--------------------------------------------------------------------------------'
    '-----------------------  read field configuration file  ------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub readFieldFile(ByVal difficulty As Integer, ByRef fieldVec() As Boolean)
        Dim fieldFile As StreamReader
        Dim numNotes As Integer = 0
        Dim fileExists As Boolean = False
        Select Case difficulty
            Case 0
                If System.IO.File.Exists(GAMEPATH & "songs\" & name & "\field_easy.txt") Then
                    fieldFile = New StreamReader(GAMEPATH & "songs\" & name & "\field_easy.txt")
                    fileExists = True
                End If
            Case 1
                If System.IO.File.Exists(GAMEPATH & "songs\" & name & "\field_medium.txt") Then
                    fieldFile = New StreamReader(GAMEPATH & "songs\" & name & "\field_medium.txt")
                    fileExists = True
                End If

            Case 2
                If System.IO.File.Exists(GAMEPATH & "songs\" & name & "\field_hard.txt") Then
                    fieldFile = New StreamReader(GAMEPATH & "songs\" & name & "\field_hard.txt")
                    fileExists = True
                End If

            Case Else
                If System.IO.File.Exists(GAMEPATH & "songs\" & name & "\field_general.txt") Then
                    fieldFile = New StreamReader(GAMEPATH & "songs\" & name & "\field_general.txt")
                    fileExists = True
                End If

        End Select

        If fileExists Then
            numNotes = fieldFile.ReadLine()
            ReDim fieldVec(numNotes)
            For i As Integer = 0 To (numNotes - 1) Step 1
                fieldVec(i) = Convert.ToBoolean(fieldFile.ReadLine)
            Next
        End If
    End Sub

End Class


'--------------------------------------------------------------------------------'
'--------------------- Fret class for storing note information ------------------'
'--------------------------------------------------------------------------------'
' sorry for cramming two classes into one file, but I really didn't want to 
' devote a whole file to this tiny little class
Public Class Fret
    Public noteCount As Integer
    Public onTimes() As Double
    Public offTimes() As Double

    Public Sub New()
        noteCount = 0
        ReDim onTimes(0)
        ReDim offTimes(0)
    End Sub

End Class