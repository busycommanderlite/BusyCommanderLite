<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BrowserForm
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
        Me.components = New System.ComponentModel.Container()
        Me.qrPictureBox = New System.Windows.Forms.PictureBox()
        Me.statusLabel = New System.Windows.Forms.Label()
        Me.pollTimer = New System.Windows.Forms.Timer(Me.components)
        CType(Me.qrPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'qrPictureBox
        '
        Me.qrPictureBox.BackColor = System.Drawing.Color.White
        Me.qrPictureBox.Location = New System.Drawing.Point(12, 50)
        Me.qrPictureBox.Name = "qrPictureBox"
        Me.qrPictureBox.Size = New System.Drawing.Size(776, 488)
        Me.qrPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.qrPictureBox.TabIndex = 0
        Me.qrPictureBox.TabStop = False
        '
        'statusLabel
        '
        Me.statusLabel.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.statusLabel.Location = New System.Drawing.Point(12, 12)
        Me.statusLabel.Name = "statusLabel"
        Me.statusLabel.Size = New System.Drawing.Size(776, 30)
        Me.statusLabel.TabIndex = 1
        Me.statusLabel.Text = "Initializing..."
        Me.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pollTimer
        '
        Me.pollTimer.Interval = 2000
        '
        'BrowserForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 550)
        Me.Controls.Add(Me.statusLabel)
        Me.Controls.Add(Me.qrPictureBox)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "BrowserForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        CType(Me.qrPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents qrPictureBox As System.Windows.Forms.PictureBox
    Friend WithEvents statusLabel As System.Windows.Forms.Label
    Friend WithEvents pollTimer As System.Windows.Forms.Timer
End Class