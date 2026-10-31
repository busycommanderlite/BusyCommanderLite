Public Class BCOptionsBox
    Public SelectedTransport As String = ""
    Private openTime As DateTime

    Public Function ShowDialogWithOptions(title As String, subtitle As String, dataString As String, Optional owner As IWin32Window = Nothing) As String
        ' Set window title and subtitle
        Text = title
        lblSubtitle.Text = subtitle
        lblSubtitle.Font = New Font("Segoe UI", 11, FontStyle.Bold)
        lblSubtitle.ForeColor = Color.Navy

        ' Setup grid columns
        dgvOptions.Columns.Clear()
        dgvOptions.Columns.Add("Transport", "Transport Details")
        dgvOptions.Columns.Add("Party", "Party")
        dgvOptions.Columns.Add("Date", "Date")

        ' Styling the grid
        With dgvOptions
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .EnableHeadersVisualStyles = False
            .RowTemplate.Height = 28
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 229, 255)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .GridColor = Color.LightGray
            .RowHeadersVisible = False
            .ReadOnly = True
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AllowUserToResizeRows = False
            .AllowUserToResizeColumns = False

            ' Fixed column widths
            .Columns(0).Width = 220 ' Transport
            .Columns(1).Width = 220 ' Party
            .Columns(2).Width = 140 ' Date

            ' Disable sorting
            For Each col As DataGridViewColumn In .Columns
                col.SortMode = DataGridViewColumnSortMode.NotSortable
                col.Resizable = DataGridViewTriState.False
            Next
        End With

        ' Add data
        Const LineSep As String = ";;;"
        Const EntrySep As String = "~~~"
        Dim rows() As String = dataString.Split(New String() {LineSep}, StringSplitOptions.RemoveEmptyEntries)

        For Each row As String In rows
            Dim parts() As String = row.Split(New String() {EntrySep}, StringSplitOptions.None)
            If parts.Length = 3 Then
                dgvOptions.Rows.Add(parts(1).Trim(), parts(2).Trim(), parts(0).Trim())
            End If
        Next

        ' Clear selection initially
        If dgvOptions.Rows.Count > 0 Then
            dgvOptions.ClearSelection()
            dgvOptions.Rows(0).Selected = True
        End If
        dgvOptions.Focus()

        ' Show form and return result
        SelectedTransport = ""
        FocusDiagnostics.Log(String.Format("BCOptionsBox ShowDialogWithOptions: Title='{0}', Owner={1}", title, FocusDiagnostics.GetWindowInfo(If(owner IsNot Nothing, owner.Handle, IntPtr.Zero))))
        Dim dlgResult As DialogResult = If(owner IsNot Nothing, ShowDialog(owner), ShowDialog())
        If dlgResult = DialogResult.OK AndAlso String.IsNullOrEmpty(SelectedTransport) Then
            Dim selRow As DataGridViewRow = Nothing

            If dgvOptions.SelectedRows.Count > 0 Then
                selRow = dgvOptions.SelectedRows(0)
            ElseIf dgvOptions.CurrentRow IsNot Nothing Then
                selRow = dgvOptions.CurrentRow
            End If

            If selRow IsNot Nothing Then
                SelectedTransport = selRow.Cells("Transport").Value.ToString()
            End If
        End If

        FocusDiagnostics.Log(String.Format("BCOptionsBox ShowDialogWithOptions: dlgResult={0}, SelectedTransport='{1}'", dlgResult, SelectedTransport))
        Return SelectedTransport
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.MediumSlateBlue, ButtonBorderStyle.Solid)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCOptionsBox ProcessCmdKey: keyData={0}, elapsedMs={1:F0}", keyData, elapsedMs))

        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCOptionsBox ProcessCmdKey: Swallowed keystroke because elapsedMs < 300")
            Return True
        End If

        If keyData = Keys.F2 Then
            FocusDiagnostics.Log("BCOptionsBox ProcessCmdKey: Submit via F2")
            SubmitSelection()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub SubmitSelection()
        Dim selRow As DataGridViewRow = Nothing
        If dgvOptions.SelectedRows.Count > 0 Then
            selRow = dgvOptions.SelectedRows(0)
        ElseIf dgvOptions.CurrentRow IsNot Nothing Then
            selRow = dgvOptions.CurrentRow
        End If
        If selRow IsNot Nothing Then
            SelectedTransport = selRow.Cells("Transport").Value.ToString()
        End If
        FocusDiagnostics.Log(String.Format("BCOptionsBox SubmitSelection: Selected='{0}'", SelectedTransport))
        FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
        FocusDiagnostics.PurgeKeyboardQueue()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub dgvOptions_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvOptions.KeyDown
        FocusDiagnostics.Log(String.Format("BCOptionsBox dgvOptions_KeyDown: KeyCode={0}", e.KeyCode))
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Escape Then
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub dgvOptions_KeyUp(sender As Object, e As KeyEventArgs) Handles dgvOptions.KeyUp
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCOptionsBox dgvOptions_KeyUp: KeyCode={0}, elapsedMs={1:F0}", e.KeyCode, elapsedMs))
        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCOptionsBox dgvOptions_KeyUp: Ignored because elapsedMs < 300")
            Return
        End If
        If e.KeyCode = Keys.Enter Then
            FocusDiagnostics.Log("BCOptionsBox dgvOptions_KeyUp: OK via Enter")
            SubmitSelection()
        ElseIf e.KeyCode = Keys.Escape Then
            FocusDiagnostics.Log("BCOptionsBox dgvOptions_KeyUp: Cancel via Escape")
            SelectedTransport = ""
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
            FocusDiagnostics.PurgeKeyboardQueue()
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    Private Sub dgvOptions_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvOptions.CellDoubleClick
        If e.RowIndex >= 0 Then
            SelectedTransport = dgvOptions.Rows(e.RowIndex).Cells("Transport").Value.ToString()
            FocusDiagnostics.Log(String.Format("BCOptionsBox CellDoubleClick: Selected='{0}'", SelectedTransport))
            DialogResult = DialogResult.OK
            Close()
        End If
    End Sub

    Private Sub BCOptionsBox_Load(sender As Object, e As EventArgs) Handles Me.Load
        openTime = DateTime.Now
        KeyPreview = True
        StartPosition = FormStartPosition.CenterScreen
        FocusDiagnostics.LogSnapshot("BCOptionsBox_Load", Me.Handle)
    End Sub

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Private Sub BCOptionsBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            FocusDiagnostics.LogSnapshot("BCOptionsBox_Shown Entry", Me.Handle)
            Activate()
            Dim setFgRes = SetForegroundWindow(Handle)
            FocusDiagnostics.Log(String.Format("BCOptionsBox_Shown: SetForegroundWindow returned {0}", setFgRes))
            dgvOptions.Focus()
            FocusDiagnostics.LogSnapshot("BCOptionsBox_Shown Exit", Me.Handle)
        Catch ex As Exception
            FocusDiagnostics.Log("BCOptionsBox_Shown Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub BCOptionsBox_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        FocusDiagnostics.Log(String.Format("BCOptionsBox ACTIVATED. CurrentFocus={0}", FocusDiagnostics.GetWindowInfo(FocusDiagnostics.GetFocus())))
    End Sub

    Private isClosing As Boolean = False

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        isClosing = True
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub BCOptionsBox_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isClosing OrElse Me.DialogResult <> DialogResult.None OrElse Me.Disposing OrElse Me.IsDisposed Then
            FocusDiagnostics.Log("BCOptionsBox Deactivated (normal close)")
        Else
            Dim fg = FocusDiagnostics.GetForegroundWindow()
            If fg <> Me.Handle Then
                FocusDiagnostics.LogSnapshot("BCOptionsBox DEACTIVATED UNEXPECTEDLY (lost focus to external window!)", Me.Handle)
            Else
                FocusDiagnostics.Log("BCOptionsBox Deactivated (internal)")
            End If
        End If
    End Sub

End Class