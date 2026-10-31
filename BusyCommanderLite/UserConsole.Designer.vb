<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UserConsole
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.CancelBtn = New System.Windows.Forms.Button()
        Me.SubmitBtn = New System.Windows.Forms.Button()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.DN1tb = New System.Windows.Forms.TextBox()
        Me.DN2tb = New System.Windows.Forms.TextBox()
        Me.DN3tb = New System.Windows.Forms.TextBox()
        Me.DN4tb = New System.Windows.Forms.TextBox()
        Me.QRlbl = New System.Windows.Forms.Label()
        Me.RcptNtfFrmtTB = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.DispatchTB = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.RcptNtfGrpCB = New System.Windows.Forms.ComboBox()
        Me.SndRcptCstCB = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.RcptFrmtTB = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.RcptPrnFrmtCB = New System.Windows.Forms.ComboBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.BillCptnTB = New System.Windows.Forms.TextBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.SndBilCpnCB = New System.Windows.Forms.ComboBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.BillPrnFrmtCB = New System.Windows.Forms.ComboBox()
        Me.CompanyLbl = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.DN1Status = New System.Windows.Forms.Label()
        Me.DN2Status = New System.Windows.Forms.Label()
        Me.DN3Status = New System.Windows.Forms.Label()
        Me.DN4Status = New System.Windows.Forms.Label()
        Me.SendBillAsCB = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ReduceAmtCB = New System.Windows.Forms.CheckBox()
        Me.UseKrutiDevCB = New System.Windows.Forms.CheckBox()
        Me.EnableHttpServerCB = New System.Windows.Forms.CheckBox()
        Me.btnSMSHelp = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CancelBtn
        '
        Me.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CancelBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelBtn.Location = New System.Drawing.Point(489, 527)
        Me.CancelBtn.Name = "CancelBtn"
        Me.CancelBtn.Size = New System.Drawing.Size(406, 50)
        Me.CancelBtn.TabIndex = 52
        Me.CancelBtn.Text = "Cancel"
        Me.CancelBtn.UseVisualStyleBackColor = True
        '
        'SubmitBtn
        '
        Me.SubmitBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SubmitBtn.Location = New System.Drawing.Point(12, 527)
        Me.SubmitBtn.Name = "SubmitBtn"
        Me.SubmitBtn.Size = New System.Drawing.Size(406, 50)
        Me.SubmitBtn.TabIndex = 51
        Me.SubmitBtn.Text = "Submit"
        Me.SubmitBtn.UseVisualStyleBackColor = True
        '
        'Label9
        '
        Me.Label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(12, 9)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(200, 40)
        Me.Label9.TabIndex = 20
        Me.Label9.Text = "Default Number 1"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label10
        '
        Me.Label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(12, 52)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(200, 40)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "Default Number 2"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label11
        '
        Me.Label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(12, 95)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(200, 40)
        Me.Label11.TabIndex = 22
        Me.Label11.Text = "Default Number 3"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label12
        '
        Me.Label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(12, 138)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(200, 40)
        Me.Label12.TabIndex = 23
        Me.Label12.Text = "Default Number 4"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label13
        '
        Me.Label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(489, 409)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(200, 25)
        Me.Label13.TabIndex = 24
        Me.Label13.Text = "Receipt Notification Group"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label19
        '
        Me.Label19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label19.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label19.Location = New System.Drawing.Point(489, 4)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(200, 28)
        Me.Label19.TabIndex = 30
        Me.Label19.Text = "Bill Format For Sending"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label20
        '
        Me.Label20.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label20.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label20.Location = New System.Drawing.Point(12, 209)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(200, 74)
        Me.Label20.TabIndex = 31
        Me.Label20.Text = "QR Path" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(Click to Set)"
        Me.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DN1tb
        '
        Me.DN1tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN1tb.Location = New System.Drawing.Point(218, 11)
        Me.DN1tb.MaximumSize = New System.Drawing.Size(200, 40)
        Me.DN1tb.MaxLength = 10
        Me.DN1tb.Name = "DN1tb"
        Me.DN1tb.Size = New System.Drawing.Size(182, 40)
        Me.DN1tb.TabIndex = 34
        Me.DN1tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DN2tb
        '
        Me.DN2tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN2tb.Location = New System.Drawing.Point(218, 52)
        Me.DN2tb.MaximumSize = New System.Drawing.Size(200, 40)
        Me.DN2tb.MaxLength = 10
        Me.DN2tb.Name = "DN2tb"
        Me.DN2tb.Size = New System.Drawing.Size(182, 40)
        Me.DN2tb.TabIndex = 35
        Me.DN2tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DN3tb
        '
        Me.DN3tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN3tb.Location = New System.Drawing.Point(218, 95)
        Me.DN3tb.MaximumSize = New System.Drawing.Size(200, 40)
        Me.DN3tb.MaxLength = 10
        Me.DN3tb.Name = "DN3tb"
        Me.DN3tb.Size = New System.Drawing.Size(182, 40)
        Me.DN3tb.TabIndex = 36
        Me.DN3tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DN4tb
        '
        Me.DN4tb.Font = New System.Drawing.Font("Microsoft Sans Serif", 20.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN4tb.Location = New System.Drawing.Point(218, 138)
        Me.DN4tb.MaximumSize = New System.Drawing.Size(200, 40)
        Me.DN4tb.MaxLength = 10
        Me.DN4tb.Name = "DN4tb"
        Me.DN4tb.Size = New System.Drawing.Size(182, 40)
        Me.DN4tb.TabIndex = 37
        Me.DN4tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'QRlbl
        '
        Me.QRlbl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.QRlbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.QRlbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.QRlbl.Location = New System.Drawing.Point(218, 208)
        Me.QRlbl.Name = "QRlbl"
        Me.QRlbl.Size = New System.Drawing.Size(200, 75)
        Me.QRlbl.TabIndex = 40
        Me.QRlbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RcptNtfFrmtTB
        '
        Me.RcptNtfFrmtTB.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RcptNtfFrmtTB.Location = New System.Drawing.Point(695, 434)
        Me.RcptNtfFrmtTB.MaxLength = 1000
        Me.RcptNtfFrmtTB.Multiline = True
        Me.RcptNtfFrmtTB.Name = "RcptNtfFrmtTB"
        Me.RcptNtfFrmtTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.RcptNtfFrmtTB.Size = New System.Drawing.Size(200, 93)
        Me.RcptNtfFrmtTB.TabIndex = 50
        Me.RcptNtfFrmtTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label15
        '
        Me.Label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(489, 434)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(200, 93)
        Me.Label15.TabIndex = 77
        Me.Label15.Text = "Receipt Notification Format"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DispatchTB
        '
        Me.DispatchTB.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DispatchTB.Location = New System.Drawing.Point(218, 409)
        Me.DispatchTB.MaxLength = 1000
        Me.DispatchTB.Multiline = True
        Me.DispatchTB.Name = "DispatchTB"
        Me.DispatchTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.DispatchTB.Size = New System.Drawing.Size(200, 115)
        Me.DispatchTB.TabIndex = 41
        Me.DispatchTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label17
        '
        Me.Label17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(12, 409)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(200, 115)
        Me.Label17.TabIndex = 79
        Me.Label17.Text = "Dispatch Format"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RcptNtfGrpCB
        '
        Me.RcptNtfGrpCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.RcptNtfGrpCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RcptNtfGrpCB.FormattingEnabled = True
        Me.RcptNtfGrpCB.Location = New System.Drawing.Point(695, 409)
        Me.RcptNtfGrpCB.MaxDropDownItems = 100
        Me.RcptNtfGrpCB.Name = "RcptNtfGrpCB"
        Me.RcptNtfGrpCB.Size = New System.Drawing.Size(200, 33)
        Me.RcptNtfGrpCB.TabIndex = 49
        '
        'SndRcptCstCB
        '
        Me.SndRcptCstCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SndRcptCstCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SndRcptCstCB.FormattingEnabled = True
        Me.SndRcptCstCB.Items.AddRange(New Object() {"no", "Print Format", "Customized Message"})
        Me.SndRcptCstCB.Location = New System.Drawing.Point(695, 228)
        Me.SndRcptCstCB.MaxDropDownItems = 100
        Me.SndRcptCstCB.Name = "SndRcptCstCB"
        Me.SndRcptCstCB.Size = New System.Drawing.Size(200, 33)
        Me.SndRcptCstCB.TabIndex = 46
        '
        'Label21
        '
        Me.Label21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label21.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label21.Location = New System.Drawing.Point(489, 228)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(200, 25)
        Me.Label21.TabIndex = 82
        Me.Label21.Text = "Send Receipt To Customer"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RcptFrmtTB
        '
        Me.RcptFrmtTB.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RcptFrmtTB.Location = New System.Drawing.Point(695, 282)
        Me.RcptFrmtTB.MaxLength = 1000
        Me.RcptFrmtTB.Multiline = True
        Me.RcptFrmtTB.Name = "RcptFrmtTB"
        Me.RcptFrmtTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.RcptFrmtTB.Size = New System.Drawing.Size(200, 93)
        Me.RcptFrmtTB.TabIndex = 48
        Me.RcptFrmtTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label23
        '
        Me.Label23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(489, 282)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(200, 93)
        Me.Label23.TabIndex = 84
        Me.Label23.Text = "Receipt Format"
        Me.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'RcptPrnFrmtCB
        '
        Me.RcptPrnFrmtCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.RcptPrnFrmtCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RcptPrnFrmtCB.FormattingEnabled = True
        Me.RcptPrnFrmtCB.Location = New System.Drawing.Point(695, 255)
        Me.RcptPrnFrmtCB.MaxDropDownItems = 100
        Me.RcptPrnFrmtCB.Name = "RcptPrnFrmtCB"
        Me.RcptPrnFrmtCB.Size = New System.Drawing.Size(200, 33)
        Me.RcptPrnFrmtCB.TabIndex = 47
        '
        'Label25
        '
        Me.Label25.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label25.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label25.Location = New System.Drawing.Point(489, 255)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(200, 25)
        Me.Label25.TabIndex = 86
        Me.Label25.Text = "Receipt Print Format"
        Me.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BillCptnTB
        '
        Me.BillCptnTB.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BillCptnTB.Location = New System.Drawing.Point(695, 91)
        Me.BillCptnTB.MaxLength = 1000
        Me.BillCptnTB.Multiline = True
        Me.BillCptnTB.Name = "BillCptnTB"
        Me.BillCptnTB.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.BillCptnTB.Size = New System.Drawing.Size(200, 93)
        Me.BillCptnTB.TabIndex = 45
        Me.BillCptnTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label27
        '
        Me.Label27.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label27.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label27.Location = New System.Drawing.Point(489, 91)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(200, 93)
        Me.Label27.TabIndex = 90
        Me.Label27.Text = "Bill Caption"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'SndBilCpnCB
        '
        Me.SndBilCpnCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SndBilCpnCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SndBilCpnCB.FormattingEnabled = True
        Me.SndBilCpnCB.Items.AddRange(New Object() {"yes", "no"})
        Me.SndBilCpnCB.Location = New System.Drawing.Point(695, 62)
        Me.SndBilCpnCB.MaxDropDownItems = 100
        Me.SndBilCpnCB.Name = "SndBilCpnCB"
        Me.SndBilCpnCB.Size = New System.Drawing.Size(200, 33)
        Me.SndBilCpnCB.TabIndex = 44
        '
        'Label29
        '
        Me.Label29.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label29.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label29.Location = New System.Drawing.Point(489, 62)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(200, 28)
        Me.Label29.TabIndex = 88
        Me.Label29.Text = "Send Bill With Caption"
        Me.Label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BillPrnFrmtCB
        '
        Me.BillPrnFrmtCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.BillPrnFrmtCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BillPrnFrmtCB.FormattingEnabled = True
        Me.BillPrnFrmtCB.Location = New System.Drawing.Point(695, 4)
        Me.BillPrnFrmtCB.MaxDropDownItems = 100
        Me.BillPrnFrmtCB.Name = "BillPrnFrmtCB"
        Me.BillPrnFrmtCB.Size = New System.Drawing.Size(200, 33)
        Me.BillPrnFrmtCB.TabIndex = 42
        '
        'CompanyLbl
        '
        Me.CompanyLbl.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.CompanyLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.CompanyLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CompanyLbl.Location = New System.Drawing.Point(218, 305)
        Me.CompanyLbl.Name = "CompanyLbl"
        Me.CompanyLbl.Size = New System.Drawing.Size(200, 70)
        Me.CompanyLbl.TabIndex = 94
        Me.CompanyLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(12, 305)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(200, 70)
        Me.Label6.TabIndex = 95
        Me.Label6.Text = "Company Name"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DN1Status
        '
        Me.DN1Status.BackColor = System.Drawing.Color.Red
        Me.DN1Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DN1Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN1Status.Location = New System.Drawing.Point(398, 11)
        Me.DN1Status.Name = "DN1Status"
        Me.DN1Status.Size = New System.Drawing.Size(20, 40)
        Me.DN1Status.TabIndex = 96
        Me.DN1Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DN2Status
        '
        Me.DN2Status.BackColor = System.Drawing.Color.Red
        Me.DN2Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DN2Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN2Status.Location = New System.Drawing.Point(398, 52)
        Me.DN2Status.Name = "DN2Status"
        Me.DN2Status.Size = New System.Drawing.Size(20, 40)
        Me.DN2Status.TabIndex = 97
        Me.DN2Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DN3Status
        '
        Me.DN3Status.BackColor = System.Drawing.Color.Red
        Me.DN3Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DN3Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN3Status.Location = New System.Drawing.Point(398, 95)
        Me.DN3Status.Name = "DN3Status"
        Me.DN3Status.Size = New System.Drawing.Size(20, 40)
        Me.DN3Status.TabIndex = 98
        Me.DN3Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DN4Status
        '
        Me.DN4Status.BackColor = System.Drawing.Color.Red
        Me.DN4Status.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.DN4Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DN4Status.Location = New System.Drawing.Point(398, 138)
        Me.DN4Status.Name = "DN4Status"
        Me.DN4Status.Size = New System.Drawing.Size(20, 40)
        Me.DN4Status.TabIndex = 99
        Me.DN4Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'SendBillAsCB
        '
        Me.SendBillAsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.SendBillAsCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SendBillAsCB.FormattingEnabled = True
        Me.SendBillAsCB.Items.AddRange(New Object() {"Image", "PDF"})
        Me.SendBillAsCB.Location = New System.Drawing.Point(695, 33)
        Me.SendBillAsCB.MaxDropDownItems = 100
        Me.SendBillAsCB.Name = "SendBillAsCB"
        Me.SendBillAsCB.Size = New System.Drawing.Size(200, 33)
        Me.SendBillAsCB.TabIndex = 43
        '
        'Label5
        '
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(489, 33)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(200, 28)
        Me.Label5.TabIndex = 101
        Me.Label5.Text = "Send Bill As"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ReduceAmtCB
        '
        Me.ReduceAmtCB.AutoSize = True
        Me.ReduceAmtCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ReduceAmtCB.Location = New System.Drawing.Point(489, 382)
        Me.ReduceAmtCB.Name = "ReduceAmtCB"
        Me.ReduceAmtCB.Size = New System.Drawing.Size(370, 20)
        Me.ReduceAmtCB.TabIndex = 48
        Me.ReduceAmtCB.Text = "Reduce amounts by 100x in notifications (Privacy Mode)"
        Me.ReduceAmtCB.UseVisualStyleBackColor = True
        '
        'UseKrutiDevCB
        '
        Me.UseKrutiDevCB.AutoSize = True
        Me.UseKrutiDevCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.UseKrutiDevCB.Location = New System.Drawing.Point(489, 408)
        Me.UseKrutiDevCB.Name = "UseKrutiDevCB"
        Me.UseKrutiDevCB.Size = New System.Drawing.Size(250, 20)
        Me.UseKrutiDevCB.TabIndex = 49
        Me.UseKrutiDevCB.Text = "Use KrutiDev in Print Name field"
        Me.UseKrutiDevCB.UseVisualStyleBackColor = True
        '
        'EnableHttpServerCB
        '
        Me.EnableHttpServerCB.AutoSize = True
        Me.EnableHttpServerCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EnableHttpServerCB.Location = New System.Drawing.Point(12, 382)
        Me.EnableHttpServerCB.Name = "EnableHttpServerCB"
        Me.EnableHttpServerCB.Size = New System.Drawing.Size(370, 20)
        Me.EnableHttpServerCB.TabIndex = 50
        Me.EnableHttpServerCB.Text = "Enable HTTP Server for SMS Integration"
        Me.EnableHttpServerCB.UseVisualStyleBackColor = True
        '
        'btnSMSHelp
        '
        Me.btnSMSHelp.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSMSHelp.Location = New System.Drawing.Point(388, 378)
        Me.btnSMSHelp.Name = "btnSMSHelp"
        Me.btnSMSHelp.Size = New System.Drawing.Size(95, 27)
        Me.btnSMSHelp.TabIndex = 51
        Me.btnSMSHelp.Text = "Setup Info"
        Me.btnSMSHelp.UseVisualStyleBackColor = True
        '
        'UserConsole
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.CancelButton = Me.CancelBtn
        Me.ClientSize = New System.Drawing.Size(907, 589)
        Me.ControlBox = False
        Me.Controls.Add(Me.EnableHttpServerCB)
        Me.Controls.Add(Me.btnSMSHelp)
        Me.Controls.Add(Me.UseKrutiDevCB)
        Me.Controls.Add(Me.ReduceAmtCB)
        Me.Controls.Add(Me.SendBillAsCB)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.DN4Status)
        Me.Controls.Add(Me.DN3Status)
        Me.Controls.Add(Me.DN2Status)
        Me.Controls.Add(Me.DN1Status)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.CompanyLbl)
        Me.Controls.Add(Me.BillPrnFrmtCB)
        Me.Controls.Add(Me.BillCptnTB)
        Me.Controls.Add(Me.Label27)
        Me.Controls.Add(Me.SndBilCpnCB)
        Me.Controls.Add(Me.Label29)
        Me.Controls.Add(Me.RcptPrnFrmtCB)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.RcptFrmtTB)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.SndRcptCstCB)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.RcptNtfGrpCB)
        Me.Controls.Add(Me.DispatchTB)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.RcptNtfFrmtTB)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.QRlbl)
        Me.Controls.Add(Me.DN4tb)
        Me.Controls.Add(Me.DN3tb)
        Me.Controls.Add(Me.DN2tb)
        Me.Controls.Add(Me.DN1tb)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.CancelBtn)
        Me.Controls.Add(Me.SubmitBtn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "UserConsole"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "User Console"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CancelBtn As Windows.Forms.Button
    Friend WithEvents SubmitBtn As Windows.Forms.Button
    Friend WithEvents Label9 As Windows.Forms.Label
    Friend WithEvents Label10 As Windows.Forms.Label
    Friend WithEvents Label11 As Windows.Forms.Label
    Friend WithEvents Label12 As Windows.Forms.Label
    Friend WithEvents Label13 As Windows.Forms.Label
    Friend WithEvents Label19 As Windows.Forms.Label
    Friend WithEvents Label20 As Windows.Forms.Label
    Friend WithEvents DN1tb As Windows.Forms.TextBox
    Friend WithEvents DN2tb As Windows.Forms.TextBox
    Friend WithEvents DN3tb As Windows.Forms.TextBox
    Friend WithEvents DN4tb As Windows.Forms.TextBox
    Friend WithEvents QRlbl As Windows.Forms.Label
    Friend WithEvents RcptNtfFrmtTB As Windows.Forms.TextBox
    Friend WithEvents Label15 As Windows.Forms.Label
    Friend WithEvents DispatchTB As Windows.Forms.TextBox
    Friend WithEvents Label17 As Windows.Forms.Label
    Friend WithEvents RcptNtfGrpCB As Windows.Forms.ComboBox
    Friend WithEvents SndRcptCstCB As Windows.Forms.ComboBox
    Friend WithEvents Label21 As Windows.Forms.Label
    Friend WithEvents RcptFrmtTB As Windows.Forms.TextBox
    Friend WithEvents Label23 As Windows.Forms.Label
    Friend WithEvents RcptPrnFrmtCB As Windows.Forms.ComboBox
    Friend WithEvents Label25 As Windows.Forms.Label
    Friend WithEvents BillCptnTB As Windows.Forms.TextBox
    Friend WithEvents Label27 As Windows.Forms.Label
    Friend WithEvents SndBilCpnCB As Windows.Forms.ComboBox
    Friend WithEvents Label29 As Windows.Forms.Label
    Friend WithEvents BillPrnFrmtCB As Windows.Forms.ComboBox
    Friend WithEvents CompanyLbl As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents DN1Status As Windows.Forms.Label
    Friend WithEvents DN2Status As Windows.Forms.Label
    Friend WithEvents DN3Status As Windows.Forms.Label
    Friend WithEvents DN4Status As Windows.Forms.Label
    Friend WithEvents SendBillAsCB As Windows.Forms.ComboBox
    Friend WithEvents Label5 As Windows.Forms.Label
    Friend WithEvents ReduceAmtCB As Windows.Forms.CheckBox
    Friend WithEvents UseKrutiDevCB As Windows.Forms.CheckBox
    Friend WithEvents EnableHttpServerCB As Windows.Forms.CheckBox
    Friend WithEvents btnSMSHelp As Windows.Forms.Button
End Class

