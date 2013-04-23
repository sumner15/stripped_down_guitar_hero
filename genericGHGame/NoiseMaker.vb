Imports irrklang
Imports System.Random
Public Class NoiseMaker
    Private irrKlangEngine As New ISoundEngine()
    Public badNote1 As ISound
    Private badNote2 As ISound
    Private noisePath As String
    Private vol As Single = 0.4
    Private rangen As Random

    '--------------------------------------------------------------------------------'
    '----------------------------------- constructor --------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New()
        noisePath = GAMEPATH & "sounds\"
        rangen = New Random(20)

        If System.IO.File.Exists(noisePath & "fiba6.ogg") Then
            If badNote1 IsNot Nothing Then
                badNote1.Stop()
                badNote1.Dispose()
            End If
            'badNote1 = irrKlangEngine.Play2D(noisePath & "fiba6.ogg", False, False, StreamMode.AutoDetect, True)
            'badNote1.Paused = True
            'badNote1.Volume = vol
        End If

        noisePath = GAMEPATH & "sounds\"
        If System.IO.File.Exists(noisePath & "crunch1.ogg") Then
            If badNote2 IsNot Nothing Then
                badNote2.Stop()
                badNote2.Dispose()
            End If
            'badNote2 = irrKlangEngine.Play2D(noisePath & "crunch1.ogg", False, False, StreamMode.AutoDetect, True)
            'badNote2.Paused = True
            'badNote2.Volume = vol
        End If

    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------------- play sound ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub playBadNote()
        Dim randVal As Single = rangen.Next(2)
        If randVal >= 1 Then
            badNote1 = irrKlangEngine.Play2D(noisePath & "fiba6.ogg", False, False, StreamMode.AutoDetect, True)
            badNote2.Paused = False
            badNote2.Volume = vol
        Else
            badNote2 = irrKlangEngine.Play2D(noisePath & "crunch1.ogg", False, False, StreamMode.AutoDetect, True)
            badNote2.Paused = False
            badNote2.Volume = vol
        End If

    End Sub

End Class
