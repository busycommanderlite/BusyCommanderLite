<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SMSHelpForm
    Inherits System.Windows.Forms.Form

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtURL = New System.Windows.Forms.TextBox()
        Me.btnCopyUrl = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.optPdf = New System.Windows.Forms.RadioButton()
        Me.optImage = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(315, 21)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Busy WhatsApp / SMS Integration Setup"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(17, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(496, 15)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "To send WhatsApp messages directly from Busy's Print Menu, configure the following URL:"
        '
        'optPdf
        '
        Me.optPdf.AutoSize = True
        Me.optPdf.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optPdf.Location = New System.Drawing.Point(20, 68)
        Me.optPdf.Name = "optPdf"
        Me.optPdf.Size = New System.Drawing.Size(209, 19)
        Me.optPdf.TabIndex = 2
        Me.optPdf.TabStop = True
        Me.optPdf.Text = "I want to send bills as PDF files"
        Me.optPdf.UseVisualStyleBackColor = True
        '
        'optImage
        '
        Me.optImage.AutoSize = True
        Me.optImage.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.optImage.Location = New System.Drawing.Point(260, 68)
        Me.optImage.Name = "optImage"
        Me.optImage.Size = New System.Drawing.Size(193, 19)
        Me.optImage.TabIndex = 3
        Me.optImage.TabStop = True
        Me.optImage.Text = "I want to send bills as Images"
        Me.optImage.UseVisualStyleBackColor = True
        '
        'txtURL
        '
        Me.txtURL.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtURL.Location = New System.Drawing.Point(20, 96)
        Me.txtURL.Name = "txtURL"
        Me.txtURL.ReadOnly = True
        Me.txtURL.Size = New System.Drawing.Size(535, 23)
        Me.txtURL.TabIndex = 4
        Me.txtURL.Text = "http://localhost:54321/send?phoneNo=mobNum&messageText=message"
        '
        'btnCopyUrl
        '
        Me.btnCopyUrl.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCopyUrl.Location = New System.Drawing.Point(565, 94)
        Me.btnCopyUrl.Name = "btnCopyUrl"
        Me.btnCopyUrl.Size = New System.Drawing.Size(100, 27)
        Me.btnCopyUrl.TabIndex = 5
        Me.btnCopyUrl.Text = "Copy URL"
        Me.btnCopyUrl.UseVisualStyleBackColor = True
        '
        'lblHint
        '
        Me.lblHint.AutoSize = True
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHint.ForeColor = System.Drawing.Color.DimGray
        Me.lblHint.Location = New System.Drawing.Point(17, 134)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(431, 13)
        Me.lblHint.TabIndex = 6
        Me.lblHint.Text = "Configure this in Busy under Administration -> Configuration -> SMS Configuration."
        '
        'btnClose
        '
        Me.btnClose.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClose.Location = New System.Drawing.Point(565, 127)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(100, 27)
        Me.btnClose.TabIndex = 7
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'SMSHelpForm
        '
        Me.AcceptButton = Me.btnCopyUrl
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnClose
        Me.ClientSize = New System.Drawing.Size(684, 168)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.lblHint)
        Me.Controls.Add(Me.btnCopyUrl)
        Me.Controls.Add(Me.txtURL)
        Me.Controls.Add(Me.optImage)
        Me.Controls.Add(Me.optPdf)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SMSHelpForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "WhatsApp / SMS Configuration Guide"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtURL As System.Windows.Forms.TextBox
    Friend WithEvents btnCopyUrl As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents optPdf As System.Windows.Forms.RadioButton
    Friend WithEvents optImage As System.Windows.Forms.RadioButton
End Class
