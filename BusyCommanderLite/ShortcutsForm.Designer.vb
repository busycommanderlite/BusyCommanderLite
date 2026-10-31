<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ShortcutsForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer
    Private rtbShortcuts As System.Windows.Forms.RichTextBox
    Private pnlBottom As System.Windows.Forms.Panel
    Private lblHint As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ShortcutsForm))
        Me.rtbShortcuts = New System.Windows.Forms.RichTextBox()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.pnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'rtbShortcuts
        '
        Me.rtbShortcuts.BackColor = System.Drawing.Color.White
        Me.rtbShortcuts.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbShortcuts.Font = New System.Drawing.Font("Consolas", 16.0!, System.Drawing.FontStyle.Bold)
        Me.rtbShortcuts.Location = New System.Drawing.Point(0, 0)
        Me.rtbShortcuts.Name = "rtbShortcuts"
        Me.rtbShortcuts.ReadOnly = True
        Me.rtbShortcuts.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        Me.rtbShortcuts.Size = New System.Drawing.Size(600, 499)
        Me.rtbShortcuts.TabIndex = 0
        Me.rtbShortcuts.Text = resources.GetString("rtbShortcuts")
        '
        'pnlBottom
        '
        Me.pnlBottom.BackColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer), CType(CType(220, Byte), Integer))
        Me.pnlBottom.Controls.Add(Me.lblHint)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 502)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(600, 30)
        Me.pnlBottom.TabIndex = 1
        '
        'lblHint
        '
        Me.lblHint.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Italic)
        Me.lblHint.ForeColor = System.Drawing.Color.Black
        Me.lblHint.Location = New System.Drawing.Point(0, 0)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(600, 30)
        Me.lblHint.TabIndex = 0
        Me.lblHint.Text = "Press Esc to Close"
        Me.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ShortcutsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(600, 532)
        Me.ControlBox = False
        Me.Controls.Add(Me.rtbShortcuts)
        Me.Controls.Add(Me.pnlBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "ShortcutsForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
End Class