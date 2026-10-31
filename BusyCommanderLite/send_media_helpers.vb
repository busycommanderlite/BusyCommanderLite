    Private Sub SendMediaMessage(MobileNumber As String, Media As Bitmap)
        Dim MS = New MemoryStream()
        Media.Save(MS, ImageFormat.Jpeg)
        Dim b64media = Convert.ToBase64String(MS.ToArray())
        SendMediaToGoServer(MobileNumber, b64media, "image", "image/jpeg", "image.jpg", "")
    End Sub

    Private Sub SendMediaMessageWithCaption(MobileNumber As String, Media As Bitmap, Caption As String)
        Dim MS = New MemoryStream()
        Media.Save(MS, ImageFormat.Jpeg)
        Dim b64media = Convert.ToBase64String(MS.ToArray())
        SendMediaToGoServer(MobileNumber, b64media, "image", "image/jpeg", "image.jpg", Caption)
    End Sub

    Private Sub SendPdfMessage(MobileNumber As String, FileName As String, Caption As String)
        Dim b64media = Convert.ToBase64String(File.ReadAllBytes(FileName))
        Dim cleanFileName As String = System.Text.RegularExpressions.Regex.Replace(System.IO.Path.GetFileName(FileName), "[^A-Za-z0-9.]", "")
        SendMediaToGoServer(MobileNumber, b64media, "document", "application/pdf", cleanFileName, Caption)
    End Sub
