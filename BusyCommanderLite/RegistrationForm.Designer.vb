<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RegistrationForm
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
        Me.SubmitBtn = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CompanyCB = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BusyDataPathTB = New System.Windows.Forms.TextBox()
        Me.StatusLbl = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'SubmitBtn
        '
        Me.SubmitBtn.Enabled = False
        Me.SubmitBtn.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SubmitBtn.Location = New System.Drawing.Point(3, 286)
        Me.SubmitBtn.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.SubmitBtn.Name = "SubmitBtn"
        Me.SubmitBtn.Size = New System.Drawing.Size(297, 79)
        Me.SubmitBtn.TabIndex = 4
        Me.SubmitBtn.Text = "Submit"
        Me.SubmitBtn.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(55, 9)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(170, 36)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Installation"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(2, 159)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(157, 25)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Select Company"
        '
        'CompanyCB
        '
        Me.CompanyCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CompanyCB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CompanyCB.FormattingEnabled = True
        Me.CompanyCB.Location = New System.Drawing.Point(7, 188)
        Me.CompanyCB.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.CompanyCB.MaxDropDownItems = 100
        Me.CompanyCB.Name = "CompanyCB"
        Me.CompanyCB.Size = New System.Drawing.Size(293, 33)
        Me.CompanyCB.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(3, 372)
        Me.Button1.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(297, 79)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Cancel"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(-1, 86)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(158, 25)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Data Folder Path"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BusyDataPathTB
        '
        Me.BusyDataPathTB.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BusyDataPathTB.Location = New System.Drawing.Point(4, 115)
        Me.BusyDataPathTB.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.BusyDataPathTB.Name = "BusyDataPathTB"
        Me.BusyDataPathTB.Size = New System.Drawing.Size(296, 34)
        Me.BusyDataPathTB.TabIndex = 10
        '
        'StatusLbl
        '
        Me.StatusLbl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusLbl.Location = New System.Drawing.Point(10, 227)
        Me.StatusLbl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.StatusLbl.Name = "StatusLbl"
        Me.StatusLbl.Size = New System.Drawing.Size(292, 27)
        Me.StatusLbl.TabIndex = 12
        Me.StatusLbl.Text = "No Companies Found"
        Me.StatusLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'RegistrationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(313, 482)
        Me.ControlBox = False
        Me.Controls.Add(Me.StatusLbl)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.BusyDataPathTB)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.CompanyCB)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.SubmitBtn)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
        Me.Name = "RegistrationForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents SubmitBtn As Windows.Forms.Button
    Friend WithEvents Label3 As Windows.Forms.Label
    Friend WithEvents Label6 As Windows.Forms.Label
    Friend WithEvents CompanyCB As Windows.Forms.ComboBox
    Friend WithEvents Button1 As Windows.Forms.Button
    Friend WithEvents Label4 As Label
    Friend WithEvents BusyDataPathTB As TextBox
    Friend WithEvents StatusLbl As Label
End Class