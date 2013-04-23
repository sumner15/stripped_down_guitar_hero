'----------------------------------------------------------------------------------'
'------------------------- this class defines a single light ----------------------'
'----------------------------------------------------------------------------------'
Public Class MyLight
    Public lightAmbient(4) As Single
    Public lightDiffuse(4) As Single
    Public lightSpecular(4) As Single

    Public lightPosition(4) As Single

    '----------------------------------------------------------------------------------'
    '------------------------ default for light class constructor ---------------------'
    '----------------------------------------------------------------------------------'
    ' this will load lighting values that I like. it will place the light at 0,0,0
    Public Sub New()
        lightAmbient = {1.0F, 1.0F, 1.0F, 1.0F}
        lightDiffuse = {0.75F, 0.75F, 0.75F, 1.0F}
        lightSpecular = {0.25F, 0.25F, 0.25F, 1.0F}
        lightPosition = {0.0F, 0.0F, 0.0F, 1.0F}
    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------ alternative light class constructor ---------------------'
    '----------------------------------------------------------------------------------'
    ' this will load lighting values that I like. it will place the light at the specified pos
    Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
        lightAmbient = {1.0F, 1.0F, 1.0F, 1.0F}
        lightDiffuse = {0.75F, 0.75F, 0.75F, 1.0F}
        lightSpecular = {0.2F, 0.2F, 0.2F, 1.0F}
        lightPosition = {x, y, z, 1.0F}
    End Sub
End Class
