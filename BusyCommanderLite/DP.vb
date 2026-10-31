Public Class DP

    Public Sub New(PicURL As String)
        InitializeComponent()
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        If Not String.IsNullOrEmpty(PicURL) AndAlso Not PicURL.ToUpper().Equals("UNDEFINED") Then
            Try
                PictureBox1.Load(PicURL)
            Catch
            End Try
        End If
        PictureBox1.Focus()
        CenterToScreen()
    End Sub

    Public Sub New(img As Image)
        InitializeComponent()
        PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        If img IsNot Nothing Then
            PictureBox1.Image = img
        End If
        PictureBox1.Focus()
        CenterToScreen()
    End Sub

    Private Sub DP_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        If e.KeyChar = ChrW(Keys.Escape) Then
            Close()
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Close()
    End Sub

    Private Sub DP_Click(sender As Object, e As EventArgs) Handles Me.Click
        Close()
    End Sub

    Private Sub ExitTimer_Tick(sender As Object, e As EventArgs) Handles ExitTimer.Tick
        Close()
    End Sub

End Class