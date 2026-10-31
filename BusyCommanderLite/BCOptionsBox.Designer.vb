<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BCOptionsBox
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
    Friend WithEvents dgvOptions As System.Windows.Forms.DataGridView
    Friend WithEvents pnlBottom As System.Windows.Forms.Panel
    Friend WithEvents lblHint As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.dgvOptions = New System.Windows.Forms.DataGridView()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lblHint = New System.Windows.Forms.Label()
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblSubtitle.Size = New System.Drawing.Size(600, 40)
        Me.lblSubtitle.TabIndex = 0
        Me.lblSubtitle.Text = "SUBTITLE"
        Me.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'dgvOptions
        '
        Me.dgvOptions.AllowUserToAddRows = False
        Me.dgvOptions.AllowUserToDeleteRows = False
        Me.dgvOptions.AllowUserToResizeColumns = False
        Me.dgvOptions.AllowUserToResizeRows = False
        Me.dgvOptions.BackgroundColor = System.Drawing.SystemColors.Control
        Me.dgvOptions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.dgvOptions.ColumnHeadersHeight = 34
        Me.dgvOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.dgvOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOptions.EnableHeadersVisualStyles = False
        Me.dgvOptions.GridColor = System.Drawing.Color.LightGray
        Me.dgvOptions.Location = New System.Drawing.Point(0, 40)
        Me.dgvOptions.Name = "dgvOptions"
        Me.dgvOptions.ReadOnly = True
        Me.dgvOptions.RowHeadersVisible = False
        Me.dgvOptions.RowTemplate.Height = 28
        Me.dgvOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvOptions.Size = New System.Drawing.Size(600, 310)
        Me.dgvOptions.TabIndex = 1
        '
        'pnlBottom
        '
        Me.pnlBottom.BackColor = System.Drawing.Color.MediumPurple
        Me.pnlBottom.Controls.Add(Me.lblHint)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 350)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(600, 28)
        Me.pnlBottom.TabIndex = 2
        '
        'lblHint
        '
        Me.lblHint.BackColor = System.Drawing.Color.MediumSlateBlue
        Me.lblHint.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblHint.ForeColor = System.Drawing.Color.White
        Me.lblHint.Location = New System.Drawing.Point(0, 0)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(600, 28)
        Me.lblHint.TabIndex = 0
        Me.lblHint.Text = "[ Esc - Quit  |  F2 - Done ]"
        Me.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BCOptionsBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(600, 378)
        Me.ControlBox = False
        Me.Controls.Add(Me.dgvOptions)
        Me.Controls.Add(Me.lblSubtitle)
        Me.Controls.Add(Me.pnlBottom)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.KeyPreview = True
        Me.Name = "BCOptionsBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.TopMost = True
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
End Class