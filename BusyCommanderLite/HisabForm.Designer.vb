<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HisabForm
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.HisabDataGrid = New System.Windows.Forms.DataGridView()
        Me.Dinank = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Baki = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Jama = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Days = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PartyLabel = New System.Windows.Forms.Label()
        Me.TotalLabel = New System.Windows.Forms.Label()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        CType(Me.HisabDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'HisabDataGrid
        '
        Me.HisabDataGrid.AllowUserToAddRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.HisabDataGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.HisabDataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLightLight
        DataGridViewCellStyle2.Font = New System.Drawing.Font("MS Reference Sans Serif", 27.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.HisabDataGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.HisabDataGrid.ColumnHeadersHeight = 80
        Me.HisabDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.HisabDataGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Dinank, Me.Baki, Me.Jama, Me.Days})
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle7.Font = New System.Drawing.Font("MS Reference Sans Serif", 26.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.HisabDataGrid.DefaultCellStyle = DataGridViewCellStyle7
        Me.HisabDataGrid.EnableHeadersVisualStyles = False
        Me.HisabDataGrid.Location = New System.Drawing.Point(52, 107)
        Me.HisabDataGrid.Name = "HisabDataGrid"
        Me.HisabDataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.HisabDataGrid.RowHeadersVisible = False
        DataGridViewCellStyle8.Font = New System.Drawing.Font("MS Reference Sans Serif", 27.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HisabDataGrid.RowsDefaultCellStyle = DataGridViewCellStyle8
        Me.HisabDataGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.HisabDataGrid.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("MS Reference Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.HisabDataGrid.RowTemplate.Height = 55
        Me.HisabDataGrid.Size = New System.Drawing.Size(611, 150)
        Me.HisabDataGrid.TabIndex = 0
        '
        'Dinank
        '
        Me.Dinank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Dinank.DefaultCellStyle = DataGridViewCellStyle3
        Me.Dinank.HeaderText = "दिनांक"
        Me.Dinank.Name = "Dinank"
        '
        'Baki
        '
        Me.Baki.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Baki.DefaultCellStyle = DataGridViewCellStyle4
        Me.Baki.FillWeight = 80.0!
        Me.Baki.HeaderText = "बाकी"
        Me.Baki.Name = "Baki"
        '
        'Jama
        '
        Me.Jama.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Jama.DefaultCellStyle = DataGridViewCellStyle5
        Me.Jama.FillWeight = 80.0!
        Me.Jama.HeaderText = "जमा"
        Me.Jama.Name = "Jama"
        '
        'Days
        '
        Me.Days.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Days.DefaultCellStyle = DataGridViewCellStyle6
        Me.Days.FillWeight = 50.0!
        Me.Days.HeaderText = "समय"
        Me.Days.Name = "Days"
        '
        'PartyLabel
        '
        Me.PartyLabel.Font = New System.Drawing.Font("MS Reference Sans Serif", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PartyLabel.Location = New System.Drawing.Point(52, 40)
        Me.PartyLabel.Name = "PartyLabel"
        Me.PartyLabel.Size = New System.Drawing.Size(611, 62)
        Me.PartyLabel.TabIndex = 1
        Me.PartyLabel.Text = "Party Name Long Format"
        Me.PartyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TotalLabel
        '
        Me.TotalLabel.Font = New System.Drawing.Font("MS Reference Sans Serif", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TotalLabel.Location = New System.Drawing.Point(52, 260)
        Me.TotalLabel.Name = "TotalLabel"
        Me.TotalLabel.Size = New System.Drawing.Size(611, 62)
        Me.TotalLabel.TabIndex = 2
        Me.TotalLabel.Text = "कुल बकाया"
        Me.TotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.TotalLabel.Visible = False
        '
        'PrintDocument1
        '
        '
        'HisabForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ClientSize = New System.Drawing.Size(893, 391)
        Me.Controls.Add(Me.TotalLabel)
        Me.Controls.Add(Me.PartyLabel)
        Me.Controls.Add(Me.HisabDataGrid)
        Me.Name = "HisabForm"
        CType(Me.HisabDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents HisabDataGrid As System.Windows.Forms.DataGridView
    Friend WithEvents PartyLabel As System.Windows.Forms.Label
    Friend WithEvents TotalLabel As System.Windows.Forms.Label
    Friend WithEvents Dinank As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Baki As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Jama As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Days As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
End Class