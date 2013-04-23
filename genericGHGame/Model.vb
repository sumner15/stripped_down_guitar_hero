'----------------------------------------------------------------------------------'
'--------- this class contains all the instructions for drawing a 3D model --------'
'----------------------------------------------------------------------------------'
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports System.Math
Public Class Model
    Public mesh As MeshVbo
    Public pos(2) As Single
    Public ang(2) As Single
    Public scale(2) As Single
    Public scaleBase As Single
    Private useTexture As Boolean

    Private vibOn As Boolean = False
    Private vibAmp As Single = 0

    Private swellOn As Boolean = False
    Private swellAmp As Single

    Private timer As Stopwatch
    Public useTransparency As Boolean = False
    Private file As String = ""

    Public visable As Boolean = True

    '----------------------------------------------------------------------------------'
    '--------------------------------- default constructor ----------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub New(ByVal fName As String, ByRef modPos() As Single, ByRef modAng() As Single, ByVal modS As Single)
        mesh = New MeshVbo(poly.TRIS)
        mesh.readWavefront(fName & ".obj")
        mesh.loadVbo()

        pos = modPos
        ang = modAng
        scale(0) = modS
        scale(1) = modS
        scale(2) = modS
        useTexture = 0
        file = fName

    End Sub

    Public Sub New(ByVal fName As String, ByVal tName As String, ByRef modPos() As Single, ByRef modAng() As Single, ByVal modS As Single)
        mesh = New MeshVbo(poly.TRIS)
        mesh.readWavefront(fName & ".obj")
        mesh.loadVbo()
        mesh.loadTexture(tName & ".bmp")

        pos = modPos
        ang = modAng
        scale(0) = modS
        scale(1) = modS
        scale(2) = modS
        useTexture = 1
        file = fName
    End Sub

    Public Sub New(ByVal fName As String, ByVal tName As String, ByRef modPos() As Single, ByRef modAng() As Single, ByVal modS() As Single)
        mesh = New MeshVbo(poly.TRIS)
        mesh.readWavefront(fName & ".obj")
        mesh.loadVbo()
        mesh.loadTexture(tName & ".bmp")

        pos = modPos
        ang = modAng
        scale = modS
        useTexture = 1
        file = fName
    End Sub

    Public Sub New(ByVal fName As String, ByRef modPos() As Single, ByRef modAng() As Single, ByVal modS() As Single)
        mesh = New MeshVbo(poly.TRIS)
        mesh.readWavefront(fName & ".obj")
        mesh.loadVbo()

        pos = modPos
        ang = modAng
        scale = modS
        useTexture = 0
        file = fName

    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------------- draw target ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub drawModel()

        If visable Then
            If useTexture Then
                GL.Enable(EnableCap.Texture2D)
                GL.BindTexture(TextureTarget.Texture2D, mesh.textureID)
            Else
                'GL.Disable(EnableCap.Texture2D)
            End If

            If useTransparency Then
                GL.Enable(EnableCap.Blend)
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One)
            End If

            If vibOn Then vibrate(2, 10, vibAmp, 0.2)
            If swellOn Then swell(swellAmp, 0.1)

            GL.PushMatrix()
            GL.Translate(pos(0), pos(1), pos(2))
            GL.Scale(scale(0), scale(1), scale(2))
            GL.Rotate(ang(0), 1, 0, 0)
            GL.Rotate(ang(1), 0, 1, 0)
            GL.Rotate(ang(2), 0, 0, 1)
            mesh.drawVbo()
            GL.PopMatrix()
            GL.Disable(EnableCap.Texture2D)

            If useTransparency Then
                GL.Disable(EnableCap.Blend)
            End If
        End If

    End Sub

#Region "effects"
    '--------------------------------------------------------------------------------'
    '-------------------------------- start vibration -------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub startVibration(ByRef amp As Single)
        If Not vibOn Then
            If timer Is Nothing Then
                timer = New Stopwatch
                timer.Start()
            Else
                timer.Restart()
            End If
            vibOn = True
            vibAmp = amp
        Else

        End If
    End Sub
    '--------------------------------------------------------------------------------'
    '-------------------------------- start vibration -------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub vibrate(ByRef axis As Single, ByRef freq As Single, ByRef amp As Single, ByRef dur As Single)
        Dim t As Single = timer.ElapsedMilliseconds / 1000
        If axis >= 0 And axis <= 2 Then
            ang(axis) += amp * Exp(-t / (dur)) * Sin(2 * 3.14159 * freq * t)
            If t > dur Then
                vibOn = False
                ang(axis) = 0
            End If

        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------------- start swell ---------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub startSwell(ByRef amp As Single)
        If Not swellOn Then
            If timer Is Nothing Then
                timer = New Stopwatch
                timer.Start()
            Else
                timer.Restart()
            End If
            swellOn = True
            scaleBase = scale(0)
            swellAmp = amp
        Else

        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------------- run swell -----------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub swell(ByRef amp As Single, ByRef dur As Single)
        Dim t As Single = timer.ElapsedMilliseconds / 1000
        scale(0) = amp * Sin(2 * 3.14159 * 1 / (dur * 2) * t) + scaleBase
        scale(1) = scale(0)
        scale(2) = scale(0)
        If t > dur Then
            swellOn = False
            scale(0) = scaleBase
            scale(1) = scale(0)
            scale(2) = scale(0)
        End If

    End Sub

#End Region

    '----------------------------------------------------------------------------------'
    '----------------------------------- free memory ----------------------------------'
    '----------------------------------------------------------------------------------'
    ' freesup the memory - cal only when you are done with it.
    Public Sub freeMemory()
        mesh.freeBuffers()

        If useTexture Then
            GL.DeleteTextures(TextureTarget.Texture2D, mesh.textureID)
        End If

    End Sub

End Class
