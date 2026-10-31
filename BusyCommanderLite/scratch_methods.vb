
    Public Sub SendSign(MobileNumber As String, PartyName As String, PartyNameHindi As String, NagAndGadi As String)
        NagAndGadi = NagAndGadi.Replace("[", "").Replace("]", "").Trim()
        If NagAndGadi.Split("@"c).Length >= 2 Then
            'Send whatsapp
            Dim content As String

            'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
            If Not PartyNameHindi.Equals(PartyName) Then
                PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
                content = String.Format(DecodeString(MyApp.KeyDict("SignText")), NagAndGadi.Split("@"c)(0), PartyNameHindi, NagAndGadi.Split("@"c)(1))
            Else
                content = String.Format(DecodeString(MyApp.KeyDict("SignText")), NagAndGadi.Split("@"c)(0), PartyName, NagAndGadi.Split("@"c)(1))
            End If
            SendTextMessage(MobileNumber, content)
        End If
    End Sub

    Public Sub SendHisab(MobileNumber As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Dim hisabForm As New HisabForm()
        Dim bitMap As Bitmap
        Dim commonWidth, gridHeight, partyLabelHeight, totalLabelHeight As Integer

        Dim bakiAmt As Integer = 0
        Dim jamaAmt As Integer = 0
        Dim currentAmt As Integer
        Dim daysOld As Integer

        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            hisabForm.PartyLabel.Text = PartyNameHindi
            'hisabForm.PartyLabel.Font = New Font("Kruti Dev 100", hisabForm.PartyLabel.Font.Size + 10)
        Else
            hisabForm.PartyLabel.Text = PartyName
        End If
        For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 3 Then
                If Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1).Trim().Equals("") Then
                    'Baki Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1))
                    bakiAmt += currentAmt
                    daysOld = (Today - CDate(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0))).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), CStr(currentAmt), "", daysOld & " दिन"})
                ElseIf Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2).Trim().Equals("") Then
                    'Jama Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
                    jamaAmt += currentAmt
                    daysOld = (Today - CDate(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0))).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), "", CStr(currentAmt), daysOld & " दिन"})
                End If
            End If
        Next
        hisabForm.TotalLabel.Text = "कुल बकाया : " & (bakiAmt - jamaAmt)

        hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
        Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold), .BackColor = Color.FromArgb(230, 230, 230)}
        hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

        partyLabelHeight = hisabForm.PartyLabel.Height
        totalLabelHeight = hisabForm.TotalLabel.Height
        hisabForm.HisabDataGrid.Height = hisabForm.HisabDataGrid.ColumnHeadersHeight + hisabForm.HisabDataGrid.RowCount * hisabForm.HisabDataGrid.Rows(0).Height + 3
        commonWidth = hisabForm.HisabDataGrid.Width
        gridHeight = hisabForm.HisabDataGrid.Height
        hisabForm.HisabDataGrid.ClearSelection()
        bitMap = New Bitmap(commonWidth + 40, gridHeight + partyLabelHeight + totalLabelHeight + 40)
        Drawing.Graphics.FromImage(bitMap).Clear(Color.White)
        hisabForm.PartyLabel.DrawToBitmap(bitMap, New Rectangle(20, 20, commonWidth, partyLabelHeight))
        hisabForm.HisabDataGrid.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight, commonWidth, gridHeight))
        hisabForm.TotalLabel.DrawToBitmap(bitMap, New Rectangle(20, 20 + partyLabelHeight + gridHeight, commonWidth, totalLabelHeight))

        'Send whatsapp
        SendMediaMessage(MobileNumber, bitMap)
    End Sub

    Public Sub PrintHisab(LongShort As String, PartyName As String, PartyNameHindi As String, HisabText As String)
        Dim hisabForm As New HisabForm

        Dim bakiAmt As Integer = 0
        Dim jamaAmt As Integer = 0
        Dim currentAmt As Integer
        Dim daysOld As Integer

        'If PartyNameHindi and PartyName are same, it means no PartyNameHindi was found
        If Not PartyNameHindi.Equals(PartyName) Then
            PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
            hisabForm.PartyLabel.Text = PartyNameHindi
            'hisabForm.PartyLabel.Font = New Font("Kruti Dev 100", hisabForm.PartyLabel.Font.Size + 10)
        Else
            hisabForm.PartyLabel.Text = PartyName
        End If
        If LongShort.Equals("Long") Then
            hisabForm.LongSlip = True
        ElseIf LongShort.Equals("Short") Then
            hisabForm.LongSlip = False
        Else
            hisabForm.LongSlip = False
        End If
        For Each singleLine As String In HisabText.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 3 Then
                If Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1).Trim().Equals("") Then
                    'Baki Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1))
                    bakiAmt += currentAmt
                    Dim dateStr = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                    Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                    daysOld = (Today - entryDate).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), CStr(currentAmt), "", daysOld & " दिन"})
                ElseIf Not singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2).Trim().Equals("") Then
                    'Jama Entry
                    currentAmt = CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
                    jamaAmt += currentAmt
                    Dim dateStr = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                    Dim entryDate = Date.ParseExact(dateStr.Trim("""".ToArray()), "dd-MM-yyyy", Globalization.DateTimeFormatInfo.InvariantInfo)
                    daysOld = (Today - CDate(entryDate)).Days
                    hisabForm.HisabDataGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0), "", CStr(currentAmt), daysOld & " दिन"})
                End If
            End If
        Next
        hisabForm.TotalLabel.Text = "कुल बकाया : " & (bakiAmt - jamaAmt)

        hisabForm.HisabDataGrid.Rows.Add(New String() {"Total", CStr(bakiAmt), CStr(jamaAmt), ""})
        Dim cellStyle As New DataGridViewCellStyle() With {.Font = New System.Drawing.Font("MS Reference Sans Serif", 20.0!, FontStyle.Bold)}
        hisabForm.HisabDataGrid.Rows(hisabForm.HisabDataGrid.Rows.Count - 1).DefaultCellStyle = cellStyle

        hisabForm.HisabDataGrid.ClearSelection()
        hisabForm.PrintDocument1.Print()
    End Sub

    Public Function PrintSign(SignContent As String, GadiNumber As String) As Boolean
        Dim signForm As New SignForm()
        Dim totalDag As Integer
        Dim PartyName, VchNo, DagCnt, PartyNameHindi As String

        If SignContent.Split(New String() {LineSep}, StringSplitOptions.None).Length = 0 Then
            Return False
        End If
        GadiNumber = "गाड़ी नं:-" & GadiNumber
        GadiNumber &= String.Join("", Enumerable.Repeat(" ", 29 - Len(GadiNumber)))
        signForm.DateLabel.Text = GadiNumber & Now.ToString("dddd dd-MM-yyyy hh:mm tt")
        totalDag = 0
        For Each singleLine As String In SignContent.Split(New String() {LineSep}, StringSplitOptions.None)
            If singleLine.Split(New String() {EntrySep}, StringSplitOptions.None).Length = 4 Then
                PartyName = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(0)
                VchNo = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(1)
                DagCnt = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2)
                PartyNameHindi = singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(3)
                If Not PartyName.Equals(PartyNameHindi) Then
                    PartyNameHindi = ConvertKrutidevToUnicode(PartyNameHindi)
                    signForm.SignGrid.Rows.Add(New String() {DagCnt, "डाग", PartyNameHindi})
                Else
                    signForm.SignGrid.Rows.Add(New String() {singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2), "डाग", PartyName})
                End If
                totalDag += CInt(singleLine.Split(New String() {EntrySep}, StringSplitOptions.None)(2))
            End If
        Next
        signForm.TotalLabel.Text = "कुल " & totalDag & " डाग             दस्तखत:-"

        signForm.SignGrid.ClearSelection()
        Try
            signForm.PrintDocument1.Print()
        Catch ex As Exception
            MsgBox("Error printing slip: " & ex.Message, MsgBoxStyle.Exclamation, "Print Error")
            Return False
        End Try
        Return True
    End Function

