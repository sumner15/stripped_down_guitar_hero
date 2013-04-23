'----------------------------------------------------------------------------------'
'------------------ this class defines a sign with changable text -----------------'
'----------------------------------------------------------------------------------'
' only use this for text that changes infrequently.
Imports System.Drawing
Imports OpenTK
Imports OpenTK.Graphics.OpenGL
Imports System.Drawing.Imaging

Public Class TextSign
    Private sw33t_text As Bitmap
    Private textTexture As Integer
    Private myFont As Font
    Private textPos As PointF
    Private w As Integer
    Private h As Integer

    Private initTextureMade As Boolean = False

    Public scale As Single = 1
    Public pos() As Single = {0, 1}
    Public visable As Boolean = True

    '----------------------------------------------------------------------------------'
    '------------------------------- constructor of d00m ------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub New(ByVal text As String)
        h = 1
        w = Math.Round((text.Length - 1) / 3) * 2

        textTexture = GL.GenTexture()
        sw33t_text = New Bitmap(w * 256, h * 256)

        GL.BindTexture(TextureTarget.Texture2D, textTexture)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(All.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(All.Linear))
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sw33t_text.Width, sw33t_text.Height, 0, _
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero) ' just allocate memory, so we can update efficiently using TexSubImage2D

        Dim fsize As Integer = 80

        myFont = New Font("Veranda", fsize)
        'textPos = New PointF((w * 256) / 2 - ((text.Length) / 2) * fsize, (h * 256) / 2 - fsize)
        textPos = New PointF((w * 256) / 2, (h * 256) / 2)

        makeStringBMP(text)

    End Sub

    '----------------------------------------------------------------------------------'
    '---------------------------- create bitmap of a string ---------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub makeStringBMP(ByRef text As String)

        If Not initTextureMade Then
            initTextureMade = True
        Else
            GL.BindTexture(TextureTarget.Texture2D, textTexture)
        End If

        Dim sf As StringFormat = New StringFormat()
        sf.LineAlignment = StringAlignment.Center
        sf.Alignment = StringAlignment.Center

        Dim gfx As Graphics = Graphics.FromImage(sw33t_text)
        gfx.Clear(Color.Transparent)
        gfx.DrawString(text, myFont, Brushes.White, textPos, sf)

        Dim data As BitmapData = sw33t_text.LockBits(New Rectangle(0, 0, sw33t_text.Width, sw33t_text.Height), ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, sw33t_text.Width, sw33t_text.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)
        sw33t_text.UnlockBits(data)

    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------------- draw sign ----------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub drawSign()
        GL.Enable(EnableCap.ColorMaterial)
        GL.Color4(1.0F, 1.0F, 1.0F, 1.0F)

        GL.Enable(EnableCap.Texture2D)
        GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha)

        If visable Then
            GL.BindTexture(TextureTarget.Texture2D, textTexture)
            GL.PushMatrix()
            GL.Rotate(-25, 1, 0, 0)
            GL.Translate(pos(0), pos(1), 0)
            GL.Scale(scale, scale, scale)


            GL.Begin(BeginMode.Quads)
            GL.TexCoord2(0.0F, 1.0F) : GL.Vertex2(-w / 2, -h / 2) : GL.Normal3(-0.1, -0.1, 1)
            GL.TexCoord2(1.0F, 1.0F) : GL.Vertex2(w / 2, -h / 2) : GL.Normal3(0.1, -0.1, 1)
            GL.TexCoord2(1.0F, 0.0F) : GL.Vertex2(w / 2, h / 2) : GL.Normal3(0.1, 0.1, 1)
            GL.TexCoord2(0.0F, 0.0F) : GL.Vertex2(-w / 2, h / 2) : GL.Normal3(-0.1, 0.1, 1)
            GL.End()

            GL.PopMatrix()
        End If

        GL.Disable(EnableCap.Blend)
        GL.Disable(EnableCap.Texture2D)
        GL.Disable(EnableCap.ColorMaterial)
    End Sub

End Class
