Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class BusyCommanderHttpServer
    Private listener As HttpListener
    Private serverThread As Thread
    Private folderPath As String
    Private running As Boolean = False

    Public Sub Start(portNum As String, folderPath As String)
        Me.folderPath = folderPath
        listener = New HttpListener()
        listener.Prefixes.Add($"http://localhost:{portNum}/")
        listener.Start()

        running = True
        serverThread = New Thread(AddressOf ListenLoop)
        serverThread.IsBackground = True
        serverThread.Start()
    End Sub

    Private Sub ListenLoop()
        While running AndAlso listener IsNot Nothing AndAlso listener.IsListening
            Try
                Dim context = listener.GetContext()
                HandleRequest(context)
            Catch ex As HttpListenerException
                Exit While ' occurs when listener.Stop is called
            Catch ex As ObjectDisposedException
                Exit While ' occurs when listener.Close is called
            Catch ex As Exception
                ' Log if needed
            End Try
        End While
    End Sub

    Private Sub HandleRequest(context As HttpListenerContext)
        Dim request = context.Request
        Dim response = context.Response

        Try
            If request.HttpMethod = "POST" Then
                Dim contentType = request.ContentType
                If Not String.IsNullOrEmpty(contentType) AndAlso contentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) Then
                    Dim boundaryMatch = Regex.Match(contentType, "boundary=(?:""([^""]+)""|([^;]+))", RegexOptions.IgnoreCase)
                    Dim boundaryVal = If(boundaryMatch.Groups(1).Success, boundaryMatch.Groups(1).Value, boundaryMatch.Groups(2).Value).Trim()
                    Dim boundaryBytes = Encoding.ASCII.GetBytes("--" & boundaryVal)
                    Dim crlfCrlfBytes = Encoding.ASCII.GetBytes(vbCrLf & vbCrLf)

                    ' Read raw body bytes directly without string conversion
                    Dim bodyBytes As Byte()
                    Using ms As New MemoryStream()
                        request.InputStream.CopyTo(ms)
                        bodyBytes = ms.ToArray()
                    End Using

                    Dim phone As String = Nothing
                    Dim message As String = Nothing
                    Dim fileName As String = Nothing
                    Dim savedFilePath As String = Nothing

                    Dim pos = IndexOfBytes(bodyBytes, boundaryBytes, 0)
                    While pos >= 0 AndAlso pos + boundaryBytes.Length + 2 <= bodyBytes.Length
                        ' Check for end boundary ("--")
                        If pos + boundaryBytes.Length + 1 < bodyBytes.Length AndAlso
                           bodyBytes(pos + boundaryBytes.Length) = 45 AndAlso
                           bodyBytes(pos + boundaryBytes.Length + 1) = 45 Then
                            Exit While
                        End If

                        ' Skip boundary + CRLF
                        Dim startOfHeaders = pos + boundaryBytes.Length
                        If startOfHeaders + 1 < bodyBytes.Length AndAlso
                           bodyBytes(startOfHeaders) = 13 AndAlso bodyBytes(startOfHeaders + 1) = 10 Then
                            startOfHeaders += 2
                        End If

                        Dim headerEnd = IndexOfBytes(bodyBytes, crlfCrlfBytes, startOfHeaders)
                        If headerEnd < 0 Then Exit While

                        Dim headerText = Encoding.UTF8.GetString(bodyBytes, startOfHeaders, headerEnd - startOfHeaders)
                        Dim contentStart = headerEnd + 4

                        Dim nextBoundaryPos = IndexOfBytes(bodyBytes, boundaryBytes, contentStart)
                        If nextBoundaryPos < 0 Then Exit While

                        ' Content ends before the CRLF preceding the next boundary
                        Dim contentEnd = nextBoundaryPos
                        If contentEnd >= contentStart + 2 AndAlso
                           bodyBytes(contentEnd - 2) = 13 AndAlso bodyBytes(contentEnd - 1) = 10 Then
                            contentEnd -= 2
                        End If
                        Dim contentLen = Math.Max(0, contentEnd - contentStart)

                        If headerText.Contains("name=""mobNum""") Then
                            phone = Encoding.UTF8.GetString(bodyBytes, contentStart, contentLen).Trim()
                        ElseIf headerText.Contains("name=""message""") Then
                            message = Encoding.UTF8.GetString(bodyBytes, contentStart, contentLen).Trim()
                        ElseIf headerText.Contains("name=""uploadFile""") Then
                            Dim fnMatch = Regex.Match(headerText, "filename=(?:""([^""]+)""|([^\r\n;]+))", RegexOptions.IgnoreCase)
                            If fnMatch.Success Then
                                fileName = If(fnMatch.Groups(1).Success, fnMatch.Groups(1).Value, fnMatch.Groups(2).Value).Trim()
                                If Not String.IsNullOrEmpty(fileName) Then
                                    fileName = Path.GetFileName(fileName)
                                    savedFilePath = Path.Combine(folderPath, fileName)
                                    ' Write exact raw binary slice to disk
                                    Using fs As New FileStream(savedFilePath, FileMode.Create, FileAccess.Write, FileShare.None)
                                        fs.Write(bodyBytes, contentStart, contentLen)
                                    End Using
                                End If
                            End If
                        End If

                        pos = nextBoundaryPos
                    End While

                    ' Dispatch if file and phone were provided
                    If Not String.IsNullOrEmpty(savedFilePath) AndAlso File.Exists(savedFilePath) Then
                        Dim sendSuccess = False
                        If request.Url.AbsolutePath.Equals("/send", StringComparison.OrdinalIgnoreCase) Then
                            sendSuccess = MyApp.brForm.SendFileFromHTTPServer(savedFilePath, phone, message)
                        ElseIf request.Url.AbsolutePath.Equals("/sendimage", StringComparison.OrdinalIgnoreCase) Then
                            sendSuccess = MyApp.brForm.SendImageFromHTTPServer(savedFilePath, phone, message)
                        End If

                        If sendSuccess Then
                            response.StatusCode = 200
                            WriteResponse(response, "OK")
                        Else
                            response.StatusCode = 500
                            WriteResponse(response, "Error sending WhatsApp")
                        End If
                        Return
                    End If

                    response.StatusCode = 400
                    WriteResponse(response, "Bad Request: Missing upload file")
                    Return
                End If
            End If

            response.StatusCode = 404
            WriteResponse(response, "Not Found")

        Catch ex As Exception
            response.StatusCode = 500
            WriteResponse(response, "Error: " & ex.Message)
        End Try
    End Sub

    Private Function IndexOfBytes(source As Byte(), pattern As Byte(), startIndex As Integer) As Integer
        If source Is Nothing OrElse pattern Is Nothing OrElse source.Length = 0 OrElse pattern.Length = 0 Then Return -1
        Dim max = source.Length - pattern.Length
        For i As Integer = startIndex To max
            Dim match As Boolean = True
            For j As Integer = 0 To pattern.Length - 1
                If source(i + j) <> pattern(j) Then
                    match = False
                    Exit For
                End If
            Next
            If match Then Return i
        Next
        Return -1
    End Function

    Private Sub WriteResponse(response As HttpListenerResponse, message As String)
        Try
            Dim buffer = Encoding.UTF8.GetBytes(message)
            response.ContentLength64 = buffer.Length
            Using response.OutputStream
                response.OutputStream.Write(buffer, 0, buffer.Length)
            End Using
        Catch
        End Try
    End Sub

    Public Sub [Stop]()
        running = False
        If listener IsNot Nothing Then
            Try
                If listener.IsListening Then
                    listener.Stop()
                End If
                listener.Close()
            Catch
            End Try
        End If
        If serverThread IsNot Nothing AndAlso serverThread.IsAlive Then
            serverThread.Join(1000) ' wait for thread to exit
        End If
    End Sub
End Class
