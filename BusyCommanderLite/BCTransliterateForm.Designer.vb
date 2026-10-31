<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BCTransliterateForm
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
        Me.pnlTitle = New System.Windows.Forms.Panel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSubheading = New System.Windows.Forms.Label()
        Me.ResultsLB = New System.Windows.Forms.ListBox()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lblFooter = New System.Windows.Forms.Label()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.pnlTitle.SuspendLayout()
        Me.pnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTitle
        '
        Me.pnlTitle.BackColor = System.Drawing.Color.SteelBlue
        Me.pnlTitle.Controls.Add(Me.lblTitle)
        Me.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTitle.Location = New System.Drawing.Point(0, 0)
        Me.pnlTitle.Name = "pnlTitle"
        Me.pnlTitle.Size = New System.Drawing.Size(522, 32)
        Me.pnlTitle.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblTitle.ForeColor = System.Drawing.Color.White
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(522, 32)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Transliterate to Kruti Dev"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblSubheading
        '
        Me.lblSubheading.BackColor = System.Drawing.SystemColors.Control
        Me.lblSubheading.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblSubheading.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.lblSubheading.Location = New System.Drawing.Point(0, 32)
        Me.lblSubheading.Name = "lblSubheading"
        Me.lblSubheading.Size = New System.Drawing.Size(522, 40)
        Me.lblSubheading.TabIndex = 1
        Me.lblSubheading.Text = "Select a Hindi suggestion from the list below"
        Me.lblSubheading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ResultsLB
        '
        Me.ResultsLB.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ResultsLB.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!)
        Me.ResultsLB.FormattingEnabled = True
        Me.ResultsLB.ItemHeight = 29
        Me.ResultsLB.Location = New System.Drawing.Point(0, 72)
        Me.ResultsLB.Name = "ResultsLB"
        Me.ResultsLB.Size = New System.Drawing.Size(522, 153)
        Me.ResultsLB.TabIndex = 2
        '
        'pnlBottom
        '
        Me.pnlBottom.BackColor = System.Drawing.Color.SteelBlue
        Me.pnlBottom.Controls.Add(Me.lblFooter)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 225)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(522, 28)
        Me.pnlBottom.TabIndex = 3
        '
        'lblFooter
        '
        Me.lblFooter.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblFooter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblFooter.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblFooter.ForeColor = System.Drawing.Color.White
        Me.lblFooter.Location = New System.Drawing.Point(0, 0)
        Me.lblFooter.Name = "lblFooter"
        Me.lblFooter.Size = New System.Drawing.Size(522, 28)
        Me.lblFooter.TabIndex = 0
        Me.lblFooter.Text = "[ Esc - Quit  |  F2 - Done ]"
        Me.lblFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(0, 285)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(522, 88)
        Me.WebBrowser1.TabIndex = 4
        Me.WebBrowser1.Visible = False
        '
        'BCTransliterateForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(522, 253)
        Me.ControlBox = False
        Me.Controls.Add(Me.ResultsLB)
        Me.Controls.Add(Me.lblSubheading)
        Me.Controls.Add(Me.pnlTitle)
        Me.Controls.Add(Me.pnlBottom)
        Me.Controls.Add(Me.WebBrowser1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "BCTransliterateForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        Me.pnlTitle.ResumeLayout(False)
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTitle As Panel
    Friend WithEvents lblTitle As Label
    Friend WithEvents lblSubheading As Label
    Friend WithEvents ResultsLB As ListBox
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents lblFooter As Label
    Friend WithEvents WebBrowser1 As WebBrowser

End Class