Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Linq
Imports System.Windows.Forms

Public Class DispatchManagerForm
    Inherits Form

    Private WithEvents dgvDispatches As DataGridView
    Private WithEvents lblTitle As Label

    Public UpdatedVchNo As String = ""

    Private currentDispatches As List(Of DispatchRecord)
    Private loadedCount As Integer = 0
    Private isAppendingBatch As Boolean = False
    Private isLoading As Boolean = False
    Private ReadOnly loadedIds As New HashSet(Of Integer)()

    Private Shared ReadOnly FontPendingMain As New Font("Segoe UI Semibold", 15.0F, FontStyle.Bold)
    Private Shared ReadOnly FontPendingDate As New Font("Segoe UI Semibold", 10.5F, FontStyle.Bold)
    Private Shared ReadOnly FontCompletedMain As New Font("Segoe UI", 15.0F, FontStyle.Regular)
    Private Shared ReadOnly FontCompletedDate As New Font("Segoe UI", 10.5F, FontStyle.Regular)

    Public Sub New()
        InitializeComponent()
    End Sub


    Private Sub InitializeComponent()
        Me.dgvDispatches = New DataGridView()
        Me.lblTitle = New Label()

        CType(Me.dgvDispatches, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()

        ' lblTitle — MediumSlateBlue header (BCDialogBox style)
        Me.lblTitle.Dock = DockStyle.Top
        Me.lblTitle.Height = 40
        Me.lblTitle.BackColor = Color.MediumSlateBlue
        Me.lblTitle.ForeColor = Color.White
        Me.lblTitle.Font = New Font("Segoe UI", 16.0F, FontStyle.Bold)
        Me.lblTitle.Text = "UnUpdated Dispatches"
        Me.lblTitle.TextAlign = ContentAlignment.MiddleCenter

        ' dgvDispatches
        Me.dgvDispatches.Dock = DockStyle.Fill
        Me.dgvDispatches.AllowUserToAddRows = False
        Me.dgvDispatches.AllowUserToDeleteRows = False
        Me.dgvDispatches.AllowUserToResizeRows = False
        Me.dgvDispatches.ReadOnly = True
        Me.dgvDispatches.MultiSelect = False
        Me.dgvDispatches.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvDispatches.BackgroundColor = Color.White
        Me.dgvDispatches.BorderStyle = BorderStyle.None
        Me.dgvDispatches.RowHeadersVisible = False
        Me.dgvDispatches.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvDispatches.Font = New Font("Segoe UI", 15.0F)
        Me.dgvDispatches.ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 15.0F, FontStyle.Bold)
        Me.dgvDispatches.ColumnHeadersDefaultCellStyle.BackColor = Color.MediumSlateBlue
        Me.dgvDispatches.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.dgvDispatches.EnableHeadersVisualStyles = False
        Me.dgvDispatches.ColumnHeadersHeight = 40
        Me.dgvDispatches.RowTemplate.Height = 38
        Me.dgvDispatches.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 220, 255)
        Me.dgvDispatches.DefaultCellStyle.SelectionForeColor = Color.Black

        ' Form — no title bar (BCDialogBox style)
        Me.AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.WhiteSmoke
        Me.ClientSize = New Size(780, 250)
        Me.Controls.Add(Me.dgvDispatches)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.FormBorderStyle = FormBorderStyle.None
        Me.KeyPreview = True
        Me.MinimumSize = New Size(450, 300)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.ShowInTaskbar = False
        Me.ControlBox = False
        Me.Text = "UnUpdated Dispatches"

        CType(Me.dgvDispatches, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        ' Draw a 2px MediumSlateBlue border around the entire form
        Using pen As New Pen(Color.MediumSlateBlue, 2)
            e.Graphics.DrawRectangle(pen, 1, 1, Me.ClientSize.Width - 2, Me.ClientSize.Height - 2)
        End Using
    End Sub

    Private Sub DispatchManagerForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        SetupGridColumns()
        LoadDispatches()
    End Sub

    Private Sub SetupGridColumns()
        dgvDispatches.Columns.Clear()

        ' Hidden data columns
        Dim colId As New DataGridViewTextBoxColumn With {
            .Name = "Id",
            .HeaderText = "ID",
            .Visible = False
        }
        Dim colGiven As New DataGridViewTextBoxColumn With {
            .Name = "GivenGadiNo",
            .HeaderText = "GivenGadi",
            .Visible = False
        }
        Dim colVchNo As New DataGridViewTextBoxColumn With {
            .Name = "VchNo",
            .HeaderText = "VchNo",
            .Visible = False
        }

        ' Visible columns
        Dim colDate As New DataGridViewTextBoxColumn With {
            .Name = "Date",
            .HeaderText = "Date",
            .FillWeight = 110
        }
        Dim colParty As New DataGridViewTextBoxColumn With {
            .Name = "PartyName",
            .HeaderText = "Party Name",
            .FillWeight = 200
        }
        Dim colNag As New DataGridViewTextBoxColumn With {
            .Name = "Nag",
            .HeaderText = "Nag",
            .FillWeight = 40
        }
        Dim colActual As New DataGridViewTextBoxColumn With {
            .Name = "ActualGadiNo",
            .HeaderText = "Gadi",
            .FillWeight = 120
        }

        dgvDispatches.Columns.AddRange({colId, colGiven, colVchNo, colDate, colParty, colNag, colActual})
    End Sub

    Private Sub UpdateTitleCounter()
        If currentDispatches Is Nothing OrElse currentDispatches.Count = 0 Then
            lblTitle.Text = "UnUpdated Dispatches (0 records)"
        Else
            lblTitle.Text = String.Format("UnUpdated Dispatches (Showing {0} of {1})", dgvDispatches.Rows.Count, currentDispatches.Count)
        End If
    End Sub

    Private Sub AddRecordRow(rec As DispatchRecord)
        If rec Is Nothing Then Return
        If loadedIds.Contains(rec.Id) Then Return
        loadedIds.Add(rec.Id)

        Dim idx = dgvDispatches.Rows.Add(rec.Id, rec.GivenGadiNo, rec.VchNo, rec.CreatedAt.ToString("dd-MMM-yy HH:mm"), rec.PartyName, rec.Nag, rec.ActualGadiNo)
        Dim row = dgvDispatches.Rows(idx)
        If String.IsNullOrWhiteSpace(rec.ActualGadiNo) Then
            ' PENDING — bold orange-red
            row.DefaultCellStyle.ForeColor = Color.OrangeRed
            row.DefaultCellStyle.Font = FontPendingMain
            row.Cells("Date").Style.Font = FontPendingDate
        Else
            ' COMPLETED — normal dark green
            row.DefaultCellStyle.ForeColor = Color.DarkGreen
            row.DefaultCellStyle.Font = FontCompletedMain
            row.Cells("Date").Style.Font = FontCompletedDate
        End If
    End Sub

    Private Sub AppendBatch(batchSize As Integer)
        If isLoading OrElse isAppendingBatch Then Return
        If currentDispatches Is Nothing Then Return
        If loadedCount >= currentDispatches.Count Then Return

        isAppendingBatch = True
        dgvDispatches.SuspendLayout()
        Try
            Dim countToAdd = Math.Min(batchSize, currentDispatches.Count - loadedCount)
            For i As Integer = 0 To countToAdd - 1
                AddRecordRow(currentDispatches(loadedCount + i))
            Next
            loadedCount += countToAdd
            UpdateTitleCounter()
        Finally
            dgvDispatches.ResumeLayout()
            isAppendingBatch = False
        End Try
    End Sub

    Private Sub LoadDispatches()
        isLoading = True
        dgvDispatches.SuspendLayout()
        Try
            ' Get ALL dispatches; sort pending (orange-red) on top, completed (green) at bottom
            Dim allList As List(Of DispatchRecord) = DispatchRepository.GetAllDispatches()

            Dim pending = allList.Where(Function(r) String.IsNullOrWhiteSpace(r.ActualGadiNo)) _
                                 .OrderByDescending(Function(r) r.Id).ToList()
            Dim completed = allList.Where(Function(r) Not String.IsNullOrWhiteSpace(r.ActualGadiNo)) _
                                   .OrderByDescending(Function(r) r.Id).ToList()

            currentDispatches = pending.Concat(completed).ToList()
            dgvDispatches.Rows.Clear()
            loadedIds.Clear()
            loadedCount = 0

            If currentDispatches.Count = 0 Then
                UpdateTitleCounter()
                Return
            End If

            ' Initial batch: strictly 10 entries (or all if fewer than 10)
            Dim initialCount = Math.Min(10, currentDispatches.Count)

            For i As Integer = 0 To initialCount - 1
                AddRecordRow(currentDispatches(i))
            Next
            loadedCount = initialCount
            UpdateTitleCounter()

            If dgvDispatches.Rows.Count > 0 Then
                dgvDispatches.Rows(0).Selected = True
                dgvDispatches.FirstDisplayedScrollingRowIndex = 0
            End If
        Finally
            isLoading = False
            dgvDispatches.ResumeLayout()
        End Try

        dgvDispatches.Focus()
    End Sub

    Private Sub ReturnVchNoAndClose()
        If dgvDispatches.SelectedRows.Count = 0 Then Return

        Dim row = dgvDispatches.SelectedRows(0)
        Dim vchNo = Convert.ToString(row.Cells("VchNo").Value)

        Me.UpdatedVchNo = vchNo
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Function GetNextPendingId(currentId As Integer) As Integer
        If currentDispatches Is Nothing Then Return -1
        Dim nextPending = currentDispatches.Where(Function(r) String.IsNullOrWhiteSpace(r.ActualGadiNo) AndAlso r.Id <> currentId) _
                                           .OrderByDescending(Function(r) r.Id).ToList()
        If nextPending.Count > 0 Then Return nextPending(0).Id
        Return -1
    End Function

    Private Sub SelectRowById(id As Integer)
        If id <= 0 Then
            If dgvDispatches.Rows.Count > 0 Then
                dgvDispatches.Rows(0).Selected = True
            End If
        Else
            For Each r As DataGridViewRow In dgvDispatches.Rows
                If Convert.ToInt32(r.Cells("Id").Value) = id Then
                    r.Selected = True
                    dgvDispatches.FirstDisplayedScrollingRowIndex = r.Index
                    Exit For
                End If
            Next
        End If
        dgvDispatches.Focus()
    End Sub

    Private Sub DeleteSelectedRow()
        If dgvDispatches.SelectedRows.Count = 0 Then Return

        Dim row = dgvDispatches.SelectedRows(0)
        Dim id = Convert.ToInt32(row.Cells("Id").Value)
        Dim partyName = Convert.ToString(row.Cells("PartyName").Value)
        Dim nag = Convert.ToString(row.Cells("Nag").Value)
        Dim delIdx = row.Index

        Dim confirm = MessageBox.Show(
            "Delete dispatch for " & partyName & " (Nag: " & nag & ")?" & Environment.NewLine &
            "This cannot be undone.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

        If confirm = DialogResult.Yes Then
            DispatchRepository.DeleteDispatch(id)

            isLoading = True
            Try
                dgvDispatches.Rows.RemoveAt(delIdx)
                loadedIds.Remove(id)
                If currentDispatches IsNot Nothing Then
                    currentDispatches.RemoveAll(Function(r) r.Id = id)
                End If
                loadedCount = Math.Max(0, loadedCount - 1)
                UpdateTitleCounter()

                If dgvDispatches.Rows.Count > 0 Then
                    Dim newSelIdx = Math.Min(delIdx, dgvDispatches.Rows.Count - 1)
                    dgvDispatches.Rows(newSelIdx).Selected = True
                End If
            Finally
                isLoading = False
            End Try
        End If
    End Sub

    Private Function Clean10DigitPhone(num As String) As String
        If String.IsNullOrWhiteSpace(num) Then Return ""
        Dim clean = New String(num.Where(Function(c) Char.IsDigit(c)).ToArray())
        If clean.Length >= 10 Then
            Return clean.Substring(clean.Length - 10)
        End If
        Return ""
    End Function

    Private openTime As DateTime = DateTime.Now

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If (DateTime.Now - openTime).TotalMilliseconds < 300 AndAlso (keyData = Keys.Enter OrElse keyData = Keys.Escape) Then
            Return True
        End If

        Select Case keyData
            Case Keys.Down, Keys.PageDown, Keys.End
                If Not isLoading AndAlso Not isAppendingBatch AndAlso dgvDispatches.SelectedRows.Count > 0 Then
                    Dim selIdx = dgvDispatches.SelectedRows(0).Index
                    If selIdx >= (dgvDispatches.Rows.Count - 2) AndAlso currentDispatches IsNot Nothing AndAlso loadedCount < currentDispatches.Count Then
                        AppendBatch(50)
                    End If
                End If
            Case Keys.Escape
                Return True
            Case Keys.Enter
                ' Suppress Enter in ProcessCmdKey to prevent DataGridView from moving row down
                ' The actual action will be handled in dgvDispatches_KeyUp
                If dgvDispatches.Focused OrElse Me.Focused Then Return True
            Case Keys.Delete
                If dgvDispatches.Focused OrElse Me.Focused Then
                    DeleteSelectedRow()
                    Return True
                End If
            Case Keys.F5
                LoadDispatches()
                Return True
        End Select
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub DispatchManagerForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            Activate()
            dgvDispatches.Focus()
        Catch
        End Try
    End Sub

    Private Sub dgvDispatches_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDispatches.CellDoubleClick
        If e.RowIndex >= 0 Then
            ReturnVchNoAndClose()
        End If
    End Sub

    Private Sub dgvDispatches_KeyUp(sender As Object, e As KeyEventArgs) Handles dgvDispatches.KeyUp, Me.KeyUp
        If e.KeyCode = Keys.Enter Then
            ReturnVchNoAndClose()
        ElseIf e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub dgvDispatches_Scroll(sender As Object, e As ScrollEventArgs) Handles dgvDispatches.Scroll
        If isLoading OrElse isAppendingBatch Then Return
        Try
            Dim visibleRows = dgvDispatches.DisplayedRowCount(False)
            Dim firstRow = dgvDispatches.FirstDisplayedScrollingRowIndex
            If firstRow > 0 AndAlso (firstRow + visibleRows) >= (dgvDispatches.Rows.Count - 2) Then
                If currentDispatches IsNot Nothing AndAlso loadedCount < currentDispatches.Count Then
                    AppendBatch(50)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub dgvDispatches_SelectionChanged(sender As Object, e As EventArgs) Handles dgvDispatches.SelectionChanged
        If isLoading OrElse isAppendingBatch Then Return
        Try
            If dgvDispatches.Rows.Count >= 10 AndAlso dgvDispatches.SelectedRows.Count > 0 Then
                Dim selIdx = dgvDispatches.SelectedRows(0).Index
                If selIdx >= (dgvDispatches.Rows.Count - 1) Then
                    If currentDispatches IsNot Nothing AndAlso loadedCount < currentDispatches.Count Then
                        AppendBatch(50)
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

End Class
