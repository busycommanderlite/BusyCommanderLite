Imports System.Net.NetworkInformation

Public Class BCMobileBox

    Private SelectedMobile As String = Nothing
    Private rbOther As RadioButton
    Private txtOther As TextBox
    Private EntrySep = "~~~"
    Private openTime As DateTime

    Public Function ShowDialogWithMobiles(dataString As String, Optional owner As IWin32Window = Nothing) As String
        openTime = DateTime.Now
        ' Clear existing
        pnlOptions.Controls.Clear()

        ' Add radio buttons dynamically
        Dim numbers() As String = dataString.Split(New String() {EntrySep}, StringSplitOptions.RemoveEmptyEntries)

        For Each num In numbers
            Dim rb As New RadioButton()
            rb.Text = num.Trim()
            rb.AutoSize = True
            rb.Font = New Font("Segoe UI", 12.0!)
            rb.Margin = New Padding(5)
            pnlOptions.Controls.Add(rb)
        Next

        ' Add "Other" radio + textbox in the same flow panel
        rbOther = New RadioButton()
        rbOther.Text = ""
        rbOther.AutoSize = True
        rbOther.Font = New Font("Segoe UI", 12.0!)
        rbOther.Margin = New Padding(5, 10, 0, 0)

        txtOther = New TextBox()
        txtOther.Font = New Font("Segoe UI", 12.0!)
        txtOther.MaxLength = 10
        txtOther.Enabled = False
        txtOther.Width = 150
        txtOther.Margin = New Padding(5, 7, 0, 0)

        AddHandler rbOther.CheckedChanged, Sub()
                                               txtOther.Enabled = rbOther.Checked
                                               If rbOther.Checked Then
                                                   txtOther.Focus()
                                               Else
                                                   txtOther.Text = ""
                                               End If
                                           End Sub
        pnlOptions.Controls.Add(rbOther)
        pnlOptions.Controls.Add(txtOther)

        ' Default select first if available
        Dim firstRB = pnlOptions.Controls.OfType(Of RadioButton).FirstOrDefault()
        If firstRB IsNot Nothing Then
            firstRB.Checked = True
        End If

        ' Show modal with owner to lock parent window
        Dim dlgResult As DialogResult = If(owner IsNot Nothing, Me.ShowDialog(owner), Me.ShowDialog())
        If dlgResult = DialogResult.OK Then
            Return SelectedMobile
        End If

        Return Nothing
    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function

    Private Sub BCMobileBox_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try
            FocusDiagnostics.LogSnapshot("BCMobileBox_Shown Entry", Me.Handle)
            Me.Activate()
            Dim setFgRes = SetForegroundWindow(Me.Handle)
            FocusDiagnostics.Log(String.Format("BCMobileBox_Shown: SetForegroundWindow returned {0}", setFgRes))
            If rbOther IsNot Nothing AndAlso rbOther.Checked Then
                txtOther.Focus()
            Else
                Dim checkedRB = pnlOptions.Controls.OfType(Of RadioButton).FirstOrDefault(Function(r) r.Checked)
                If checkedRB IsNot Nothing Then
                    checkedRB.Focus()
                Else
                    Dim firstRB = pnlOptions.Controls.OfType(Of RadioButton).FirstOrDefault()
                    If firstRB IsNot Nothing Then
                        firstRB.Focus()
                    End If
                End If
            End If
            FocusDiagnostics.LogSnapshot("BCMobileBox_Shown Exit", Me.Handle)
        Catch ex As Exception
            FocusDiagnostics.Log("BCMobileBox_Shown Exception: " & ex.Message)
        End Try
    End Sub

    Private Sub BCMobileBox_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        FocusDiagnostics.Log(String.Format("BCMobileBox ACTIVATED. CurrentFocus={0}", FocusDiagnostics.GetWindowInfo(FocusDiagnostics.GetFocus())))
    End Sub

    Private isClosing As Boolean = False

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        isClosing = True
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub BCMobileBox_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If isClosing OrElse Me.DialogResult <> DialogResult.None OrElse Me.Disposing OrElse Me.IsDisposed Then
            FocusDiagnostics.Log("BCMobileBox Deactivated (normal close)")
        Else
            Dim fg = FocusDiagnostics.GetForegroundWindow()
            If fg <> Me.Handle Then
                FocusDiagnostics.LogSnapshot("BCMobileBox DEACTIVATED UNEXPECTEDLY (lost focus to external window!)", Me.Handle)
            Else
                FocusDiagnostics.Log("BCMobileBox Deactivated (internal)")
            End If
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.MediumSlateBlue, ButtonBorderStyle.Solid)
    End Sub

    ' Handle F2 = OK, Esc = Cancel
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Dim elapsedMs = (DateTime.Now - openTime).TotalMilliseconds
        FocusDiagnostics.Log(String.Format("BCMobileBox ProcessCmdKey: keyData={0}, elapsedMs={1:F0}", keyData, elapsedMs))

        If elapsedMs < 300 Then
            FocusDiagnostics.Log("BCMobileBox ProcessCmdKey: Swallowed keystroke because elapsedMs < 300")
            Return True ' Eat the keypress
        End If

        lblStatus.Visible = False
        If keyData = Keys.F2 OrElse keyData = Keys.Enter Then
            If rbOther IsNot Nothing AndAlso rbOther.Checked Then
                If txtOther.Text.Length = 10 AndAlso IsNumeric(txtOther.Text) Then
                    SelectedMobile = txtOther.Text
                    FocusDiagnostics.Log("BCMobileBox ProcessCmdKey: OK selected from txtOther: " & SelectedMobile)
                    FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
                    FocusDiagnostics.PurgeKeyboardQueue()
                    Me.DialogResult = DialogResult.OK
                Else
                    lblStatus.Text = "Enter 10-digit number"
                    lblStatus.ForeColor = Color.White
                    lblStatus.BackColor = Color.Red
                    lblStatus.Visible = True
                    FocusDiagnostics.Log("BCMobileBox ProcessCmdKey: txtOther invalid input: " & txtOther.Text)
                End If
            Else
                For Each ctrl In pnlOptions.Controls.OfType(Of RadioButton)()
                    If ctrl.Checked Then
                        SelectedMobile = ctrl.Text
                        FocusDiagnostics.Log("BCMobileBox ProcessCmdKey: OK selected from RadioButton: " & SelectedMobile)
                        FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
                        FocusDiagnostics.PurgeKeyboardQueue()
                        Me.DialogResult = DialogResult.OK
                        Exit For
                    End If
                Next
            End If
            Return True
        ElseIf keyData = Keys.Escape Then
            FocusDiagnostics.Log("BCMobileBox ProcessCmdKey: Cancel via Escape")
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)
            FocusDiagnostics.PurgeKeyboardQueue()
            Me.DialogResult = DialogResult.Cancel
            Return True
        ElseIf rbOther IsNot Nothing AndAlso rbOther.Checked Then
            If keyData = Keys.Up OrElse keyData = Keys.Down Then
                pnlOptions.Controls.OfType(Of RadioButton).First().Select()
            End If
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

End Class