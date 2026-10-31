Imports System.IO

Public Class SMSHelpForm
    Private Sub SMSHelpForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        optPdf.Checked = True
        UpdateURL()
    End Sub
    
    Private Sub UpdateURL()
        If optImage.Checked Then
            txtURL.Text = "http://localhost:54321/sendimage?phoneNo=mobNum&messageText=message"
        Else
            txtURL.Text = "http://localhost:54321/send?phoneNo=mobNum&messageText=message"
        End If
    End Sub

    Private Sub optPdf_CheckedChanged(sender As Object, e As EventArgs) Handles optPdf.CheckedChanged
        UpdateURL()
    End Sub

    Private Sub optImage_CheckedChanged(sender As Object, e As EventArgs) Handles optImage.CheckedChanged
        UpdateURL()
    End Sub

    Private Sub btnCopyUrl_Click(sender As Object, e As EventArgs) Handles btnCopyUrl.Click
        Try
            Clipboard.SetText(txtURL.Text)
            Dim box As New BCMessageBox()
            box.ShowAlert("Copied", "URL copied to clipboard!", "Info", Me)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class
