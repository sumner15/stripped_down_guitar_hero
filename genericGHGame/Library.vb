Imports System.IO
'================================================================================'
'--------------------------------- Library class --------------------------------'
'================================================================================'
Public Class Library
    Private songList As String = "allSongs.txt"
    Public libSize As Integer
    Public songs() As Song
    Public songNames() As String

    '--------------------------------------------------------------------------------'
    '--------------------------- constructor for library ----------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New()
        Dim fileReader As StreamReader
        Dim idstring As String
        'need to find out how many subjects we have
        fileReader = My.Computer.FileSystem.OpenTextFileReader(GAMEPATH & "songs\" & songList)
        libSize = 0
        While (Not fileReader.EndOfStream)
            idstring = fileReader.ReadLine()
            If (Not (idstring = "") And Not (idstring = " ")) Then libSize += 1
        End While

        'now we can actually read in the subject's data
        ReDim songs(libSize - 1)
        ReDim songNames(libSize - 1)
        If libSize > 0 Then
            fileReader.Close()
            fileReader = My.Computer.FileSystem.OpenTextFileReader(GAMEPATH & "songs\" & songList)

            For i As Integer = 0 To (libSize - 1) Step 1
                songNames(i) = fileReader.ReadLine()
                songs(i) = New Song(songNames(i))
            Next i

            fileReader.Close()
        End If

    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------- add song to the library ---------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub addSong(ByRef sng As Song)
        Dim idLine = vbNewLine & sng.name
        Dim oldSongs() As Song
        Dim oldsongNames() As String
        My.Computer.FileSystem.WriteAllText(GAMEPATH & "songs\" & songList, idLine, True)

        oldSongs = songs
        oldsongNames = songNames
        ReDim songs(libSize)
        ReDim songNames(libSize)

        For i = 0 To (libSize - 1) Step 1
            songs(i) = oldSongs(i)
            songNames(i) = oldsongNames(i)
        Next i

        libSize += 1

        songs(libSize - 1) = sng
        songNames(libSize - 1) = sng.name
    End Sub


End Class
