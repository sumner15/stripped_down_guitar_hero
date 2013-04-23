Imports System.IO
'================================================================================'
'-------------------------------- Population class ------------------------------'
'================================================================================'
' organizer for subjects
Public Class Population
    'Private popFile As String = "population.txt"
    Private subList As String = "allSubjects.txt"
    Public popSize As Integer
    Public subjects() As Subject
    Public subIds() As String

    '--------------------------------------------------------------------------------'
    '------------------------- constructor for population ---------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub New()
        Dim fileReader As StreamReader
        Dim idstring As String
        'need to find out how many subjects we have
        fileReader = My.Computer.FileSystem.OpenTextFileReader(GAMEPATH & "Subjects\" & subList)
        popSize = 0
        While (Not fileReader.EndOfStream)
            idstring = fileReader.ReadLine()
            If (Not (idstring = "") And Not (idstring = " ")) Then popSize += 1
        End While

        'now we can actually read in the subject's data
        ReDim subjects(popSize - 1)
        ReDim subIds(popSize - 1)
        If popSize > 0 Then
            fileReader.Close()
            fileReader = My.Computer.FileSystem.OpenTextFileReader(GAMEPATH & "Subjects\" & subList)

            For i As Integer = 0 To (popSize - 1) Step 1
                subIds(i) = fileReader.ReadLine()
                subjects(i) = New Subject(subIds(i))
            Next i

            fileReader.Close()
        End If

    End Sub

    '--------------------------------------------------------------------------------'
    '------------------------- add subject to the population ------------------------'
    '--------------------------------------------------------------------------------'
    Public Sub addSubejct(ByRef subject As Subject)
        Dim idLine = vbNewLine & subject.ID
        Dim oldSubjects() As Subject
        Dim oldSubIds() As String
        My.Computer.FileSystem.WriteAllText(GAMEPATH & "Subjects\" & subList, idLine, True)

        oldSubjects = subjects
        oldSubIds = subIds
        ReDim subjects(popSize)
        ReDim subIds(popSize)

        For i = 0 To (popSize - 1) Step 1
            subjects(i) = oldSubjects(i)
            subIds(i) = oldSubIds(i)
        Next i

        popSize += 1

        subjects(popSize - 1) = subject
        subIds(popSize - 1) = subject.ID
        'grpAssignment(subject, grp)
        subject.update()
    End Sub

End Class
