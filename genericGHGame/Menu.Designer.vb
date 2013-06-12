<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Menu
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Menu))
        Me.difficultyList = New System.Windows.Forms.ListBox()
        Me.playSongBtn = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.songNameLbl = New System.Windows.Forms.Label()
        Me.thumbnail = New System.Windows.Forms.PictureBox()
        Me.songList = New System.Windows.Forms.ListBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.updateLstBtn = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.subIDLbl = New System.Windows.Forms.Label()
        Me.subIdTb = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.subjectList = New System.Windows.Forms.ListBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.NoteRateHSB = New System.Windows.Forms.HScrollBar()
        Me.TargRateHSB = New System.Windows.Forms.HScrollBar()
        Me.NoteRateLbl = New System.Windows.Forms.Label()
        Me.TargRateLbl = New System.Windows.Forms.Label()
        Me.Panel2.SuspendLayout()
        CType(Me.thumbnail, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'difficultyList
        '
        Me.difficultyList.FormattingEnabled = True
        Me.difficultyList.Items.AddRange(New Object() {"easy", "medium", "hard"})
        Me.difficultyList.Location = New System.Drawing.Point(566, 355)
        Me.difficultyList.Name = "difficultyList"
        Me.difficultyList.Size = New System.Drawing.Size(67, 56)
        Me.difficultyList.TabIndex = 21
        '
        'playSongBtn
        '
        Me.playSongBtn.Location = New System.Drawing.Point(382, 354)
        Me.playSongBtn.Name = "playSongBtn"
        Me.playSongBtn.Size = New System.Drawing.Size(178, 58)
        Me.playSongBtn.TabIndex = 19
        Me.playSongBtn.Text = "Play Song"
        Me.playSongBtn.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.songNameLbl)
        Me.Panel2.Controls.Add(Me.thumbnail)
        Me.Panel2.Location = New System.Drawing.Point(382, 67)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(251, 281)
        Me.Panel2.TabIndex = 18
        '
        'songNameLbl
        '
        Me.songNameLbl.AutoSize = True
        Me.songNameLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.songNameLbl.Location = New System.Drawing.Point(-1, 251)
        Me.songNameLbl.Name = "songNameLbl"
        Me.songNameLbl.Size = New System.Drawing.Size(111, 24)
        Me.songNameLbl.TabIndex = 2
        Me.songNameLbl.Text = "Song Name"
        '
        'thumbnail
        '
        Me.thumbnail.Location = New System.Drawing.Point(3, 3)
        Me.thumbnail.Name = "thumbnail"
        Me.thumbnail.Size = New System.Drawing.Size(245, 245)
        Me.thumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.thumbnail.TabIndex = 0
        Me.thumbnail.TabStop = False
        '
        'songList
        '
        Me.songList.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.songList.FormattingEnabled = True
        Me.songList.ItemHeight = 18
        Me.songList.Location = New System.Drawing.Point(217, 67)
        Me.songList.Name = "songList"
        Me.songList.Size = New System.Drawing.Size(159, 346)
        Me.songList.TabIndex = 17
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.updateLstBtn)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.subIDLbl)
        Me.Panel1.Controls.Add(Me.subIdTb)
        Me.Panel1.Location = New System.Drawing.Point(35, 313)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(176, 100)
        Me.Panel1.TabIndex = 16
        '
        'updateLstBtn
        '
        Me.updateLstBtn.Location = New System.Drawing.Point(7, 58)
        Me.updateLstBtn.Name = "updateLstBtn"
        Me.updateLstBtn.Size = New System.Drawing.Size(153, 23)
        Me.updateLstBtn.TabIndex = 3
        Me.updateLstBtn.Text = "Update Subject List"
        Me.updateLstBtn.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(26, 4)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(131, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Add New Subject"
        '
        'subIDLbl
        '
        Me.subIDLbl.AutoSize = True
        Me.subIDLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.subIDLbl.Location = New System.Drawing.Point(3, 26)
        Me.subIDLbl.Name = "subIDLbl"
        Me.subIDLbl.Size = New System.Drawing.Size(84, 20)
        Me.subIDLbl.TabIndex = 1
        Me.subIDLbl.Text = "Subject ID"
        '
        'subIdTb
        '
        Me.subIdTb.Location = New System.Drawing.Point(91, 28)
        Me.subIdTb.Name = "subIdTb"
        Me.subIdTb.Size = New System.Drawing.Size(69, 20)
        Me.subIdTb.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 21.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(233, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(195, 33)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "Flicker Hero!"
        '
        'subjectList
        '
        Me.subjectList.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.subjectList.FormattingEnabled = True
        Me.subjectList.ItemHeight = 18
        Me.subjectList.Location = New System.Drawing.Point(33, 67)
        Me.subjectList.Name = "subjectList"
        Me.subjectList.Size = New System.Drawing.Size(178, 238)
        Me.subjectList.TabIndex = 14
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(43, 429)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(170, 20)
        Me.Label3.TabIndex = 23
        Me.Label3.Text = "Note Flicker Rate (Hz):"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(31, 459)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(182, 20)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "Target Flicker Rate (Hz):"
        '
        'NoteRateHSB
        '
        Me.NoteRateHSB.LargeChange = 1
        Me.NoteRateHSB.Location = New System.Drawing.Point(239, 429)
        Me.NoteRateHSB.Maximum = 60
        Me.NoteRateHSB.Minimum = 5
        Me.NoteRateHSB.Name = "NoteRateHSB"
        Me.NoteRateHSB.Size = New System.Drawing.Size(394, 20)
        Me.NoteRateHSB.TabIndex = 25
        Me.NoteRateHSB.Value = 30
        '
        'TargRateHSB
        '
        Me.TargRateHSB.LargeChange = 1
        Me.TargRateHSB.Location = New System.Drawing.Point(239, 459)
        Me.TargRateHSB.Maximum = 60
        Me.TargRateHSB.Minimum = 5
        Me.TargRateHSB.Name = "TargRateHSB"
        Me.TargRateHSB.Size = New System.Drawing.Size(394, 20)
        Me.TargRateHSB.TabIndex = 26
        Me.TargRateHSB.Value = 30
        '
        'NoteRateLbl
        '
        Me.NoteRateLbl.AutoSize = True
        Me.NoteRateLbl.BackColor = System.Drawing.Color.Transparent
        Me.NoteRateLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NoteRateLbl.ForeColor = System.Drawing.Color.Red
        Me.NoteRateLbl.Location = New System.Drawing.Point(209, 429)
        Me.NoteRateLbl.Name = "NoteRateLbl"
        Me.NoteRateLbl.Size = New System.Drawing.Size(29, 20)
        Me.NoteRateLbl.TabIndex = 27
        Me.NoteRateLbl.Text = "30"
        '
        'TargRateLbl
        '
        Me.TargRateLbl.AutoSize = True
        Me.TargRateLbl.BackColor = System.Drawing.Color.Transparent
        Me.TargRateLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TargRateLbl.ForeColor = System.Drawing.Color.Red
        Me.TargRateLbl.Location = New System.Drawing.Point(209, 459)
        Me.TargRateLbl.Name = "TargRateLbl"
        Me.TargRateLbl.Size = New System.Drawing.Size(29, 20)
        Me.TargRateLbl.TabIndex = 28
        Me.TargRateLbl.Text = "30"
        '
        'Menu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(666, 506)
        Me.Controls.Add(Me.TargRateLbl)
        Me.Controls.Add(Me.NoteRateLbl)
        Me.Controls.Add(Me.TargRateHSB)
        Me.Controls.Add(Me.NoteRateHSB)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.difficultyList)
        Me.Controls.Add(Me.playSongBtn)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.songList)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.subjectList)
        Me.Name = "Menu"
        Me.Text = "Menu"
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.thumbnail, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents difficultyList As System.Windows.Forms.ListBox
    Friend WithEvents playSongBtn As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents songNameLbl As System.Windows.Forms.Label
    Friend WithEvents thumbnail As System.Windows.Forms.PictureBox
    Friend WithEvents songList As System.Windows.Forms.ListBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents updateLstBtn As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents subIDLbl As System.Windows.Forms.Label
    Friend WithEvents subIdTb As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents subjectList As System.Windows.Forms.ListBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents NoteRateHSB As System.Windows.Forms.HScrollBar
    Friend WithEvents TargRateHSB As System.Windows.Forms.HScrollBar
    Friend WithEvents NoteRateLbl As System.Windows.Forms.Label
    Friend WithEvents TargRateLbl As System.Windows.Forms.Label
End Class
