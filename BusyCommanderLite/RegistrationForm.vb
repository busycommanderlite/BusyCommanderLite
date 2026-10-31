Imports System.IO

Public Class RegistrationForm
    Private Const NewSignText = "प्रिय ग्राहक," & vbCrLf & vbCrLf & "आपका ऑर्डर भेज दिया गया है" & vbCrLf & "कुल नग: *{0}*" & vbCrLf & "नाम: *{1}*" & vbCrLf & "ट्रांसपोर्ट: *{2}*" & vbCrLf & vbCrLf & "धन्यवाद"
    Private Const NewJamaNotificationText = "पार्टी: *{0}*" & vbCrLf & "रकम: *{1}*" & vbCrLf & "जमा प्रकार: *{2}*" & vbCrLf & "विवरण: *{3}*" & vbCrLf & "बकाया: *{4}*"
    Private Const ReceiptTextFormat = "प्रिय *{0}*" & vbCrLf & "हमें आपका *{1}* का *{2}* मिल गया है" & vbCrLf & "विवरण: *{3}*" & vbCrLf & "आपका बकाया: *{4}*" & vbCrLf & vbCrLf & "धन्यवाद"
    Private Const BillCaptionFormat = "नियम एवं शर्तें" & vbCrLf & "1. ट्रांसपोर्टर द्वारा माल को हुए किसी भी नुकसान के लिए हम जिम्मेदार नहीं हैं।" & vbCrLf & "2. बिना रसीद प्राप्त किए इस बिल का भुगतान न करें।"
    Private BusyCompList As Dictionary(Of String, String)
    Private BusyScriptControl As MSScriptControl.ScriptControl

    Private Sub RegistrationForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Force form to appear on top of the MSI Installer dialog
        Me.TopMost = True
        
        'Create VBScript for Managing Busy
        If BusyScriptControl Is Nothing Then
            BusyScriptControl = New MSScriptControl.ScriptControl With {
                    .Language = "vbscript"
                }
        End If
        BusyScriptControl.ExecuteStatement(VBscript)
        BusyDataPathTB.Text = Busy_Data_Path
        GetCompaniesListAndLoadCombobox()
        BusyDataPathTB.Select()
    End Sub

    Private Sub GetCompaniesListAndLoadCombobox()
        CompanyCB.ResetText()
        CompanyCB.Items.Clear()
        If Not Directory.Exists(Busy_Data_Path) Then
            SubmitBtn.Enabled = False
            StatusLbl.Text = "Directory doesn't exists"
            StatusLbl.ForeColor = Color.Red
            Exit Sub
        End If

        BusyCompList = New Dictionary(Of String, String)
        Dim CompListArray = BusyScriptControl.Eval("GetCompanies(""" & Busy_Data_Path & """)")
        For Each CompStr In CompListArray
            Dim CompParts = CompStr.ToString().Split(New String() {EntrySep}, StringSplitOptions.None)
            BusyCompList.Add(CompParts(1), CompParts(0))
        Next
        If BusyCompList.Count = 0 Then
            SubmitBtn.Enabled = False
            StatusLbl.Text = "No Companies Found"
            StatusLbl.ForeColor = Color.Red
        Else
            StatusLbl.Text = BusyCompList.Count & " Companies Found"
            StatusLbl.ForeColor = Color.Green
            SubmitBtn.Enabled = True
            CompanyCB.Items.AddRange(BusyCompList.Keys.ToArray())
        End If
    End Sub

    Private Sub SubmitBtn_Click(sender As Object, e As EventArgs) Handles SubmitBtn.Click
        If CompanyCB.SelectedItem Is Nothing Then
            StatusLbl.Text = "Please select a company"
            StatusLbl.ForeColor = Color.Red
            Exit Sub
        End If
        Dim CompanyName = CompanyCB.SelectedItem.ToString()
        If CompanyName.Trim().Equals("") Then
            StatusLbl.Text = "Company name incorrect"
            StatusLbl.ForeColor = Color.Red
            Exit Sub
        End If

        If MsgBox("Have you taken BACKUP of " & vbLf & CompanyName, MsgBoxStyle.YesNo) <> vbYes Then
            Exit Sub
        End If

        If MsgBox("Install Custom Valdations To" & vbLf & CompanyName & vbLf & "Please keep Busy closed during install", MsgBoxStyle.YesNo) <> vbYes Then
            Exit Sub
        End If

        Dim app As New MyApp()
        Dim bcMainStr As String = app.GetBCMain()
        
        If bcMainStr = "NETWORK_ERROR" OrElse String.IsNullOrWhiteSpace(bcMainStr) Then
            MsgBox("Could not fetch BCMain from server. Installation aborted.", MsgBoxStyle.Critical, "Error")
            Exit Sub
        End If
        
        ' CRITICAL FIX: Force all newlines to CRLF. Busy's internal XML parser fails silently if it encounters Unix LFs (\n)!
        bcMainStr = bcMainStr.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf).Replace(vbLf, vbCrLf)
        
        ' PATCH for sandbox: redirect the Firebase script to load BusyCommanderLite instead of BusyCommander
        bcMainStr = bcMainStr.Replace("BusyCommander.", "BusyCommanderLite.")

        ' Get the REAL Dongle Number from Busy to inject into the script!
        Dim BFI As Object = CreateObject("Busy2L21.CFixedInterface")
        Dim realDongleNo As String = BFI.GetDongleSrNo()
        If String.IsNullOrWhiteSpace(realDongleNo) Then
            realDongleNo = "NA"
        End If

        ' (Removed BusyLicenseNumber injection for BCMain because modifying its source code causes an infinite OTA update loop)

        ' Build the EXACT VBScript syntax used in the original version (using & VbCrLf & _)
        Dim lines() As String = bcMainStr.Split(New String() {vbCrLf, vbCr, vbLf}, StringSplitOptions.None)
        Dim vbScriptBuilder As New System.Text.StringBuilder()
        vbScriptBuilder.AppendLine("Dim BCMain")
        vbScriptBuilder.Append("BCMain = ")
        
        For i As Integer = 0 To lines.Length - 1
            vbScriptBuilder.Append("""" & lines(i).Replace("""", """""") & """")
            If i < lines.Length - 1 Then
                vbScriptBuilder.AppendLine(" & vbCrLf & _")
            End If
        Next
        
        ' Inject the perfectly formatted multiline string into VBScript's global memory using AddCode
        BusyScriptControl.AddCode(vbScriptBuilder.ToString())
        
        ' Now call Eval exactly like the original working version did!
        Dim InstallStatus = BusyScriptControl.Eval("InstallMain(""" & BusyCompList(CompanyName) & """,""" & Busy_Data_Path & """)")

        If InstallStatus.Equals("Success") OrElse InstallStatus.Equals("Replaced") Then
            If InstallStatus.Equals("Replaced") Then
                MsgBox("An existing BCMain configuration was detected in """ & CompanyName & """ and has been successfully updated to Version 1.", MsgBoxStyle.Information, "BusyCommander Upgrade")
            End If
            Dim existingDict = ReadConfigFile()
            Dim newKeyDict As Dictionary(Of String, String)
            If existingDict IsNot Nothing AndAlso existingDict.Count > 0 Then
                newKeyDict = New Dictionary(Of String, String)(existingDict, StringComparer.OrdinalIgnoreCase)
                newKeyDict("CompanyName") = CompanyName
            Else
                newKeyDict = New Dictionary(Of String, String) From {
                        {"LineSep", ";;;"},
                        {"EntrySep", "~~~"},
                        {"DefaultNumber1", "NA"},
                        {"DefaultNumber2", "NA"},
                        {"DefaultNumber3", "NA"},
                        {"DefaultNumber4", "NA"},
                        {"QRPath", "C:\Busywin\WhatsApp\QR.jpeg"},
                        {"PaymentAddress", "NA"},
                        {"CompanyName", CompanyName},
                        {"SignText", EncodeString(NewSignText)},
                        {"BillFormat", "NA"},
                        {"SendBillCaption", "no"},
                        {"BillCaption", EncodeString(BillCaptionFormat)},
                        {"SendReceiptToCustomer", "Customized Message"},
                        {"ReceiptPrintFormat", "NA"},
                        {"ReceiptText", EncodeString(ReceiptTextFormat)},
                        {"JamaNotGrp", "NA"},
                        {"JamaNotificationText", EncodeString(NewJamaNotificationText)},
                        {"SendBillAs", "Image"},
                        {"AccountColumnSalesRegister", "Account"},
                        {"VchNoColumnSalesRegister", "Vch/Bill No"},
                        {"AmountColumnSalesRegister", "Total Amount"},
                        {"VchNoColumnAccountLedger", "Vch/Bill No"},
                        {"TypeColumnAccountLedger", "Type"},
                        {"SaleTypeAccountLedger", "Sale"},
                        {"DatedColumnStatement", "Dated"},
                        {"ReceivableColumnStatement", "Receivable"},
                        {"PayableColumnStatement", "Payable"},
                        {"RefNoColumnStatement", "Ref. No."},
                        {"TypeColumnStatement", "Type"},
                        {"SaleTypeStatement", "Sale"},
                        {"WPPVersion", "0"}
                }
            End If
            SaveConfigFile(newKeyDict)

            ' Ensure Register.vbs exists in installation folder for easy re-registration
            Try
                Dim regScriptDir = Path.GetDirectoryName(Busy_Config_Path)
                If Not Directory.Exists(regScriptDir) Then
                    Directory.CreateDirectory(regScriptDir)
                End If
                Dim regScriptPath = Path.Combine(regScriptDir, "Register.vbs")
                If Not File.Exists(regScriptPath) Then
                    File.WriteAllText(regScriptPath, "Dim BCObj" & vbCrLf & "Set BCObj = CreateObject(""BusyCommanderLite.MyApp"")" & vbCrLf & "BCObj.StartRegistration()" & vbCrLf & "Set BCObj = Nothing" & vbCrLf)
                End If
            Catch
            End Try

            DialogResult = DialogResult.OK
            MsgBox("Please open company """ & CompanyName & """ to complete registration", MsgBoxStyle.Information)
        Else
            DialogResult = DialogResult.Abort
            MsgBox(InstallStatus, MsgBoxStyle.Critical)
        End If
        Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub BusyDataPathTB_Leave(sender As Object, e As EventArgs) Handles BusyDataPathTB.Leave
        If Not BusyDataPathTB.Text.EndsWith("\") Then
            BusyDataPathTB.Text &= "\"
        End If
        Busy_Data_Path = BusyDataPathTB.Text
        GetCompaniesListAndLoadCombobox()
    End Sub

End Class