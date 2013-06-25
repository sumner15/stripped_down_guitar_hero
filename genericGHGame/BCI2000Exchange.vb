' tag: wadsworth -
'In the BCI2000Exchange.vb class, I made a few additions (see Revision 23). I essentially copied the constructor and Update functions
'in order to handle each of the current two games we are working with (rehabHero and riffHero). Of course, this isn't the cleanest 
'way to do it. I have a suspicion that with the new way we are doing things, we should be able to pass in the fingerBot class only to 
'the BCI2000Exchange constructor and its Update function. I don't want to go making that kind of high level reconstruction of the 
'BCI2000 class, though. I just added the new constructor and Update function for the time being, but these should be deleted in the 
'future, and BCI2000Exchange rewritten to handle several game types.  -Sumner

Imports BCI2000AutomationLib

Public Class BCI2000Exchange

#Region "Members"
    Public remote As BCI2000Remote
    Private modules(0 To 2) As String
    Private lastCall As Date
    Private lastSourceTime As Double
    Private Const stateScaling As Integer = 100000
    Private Const stateOffset As Integer = 0 'stateScaling
    Private samplesPerBlock As Double
    Private samplesPerSecond As Double
    Private blockDurationMsec As Double

    Private sampleBlockIndex As UInteger
    Private running As Boolean
    Private sourceTime As Double
    Private controlSignal As Double
    Private flamePos As Integer

    'Private Const timeRes As UInteger = 10 '10 msec is standard. This affects all processes on the OS
    'Declare Function timeBeginPeriod Lib "winmm.dll" (uPeriod As UInteger) As Integer
    'Declare Function timeEndPeriod Lib "winmm.dll" (uPeriod As UInteger) As Integer

    Private udpSender As System.Net.Sockets.UdpClient
    Private udpReceiver As System.Net.Sockets.UdpClient
    Private remoteIPEndPoint As System.Net.IPEndPoint
    Private listenerThread As System.Threading.Thread
    Private listenerLock As System.Threading.Mutex
    Private keepListening As Boolean
    Private watchMessage As String = ""

    Private operatorWindow As Boolean = False
    Private verbose As Boolean = False
    Private visualize As Boolean = True
    Private udpIncomingPort As Integer = 4567 ' specify port number, or 0 to use BCI2000Automation calls for incoming updates instead
    Private udpOutgoingPort As Integer = 5678 ' specify port number, or 0 to use BCI2000Automation calls for outgoing updates instead
    ' TODO: ideally we would get rid of the udp communication and use BCI2000Automation COM calls exclusively, making for much simpler vb code in this file, but currently each interpreter command takes too long to return (Juergen will try to fix this)
#End Region

    Private Sub Die(ByVal msg As String)
        Throw New Exception(msg)
    End Sub

    Private Sub Die()
        Die(remote.Result)
    End Sub

    Private Sub CheckInit()
        If remote Is Nothing Then Die("BCI2000Exchange object not initialized")
    End Sub

    Public Sub ExecuteScript(ByVal cmd As String)
        CheckInit()
        If remote.Execute(cmd) <> 0 Then Die() ' old convention: Execute() returns zero on success, unlike other methods
    End Sub

    Public Sub New(ByRef game As SongGame)
        'timeBeginPeriod(timeRes)
        remote = New BCI2000Remote()
        remote.WindowVisible = operatorWindow
        If Not remote.Connect() Then Die()

        ' Parameter handling 0: Define any BCI2000 parameters that this experiment will use
        ExecuteScript("ADD PARAMETER Application:SongGame   string    SongPath=                 %     % % %")
        ExecuteScript("ADD PARAMETER Application:SongGame   int       NumberOfNotes=            0     0 0 %")

        For i = 0 To game.fretboard.strings.Length - 1
            ExecuteScript("ADD STATE GuitarString" & (i + 1) & "TimeToNote 13 0")
        Next
        ExecuteScript("ADD STATE NextString  3 0")
        ExecuteScript("ADD STATE HitFeedback 3 0")

        modules(0) = "gUSBampSource32Release --local"  'TODO: get the 32-bit 3.12.00 DLL and replace the one that's currently in prog
        'modules(0) = "SignalGenerator --local" 'TODO: this line is for testing with a fake signal in the absence of actual EEG hardware - remove it!
        'modules(0) = modules(0) & " --FileFormat=Null"  'TODO: this line prevents EEG data from being saved to disk - remove it!

        modules(1) = "DummySignalProcessing --local" 'TODO: eventually, replace this with some real BCI signal processing (such as SpectralSignalProcessing) to do real BCI interaction
        modules(2) = "DummyApplication --local" ' this one can probably be left as is: the song game takes on the role of the application module
        If Not remote.StartupModules(modules) Then Die()


        ' Parameter handling 1: Set some initial defaults:  (note that .prm files will probably overrides these settings)
        ExecuteScript("SET PARAMETER SamplingRate   600")
        ExecuteScript("SET PARAMETER SampleBlockSize 30") ' defaults: 600Hz sample rate, 20Hz block rate (50ms blocks)
        ExecuteScript("SET PARAMETER VisualizeSource  1")
        ExecuteScript("SET PARAMETER VisualizeTiming  0")

        ' Parameter handling 2: Load parameter files
        ExecuteScript("LOAD PARAMETERFILE ../parms/fragments/amplifiers/ANTEGI.prm") 'Load the amplifier for HNL

        ' Parameter handling 3: set any "read-only" parameters that we don't want overwritten by carelessly saved overcomplete parameter files
        If udpOutgoingPort Then
            ExecuteScript("SET PARAMETER Connector:Connector%20Input list   ConnectorInputFilter=  1 *")
            ExecuteScript("SET PARAMETER Connector:Connector%20Input string ConnectorInputAddress=   localhost:" & udpOutgoingPort)
        Else
            ExecuteScript("SET PARAMETER Connector:Connector%20Input string ConnectorInputAddress=   %")
        End If

        'Console.WriteLine("Current subject: " & currentSub.ID)
        remote.SubjectID = currentSub.ID.Replace(" ", "")
        remote.SessionID = currentSub.lastSessionNumber.ToString("000.###")
        SetParameter("SongPath", game.mySong.songPath)
        SetParameter("NumberOfNotes", game.fretboard.numNotes)

        SetParameter("HgIdRehabHeroGame", "TODO") ' use Shell command - but how to get the text output?
        'ExecuteScript("SET PARAMETER HgIdTheBrainPart ""${system C:\Program Files\TortoiseHG\hg id}""") ' TODO: spaces in absolute path lead to escaping hell; without absolute path, does not find hg;
        ' TODO: in any case it would be better to do the hg id at compile time rather than run time - but that's complicated and dependency-ridden


        If visualize Then
            Dim expr As String
            'expr = "SET PARAMETER Filtering matrix Expressions= { PosF1 VelF1 Kp1 PosF2 VelF2 Kp2 } 1 "  ' TODO: how to change the channel labels output by the ExpressionFilter? these matrix row labels don't work
            'expr = expr & " 100*(FingerBotPosF1-" & stateOffset & ")/" & stateScaling
            'expr = expr & " 100*(FingerBotVelF1-" & stateOffset & ")/" & stateScaling
            'expr = expr & " 100*(FingerBotKp1-" & stateOffset & ")/" & stateScaling
            'expr = expr & " 100*(FingerBotPosF2-" & stateOffset & ")/" & stateScaling
            'expr = expr & " 100*(FingerBotVelF2-" & stateOffset & ")/" & stateScaling
            'expr = expr & " 100*(FingerBotKp2-" & stateOffset & ")/" & stateScaling
            ExecuteScript(expr)
            ExecuteScript("SET PARAMETER Filtering matrix Expressions= 5 1 GuitarString1TimeToNote GuitarString2TimeToNote GuitarString3TimeToNote GuitarString4TimeToNote GuitarString5TimeToNote")
            ExecuteScript("SET PARAMETER Filtering matrix Expressions= 2 1 NextString HitFeedback")
            ExecuteScript("SET PARAMETER VisualizeExpressionFilter 1")
            ExecuteScript("SET PARAMETER VisualizeSource 1")
            ExecuteScript("SET PARAMETER VisualizeTiming 1")
        End If

        If Not remote.SetConfig() Then Die()

        Dim tempStr As String = ""
        If Not remote.GetParameter("SamplingRate", tempStr) Then Die()
        samplesPerSecond = CDbl(tempStr)
        If Not remote.GetParameter("SampleBlockSize", tempStr) Then Die()
        samplesPerBlock = CDbl(tempStr)
        blockDurationMsec = 1000.0 * samplesPerBlock / samplesPerSecond

        If Not remote.Start() Then Die()

        If udpIncomingPort Then
            remote.Execute("WATCH Running SourceTime Signal(1,1) AT localhost:" & udpIncomingPort) ' NB: cannot do this in ExecuteScript(), because remote.Execute() returns non-zero from the WATCH command even when it succeeds (usually indicates failure, and so triggers an exception in ExecuteScript)
            udpReceiver = New System.Net.Sockets.UdpClient(udpIncomingPort)
            remoteIPEndPoint = New System.Net.IPEndPoint(System.Net.IPAddress.Any, udpIncomingPort)
            listenerThread = New System.Threading.Thread(AddressOf ReceiveMessages)
            listenerLock = New System.Threading.Mutex()
            keepListening = True
            listenerThread.Start()
        End If
        If udpOutgoingPort Then
            udpSender = New System.Net.Sockets.UdpClient()
            udpSender.Connect("localhost", udpOutgoingPort)
        End If

        lastSourceTime = -1.23 ' a value that would never be returned by an actual call to remote.GetStateVariable("SourceTime")
        lastCall = Now()
    End Sub


    Public Sub Close()
        If udpIncomingPort Then
            keepListening = False
            Threading.Thread.Sleep(100)
            udpReceiver.Close()
        End If
        If udpOutgoingPort Then
            udpSender.Close()
        End If
        If Not (remote Is Nothing) Then remote.Disconnect()
        remote = Nothing
        'timeEndPeriod(timeRes)
    End Sub

    Private Sub ReceiveMessages()
        While keepListening
            Dim raw As Byte() = udpReceiver.Receive(remoteIPEndPoint)
            If raw.Length Then
                listenerLock.WaitOne()
                watchMessage = System.Text.Encoding.ASCII.GetString(raw)
                listenerLock.ReleaseMutex()
            End If
            System.Threading.Thread.Yield()
        End While
    End Sub

    Private Sub Incoming()

        If udpIncomingPort Then
            listenerLock.WaitOne()
            Dim delims As Char() = {vbTab}
            Dim substrings As String() = watchMessage.Split(delims)
            If substrings.Length = 3 Or substrings.Length = 4 Then
                sampleBlockIndex = CUInt(substrings(0))
                running = CBool(substrings(1))
                sourceTime = CDbl(substrings(2))
                If substrings.Length >= 4 Then controlSignal = CDbl(substrings(3))
            End If
            listenerLock.ReleaseMutex()
        Else
            CheckInit()
            Dim systemState As String = "Running"
            If Not remote.GetSystemState(systemState) Then Die()
            running = (systemState = "Running")
            sourceTime = 0
            If Not running Then Return
            If Not remote.GetStateVariable("SourceTime", sourceTime) Then Die()
            If Not remote.GetControlSignal(1, 1, controlSignal) Then Die() 'TODO: to do true BCI interaction, put a real signal-processing module in place instead of DummySignalProcessing
        End If

    End Sub

    Public Sub SetState(ByVal name As String, ByVal value As Double)

        If udpOutgoingPort Then
            Dim raw As Byte() = System.Text.Encoding.ASCII.GetBytes(name & " " & value & vbLf)
            udpSender.Send(raw, raw.Length)
        Else
            CheckInit()
            If Not remote.SetStateVariable(name, value) Then Die()
        End If

    End Sub

    Public Function EscapeString(ByVal s As String) As String
        If s.Length = 0 Then Return "%" Else Return s.Replace("%", "%%").Replace(" ", "%20")
    End Function

    Public Sub SetParameter(ByVal name As String, ByVal value As String)
        CheckInit()
        ExecuteScript("SET PARAMETER " & name & " " & EscapeString(value))
    End Sub

    Public Sub SetParameter(ByVal fullParameterLine As String)
        CheckInit()
        ExecuteScript("SET PARAMETER " & fullParameterLine)
    End Sub

    Public Sub Update(ByRef game As SongGame)

        For i = 0 To game.fretboard.targets.Length - 1
            If game.fretboard.targets(i).GetFlameState() Then flamePos = i + 1
        Next

        CheckInit()

        Dim tStart, tEnd As Date
        Dim updatePeriod, incomingDuration, outgoingDuration As TimeSpan

        tStart = Now() : updatePeriod = tStart - lastCall : lastCall = tStart

        Incoming()

        tEnd = Now() : incomingDuration = tEnd - tStart : tStart = tEnd

        If sourceTime = lastSourceTime Then Return
        lastSourceTime = sourceTime
        ' TODO: Ideally BCI2000's interpreter needs to be altered to have the capability to set multiple states while ensuring that all the changes happen in the same SampleBlock.
        '       As it is, the sourceTime logic above should approximate this, but only if BCI2000's SampleBlock duration is at least twice the period with which this Sub gets called in the game update loop

        'Dim propGains As Single() = game.secondHand.getPropGains()
        ' TODO: gains are internally called Kp1 and Kp2, BUT it seems like this might be a different "1"/"2" convention from F1/F2 because
        ' right-/left-handedness seems to be handled differently. Therefore, check that BCI2000's Kp1 always goes with finger 1 and Kp2
        ' with finger 2, regardless of gravity.  If the convention really is different, name the BCI2000 states Kp1 and Kp2 differently.
        ' There are two places where this is important, both marked @@@
        'SetState("FingerBotPosF1", game.secondHand.posF1 * stateScaling + stateOffset)
        'SetState("FingerBotVelF1", game.secondHand.velF1 * stateScaling + stateOffset)
        'SetState("FingerBotKp1", propGains(0) * stateScaling + stateOffset)
        'SetState("FingerBotPosF2", game.secondHand.posF2 * stateScaling + stateOffset)
        'SetState("FingerBotVelF2", game.secondHand.velF2 * stateScaling + stateOffset)
        'SetState("FingerBotKp2", propGains(1) * stateScaling + stateOffset)
        'SetState("FingerBotTargetTime", game.secondHand.targetTime)
        SetState("HitFeedback", flamePos) : flamePos = 0

        Dim nextString As Integer = 0
        Dim minNoteTime As Double = 3600000
        For i = 0 To game.fretboard.strings.Length - 1
            Dim nn As Integer = game.fretboard.strings(i).nextNote
            Dim noteTime As Double = Math.Round(game.fretboard.strings(i).noteTimes(nn) - game.secondHand.targetTime)
            Dim nonNegativeNoteTime As Double = noteTime
            If nonNegativeNoteTime < 0 And nn < game.fretboard.strings(i).noteTimes.Length - 1 Then nonNegativeNoteTime = Math.Round(game.fretboard.strings(i).noteTimes(nn + 1) - game.secondHand.targetTime)
            If nonNegativeNoteTime >= 0 And nonNegativeNoteTime <= minNoteTime Then
                minNoteTime = nonNegativeNoteTime
                nextString = i + 1
            End If
            Const maxNoteTime As Double = 5000
            If noteTime < 0 Or noteTime > maxNoteTime Then noteTime = maxNoteTime + 1
            SetState("GuitarString" & (i + 1) & "TimeToNote", noteTime)
        Next
        SetState("NextString", nextString)


        tEnd = Now() : outgoingDuration = tEnd - tStart : tStart = tEnd

        If verbose Then
            Console.WriteLine("Source module is " & modules(0))
            Console.WriteLine("SamplingRate = " & samplesPerSecond & ";  SampleBlockSize = " & samplesPerBlock & ";   Block duration = " & blockDurationMsec & "msec")
            Console.Write("Previous event loop period was      ") : Console.WriteLine(updatePeriod)
            Console.Write("Performed incoming operations in    ") : Console.WriteLine(incomingDuration)
            Console.Write("Performed outgoing operations in    ") : Console.WriteLine(outgoingDuration)
            Console.Write("Console writes except for this one: ") : Console.WriteLine(Now() - tStart)
        End If

    End Sub

End Class
