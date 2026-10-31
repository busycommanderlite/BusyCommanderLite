Imports System.IO
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Drawing
Imports System.Drawing.Imaging
Imports Spire.Pdf
Imports Spire.Pdf.Graphics
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class BrowserForm
    Private ReadOnly client As New HttpClient()
    Private _isConnected As Boolean = False
    Private _hasShownUserConsole As Boolean = False

    Public ReadOnly Property IsConnected As Boolean
        Get
            Return _isConnected
        End Get
    End Property

    Public myUserConsole As UserConsole

    Private Sub BrowserForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            GoServerInterop.StartServer()
        Catch ex As Exception
            MessageBox.Show("Failed to start Go server: " & ex.Message & vbCrLf & ex.StackTrace, "Server Error")
        End Try

        Me.TopMost = True
        pollTimer.Start()
        UpdateStatusAsync()
    End Sub

    Private Sub BrowserForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            e.Cancel = True
            Me.Hide()
        End If
    End Sub

    Public Sub OpenUserConsole()
        Try
            If myUserConsole Is Nothing OrElse myUserConsole.IsDisposed Then
                myUserConsole = New UserConsole()
            End If

            If myUserConsole.WindowState = FormWindowState.Minimized Then
                myUserConsole.WindowState = FormWindowState.Normal
            End If

            myUserConsole.TopMost = True
            myUserConsole.BringToFront()
            myUserConsole.Show()
            myUserConsole.Activate()
        Catch ex As Exception
        End Try
    End Sub

    Private Async Sub pollTimer_Tick(sender As Object, e As EventArgs) Handles pollTimer.Tick
        Await UpdateStatusAsync()
    End Sub

    Private Async Function UpdateStatusAsync() As Task
        Try
            Dim response = Await client.GetStringAsync("http://localhost:18080/status")
            Dim json = JObject.Parse(response)
            Dim status = json("status")?.ToString()
            Dim qrB64 = json("qr")?.ToString()

            If status = "LOGGED_IN" Then
                MyApp.LoggedIn = True
                _isConnected = True
                statusLabel.Text = "WhatsApp Logged In"
                statusLabel.ForeColor = Color.Green
                qrPictureBox.Image = Nothing
                pollTimer.Interval = 5000 ' Slow down polling

                If MyApp.GroupsDict Is Nothing Then
                    Try
                        Dim groupResp = Await client.GetStringAsync("http://localhost:18080/groups")
                        MyApp.GroupsDict = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Dictionary(Of String, String))(groupResp)
                    Catch ex As Exception
                        MyApp.GroupsDict = New Dictionary(Of String, String)()
                    End Try
                End If

                If Not _hasShownUserConsole Then
                    _hasShownUserConsole = True
                    OpenUserConsole()
                End If

                If Not MyApp.Debug_Mode Then
                    Me.Hide()
                End If

            ElseIf status = "WAITING_FOR_QR" Then
                MyApp.LoggedIn = False
                _isConnected = False
                _hasShownUserConsole = False
                statusLabel.Text = "WAIT..."
                statusLabel.ForeColor = Color.Blue
                pollTimer.Interval = 1000
                If Not String.IsNullOrEmpty(qrB64) Then
                    Try
                        Dim imageBytes = Convert.FromBase64String(qrB64)
                        Using ms As New MemoryStream(imageBytes)
                            qrPictureBox.Image = Image.FromStream(ms)
                        End Using
                    Catch
                    End Try
                End If

                If Not Me.Visible Then
                    Me.Show()
                    Me.BringToFront()
                End If
                If myUserConsole IsNot Nothing AndAlso Not myUserConsole.IsDisposed AndAlso myUserConsole.Visible Then
                    myUserConsole.Hide()
                End If

            ElseIf status = "CONNECTING" Then
                MyApp.LoggedIn = False
                _isConnected = False
                _hasShownUserConsole = False
                statusLabel.Text = "Connecting..."
                statusLabel.ForeColor = Color.DarkOrange
                pollTimer.Interval = 1000
                qrPictureBox.Image = Nothing
            Else
                MyApp.LoggedIn = False
                _isConnected = False
                _hasShownUserConsole = False
                statusLabel.Text = "WhatsApp Disconnected"
                statusLabel.ForeColor = Color.Red
            End If
        Catch ex As Exception
            MyApp.LoggedIn = False
            _isConnected = False
            _hasShownUserConsole = False
            statusLabel.Text = "Go Server Offline"
            statusLabel.ForeColor = Color.Red
            qrPictureBox.Image = Nothing
        End Try
    End Function

    ' =========================================================================
    ' PUBLIC METHODS PORTED TO GO SERVER (Currently only handles Text Messages)
    ' =========================================================================

    Public Sub SendTextMessage(MobileNumber As String, MessageText As String, Optional IsWID As Boolean = False)
        If Not _isConnected Then Return
        SendToGoServer(MobileNumber, MessageText)
    End Sub

    Public Shared Function ResolveDisplayName(partyName As String, partyNameHindi As String) As String
        Dim useKrutiDev As Boolean = True
        If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("UseKrutiDev") Then
            useKrutiDev = MyApp.KeyDict("UseKrutiDev").Equals("yes", StringComparison.OrdinalIgnoreCase)
        End If

        If useKrutiDev Then
            ' User enabled "Use KrutiDev in Print Name field":
            ' PrintName contains KrutiDev Hindi text. If provided and not equal to PartyName, convert to Unicode Hindi.
            If Not String.IsNullOrEmpty(partyNameHindi) AndAlso Not partyNameHindi.Equals(partyName, StringComparison.OrdinalIgnoreCase) Then
                Return ConvertKrutidevToUnicode(partyNameHindi)
            Else
                Return partyName
            End If
        Else
            ' User disabled "Use KrutiDev in Print Name field":
            ' PrintName is NOT in KrutiDev Hindi (it's English or plain text).
            ' Never call ConvertKrutidevToUnicode, otherwise English text gets corrupted into junk characters.
            If Not String.IsNullOrEmpty(partyNameHindi) Then
                Return partyNameHindi
            Else
                Return partyName
            End If
        End If
    End Function

    Public Sub SendJamaNotification(PartyName As String, PartyNameHindi As String, AmountString As String, Narration As String, JamaType As String, Balance As String)
        'Send jama notification whatsapp
        Dim reduce100x As Boolean = True
        If MyApp.KeyDict.ContainsKey("ReduceAmount100x") AndAlso
           MyApp.KeyDict("ReduceAmount100x").Equals("no", StringComparison.OrdinalIgnoreCase) Then
            reduce100x = False
        End If

        If reduce100x Then
            AmountString = (CDbl(AmountString) / 100.0).ToString("N2")
            Balance = (CDbl(Balance) / 100.0).ToString("N2")
        Else
            AmountString = CDbl(AmountString).ToString("N2")
            Balance = CDbl(Balance).ToString("N2")
        End If

        Dim content As String
        If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("JamaNotGrp") AndAlso MyApp.KeyDict("JamaNotGrp").Equals("NA", StringComparison.OrdinalIgnoreCase) Then
            Exit Sub
        End If
        Dim displayName As String = ResolveDisplayName(PartyName, PartyNameHindi)
        content = String.Format(DecodeString(MyApp.KeyDict("JamaNotificationText")), displayName, AmountString, JamaType, Narration, Balance)
        
        SendToGoServer(MyApp.KeyDict("JamaNotGrp"), content, isEphemeral:=True, expiration:=86400)
    End Sub

    Public Sub SendContactToJamaGroup(MobileNumber As String, PartyName As String)
        If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("JamaNotGrp") AndAlso Not MyApp.KeyDict("JamaNotGrp").Equals("NA", StringComparison.OrdinalIgnoreCase) Then
            SendContactToGoServer(MyApp.KeyDict("JamaNotGrp"), MobileNumber, PartyName, isEphemeral:=True, expiration:=86400)
        End If
    End Sub

    Public Sub SendHisab(MobileNumber As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Using hisabForm As New HisabForm()
            Dim commonWidth, gridHeight, partyLabelHeight, totalLabelHeight As Integer

            Dim bakiAmt As Integer = 0
            Dim jamaAmt As Integer = 0
            Dim currentAmt As Integer
            Dim daysOld As Integer

            If String.IsNullOrEmpty(HisabText) Then
                HisabText = LineSep
            End If

            hisabForm.PartyLabel.Text = ResolveDisplayName(PartyName, PartyNameHindi)
            For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
                Dim parts = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)
                If parts.Length = 3 Then
                    If Not parts(1).Trim().Equals("") Then
                        'Baki Entry
                        currentAmt = CInt(parts(1))
                        bakiAmt += currentAmt
                        Try
                            Dim dateStr = parts(0)
                            Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToCharArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                            daysOld = (Today - entryDate).Days
                        Catch
                            daysOld = 0
                        End Try
                        hisabForm.HisabDataGrid.Rows.Add(New String() {parts(0), CStr(currentAmt), "", daysOld & " दिन"})
                    ElseIf Not parts(2).Trim().Equals("") Then
                        'Jama Entry
                        currentAmt = CInt(parts(2))
                        jamaAmt += currentAmt
                        Try
                            Dim dateStr = parts(0)
                            Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToCharArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                            daysOld = (Today - entryDate).Days
                        Catch
                            daysOld = 0
                        End Try
                        hisabForm.HisabDataGrid.Rows.Add(New String() {parts(0), "", CStr(currentAmt), daysOld & " दिन"})
                    End If
                End If
            Next
            hisabForm.TotalLabel.Text = "कुल बाकी : " & (bakiAmt - jamaAmt)

            hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
            Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold), .BackColor = Color.FromArgb(230, 230, 230)}
            hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

            partyLabelHeight = hisabForm.PartyLabel.Height
            totalLabelHeight = hisabForm.TotalLabel.Height
            hisabForm.HisabDataGrid.Height = hisabForm.HisabDataGrid.ColumnHeadersHeight + hisabForm.HisabDataGrid.RowCount * hisabForm.HisabDataGrid.Rows(0).Height + 3
            commonWidth = hisabForm.HisabDataGrid.Width
            gridHeight = hisabForm.HisabDataGrid.Height
            hisabForm.HisabDataGrid.ClearSelection()

            Using bitMap As New Bitmap(commonWidth + 40, gridHeight + partyLabelHeight + totalLabelHeight + 40)
                Using g As Graphics = Graphics.FromImage(bitMap)
                    g.Clear(Color.White)
                End Using
                hisabForm.PartyLabel.DrawToBitmap(bitMap, New Rectangle(20, 20, commonWidth, partyLabelHeight))
                hisabForm.HisabDataGrid.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight, commonWidth, gridHeight))
                hisabForm.TotalLabel.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight + gridHeight, commonWidth, totalLabelHeight))

                SendMediaMessage(MobileNumber, bitMap)
            End Using
        End Using
    End Sub
    
    Public Sub PrintHisab(LongShort As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Dim hisabForm As New HisabForm

        Dim bakiAmt As Integer = 0
        Dim jamaAmt As Integer = 0
        Dim currentAmt As Integer
        Dim daysOld As Integer

        If String.IsNullOrEmpty(HisabText) Then
            HisabText = LineSep
        End If

        hisabForm.PartyLabel.Text = ResolveDisplayName(PartyName, PartyNameHindi)
        If String.Equals(LongShort, "Long", StringComparison.OrdinalIgnoreCase) Then
            hisabForm.LongSlip = True
        Else
            hisabForm.LongSlip = False
        End If
        For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
            Dim parts = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)
            If parts.Length = 3 Then
                If Not parts(1).Trim().Equals("") Then
                    'Baki Entry
                    currentAmt = CInt(parts(1))
                    bakiAmt += currentAmt
                    Try
                        Dim dateStr = parts(0)
                        Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                        daysOld = (Today - entryDate).Days
                    Catch
                        daysOld = 0
                    End Try
                    hisabForm.HisabDataGrid.Rows.Add(New String() {parts(0), CStr(currentAmt), "", daysOld & " दिन"})
                ElseIf Not parts(2).Trim().Equals("") Then
                    'Jama Entry
                    currentAmt = CInt(parts(2))
                    jamaAmt += currentAmt
                    Try
                        Dim dateStr = parts(0)
                        Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                        daysOld = (Today - entryDate).Days
                    Catch
                        daysOld = 0
                    End Try
                    hisabForm.HisabDataGrid.Rows.Add(New String() {parts(0), "", CStr(currentAmt), daysOld & " दिन"})
                End If
            End If
        Next
        hisabForm.TotalLabel.Text = "कुल बकाया : " & (bakiAmt - jamaAmt)

        hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
        Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold)}
        hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

        hisabForm.HisabDataGrid.ClearSelection()

        Try
            hisabForm.PrintDocument1.Print()
        Catch ex As Exception
            MsgBox("Error printing slip: " & ex.Message, MsgBoxStyle.Exclamation, "Print Error")
        Finally
            hisabForm.Dispose()
        End Try
    End Sub

    Public Sub SendReceipt(FileName As String, MobileNumber As String, PartyName As String, PartyNameHindi As String, AmountString As String, Narration As String, JamaType As String, Balance As String)
        Try
            GoServerInterop.LogToFile("BrowserForm.SendReceipt called: Mode=" & MyApp.KeyDict("SendReceiptToCustomer") & ", Mobile=" & MobileNumber & ", File=" & FileName)
            'Send receipt whatsapp
            'Check which form of receipt needs to be sent
            If MyApp.KeyDict("SendReceiptToCustomer").Equals("no") Then
                Exit Sub
            ElseIf MyApp.KeyDict("SendReceiptToCustomer").Equals("Print Format") Then
                SendFile(FileName, MobileNumber, "", "no", "no")
                Exit Sub
            End If
            'Code reaches here it means it is Customized Message
            Dim reduce100x As Boolean = True
            If MyApp.KeyDict.ContainsKey("ReduceAmount100x") AndAlso
               MyApp.KeyDict("ReduceAmount100x").Equals("no", StringComparison.OrdinalIgnoreCase) Then
                reduce100x = False
            End If

            If reduce100x Then
                AmountString = (CDbl(AmountString) / 100.0).ToString("N2")
                Balance = (CDbl(Balance) / 100.0).ToString("N2")
            Else
                AmountString = CDbl(AmountString).ToString("N2")
                Balance = CDbl(Balance).ToString("N2")
            End If

            Dim content As String
            Dim displayName As String = ResolveDisplayName(PartyName, PartyNameHindi)
            content = String.Format(DecodeString(MyApp.KeyDict("ReceiptText")), displayName, AmountString, JamaType, Narration, Balance)
            SendTextMessage(MobileNumber, content)
        Catch ex As Exception
            GoServerInterop.LogToFile("BrowserForm.SendReceipt Exception: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Public Sub SendSign(MobileNumber As String, PartyName As String, PartyNameHindi As String, NagAndGadi As String)
        NagAndGadi = NagAndGadi.Replace("[", "").Replace("]", "").Trim()
        If NagAndGadi.Split("@"c).Length >= 2 Then
            'Send whatsapp
            Dim content As String

            Dim displayName As String = ResolveDisplayName(PartyName, PartyNameHindi)
            content = String.Format(DecodeString(MyApp.KeyDict("SignText")), NagAndGadi.Split("@"c)(0), displayName, NagAndGadi.Split("@"c)(1))
            SendTextMessage(MobileNumber, content)
        End If
    End Sub

    Public Function PrintSign(SignContent As String, GadiNumber As String) As Boolean
        Dim signForm As New SignForm()
        Dim totalDag As Integer
        Dim PartyName, VchNo, DagCnt, PartyNameHindi As String

        If SignContent.Split(New String() {LineSep}, StringSplitOptions.None).Length = 0 Then
            Return False
        End If
        signForm.GadiText = If(String.IsNullOrWhiteSpace(GadiNumber), "", GadiNumber.Trim())
        signForm.DateText = Now.ToString("dddd dd-MM-yyyy hh:mm tt")
        signForm.DateLabel.Text = If(String.IsNullOrWhiteSpace(signForm.GadiText), signForm.DateText, signForm.GadiText & "   " & signForm.DateText)
        totalDag = 0
        For Each singleLine As String In SignContent.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 4 Then
                PartyName = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                VchNo = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1)
                DagCnt = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2)
                PartyNameHindi = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(3)
                Dim displayName As String = ResolveDisplayName(PartyName, PartyNameHindi)
                signForm.SignGrid.Rows.Add(New String() {DagCnt, "डाग", displayName})
                Dim dagVal As Integer = 0
                Integer.TryParse(DagCnt, dagVal)
                totalDag += dagVal
            End If
        Next
        signForm.TotalLabel.Text = "कुल " & totalDag & " डाग             दस्तखत:-"

        signForm.SignGrid.ClearSelection()
        
        Try
            signForm.PrintDocument1.Print()
        Catch ex As Exception
            GoServerInterop.LogToFile("PrintSign Exception: " & ex.Message & vbCrLf & ex.StackTrace)
            MsgBox("Error printing slip: " & ex.Message, MsgBoxStyle.Exclamation, "Print Error")
            Return False
        Finally
            signForm.Dispose()
        End Try
        Return True
    End Function

    Public Async Sub GetDP(MobileNumber As String)
        If Not _isConnected Then
            MsgBox("WhatsApp is not connected.", MsgBoxStyle.Exclamation, "BusyCommander")
            Return
        End If

        Try
            Dim jid = FormatWhatsAppJID(MobileNumber)
            If String.IsNullOrEmpty(jid) Then Return

            Dim response = Await client.GetStringAsync("http://localhost:18080/profile_pic?phone=" & Uri.EscapeDataString(jid))
            Dim json = JObject.Parse(response)
            Dim picUrl = json("url")?.ToString()

            If Not String.IsNullOrEmpty(picUrl) Then
                Dim imgBytes = Await client.GetByteArrayAsync(picUrl)
                Using ms As New MemoryStream(imgBytes)
                    Dim img = Image.FromStream(ms)
                    Using formDP As New DP(New Bitmap(img))
                        formDP.TopMost = True
                        formDP.StartPosition = FormStartPosition.CenterScreen
                        formDP.ShowDialog()
                    End Using
                End Using
            Else
                ' If no DP found, check if number is on WhatsApp at all
                Dim checkNum = jid.Split("@"c)(0)
                Dim chkResp = Await client.GetStringAsync("http://localhost:18080/check?number=" & checkNum)
                Dim chkJson = JObject.Parse(chkResp)
                Dim isValid = CBool(chkJson("isValid"))
                If isValid Then
                    MsgBox("Profile picture is not available (either not set or hidden by privacy settings).", MsgBoxStyle.Information, "WhatsApp")
                Else
                    MsgBox("This mobile number is not registered on WhatsApp.", MsgBoxStyle.Exclamation, "WhatsApp")
                End If
            End If
        Catch ex As Exception
            MsgBox("Error fetching profile picture: " & ex.Message, MsgBoxStyle.Exclamation, "BusyCommander")
        End Try
    End Sub

    Public Async Sub CheckNumber(MobileNumber As String, PostProcessing As String)
        If Not _isConnected Then Return

        Try
            Dim response = Await client.GetStringAsync("http://localhost:18080/check?number=" & MobileNumber)
            Dim json = JObject.Parse(response)
            Dim isValid = CBool(json("isValid"))

            If myUserConsole IsNot Nothing AndAlso Not myUserConsole.IsDisposed Then
                Dim targetColor = If(isValid, Color.Green, Color.Red)
                If PostProcessing = "CheckTB1" Then
                    myUserConsole.DN1Status.BackColor = targetColor
                ElseIf PostProcessing = "CheckTB2" Then
                    myUserConsole.DN2Status.BackColor = targetColor
                ElseIf PostProcessing = "CheckTB3" Then
                    myUserConsole.DN3Status.BackColor = targetColor
                ElseIf PostProcessing = "CheckTB4" Then
                    myUserConsole.DN4Status.BackColor = targetColor
                End If
            End If
        Catch
            ' Fallback to Red on error
            If myUserConsole IsNot Nothing AndAlso Not myUserConsole.IsDisposed Then
                If PostProcessing = "CheckTB1" Then
                    myUserConsole.DN1Status.BackColor = Color.Red
                ElseIf PostProcessing = "CheckTB2" Then
                    myUserConsole.DN2Status.BackColor = Color.Red
                ElseIf PostProcessing = "CheckTB3" Then
                    myUserConsole.DN3Status.BackColor = Color.Red
                ElseIf PostProcessing = "CheckTB4" Then
                    myUserConsole.DN4Status.BackColor = Color.Red
                End If
            End If
        End Try
    End Sub

    Public Function SendFile(FileName As String, MobileNumber As String, Amount As String, IsQR As String, IsBill As String) As Boolean
        Try
            GoServerInterop.LogToFile("SendFile called: File=" & FileName & ", Mobile=" & MobileNumber & ", IsBill=" & IsBill & ", IsQR=" & IsQR & ", SendBillAs=" & MyApp.KeyDict("SendBillAs"))
            If Not File.Exists(FileName) Then
                GoServerInterop.LogToFile("SendFile error: File does not exist: " & FileName)
                Return False
            End If

            ' If it's a Bill
            If IsBill.Equals("yes", StringComparison.OrdinalIgnoreCase) Then
                Dim sendAsImage = MyApp.KeyDict.ContainsKey("SendBillAs") AndAlso MyApp.KeyDict("SendBillAs").Equals("Image", StringComparison.OrdinalIgnoreCase)
                Dim sendCaption = MyApp.KeyDict.ContainsKey("SendBillCaption") AndAlso MyApp.KeyDict("SendBillCaption").Equals("yes", StringComparison.OrdinalIgnoreCase)
                Dim billCaption = If(sendCaption AndAlso MyApp.KeyDict.ContainsKey("BillCaption"), DecodeString(MyApp.KeyDict("BillCaption")), "")

                If sendAsImage AndAlso FileName.ToUpper().EndsWith(".PDF") Then
                    Dim pdf As New PdfDocument()
                    Try
                        pdf.LoadFromFile(FileName)
                        For pIdx As Integer = 0 To pdf.Pages.Count - 1
                            Dim rawImg = pdf.SaveAsImage(pIdx, PdfImageType.Bitmap, ImageDPI, CInt(ImageDPI / 4))
                            Dim bmp As New Bitmap(rawImg)
                            Dim cropped = CropImage(bmp)
                            Dim imgToSend = If(cropped, bmp)
                            If pIdx = 0 AndAlso Not String.IsNullOrEmpty(billCaption) Then
                                SendMediaMessageWithCaption(MobileNumber, imgToSend, billCaption)
                            Else
                                SendMediaMessage(MobileNumber, imgToSend)
                            End If
                            If cropped IsNot Nothing AndAlso cropped IsNot bmp Then
                                cropped.Dispose()
                            End If
                            bmp.Dispose()
                            rawImg.Dispose()
                        Next
                    Finally
                        pdf.Close()
                    End Try
                Else
                    If sendAsImage Then
                        Dim uploadImg = LoadAndCropImageFromFile(FileName)
                        Try
                            If Not String.IsNullOrEmpty(billCaption) Then
                                SendMediaMessageWithCaption(MobileNumber, uploadImg, billCaption)
                            Else
                                SendMediaMessage(MobileNumber, uploadImg)
                            End If
                        Finally
                            If uploadImg IsNot Nothing Then uploadImg.Dispose()
                        End Try
                    Else
                        SendPdfMessage(MobileNumber, FileName, billCaption)
                    End If
                End If
            ElseIf IsQR.Equals("yes", StringComparison.OrdinalIgnoreCase) Then
                If Not Amount.Equals("0") Then
                    Dim QRimg = CreateQRWithAmount(MyApp.KeyDict("PaymentAddress"), CInt(Amount))
                    If QRimg IsNot Nothing Then
                        Try
                            SendMediaMessage(MobileNumber, QRimg)
                        Finally
                            QRimg.Dispose()
                        End Try
                    Else
                        Dim uploadImg = LoadAndCropImageFromFile(FileName)
                        Try
                            SendMediaMessage(MobileNumber, uploadImg)
                        Finally
                            If uploadImg IsNot Nothing Then uploadImg.Dispose()
                        End Try
                    End If
                Else
                    Dim uploadImg = LoadAndCropImageFromFile(FileName)
                    Try
                        SendMediaMessage(MobileNumber, uploadImg)
                    Finally
                        If uploadImg IsNot Nothing Then uploadImg.Dispose()
                    End Try
                End If
            Else
                ' Receipt or generic file (IsBill = "no", IsQR = "no")
                Dim sendAsImage = MyApp.KeyDict.ContainsKey("SendBillAs") AndAlso MyApp.KeyDict("SendBillAs").Equals("Image", StringComparison.OrdinalIgnoreCase)
                If sendAsImage Then
                    Dim uploadImg = LoadAndCropImageFromFile(FileName)
                    Try
                        SendMediaMessage(MobileNumber, uploadImg)
                    Finally
                        If uploadImg IsNot Nothing Then uploadImg.Dispose()
                    End Try
                Else
                    SendPdfMessage(MobileNumber, FileName, "")
                End If
            End If

            Return True
        Catch ex As Exception
            GoServerInterop.LogToFile("SendFile Exception: " & ex.Message & vbCrLf & ex.StackTrace)
            Return False
        End Try
    End Function

    Private Function LoadAndCropImageFromFile(filePath As String) As Bitmap
        If filePath.ToUpper().EndsWith(".PDF") Then
            Dim pdf As New PdfDocument()
            Try
                pdf.LoadFromFile(filePath)
                Dim rawImg = pdf.SaveAsImage(0, PdfImageType.Bitmap, ImageDPI, CInt(ImageDPI / 4))
                Dim bmp As New Bitmap(rawImg)
                Dim cropped = CropImage(bmp)
                rawImg.Dispose()
                If cropped IsNot Nothing Then
                    bmp.Dispose()
                    Return cropped
                Else
                    Return bmp
                End If
            Finally
                pdf.Close()
            End Try
        Else
            Return New Bitmap(filePath)
        End If
    End Function

    Public Function SendFileFromHTTPServer(FileName As String, MobileNumber As String, MessageText As String) As Boolean
        If Not File.Exists(FileName) Then
            Return False
        End If
        Dim captionToSend = MessageText
        Dim sendCaption = MyApp.KeyDict.ContainsKey("SendBillCaption") AndAlso MyApp.KeyDict("SendBillCaption").Equals("yes", StringComparison.OrdinalIgnoreCase)
        If String.IsNullOrWhiteSpace(captionToSend) AndAlso sendCaption AndAlso MyApp.KeyDict.ContainsKey("BillCaption") Then
            captionToSend = DecodeString(MyApp.KeyDict("BillCaption"))
        End If
        SendPdfMessage(MobileNumber, FileName, captionToSend)
        Return True
    End Function

    Public Function SendImageFromHTTPServer(FileName As String, MobileNumber As String, MessageText As String) As Boolean
        If Not File.Exists(FileName) Then
            Return False
        End If

        Dim captionToSend = MessageText
        Dim sendCaption = MyApp.KeyDict.ContainsKey("SendBillCaption") AndAlso MyApp.KeyDict("SendBillCaption").Equals("yes", StringComparison.OrdinalIgnoreCase)
        If String.IsNullOrWhiteSpace(captionToSend) AndAlso sendCaption AndAlso MyApp.KeyDict.ContainsKey("BillCaption") Then
            captionToSend = DecodeString(MyApp.KeyDict("BillCaption"))
        End If

        Dim uploadImg = LoadAndCropImageFromFile(FileName)
        Try
            If Not String.IsNullOrEmpty(captionToSend) Then
                SendMediaMessageWithCaption(MobileNumber, uploadImg, captionToSend)
            Else
                SendMediaMessage(MobileNumber, uploadImg)
            End If
            Return True
        Finally
            If uploadImg IsNot Nothing Then uploadImg.Dispose()
        End Try
    End Function

    ' =========================================================================
    ' INTERNAL HTTP CALL TO GO SERVER
    ' =========================================================================
    Private Function FormatWhatsAppJID(phone As String) As String
        If String.IsNullOrWhiteSpace(phone) Then Return ""
        phone = phone.Trim()

        ' If it's a WhatsApp Group JID (e.g., Jama group 120363424008458963@g.us), return as-is
        If phone.EndsWith("@g.us", StringComparison.OrdinalIgnoreCase) Then
            Return phone
        End If

        ' If it has an existing domain suffix (@c.us or @s.whatsapp.net), strip it to normalize the number
        If phone.Contains("@") Then
            phone = phone.Split("@"c)(0)
        End If

        ' Extract numeric digits only
        Dim digits As New System.Text.StringBuilder()
        For Each ch As Char In phone
            If Char.IsDigit(ch) Then digits.Append(ch)
        Next
        Dim raw As String = digits.ToString()

        ' In India, mobile numbers are 10 digits (even if starting with 91, 98, 70, etc.).
        ' With country code 91, they are 12 digits.
        If raw.Length = 10 Then
            ' 10 digits -> ALWAYS prepend country code 91
            raw = COUNTRY_CODE & raw
        ElseIf raw.Length = 11 AndAlso raw.StartsWith("0") Then
            ' 11 digits with leading 0 (e.g. 09109172523) -> replace 0 with country code 91
            raw = COUNTRY_CODE & raw.Substring(1)
        ElseIf raw.Length = 12 AndAlso raw.StartsWith(COUNTRY_CODE) Then
            ' Already 12 digits starting with country code 91 -> keep as is
        ElseIf raw.Length > 10 AndAlso Not raw.StartsWith(COUNTRY_CODE) Then
            ' If longer than 10 but missing country code, take last 10 digits and prepend 91
            raw = COUNTRY_CODE & raw.Substring(raw.Length - 10)
        End If

        Return raw & "@s.whatsapp.net"
    End Function

    Public Async Function SendToGoServerAsync(phone As String, message As String, Optional isEphemeral As Boolean = False, Optional expiration As UInteger = 86400) As Task(Of Tuple(Of Boolean, String))
        Try
            phone = FormatWhatsAppJID(phone)
            If String.IsNullOrEmpty(phone) Then Return Tuple.Create(False, "Invalid phone number or JID.")

            GoServerInterop.LogToFile("Sending message to " & phone & " (Ephemeral=" & isEphemeral.ToString() & ", Exp=" & expiration & ")")
            Dim jobj As New JObject(
                New JProperty("phone", phone),
                New JProperty("message", message)
            )
            If isEphemeral Then
                jobj.Add("ephemeral", True)
                jobj.Add("expiration", expiration)
            End If

            Dim payload = jobj.ToString()
            Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync("http://localhost:18080/send", content)
            Dim respStr = Await resp.Content.ReadAsStringAsync()
            GoServerInterop.LogToFile("SendToGoServer to " & phone & " response: " & resp.StatusCode.ToString() & " - " & respStr)
            Return Tuple.Create(resp.IsSuccessStatusCode, respStr)
        Catch ex As Exception
            GoServerInterop.LogToFile("SendToGoServer Exception: " & ex.Message)
            Return Tuple.Create(False, ex.Message)
        End Try
    End Function

    Public Async Sub SendToGoServer(phone As String, message As String, Optional isEphemeral As Boolean = False, Optional expiration As UInteger = 86400)
        Await SendToGoServerAsync(phone, message, isEphemeral, expiration)
    End Sub

    Private Async Sub SendContactToGoServer(recipient As String, contactNumber As String, contactName As String, Optional isEphemeral As Boolean = False, Optional expiration As UInteger = 86400)
        Try
            recipient = FormatWhatsAppJID(recipient)
            If String.IsNullOrEmpty(recipient) Then Exit Sub

            GoServerInterop.LogToFile("Sending contact to " & recipient & ", Contact=" & contactName & " (" & contactNumber & ", Ephemeral=" & isEphemeral.ToString() & ")")
            Dim jobj As New JObject(
                New JProperty("recipient", recipient),
                New JProperty("phone", contactNumber),
                New JProperty("name", contactName)
            )
            If isEphemeral Then
                jobj.Add("ephemeral", True)
                jobj.Add("expiration", expiration)
            End If

            Dim payload = jobj.ToString()
            Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync("http://localhost:18080/send_contact", content)
            Dim respStr = Await resp.Content.ReadAsStringAsync()
            GoServerInterop.LogToFile("SendContactToGoServer to " & recipient & " response: " & resp.StatusCode.ToString() & " - " & respStr)
        Catch ex As Exception
            GoServerInterop.LogToFile("SendContactToGoServer Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub SendMediaMessage(MobileNumber As String, Media As Bitmap)
        Using MS As New MemoryStream()
            Media.Save(MS, ImageFormat.Jpeg)
            Dim b64media = Convert.ToBase64String(MS.ToArray())
            SendMediaToGoServer(MobileNumber, b64media, "image", "image/jpeg", "image.jpg", "")
        End Using
    End Sub

    Private Sub SendMediaMessageWithCaption(MobileNumber As String, Media As Bitmap, Caption As String)
        Using MS As New MemoryStream()
            Media.Save(MS, ImageFormat.Jpeg)
            Dim b64media = Convert.ToBase64String(MS.ToArray())
            SendMediaToGoServer(MobileNumber, b64media, "image", "image/jpeg", "image.jpg", Caption)
        End Using
    End Sub

    Private Sub SendPdfMessage(MobileNumber As String, FileName As String, Caption As String)
        Try
            GoServerInterop.LogToFile("SendPdfMessage called: File=" & FileName & ", Mobile=" & MobileNumber & ", Caption=" & If(String.IsNullOrEmpty(Caption), "<EMPTY>", Caption))
            If Not File.Exists(FileName) Then
                GoServerInterop.LogToFile("SendPdfMessage error: File does not exist: " & FileName)
                Exit Sub
            End If
            Dim fileBytes As Byte()
            Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using ms As New MemoryStream()
                    fs.CopyTo(ms)
                    fileBytes = ms.ToArray()
                End Using
            End Using
            Dim b64media = Convert.ToBase64String(fileBytes)
            Dim cleanFileName As String = System.Text.RegularExpressions.Regex.Replace(System.IO.Path.GetFileName(FileName), "[^A-Za-z0-9.]", "")
            SendMediaToGoServer(MobileNumber, b64media, "document", "application/pdf", cleanFileName, Caption)
        Catch ex As Exception
            GoServerInterop.LogToFile("SendPdfMessage Exception: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    Private Async Sub SendMediaToGoServer(phone As String, base64Data As String, mediaType As String, mimeType As String, fileName As String, caption As String, Optional isEphemeral As Boolean = False, Optional expiration As UInteger = 86400)
        Try
            phone = FormatWhatsAppJID(phone)
            If String.IsNullOrEmpty(phone) Then Exit Sub

            GoServerInterop.LogToFile("Sending media to " & phone & ", Type=" & mediaType & ", CaptionLen=" & If(caption Is Nothing, 0, caption.Length) & ", Ephemeral=" & isEphemeral.ToString())
            Dim jobj As New JObject(
                New JProperty("phone", phone),
                New JProperty("mimetype", mimeType),
                New JProperty("filename", fileName),
                New JProperty("caption", caption),
                New JProperty("base64", base64Data),
                New JProperty("type", mediaType)
            )
            If isEphemeral Then
                jobj.Add("ephemeral", True)
                jobj.Add("expiration", expiration)
            End If

            Dim payload = jobj.ToString()
            Dim content = New StringContent(payload, Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync("http://localhost:18080/send_media", content)
            Dim respStr = Await resp.Content.ReadAsStringAsync()
            GoServerInterop.LogToFile("SendMediaToGoServer to " & phone & " response: " & resp.StatusCode.ToString() & " - " & respStr)
        Catch ex As Exception
            GoServerInterop.LogToFile("SendMediaToGoServer Exception: " & ex.Message)
        End Try
    End Sub

    Public Async Sub SetDisappearingTimerToGoServer(phone As String, timerSeconds As UInteger)
        Try
            phone = FormatWhatsAppJID(phone)
            If String.IsNullOrEmpty(phone) Then Exit Sub

            GoServerInterop.LogToFile("Setting disappearing timer for " & phone & " to " & timerSeconds & "s")
            Dim jobj As New JObject(
                New JProperty("phone", phone),
                New JProperty("timer", timerSeconds)
            )
            Dim content = New StringContent(jobj.ToString(), Encoding.UTF8, "application/json")
            Dim resp = Await client.PostAsync("http://localhost:18080/set_disappearing", content)
            Dim respStr = Await resp.Content.ReadAsStringAsync()
            GoServerInterop.LogToFile("SetDisappearingTimerToGoServer response: " & resp.StatusCode.ToString() & " - " & respStr)
        Catch ex As Exception
            GoServerInterop.LogToFile("SetDisappearingTimerToGoServer Exception: " & ex.Message)
        End Try
    End Sub

End Class

