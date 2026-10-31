Imports System.Drawing
Imports System.Windows.Forms

Public Class BCProgressBox
    Private ownerWrapper As IWin32Window
    Private disabledOwnerHwnd As IntPtr = IntPtr.Zero
    Private disabledChildHwnds As New List(Of IntPtr)()
    Private safetyTimer As Windows.Forms.Timer

    Public Sub StartWait(title As String, message As String)
        Try
            Me.Text = title
            lblTitle.Text = If(String.IsNullOrEmpty(title), "BusyCommander", title)
            lblMessage.Text = message.Replace("\n", vbCrLf)
            ProgressBar1.Style = ProgressBarStyle.Marquee
            ProgressBar1.MarqueeAnimationSpeed = 30

            FocusDiagnostics.Log(String.Format("BCProgressBox StartWait: Title='{0}' Msg='{1}'", title, message))

            ' 1. Purge any stale keyboard messages
            Dim purged = FocusDiagnostics.PurgeKeyboardQueue()
            If purged > 0 Then
                FocusDiagnostics.Log(String.Format("BCProgressBox: Purged {0} stale keyboard message(s)", purged))
            End If

            ' 2. Resolve owner window
            ownerWrapper = MyApp.GetActiveWindowOwner()
            Dim ownerHwnd = If(ownerWrapper IsNot Nothing, ownerWrapper.Handle, IntPtr.Zero)

            ' 3. Disable owner window and its child controls to completely lock Busy input
            If ownerHwnd <> IntPtr.Zero AndAlso FocusDiagnostics.IsWindow(ownerHwnd) Then
                If FocusDiagnostics.IsWindowEnabled(ownerHwnd) Then
                    FocusDiagnostics.EnableWindow(ownerHwnd, False)
                    disabledOwnerHwnd = ownerHwnd
                    FocusDiagnostics.Log(String.Format("BCProgressBox: Disabled owner window {0}", FocusDiagnostics.GetWindowInfo(ownerHwnd)))
                End If

                Dim grids = FocusDiagnostics.FindChildWindowsByClass(ownerHwnd, "ListView20WndClass")
                For Each g In grids
                    If FocusDiagnostics.IsWindow(g) AndAlso FocusDiagnostics.IsWindowEnabled(g) Then
                        FocusDiagnostics.EnableWindow(g, False)
                        disabledChildHwnds.Add(g)
                        FocusDiagnostics.Log(String.Format("BCProgressBox: Disabled child control {0}", FocusDiagnostics.GetWindowInfo(g)))
                    End If
                Next
            End If

            ' 4. Center on owner window
            If ownerHwnd <> IntPtr.Zero Then
                Dim rect As FocusDiagnostics.RECT
                FocusDiagnostics.GetWindowRect(ownerHwnd, rect)
                Dim w = Me.Width
                Dim h = Me.Height
                Dim left = rect.Left + (rect.Right - rect.Left - w) \ 2
                Dim top = rect.Top + (rect.Bottom - rect.Top - h) \ 2
                Me.StartPosition = FormStartPosition.Manual
                Me.Location = New Point(Math.Max(0, left), Math.Max(0, top))
            Else
                Me.StartPosition = FormStartPosition.CenterScreen
            End If

            ' 5. Set wait cursor
            Cursor.Current = Cursors.WaitCursor

            ' 6. Show modelessly
            If ownerWrapper IsNot Nothing Then
                Me.Show(ownerWrapper)
            Else
                Me.Show()
            End If
            Me.BringToFront()
            Me.Refresh()
            Application.DoEvents()

            ' 7. Start safety timer (60s) to guarantee Busy is never permanently locked
            safetyTimer = New Windows.Forms.Timer()
            safetyTimer.Interval = 60000
            AddHandler safetyTimer.Tick, AddressOf OnSafetyTimerTick
            safetyTimer.Start()

        Catch ex As Exception
            FocusDiagnostics.Log("BCProgressBox StartWait Exception: " & ex.Message)
        End Try
    End Sub

    Public Sub SetMessage(message As String)
        Try
            lblMessage.Text = message.Replace("\n", vbCrLf)
            lblMessage.Refresh()
            Application.DoEvents()
        Catch ex As Exception
            FocusDiagnostics.Log("BCProgressBox SetMessage Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub OnSafetyTimerTick(sender As Object, e As EventArgs)
        FocusDiagnostics.Log("BCProgressBox: Safety timer expired! Automatically stopping wait lock.")
        StopWait()
    End Sub

    Public Sub StopWait()
        Try
            If safetyTimer IsNot Nothing Then
                safetyTimer.Stop()
                safetyTimer.Dispose()
                safetyTimer = Nothing
            End If

            ' 1. Discard/purge any keystrokes pressed by user during the wait
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
            Dim purged = FocusDiagnostics.PurgeKeyboardQueue()
            If purged > 0 Then
                FocusDiagnostics.Log(String.Format("BCProgressBox StopWait: Purged {0} keystroke(s) pressed during wait", purged))
            End If

            ' 2. Re-enable child controls
            For Each h In disabledChildHwnds
                If FocusDiagnostics.IsWindow(h) Then
                    FocusDiagnostics.EnableWindow(h, True)
                    FocusDiagnostics.Log(String.Format("BCProgressBox: Re-enabled child control {0}", FocusDiagnostics.GetWindowInfo(h)))
                End If
            Next
            disabledChildHwnds.Clear()

            ' 3. Re-enable owner window
            If disabledOwnerHwnd <> IntPtr.Zero AndAlso FocusDiagnostics.IsWindow(disabledOwnerHwnd) Then
                FocusDiagnostics.EnableWindow(disabledOwnerHwnd, True)
                FocusDiagnostics.Log(String.Format("BCProgressBox: Re-enabled owner window {0}", FocusDiagnostics.GetWindowInfo(disabledOwnerHwnd)))
                disabledOwnerHwnd = IntPtr.Zero
            End If
            FocusDiagnostics.PurgeKeyboardQueue()

            ' 4. Restore cursor
            Cursor.Current = Cursors.Default

            ' 5. Close this wait form
            Me.Close()
            Me.Dispose()
            FocusDiagnostics.Log("BCProgressBox StopWait completed.")
        Catch ex As Exception
            FocusDiagnostics.Log("BCProgressBox StopWait Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub BCProgressBox_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        ' Discard all keystrokes during the wait
        e.Handled = True
        e.SuppressKeyPress = True
        FocusDiagnostics.Log(String.Format("BCProgressBox KeyDown: Discarded key {0}", e.KeyCode))
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim rect As New Rectangle(0, 0, Me.Width - 1, Me.Height - 1)
        e.Graphics.DrawRectangle(Pens.Gray, rect)
    End Sub
End Class