'--------------------------------------------------------------------------------'
'--------------------------------- Material class -------------------------------'
'--------------------------------------------------------------------------------'
' this class will store the material data read from material files. It will also
' store an array of vertex indices describing which surfaces the material should
' be applied to. when it comes time to draw the material, these indices should be

Imports System
Imports System.Runtime.InteropServices

Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Graphics
Public Class Material
    Public Ns As Single
    Public Ka As Color4
    Public Kd As Color4
    Public Ks As Color4
    Public Ni As Single
    Public d As Single
    Public illum As Single
    Public TexFPath As String

    Public name As String

    Public Indices() As Integer ' this is an array of vertex indices describing tho surfaces this material should be applied to.
    Public numIndices As Integer = 0
    Private ElementBufferID As Integer
    Public numElements As Integer

#Region "constructors"
    '--------------------------------------------------------------------------------'
    '------------------------------ default contructor ------------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New()
        Ns = 5.882353
        Ka = New Color4(0.0F, 0.0F, 0.0F, 1.0F)
        Kd = New Color4(0.0F, 0.64F, 0.607436F, 1.0F)
        Ks = New Color4(0.140857F, 0.340909F, 0.336568F, 1.0F)
        Ni = 1
        d = 1
        illum = 2
    End Sub
#End Region

    '--------------------------------------------------------------------------------'
    '------------------------------ load element buffer -----------------------------'
    '--------------------------------------------------------------------------------'
    ' this function loads the indices array into an element buffer
    Public Sub loadIndBuffer()
        Dim bufferSize As Integer
        If (Not IsDBNull(Indices)) Then
            GL.GenBuffers(1, ElementBufferID)  'Generate Array Buffer Id
            GL.BindBuffer(BufferTarget.ArrayBuffer, ElementBufferID)  'Bind current context to Array Buffer ID
            GL.BufferData(BufferTarget.ArrayBuffer, CType(Indices.Length * Marshal.SizeOf(GetType(Integer)), IntPtr), Indices, BufferUsageHint.StaticDraw)  'Send data to buffer

            ' check the buffer size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
            If (Indices.Length * Marshal.SizeOf(GetType(Integer)) <> bufferSize) Then _
                Throw New ApplicationException("Vertex array not uploaded correctly - colors")

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding

            numElements = Indices.Length
        End If
    End Sub


    '--------------------------------------------------------------------------------'
    '------------------------------- apply the Material -----------------------------'
    '--------------------------------------------------------------------------------'
    ' this function actually applys the material and binds our array buffer
    Public Sub bindMat()
        GL.Material(MaterialFace.Front, MaterialParameter.Ambient, Ka)
        GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, Kd)
        GL.Material(MaterialFace.Front, MaterialParameter.Specular, Ks)
        GL.Material(MaterialFace.Front, MaterialParameter.Emission, Ns)
        GL.ShadeModel(ShadingModel.Smooth)

        If (ElementBufferID <> 0) Then
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferID)
        End If

    End Sub

    '--------------------------------------------------------------------------------'
    '-------------------------- clean it all up in the end --------------------------'
    '--------------------------------------------------------------------------------'
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
