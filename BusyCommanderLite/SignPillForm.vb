Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Public Class SignPillForm
    Inherits Form

    Private Shared currentInstance As SignPillForm = Nothing
    Private Shared ReadOnly instanceLock As New Object()

    Private lblTally As Label

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.lblTally = New Label()
        Me.SuspendLayout()

        ' lblTally
        Me.lblTally.Dock = DockStyle.Fill
        Me.lblTally.Font = New Font("Segoe UI Semibold", 10.5F, FontStyle.Bold)
        Me.lblTally.ForeColor = Color.White
        Me.lblTally.Location = New Point(0, 0)
        Me.lblTally.Name = "lblTally"
        Me.lblTally.Size = New Size(210, 42)
        Me.lblTally.TabIndex = 0
        Me.lblTally.Text = Char.ConvertFromUtf32(&H1F4E6) & " 0 Nag (0 Bills)"
        Me.lblTally.TextAlign = ContentAlignment.MiddleCenter

        ' SignPillForm
        Me.AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.BackColor = Color.FromArgb(30, 34, 42)
        Me.ClientSize = New Size(210, 42)
        Me.Controls.Add(Me.lblTally)
        Me.Font = New Font("Segoe UI", 9.0F)
        Me.FormBorderStyle = FormBorderStyle.None
        Me.Name = "SignPillForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = FormStartPosition.Manual
        Me.Text = "Sign Tally"
        Me.TopMost = True
        Me.ResumeLayout(False)

        ApplyRoundedCorners()
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            ' WS_EX_NOACTIVATE = &H8000000 ensures this window NEVER takes focus from Busy
            cp.ExStyle = cp.ExStyle Or &H8000000
            cp.ExStyle = cp.ExStyle Or &H8 ' WS_EX_TOPMOST
            Return cp
        End Get
    End Property

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Private Sub ApplyRoundedCorners()
        Try
            Dim path As New GraphicsPath()
            Dim radius As Integer = 16
            Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90)
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90)
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90)
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90)
            path.CloseFigure()
            Me.Region = New Region(path)
        Catch
        End Try
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        ApplyRoundedCorners()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using pen As New Pen(Color.FromArgb(80, 90, 110), 1.5F)
            Dim radius As Integer = 16
            Dim rect As New Rectangle(1, 1, Me.Width - 2, Me.Height - 2)
            Using path As New GraphicsPath()
                path.AddArc(rect.X, rect.Y, radius, radius, 180, 90)
                path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90)
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90)
                path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90)
                path.CloseFigure()
                e.Graphics.DrawPath(pen, path)
            End Using
        End Using
    End Sub

    Private Sub SetTally(totalNag As Integer, billCount As Integer)
        If totalNag <= 0 Then
            Me.Hide()
            Return
        End If

        Dim billText = If(billCount = 1, "1 Bill", $"{billCount} Bills")
        lblTally.Text = Char.ConvertFromUtf32(&H1F4E6) & $" {totalNag} Nag ({billText})"

        ' Position at top-right of the working area
        Dim screenArea = Screen.PrimaryScreen.WorkingArea
        Me.Location = New Point(screenArea.Right - Me.Width - 35, screenArea.Top + 35)

        If Not Me.Visible Then
            Me.Show()
        End If
    End Sub

    ' -------------------------------------------------------------------------
    ' Shared API
    ' -------------------------------------------------------------------------
    Public Shared Sub ShowTally(totalNag As Integer, billCount As Integer)
        SyncLock instanceLock
            If currentInstance Is Nothing OrElse currentInstance.IsDisposed Then
                currentInstance = New SignPillForm()
            End If

            If currentInstance.InvokeRequired Then
                currentInstance.BeginInvoke(Sub() currentInstance.SetTally(totalNag, billCount))
            Else
                currentInstance.SetTally(totalNag, billCount)
            End If
        End SyncLock
    End Sub

    Public Shared Sub HideTally()
        SyncLock instanceLock
            If currentInstance IsNot Nothing AndAlso Not currentInstance.IsDisposed Then
                If currentInstance.InvokeRequired Then
                    currentInstance.BeginInvoke(Sub() currentInstance.Hide())
                Else
                    currentInstance.Hide()
                End If
            End If
        End SyncLock
    End Sub

End Class

