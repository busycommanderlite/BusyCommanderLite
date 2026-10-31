<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BCDialogBox
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubheading = New System.Windows.Forms.Label()
        Me.txtInput = New System.Windows.Forms.TextBox()
        Me.lblBottom = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(375, 30)
        Me.lblTitle.TabIndex = 3
        Me.lblTitle.Text = "Title"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSubheading
        '
        Me.lblSubheading.BackColor = System.Drawing.SystemColors.Control
        Me.lblSubheading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblSubheading.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold)
        Me.lblSubheading.Location = New System.Drawing.Point(8, 38)
        Me.lblSubheading.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblSubheading.Name = "lblSubheading"
        Me.lblSubheading.Size = New System.Drawing.Size(360, 115)
        Me.lblSubheading.TabIndex = 0
        Me.lblSubheading.Text = "Line 1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Line 2" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Line 3" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Line 4"
        Me.lblSubheading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'txtInput
        '
        Me.txtInput.BackColor = System.Drawing.Color.White
        Me.txtInput.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtInput.ForeColor = System.Drawing.Color.MidnightBlue
        Me.txtInput.Location = New System.Drawing.Point(8, 160)
        Me.txtInput.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtInput.Name = "txtInput"
        Me.txtInput.Size = New System.Drawing.Size(360, 29)
        Me.txtInput.TabIndex = 1
        '
        'lblBottom
        '
        Me.lblBottom.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblBottom.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblBottom.ForeColor = System.Drawing.Color.White
        Me.lblBottom.Location = New System.Drawing.Point(0, 198)
        Me.lblBottom.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBottom.Name = "lblBottom"
        Me.lblBottom.Size = New System.Drawing.Size(375, 32)
        Me.lblBottom.TabIndex = 2
        Me.lblBottom.Text = "[ Esc - Quit  |  F2 - Done ]"
        Me.lblBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BCDialogBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(375, 230)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblBottom)
        Me.Controls.Add(Me.txtInput)
        Me.Controls.Add(Me.lblSubheading)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "BCDialogBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubheading As Label
    Friend WithEvents txtInput As TextBox
    Friend WithEvents lblBottom As Label
End Class