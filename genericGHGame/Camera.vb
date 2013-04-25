'testing version control 4/25
'----------------------------------------------------------------------------------'
'---------- this class defines a camera - this is what shows our graphics ---------'
'----------------------------------------------------------------------------------'
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL

Public Class Camera

    Private lookAt(2) As Single
    Public camPos(2) As Single

    Public pitch As Single
    Public roll As Single
    Public yaw As Single

    '----------------------------------------------------------------------------------'
    '--------------------------------- default constructor ----------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub New()
        lookAt = {0.0F, 0.0F, 0.0F}
        camPos = {0.0F, 5.0F, 5.0F}
        pitch = 0
        roll = 0
        yaw = 0
        setViewPoint()
    End Sub

    '----------------------------------------------------------------------------------'
    '-------------------------------- alternate constructor ---------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub New(ByVal cam() As Single, ByVal target() As Single)
        lookAt = target
        camPos = cam
        pitch = 0
        roll = 0
        yaw = 0
        setViewPoint()
    End Sub

    '----------------------------------------------------------------------------------'
    '--------------------- defines the perspective for our camera ---------------------'
    '----------------------------------------------------------------------------------'
    Public Sub ViewPerspective(ByVal Width, ByVal Height) ' Set Up A Perspective View
        GL.MatrixMode(MatrixMode.Projection) ' Select Projection
        GL.LoadIdentity()
        Dim perspective1 As Matrix4 = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _
                                             CSng((Width) / (Height)), 1, 640)
        GL.LoadMatrix(perspective1)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    '----------------------------------------------------------------------------------'
    '-------------------- controls the orientation of the camera ----------------------'
    '----------------------------------------------------------------------------------'
    Public Sub setViewPoint()
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        Dim view_ = OpenTK.Matrix4.LookAt(camPos(0), camPos(1), camPos(2), lookAt(0), lookAt(1), lookAt(2), 0, 1, 0) ' Lookat(camPos,targetPos,upVector)

        GL.MatrixMode(MatrixMode.Modelview)
        'GL.PushMatrix()
        GL.LoadMatrix(view_)
        GL.Rotate(pitch, 0.0, 1.0, 0.0)
        GL.Rotate(roll, 1.0, 0.0, 0.0)
        'GL.PopMatrix()

    End Sub

    '----------------------------------------------------------------------------------'
    '-------------------------------- set camera pos ----------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub setCamPos(ByRef pos() As Single)
        camPos = pos
    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------- set camera target --------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub setLookAt(ByRef pos() As Single)
        lookAt = pos
    End Sub


End Class
