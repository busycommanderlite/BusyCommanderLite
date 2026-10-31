Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class UserConsole
    Private PaymentAddress As String

    Private Sub UserConsole_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim cfg = ReadConfigFile()
            If cfg IsNot Nothing Then
                MyApp.KeyDict = cfg
            End If
        Catch ex As Exception
        End Try

        'Set KeyDict Values
        CompanyLbl.Text = MyApp.KeyDict("CompanyName")
        SndRcptCstCB.SelectedIndex = SndRcptCstCB.FindStringExact(MyApp.KeyDict("SendReceiptToCustomer"))

        DN1tb.Text = MyApp.KeyDict("DefaultNumber1")
        DN2tb.Text = MyApp.KeyDict("DefaultNumber2")
        DN3tb.Text = MyApp.KeyDict("DefaultNumber3")
        DN4tb.Text = MyApp.KeyDict("DefaultNumber4")
        QRlbl.Text = MyApp.KeyDict("QRPath")

        RcptNtfGrpCB.Items.Add("NA")
        If MyApp.GroupsDict IsNot Nothing Then
            RcptNtfGrpCB.Items.AddRange(MyApp.GroupsDict.Values.ToArray())
            If MyApp.GroupsDict.ContainsKey(MyApp.KeyDict("JamaNotGrp")) Then
                RcptNtfGrpCB.SelectedIndex = MyApp.GroupsDict.Keys.ToList().IndexOf(MyApp.KeyDict("JamaNotGrp")) + 1
            Else
                RcptNtfGrpCB.SelectedIndex = RcptNtfGrpCB.FindStringExact("NA")
            End If
        Else
            RcptNtfGrpCB.SelectedIndex = RcptNtfGrpCB.FindStringExact("NA")
        End If
        If MyApp.KeyDict.ContainsKey("ReduceAmount100x") Then
            ReduceAmtCB.Checked = MyApp.KeyDict("ReduceAmount100x").Equals("yes", StringComparison.OrdinalIgnoreCase)
        Else
            ReduceAmtCB.Checked = True
        End If
        If MyApp.KeyDict.ContainsKey("UseKrutiDev") Then
            UseKrutiDevCB.Checked = MyApp.KeyDict("UseKrutiDev").Equals("yes", StringComparison.OrdinalIgnoreCase)
        Else
            UseKrutiDevCB.Checked = True ' default to true for backwards compatibility
        End If
        If MyApp.KeyDict.ContainsKey("EnableHTTPServer") Then
            EnableHttpServerCB.Checked = MyApp.KeyDict("EnableHTTPServer").Equals("yes", StringComparison.OrdinalIgnoreCase)
        Else
            EnableHttpServerCB.Checked = False
        End If
        SendBillAsCB.SelectedIndex = SendBillAsCB.FindStringExact(MyApp.KeyDict("SendBillAs"))
        RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
        DispatchTB.Text = DecodeString(MyApp.KeyDict("SignText"))
        RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
        SndBilCpnCB.SelectedIndex = SndBilCpnCB.FindStringExact(MyApp.KeyDict("SendBillCaption"))
        BillCptnTB.Text = DecodeString(MyApp.KeyDict("BillCaption"))

        'Set formats in form
        BillPrnFrmtCB.Items.Clear()
        BillPrnFrmtCB.Items.Add("NA")
        BillPrnFrmtCB.Items.AddRange(MyApp.BillFormats.ToArray())
        If MyApp.BillFormats.Contains(MyApp.KeyDict("BillFormat")) Then
            BillPrnFrmtCB.SelectedIndex = BillPrnFrmtCB.FindStringExact(MyApp.KeyDict("BillFormat"))
        Else
            BillPrnFrmtCB.SelectedIndex = BillPrnFrmtCB.FindStringExact("NA")
        End If
        RcptPrnFrmtCB.Items.Clear()
        RcptPrnFrmtCB.Items.Add("NA")
        RcptPrnFrmtCB.Items.AddRange(MyApp.ReceiptFormats.ToArray())
        If MyApp.ReceiptFormats.Contains(MyApp.KeyDict("ReceiptPrintFormat")) Then
            RcptPrnFrmtCB.SelectedIndex = RcptPrnFrmtCB.FindStringExact(MyApp.KeyDict("ReceiptPrintFormat"))
        Else
            RcptPrnFrmtCB.SelectedIndex = RcptPrnFrmtCB.FindStringExact("NA")
        End If

        Me.TopMost = True
        Me.BringToFront()
        'myBrowserForm.Hide()
    End Sub

    Private Function CheckValues() As Boolean
        If Not DN1Status.BackColor.Equals(Color.Green) Then
            DN1tb.Focus()
            MsgBox("Incorrect Input", MsgBoxStyle.Critical)
            Return False
        ElseIf Not DN2Status.BackColor.Equals(Color.Green) Then
            DN2tb.Focus()
            MsgBox("Incorrect Input", MsgBoxStyle.Critical)
            Return False
        ElseIf Not DN3Status.BackColor.Equals(Color.Green) Then
            DN3tb.Focus()
            MsgBox("Incorrect Input", MsgBoxStyle.Critical)
            Return False
        ElseIf Not DN4Status.BackColor.Equals(Color.Green) Then
            DN4tb.Focus()
            MsgBox("Incorrect Input", MsgBoxStyle.Critical)
            Return False
        End If

        QRlbl.Text = QRlbl.Text.Trim().Trim(""""c, "'"c)
        If File.Exists(QRlbl.Text) Then
            PaymentAddress = GetPaymentAddressFromQR(QRlbl.Text)
            If PaymentAddress.Equals("") Then
                MsgBox("QR is Incorrect!" & vbLf & "Click the path to set it.", MsgBoxStyle.Critical)
                Return False
            End If
        Else
            PaymentAddress = MyApp.KeyDict("PaymentAddress")
        End If

        Return True
    End Function

    Private Function CountString(inputString As String, stringToBeSearched As String) As Double
        Return (inputString.Length - inputString.Replace(stringToBeSearched, String.Empty).Length) / stringToBeSearched.Length
    End Function

    Private Function GetPaymentAddressFromQR(ImagePath As String) As String
        ' 1. If path is unchanged and we already have a saved PaymentAddress, reuse it immediately
        If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("QRPath") AndAlso MyApp.KeyDict.ContainsKey("PaymentAddress") Then
            If ImagePath.Trim().Equals(MyApp.KeyDict("QRPath").Trim(), StringComparison.OrdinalIgnoreCase) Then
                Dim savedPA = MyApp.KeyDict("PaymentAddress").Trim()
                If Not String.IsNullOrEmpty(savedPA) AndAlso Not savedPA.Equals("NA", StringComparison.OrdinalIgnoreCase) Then
                    Return savedPA
                End If
            End If
        End If

        ' 2. Try decoding locally via Go server at http://localhost:18080/decode_qr?path=...
        Try
            Using client As New HttpClient()
                client.Timeout = TimeSpan.FromSeconds(5)
                Dim encodedPath = Uri.EscapeDataString(ImagePath)
                Dim url = QRDecoderURL & encodedPath
                Dim response = client.GetAsync(url).GetAwaiter().GetResult()
                If response.IsSuccessStatusCode Then
                    Dim strJSON = response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                    If Not String.IsNullOrWhiteSpace(strJSON) Then
                        Dim obj = JObject.Parse(strJSON)
                        If obj("status") IsNot Nothing AndAlso obj("status").ToString() = "OK" Then
                            Dim paVal = If(obj("pa") IsNot Nothing, obj("pa").ToString().Trim(), "")
                            If Not String.IsNullOrEmpty(paVal) Then
                                Return paVal
                            End If
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            ' Safe fallback if local Go server decoder is unreachable
        End Try

        ' 3. Fallback to existing PaymentAddress from config if available
        If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("PaymentAddress") Then
            Dim savedPA = MyApp.KeyDict("PaymentAddress").Trim()
            If Not String.IsNullOrEmpty(savedPA) AndAlso Not savedPA.Equals("NA", StringComparison.OrdinalIgnoreCase) Then
                Return savedPA
            End If
        End If

        Return ""
    End Function

    Private Sub CancelBtn_Click(sender As Object, e As EventArgs) Handles CancelBtn.Click
        Me.Close()
    End Sub

    Private Sub SubmitBtn_Click(sender As Object, e As EventArgs) Handles SubmitBtn.Click
        'Submit Buton

        If Not CheckValues() Then
            Exit Sub
        End If

        Dim jamaGroup = "NA"
        If MyApp.GroupsDict IsNot Nothing AndAlso RcptNtfGrpCB.SelectedItem IsNot Nothing AndAlso MyApp.GroupsDict.ContainsValue(RcptNtfGrpCB.SelectedItem.ToString()) Then
            jamaGroup = MyApp.GroupsDict.ElementAt(RcptNtfGrpCB.SelectedIndex - 1).Key
        End If

        Dim NewKeyDict As New Dictionary(Of String, String) From {
            {"LineSep", MyApp.KeyDict("LineSep")},
            {"EntrySep", MyApp.KeyDict("EntrySep")},
            {"DefaultNumber1", DN1tb.Text},
            {"DefaultNumber2", DN2tb.Text},
            {"DefaultNumber3", DN3tb.Text},
            {"DefaultNumber4", DN4tb.Text},
            {"QRPath", QRlbl.Text},
            {"PaymentAddress", PaymentAddress},
            {"CompanyName", MyApp.KeyDict("CompanyName")},
            {"SignText", EncodeString(DispatchTB.Text)},
            {"BillFormat", BillPrnFrmtCB.Text},
            {"SendBillCaption", SndBilCpnCB.Text},
            {"BillCaption", EncodeString(BillCptnTB.Text)},
            {"SendReceiptToCustomer", SndRcptCstCB.Text},
            {"ReceiptPrintFormat", RcptPrnFrmtCB.Text},
            {"ReceiptText", EncodeString(RcptFrmtTB.Text)},
            {"JamaNotGrp", jamaGroup},
            {"SendBillAs", SendBillAsCB.Text},
            {"JamaNotificationText", EncodeString(RcptNtfFrmtTB.Text)},
            {"ReduceAmount100x", If(ReduceAmtCB.Checked, "yes", "no")},
            {"UseKrutiDev", If(UseKrutiDevCB.Checked, "yes", "no")},
            {"AccountColumnSalesRegister", MyApp.KeyDict("AccountColumnSalesRegister")},
            {"VchNoColumnSalesRegister", MyApp.KeyDict("VchNoColumnSalesRegister")},
            {"AmountColumnSalesRegister", MyApp.KeyDict("AmountColumnSalesRegister")},
            {"VchNoColumnAccountLedger", MyApp.KeyDict("VchNoColumnAccountLedger")},
            {"TypeColumnAccountLedger", MyApp.KeyDict("TypeColumnAccountLedger")},
            {"SaleTypeAccountLedger", MyApp.KeyDict("SaleTypeAccountLedger")},
            {"DatedColumnStatement", MyApp.KeyDict("DatedColumnStatement")},
            {"ReceivableColumnStatement", MyApp.KeyDict("ReceivableColumnStatement")},
            {"PayableColumnStatement", MyApp.KeyDict("PayableColumnStatement")},
            {"RefNoColumnStatement", MyApp.KeyDict("RefNoColumnStatement")},
            {"TypeColumnStatement", MyApp.KeyDict("TypeColumnStatement")},
            {"SaleTypeStatement", MyApp.KeyDict("SaleTypeStatement")},
            {"WPPVersion", MyApp.KeyDict("WPPVersion")},
            {"EnableHTTPServer", If(EnableHttpServerCB.Checked, "yes", "no")}
        }

        SaveConfigFile(NewKeyDict)
        MyApp.KeyDict = NewKeyDict

        If Not jamaGroup.Equals("NA", StringComparison.OrdinalIgnoreCase) AndAlso MyApp.brForm IsNot Nothing Then
            MyApp.brForm.SetDisappearingTimerToGoServer(jamaGroup, 86400)
        End If

        Dim httpStatusMsg As String = ""
        If EnableHttpServerCB.Checked Then
            If MyApp.myServer Is Nothing AndAlso MyApp.brForm IsNot Nothing Then
                Dim res = MyApp.ToggleHttpServer(True)
                If res = "started" Then
                    httpStatusMsg = vbCrLf & "HTTP Server started on port " & MyApp.Port_Num & "."
                ElseIf res.StartsWith("error:") Then
                    httpStatusMsg = vbCrLf & "Warning: Could not start HTTP Server: " & res.Substring(6)
                End If
            End If
        Else
            If MyApp.myServer IsNot Nothing Then
                MyApp.ToggleHttpServer(False)
                httpStatusMsg = vbCrLf & "HTTP Server stopped."
            End If
        End If

        MsgBox("Update Successful" & httpStatusMsg)
        Me.Close()
    End Sub

    Private Sub QRlbl_Click(sender As Object, e As EventArgs) Handles QRlbl.Click
        Dim fd As New OpenFileDialog With {
            .Title = "Select QR Image File",
            .InitialDirectory = CurDir(),
            .Filter = "Image Files|*.jpg;*.jpeg;*.png;",
            .FilterIndex = 2,
            .RestoreDirectory = True
        }

        If fd.ShowDialog() = DialogResult.OK Then
            QRlbl.Text = fd.FileName
        End If
    End Sub

    Private Sub RcptNtfGrpCB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RcptNtfGrpCB.SelectedIndexChanged
        If RcptNtfGrpCB.SelectedItem.Equals("NA") Then
            RcptNtfFrmtTB.Enabled = False
        Else
            RcptNtfFrmtTB.Enabled = True
        End If
    End Sub

    Private Sub SndRcptCstCB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SndRcptCstCB.SelectedIndexChanged
        If SndRcptCstCB.SelectedIndex = 0 Then
            RcptPrnFrmtCB.Enabled = False
            RcptFrmtTB.Enabled = False
        ElseIf SndRcptCstCB.SelectedIndex = 1 Then
            RcptPrnFrmtCB.Enabled = True
            RcptFrmtTB.Enabled = False
        ElseIf SndRcptCstCB.SelectedIndex = 2 Then
            RcptPrnFrmtCB.Enabled = False
            RcptFrmtTB.Enabled = True
        End If
    End Sub

    Private Sub SndBilCpnCB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles SndBilCpnCB.SelectedIndexChanged
        If SndBilCpnCB.SelectedIndex = 0 Then
            BillCptnTB.Enabled = True
        Else
            BillCptnTB.Enabled = False
        End If
    End Sub

    Private Sub RcptFrmtTB_LostFocus(sender As Object, e As EventArgs) Handles RcptFrmtTB.LostFocus
        If CountString(RcptFrmtTB.Text, "{0}") > 1 Then
            MsgBox("Must contain at most one {0} for PartyName")
            RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
            RcptFrmtTB.Focus()
        ElseIf CountString(RcptFrmtTB.Text, "{1}") <> 1 Then
            MsgBox("Must contain one {1} for Amount")
            RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
            RcptFrmtTB.Focus()
        ElseIf CountString(RcptFrmtTB.Text, "{2}") > 1 Then
            MsgBox("Must contain at most one {2} for Mode")
            RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
            RcptFrmtTB.Focus()
        ElseIf CountString(RcptFrmtTB.Text, "{3}") > 1 Then
            MsgBox("Must contain at most one {3} for Narration")
            RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
            RcptFrmtTB.Focus()
        ElseIf CountString(RcptFrmtTB.Text, "{4}") > 1 Then
            MsgBox("Must contain at most one {4} for Balance")
            RcptFrmtTB.Text = DecodeString(MyApp.KeyDict("ReceiptText"))
            RcptFrmtTB.Focus()
        End If
    End Sub

    Private Sub DispatchTB_LostFocus(sender As Object, e As EventArgs) Handles DispatchTB.LostFocus
        If CountString(DispatchTB.Text, "{0}") <> 1 Then
            MsgBox("Must contain one {0} for NagCount")
            DispatchTB.Text = DecodeString(MyApp.KeyDict("SignText"))
            DispatchTB.Focus()
        ElseIf CountString(DispatchTB.Text, "{1}") > 1 Then
            MsgBox("Must contain at most one {1} for PartyName")
            DispatchTB.Text = DecodeString(MyApp.KeyDict("SignText"))
            DispatchTB.Focus()
        ElseIf CountString(DispatchTB.Text, "{2}") > 1 Then
            MsgBox("Must contain at most one {2} for Transport")
            DispatchTB.Text = DecodeString(MyApp.KeyDict("SignText"))
            DispatchTB.Focus()
        End If
    End Sub

    Private Sub RcptNtfFrmtTB_LostFocus(sender As Object, e As EventArgs) Handles RcptNtfFrmtTB.LostFocus
        If CountString(RcptNtfFrmtTB.Text, "{0}") <> 1 Then
            MsgBox("Must contain one {0} for PartyName")
            RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
            RcptNtfFrmtTB.Focus()
        ElseIf CountString(RcptNtfFrmtTB.Text, "{1}") <> 1 Then
            MsgBox("Must contain one {1} for Amount")
            RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
            RcptNtfFrmtTB.Focus()
        ElseIf CountString(RcptNtfFrmtTB.Text, "{2}") > 1 Then
            MsgBox("Must contain at most one {2} for Mode")
            RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
            RcptNtfFrmtTB.Focus()
        ElseIf CountString(RcptNtfFrmtTB.Text, "{3}") > 1 Then
            MsgBox("Must contain at most one {3} for Narration")
            RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
            RcptNtfFrmtTB.Focus()
        ElseIf CountString(RcptNtfFrmtTB.Text, "{4}") > 1 Then
            MsgBox("Must contain at most one {4} for Balance")
            RcptNtfFrmtTB.Text = DecodeString(MyApp.KeyDict("JamaNotificationText"))
            RcptNtfFrmtTB.Focus()
        End If
    End Sub

    Private Sub DN1tb_TextChanged(sender As Object, e As EventArgs) Handles DN1tb.TextChanged
        If DN1tb.Text.Equals("NA") Then
            DN1Status.BackColor = Color.Green
        ElseIf CheckMobileNumberFormat(DN1tb.Text) Then
            MyApp.brForm.CheckNumber(DN1tb.Text, "CheckTB1")
        Else
            DN1Status.BackColor = Color.Red
        End If
    End Sub

    Private Sub DN2tb_TextChanged(sender As Object, e As EventArgs) Handles DN2tb.TextChanged
        If DN2tb.Text.Equals("NA") Then
            DN2Status.BackColor = Color.Green
        ElseIf CheckMobileNumberFormat(DN2tb.Text) Then
            MyApp.brForm.CheckNumber(DN2tb.Text, "CheckTB2")
        Else
            DN2Status.BackColor = Color.Red
        End If
    End Sub

    Private Sub DN3tb_TextChanged(sender As Object, e As EventArgs) Handles DN3tb.TextChanged
        If DN3tb.Text.Equals("NA") Then
            DN3Status.BackColor = Color.Green
        ElseIf CheckMobileNumberFormat(DN3tb.Text) Then
            MyApp.brForm.CheckNumber(DN3tb.Text, "CheckTB3")
        Else
            DN3Status.BackColor = Color.Red
        End If
    End Sub

    Private Sub DN4tb_TextChanged(sender As Object, e As EventArgs) Handles DN4tb.TextChanged
        If DN4tb.Text.Equals("NA") Then
            DN4Status.BackColor = Color.Green
        ElseIf CheckMobileNumberFormat(DN4tb.Text) Then
            MyApp.brForm.CheckNumber(DN4tb.Text, "CheckTB4")
        Else
            DN4Status.BackColor = Color.Red
        End If
    End Sub

    Private Sub DN1tb_LostFocus(sender As Object, e As EventArgs) Handles DN1tb.LostFocus
        If DN1tb.Text.Trim().Equals("") Then
            DN1tb.Text = "NA"
        End If
    End Sub

    Private Sub DN2tb_LostFocus(sender As Object, e As EventArgs) Handles DN2tb.LostFocus
        If DN2tb.Text.Trim().Equals("") Then
            DN2tb.Text = "NA"
        End If
    End Sub

    Private Sub DN3tb_LostFocus(sender As Object, e As EventArgs) Handles DN3tb.LostFocus
        If DN3tb.Text.Trim().Equals("") Then
            DN3tb.Text = "NA"
        End If
    End Sub

    Private Sub DN4tb_LostFocus(sender As Object, e As EventArgs) Handles DN4tb.LostFocus
        If DN4tb.Text.Trim().Equals("") Then
            DN4tb.Text = "NA"
        End If
    End Sub

    Private Sub btnSMSHelp_Click(sender As Object, e As EventArgs) Handles btnSMSHelp.Click
        Dim f As New SMSHelpForm()
        f.ShowDialog(Me)
    End Sub
End Class
