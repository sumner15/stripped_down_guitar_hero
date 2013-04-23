Imports System
Imports System.Windows.Forms

Module Module1

    'Public GAMEPATH As String = "C:\ROBOTIC LAB\Hand Rehabilitation Robot\expo2012\gameWindowExp\"
    Public GAMEPATH As String = Application.StartupPath
    Public positions() As Double = {2.25, 1.15, 0.0, -1.15, -2.25}
    Public FPS As Double = 200
    Public diagnostic As Boolean = False

    Public currentSub As Subject
    Public currentSong As Song
    Public moreNoise As NoiseMaker
    Public trialStr As String = ""

    Public menu As Menu

    Sub Main()
        GAMEPATH = GAMEPATH.Substring(0, GAMEPATH.LastIndexOf("\"))
        GAMEPATH = GAMEPATH.Substring(0, GAMEPATH.LastIndexOf("\") + 1)

        Application.EnableVisualStyles()
        menu = New Menu
        Application.Run(menu)

    End Sub

End Module
