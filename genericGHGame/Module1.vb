Imports System
Imports System.Windows.Forms

Module Module1

    'Public GAMEPATH As String = "C:\ROBOTIC LAB\Hand Rehabilitation Robot\expo2012\gameWindowExp\"
    Public GAMEPATH As String = Application.StartupPath
    Public positions() As Double = {2.25, 1.15, 0.0, -1.15, -2.25}
    Public FPS As Double = 60
    Public diagnostic As Boolean = False

    Public currentSub As Subject
    Public currentSong As Song
    Public moreNoise As NoiseMaker
    Public trialStr As String = ""
    Public noteRate As Integer = 30
    Public targRate As Integer = 30

    Public menu As Menu

    Sub Main()
        GAMEPATH = GAMEPATH.Substring(0, GAMEPATH.LastIndexOf("\"))
        GAMEPATH = GAMEPATH.Substring(0, GAMEPATH.LastIndexOf("\") + 1)
        makeAbsentDirectories()

        Application.EnableVisualStyles()
        menu = New Menu
        Application.Run(menu)

    End Sub

    ''------------------------------- make absent directories ---------------------------''
    '' HG will not track empty directories, and we don't want it to track folders full of
    '' data. As such, we need to make our data directories if they do not exist
    Sub makeAbsentDirectories()
        'subjects
        If (Not System.IO.Directory.Exists(GAMEPATH & "Subjects")) Then
            System.IO.Directory.CreateDirectory(GAMEPATH & "Subjects")
            Console.WriteLine("made subjects dir")
        End If
        If (Not System.IO.File.Exists(GAMEPATH & "Subjects\" & "allSubjects.txt")) Then
            Dim allSubjectsFile As New System.IO.StreamWriter(GAMEPATH & "Subjects\" & "allSubjects.txt")
            allSubjectsFile.WriteLine("default")
            allSubjectsFile.Close()
        End If

    End Sub

End Module
