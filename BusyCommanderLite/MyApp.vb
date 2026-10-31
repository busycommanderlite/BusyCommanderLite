Imports System.IO
Imports System.Web.Script.Serialization

<ComClass(MyApp.ClassId, MyApp.InterfaceId, MyApp.EventsId)>
Public Class MyApp

    Public Const ClassId As String = "8D4C6B55-9B51-4B53-BA58-3A1B2E16B6C1"
    Public Const InterfaceId As String = "A0E09E84-8A7F-4D67-9132-B8B51B84F8A2"
    Public Const EventsId As String = "F9262A84-2477-4B89-8F28-3B13A50D0B33"

    Public Const TempFolderPath As String = "C:\BusyWin\WhatsApp\Temp\"
    Public Const Port_Num As String = "54321"

    'Shared Variables Across Program
    Private Shared ReadOnly commandLock As New Object()
    Public Shared brForm As BrowserForm
    Public Shared myServer As BusyCommanderHttpServer

    Public Shared BillFormats As List(Of String)
    Public Shared ReceiptFormats As List(Of String)
    Public Shared PurcFormats As List(Of String)
    Public Shared GroupsDict As Dictionary(Of String, String)
    Public Shared KeyDict As Dictionary(Of String, String)
    Public Shared Debug_Mode As Boolean = False
    Public Shared LoggedIn As Boolean = False

    Public Function CloseServer() As Boolean
        SyncLock commandLock
            If brForm Is Nothing Then
                Return False
            End If
            If myServer IsNot Nothing Then
                myServer.Stop()
                myServer = Nothing
            End If
            brForm.Close()
            brForm = Nothing
            Return True
        End SyncLock
    End Function

    Public Function SetHttpServerEnabled(enabled As Boolean) As String
        Return ToggleHttpServer(enabled)
    End Function

    Public Shared Function ToggleHttpServer(enabled As Boolean) As String
        SyncLock commandLock
            If brForm IsNot Nothing AndAlso Not brForm.IsDisposed Then
                If enabled Then
                    If myServer Is Nothing Then
                        Try
                            myServer = New BusyCommanderHttpServer()
                            myServer.Start(Port_Num, TempFolderPath)
                            Return "started"
                        Catch ex As Exception
                            myServer = Nothing
                            Return "error: " & ex.Message
                        End Try
                    Else
                        Return "already_running"
                    End If
                Else
                    If myServer IsNot Nothing Then
                        Try
                            myServer.Stop()
                        Catch ex As Exception
                        Finally
                            myServer = Nothing
                        End Try
                        Return "stopped"
                    Else
                        Return "already_stopped"
                    End If
                End If
            End If
            Return "server_not_running"
        End SyncLock
    End Function

    Public Function IsMainReady() As Boolean
        If brForm Is Nothing Then
            Return False
        Else
            Return LoggedIn
        End If
    End Function

    Public Function IsServerRunning() As Boolean
        If brForm Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    Public Function ShowConsole() As Boolean
        Try
            If brForm Is Nothing OrElse brForm.IsDisposed Then
                Return False
            End If

            If brForm.InvokeRequired Then
                brForm.Invoke(Sub() brForm.OpenUserConsole())
            Else
                brForm.OpenUserConsole()
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function GetConfig(key As String) As String
        Try
            If KeyDict Is Nothing Then
                KeyDict = ReadConfigFile()
            End If
            If KeyDict IsNot Nothing AndAlso KeyDict.ContainsKey(key) Then
                Return KeyDict(key)
            End If
        Catch ex As Exception
        End Try
        Return ""
    End Function

    Public Function SetConfig(key As String, value As String) As Boolean
        Try
            If KeyDict Is Nothing Then
                KeyDict = ReadConfigFile()
            End If
            If KeyDict Is Nothing Then
                KeyDict = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            End If
            KeyDict(key) = value
            SaveConfigFile(KeyDict)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function GetBillFormat(Optional isPurc As Boolean = False) As String
        Dim formatList As List(Of String) = If(isPurc, PurcFormats, BillFormats)
        If formatList IsNot Nothing AndAlso formatList.Count > 0 Then
            Dim dataString = String.Join("~~~", formatList)
            Dim chosen = ShowOptionsBox("Formatted Bill", "Select Bill Print Format", dataString)
            If String.IsNullOrEmpty(chosen) OrElse chosen = "notrunning" Then
                Return "NA"
            End If
            Return chosen
        End If
        Return GetConfig("BillFormat")
    End Function

    Public Function PrintHisab(LongShort As String, PartyName As String, PartyNameHindi As String, HisabText As String) As Boolean
        If brForm Is Nothing Then
            Return False
        End If
        If String.IsNullOrEmpty(HisabText) Then
            HisabText = LineSep
        End If
        brForm.PrintHisab(LongShort, PartyName, PartyNameHindi, HisabText)
        Return True
    End Function

    Public Function PrintSign(SignContent As String, GadiNumber As String) As Boolean
        If brForm Is Nothing Then
            Return False
        End If
        brForm.PrintSign(SignContent, GadiNumber)
        Return True
    End Function

    Public Function SaveSignDispatches(SignContent As String, GivenGadi As String) As Boolean
        Return DispatchRepository.SaveDispatches(SignContent, GivenGadi)
    End Function

    Public Function ShowDispatchManager(Optional selectedVchNo As String = "") As String
        Dim form As New DispatchManagerForm()
        Using guard As New DialogFocusGuard()
            form.ShowDialog(guard.Owner)
        End Using
        Return form.UpdatedVchNo
    End Function


    Public Function UpdateSignTally(totalNag As Integer, billCount As Integer) As Boolean
        SignPillForm.ShowTally(totalNag, billCount)
        Return True
    End Function

    Public Function HideSignTally() As Boolean
        SignPillForm.HideTally()
        Return True
    End Function

    Public Function GetLatestActualGadiForParty(PartyName As String) As String
        Return DispatchRepository.GetLatestActualGadiForParty(PartyName)
    End Function

    Public Function GetDispatchInfo(vchNo As String) As String
        Return DispatchRepository.GetDispatchInfoSummary(vchNo)
    End Function

    Public Function SendHisab(MobileNumber As String, PartyName As String, PartyNameHindi As String, HisabText As String) As Boolean
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing Then
            Return False
        End If
        If String.IsNullOrEmpty(HisabText) Then
            HisabText = LineSep
        End If
        brForm.SendHisab(MobileNumber, PartyName, PartyNameHindi, HisabText)
        Return True
    End Function

    Public Function SendFile(FileName As String, MobileNumber As String, Amount As String, IsQR As String, IsBill As String) As Boolean
        If Not System.IO.File.Exists(FileName) OrElse New System.IO.FileInfo(FileName).Length = 0 Then
            MsgBox("Busy License may have become invalid. Please restart Busy Accounting Software and check your license dongle/softlock.", MsgBoxStyle.Critical, "Busy License Error")
            Return False
        End If
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing Then
            Return False
        End If
        brForm.SendFile(FileName, MobileNumber, Amount, IsQR, IsBill)
        Return True
    End Function

    Public Function SendJamaNotification(PartyName As String, PartyNameHindi As String, AmountString As String, Narration As String, JamaType As String, Balance As String) As Boolean
        If brForm Is Nothing Then
            Return False
        End If
        brForm.SendJamaNotification(PartyName, PartyNameHindi, AmountString, Narration, JamaType, Balance)
        Return True
    End Function

    Public Function SendReceipt(FileName As String, MobileNumber As String, PartyName As String, PartyNameHindi As String, AmountString As String, Narration As String, JamaType As String, Balance As String) As Boolean
        Try
            GoServerInterop.LogToFile("SendReceipt called: FileName=" & FileName & ", Mobile=" & MobileNumber & ", Party=" & PartyName)
            ' Check if a physical file is expected (Print Format) or if it's a text-only receipt ("NoFile")
            If Not String.IsNullOrEmpty(FileName) AndAlso Not FileName.Equals("NoFile", StringComparison.OrdinalIgnoreCase) Then
                ' Wait up to 3 seconds for the PDF to be fully generated by Busy
                Dim maxWaitMs As Integer = 3000
                Dim elapsed As Integer = 0
                While (Not System.IO.File.Exists(FileName) OrElse New System.IO.FileInfo(FileName).Length = 0) AndAlso elapsed < maxWaitMs
                    System.Threading.Thread.Sleep(100)
                    elapsed += 100
                End While

                If Not System.IO.File.Exists(FileName) OrElse New System.IO.FileInfo(FileName).Length = 0 Then
                    GoServerInterop.LogToFile("SendReceipt error: File not found or empty after " & elapsed & "ms: " & FileName)
                    MsgBox("Receipt file not found: " & FileName & vbCrLf & "Please check Receipt Print Format setting in UserConsole.", MsgBoxStyle.Critical, "Receipt File Error")
                    Return False
                End If
            End If

            MobileNumber = ExtractMobileNumber(MobileNumber)
            If MobileNumber = "" Then
                GoServerInterop.LogToFile("SendReceipt error: Extracted mobile number is empty")
                Return False
            End If
            If brForm Is Nothing Then
                GoServerInterop.LogToFile("SendReceipt error: brForm is Nothing")
                Return False
            End If
            brForm.SendReceipt(FileName, MobileNumber, PartyName, PartyNameHindi, AmountString, Narration, JamaType, Balance)
            Return True
        Catch ex As Exception
            GoServerInterop.LogToFile("SendReceipt Exception: " & ex.Message & vbCrLf & ex.StackTrace)
            Return False
        End Try
    End Function

    Public Function SendSign(MobileNumber As String, PartyName As String, PartyNameHindi As String, NagAndGadi As String) As Boolean
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing Then
            Return False
        End If
        brForm.SendSign(MobileNumber, PartyName, PartyNameHindi, NagAndGadi)
        Return True
    End Function

    Public Function SendTextMessage(MobileNumber As String, MessageText As String) As Boolean
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing Then
            Return False
        End If
        brForm.SendTextMessage(MobileNumber, MessageText)
        Return True
    End Function

    Public Function GetDP(MobileNumber As String) As Boolean
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing OrElse brForm.IsDisposed Then
            Return False
        End If
        If brForm.InvokeRequired Then
            brForm.BeginInvoke(Sub() brForm.GetDP(MobileNumber))
        Else
            brForm.GetDP(MobileNumber)
        End If
        Return True
    End Function

    Public Function SendContactToJamaGroup(MobileNumber As String, PartyName As String) As Boolean
        MobileNumber = ExtractMobileNumber(MobileNumber)
        If MobileNumber = "" Then Return False
        If brForm Is Nothing Then
            Return False
        End If
        brForm.SendContactToJamaGroup(MobileNumber, PartyName)
        Return True
    End Function

    Public Function GetExtractedMobileNumber(MobNum As String) As String
        Return ExtractMobileNumber(MobNum)
    End Function

    Private Class TimeoutWebClient
        Inherits System.Net.WebClient
        Protected Overrides Function GetWebRequest(uri As Uri) As System.Net.WebRequest
            Dim w As System.Net.WebRequest = MyBase.GetWebRequest(uri)
            w.Timeout = 5000 ' 5 seconds timeout
            Return w
        End Function
    End Class

    Private Shared hasCheckedDllUpdatesOnOpen As Boolean = False
    Private Shared ReadOnly initLock As New Object()

    Private Shared Sub TriggerBackgroundDllCheck()
        SyncLock initLock
            If Not hasCheckedDllUpdatesOnOpen Then
                hasCheckedDllUpdatesOnOpen = True
                OtaUpdater.StartBackgroundUpdateCheck()
            End If
        End SyncLock
    End Sub

    Private Function GetScriptFromFirebase(scriptName As String) As String
        TriggerBackgroundDllCheck()
        Try
            Dim url As String = "https://busymasterdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/bc_scripts/" & scriptName & ".json"
            Using client As New TimeoutWebClient()
                Dim response As String = client.DownloadString(url)
                If response = "null" Then Return "NETWORK_ERROR"
                ' Firebase returns strings wrapped in quotes
                If response.StartsWith("""") AndAlso response.EndsWith("""") Then
                    response = response.Substring(1, response.Length - 2)
                End If
                Dim decodedScript As String = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(response))
                
                ' CRITICAL FIX: Ensure the VBScript block inside <ReportScript> is properly XML Encoded 
                ' so Busy doesn't choke on <, >, & and silently abort parsing.
                Dim startTag As String = "<ReportScript>"
                Dim endTag As String = "</ReportScript>"
                Dim startIndex As Integer = decodedScript.IndexOf(startTag)
                Dim endIndex As Integer = decodedScript.IndexOf(endTag)
        
                If startIndex >= 0 AndAlso endIndex > startIndex Then
                    startIndex += startTag.Length
                    Dim scriptContent As String = decodedScript.Substring(startIndex, endIndex - startIndex)
                    
                    ' Unescape first just in case it's partially escaped, then fully escape
                    scriptContent = scriptContent.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&")
                    scriptContent = scriptContent.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
                    
                    decodedScript = decodedScript.Substring(0, startIndex) & scriptContent & decodedScript.Substring(endIndex)
                End If

                ' CRITICAL FIX: Force all newlines to CRLF and remove trailing whitespaces/newlines.
                ' VBScript's Trim() only removes spaces, so trailing \n causes invisible string mismatches!
                decodedScript = decodedScript.Trim().Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace(vbLf, vbCrLf)

                decodedScript = decodedScript.Replace("BusyCommander.", "BusyCommanderLite.")
                Return decodedScript
            End Using
        Catch ex As Exception
            Return "NETWORK_ERROR"
        End Try
    End Function

    Public Function GetBCAccount() As String
        Return GetScriptFromFirebase("BCAccount")
    End Function

    Public Function GetBCMain() As String
        Return GetScriptFromFirebase("BCMain")
    End Function

    Public Function GetBCItem() As String
        Return GetScriptFromFirebase("BCItem")
    End Function

    Public Function GetBCReceipt() As String
        Return GetScriptFromFirebase("BCReceipt")
    End Function

    Public Function GetBCRepListView() As String
        Dim script As String = GetScriptFromFirebase("BCRepListView")
        If script <> "NETWORK_ERROR" AndAlso Not String.IsNullOrWhiteSpace(script) Then
            ' Ensure KeyCode = 80 ('P') initializes hisabText just like KeyCode = 72 ('H') so 0 Hisab prints properly
            Dim targetPattern As String = "dagCount = 0" & vbCrLf & vbTab & vbTab & vbTab & "partyName = Replace(BusyForm.Controls(""lblTopInfo1"").Caption, ""Account : "", """")" & vbCrLf & vbTab & vbTab & vbTab & "For Each ListItem In BusyForm.Controls(""LV1"").ListItems"
            Dim replacementPattern As String = "dagCount = 0" & vbCrLf & vbTab & vbTab & vbTab & "partyName = Replace(BusyForm.Controls(""lblTopInfo1"").Caption, ""Account : "", """")" & vbCrLf & vbTab & vbTab & vbTab & "hisabText = LineSep" & vbCrLf & vbTab & vbTab & vbTab & "For Each ListItem In BusyForm.Controls(""LV1"").ListItems"
            If script.Contains(targetPattern) Then
                script = script.Replace(targetPattern, replacementPattern)
            End If
        End If
        Return script
    End Function

    Public Function GetBCRepListViewScript() As String
        Return ExtractScript(GetBCRepListView())
    End Function

    Public Function ProcessGKey(vchNo As String, partyName As String) As String
        FocusDiagnostics.LogSnapshot(String.Format("ProcessGKey ENTRY: VchNo='{0}', Party='{1}'", vchNo, partyName))
        Dim dispatches = DispatchRepository.GetDispatchesForVoucher(vchNo)
        If dispatches.Count = 0 Then
            FocusDiagnostics.Log("ProcessGKey: No dispatches found for voucher, returning NO_SIGN")
            Return "NO_SIGN"
        End If

        Dim unupdated = dispatches.Where(Function(d) String.IsNullOrWhiteSpace(d.ActualGadiNo)).OrderBy(Function(d) d.Id).ToList()
        If unupdated.Count = 0 Then
            ' All are updated
            Dim summary = DispatchRepository.GetDispatchInfoSummary(vchNo)
            Me.ShowMsg(summary, "Dispatch Details")
            FocusDiagnostics.Log("ProcessGKey: All dispatches updated, showing summary and returning CANCELLED")
            Return "CANCELLED"
        End If

        ' Process the first unupdated sign
        Dim firstUnupdated = unupdated(0)
        
        Dim prefill = DispatchRepository.GetLatestActualGadiForParty(partyName)
        If String.IsNullOrWhiteSpace(prefill) Then
            prefill = If(firstUnupdated.GivenGadiNo, "").Trim()
        End If
        
        Dim dialogTitle = "Assign Gadi"
        Dim subheading = partyName & Chr(13) & Chr(10) & "Nag: " & firstUnupdated.Nag
        
        Dim dlg As New BCDialogBox()
        Dim typedGadi As String = Nothing
        Using guard As New DialogFocusGuard()
            typedGadi = dlg.ShowDialogWithData(dialogTitle, subheading, prefill, guard.Owner)
        End Using
        
        If String.IsNullOrEmpty(typedGadi) Then
            FocusDiagnostics.Log("ProcessGKey: Gadi input cancelled or empty")
            Return "CANCELLED"
        End If
        
        Dim dataString = DispatchRepository.GetVehicleMatchingDataString(typedGadi)
        Dim optBox As New BCOptionsBox()
        Dim chosen As String = Nothing
        Using guard As New DialogFocusGuard()
            chosen = optBox.ShowDialogWithOptions("Dispatch Selection", "Records Matching Your Entry", dataString, guard.Owner)
        End Using
        If String.IsNullOrEmpty(chosen) Then
            FocusDiagnostics.Log("ProcessGKey: Option box selection cancelled or empty")
            Return "CANCELLED"
        End If
        typedGadi = chosen
        DispatchRepository.UpdateActualGadi(firstUnupdated.Id, typedGadi)
        FocusDiagnostics.Log(String.Format("ProcessGKey: Updated actual gadi to '{0}'", typedGadi))
        Return "DO_SEND|" & partyName & "|" & firstUnupdated.Nag & "|" & typedGadi
    End Function


    Private Function ExtractScript(xml As String) As String
        Try
            Dim startTag As String = "<ReportScript>"
            Dim endTag As String = "</ReportScript>"
            Dim startIndex As Integer = xml.IndexOf(startTag)
            If startIndex >= 0 Then
                startIndex += startTag.Length
                Dim endIndex As Integer = xml.IndexOf(endTag, startIndex)
                If endIndex >= 0 Then
                    Dim script As String = xml.Substring(startIndex, endIndex - startIndex)
                    script = script.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", """")
                    ' ExecuteGlobal cannot contain Option Explicit
                    script = script.Replace("Option Explicit", "")
                    Return script
                End If
            End If
        Catch
        End Try
        Return xml
    End Function

    Public Sub ShowProgressBox(title As String, message As String)
    End Sub

    Public Sub CloseProgressBox()
    End Sub

    Private Class WindowWrapper
        Implements IWin32Window

        Private ReadOnly _hwnd As IntPtr

        Public Sub New(handle As IntPtr)
            _hwnd = handle
        End Sub

        Public ReadOnly Property Handle As IntPtr Implements IWin32Window.Handle
            Get
                Return _hwnd
            End Get
        End Property
    End Class

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function GetForegroundWindow() As IntPtr
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function GetActiveWindow() As IntPtr
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function GetAncestor(hwnd As IntPtr, gaFlags As UInteger) As IntPtr
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function GetWindowThreadProcessId(hwnd As IntPtr, ByRef lpdwProcessId As UInteger) As UInteger
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hwnd As IntPtr) As Boolean
    End Function

    Friend Shared Sub RestoreFocus(owner As IWin32Window, Optional childControl As IntPtr = Nothing)
        FocusDiagnostics.LogSnapshot("RestoreFocus ENTRY", If(owner IsNot Nothing, owner.Handle, IntPtr.Zero))
        If owner IsNot Nothing AndAlso owner.Handle <> IntPtr.Zero Then
            Try
                Dim res = SetForegroundWindow(owner.Handle)
                FocusDiagnostics.Log(String.Format("RestoreFocus: SetForegroundWindow({0}) returned {1}", FocusDiagnostics.GetWindowInfo(owner.Handle), res))
                If childControl <> IntPtr.Zero AndAlso FocusDiagnostics.IsWindow(childControl) Then
                    FocusDiagnostics.SetFocus(childControl)
                    FocusDiagnostics.Log(String.Format("RestoreFocus: SetFocus({0})", FocusDiagnostics.GetWindowInfo(childControl)))
                End If
            Catch ex As Exception
                FocusDiagnostics.Log("RestoreFocus Exception: " & ex.Message)
            End Try
        End If
    End Sub

    Private Const GA_ROOT As UInteger = 2
    Private Const GA_ROOTOWNER As UInteger = 3

    Friend Shared Function GetActiveWindowOwner() As IWin32Window
        Dim hwnd As IntPtr = IntPtr.Zero
        Dim curProcId As UInteger = 0

        Try
            curProcId = CUInt(Process.GetCurrentProcess().Id)
        Catch
        End Try

        ' Tier 1: Try GetForegroundWindow()
        Dim fgHwnd As IntPtr = GetForegroundWindow()
        If fgHwnd <> IntPtr.Zero Then
            Dim fgProcId As UInteger = 0
            GetWindowThreadProcessId(fgHwnd, fgProcId)
            FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 1 - Foreground={0}", FocusDiagnostics.GetWindowInfo(fgHwnd)))
            If curProcId = 0 OrElse fgProcId = curProcId Then
                ' Belongs to Busy! Resolve root owner
                Dim rootHwnd As IntPtr = GetAncestor(fgHwnd, GA_ROOTOWNER)
                FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 1 - RootOwner={0}", FocusDiagnostics.GetWindowInfo(rootHwnd)))
                If rootHwnd <> IntPtr.Zero Then
                    hwnd = rootHwnd
                Else
                    hwnd = fgHwnd
                End If
            Else
                FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 1 FG belongs to different process ({0} vs {1})", fgProcId, curProcId))
            End If
        End If

        ' Tier 2: If Tier 1 didn't yield a Busy window (e.g. during a transition),
        ' try GetActiveWindow() on the current thread (Busy's UI thread)
        If hwnd = IntPtr.Zero Then
            Dim actHwnd As IntPtr = GetActiveWindow()
            FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 2 - ActiveWindow={0}", FocusDiagnostics.GetWindowInfo(actHwnd)))
            If actHwnd <> IntPtr.Zero Then
                Dim rootHwnd As IntPtr = GetAncestor(actHwnd, GA_ROOTOWNER)
                FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 2 - RootOwner={0}", FocusDiagnostics.GetWindowInfo(rootHwnd)))
                If rootHwnd <> IntPtr.Zero Then
                    hwnd = rootHwnd
                Else
                    hwnd = actHwnd
                End If
            End If
        End If

        ' Tier 3: In-process fallback to Busy's main application window handle
        If hwnd = IntPtr.Zero Then
            Try
                Dim mainHwnd As IntPtr = Process.GetCurrentProcess().MainWindowHandle
                FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 3 - MainWindowHandle={0}", FocusDiagnostics.GetWindowInfo(mainHwnd)))
                If mainHwnd <> IntPtr.Zero Then
                    hwnd = mainHwnd
                End If
            Catch ex As Exception
                FocusDiagnostics.Log("GetActiveWindowOwner: Tier 3 Exception: " & ex.Message)
            End Try
        End If

        ' Tier 4: Fallback to whatever GetForegroundWindow() returned
        If hwnd = IntPtr.Zero AndAlso fgHwnd <> IntPtr.Zero Then
            FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Tier 4 - Fallback to fgHwnd={0}", FocusDiagnostics.GetWindowInfo(fgHwnd)))
            hwnd = fgHwnd
        End If

        FocusDiagnostics.Log(String.Format("GetActiveWindowOwner: Final Owner HWND={0}", FocusDiagnostics.GetWindowInfo(hwnd)))
        If hwnd <> IntPtr.Zero Then
            Return New WindowWrapper(hwnd)
        End If
        Return Nothing
    End Function

    Public Function ShowTransliterator(MangalHindi As String) As String
        If KeyDict.ContainsKey("UseKrutiDev") AndAlso KeyDict("UseKrutiDev").Equals("no", StringComparison.OrdinalIgnoreCase) Then
            Return ""
        End If
        If brForm Is Nothing Then
            Return "notrunning"
        End If
        Dim form As New BCTransliterateForm()
        Dim result As String = Nothing
        Using guard As New DialogFocusGuard()
            result = form.ShowDialogWithData(MangalHindi, guard.Owner)
        End Using
        Return result
    End Function

    Public Function ShowTransliterateBox(MangalHindi As String) As String
        If brForm Is Nothing Then
            Return "notrunning"
        End If
        Dim form As New BCTransliterateForm()
        Dim result As String = Nothing
        Using guard As New DialogFocusGuard()
            result = form.ShowDialogWithData(MangalHindi, guard.Owner)
        End Using
        Return result
    End Function

    Public Function ShowOptionsBox(title As String, subtitle As String, dataString As String) As String
        If brForm Is Nothing Then
            Return "notrunning"
        End If
        FocusDiagnostics.LogSnapshot("ShowOptionsBox ENTRY: Title='" & title & "'")
        Dim form As New BCOptionsBox()
        Dim result As String = Nothing
        Using guard As New DialogFocusGuard()
            result = form.ShowDialogWithOptions(title, subtitle, dataString, guard.Owner)
        End Using
        FocusDiagnostics.Log(String.Format("ShowOptionsBox EXIT: Result='{0}'", result))
        Return result
    End Function

    Public Function ShowMobileBox(dataString As String) As String
        If brForm Is Nothing Then
            Return "notrunning"
        End If
        FocusDiagnostics.LogSnapshot("ShowMobileBox ENTRY: Data='" & dataString & "'")
        Dim form As New BCMobileBox()
        Dim result As String = Nothing
        Using guard As New DialogFocusGuard()
            result = form.ShowDialogWithMobiles(dataString, guard.Owner)
        End Using
        FocusDiagnostics.Log(String.Format("ShowMobileBox EXIT: Result='{0}'", result))
        Return result
    End Function

    Public Function ShowDialogBox(title As String, subheading As String, defaultValue As String, Optional maxLength As Integer = 0) As String
        If brForm Is Nothing Then
            Return "notrunning"
        End If
        FocusDiagnostics.LogSnapshot("ShowDialogBox ENTRY: Title='" & title & "' Subheading='" & subheading & "'")
        Dim form As New BCDialogBox()
        Dim result As String = Nothing
        Using guard As New DialogFocusGuard()
            result = form.ShowDialogWithData(title, subheading, defaultValue, guard.Owner, maxLength)
        End Using
        FocusDiagnostics.Log(String.Format("ShowDialogBox EXIT: Result='{0}'", result))
        Return result
    End Function

    Public Sub ShowShortcuts()
        If brForm Is Nothing Then
            Exit Sub
        End If
        FocusDiagnostics.LogSnapshot("ShowShortcuts ENTRY")
        Dim form As New ShortcutsForm
        Using guard As New DialogFocusGuard()
            form.ShowDialog(guard.Owner)
        End Using
        FocusDiagnostics.Log("ShowShortcuts EXIT")
    End Sub

    Public Sub StartRegistration()
        FocusDiagnostics.LogSnapshot("StartRegistration ENTRY")
        Dim regForm As New RegistrationForm()
        Using guard As New DialogFocusGuard()
            regForm.ShowDialog(guard.Owner)
        End Using
        FocusDiagnostics.Log("StartRegistration EXIT")
    End Sub

    Private Shared currentWaitBox As BCProgressBox = Nothing

    Public Sub BeginWait(Title As String, Message As String)
        Try
            EndWait()
            currentWaitBox = New BCProgressBox()
            currentWaitBox.StartWait(Title, Message)
        Catch ex As Exception
            FocusDiagnostics.Log("BeginWait error: " & ex.Message)
        End Try
    End Sub

    Public Sub SetWaitMessage(Message As String)
        Try
            If currentWaitBox IsNot Nothing Then
                currentWaitBox.SetMessage(Message)
            End If
        Catch ex As Exception
            FocusDiagnostics.Log("SetWaitMessage error: " & ex.Message)
        End Try
    End Sub

    Public Sub EndWait()
        Try
            If currentWaitBox IsNot Nothing Then
                currentWaitBox.StopWait()
                currentWaitBox = Nothing
            End If
        Catch ex As Exception
            FocusDiagnostics.Log("EndWait error: " & ex.Message)
        End Try
    End Sub

    Public Sub ShowSorryMsg(Msg As String, Title As String)
        EndWait()
        FocusDiagnostics.LogSnapshot("ShowSorryMsg ENTRY: Title='" & Title & "' Msg='" & Msg & "'")
        Dim box As New BCMessageBox()
        Using guard As New DialogFocusGuard()
            box.ShowAlert(Title, Msg, "Error", guard.Owner)
        End Using
        FocusDiagnostics.Log("ShowSorryMsg EXIT")
    End Sub

    Public Sub ShowHappyMsg(Msg As String, Title As String)
        EndWait()
        FocusDiagnostics.LogSnapshot("ShowHappyMsg ENTRY: Title='" & Title & "' Msg='" & Msg & "'")
        Dim box As New BCMessageBox()
        Using guard As New DialogFocusGuard()
            box.ShowAlert(Title, Msg, "Info", guard.Owner)
        End Using
        FocusDiagnostics.Log("ShowHappyMsg EXIT")
    End Sub

    Public Sub ShowMsg(Msg As String, Title As String)
        EndWait()
        FocusDiagnostics.LogSnapshot("ShowMsg ENTRY: Title='" & Title & "' Msg='" & Msg & "'")
        Dim box As New BCMessageBox()
        Using guard As New DialogFocusGuard()
            box.ShowAlert(Title, Msg, "Info", guard.Owner)
        End Using
        FocusDiagnostics.Log("ShowMsg EXIT")
    End Sub

    Public Sub Log(Msg As String)
        GoServerInterop.LogToFile(Msg)
    End Sub

    Public Function AskYesNo(Prompt As String, DefaultVal As Boolean, Unk As Boolean, Title As String) As Boolean
        EndWait()
        FocusDiagnostics.LogSnapshot(String.Format("AskYesNo ENTRY (4-arg): Title='{0}' Prompt='{1}'", Title, Prompt))
        Dim box As New BCMessageBox()
        Dim result As DialogResult
        Using guard As New DialogFocusGuard()
            result = box.ShowAlert(Title, Prompt, "YesNo", guard.Owner)
        End Using
        FocusDiagnostics.Log(String.Format("AskYesNo EXIT (4-arg): Result='{0}'", result))
        Return result = DialogResult.Yes
    End Function

    Public Function AskYesNo(Prompt As String, Title As String) As Boolean
        EndWait()
        FocusDiagnostics.LogSnapshot("AskYesNo ENTRY (2-arg): Title='" & Title & "' Prompt='" & Prompt & "'")
        Dim box As New BCMessageBox()
        Dim result As DialogResult
        Using guard As New DialogFocusGuard()
            result = box.ShowAlert(Title, Prompt, "YesNo", guard.Owner)
        End Using
        FocusDiagnostics.Log(String.Format("AskYesNo EXIT (2-arg): Result='{0}'", result))
        Return result = DialogResult.Yes
    End Function

    Public Function StartWhatsapp(busyLicenseNumber As String, compNameReceived As String, bFormats As String, rFormats As String, Optional pFormats As String = "") As String
        SyncLock commandLock
            If brForm IsNot Nothing Then
                Return "Server is already running"
            End If
            GoServerInterop.LogToFile("StartWhatsapp called: License=" & busyLicenseNumber & ", Company=" & compNameReceived & ", BillFormats=" & bFormats & ", ReceiptFormats=" & rFormats & ", PurcFormats=" & pFormats)
            
            ' 1. Apply any pending OTA updates (.new -> active)
            Dim updateMsg As String = OtaUpdater.ApplyPendingUpdates()
            If Not String.IsNullOrEmpty(updateMsg) Then
                ' Show notification serially on the main UI thread!
                ShowHappyMsg(updateMsg, "OTA Update")
            End If

            ' 2. Check if an OTA download is currently in progress
            If OtaUpdater.IsDownloadInProgress() Then
                GoServerInterop.LogToFile("StartWhatsapp: An OTA download is in progress, waiting serially...")
                BeginWait("BusyCommander Update", "Downloading latest update... Please wait.")
                Try
                    Dim dlTask = OtaUpdater.GetActiveDownloadTask()
                    If dlTask IsNot Nothing Then
                        Dim completed = dlTask.Wait(TimeSpan.FromSeconds(12))
                        EndWait()
                        If completed Then
                            ' Apply newly downloaded updates immediately
                            Dim freshUpdateMsg = OtaUpdater.ApplyPendingUpdates()
                            If Not String.IsNullOrEmpty(freshUpdateMsg) Then
                                ShowHappyMsg(freshUpdateMsg, "OTA Update")
                            End If
                        Else
                            ' Download taking longer than 12 seconds - postpone startup to prevent lock/freeze
                            ShowMsg("Update is taking longer to download and will finish in the background." & vbCrLf & _
                                    "Startup of BusyCommander has been postponed to avoid conflicts." & vbCrLf & _
                                    "You can start BusyCommander from the menu when ready.", "OTA Update")
                            Return "Update downloading in background. Please start server when ready."
                        End If
                    Else
                        EndWait()
                    End If
                Catch ex As Exception
                    EndWait()
                    GoServerInterop.LogToFile("StartWhatsapp: Error waiting for update: " & ex.Message)
                End Try
            End If

            BillFormats = New List(Of String)
            If Not bFormats.Equals("") Then
                For Each singleEntry In bFormats.Split(New String() {EntrySep}, StringSplitOptions.None)
                    If Not singleEntry.Trim().Equals("") Then
                        BillFormats.Add(singleEntry)
                    End If
                Next
            End If
            ReceiptFormats = New List(Of String)
            If Not rFormats.Equals("") Then
                For Each singleEntry In rFormats.Split(New String() {EntrySep}, StringSplitOptions.None)
                    If Not singleEntry.Trim().Equals("") Then
                        ReceiptFormats.Add(singleEntry)
                    End If
                Next
            End If
            PurcFormats = New List(Of String)
            If Not String.IsNullOrEmpty(pFormats) Then
                For Each singleEntry In pFormats.Split(New String() {EntrySep}, StringSplitOptions.None)
                    If Not String.IsNullOrWhiteSpace(singleEntry) Then
                        PurcFormats.Add(singleEntry.Trim())
                    End If
                Next
            End If
            'Check if server.dll is present
            Dim myDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            Dim serverDllPath = Path.Combine(myDir, "server.dll")
            If Not File.Exists(serverDllPath) Then
                Return "server.dll not found in installation folder"
            End If

            'Check if busy dll is present
            If Not File.Exists(Busy_DLL_Path) Then
                Return "DLL file not found"
            End If

            'Read config file
            KeyDict = ReadConfigFile()
            If KeyDict Is Nothing Then
                Return "Configuration file not found"
            End If

            'Check KeyDict Valid
            If Not KeyDict.ContainsKey("CompanyName") Then
                Return "Configuration file is corrupted"
            ElseIf Not KeyDict("CompanyName").Equals(compNameReceived) Then
                Return "Wrong CompanyName:" & KeyDict("CompanyName")
            End If

            Try
                'Initialize Browser
                brForm = New BrowserForm()
                'brForm.TopMost = True
                brForm.Show()

                'Check debug mode
                If File.Exists(Debug_File_Path) Then
                    Debug_Mode = True
                Else
                    Debug_Mode = False
                End If

                'Initialize HTTP Server
                If KeyDict.ContainsKey("EnableHTTPServer") AndAlso KeyDict("EnableHTTPServer").Equals("yes", StringComparison.OrdinalIgnoreCase) Then
                    If myServer Is Nothing Then
                        Try
                            myServer = New BusyCommanderHttpServer
                            myServer.Start(Port_Num, TempFolderPath)
                        Catch ex As Exception
                            myServer = Nothing
                        End Try
                    End If
                End If
                ' Check Firebase for OTA updates in background (completely non-blocking)
                OtaUpdater.StartBackgroundUpdateCheck()

                Return "ok"
            Catch ex As Exception
                Return ex.Message
            End Try
        End SyncLock
    End Function

End Class




