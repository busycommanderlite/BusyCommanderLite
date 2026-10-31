Public Class HisabForm
    Public LongSlip As Boolean

    Private Sub PrintDocument1_PrintPage(sender As Object, e As Drawing.Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim commonWidth As Integer

        Dim rowHeight As Integer = If(Me.HisabDataGrid.RowCount > 0, Me.HisabDataGrid.Rows(0).Height, 22)
        Me.HisabDataGrid.Height = Me.HisabDataGrid.ColumnHeadersHeight + Me.HisabDataGrid.RowCount * rowHeight + 3
        commonWidth = Me.HisabDataGrid.Width

        Using myBitmap As New Bitmap(commonWidth + 40, Me.HisabDataGrid.Height + Me.PartyLabel.Height + Me.TotalLabel.Height + 40)
            Using g As Graphics = Graphics.FromImage(myBitmap)
                g.Clear(Color.White)
            End Using
            Me.PartyLabel.DrawToBitmap(myBitmap, New Rectangle(20, 20, commonWidth, Me.PartyLabel.Height))
            Me.HisabDataGrid.DrawToBitmap(myBitmap, New Rectangle(20, 20 + Me.PartyLabel.Height, commonWidth, Me.HisabDataGrid.Height))
            Me.TotalLabel.DrawToBitmap(myBitmap, New Rectangle(20, 20 + Me.PartyLabel.Height + Me.HisabDataGrid.Height, commonWidth, Me.TotalLabel.Height))

            If LongSlip Then
                e.Graphics.DrawImage(myBitmap, 200, 0, 380, 1100)
            Else
                If CInt(550 * myBitmap.Height / 1127) > 550 Then
                    e.Graphics.DrawImage(myBitmap, 200, 0, 380, 550)
                Else
                    e.Graphics.DrawImage(myBitmap, 200, 0, 380, CInt(550 * myBitmap.Height / 1127))
                End If
            End If
        End Using
    End Sub

End Class