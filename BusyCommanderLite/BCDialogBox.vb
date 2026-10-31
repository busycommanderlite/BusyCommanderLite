Public Class BCDialogBox
    Private openTime As DateTime

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.MediumSlateBlue, ButtonBorderStyle.Solid)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCDialogBox ProcessCmdKey: keyData={0}, elapsedMs={1:F0}", keyData, elapsedMs))

        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCDialogBox ProcessCmdKey: Swallowed keystroke because elapsedMs < 300")
            Return True
        End If
        
        If keyData = Keys.F2 Then
            FocusDiagnostics.Log("BCDialogBox ProcessCmdKey: OK via F2")
            DialogResult = DialogResult.OK
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub txtInput_KeyDown(sender As Object, e As KeyEventArgs) Handles txtInput.KeyDown
        FocusDiagnostics.Log(String.Format("BCDialogBox txtInput_KeyDown: KeyCode={0}", e.KeyCode))
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Escape Then
            ' Suppress key press to prevent 'ding' sound and WM_CHAR generation
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub txtInput_KeyUp(sender As Object, e As KeyEventArgs) Handles txtInput.KeyUp
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCDialogBox txtInput_KeyUp: KeyCode={0}, elapsedMs={1:F0}", e.KeyCode, elapsedMs))
        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCDialogBox txtInput_KeyUp: Ignored because elapsedMs < 300")
            Return
        End If
        If e.KeyCode = Keys.Enter Then
            FocusDiagnostics.Log("BCDialogBox txtInput_KeyUp: OK via Enter")
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
            FocusDiagnostics.PurgeKeyboardQueue()
            DialogResult = DialogResult.OK
        ElseIf e.KeyCode = Keys.Escape Then
            FocusDiagnostics.Log("BCDialogBox txtInput_KeyUp: Cancel via Escape")
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
            FocusDiagnostics.PurgeKeyboardQueue()
            DialogResult = DialogResult.Cancel
        End If
    End Sub

    Private Sub BCDialogBox_Load(sender As Object, e As EventArgs) Handles Me.Load
        openTime = DateTime.Now
        FocusDiagnostics.LogSnapshot("BCDialogBox_Load", Me.Handle)
    End Sub

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Private Sub BCDialogBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            FocusDiagnostics.LogSnapshot("BCDialogBox_Shown Entry", Me.Handle)
            Activate()
            Dim setFgRes = SetForegroundWindow(Handle)
            FocusDiagnostics.Log(String.Format("BCDialogBox_Shown: SetForegroundWindow returned {0}", setFgRes))
            txtInput.Focus()
            txtInput.SelectAll()
            FocusDiagnostics.LogSnapshot("BCDialogBox_Shown Exit", Me.Handle)
        Catch ex As Exception
            FocusDiagnostics.Log("BCDialogBox_Shown Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub BCDialogBox_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        FocusDiagnostics.Log(String.Format("BCDialogBox ACTIVATED. CurrentFocus={0}", FocusDiagnostics.GetWindowInfo(FocusDiagnostics.GetFocus())))
    End Sub

    Private isClosing As Boolean = False

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        isClosing = True
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub BCDialogBox_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isClosing OrElse Me.DialogResult <> DialogResult.None OrElse Me.Disposing OrElse Me.IsDisposed Then
            FocusDiagnostics.Log("BCDialogBox Deactivated (normal close)")
        Else
            Dim fg = FocusDiagnostics.GetForegroundWindow()
            If fg <> Me.Handle Then
                FocusDiagnostics.LogSnapshot("BCDialogBox DEACTIVATED UNEXPECTEDLY (lost focus to external window!)", Me.Handle)
            Else
                FocusDiagnostics.Log("BCDialogBox Deactivated (internal)")
            End If
        End If
    End Sub

    Public Function ShowDialogWithData(title As String, subheading As String, defaultText As String, Optional owner As IWin32Window = Nothing, Optional maxLength As Integer = 0) As String
        lblTitle.Text = title
        lblSubheading.Text = subheading.Replace("\n", vbCrLf)
        txtInput.Text = defaultText

        ' Enforce max length only if explicitly requested
        If maxLength > 0 Then
            txtInput.MaxLength = maxLength
        Else
            txtInput.MaxLength = 32767
        End If

        Dim result As String = ""
        FocusDiagnostics.Log(String.Format("BCDialogBox ShowDialogWithData: Title='{0}', MaxLength={1}, Owner={2}", title, txtInput.MaxLength, FocusDiagnostics.GetWindowInfo(If(owner IsNot Nothing, owner.Handle, IntPtr.Zero))))
        Dim dlgResult As DialogResult = If(owner IsNot Nothing, ShowDialog(owner), ShowDialog())
        FocusDiagnostics.Log(String.Format("BCDialogBox ShowDialogWithData: dlgResult={0}, Text='{1}'", dlgResult, txtInput.Text))
        If dlgResult = DialogResult.OK Then
            result = Trim(txtInput.Text)
            If maxLength > 0 AndAlso result.Length > maxLength Then
                result = result.Substring(0, maxLength).Trim()
            End If
        End If
        Return result
    End Function

End Class