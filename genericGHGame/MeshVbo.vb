Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports System.Drawing
Imports System.Drawing.Imaging

Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports OpenTK.Graphics

Enum poly
    TRIS = 1
    QUADS = 2
End Enum

Public Class MeshVbo
    Private Positions() As Vector3
    Private Indices() As Integer
    Private Normals() As Vector3
    Private Texcoords() As Vector2
    Private Colors() As Integer

    Private VertexBufferID As Integer
    Private TexcoordBufferID As Integer
    Private NormalBufferID As Integer
    Private ElementBufferID As Integer
    Private ColorBufferID As Integer
    Private numElements As Integer

    Public textureID As Integer
    Private meshTexture As Bitmap

    Private PositionsLst() As Vector3
    Private NormalsLst() As Vector3   ' these are face normals (they need to be converted)
    Private TexcoordsLst() As Vector2

    Private PositionsInd() As Integer
    Private NormalsInd() As Integer
    Private TexcoordsInd() As Integer

    Private VertexCount As Integer = 0
    Private numFaces As Integer = 0
    Private posNorms() As Vector3    ' note: this list of normals does not correspond to each vertex, but to each position value - you can use the position indices to reference them to vertices

    Public Materials() As Material

    Private useColors As Boolean = False
    Private useNormals As Boolean = False
    Private useTexcoords As Boolean = False

    Public useMaterial As Boolean = True
    Public polyType As Integer = poly.TRIS
    Public objName As String

#Region "constructors"

    Public Sub New()
        ReDim Positions(0)
        ReDim Indices(0)
        ReDim Normals(0)
        ReDim Texcoords(0)
        ReDim Colors(0)
    End Sub

    Public Sub New(ByVal testConst As Char)
        ReDim Positions(23)
        ReDim Indices(23)
        ReDim Normals(23)
        ReDim Texcoords(23)
        ReDim Colors(23)

        polyType = poly.QUADS

        'bottom
        Positions(0) = New Vector3(1.0, -1.0, -1.0)  '1
        Positions(1) = New Vector3(1.0, -1.0, 1.0)   '2
        Positions(2) = New Vector3(-1.0, -1.0, 1.0)  '3
        Positions(3) = New Vector3(-1.0, -1.0, -1.0) '4
        'top
        Positions(4) = New Vector3(1.0, 1.0, -1.0)   '5
        Positions(5) = New Vector3(1.0, 1.0, 1.0)    '6
        Positions(6) = New Vector3(-1.0, 1.0, 1.0)   '7
        Positions(7) = New Vector3(-1.0, 1.0, -1.0)  '8
        'back
        Positions(8) = New Vector3(1.0, 1.0, -1.0)   '5
        Positions(9) = New Vector3(1.0, -1.0, -1.0)  '1
        Positions(10) = New Vector3(-1.0, -1.0, -1.0) '4
        Positions(11) = New Vector3(-1.0, 1.0, -1.0) '8
        'left
        Positions(12) = New Vector3(-1.0, 1.0, -1.0) '8
        Positions(13) = New Vector3(-1.0, -1.0, -1.0) '4
        Positions(14) = New Vector3(-1.0, -1.0, 1.0)  '3
        Positions(15) = New Vector3(-1.0, 1.0, 1.0)   '7
        'right
        Positions(16) = New Vector3(1.0, 1.0, -1.0)   '5
        Positions(17) = New Vector3(1.0, -1.0, -1.0)  '1
        Positions(18) = New Vector3(1.0, -1.0, 1.0)   '2
        Positions(19) = New Vector3(1.0, 1.0, 1.0)    '6
        'front
        Positions(20) = New Vector3(1.0, 1.0, 1.0)    '6
        Positions(21) = New Vector3(1.0, -1.0, 1.0)   '2
        Positions(22) = New Vector3(-1.0, -1.0, 1.0)  '3
        Positions(23) = New Vector3(-1.0, 1.0, 1.0)   '7

        For i = 0 To 23
            Normals(i) = Positions(i)
            Normals(i).NormalizeFast()
        Next i

        For i = 0 To 23 Step 4
            Texcoords(i) = New Vector2(0.0, 0.0)
            Texcoords(i + 1) = New Vector2(1.0, 0.0)
            Texcoords(i + 2) = New Vector2(1.0, 1.0)
            Texcoords(i + 3) = New Vector2(0.0, 1.0)
        Next

        For i = 0 To 3 Step 1
            Colors(i) = convertRGBA32(Color.BlanchedAlmond)
            Colors(4 + i) = convertRGBA32(Color.Aqua)
            Colors(8 + i) = convertRGBA32(Color.LightPink)
            Colors(12 + i) = convertRGBA32(Color.Crimson)
            Colors(16 + i) = convertRGBA32(Color.Cyan)
            Colors(20 + i) = convertRGBA32(Color.Cyan)
        Next i

        Indices = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23}

    End Sub

    Public Sub New(ByVal fName As String)
        ReDim Positions(0)
        ReDim Indices(0)
        ReDim Normals(0)
        ReDim Texcoords(0)
        ReDim Colors(0)
        objName = fName
    End Sub

    Public Sub New(ByVal polyType_ As Integer)
        polyType = polyType_
        ReDim Positions(0)
        ReDim Indices(0)
        ReDim Normals(0)
        ReDim Texcoords(0)
        ReDim Colors(0)
    End Sub

#End Region

#Region "functions for controlling graphics buffers"
    '' -----------------------------------------------------------------------------------------------------''
    '' ---------------------------- this sub loads our Mesh data into buffers ------------------------------''
    '' -----------------------------------------------------------------------------------------------------''
    Public Sub loadVbo()
        Dim bufferSize As Integer

        If (Not IsDBNull(Colors)) Then
            GL.GenBuffers(1, ColorBufferID)  'Generate Array Buffer Id
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferID)  'Bind current context to Array Buffer ID
            GL.BufferData(BufferTarget.ArrayBuffer, CType(Colors.Length * Marshal.SizeOf(GetType(Integer)), IntPtr), Colors, BufferUsageHint.StaticDraw)  'Send data to buffer

            ' check the buffer size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
            If (Colors.Length * Marshal.SizeOf(GetType(Integer)) <> bufferSize) Then _
                Throw New ApplicationException("Vertex array not uploaded correctly - colors")

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding
        End If

        If (Not IsDBNull(Normals)) Then
            GL.GenBuffers(1, NormalBufferID)  'Generate Array Buffer Id
            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBufferID)  'Bind current context to Array Buffer ID
            GL.BufferData(BufferTarget.ArrayBuffer, CType(Normals.Length * Vector3.SizeInBytes, IntPtr), Normals, BufferUsageHint.StaticDraw)  'Send data to buffer

            ' check the buffer size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
            If (Normals.Length * Vector3.SizeInBytes <> bufferSize) Then _
                Throw New ApplicationException("Vertex array not uploaded correctly - normals")

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding
        End If

        If (Not IsDBNull(Texcoords)) Then
            GL.GenBuffers(1, TexcoordBufferID)  'Generate Array Buffer Id
            GL.BindBuffer(BufferTarget.ArrayBuffer, TexcoordBufferID)  'Bind current context to Array Buffer ID
            GL.BufferData(BufferTarget.ArrayBuffer, CType(Texcoords.Length * Vector2.SizeInBytes, IntPtr), Texcoords, BufferUsageHint.StaticDraw)  'Send data to buffer

            ' check the buffer size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
            If (Texcoords.Length * Vector2.SizeInBytes <> bufferSize) Then _
                Throw New ApplicationException("Vertex array not uploaded correctly - texcoords")

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding
        End If

        If (Not IsDBNull(Positions)) Then
            GL.GenBuffers(1, VertexBufferID)  'Generate Array Buffer Id
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID)  'Bind current context to Array Buffer ID
            GL.BufferData(BufferTarget.ArrayBuffer, CType(Positions.Length * Vector3.SizeInBytes, IntPtr), Positions, BufferUsageHint.StaticDraw)  'Send data to buffer

            ' check the buffer size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
            If (Positions.Length * Vector3.SizeInBytes <> bufferSize) Then _
                Throw New ApplicationException("Vertex array not uploaded correctly - positions")

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding
        End If

        If useMaterial Then
            For i = 0 To (Materials.Length - 1)
                Materials(i).loadIndBuffer()
            Next i
            'writeMaterials()
        Else
            If (Not IsDBNull(Indices)) Then
                GL.GenBuffers(1, ElementBufferID)  'Generate Array Buffer Id
                GL.BindBuffer(BufferTarget.ArrayBuffer, ElementBufferID)  'Bind current context to Array Buffer ID
                GL.BufferData(BufferTarget.ArrayBuffer, CType(Indices.Length * Marshal.SizeOf(GetType(Integer)), IntPtr), Indices, BufferUsageHint.StaticDraw)  'Send data to buffer

                ' check the buffer size
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, bufferSize)
                If (Indices.Length * Marshal.SizeOf(GetType(Integer)) <> bufferSize) Then _
                    Throw New ApplicationException("Vertex array not uploaded correctly - indices")

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0)  ' clear the buffer binding
            Else
                MsgBox("indices fail" & vbNewLine)
            End If
            numElements = Indices.Length
        End If


    End Sub

    '' -----------------------------------------------------------------------------------------------------''
    '' --------------------------- this sub draws the objects stored in our buffers ------------------------''
    '' -----------------------------------------------------------------------------------------------------''
    Public Sub drawVbo()
        GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit)
        If (VertexBufferID = 0) Then Return
        If (ElementBufferID = 0 And useMaterial = False) Then
            MsgBox("failed to draw - no element buffer" & vbNewLine)
            Return
        End If


        ' first deal with normals and colors
        If (GL.IsEnabled(EnableCap.Lighting)) Then
            If (NormalBufferID <> 0) Then
                ' Bind to the Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBufferID)
                ' Set the Pointer to the current bound array describing how the data ia stored
                GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero)
                ' Enable the client state so it will use this array buffer pointer
                GL.EnableClientState(ArrayCap.NormalArray)
            End If
        End If
        If (ColorBufferID <> 0 And ((Not GL.IsEnabled(EnableCap.Lighting)) Or GL.IsEnabled(EnableCap.ColorMaterial))) Then
            ' Bind to the Array Buffer ID
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorBufferID)
            ' Set the Pointer to the current bound array describing how the data ia stored
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, Marshal.SizeOf(GetType(Integer)), IntPtr.Zero)
            ' Enable the client state so it will use this array buffer pointer
            GL.EnableClientState(ArrayCap.ColorArray)
        End If


        ' Now for the textures
        If (GL.IsEnabled(EnableCap.Texture2D)) Then
            If (TexcoordBufferID <> 0) Then
                ' Bind to the Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, TexcoordBufferID)
                ' Set the Pointer to the current bound array describing how the data ia stored
                GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero)
                ' Enable the client state so it will use this array buffer pointer
                GL.EnableClientState(ArrayCap.TextureCoordArray)
            End If
        End If

        ' Now for the actual position vertices
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferID)  ' Bind to the Array Buffer ID
        GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero)
        GL.EnableClientState(ArrayCap.VertexArray)

        If useMaterial Then
            For i = 0 To (Materials.Length - 1)
                Materials(i).bindMat()
                If polyType = poly.QUADS Then
                    GL.DrawElements(BeginMode.Quads, Materials(i).numElements, DrawElementsType.UnsignedInt, IntPtr.Zero)
                ElseIf polyType = poly.TRIS Then
                    GL.DrawElements(BeginMode.Triangles, Materials(i).numElements, DrawElementsType.UnsignedInt, IntPtr.Zero)
                End If
            Next i
        Else
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferID)

            ' Draw the elements in the element array buffer
            ' Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
            'GL.Material(MaterialFace.Front, MaterialParameter.Ambient, New Color4(1.0F, 1.0F, 1.0F, 1.0F))
            'GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, New Color4(1.0F, 1.0F, 1.0F, 1.0F))
            'GL.Material(MaterialFace.Front, MaterialParameter.Specular, New Color4(1.0F, 1.0F, 1.0F, 1.0F))
            'GL.Material(MaterialFace.Front, MaterialParameter.Emission, 1.0F)
            GL.Enable(EnableCap.ColorMaterial)
            GL.Color4(1.0F, 1.0F, 1.0F, 0.98F)
            GL.ShadeModel(ShadingModel.Smooth)
            If polyType = poly.QUADS Then
                GL.DrawElements(BeginMode.Quads, numElements, DrawElementsType.UnsignedInt, IntPtr.Zero)
            ElseIf polyType = poly.TRIS Then
                GL.DrawElements(BeginMode.Triangles, numElements, DrawElementsType.UnsignedInt, IntPtr.Zero)
            End If
            GL.Disable(EnableCap.ColorMaterial)
        End If

        ' Restore the state
        GL.PopClientAttrib()

    End Sub

    '' -----------------------------------------------------------------------------------------------------''
    '' ------------------------------ this sub clears out our buffers --------------------------------------''
    '' -----------------------------------------------------------------------------------------------------''
    Public Sub freeBuffers()
        If (VertexBufferID <> 0) Then GL.DeleteBuffers(1, VertexBufferID)
        If (TexcoordBufferID <> 0) Then GL.DeleteBuffers(1, TexcoordBufferID)
        If (NormalBufferID <> 0) Then GL.DeleteBuffers(1, NormalBufferID)
        If (ElementBufferID <> 0) Then GL.DeleteBuffers(1, ElementBufferID)
    End Sub
#End Region

    '' -----------------------------------------------------------------------------------------------------''
    '' ----------------- this sub loads a texture from a file and runs the create texture sub --------------''
    '' -----------------------------------------------------------------------------------------------------''
    ' generate a texture
    Public Sub loadTexture(ByVal fName As String)
        If Dir$(GAMEPATH & "textures\" & fName) <> "" Then
            meshTexture = New Bitmap(GAMEPATH & "textures\" & fName)
            meshTexture.RotateFlip(RotateFlipType.Rotate180FlipX)
            createTexture()
        Else
            MsgBox("texture fail")
        End If
    End Sub

    '' -----------------------------------------------------------------------------------------------------''
    '' -------------------------------- this sub creates our texture ---------------------------------------''
    '' -----------------------------------------------------------------------------------------------------''
    Public Sub createTexture()
        GL.GenTextures(1, textureID)
        GL.BindTexture(TextureTarget.Texture2D, textureID)

        Dim data As BitmapData = meshTexture.LockBits(New System.Drawing.Rectangle(0, 0, meshTexture.Width, meshTexture.Height), _
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb)

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, _
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0)

        GL.GenerateMipmap(TextureTarget.Texture2D) ' generates a mipmap of our texture

        meshTexture.UnlockBits(data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, CInt(TextureMagFilter.Linear))
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, CInt(TextureMinFilter.LinearMipmapNearest))
        GL.BindTexture(TextureTarget.Texture2D, 0)
    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ------------------------ Read data from wavefront file ----------------------------------------- '
    ' ------------------------------------------------------------------------------------------------ '
    ' for now we will just focus on reading the data from the file. We will organize that data in other functions
    Public Sub readWavefront(ByVal fName As String)
        Dim fPath As String
        Dim currentRow As String()
        Dim pCount As Integer = 0
        Dim nCount As Integer = 0
        Dim iCount As Integer = 0
        Dim tcCount As Integer = 0
        'define counters for all of our arrays - this is just the easiest way
        Dim vertsPerFace As Integer = 0
        Dim iPosLst As Integer = 0
        Dim iPosInd As Integer = 0
        Dim iNormLst As Integer = 0
        Dim iNormInd As Integer = 0
        Dim iTexLst As Integer = 0
        Dim iTexInd As Integer = 0
        Dim faceCount As Integer = 0
        Dim currentMat As Integer = 0
        Dim matInd As Integer = 0

        fPath = GAMEPATH & "objFiles\" & fName

        ' first we need to read the material file
        readMtlFile(fName)
        If Materials.Length = 0 Then useMaterial = False
        If (Not File.Exists(fPath)) Then Throw New ApplicationException("Invalid path to obj file :(")

        ' ------------------------------------------------------------------------------------------------ '
        ' -- Initial scan of the file -------------------------------------------------------------------- '
        ' ------------------------------------------------------------------------------------------------ '
        Dim MyReader As New TextFieldParser(fPath)
        MyReader.SetDelimiters(" ", "/")

        While Not MyReader.EndOfData
            Try
                currentRow = MyReader.ReadFields()
                If (currentRow(0) = "#") Then
                ElseIf (currentRow(0) = "o") Then
                ElseIf (currentRow(0) = "v") Then
                    pCount = pCount + 1
                ElseIf (currentRow(0) = "vn") Then
                    nCount = nCount + 1
                ElseIf (currentRow(0) = "vt") Then
                    tcCount = tcCount + 1
                ElseIf (currentRow(0) = "f") Then
                    iCount = iCount + 1
                    If vertsPerFace = 0 Then vertsPerFace = (currentRow.Length - 1) / 3
                    If useMaterial Then Materials(currentMat).numIndices += vertsPerFace
                ElseIf (currentRow(0) = "usemtl") Then
                    If Not (currentRow(1) = "(null)") Then currentMat = getMatFromName(currentRow(1))
                End If
            Catch ex As MalformedLineException
                MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
            End Try
        End While

        numFaces = iCount

        If vertsPerFace = 3 Then
            polyType = poly.TRIS
        Else
            polyType = poly.QUADS
        End If

        'determine whether or not we are setting normals and texcoords
        If nCount > 0 Then useNormals = True
        If tcCount > 0 Then useTexcoords = True

        ' define the dimensions of our buffer arrays
        ReDim PositionsLst(pCount - 1)
        ReDim PositionsInd(iCount * vertsPerFace - 1)
        If useNormals Then
            ReDim NormalsLst(nCount - 1)
            ReDim NormalsInd(iCount * vertsPerFace - 1)
        End If
        If useTexcoords Then
            ReDim TexcoordsLst(tcCount - 1)
            ReDim TexcoordsInd(iCount * vertsPerFace - 1)
        End If
        ' reserver space for our material indices
        For i = 0 To (Materials.Length - 1) Step 1
            ReDim Materials(i).Indices(Materials(i).numIndices)
        Next

        ' ------------------------------------------------------------------------------------------------ '
        ' -- Actually read the file ---------------------------------------------------------------------- '
        ' ------------------------------------------------------------------------------------------------ '

        ' close and reopen the reader because it's lame
        MyReader.Close()
        MyReader = New TextFieldParser(fPath)
        MyReader.SetDelimiters(" ", "/")

        'read data from the file into our arrays
        While Not MyReader.EndOfData
            Try
                currentRow = MyReader.ReadFields()
                If (currentRow(0) = "#") Then
                ElseIf (currentRow(0) = "v") Then
                    PositionsLst(iPosLst) = New Vector3(CDbl(currentRow(1)), CDbl(currentRow(2)), CDbl(currentRow(3)))
                    iPosLst = iPosLst + 1
                ElseIf (currentRow(0) = "vt") Then
                    TexcoordsLst(iTexLst) = New Vector2(CDbl(currentRow(1)), CDbl(currentRow(2)))
                    iTexLst = iTexLst + 1
                ElseIf (currentRow(0) = "vn") Then
                    NormalsLst(iNormLst) = New Vector3(CDbl(currentRow(1)), CDbl(currentRow(2)), CDbl(currentRow(3)))
                    iNormLst = iNormLst + 1
                ElseIf (currentRow(0) = "f") Then
                    '--------- record position indices -----------------------------------
                    For iFace = 0 To vertsPerFace - 1 Step 1
                        PositionsInd(iPosInd + iFace) = CDbl(currentRow(3 * iFace + 1) - 1)
                    Next iFace
                    iPosInd = iPosInd + vertsPerFace

                    '--------- record material indices -----------------------------------
                    For iFace = 0 To vertsPerFace - 1 Step 1
                        If useMaterial Then Materials(currentMat).Indices(matInd + iFace) = faceCount * vertsPerFace + iFace
                    Next iFace
                    matInd = matInd + vertsPerFace

                    '--------- record texcoord indices -----------------------------------
                    If useTexcoords Then
                        For iFace = 0 To vertsPerFace - 1 Step 1
                            TexcoordsInd(iTexInd + iFace) = CDbl(currentRow(3 * iFace + 2) - 1)
                        Next iFace
                        iTexInd = iTexInd + vertsPerFace
                    End If

                    '--------- record normals indices ------------------------------------
                    If useNormals Then
                        For iFace = 0 To vertsPerFace - 1 Step 1
                            NormalsInd(iNormInd + iFace) = CDbl(currentRow(3 * iFace + 3) - 1)
                        Next iFace
                        iNormInd = iNormInd + vertsPerFace
                    End If

                    faceCount += 1

                ElseIf (currentRow(0) = "usemtl") Then
                    currentMat = getMatFromName(currentRow(1))
                    matInd = 0
                ElseIf (currentRow(0) = "mtllib") Then
                    ' don't care, I already have the file name
                End If

            Catch ex As MalformedLineException
                MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
            End Try
        End While

        MyReader.Close()
        MyReader.Dispose()

        ' Now we need to get the data into a structure that is actually useful
        restructureVertices()
    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ----------------------------------- Reads a material file -------------------------------------- '
    ' ------------------------------------------------------------------------------------------------ '
    Public Sub readMtlFile(ByVal fName As String)
        Dim fPath As String
        fPath = GAMEPATH & "objFiles\" & fName
        fPath = IO.Path.ChangeExtension(fPath, ".mtl")

        Dim currentRow As String()
        Dim numMats As Integer = 0

        Dim MyReader As New TextFieldParser(fPath)
        MyReader.SetDelimiters(" ")

        ' first we need to determine how many materials our object has
        While Not MyReader.EndOfData
            Try
                currentRow = MyReader.ReadFields()
                If (currentRow(0) = "newmtl") Then
                    numMats = numMats + 1
                End If
            Catch ex As MalformedLineException
                MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
            End Try
        End While

        ' We need to create the objects themselves
        ReDim Materials(numMats - 1)
        For i = 0 To (Materials.Length - 1) Step 1
            Materials(i) = New Material()
        Next

        Dim currentMat = 0

        ' close and reopen the reader because it's lame
        MyReader.Close()
        MyReader = New TextFieldParser(fPath)
        MyReader.SetDelimiters(" ")

        ' why do we use currentMat -1  as our index instead of currentMat? because newmtl come before the material
        While Not MyReader.EndOfData
            Try
                currentRow = MyReader.ReadFields()
                If (currentRow(0) = "newmtl") Then
                    currentMat = currentMat + 1
                    Materials(currentMat - 1).name = currentRow(1)
                ElseIf (currentRow(0) = "Ns") Then
                    Materials(currentMat - 1).Ns = CSng(currentRow(1))
                ElseIf (currentRow(0) = "Ka") Then
                    Materials(currentMat - 1).Ka = New Color4(CSng(currentRow(1)), CSng(currentRow(2)), CSng(currentRow(3)), CSng(1.0))
                ElseIf (currentRow(0) = "Kd") Then
                    Materials(currentMat - 1).Kd = New Color4(CSng(currentRow(1)), CSng(currentRow(2)), CSng(currentRow(3)), CSng(1.0))
                ElseIf (currentRow(0) = "Ks") Then
                    Materials(currentMat - 1).Ks = New Color4(CSng(currentRow(1)), CSng(currentRow(2)), CSng(currentRow(3)), CSng(1.0))
                ElseIf (currentRow(0) = "Ni") Then
                    Materials(currentMat - 1).Ni = CSng(currentRow(1))
                ElseIf (currentRow(0) = "d") Then
                    Materials(currentMat - 1).d = CSng(currentRow(1))
                ElseIf (currentRow(0) = "illum") Then
                    Materials(currentMat - 1).illum = CSng(currentRow(1))
                ElseIf (currentRow(0) = "map_Kd") Then
                    Dim texFpathStr As String = ""
                    For istr = 1 To (currentRow.Length - 1) Step 1
                        texFpathStr = texFpathStr & currentRow(istr)
                    Next istr
                    Materials(currentMat - 1).TexFPath = texFpathStr

                End If
            Catch ex As MalformedLineException
                MsgBox("Line " & ex.Message & "is not valid and will be skipped.")
            End Try
        End While

        MyReader.Close()
        MyReader.Dispose()

    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ------------------------ convert face normals to vertex normals -------------------------------- '
    ' ------------------------------------------------------------------------------------------------ '
    ' Find the vertex normals for each position.
    Public Sub getVertNormals()
        Dim normSum As New Vector3(0.0, 0.0, 0.0)
        ReDim posNorms(PositionsLst.Length - 1)

        ' for each position, find all the normals at that position and combine the vecotrs into a single unit vector
        For i = 0 To (PositionsLst.Length - 1) Step 1                   ' notice that we are starting this at 1, not 0
            For ii = 0 To (PositionsInd.Length - 1) Step 1
                If (PositionsInd(ii) = i) Then
                    normSum = normSum + NormalsLst(NormalsInd(ii))
                End If
            Next
            ' normalize and store the new vector
            normSum.NormalizeFast()
            posNorms(i) = normSum
            normSum = Vector3.Zero
        Next i
    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ------------ this function will determine how many independant vertices we need ---------------- '
    ' ------------------------------------------------------------------------------------------------ '
    ' we will start by assuming that no vertices can be shared, then we will go through and check whether 
    ' or not vertices can be shared - update: sharing didn't work at all
    Public Sub restructureVertices()
        Dim maxVerts As Integer
        Dim vertsPerFace As Integer
        Dim allVerts() As Vertex

        If polyType = poly.QUADS Then
            vertsPerFace = 4
        ElseIf polyType = poly.TRIS Then
            vertsPerFace = 3
        End If

        maxVerts = numFaces * vertsPerFace
        ReDim allVerts(maxVerts - 1)
        ReDim Positions(maxVerts - 1)
        ReDim Normals(maxVerts - 1)
        ReDim Texcoords(maxVerts - 1)

        getVertNormals()

        'create a list of vertices without using any sharing
        For i = 0 To (maxVerts - 1) Step 1
            If useTexcoords Then
                allVerts(i) = New Vertex(PositionsLst(PositionsInd(i)), posNorms(PositionsInd(i)), TexcoordsLst(TexcoordsInd(i)))
            Else
                allVerts(i) = New Vertex(PositionsLst(PositionsInd(i)), posNorms(PositionsInd(i)))
            End If

        Next i

        For i = 0 To (allVerts.Length - 1) Step 1
            Positions(i) = allVerts(i).Position
        Next i
        For i = 0 To (allVerts.Length - 1) Step 1
            Normals(i) = allVerts(i).Normal
        Next i
        If useTexcoords Then
            For i = 0 To (allVerts.Length - 1) Step 1
                Texcoords(i) = allVerts(i).Texcoord
            Next i
        End If

        ReDim Indices(allVerts.Length - 1)
        For i = 0 To (allVerts.Length - 1) Step 1
            Indices(i) = i
        Next i

        Dim indFile As String
        indFile = "vertex indices" & vbNewLine
        For i = 0 To (Indices.Length - 1) Step 1
            indFile = indFile & CStr(Indices(i)) & vbNewLine
        Next

    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ------------ this function converts a color object into it's integer representation ------------ '
    ' ------------------------------------------------------------------------------------------------ '
    Private Function convertRGBA32(ByVal c As Color) As Integer
        Return CInt((CInt(c.A) << CInt(24)) Or (CInt(c.B) << CInt(16)) Or (CInt(c.G) << 8) Or CInt(c.R))
    End Function

    ' ------------------------------------------------------------------------------------------------ '
    ' ---------------------------------- get material number from name ------------------------------- '
    ' ------------------------------------------------------------------------------------------------ '
    ' scans the list of materials and gets picks out the one with the mathcing name - it's lame, but 
    ' It's the only way I see to do it. If there is no match, it returns zero.
    Private Function getMatFromName(ByVal name As String) As Integer
        Dim matNum As Integer = 0
        If Materials.Length > 0 Then
            For i = 0 To (Materials.Length - 1)
                If Materials(i).name = name Then
                    matNum = i
                    Exit For
                End If
            Next
        End If
        Return matNum
    End Function

    Public Sub writeMaterials()
        For i = 0 To (Materials.Length - 1)
            MsgBox(Materials(i).name & vbTab & Materials(i).Kd.ToString() & vbNewLine)
        Next
    End Sub

    ' ------------------------------------------------------------------------------------------------ '
    ' ------------------------------------ clear up the memory --------------------------------------- '
    ' ------------------------------------------------------------------------------------------------ '
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
