<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BCMobileBox
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Friend WithEvents lblSubtitle As System.Windows.Forms.Label
    Friend WithEvents pnlOptions As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents pnlBottom As System.Windows.Forms.Panel
    Friend WithEvents lblHint As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.pnlOptions = New System.Windows.Forms.FlowLayoutPanel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.pnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblSubtitle
        '
        Me.lblSubtitle.BackColor = System.Drawing.SystemColors.Control
        Me.lblSubtitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblSubtitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblSubtitle.ForeColor = System.Drawing.Color.Black
        Me.lblSubtitle.Location = New System.Drawing.Point(0, 0)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Size = New System.Drawing.Size(306, 40)
        Me.lblSubtitle.TabIndex = 0
        Me.lblSubtitle.Text = "Select Mobile Number"
        Me.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlOptions
        '
        Me.pnlOptions.AutoScroll = True
        Me.pnlOptions.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.pnlOptions.Location = New System.Drawing.Point(0, 40)
        Me.pnlOptions.Name = "pnlOptions"
        Me.pnlOptions.Padding = New System.Windows.Forms.Padding(20, 10, 20, 10)
        Me.pnlOptions.Size = New System.Drawing.Size(306, 261)
        Me.pnlOptions.TabIndex = 1
        Me.pnlOptions.WrapContents = False
        Me.pnlOptions.BorderStyle = BorderStyle.FixedSingle
        '
        'lblStatus
        '
        Me.lblStatus.BackColor = System.Drawing.Color.Firebrick
        Me.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblStatus.ForeColor = System.Drawing.Color.White
        Me.lblStatus.Location = New System.Drawing.Point(0, 304)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(306, 25)
        Me.lblStatus.TabIndex = 2
        Me.lblStatus.Text = "Enter a valid 10-digit mobile number."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblStatus.Visible = False
        '
        'pnlBottom
        '
        Me.pnlBottom.BackColor = System.Drawing.Color.MediumPurple
        Me.pnlBottom.Controls.Add(Me.lblHint)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 329)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(306, 28)
        Me.pnlBottom.TabIndex = 3
        '
        'lblHint
        '
        Me.lblHint.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblHint.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHint.ForeColor = System.Drawing.Color.White
        Me.lblHint.Location = New System.Drawing.Point(0, 0)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(306, 28)
        Me.lblHint.TabIndex = 0
        Me.lblHint.Text = "[ Esc - Quit  |  F2 - Done ]"
        Me.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BCMobileBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(306, 357)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.pnlOptions)
        Me.Controls.Add(Me.lblSubtitle)
        Me.Controls.Add(Me.pnlBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "BCMobileBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

End Class