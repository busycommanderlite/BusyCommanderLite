Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Printing

Public Class SignForm

    Public Property GadiText As String = ""
    Public Property DateText As String = ""

    Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        ' Target slip bounds on the printed page (hundredths of an inch):
        ' X = 200 (2.0 inch left margin)
        ' Width = 380 (3.8 inch width)
        Dim startX As Integer = 200
        Dim slipWidth As Integer = 380

        Dim rowCount As Integer = Me.SignGrid.Rows.Count
        Dim effectiveRowCount As Integer = Math.Max(1, rowCount)

        ' Moving the center cut line to the exact physical A6 half (~1 cm above original):
        ' Original was ~289.0!, test was 248.0! (moved ~2cm). Target is 270.0! (exactly 1 cm above original).
        Dim topMargin As Single = 16.0!
        Dim middleGap As Single = 24.0!
        Dim copyHeight As Single = 242.0!

        Dim headerHeight As Single = 38.0!
        Dim footerHeight As Single = 38.0!
        Dim gridHeight As Single = copyHeight - headerHeight - footerHeight   ' 166.0!
        Dim rowHeight As Single = gridHeight / CSng(effectiveRowCount)

        Dim copy1Y As Single = topMargin                                      ' 16.0!
        Dim cutLineY As Single = copy1Y + copyHeight + (middleGap / 2.0!)     ' 270.0!
        Dim copy2Y As Single = copy1Y + copyHeight + middleGap                ' 282.0!

        Dim rowFontSize As Single
        If rowHeight >= 100.0! Then
            rowFontSize = 13.5!
        ElseIf rowHeight >= 60.0! Then
            rowFontSize = 12.5!
        ElseIf rowHeight >= 45.0! Then
            rowFontSize = 11.5!
        ElseIf rowHeight >= 35.0! Then
            rowFontSize = 10.5!
        ElseIf rowHeight >= 26.0! Then
            rowFontSize = 9.5!
        ElseIf rowHeight >= 20.0! Then
            rowFontSize = 8.5!
        Else
            rowFontSize = 7.5!
        End If

        Dim gadiPart As String = If(Not String.IsNullOrEmpty(Me.GadiText), Me.GadiText, "")
        Dim datePart As String = If(Not String.IsNullOrEmpty(Me.DateText), Me.DateText, "")
        If String.IsNullOrEmpty(datePart) AndAlso String.IsNullOrEmpty(gadiPart) Then
            gadiPart = If(Me.DateLabel.Text, "")
            datePart = Now.ToString("dddd dd-MM-yyyy hh:mm tt")
        End If

        Dim headerFontSize As Single = 10.0!
        Dim footerFontSize As Single = Math.Max(8.5!, Math.Min(11.0!, footerHeight * 0.30!))

        ' Column widths (Nag: 50, Description "डाग": 70, Party Name: remainder ~260)
        Dim col0Width As Single = 50.0!
        Dim col1Width As Single = 70.0!
        Dim col2Width As Single = CSng(slipWidth) - col0Width - col1Width

        Using fontHeader As New Font("MS Reference Sans Serif", headerFontSize, FontStyle.Bold),
              fontFooter As New Font("MS Reference Sans Serif", footerFontSize, FontStyle.Bold),
              fontNag As New Font("MS Reference Sans Serif", rowFontSize, FontStyle.Bold),
              fontRow As New Font("MS Reference Sans Serif", rowFontSize, FontStyle.Regular),
              fontParty As New Font("MS Reference Sans Serif", rowFontSize, FontStyle.Bold),
              penBorder As New Pen(Color.Black, 1.2!),
              penRowDivider As New Pen(Color.FromArgb(170, 170, 170), 0.8!) With {.DashStyle = DashStyle.Dot},
              penCutDash As New Pen(Color.FromArgb(120, 120, 120), 1.0!) With {.DashPattern = New Single() {6.0!, 4.0!}},
              sfCenter As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap},
              sfLeft As New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap},
              sfRight As New StringFormat() With {.Alignment = StringAlignment.Far, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap}

            ' Sub-routine to draw one copy of the slip at a given Y position
            Dim DrawCopy = Sub(copyY As Single)
                               Dim curY As Single = copyY

                                ' 1. Header Box (Gadi Number on Left, Date & Time on Right)
                                Dim headerRect As New RectangleF(CSng(startX), curY, CSng(slipWidth), headerHeight)
                                Dim headerPadRect As New RectangleF(CSng(startX) + 8.0!, curY, CSng(slipWidth) - 16.0!, headerHeight)
                                g.DrawRectangle(penBorder, headerRect.X, headerRect.Y, headerRect.Width, headerRect.Height)

                                If Not String.IsNullOrEmpty(gadiPart) Then
                                    Dim szDate As SizeF = If(Not String.IsNullOrEmpty(datePart), g.MeasureString(datePart, fontHeader), SizeF.Empty)
                                    Dim gadiWidth As Single = Math.Max(40.0!, headerPadRect.Width - szDate.Width - 10.0!)
                                    Dim gadiRect As New RectangleF(headerPadRect.X, headerPadRect.Y, gadiWidth, headerPadRect.Height)

                                    ' Auto-scale font if gadi text is long so it fits completely without truncation
                                    Dim gadiFontSize As Single = headerFontSize
                                    Using testFont As New Font("MS Reference Sans Serif", gadiFontSize, FontStyle.Bold)
                                        Dim szGadi As SizeF = g.MeasureString(gadiPart, testFont)
                                        If szGadi.Width > gadiWidth Then
                                            gadiFontSize = Math.Max(7.5!, (gadiWidth / szGadi.Width) * gadiFontSize)
                                        End If
                                    End Using

                                    Using fontGadi As New Font("MS Reference Sans Serif", gadiFontSize, FontStyle.Bold)
                                        g.DrawString(gadiPart, fontGadi, Brushes.Black, gadiRect, sfLeft)
                                    End Using
                                End If
                                If Not String.IsNullOrEmpty(datePart) Then
                                    g.DrawString(datePart, fontHeader, Brushes.Black, headerPadRect, sfRight)
                                End If
                                curY += headerHeight

                                ' 2. Sign Content (Outer solid box border)
                               Dim gridStartY As Single = curY
                               g.DrawRectangle(penBorder, CSng(startX), gridStartY, CSng(slipWidth), gridHeight)

                               ' Inside the box:
                               ' - NO vertical divider lines between columns
                               ' - Subtle dotted line between consecutive signs (rows)
                               For r As Integer = 0 To rowCount - 1
                                   Dim row As DataGridViewRow = Me.SignGrid.Rows(r)
                                   Dim valNag As String = If(row.Cells(0).Value?.ToString(), "")
                                   Dim valDesc As String = If(row.Cells(1).Value?.ToString(), "")
                                   Dim valParty As String = If(row.Cells(2).Value?.ToString(), "")

                                   ' Draw dotted divider between rows
                                   If r > 0 Then
                                       g.DrawLine(penRowDivider, CSng(startX), curY, CSng(startX + slipWidth), curY)
                                   End If

                                   Dim cell0Rect As New RectangleF(CSng(startX), curY, col0Width, rowHeight)
                                   Dim cell1Rect As New RectangleF(CSng(startX) + col0Width, curY, col1Width, rowHeight)
                                   Dim cell2Rect As New RectangleF(CSng(startX) + col0Width + col1Width, curY, col2Width, rowHeight)

                                   ' 1. Nag (bold, centered in column 0)
                                   g.DrawString(valNag, fontNag, Brushes.Black, cell0Rect, sfCenter)

                                   ' 2. Description ("डाग", regular, left-aligned in column 1)
                                   Dim cell1Padded As New RectangleF(cell1Rect.X + 4.0!, cell1Rect.Y, cell1Rect.Width - 6.0!, cell1Rect.Height)
                                   g.DrawString(valDesc, fontRow, Brushes.Black, cell1Padded, sfLeft)

                                   ' 3. Party Name (bold, right-aligned in column 2 with margin)
                                   Dim cell2Padded As New RectangleF(cell2Rect.X + 4.0!, cell2Rect.Y, cell2Rect.Width - 10.0!, cell2Rect.Height)
                                   g.DrawString(valParty, fontParty, Brushes.Black, cell2Padded, sfRight)

                                   curY += rowHeight
                               Next

                               ' 3. Footer Box (TotalLabel: Total dag & signature line)
                               curY = gridStartY + gridHeight
                               Dim footerRect As New RectangleF(CSng(startX), curY, CSng(slipWidth), footerHeight)
                               g.DrawRectangle(penBorder, footerRect.X, footerRect.Y, footerRect.Width, footerRect.Height)
                               Dim footerPadded As New RectangleF(footerRect.X + 8.0!, footerRect.Y, footerRect.Width - 16.0!, footerRect.Height)
                               Dim footerText As String = If(Me.TotalLabel.Text, "")
                               g.DrawString(footerText, fontFooter, Brushes.Black, footerPadded, sfLeft)
                           End Sub

            ' Draw Copy 1 (Top copy)
            DrawCopy(copy1Y)

            ' Separator dashed cut line between the two copies (~1 cm higher)
            g.DrawLine(penCutDash, CSng(startX), cutLineY, CSng(startX + slipWidth), cutLineY)

            ' Draw Copy 2 (Bottom copy)
            DrawCopy(copy2Y)

        End Using

        e.HasMorePages = False
    End Sub

End Class
