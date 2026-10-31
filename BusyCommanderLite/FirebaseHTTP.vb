Imports System.IO
Imports System.Net.Http
Imports System.Text

Module FirebaseHTTP

    Private Const BasePath As String = "https://busymasterdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/"
    Private ReadOnly client As New HttpClient() With {.Timeout = TimeSpan.FromSeconds(5)}

    Public Async Function GetAsync(path As String) As Task(Of String)
        Dim url = $"{BasePath}{path}.json"
        Using response = Await client.GetAsync(url)
            response.EnsureSuccessStatusCode()
            Return Await response.Content.ReadAsStringAsync()
        End Using
    End Function

End Module
