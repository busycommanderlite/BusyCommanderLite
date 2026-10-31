<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BCMessageBox
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
        Me.lblMessage = New System.Windows.Forms.Label()
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
        'lblMessage
        '
        Me.lblMessage.BackColor = System.Drawing.SystemColors.Control
        Me.lblMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblMessage.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblMessage.Location = New System.Drawing.Point(8, 38)
        Me.lblMessage.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(360, 140)
        Me.lblMessage.TabIndex = 0
        Me.lblMessage.Text = "Message"
        Me.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBottom
        '
        Me.lblBottom.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblBottom.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblBottom.ForeColor = System.Drawing.Color.White
        Me.lblBottom.Location = New System.Drawing.Point(0, 186)
        Me.lblBottom.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblBottom.Name = "lblBottom"
        Me.lblBottom.Size = New System.Drawing.Size(375, 32)
        Me.lblBottom.TabIndex = 2
        Me.lblBottom.Text = "[ Enter - OK ]"
        Me.lblBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BCMessageBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(375, 218)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblBottom)
        Me.Controls.Add(Me.lblMessage)
        Me.Controls.Add(Me.lblTitle)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "BCMessageBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents lblBottom As System.Windows.Forms.Label
End Class
