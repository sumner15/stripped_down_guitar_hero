Imports System.IO

'================================================================================'
'---------------------------------- Subject class -------------------------------'
'================================================================================'
Public Class Subject
    Public num As Integer           ' number corresponding to subejct
    Public ID As String             ' the user's subject ID
    Public LoginDate As Date        ' date subject was created
    Public Kp1 As Single
    Public Kp2 As Single
    Public Kd1 As Single
    Public Kd2 As Single
    'Public dataFiles(10) As String  'array of file names for experimental dta files

    '--------------------------------------------------------------------------------'
    '------------------------- constructor for new subjects -------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New(ByRef subNum As Integer, ByRef subID As String)
        num = subNum
        ID = subID
        LoginDate = Now
        Kp1 = 10
        Kp2 = 10
        Kd1 = 1
        Kd2 = 1
        writeSubjectFile()
    End Sub

    Public Sub New(ByVal subID As String)
        readSubjectFile(subID)
    End Sub

    '--------------------------------------------------------------------------------'
    '----------------------------- write subejct file -------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub writeSubjectFile()
        Dim subjectFile As StreamWriter
        subjectFile = New StreamWriter(GAMEPATH & "Subjects\" & ID & ".txt")
        'subjectFile = My.Computer.FileSystem.OpenTextFileWriter(GAMEPATH & "Subjects\" & ID & ".txt", True)
        subjectFile.WriteLine(num)
        subjectFile.WriteLine(ID)
        subjectFile.WriteLine(LoginDate)
        subjectFile.WriteLine(Kp1)
        subjectFile.WriteLine(Kp2)
        subjectFile.WriteLine(Kd1)
        subjectFile.WriteLine(Kd2)
        subjectFile.Close()
    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------------- write read file --------------------------------'
    '--------------------------------------------------------------------------------'
    Private Sub readSubjectFile(ByVal subId As String)
        Dim subjectFile As StreamReader
        subjectFile = New StreamReader(GAMEPATH & "subjects\" & subId & ".txt")
        'subjectFile = My.Computer.FileSystem.OpenTextFileReader(GAMEPATH & "subjects\" & subId & ".txt")
        num = subjectFile.ReadLine()
        ID = subjectFile.ReadLine()
        LoginDate = subjectFile.ReadLine()
        Kp1 = subjectFile.ReadLine()
        Kp2 = subjectFile.ReadLine()
        Kd1 = subjectFile.ReadLine()
        Kd2 = subjectFile.ReadLine()
        subjectFile.Close()
    End Sub
    '--------------------------------------------------------------------------------'
    '------------------------------- update subject file ----------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub update()
        writeSubjectFile()
    End Sub
End Class
