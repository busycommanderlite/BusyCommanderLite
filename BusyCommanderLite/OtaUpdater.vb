Imports System.IO
Imports System.IO.Compression
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Module OtaUpdater

    Private Const FIREBASE_BASE_URL As String = "https://busymasterdatabase-default-rtdb.asia-southeast1.firebasedatabase.app/bc_scripts/"
    Private ReadOnly httpClient As New HttpClient() With {.Timeout = TimeSpan.FromMinutes(3)}

    ''' <summary>
    ''' Gets the installation directory where server.dll and BusyCommanderLite.dll reside.
    ''' </summary>
    Public Function GetAppDirectory() As String
        Return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
    End Function

    ''' <summary>
    ''' Reads the currently active integer version from {targetKey}_version.txt (default: 1).
    ''' targetKey is "server" or "bclite".
    ''' </summary>
    Public Function GetLocalVersion(targetKey As String) As Integer
        Try
            Dim verFile = Path.Combine(GetAppDirectory(), targetKey & "_version.txt")
            If File.Exists(verFile) Then
                Dim text = File.ReadAllText(verFile).Trim()
                Dim v As Integer
                If Integer.TryParse(text, v) Then Return v
            End If
        Catch ex As Exception
        End Try
        Return 1
    End Function

    ''' <summary>
    ''' Cleans up any unlocked .old backup files from previous updates.
    ''' </summary>
    Private Sub CleanOldBackups(appDir As String, dllFileName As String)
        Try
            For Each f In Directory.GetFiles(appDir, dllFileName & "*.old")
                Try
                    File.Delete(f)
                Catch
                    ' Still locked by another running process, skip
                End Try
            Next
        Catch
        End Try
    End Sub

    ''' <summary>
    ''' Safely moves an active (and possibly locked) DLL into a backup slot, using a numbered suffix if the standard .old is locked.
    ''' </summary>
    Private Sub SafeMoveToBackup(appDir As String, filePath As String, dllFileName As String)
        If Not File.Exists(filePath) Then Return

        Dim baseOld = Path.Combine(appDir, dllFileName & ".old")
        Dim targetOld = baseOld
        Dim counter As Integer = 1

        While File.Exists(targetOld)
            Try
                File.Delete(targetOld)
                Exit While
            Catch ex As Exception
                targetOld = Path.Combine(appDir, dllFileName & "." & counter & ".old")
                counter += 1
            End Try
        End While

        File.Move(filePath, targetOld)
    End Sub

    ''' <summary>
    ''' Validates that a file exists, meets minimum size, and has a valid PE header ("MZ").
    ''' </summary>
    Private Function IsValidDll(filePath As String, minSize As Long) As Boolean
        Try
            If Not File.Exists(filePath) Then Return False
            Dim fi As New FileInfo(filePath)
            If fi.Length < minSize Then Return False

            Using fs As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Dim header(1) As Byte
                If fs.Read(header, 0, 2) = 2 Then
                    ' Windows Executable / DLL header begins with "MZ" (0x4D, 0x5A)
                    Return (header(0) = &H4D AndAlso header(1) = &H5A)
                End If
            End Using
        Catch ex As Exception
        End Try
        Return False
    End Function

    Private currentDownloadTask As Task = Nothing
    Private ReadOnly downloadLock As New Object()

    ''' <summary>
    ''' Returns True if a background download of an OTA update is currently in progress.
    ''' </summary>
    Public Function IsDownloadInProgress() As Boolean
        SyncLock downloadLock
            Return currentDownloadTask IsNot Nothing AndAlso Not currentDownloadTask.IsCompleted
        End SyncLock
    End Function

    ''' <summary>
    ''' Returns the active download Task, allowing caller to Wait() synchronously.
    ''' </summary>
    Public Function GetActiveDownloadTask() As Task
        SyncLock downloadLock
            Return currentDownloadTask
        End SyncLock
    End Function

    ''' <summary>
    ''' Starts the background update check/download if not already running.
    ''' Returns the running Task.
    ''' </summary>
    Public Function StartBackgroundUpdateCheck() As Task
        SyncLock downloadLock
            If currentDownloadTask Is Nothing OrElse currentDownloadTask.IsCompleted Then
                currentDownloadTask = Task.Run(Function() CheckAndDownloadUpdatesAsync())
            End If
            Return currentDownloadTask
        End SyncLock
    End Function

    ''' <summary>
    ''' Called at startup BEFORE LoadLibrary("server.dll") is executed.
    ''' If server.dll.new or BusyCommanderLite.dll.new exists, it swaps them safely in-place.
    ''' Returns a user-facing notification string if any files were updated, or "" if none.
    ''' </summary>
    Public Function ApplyPendingUpdates() As String
        Try
            Dim appDir = GetAppDirectory()
            Dim swappedMessages As New List(Of String)()

            ' 1. Check and swap server.dll if server.dll.new exists
            Dim msgServer = SwapPendingDll(appDir, "server", "server.dll", 5 * 1024 * 1024)
            If Not String.IsNullOrEmpty(msgServer) Then swappedMessages.Add(msgServer)

            ' 2. Check and swap BusyCommanderLite.dll if BusyCommanderLite.dll.new exists
            Dim msgBclite = SwapPendingDll(appDir, "bclite", "BusyCommanderLite.dll", 50 * 1024)
            If Not String.IsNullOrEmpty(msgBclite) Then swappedMessages.Add(msgBclite)

            ' 3. Clean up obsolete legacy files and old backups in the background
            CleanUpObsoleteFiles(appDir)

            If swappedMessages.Count > 0 Then
                Return String.Join(vbCrLf, swappedMessages)
            End If

        Catch ex As Exception
            GoServerInterop.LogToFile("OTA: ApplyPendingUpdates exception: " & ex.Message)
        End Try
        Return ""
    End Function

    ''' <summary>
    ''' Automatically cleans up obsolete legacy files, old backups, and stale browser caches from C:\BusyWin\WhatsApp.
    ''' Runs safely in the background so it never blocks or slows down startup.
    ''' </summary>
    Public Sub CleanUpObsoleteFiles(Optional appDir As String = Nothing)
        If String.IsNullOrEmpty(appDir) Then appDir = GetAppDirectory()
        Task.Run(Sub()
                     Try
                         ' 1. Clean up unlocked .old backup files
                         For Each oldFile In Directory.GetFiles(appDir, "*.old")
                             Try
                                 File.Delete(oldFile)
                             Catch
                             End Try
                         Next

                         ' 2. Clean up legacy scripts and unused installer state / dead DLLs
                         Dim obsoleteFiles = {"CoreScript.js", "WhatsappScript.js", "BusyCommanderLite.InstallState",
                                              "Microsoft.Web.WebView2.Core.dll", "Microsoft.Web.WebView2.WinForms.dll",
                                              "Microsoft.Web.WebView2.Wpf.dll", "WebView2Loader.dll", "LOG"}
                         For Each fileName In obsoleteFiles
                             Try
                                 Dim p = Path.Combine(appDir, fileName)
                                 If File.Exists(p) Then File.Delete(p)
                             Catch
                             End Try
                         Next

                         ' 3. Clean up legacy 800 MB WebView2 cache directory (WhatsappData)
                         Dim legacyDataDir = Path.Combine(appDir, "WhatsappData")
                         If Directory.Exists(legacyDataDir) Then
                             Try
                                 Directory.Delete(legacyDataDir, True)
                                 GoServerInterop.LogToFile("CleanUp: Removed legacy WhatsappData browser cache (~800 MB freed).")
                             Catch ex As Exception
                                 GoServerInterop.LogToFile("CleanUp: Note on WhatsappData removal: " & ex.Message)
                             End Try
                         End If

                     Catch ex As Exception
                         GoServerInterop.LogToFile("CleanUp: Exception: " & ex.Message)
                     End Try
                 End Sub)
    End Sub

    ''' <summary>
    ''' Swaps target.dll.new into target.dll if valid.
    ''' Returns a success message if swapped, or "" if not.
    ''' </summary>
    Private Function SwapPendingDll(appDir As String, targetKey As String, dllFileName As String, minSize As Long) As String
        Dim dllPath = Path.Combine(appDir, dllFileName)
        Dim dllNew = Path.Combine(appDir, dllFileName & ".new")
        Dim verNew = Path.Combine(appDir, targetKey & "_version.new")
        Dim verCurrent = Path.Combine(appDir, targetKey & "_version.txt")

        ' Clean up any unlocked .old files from previous sessions
        CleanOldBackups(appDir, dllFileName)

        If File.Exists(dllNew) Then
            Try
                If IsValidDll(dllNew, minSize) Then
                    SafeMoveToBackup(appDir, dllPath, dllFileName)
                    File.Move(dllNew, dllPath)
                    GoServerInterop.LogToFile("OTA: Successfully swapped " & dllFileName & " from .new file!")

                    ' Update version file
                    Dim verText As String = ""
                    If File.Exists(verNew) Then
                        verText = File.ReadAllText(verNew).Trim()
                        File.Copy(verNew, verCurrent, True)
                        File.Delete(verNew)
                    End If

                    Dim verSuffix = If(String.IsNullOrEmpty(verText), "", " to v" & verText)
                    Return dllFileName & " was updated successfully via OTA" & verSuffix & "!"
                Else
                    GoServerInterop.LogToFile("OTA: " & dllFileName & ".new failed validation. Discarding invalid file.")
                    File.Delete(dllNew)
                    If File.Exists(verNew) Then File.Delete(verNew)
                End If
            Catch ex As Exception
                GoServerInterop.LogToFile("OTA: Failed to swap " & dllFileName & ": " & ex.Message)
            End Try
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Background entry point called by MyApp.StartWhatsapp to asynchronously check and download updates.
    ''' Completely non-blocking and safe.
    ''' </summary>
    Public Async Function CheckAndDownloadUpdatesAsync() As Task
        Try
            ' 1. Check server.dll updates
            Await DownloadAndStageUpdateAsync("server", "server.dll", 5 * 1024 * 1024)

            ' 2. Check BusyCommanderLite.dll updates
            Await DownloadAndStageUpdateAsync("bclite", "BusyCommanderLite.dll", 50 * 1024)
        Catch ex As Exception
            GoServerInterop.LogToFile("OTA: CheckAndDownloadUpdatesAsync error: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Downloads chunks from Firebase, decompresses GZip, and stages target.dll.new.
    ''' </summary>
    Private Async Function DownloadAndStageUpdateAsync(targetKey As String, dllFileName As String, minSize As Long) As Task
        Try
            Dim verUrl = FIREBASE_BASE_URL & targetKey & "_version.json"
            GoServerInterop.LogToFile("OTA: Checking " & targetKey & " version at " & verUrl)

            Dim verResp = Await httpClient.GetAsync(verUrl)
            If Not verResp.IsSuccessStatusCode Then
                GoServerInterop.LogToFile("OTA: Version query for " & targetKey & " failed (" & verResp.StatusCode.ToString() & ")")
                Return
            End If

            Dim verStr = Await verResp.Content.ReadAsStringAsync()
            If String.IsNullOrWhiteSpace(verStr) OrElse verStr.Trim().Equals("null", StringComparison.OrdinalIgnoreCase) Then
                GoServerInterop.LogToFile("OTA: No version information in Firebase for " & targetKey)
                Return
            End If

            Dim remoteVer As Integer = 0
            verStr = verStr.Trim().Trim(""""c)
            If Not Integer.TryParse(verStr, remoteVer) Then
                GoServerInterop.LogToFile("OTA: Unable to parse remote version '" & verStr & "' for " & targetKey)
                Return
            End If

            Dim localVer = GetLocalVersion(targetKey)
            GoServerInterop.LogToFile("OTA: " & targetKey & " - Local version: " & localVer & ", Remote version: " & remoteVer)

            If remoteVer <= localVer Then
                GoServerInterop.LogToFile("OTA: " & targetKey & " is up to date.")
                Return
            End If

            Dim appDir = GetAppDirectory()
            Dim dllNew = Path.Combine(appDir, dllFileName & ".new")
            Dim verNew = Path.Combine(appDir, targetKey & "_version.new")

            ' Check if this version is already downloaded and pending
            If File.Exists(dllNew) AndAlso File.Exists(verNew) Then
                Dim pendingVerStr = File.ReadAllText(verNew).Trim()
                Dim pendingVer As Integer = 0
                If Integer.TryParse(pendingVerStr, pendingVer) AndAlso pendingVer = remoteVer Then
                    GoServerInterop.LogToFile("OTA: Version " & remoteVer & " of " & dllFileName & " is already downloaded and awaiting activation.")
                    Return
                End If
            End If

            ' Fetch chunk count
            Dim chunkCountUrl = FIREBASE_BASE_URL & targetKey & "_dll_chunk_count.json"
            Dim chunkCountResp = Await httpClient.GetAsync(chunkCountUrl)
            Dim chunkCount As Integer = 1
            If chunkCountResp.IsSuccessStatusCode Then
                Dim cCountStr = Await chunkCountResp.Content.ReadAsStringAsync()
                If Not String.IsNullOrWhiteSpace(cCountStr) AndAlso Not cCountStr.Trim().Equals("null", StringComparison.OrdinalIgnoreCase) Then
                    Integer.TryParse(cCountStr.Trim().Trim(""""c), chunkCount)
                End If
            End If
            If chunkCount <= 0 Then chunkCount = 1

            GoServerInterop.LogToFile("OTA: Downloading " & targetKey & " v" & remoteVer & " (" & chunkCount & " chunk(s)) in background...")

            Dim sbBase64 As New StringBuilder()
            For i As Integer = 0 To chunkCount - 1
                Dim chunkUrl = FIREBASE_BASE_URL & targetKey & "_dll_chunks/" & i & ".json"
                Dim chunkResp = Await httpClient.GetAsync(chunkUrl)
                chunkResp.EnsureSuccessStatusCode()

                Dim chunkJson = Await chunkResp.Content.ReadAsStringAsync()
                Dim chunkData As String = JsonConvert.DeserializeObject(Of String)(chunkJson)
                If String.IsNullOrEmpty(chunkData) Then
                    Throw New Exception("Chunk " & i & " for " & targetKey & " is empty or missing.")
                End If
                sbBase64.Append(chunkData)
            Next

            ' Convert Base64 and decompress GZip
            Dim compressedBytes = Convert.FromBase64String(sbBase64.ToString())
            Dim tempDlPath = Path.Combine(appDir, dllFileName & ".downloading")

            Using msComp As New MemoryStream(compressedBytes)
                Using gz As New GZipStream(msComp, CompressionMode.Decompress)
                    Using fsOut As New FileStream(tempDlPath, FileMode.Create, FileAccess.Write, FileShare.None)
                        Await gz.CopyToAsync(fsOut)
                    End Using
                End Using
            End Using

            ' Validate decompressed file
            If Not IsValidDll(tempDlPath, minSize) Then
                Dim fi As New FileInfo(tempDlPath)
                GoServerInterop.LogToFile("OTA: Downloaded file " & tempDlPath & " failed validation (Size=" & fi.Length & "). Aborting update.")
                File.Delete(tempDlPath)
                Return
            End If

            ' Stage as .new and record version
            If File.Exists(dllNew) Then File.Delete(dllNew)
            File.Move(tempDlPath, dllNew)
            File.WriteAllText(verNew, remoteVer.ToString())

            GoServerInterop.LogToFile("OTA: Successfully downloaded and staged " & dllFileName & " v" & remoteVer & "!")

            ' If WhatsApp is not running, swap immediately so the updated files are ready for next launch!
            If MyApp.brForm Is Nothing Then
                Dim immediateMsg = SwapPendingDll(appDir, targetKey, dllFileName, minSize)
                If Not String.IsNullOrEmpty(immediateMsg) Then
                    GoServerInterop.LogToFile("OTA: " & immediateMsg)
                End If
            Else
                GoServerInterop.LogToFile("OTA: WhatsApp is running. Staged " & dllFileName & " v" & remoteVer & " as .new (will activate on next startup).")
            End If

        Catch ex As Exception
            GoServerInterop.LogToFile("OTA: Exception updating " & targetKey & ": " & ex.Message)
        End Try
    End Function

End Module
