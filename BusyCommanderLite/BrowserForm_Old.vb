Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Script.Serialization
Imports Microsoft.Web.WebView2.Core
Imports Spire.Pdf
Imports Spire.Pdf.Graphics

Public Class BrowserForm
    Private eventsScript As String = "WPP.on('conn.main_ready', () => {var dict = {}; dict['busycommander'] = 'main_ready'; window.chrome.webview.postMessage(dict);}); WPP.on('conn.logout', () => {var dict = {}; dict['busycommander'] = 'logout'; window.chrome.webview.postMessage(dict);}); WPP.on('conn.online', (online) => { var dict = {}; if (online) {dict['busycommander'] = 'online';} else {dict['busycommander'] = 'offline';} window.chrome.webview.postMessage(dict);});"

    Public myUserConsole As UserConsole

    Private Async Sub BrowserForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Start server
        MyApp.LoggedIn = False
        Dim cwv2Environment = Await CoreWebView2Environment.CreateAsync(Nothing, Whatsapp_Data_Path, Nothing)
        Await myWebView.EnsureCoreWebView2Async(cwv2Environment)
    End Sub

    Private Sub MyWebView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles myWebView.NavigationCompleted
        Dim WPPScript = File.ReadAllText(WPP_File_Path)
        If myWebView.Source.ToString().Contains("web.whatsapp.com") Then
            myWebView.CoreWebView2.ExecuteScriptAsync(WPPScript & eventsScript)

            ' 1. If local CoreScript.js exists, inject immediately (zero delay)
            If File.Exists(CoreScript_File_Path) Then
                Try
                    Dim localCore = File.ReadAllText(CoreScript_File_Path)
                    myWebView.CoreWebView2.ExecuteScriptAsync(localCore)
                Catch
                End Try
            End If


        End If
    End Sub

    Private Sub MyWebView_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs) Handles myWebView.WebMessageReceived
        Dim receivedDict = New JavaScriptSerializer().
                        Deserialize(Of Dictionary(Of String, String))(e.WebMessageAsJson)
        If receivedDict("busycommander").Equals("main_ready") Then
            MyApp.LoggedIn = True
            GetGroups()
        ElseIf receivedDict("busycommander").Equals("main_ready_online") Then
            MyApp.LoggedIn = True
        ElseIf receivedDict("busycommander").Equals("offline") Then
            MyApp.LoggedIn = False
        ElseIf receivedDict("busycommander").Equals("online") Then
            MyApp.LoggedIn = False
            Dim commandScript = "var dict = {}; if (WPP.conn.isMainReady()) {dict['busycommander'] = 'main_ready_online';} window.chrome.webview.postMessage(dict);"
            ExecuteScript(commandScript)
        ElseIf receivedDict("busycommander").Equals("logout") Then
            MyApp.LoggedIn = False
            If myUserConsole IsNot Nothing Then
                myUserConsole.Close()
                myUserConsole.Dispose()
                myUserConsole = Nothing
            End If
            BringToFront()
            Show()
        ElseIf receivedDict("busycommander").Equals("getGrps") Then
            receivedDict.Remove("busycommander")
            MyApp.GroupsDict = receivedDict
            If myUserConsole Is Nothing Then
                myUserConsole = New UserConsole()
            End If
            myUserConsole.TopMost = True
            TopMost = False
            myUserConsole.BringToFront()
            If Not MyApp.Debug_Mode Then
                Hide()
            End If
            myUserConsole.Show()
        ElseIf receivedDict("busycommander").Equals("urlDP") Then
            If Not receivedDict("urlDP").Equals("undefined") Then
                Dim t As New Timer() With {.Interval = 100}
                AddHandler t.Tick,
                    Sub()
                        t.Dispose()
                        Using formDP As New DP(receivedDict("urlDP"))
                            formDP.TopMost = True
                            formDP.StartPosition = FormStartPosition.CenterScreen
                            formDP.ShowDialog()
                        End Using
                    End Sub
                t.Start()
            Else
                CheckNumber(receivedDict("mobilenumber"), "warnexists")
            End If
        ElseIf receivedDict("busycommander").Equals("checkNumber") Then
            If receivedDict("postprocessing").Equals("CheckTB1") Then
                If receivedDict("checkNumber").ToUpper().Equals("TRUE") Then
                    myUserConsole.DN1Status.BackColor = Color.Green
                Else
                    myUserConsole.DN1Status.BackColor = Color.Red
                End If
            ElseIf receivedDict("postprocessing").Equals("CheckTB2") Then
                If receivedDict("checkNumber").ToUpper().Equals("TRUE") Then
                    myUserConsole.DN2Status.BackColor = Color.Green
                Else
                    myUserConsole.DN2Status.BackColor = Color.Red
                End If
            ElseIf receivedDict("postprocessing").Equals("CheckTB3") Then
                If receivedDict("checkNumber").ToUpper().Equals("TRUE") Then
                    myUserConsole.DN3Status.BackColor = Color.Green
                Else
                    myUserConsole.DN3Status.BackColor = Color.Red
                End If
            ElseIf receivedDict("postprocessing").Equals("CheckTB4") Then
                If receivedDict("checkNumber").ToUpper().Equals("TRUE") Then
                    myUserConsole.DN4Status.BackColor = Color.Green
                Else
                    myUserConsole.DN4Status.BackColor = Color.Red
                End If
            ElseIf receivedDict("postprocessing").Equals("warnexists") Then
                If Not receivedDict("checkNumber").ToUpper().Equals("TRUE") Then
                    MsgBox("Number not on WhatsApp", MsgBoxStyle.Information)
                Else
                    MsgBox("User has no profile picture or it is hidden by privacy settings.", MsgBoxStyle.Information, "No DP")
                End If
            End If
        ElseIf receivedDict("busycommander").Equals("debug") Then
            MsgBox(receivedDict("msg"), MsgBoxStyle.Information, "Debug Info")
        End If
    End Sub

    Public Sub SendTextMessage(MobileNumber As String, MessageText As String, Optional IsWID As Boolean = False)
        If MyApp.LoggedIn Then
            Dim MyWid As String
            If Not IsWID Then
                MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Else
                MyWid = MobileNumber
            End If
            Dim commandScript = "BC_SendText('$mywid$', '$mymsg$');"
            MessageText = MessageText.Replace(vbCr, "\n")
            MessageText = MessageText.Replace(vbLf, "")
            MessageText = MessageText.Replace(vbCrLf, "")
            MessageText = MessageText.Replace("'", "")
            MessageText = MessageText.Replace("""", "")
            ExecuteScript(commandScript.Replace("$mywid$", MyWid).Replace("$mymsg$", MessageText))
        End If
    End Sub

    Private Sub SendMediaMessage(MobileNumber As String, Media As Bitmap)
        If MyApp.LoggedIn Then
            Dim MS = New MemoryStream()
            Media.Save(MS, ImageFormat.Jpeg)
            Dim b64media = Convert.ToBase64String(MS.ToArray())
            Dim MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Dim isHD As String
            If b64media.Length < 450000 Then
                isHD = "false"
            Else
                isHD = "true"
            End If
            Dim commandScript = "BC_SendFile('$mywid$', '$b64img$', 'image', '', '', " & isHD & ");"
            ExecuteScript(commandScript.Replace("$mywid$", MyWid).Replace("$b64img$", b64media))
        End If
    End Sub

    Private Sub SendMediaMessageWithCaption(MobileNumber As String, Media As Bitmap, Caption As String)
        If MyApp.LoggedIn Then
            Dim MS = New MemoryStream()
            Media.Save(MS, ImageFormat.Jpeg)
            Dim b64media = Convert.ToBase64String(MS.ToArray())
            Dim MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Caption = Caption.Replace(vbCr, "\n")
            Caption = Caption.Replace(vbLf, "")
            Caption = Caption.Replace(vbCrLf, "")
            Caption = Caption.Replace("'", "")
            Caption = Caption.Replace("""", "")

            Dim isHD As String
            If b64media.Length < 450000 Then
                isHD = "false"
            Else
                isHD = "true"
            End If

            Dim commandScript = "BC_SendFile('$mywid$', '$b64img$', 'image', '$mycaption$', '', " & isHD & ");"
            ExecuteScript(commandScript _
                .Replace("$mywid$", MyWid) _
                .Replace("$b64img$", b64media) _
                .Replace("$mycaption$", Caption))
        End If
    End Sub

    Private Sub SendPdfMessage(MobileNumber As String, FileName As String, Caption As String)
        If MyApp.LoggedIn Then
            Dim b64media = Convert.ToBase64String(File.ReadAllBytes(FileName))
            Dim MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Caption = Caption.Replace(vbCr, "\n")
            Caption = Caption.Replace(vbLf, "")
            Caption = Caption.Replace(vbCrLf, "")
            Caption = Caption.Replace("'", "")
            Caption = Caption.Replace("""", "")
            Dim cleanFileName As String = Regex.Replace(System.IO.Path.GetFileName(FileName), "[^A-Za-z0-9]", "")
            Dim commandScript = "BC_SendFile('$mywid$', '$b64img$', 'document', '$mycaption$', '" & cleanFileName & "', false);"
            ExecuteScript(commandScript _
                .Replace("$mywid$", MyWid) _
                .Replace("$b64img$", b64media) _
                .Replace("$mycaption$", Caption))
        End If
    End Sub

    Private Sub SendContact(RecepientWid As String, ContactNumber As String, ContactName As String)
        If MyApp.LoggedIn Then
            Dim ContactWid = COUNTRY_CODE & ContactNumber & "@c.us"
            ContactName = ContactName.Replace(vbCr, "\n")
            ContactName = ContactName.Replace(vbLf, "")
            ContactName = ContactName.Replace(vbCrLf, "")
            ContactName = ContactName.Replace("'", "")
            ContactName = ContactName.Replace("""", "")

            Dim commandScript = "BC_SendContact('$recepientwid$', '$contactwid$', '$contactname$');"
            ExecuteScript(commandScript _
                .Replace("$recepientwid$", RecepientWid) _
                .Replace("$contactwid$", ContactWid) _
                .Replace("$contactname$", ContactName))
        End If
    End Sub

    Private Sub GetGroups()
        If MyApp.LoggedIn Then
            ExecuteScript("BC_GetGroups();")
        End If
    End Sub

    Public Sub GetDP(MobileNumber As String)
        If MyApp.LoggedIn And CheckMobileNumberFormat(MobileNumber) Then
            Dim MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Dim commandScript = "BC_GetDP('$mywid$', '$mymobile$');"
            ExecuteScript(commandScript.Replace("$mywid$", MyWid).Replace("$mymobile$", MobileNumber))
        End If
    End Sub

    Public Sub CheckNumber(MobileNumber As String, PostProcessing As String)
        If MyApp.LoggedIn Then
            Dim MyWid = COUNTRY_CODE & MobileNumber & "@c.us"
            Dim commandScript = "BC_CheckNumber('$mywid$', '$postprocessing$');"
            ExecuteScript(commandScript.Replace("$mywid$", MyWid).Replace("$postprocessing$", PostProcessing))
        End If
    End Sub

    Private Sub ExecuteScript(scriptStr As String)
        If myWebView.InvokeRequired Then
            myWebView.BeginInvoke(New Action(Of String)(AddressOf ExecuteScript), scriptStr)
            Return
        End If

        myWebView.CoreWebView2.ExecuteScriptAsync(scriptStr)
    End Sub

    Private Sub MyWebView_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles myWebView.CoreWebView2InitializationCompleted
        myWebView.CoreWebView2.Navigate("https://web.whatsapp.com")
    End Sub

    Public Function SendFile(FileName As String, MobileNumber As String, Amount As String, IsQR As String, IsBill As String) As Boolean
        Dim UploadImage As Bitmap

        If Not File.Exists(FileName) Then
            Return False
        End If
        If FileName.ToUpper().EndsWith(".PDF") Then
            Dim pdf As New PdfDocument
            pdf.LoadFromFile(FileName)
            Dim inputImageRaw = pdf.SaveAsImage(0, PdfImageType.Bitmap, ImageDPI, CInt(ImageDPI / 4))
            Dim inputImage = New Bitmap(inputImageRaw)
            Dim croppedImage = CropImage(inputImage)
            If croppedImage Is Nothing Then
                UploadImage = inputImage
            Else
                UploadImage = croppedImage
            End If
        Else
            UploadImage = New Bitmap(FileName)
        End If
        'Send whatsapp
        If IsBill.Equals("yes") Then
            If MyApp.KeyDict("SendBillAs").Equals("Image") AndAlso FileName.ToUpper().EndsWith(".PDF") Then
                Dim pdf As New PdfDocument
                pdf.LoadFromFile(FileName)
                Dim captionText = If(MyApp.KeyDict("SendBillCaption").Equals("yes"), DecodeString(MyApp.KeyDict("BillCaption")), "")
                For pIdx As Integer = 0 To pdf.Pages.Count - 1
                    Dim rawImg = pdf.SaveAsImage(pIdx, PdfImageType.Bitmap, ImageDPI, CInt(ImageDPI / 4))
                    Dim bmp As New Bitmap(rawImg)
                    Dim cropped = CropImage(bmp)
                    Dim imgToSend = If(cropped, bmp)
                    If pIdx = 0 AndAlso Not String.IsNullOrEmpty(captionText) Then
                        SendMediaMessageWithCaption(MobileNumber, imgToSend, captionText)
                    Else
                        SendMediaMessage(MobileNumber, imgToSend)
                    End If
                Next
            Else
                If MyApp.KeyDict("SendBillCaption").Equals("yes") Then
                    If MyApp.KeyDict("SendBillAs").Equals("Image") Then
                        SendMediaMessageWithCaption(MobileNumber, UploadImage, DecodeString(MyApp.KeyDict("BillCaption")))
                    Else
                        SendPdfMessage(MobileNumber, FileName, DecodeString(MyApp.KeyDict("BillCaption")))
                    End If
                Else
                    If MyApp.KeyDict("SendBillAs").Equals("Image") Then
                        SendMediaMessage(MobileNumber, UploadImage)
                    Else
                        SendPdfMessage(MobileNumber, FileName, "")
                    End If
                End If
            End If
        ElseIf IsQR.Equals("yes") Then
            If Not Amount.Equals("0") Then
                Dim QRimg = CreateQRWithAmount(MyApp.KeyDict("PaymentAddress"), CInt(Amount))
                If QRimg IsNot Nothing Then
                    SendMediaMessage(MobileNumber, QRimg)
                Else
                    SendMediaMessage(MobileNumber, UploadImage)
                End If
            Else
                SendMediaMessage(MobileNumber, UploadImage)
            End If
        Else
            If MyApp.KeyDict("SendBillAs").Equals("Image") Then
                SendMediaMessage(MobileNumber, UploadImage)
            Else
                SendPdfMessage(MobileNumber, FileName, "")
            End If
        End If

        Return True
    End Function

    Public Function SendFileFromHTTPServer(FileName As String, MobileNumber As String, MessageText As String) As Boolean
        If Not File.Exists(FileName) Then
            Return False
        End If
        SendPdfMessage(MobileNumber, FileName, MessageText)
        Return True
    End Function

    Public Function SendImageFromHTTPServer(FileName As String, MobileNumber As String, MessageText As String) As Boolean
        If Not File.Exists(FileName) Then
            Return False
        End If

        Dim UploadImage As Bitmap
        Dim pdf As New PdfDocument
        pdf.LoadFromFile(FileName)
        Dim inputImageRaw = pdf.SaveAsImage(0, PdfImageType.Bitmap, ImageDPI, CInt(ImageDPI / 4))
        Dim inputImage = New Bitmap(inputImageRaw)
        Dim croppedImage = CropImage(inputImage)
        If croppedImage Is Nothing Then
            UploadImage = inputImage
        Else
            UploadImage = croppedImage
        End If

        SendMediaMessageWithCaption(MobileNumber, UploadImage, MessageText)
        Return True
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
        If MyApp.KeyDict("JamaNotGrp").Equals("NA") Then
            Exit Sub
        End If
        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            content = String.Format(DecodeString(MyApp.KeyDict("JamaNotificationText")), PartyNameHindi, AmountString, JamaType, Narration, Balance)
        Else
            content = String.Format(DecodeString(MyApp.KeyDict("JamaNotificationText")), PartyName, AmountString, JamaType, Narration, Balance)
        End If
        SendTextMessage(MyApp.KeyDict("JamaNotGrp"), content, True)
    End Sub

    Public Sub SendReceipt(FileName As String, MobileNumber As String, PartyName As String, PartyNameHindi As String, AmountString As String, Narration As String, JamaType As String, Balance As String)
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
        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            content = String.Format(DecodeString(MyApp.KeyDict("ReceiptText")), PartyNameHindi, AmountString, JamaType, Narration, Balance)
        Else
            content = String.Format(DecodeString(MyApp.KeyDict("ReceiptText")), PartyName, AmountString, JamaType, Narration, Balance)
        End If
        SendTextMessage(MobileNumber, content)
    End Sub

    Public Sub SendSign(MobileNumber As String, PartyName As String, PartyNameHindi As String, NagAndGadi As String)
        NagAndGadi = NagAndGadi.Replace("[", "").Replace("]", "").Trim()
        If NagAndGadi.Split("@"c).Length >= 2 Then
            'Send whatsapp
            Dim content As String

            'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
            If Not PartyNameHindi.Equals(PartyName) Then
                PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
                content = String.Format(DecodeString(MyApp.KeyDict("SignText")), NagAndGadi.Split("@"c)(0), PartyNameHindi, NagAndGadi.Split("@"c)(1))
            Else
                content = String.Format(DecodeString(MyApp.KeyDict("SignText")), NagAndGadi.Split("@"c)(0), PartyName, NagAndGadi.Split("@"c)(1))
            End If
            SendTextMessage(MobileNumber, content)
        End If
    End Sub

    Public Sub SendHisab(MobileNumber As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Dim hisabForm As New HisabForm()
        Dim bitMap As Bitmap
        Dim commonWidth, gridHeight, partyLabelHeight, totalLabelHeight As Integer

        Dim bakiAmt As Integer = 0
        Dim jamaAmt As Integer = 0
        Dim currentAmt As Integer
        Dim daysOld As Integer

        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            hisabForm.PartyLabel.Text = PartyNameHindi
            'hisabForm.PartyLabel.Font = New Font("Kruti Dev 100", hisabForm.PartyLabel.Font.Size + 10)
        Else
            hisabForm.PartyLabel.Text = PartyName
        End If
        For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 3 Then
                If Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1).Trim().Equals("") Then
                    'Baki Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1))
                    bakiAmt += currentAmt
                    daysOld = (Today - CDate(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0))).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), CStr(currentAmt), "", daysOld & " दिन"})
                ElseIf Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2).Trim().Equals("") Then
                    'Jama Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
                    jamaAmt += currentAmt
                    daysOld = (Today - CDate(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0))).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), "", CStr(currentAmt), daysOld & " दिन"})
                End If
            End If
        Next
        hisabForm.TotalLabel.Text = "कुल बकाया : " & (bakiAmt - jamaAmt)

        hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
        Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold), .BackColor = Color.FromArgb(230, 230, 230)}
        hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

        partyLabelHeight = hisabForm.PartyLabel.Height
        totalLabelHeight = hisabForm.TotalLabel.Height
        hisabForm.HisabDataGrid.Height = hisabForm.HisabDataGrid.ColumnHeadersHeight + hisabForm.HisabDataGrid.RowCount * hisabForm.HisabDataGrid.Rows(0).Height + 3
        commonWidth = hisabForm.HisabDataGrid.Width
        gridHeight = hisabForm.HisabDataGrid.Height
        hisabForm.HisabDataGrid.ClearSelection()
        bitMap = New Bitmap(commonWidth + 40, gridHeight + partyLabelHeight + totalLabelHeight + 40)
        Drawing.Graphics.FromImage(bitMap).Clear(Color.White)
        hisabForm.PartyLabel.DrawToBitmap(bitMap, New Rectangle(20, 20, commonWidth, partyLabelHeight))
        hisabForm.HisabDataGrid.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight, commonWidth, gridHeight))
        hisabForm.TotalLabel.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight + gridHeight, commonWidth, totalLabelHeight))

        'Send whatsapp
        SendMediaMessage(MobileNumber, bitMap)
    End Sub

    Public Sub PrintHisab(LongShort As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Dim hisabForm As New HisabForm

        Dim bakiAmt As Integer = 0
        Dim jamaAmt As Integer = 0
        Dim currentAmt As Integer
        Dim daysOld As Integer

        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            hisabForm.PartyLabel.Text = PartyNameHindi
            'hisabForm.PartyLabel.Font = New Font("Kruti Dev 100", hisabForm.PartyLabel.Font.Size + 10)
        Else
            hisabForm.PartyLabel.Text = PartyName
        End If
        If LongShort.Equals("Long") Then
            hisabForm.LongSlip = True
        ElseIf LongShort.Equals("Short") Then
            hisabForm.LongSlip = False
        Else
            hisabForm.LongSlip = False
        End If
        For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 3 Then
                If Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1).Trim().Equals("") Then
                    'Baki Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1))
                    bakiAmt += currentAmt
                    Dim dateStr = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                    Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                    daysOld = (Today - entryDate).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), CStr(currentAmt), "", daysOld & " दिन"})
                ElseIf Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2).Trim().Equals("") Then
                    'Jama Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
                    jamaAmt += currentAmt
                    Dim dateStr = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                    Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                    daysOld = (Today - CDate(entryDate)).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), "", CStr(currentAmt), daysOld & " दिन"})
                End If
            End If
        Next
        hisabForm.TotalLabel.Text = "कुल बकाया : " & (bakiAmt - jamaAmt)

        hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
        Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold)}
        hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

        hisabForm.HisabDataGrid.ClearSelection()
        hisabForm.PrintDocument1.Print()
    End Sub

    Public Function PrintSign(SignContent As String, GadiNumber As String) As Boolean
        Dim signForm As New SignForm()
        Dim totalDag As Integer
        Dim PartyName, VchNo, DagCnt, PartyNameHindi As String

        If SignContent.Split(New String() {LineSep}, StringSplitOptions.None).Length = 0 Then
            Return False
        End If
        GadiNumber = "गाड़ी नं:-" & GadiNumber
        GadiNumber &= String.Join("", Enumerable.Repeat(" ", 29 - Len(GadiNumber)))
        signForm.DateLabel.Text = GadiNumber & Now.ToString("dddd dd-MM-yyyy hh:mm tt")
        totalDag = 0
        For Each singleLine As String In SignContent.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 4 Then
                PartyName = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                VchNo = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1)
                DagCnt = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2)
                PartyNameHindi = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(3)
                If Not PartyName.Equals(PartyNameHindi) Then
                    PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
                    signForm.SignGrid.Rows.Add(New String() {DagCnt, "डाग", PartyNameHindi})
                Else
                    signForm.SignGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2), "डाग", PartyName})
                End If
                totalDag += CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
            End If
        Next
        signForm.TotalLabel.Text = "कुल " & totalDag & " डाग             दस्तखत:-"

        signForm.SignGrid.ClearSelection()
        Try
            signForm.PrintDocument1.Print()
        Catch ex As Exception
            MsgBox("Error printing slip: " & ex.Message, MsgBoxStyle.Exclamation, "Print Error")
            Return False
        End Try
        Return True
    End Function

    Public Sub SendContactToJamaGroup(MobileNumber As String, PartyName As String)
        If Not CheckMobileNumberFormat(MobileNumber) Then
            Exit Sub
        End If
        If MyApp.KeyDict("JamaNotGrp").Equals("NA") Then
            Exit Sub
        End If
        SendContact(MyApp.KeyDict("JamaNotGrp"), MobileNumber, PartyName)
    End Sub

    Private Sub BrowserForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Hide()
    End Sub

End Class
