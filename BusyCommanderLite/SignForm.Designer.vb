<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SignForm
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
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.SignGrid = New System.Windows.Forms.DataGridView()
        Me.Nag = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Description = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PartyName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.DateLabel = New System.Windows.Forms.Label()
        Me.TotalLabel = New System.Windows.Forms.Label()
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument()
        CType(Me.SignGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SignGrid
        '
        Me.SignGrid.AllowUserToAddRows = False
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.SignGrid.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.SignGrid.BackgroundColor = System.Drawing.Color.White
        Me.SignGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised
        Me.SignGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLightLight
        DataGridViewCellStyle2.Font = New System.Drawing.Font("MS Reference Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlLightLight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.SignGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.SignGrid.ColumnHeadersHeight = 80
        Me.SignGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        Me.SignGrid.ColumnHeadersVisible = False
        Me.SignGrid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Nag, Me.Description, Me.PartyName})
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle6.Font = New System.Drawing.Font("MS Reference Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.SignGrid.DefaultCellStyle = DataGridViewCellStyle6
        Me.SignGrid.EnableHeadersVisualStyles = False
        Me.SignGrid.Location = New System.Drawing.Point(12, 80)
        Me.SignGrid.Name = "SignGrid"
        Me.SignGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.SignGrid.RowHeadersVisible = False
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle7.Font = New System.Drawing.Font("MS Reference Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SignGrid.RowsDefaultCellStyle = DataGridViewCellStyle7
        Me.SignGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.SignGrid.RowTemplate.DefaultCellStyle.Font = New System.Drawing.Font("MS Reference Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SignGrid.RowTemplate.Height = 55
        Me.SignGrid.Size = New System.Drawing.Size(1263, 332)
        Me.SignGrid.TabIndex = 1
        '
        'Nag
        '
        Me.Nag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.Nag.DefaultCellStyle = DataGridViewCellStyle3
        Me.Nag.FillWeight = 12.0!
        Me.Nag.HeaderText = "Nag"
        Me.Nag.Name = "Nag"
        '
        'Description
        '
        Me.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.Description.DefaultCellStyle = DataGridViewCellStyle4
        Me.Description.FillWeight = 20.0!
        Me.Description.HeaderText = "Description"
        Me.Description.Name = "Description"
        '
        'PartyName
        '
        Me.PartyName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        Me.PartyName.DefaultCellStyle = DataGridViewCellStyle5
        Me.PartyName.FillWeight = 68.0!
        Me.PartyName.HeaderText = "PartyName"
        Me.PartyName.Name = "PartyName"
        '
        'DateLabel
        '
        Me.DateLabel.BackColor = System.Drawing.Color.White
        Me.DateLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DateLabel.Location = New System.Drawing.Point(12, 9)
        Me.DateLabel.Name = "DateLabel"
        Me.DateLabel.Size = New System.Drawing.Size(1263, 68)
        Me.DateLabel.TabIndex = 2
        Me.DateLabel.Text = "15-09-2023 05:33 PM"
        Me.DateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TotalLabel
        '
        Me.TotalLabel.BackColor = System.Drawing.Color.White
        Me.TotalLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TotalLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.TotalLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TotalLabel.Location = New System.Drawing.Point(12, 415)
        Me.TotalLabel.Name = "TotalLabel"
        Me.TotalLabel.Size = New System.Drawing.Size(1263, 68)
        Me.TotalLabel.TabIndex = 3
        Me.TotalLabel.Text = "कुल 5 डाग             दस्तखत:-"
        Me.TotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PrintDocument1
        '
        '
        'SignForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1287, 614)
        Me.Controls.Add(Me.TotalLabel)
        Me.Controls.Add(Me.DateLabel)
        Me.Controls.Add(Me.SignGrid)
        Me.Name = "SignForm"
        Me.Text = "SignForm"
        CType(Me.SignGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SignGrid As System.Windows.Forms.DataGridView
    Friend WithEvents DateLabel As System.Windows.Forms.Label
    Friend WithEvents TotalLabel As System.Windows.Forms.Label
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
    Friend WithEvents Nag As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Description As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PartyName As System.Windows.Forms.DataGridViewTextBoxColumn
End Class