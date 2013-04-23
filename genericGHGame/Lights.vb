'----------------------------------------------------------------------------------'
'------------------- this class takes care of all of our lights -------------------'
'----------------------------------------------------------------------------------'
' later you should add somthing that lets you draw a transperant ball where each
' light should be. the ball visibility could be turned on and off.
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL

Public Class Lights
    Public allLights() As MyLight

    '----------------------------------------------------------------------------------'
    '------------------------------------- constructor --------------------------------'
    '----------------------------------------------------------------------------------'
    ' default uses three lights
    Public Sub New()
        ReDim allLights(2)
        allLights(0) = New MyLight(0.0F, 5.0F, 8.0F)
        allLights(1) = New MyLight(0.0F, 5.0F, 4.0F)
        allLights(2) = New MyLight(0.0F, 5.0F, 0.0F)
        initialize()
    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------- alternate constructor ----------------------------'
    '----------------------------------------------------------------------------------'
    ' default uses three lights
    Public Sub New(ByVal x() As Single, ByVal y() As Single, ByVal z() As Single)

        If (x.Length = y.Length) And (y.Length = z.Length) Then
            ReDim allLights(x.Length - 1)

            For i As Integer = 0 To (allLights.Length - 1)
                allLights(i) = New MyLight(x(i), y(i), z(i))
            Next
            initialize()
        End If

    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------ initialize the lights -----------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub initialize()
        Dim i As Integer = 0
        Dim lName As Integer = LightName.Light0
        Dim enableL As Integer = EnableCap.Light0

        GL.Enable(EnableCap.Lighting)

        For Each l As MyLight In allLights
            GL.Light(lName, LightParameter.Ambient, l.lightAmbient)
            GL.Light(lName, LightParameter.Diffuse, l.lightDiffuse)
            GL.Light(lName, LightParameter.Specular, l.lightSpecular)
            GL.Light(lName, LightParameter.Position, l.lightPosition)
            GL.Enable(enableL)
            enableL += 1
            lName += 1
            i += 1
        Next

    End Sub

End Class
