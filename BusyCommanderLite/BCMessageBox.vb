Imports System.Drawing
Imports System.Windows.Forms

Public Class BCMessageBox
    Private isYesNo As Boolean = False
    Private result As DialogResult = DialogResult.None

    Private openTime As DateTime

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Public Function ShowAlert(title As String, msg As String, type As String, owner As IWin32Window) As DialogResult
        openTime = DateTime.Now
        lblTitle.Text = title
        lblMessage.Text = msg
        
        If type = "YesNo" Then
            isYesNo = True
            lblBottom.Text = "[ Y / Enter - Yes  |  N / Esc - No ]"
            If title.ToUpper() = "WARNING" Then
                lblTitle.BackColor = Color.OrangeRed
                lblBottom.BackColor = Color.OrangeRed
            Else
                lblTitle.BackColor = Color.SteelBlue
                lblBottom.BackColor = Color.SteelBlue
            End If
        Else
            isYesNo = False
            lblBottom.Text = "[ Enter / Esc - OK ]"
            If type = "Error" Then
                lblTitle.BackColor = Color.Crimson
                lblBottom.BackColor = Color.Crimson
            Else
                lblTitle.BackColor = Color.MediumSlateBlue
                lblBottom.BackColor = Color.MediumSlateBlue
            End If
        End If

        FocusDiagnostics.Log(String.Format("BCMessageBox ShowAlert: Showing with owner={0}", FocusDiagnostics.GetWindowInfo(If(owner IsNot Nothing, owner.Handle, IntPtr.Zero))))
        If owner IsNot Nothing Then
            Me.ShowDialog(owner)
        Else
            Me.ShowDialog()
        End If

        FocusDiagnostics.Log(String.Format("BCMessageBox ShowAlert: Returned result={0}", result))
        Return result
    End Function

    Private Sub BCMessageBox_Load(sender As Object, e As EventArgs) Handles Me.Load
        FocusDiagnostics.LogSnapshot("BCMessageBox_Load", Me.Handle)
    End Sub

    Private Sub BCMessageBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            FocusDiagnostics.LogSnapshot("BCMessageBox_Shown Entry", Me.Handle)
            Me.Activate()
            Dim setFgRes = SetForegroundWindow(Me.Handle)
            FocusDiagnostics.Log(String.Format("BCMessageBox_Shown: SetForegroundWindow returned {0}", setFgRes))
            Me.Focus()
            FocusDiagnostics.LogSnapshot("BCMessageBox_Shown Exit", Me.Handle)
        Catch ex As Exception
            FocusDiagnostics.Log("BCMessageBox_Shown Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub BCMessageBox_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        FocusDiagnostics.Log(String.Format("BCMessageBox ACTIVATED. CurrentFocus={0}", FocusDiagnostics.GetWindowInfo(FocusDiagnostics.GetFocus())))
    End Sub

    Private isClosing As Boolean = False

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        isClosing = True
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub BCMessageBox_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isClosing OrElse Me.DialogResult <> DialogResult.None OrElse Me.Disposing OrElse Me.IsDisposed Then
            FocusDiagnostics.Log("BCMessageBox Deactivated (normal close)")
        Else
            Dim fg = FocusDiagnostics.GetForegroundWindow()
            If fg <> Me.Handle Then
                FocusDiagnostics.LogSnapshot("BCMessageBox DEACTIVATED UNEXPECTEDLY (lost focus to external window!)", Me.Handle)
            Else
                FocusDiagnostics.Log("BCMessageBox Deactivated (internal)")
            End If
        End If
    End Sub

    Private Sub BCMessageBox_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCMessageBox KeyDown: KeyCode={0}, elapsedMs={1:F0}", e.KeyCode, elapsedMs))

        ' ALWAYS suppress Enter, Escape, Y, N so they cannot ding or propagate to parent controls
        If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Escape OrElse e.KeyCode = Keys.Y OrElse e.KeyCode = Keys.N Then
            e.Handled = True
            e.SuppressKeyPress = True
        End If

        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCMessageBox KeyDown: Swallowed key because elapsedMs < 300")
            Return
        End If
        
        If isYesNo Then
            If e.KeyCode = Keys.Y OrElse e.KeyCode = Keys.Enter Then
                FocusDiagnostics.Log("BCMessageBox KeyDown: Yes selected via " & e.KeyCode.ToString())
                result = DialogResult.Yes
                FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
                FocusDiagnostics.PurgeKeyboardQueue()
                Me.DialogResult = DialogResult.Yes
            ElseIf e.KeyCode = Keys.N OrElse e.KeyCode = Keys.Escape Then
                FocusDiagnostics.Log("BCMessageBox KeyDown: No selected via " & e.KeyCode.ToString())
                result = DialogResult.No
                FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
                FocusDiagnostics.PurgeKeyboardQueue()
                Me.DialogResult = DialogResult.No
            End If
        Else
            If e.KeyCode = Keys.Enter OrElse e.KeyCode = Keys.Escape Then
                FocusDiagnostics.Log("BCMessageBox KeyDown: OK selected via " & e.KeyCode.ToString())
                result = DialogResult.OK
                FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
                FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
                FocusDiagnostics.PurgeKeyboardQueue()
                Me.DialogResult = DialogResult.OK
            End If
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim rect As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        e.Graphics.DrawRectangle(Pens.Gray, rect)
    End Sub
End Class
