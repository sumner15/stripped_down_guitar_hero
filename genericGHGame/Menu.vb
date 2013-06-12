'----------------------------------------------------------------------------------'
'-------------------------- the windows form menu codez ---------------------------'
'----------------------------------------------------------------------------------'
Public Class Menu
    Private pop As Population
    Private library As Library
    Private gameRunning As Boolean
    Public ourWindow As SongGame

    '--------------------------------------------------------------------------------'
    '------------------------- constructor for the menu screen ----------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        pop = New Population()
        library = New Library()
        subjectList.DataSource = pop.subIds
        songList.DataSource = library.songNames
        gameRunning = 0
        currentSub = pop.subjects(0)
        currentSong = library.songs(0)
        difficultyList.SelectedIndex = 0

        moreNoise = New NoiseMaker()
    End Sub

    '--------------------------------------------------------------------------------'
    '--------------------------- add subject button event ---------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub updateLstBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateLstBtn.Click
        Dim subj As Subject
        Dim num As Integer           ' number corresponding to subejct

        If Not (subIdTb.Text = "") Then
            num = pop.popSize + 1
            subj = New Subject(num, subIdTb.Text)
            pop.addSubejct(subj)

            subjectList.DataSource = pop.subIds
            subjectList.Update()
        Else
            MsgBox("Enter the subject's information before trying to save the subject.")
        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '--------------------------- click subject list event ---------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub subjectList_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles subjectList.SelectedValueChanged
        Dim selected As Integer
        selected = subjectList.SelectedIndex
        currentSub = pop.subjects(selected)
    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------------ click song list event ---------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub songList_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles songList.SelectedValueChanged
        Dim selected As Integer
        selected = songList.SelectedIndex
        currentSong = library.songs(selected)
        thumbnail.ImageLocation = currentSong.songPath & "thumbnail.jpg"
        songNameLbl.Text = currentSong.name
    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------------- play song button ----------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub playSongBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles playSongBtn.Click
        Dim difficulties() As Integer = {level.superEasy, level.easy, level.medium}
        If Not gameRunning Then
            gameRunning = True
            trialStr = currentSong.name
            ourWindow = New SongGame(difficulties(difficultyList.SelectedIndex))
            ourWindow.Run(FPS)
            ourWindow.Dispose()
            gameRunning = False
        End If
    End Sub

    '--------------------------------------------------------------------------------'
    '---------------------------------- flicker rate HSB ----------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub NoteRateHSB_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles NoteRateHSB.Scroll
        NoteRateLbl.Text = CStr(NoteRateHSB.Value)
        noteRate = NoteRateHSB.Value
    End Sub

    Private Sub TargRateHSB_Scroll(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ScrollEventArgs) Handles TargRateHSB.Scroll
        TargRateLbl.Text = CStr(TargRateHSB.Value)
        targRate = TargRateHSB.Value
    End Sub
End Class