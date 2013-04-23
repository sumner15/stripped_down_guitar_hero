'----------------------------------------------------------------------------------'
'------------------------------ guitar string class -------------------------------'
'----------------------------------------------------------------------------------'
' this class describes  a guitar string. it is responsible for drawing the notes 
' for the string at the correct time and registering and recording hits.
Imports System.Math
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL

Public Class GuitarString
    Public noteTimes() As Double
    Public hitTimes(0, 1) As Double  ' first column indicates whether or not the note was hit, the second column gives the time at which it was hit
    Public nextNote As Integer
    Public farNote As Integer
    Public previousNote As Integer = 0
    Private hitLast As Boolean = False
    Private winSizeS As Double = 5000  ' how early the object appears in miliseconds
    Private winSizeU As Double = 18    ' how far away the object is when it appears
    Public xPos As Double

    Private hitWin As Double = 200   ' range in which hits are counted as sucessful - note: changed from 400 on oct 12, 2012

    Public Note As New MeshVbo(poly.TRIS)

#Region "constructorz"
    Public Sub New()
        noteTimes = {5000, 10000, 15000, 20000, 22500, 25000, 17500, 30000}
        ReDim hitTimes(noteTimes.Length - 1, 1)
        xPos = 1.45
        Note.readWavefront("note3.obj")
        Note.loadTexture("note1.bmp")
        Note.loadVbo()
    End Sub

    Public Sub New(ByRef noteT() As Double)
        noteTimes = noteT
        ReDim hitTimes(noteTimes.Length - 1, 1)
        xPos = 1.45
        Note.readWavefront("note3.obj")
        Note.loadTexture("note1.bmp")
        Note.loadVbo()
    End Sub

    Public Sub New(ByVal noteT() As Double, ByVal strNum As Integer)
        noteTimes = noteT
        ReDim hitTimes(noteTimes.Length - 1, 1)
        If (strNum < 5) Then
            xPos = positions(strNum)
        Else
            Console.Write("invalid string number: " & strNum & vbNewLine)
            'ourWindow.Exit()
        End If
        Note.readWavefront("note3.obj")
        Select Case strNum
            Case 0 : Note.loadTexture("note1.bmp") : Exit Select
            Case 1 : Note.loadTexture("note2.bmp") : Exit Select
            Case 2 : Note.loadTexture("note3.bmp") : Exit Select
            Case 3 : Note.loadTexture("note4.bmp") : Exit Select
            Case 4 : Note.loadTexture("note5.bmp") : Exit Select
        End Select

        Note.loadVbo()
    End Sub
#End Region

    Public Sub freeMemory()
        Note.freeBuffers()
        GL.DeleteTextures(TextureTarget.Texture2D, Note.textureID)
    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------ draw upcoming notes -------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub drawNotes(ByVal gameTime As Double)
        ' find the first note that is more than N seconds away
        Dim farNote As Integer

        For i = nextNote To (noteTimes.Length - 1) Step 1
            If ((noteTimes(i) - gameTime) > 5000) Then
                farNote = i - 1
                Exit For
            Else
                farNote = noteTimes.Length - 1
            End If
        Next i

        Dim showNotes(farNote - nextNote) As Integer
        Dim notePos As Double

        'now we can actually draw the upcoming notes
        GL.Enable(EnableCap.Texture2D)
        GL.BindTexture(TextureTarget.Texture2D, Note.textureID)
        For i = nextNote To farNote Step 1
            notePos = (noteTimes(i) - gameTime) * winSizeU / winSizeS
            GL.PushMatrix()
            GL.Translate(xPos, 0.125, -notePos)
            Note.drawVbo()
            GL.PopMatrix()
            ' we want to draw the previous note if it was missed, but not if it was hit.
            If Not hitLast Then
                notePos = (noteTimes(previousNote) - gameTime) * winSizeU / winSizeS
                GL.PushMatrix()
                GL.Translate(xPos, 0.125, -notePos)
                Note.drawVbo()
                GL.PopMatrix()
            End If
        Next i

        ' advance next note and previous note
        If (noteTimes(nextNote) - gameTime < -(hitWin / 2)) And ((nextNote + 1) < (noteTimes.Length - 1)) Then
            previousNote = nextNote
            nextNote = nextNote + 1
        End If

    End Sub

    '----------------------------------------------------------------------------------'
    '--------------------------------- check for hit ----------------------------------'
    '----------------------------------------------------------------------------------'
    Public Function checkHit(ByVal gameTime As Double) As Boolean
        If Abs(noteTimes(nextNote) - gameTime) < hitWin / 2 Then
            hitTimes(nextNote, 0) = 1.0
            hitTimes(nextNote, 1) = noteTimes(nextNote) - gameTime
            hitLast = True
            Return True
        Else
            hitTimes(nextNote, 0) = 0.0
            hitTimes(nextNote, 1) = noteTimes(nextNote) - gameTime
            hitLast = False
            Return False
        End If
    End Function

    ' an overload to check provide the user with feedback
    Public Function checkHit(ByVal gameTime As Double, ByRef feedbackTip As String) As Boolean
        If Abs(noteTimes(nextNote) - gameTime) < hitWin / 2 Then
            hitTimes(nextNote, 0) = 1.0
            hitTimes(nextNote, 1) = noteTimes(nextNote) - gameTime
            hitLast = True
            Return True
        Else
            hitTimes(nextNote, 0) = 0.0
            hitTimes(nextNote, 1) = noteTimes(nextNote) - gameTime

            If gameTime > noteTimes(nextNote) Then
                feedbackTip = feedbackTip & "too late "
            Else
                feedbackTip = feedbackTip & "too early "
            End If

            hitLast = False
            Return False
        End If
    End Function

End Class
