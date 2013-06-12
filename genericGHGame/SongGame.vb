'================================================================================'
'-------------------------------- actual game class -----------------------------'
'================================================================================'
Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Math
Imports System.IO
Imports System.Text
Imports System.Diagnostics

Imports OpenTK
Imports OpenTK.Platform
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Imports System.Runtime.InteropServices
Imports OpenTK.Graphics

Public Class SongGame
    Inherits OpenTK.GameWindow
    Public thetaY As Single = 0.0
    Public thetaX As Single = 0.0
    Public camPos() = {0, 4.5, -5}
    Public sampTex As Bitmap
    Public texture As Integer

    Private lightAmbient() As Single = {1.0F, 1.0F, 1.0F, 1.0F}
    Private lightDiffuse() As Single = {0.75F, 0.75F, 0.75F, 1.0F}
    Private lightSpecular() As Single = {0.125F, 0.125F, 0.125F, 1.0F}
    Private lightPosition() As Single = {0.0F, 0.0F, 4.0F, 1.0F}
    Private lightPosition2() As Single = {Width / 100, Height / 100, 4.0F, 1.0F}
    Private lightPosition3() As Single = {-Width / 100, Height / 100, 4.0F, 1.0F}

    Public cloudBox As New MeshVbo(poly.QUADS)

    Public fretboard As Fretboard
    Private hitAttempted As Boolean = False
    Private blockedTrial As Boolean = False
    Private randomBlocker As New Random()

    Private instructions As New TextSign("use if you want")
    Private scoreText As New TextSign("this is used to show your score")

    Private scorefile As New StreamWriter(GAMEPATH & "scoreFiles\" & "score_" & currentSub.ID & "_" & CStr(Month(Now)) & CStr(Day(Now)) & CStr(Hour(Now)) & CStr(Minute(Now)) & ".txt")
    Private greatSuccess As Boolean = False
    Private possibleScore As Integer = 0
    Private score As Integer = 0

    Private state As Integer = 0
    Private PrevState As Integer = 0
    Private currentNote As Integer = 0
    Private lastNote As Integer

    Private legend As New Model("legend", "legendTile", {-Width / 50 + 5, 0.5 * (Height / 50), -6.0}, {90.0, 0.0, 0.0}, 1.5 * Width / Height)
    Private topPannel As New Model("topPannel", "topPannel", {0, Height / 50, -20.0}, {0.0, 180.0, 0.0}, Width / (50 * 10))
    Private progressbar As New Model("progressBar", {0, Height / 50, -20.0}, {0.0, 180.0, 0.0}, Width / (50 * 10))

    Private notePulseTime As Integer = Round(1 / 50 * 1000) '1(sec)/frequency(Hz) --> (msec)
    Private targPulseTime As Integer = Round(1 / 60 * 1000) '1(sec)/frequency(Hz) --> (msec)
    Private lastNotePulseTime As Integer
    Private lastTargPulseTime As Integer
    Private noteShow As Boolean = True
    Private targShow As Boolean = True

    Public mySong As Song
    'Public gameClock As New StopWatchCustom
    Public gameTimer As New Stopwatch()
    Private theEnd As Boolean = False

#Region "constructors"
    '----------------------------------------------------------------------------------'
    '---------------------------- alternate constructor -------------------------------'
    '----------------------------------------------------------------------------------'
    ' consider adding starting gains. pass in successrate as an input
    Public Sub New(ByVal difficulty As Integer)
        mySong = currentSong
        mySong.createAudioPlayer()
        fretboard = New Fretboard(mySong, difficulty)
        cloudBox.readWavefront("cloudSphere.obj")

        initializeGL()

        cloudBox.loadVbo()
        cloudBox.loadTexture("clouds2.bmp")
        instructions.visable = False

        lastNote = fretboard.numNotes - 1
        'MsgBox(Width / (50 * 10))

    End Sub
#End Region

#Region "graphics functions"
    '----------------------------------------------------------------------------------'
    '--------------------- defines the perspective for our camera ---------------------'
    '----------------------------------------------------------------------------------'
    Public Sub ViewPerspective() ' Set Up A Perspective View

        GL.MatrixMode(MatrixMode.Projection) ' Select Projection
        GL.LoadIdentity()
        Dim perspective1 As Matrix4 = OpenTK.Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _
                                            CSng((Width) / (Height)), 1, 640)

        GL.LoadMatrix(perspective1)
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    '----------------------------------------------------------------------------------'
    '-------------- defines the Orthographic perspective for our camera ---------------'
    '----------------------------------------------------------------------------------'
    Public Sub ViewOrtho() ' Set Up A Perspective View

        GL.MatrixMode(MatrixMode.Projection) ' Select Projection
        GL.LoadIdentity()

        GL.Ortho(-Width / 50, Width / 50, -Height / 50, Height / 50, 1, 50)

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()
    End Sub

    '----------------------------------------------------------------------------------'
    '-------------------- controls the orientation of the camera ----------------------'
    '----------------------------------------------------------------------------------'
    Public Sub setViewPoint()
        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadIdentity()

        Dim view_ = OpenTK.Matrix4.LookAt(camPos(0), camPos(1), camPos(2), 0, 0, -4, 0, 1, 0) ' Lookat(camPos,targetPos,upVector)

        GL.MatrixMode(MatrixMode.Modelview)
        GL.LoadMatrix(view_)
        GL.Rotate(thetaY, 0.0, 1.0, 0.0)
        GL.Rotate(thetaX, 1.0, 0.0, 0.0)

    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------ Initializes open GL -------------------------------'
    '----------------------------------------------------------------------------------'
    Public Sub initializeGL()

        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest)
        GL.ShadeModel(ShadingModel.Smooth)
        GL.Enable(EnableCap.DepthTest)
        GL.ClearDepth(1.0)
        GL.DepthFunc(DepthFunction.Lequal)

        'GL.ClearColor(Color.MidnightBlue)

        GL.Enable(EnableCap.Texture2D)

        GL.Light(LightName.Light0, LightParameter.Ambient, lightAmbient)
        GL.Light(LightName.Light0, LightParameter.Diffuse, lightDiffuse)
        GL.Light(LightName.Light0, LightParameter.Specular, lightSpecular)
        GL.Light(LightName.Light0, LightParameter.Position, lightPosition)

        GL.Light(LightName.Light1, LightParameter.Ambient, lightAmbient)
        GL.Light(LightName.Light1, LightParameter.Diffuse, lightDiffuse)
        GL.Light(LightName.Light1, LightParameter.Specular, lightSpecular)
        GL.Light(LightName.Light1, LightParameter.Position, lightPosition2)

        GL.Light(LightName.Light2, LightParameter.Ambient, lightAmbient)
        GL.Light(LightName.Light2, LightParameter.Diffuse, lightDiffuse)
        GL.Light(LightName.Light2, LightParameter.Specular, lightSpecular)
        GL.Light(LightName.Light2, LightParameter.Position, lightPosition2)

        GL.Enable(EnableCap.Lighting)
        GL.Enable(EnableCap.Light0)
        GL.Enable(EnableCap.Light1)
        GL.Enable(EnableCap.Light2)

        'GL.Enable(EnableCap.Blend)
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.DstAlpha)
        setViewPoint()
        GraphicsContext.CurrentContext.VSync = True ' if you don't set this to false then swapbuffers will wait for the monitor to refresh
    End Sub
#End Region

#Region "gameflow related functions"

    '----------------------------------------------------------------------------------'
    '------------------------------ update current note -------------------------------'
    '----------------------------------------------------------------------------------'
    ' this function checks if we have gone passed the current note. If so, it sets the 
    ' next note as the current note and calculates the movements times for tapper.
    Private Sub updateCurrentNote()
        If (gameTimer.ElapsedMilliseconds > (fretboard.nextNoteTime + 100) And (Not fretboard.songOver)) Then
            Dim previousNote As Single
            previousNote = fretboard.nextNotePos            

            ' write a line to the score file indicating whether or not the trial was sucessful
            If greatSuccess Then
                scorefile.WriteLine(fretboard.nextNotePos & vbTab & 1)
                possibleScore += 1 : score += 1
                setProgrssBar(score / possibleScore)
            Else
                'Console.WriteLine("Current score " & score / possibleScore)
                scorefile.WriteLine(fretboard.nextNotePos & vbTab & 0)
                possibleScore += 1
                setProgrssBar(score / possibleScore)
            End If

            greatSuccess = False ' just resetting it
            hitAttempted = False
            fretboard.getNextNote()
            currentNote += 1

        End If
    End Sub



#End Region

#Region "purdyWindow's events"
    '----------------------------------------------------------------------------------'
    '------------------------------ keyboard event handler ----------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_KeyPress(ByVal sender As Object, ByVal e As OpenTK.KeyPressEventArgs) Handles Me.KeyPress
        'Dim hit As Boolean
        Dim key As Integer = 0
        If (e.KeyChar = "1" Or e.KeyChar = "2" Or e.KeyChar = "3" Or e.KeyChar = "4" Or e.KeyChar = "5") Then
            key = AscW(e.KeyChar) - 49
            greatSuccess = fretboard.checkHit(gameTimer.ElapsedMilliseconds, key)
        ElseIf (e.KeyChar = "l") Then
            legend.visable = Not legend.visable
            Console.WriteLine("legend toggled")
        ElseIf (e.KeyChar = "f") Then
            instructions.visable = Not instructions.visable
            Console.WriteLine("feedback toggled")
        ElseIf (AscW(e.KeyChar) = 27) Then
            Me.Exit()
        End If
    End Sub

    '----------------------------------------------------------------------------------'
    '---------------------------- instructions executed on load -----------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        scorefile.WriteLine("stringNum" & vbTab & "success")
        mySong.player.Paused = False
        gameTimer.Start()
        lastNotePulseTime = gameTimer.ElapsedMilliseconds
        lastTargPulseTime = gameTimer.ElapsedMilliseconds
        'fretboard.getNextNote()

    End Sub
    '----------------------------------------------------------------------------------'
    '----------------------- drawing commands - render event --------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_RenderFrame(ByVal sender As Object, ByVal e As OpenTK.FrameEventArgs) Handles Me.RenderFrame
        ViewPerspective()
        setViewPoint()
        GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)

        GL.Enable(EnableCap.Texture2D)

        GL.PushMatrix()
        GL.Rotate(90, 0.0, 1.0, 0.0)
        GL.Translate(0.0, 0.0, -8.0)
        GL.BindTexture(TextureTarget.Texture2D, cloudBox.textureID)
        cloudBox.drawVbo()
        GL.PopMatrix()
        'GL.Disable(EnableCap.Texture2D) not sure if this will help

        'determine if it is time for our notes/targets to change flicker state 
        If (gameTimer.ElapsedMilliseconds - lastNotePulseTime) > notePulseTime Then
            If noteShow = False Then : noteShow = True
            ElseIf noteShow = True Then : noteShow = False
            End If
            lastNotePulseTime = gameTimer.ElapsedMilliseconds            
        End If
        If (gameTimer.ElapsedMilliseconds - lastTargPulseTime) > targPulseTime Then
            If targShow = False Then : targShow = True
            ElseIf targShow = True Then : targShow = False
            End If
            lastTargPulseTime = gameTimer.ElapsedMilliseconds
        End If

        fretboard.draw(gameTimer.ElapsedMilliseconds, noteShow, targShow)

        ' draw signs
        instructions.drawSign()
        If theEnd Then scoreText.drawSign()

        'now for the orthographic stuff
        ViewOrtho()
        legend.drawModel()
        topPannel.drawModel()
        progressbar.drawModel()

        Me.SwapBuffers()

    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------ resize window event -------------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        GL.Viewport(0, 0, Width, Height)
        ViewPerspective()

        ' need to reposition all of the orthographic objects
        topPannel.pos = {0.0, Height / 50, -30.0}
        topPannel.scale = {Width / (50 * 10), Width / (50 * 10), Width / (50 * 10)}
        legend.pos = {-Width / 50 + 3, 0.4 * (Height / 50), -30.0}
        legend.scale = {2 * Width / Height, 2 * Width / Height, 2 * Width / Height}
        progressbar.pos = {-0.975 * (Width / 50), (Height / 50) - 1.45 * (Width / 500), -30.0}
        progressbar.scale = {0.5 * Width / (50), Width / (500 * 3), Width / (50 * 10)}
        setProgrssBar(score / possibleScore)

        ' now we need to move the lights
        lightPosition = {0.0F, 0.0F, 4.0F, 1.0F}
        lightPosition2 = {Width / 50, Height / 100, 4.0F, 1.0F}
        lightPosition3 = {-Width / 50, Height / 100, 4.0F, 1.0F}

        GL.Light(LightName.Light0, LightParameter.Position, lightPosition)
        GL.Light(LightName.Light1, LightParameter.Position, lightPosition2)
        GL.Light(LightName.Light2, LightParameter.Position, lightPosition3)
    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------------ update frame event --------------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_UpdateFrame(ByVal sender As Object, ByVal e As OpenTK.FrameEventArgs) Handles Me.UpdateFrame
        ' you can put your state code ( or anything else of a similar nature) here
        If Mouse.Item(0) = True Then
            thetaY = Mouse.X / 1.25
            thetaX = Mouse.Y / 1.25
        End If
        camPos(2) = Mouse.Wheel + 4

        updateCurrentNote()
        If (mySong.player.Finished) And Not theEnd Then
            theEnd = True
            scoreText = New TextSign("you scored " & CStr(score) & " out of " & CStr(possibleScore))
            'scoreText = New TextSign("Good Job!")
        End If

        'System.Threading.Thread.CurrentThread.Sleep(1)


    End Sub

    '----------------------------------------------------------------------------------'
    '------------------------- unloading game window event ----------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub purdyWindow_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        GL.DeleteTextures(TextureTarget.Texture2D, cloudBox.textureID)
        cloudBox.freeBuffers()
        legend.freeMemory()

        scorefile.Close()

        fretboard.freeMemory()
        

        mySong.player.Stop()

        mySong.player.Dispose()

    End Sub

#End Region

#Region "other functions"
    '----------------------------------------------------------------------------------'
    '------------------------------ set progress bar ----------------------------------'
    '----------------------------------------------------------------------------------'
    Private Sub setProgrssBar(ByVal successRate As Single)
        ' success rate should be between 0 and 1
        progressbar.scale(0) = successRate * Width / (50)
        'Console.WriteLine("success rate: " & successRate)
    End Sub

#End Region

End Class
