Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Graphics

Public Class Vertex
    Public Position As Vector3
    Public Normal As Vector3
    Public Texcoord As Vector2
    Public Color As Color4

    Public Sub New(ByVal pos As Vector3, ByVal norm As Vector3)
        Position = pos
        Normal = norm
    End Sub

    Public Sub New(ByVal pos As Vector3, ByVal norm As Vector3, ByVal tex As Vector2)
        Position = pos
        Normal = norm
        Texcoord = tex
    End Sub

    Public Sub New(ByVal pos As Vector3, ByVal norm As Vector3, ByVal col As Color4)
        Position = pos
        Normal = norm
        Color = col
    End Sub

    Public Sub New(ByVal pos As Vector3, ByVal norm As Vector3, ByVal tex As Vector2, ByVal col As Color4)
        Position = pos
        Normal = norm
        Texcoord = tex
        Color = col
    End Sub

    Public Function eq(ByVal vrt As Vertex) As Boolean
        If ((Position = vrt.Position) And _
            (Normal = vrt.Normal) And _
            (Texcoord = vrt.Texcoord) And _
            (Color = vrt.Color)) Then
            Return True
        Else
            Return False
        End If

    End Function

End Class
