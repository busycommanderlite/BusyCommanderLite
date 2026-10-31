Public Class ShortcutsForm
    Private openTime As DateTime = DateTime.Now

    Public Sub New()
        InitializeComponent()
        ' Ensure KeyPreview is True so the form catches KeyUp events
        Me.KeyPreview = True
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If (DateTime.Now - openTime).TotalMilliseconds < 300 AndAlso (keyData = Keys.Enter OrElse keyData = Keys.Escape) Then
            Return True
        End If

        If keyData = Keys.Escape Then
            ' Suppress KeyDown. We will close the form on KeyUp.
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub ShortcutsForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
End Class