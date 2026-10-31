Imports System.Net.Http
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json.Linq

Public Class BCTransliterateForm
    Private UnicodeTextToConvert As String = ""
    Private ConversionPending As Boolean = False
    Private SelectedKrutiText As String = ""
    Private openTime As DateTime

    Public Function ShowDialogWithData(inputText As String, Optional owner As IWin32Window = Nothing) As String
        openTime = DateTime.Now
        ' Clean up query string
        Dim queryStr = Regex.Replace(inputText.Trim(), "[^a-zA-Z- ]", "")
        queryStr = queryStr.Replace("-", " - ")
        While queryStr.Contains("  ")
            queryStr = queryStr.Replace("  ", " ")
        End While
        queryStr = queryStr.Replace("-", " ")

        ' UI setup
        ResultsLB.Items.Clear()
        StartPosition = FormStartPosition.CenterScreen
        KeyPreview = True
        'Text = "Transliteration Options"
        'lblSubheading.Text = "Select the correct transliteration"

        ' Fetch transliterations
        Using client As New HttpClient()
            Try
                client.Timeout = TimeSpan.FromSeconds(5)
                Dim url = $"https://www.google.com/inputtools/request?text={Uri.EscapeDataString(queryStr)}&ime=transliteration_en_hi&num=5"
                Dim response = client.GetStringAsync(url).Result

                Dim json = JArray.Parse(response)
                If json(0).ToString() = "SUCCESS" Then
                    Dim resultSet = json(1)
                    For Each item In resultSet
                        Dim fullSuggestions = item(1)
                        For Each suggestion In fullSuggestions
                            ResultsLB.Items.Add(suggestion.ToString())
                        Next
                    Next
                End If
            Catch ex As Exception
                MessageBox.Show("Error fetching transliterations: " & ex.Message)
            End Try
        End Using

        If ResultsLB.Items.Count > 0 Then
            ResultsLB.SelectedIndex = 0
        End If

        ' Show dialog
        Dim dlgResult As DialogResult = If(owner IsNot Nothing, ShowDialog(owner), ShowDialog())
        If dlgResult = DialogResult.OK Then
            Return SelectedKrutiText
        Else
            Return ""
        End If
    End Function

    ' Keyboard handling
    Private Sub BCTransliterateForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (DateTime.Now - openTime).TotalMilliseconds < 300 Then
            e.Handled = True
            e.SuppressKeyPress = True
            Return
        End If

        If e.KeyCode = Keys.F2 Or e.KeyCode = Keys.Enter Then
            StartHtmlConversion()
        ElseIf e.KeyCode = Keys.Escape Then
            DialogResult = DialogResult.Cancel
            Close()
        End If
    End Sub

    ' Double-click to select
    Private Sub ResultsLB_DoubleClick(sender As Object, e As EventArgs) Handles ResultsLB.DoubleClick
        StartHtmlConversion()
    End Sub

    Private Sub StartHtmlConversion()
        If ResultsLB.SelectedIndex >= 0 Then
            UnicodeTextToConvert = ResultsLB.SelectedItem.ToString()
            ConversionPending = True
            WebBrowser1.ScriptErrorsSuppressed = True
            WebBrowser1.DocumentText = HtmlFileContents
        End If
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        If ConversionPending Then
            Try
                WebBrowser1.Document.GetElementById("unicode_text").InnerText = UnicodeTextToConvert
                WebBrowser1.Document.InvokeScript("Convert_to_Krutidev_010")

                Dim tmr As New Timer()
                AddHandler tmr.Tick, Sub()
                                         tmr.Stop()
                                         SaveKrutiResult()
                                     End Sub
                tmr.Interval = 300
                tmr.Start()
            Catch ex As Exception
                MessageBox.Show("Conversion failed: " & ex.Message)
                Close()
            End Try
            ConversionPending = False
        End If
    End Sub

    Private Sub SaveKrutiResult()
        Try
            Dim krutiText = WebBrowser1.Document.GetElementById("legacy_text").InnerText
            krutiText = krutiText.Replace("   ", " & ")
            SelectedKrutiText = krutiText
            DialogResult = DialogResult.OK
        Catch ex As Exception
            MessageBox.Show("Error saving file: " & ex.Message)
            DialogResult = DialogResult.Cancel
        End Try
        Close()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.MediumSlateBlue, ButtonBorderStyle.Solid)
    End Sub

End Class