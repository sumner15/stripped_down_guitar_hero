'----------------------------------------------------------------------------------'
'---------------------------------- target class ----------------------------------'
'----------------------------------------------------------------------------------'
' this class describes  a target. The class is responsible for drawing the target,
' and the target animations.

Imports System.Math
Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Graphics
Public Class Target
    Private target As MeshVbo  ' graphical representation
    Private targetHit As MeshVbo  ' graphical representation
    Private flame As MeshVbo   ' flames - replace later with a particle engine!
    'Public targetHit As MeshVbo' add another view/sequence for when they hit the key
    'define target color and stuff later
    Private flameOn As Boolean = False
    Private hitOn As Boolean = False
    Private hitOnDuration As Integer = 30
    Private flameDuration As Integer = 30 ' show flames for 50 frames
    Private flameFcount As Integer = 0
    Private hitOnCount As Integer = 0
    Private xPos As Double
    Private colors(4) As Color4

    '----------------------------------------------------------------------------------'
    '---------------------------------- constructor -----------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub New()
        target = New MeshVbo(poly.TRIS)
        targetHit = New MeshVbo(poly.TRIS)
        flame = New MeshVbo(poly.TRIS)
        Console.Write(GAMEPATH & vbNewLine)
        target.readWavefront("target2.obj")
        targetHit.readWavefront("target2hit.obj")
        flame.readWavefront("flame.obj")
        target.loadVbo()
        flame.loadVbo()

        xPos = 1.45
    End Sub

    Public Sub New(ByVal strNum As Integer)
        target = New MeshVbo(poly.TRIS)
        targetHit = New MeshVbo(poly.TRIS)
        flame = New MeshVbo(poly.TRIS)
        target.readWavefront("target2.obj")
        targetHit.readWavefront("target2hit.obj")
        flame.readWavefront("flame.obj")
        target.loadVbo()
        targetHit.loadVbo()
        flame.loadVbo()

        colors(0) = New Color4(0.005F, 0.45F, 0.005F, 1.0F)
        colors(1) = New Color4(0.1F, 0.15F, 0.7F, 1.0F)
        colors(2) = New Color4(0.7F, 0.85F, 0.2F, 1.0F)
        colors(3) = New Color4(0.93F, 0.14F, 0.13F, 1.0F)
        colors(4) = New Color4(0.93F, 0.3F, 0.05F, 1.0F) 'New Color4(0.93F, 0.6F, 0.12F, 1.0F)

        targetHit.Materials(0).Kd = colors(strNum)
        target.Materials(0).Kd = colors(strNum)

        If (strNum < 5) Then
            xPos = positions(strNum)
        Else
            Console.Write("invalid string number: " & strNum & vbNewLine)
        End If
    End Sub

    '----------------------------------------------------------------------------------'
    '--------------- draws the target under normal conditions -------------------------'
    '----------------------------------------------------------------------------------'
    ' draws the flames if flameOn is on
    Public Sub drawTarget()

        GL.PushMatrix()
        GL.Translate(xPos, 0.15, 0)
        GL.Disable(EnableCap.Texture2D)
        target.drawVbo()
        GL.PopMatrix()

        If hitOn Then
            If hitOnCount < hitOnDuration Then
                GL.PushMatrix()
                GL.Translate(xPos, 0.15, 0)
                GL.Disable(EnableCap.Texture2D)
                targetHit.drawVbo()
                GL.PopMatrix()
                hitOnCount += 1
            Else
                hitOn = False
                hitOnCount = 0
            End If
        End If

        If flameOn Then
            If flameFcount < flameDuration Then
                GL.PushMatrix()
                GL.Translate(xPos, 0.15, 0)
                GL.Disable(EnableCap.Texture2D)
                flame.drawVbo()
                GL.PopMatrix()
                flameFcount = flameFcount + 1
            Else
                flameOn = False
                flameFcount = 0
            End If
        End If

    End Sub

    '----------------------------------------------------------------------------------'
    '----------------------------- draws the flams annimation -------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub drawFlame()
        If Not flameOn Then
            flameOn = True
            hitOnCount = 0
        End If
    End Sub

    '----------------------------------------------------------------------------------'
    '----------------------------- draws the flams annimation -------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub drawHit()
        If Not flameOn Then
            hitOn = True
            flameFcount = 0
        End If
    End Sub

    '----------------------------------------------------------------------------------'
    '-------------------------------- frees up the memory -----------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub freeMemory()
        target.freeBuffers()
        flame.freeBuffers()
    End Sub

End Class
