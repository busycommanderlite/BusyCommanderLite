Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json

Public Class DispatchRepository
    Private Shared ReadOnly fileLock As New Object()
    Private Shared recordsCache As List(Of DispatchRecord) = Nothing

    Public Shared ReadOnly Property DbFolder As String
        Get
            Return "C:\BusyWin\Whatsapp"
        End Get
    End Property

    Public Shared ReadOnly Property DbPath As String
        Get
            Return Path.Combine(DbFolder, "Dispatches.json")
        End Get
    End Property

    Private Shared Sub EnsureLoaded()
        SyncLock fileLock
            If recordsCache IsNot Nothing Then Return

            If Not Directory.Exists(DbFolder) Then
                Directory.CreateDirectory(DbFolder)
            End If

            If File.Exists(DbPath) Then
                Try
                    Dim json = File.ReadAllText(DbPath)
                    recordsCache = JsonConvert.DeserializeObject(Of List(Of DispatchRecord))(json)
                Catch ex As Exception
                    recordsCache = Nothing
                End Try
            End If

            If recordsCache Is Nothing Then
                recordsCache = New List(Of DispatchRecord)()
                SaveToFile()
            End If
        End SyncLock
    End Sub

    Private Shared Sub SaveToFile()
        SyncLock fileLock
            Try
                If Not Directory.Exists(DbFolder) Then
                    Directory.CreateDirectory(DbFolder)
                End If

                Dim json = JsonConvert.SerializeObject(recordsCache, Formatting.Indented)
                Dim tempPath = DbPath & ".tmp"
                File.WriteAllText(tempPath, json)

                If File.Exists(DbPath) Then
                    File.Copy(tempPath, DbPath, True)
                    File.Delete(tempPath)
                Else
                    File.Move(tempPath, DbPath)
                End If
            Catch ex As Exception
                ' Fallback direct write
                Try
                    Dim json = JsonConvert.SerializeObject(recordsCache, Formatting.Indented)
                    File.WriteAllText(DbPath, json)
                Catch
                End Try
            End Try
        End SyncLock
    End Sub

    Public Shared Function SaveDispatches(signContent As String, givenGadi As String) As Boolean
        EnsureLoaded()
        If String.IsNullOrWhiteSpace(signContent) Then Return False

        SyncLock fileLock
            Dim lines = signContent.Split(New String() {LineSep}, StringSplitOptions.RemoveEmptyEntries)
            Dim todayStr = DateTime.Today.ToString("yyyy-MM-dd")
            Dim nextId = If(recordsCache.Count > 0, recordsCache.Max(Function(r) r.Id) + 1, 1)
            Dim addedAny = False

            For Each line In lines
                Dim parts = line.Split(New String() {EntrySep}, StringSplitOptions.None)
                If parts.Length >= 3 Then
                    Dim partyName = parts(0).Trim()
                    Dim vchNo = parts(1).Trim()
                    Dim nag = parts(2).Trim()
                    Dim partyHindi = If(parts.Length >= 4, parts(3).Trim(), partyName)

                    Dim rec As New DispatchRecord With {
                        .Id = nextId,
                        .VchNo = vchNo,
                        .PartyName = partyName,
                        .PartyHindi = partyHindi,
                        .DispatchDate = todayStr,
                        .Nag = nag,
                        .GivenGadiNo = If(givenGadi, "").Trim(),
                        .ActualGadiNo = "",
                        .CreatedAt = DateTime.Now,
                        .UpdatedAt = DateTime.Now
                    }
                    recordsCache.Add(rec)
                    nextId += 1
                    addedAny = True
                End If
            Next

            If addedAny Then
                SaveToFile()
                Return True
            End If
            Return False
        End SyncLock
    End Function

    Public Shared Function GetPendingDispatches() As List(Of DispatchRecord)
        EnsureLoaded()
        SyncLock fileLock
            Return recordsCache.Where(Function(r) String.IsNullOrWhiteSpace(r.ActualGadiNo)) _
                               .OrderByDescending(Function(r) r.Id) _
                               .ToList()
        End SyncLock
    End Function

    Public Shared Function GetAllDispatches() As List(Of DispatchRecord)
        EnsureLoaded()
        SyncLock fileLock
            Return recordsCache.OrderByDescending(Function(r) r.Id).ToList()
        End SyncLock
    End Function

    Public Shared Function GetDispatchById(id As Integer) As DispatchRecord
        EnsureLoaded()
        SyncLock fileLock
            Return recordsCache.FirstOrDefault(Function(r) r.Id = id)
        End SyncLock
    End Function

    Public Shared Function GetPendingDispatchForVoucher(vchNo As String) As DispatchRecord
        EnsureLoaded()
        If String.IsNullOrWhiteSpace(vchNo) Then Return Nothing

        SyncLock fileLock
            Return recordsCache.FirstOrDefault(Function(r) r.VchNo.Equals(vchNo.Trim(), StringComparison.OrdinalIgnoreCase) AndAlso String.IsNullOrWhiteSpace(r.ActualGadiNo))
        End SyncLock
    End Function



    Public Shared Function UpdateActualGadi(id As Integer, actualGadi As String) As Boolean
        EnsureLoaded()
        SyncLock fileLock
            Dim rec = recordsCache.FirstOrDefault(Function(r) r.Id = id)
            If rec IsNot Nothing Then
                rec.ActualGadiNo = If(actualGadi, "").Trim()
                rec.UpdatedAt = DateTime.Now
                SaveToFile()
                Return True
            End If
            Return False
        End SyncLock
    End Function

    Public Shared Function DeleteDispatch(id As Integer) As Boolean
        EnsureLoaded()
        SyncLock fileLock
            Dim rec = recordsCache.FirstOrDefault(Function(r) r.Id = id)
            If rec IsNot Nothing Then
                recordsCache.Remove(rec)
                SaveToFile()
                Return True
            End If
            Return False
        End SyncLock
    End Function

    Public Shared Function GetLatestActualGadiForParty(partyName As String) As String
        EnsureLoaded()
        If String.IsNullOrWhiteSpace(partyName) Then Return ""

        SyncLock fileLock
            Dim rec = recordsCache.Where(Function(r) r.PartyName.Equals(partyName.Trim(), StringComparison.OrdinalIgnoreCase) AndAlso Not String.IsNullOrWhiteSpace(r.ActualGadiNo)) _
                                  .OrderByDescending(Function(r) r.Id) _
                                  .FirstOrDefault()
            If rec IsNot Nothing Then
                Return rec.ActualGadiNo
            End If
            Return ""
        End SyncLock
    End Function

    Public Shared Function GetMatchingGadiNumbers(query As String) As List(Of String)
        EnsureLoaded()
        SyncLock fileLock
            Dim q = If(query, "").Trim().ToUpperInvariant()
            Return recordsCache.Where(Function(r) Not String.IsNullOrWhiteSpace(r.ActualGadiNo) AndAlso (q = "" OrElse r.ActualGadiNo.ToUpperInvariant().Contains(q))) _
                               .Select(Function(r) r.ActualGadiNo.Trim()) _
                               .Distinct(StringComparer.OrdinalIgnoreCase) _
                               .Take(20) _
                               .ToList()
        End SyncLock
    End Function

    Public Shared Function GetVehicleMatchingDataString(query As String) As String
        EnsureLoaded()
        Dim q = If(query, "").Trim()
        If String.IsNullOrEmpty(q) Then Return ""

        Dim seenTransports As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim resultLines As New List(Of String)()

        SyncLock fileLock
            ' 1. Search Dispatches.json (recordsCache)
            For Each rec In recordsCache.OrderByDescending(Function(r) r.Id)
                If Not String.IsNullOrWhiteSpace(rec.ActualGadiNo) AndAlso rec.ActualGadiNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Dim trans = rec.ActualGadiNo.Trim()
                    If seenTransports.Add(trans) Then
                        Dim dt = If(String.IsNullOrWhiteSpace(rec.DispatchDate), "Recent", rec.DispatchDate.Trim())
                        Dim pName = If(String.IsNullOrWhiteSpace(rec.PartyName), "Party", rec.PartyName.Trim())
                        resultLines.Add($"{dt}{EntrySep}{trans}{EntrySep}{pName}")
                    End If
                End If
                If Not String.IsNullOrWhiteSpace(rec.GivenGadiNo) AndAlso rec.GivenGadiNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    Dim trans = rec.GivenGadiNo.Trim()
                    If seenTransports.Add(trans) Then
                        Dim dt = If(String.IsNullOrWhiteSpace(rec.DispatchDate), "Recent", rec.DispatchDate.Trim())
                        Dim pName = If(String.IsNullOrWhiteSpace(rec.PartyName), "Party", rec.PartyName.Trim())
                        resultLines.Add($"{dt}{EntrySep}{trans}{EntrySep}{pName}")
                    End If
                End If
            Next
        End SyncLock

        ' Historical search has been removed.

        ' 3. Handle user's typed entry:
        ' If matches were found but none exactly matches q: append q as "New Entry"
        ' If no matches were found: provide q as "New Entry"
        If resultLines.Count = 0 Then
            resultLines.Add($"New Entry{EntrySep}{q}{EntrySep}New Entry")
        ElseIf Not seenTransports.Contains(q) Then
            resultLines.Add($"New Entry{EntrySep}{q}{EntrySep}New Entry")
        End If

        Return String.Join(LineSep, resultLines) & LineSep
    End Function

    Public Shared Function GetDispatchesForVoucher(vchNo As String) As List(Of DispatchRecord)
        EnsureLoaded()
        If String.IsNullOrWhiteSpace(vchNo) Then Return New List(Of DispatchRecord)()

        SyncLock fileLock
            Return recordsCache.Where(Function(r) r.VchNo.Equals(vchNo.Trim(), StringComparison.OrdinalIgnoreCase)) _
                               .OrderBy(Function(r) r.Id) _
                               .ToList()
        End SyncLock
    End Function





    Public Shared Function GetDispatchInfoSummary(vchNo As String) As String
        Dim dispatches = GetDispatchesForVoucher(vchNo)
        If dispatches.Count = 0 Then
            Return "No dispatch record found."
        End If

        Dim sb As New System.Text.StringBuilder()
        For Each d In dispatches
            Dim createdStr = d.CreatedAt.ToString("dd-MMM-yyyy hh:mm tt")
            Dim updatedStr = "Pending"
            If Not String.IsNullOrWhiteSpace(d.ActualGadiNo) Then
                updatedStr = d.UpdatedAt.ToString("dd-MMM-yyyy hh:mm tt") & "  [ " & d.ActualGadiNo & " ]"
            End If
            sb.AppendLine("Sign: " & d.Nag)
            sb.AppendLine("   Created: " & createdStr)
            sb.AppendLine("   Updated: " & updatedStr)
            sb.AppendLine(New String("-"c, 40))
        Next

        Return sb.ToString().TrimEnd()
    End Function

End Class
