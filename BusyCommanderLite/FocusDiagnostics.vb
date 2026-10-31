Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class FocusDiagnostics

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetForegroundWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetActiveWindow() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetFocus() As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetAncestor(hwnd As IntPtr, gaFlags As UInteger) As IntPtr
    End Function

    <StructLayout(LayoutKind.Sequential)>
    Public Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <DllImport("user32.dll")>
    Public Shared Function GetWindowRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetWindowThreadProcessId(hwnd As IntPtr, ByRef lpdwProcessId As UInteger) As UInteger
    End Function

    <DllImport("kernel32.dll")>
    Public Shared Function GetCurrentThreadId() As UInteger
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetWindowText(hWnd As IntPtr, lpString As StringBuilder, nMaxCount As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Public Shared Function GetClassName(hWnd As IntPtr, lpClassName As StringBuilder, nMaxCount As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Public Shared Function IsWindowEnabled(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function IsWindowVisible(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function IsWindow(hWnd As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function PeekMessage(ByRef lpMsg As MSG, hWnd As IntPtr, wMsgFilterMin As UInteger, wMsgFilterMax As UInteger, wRemoveMsg As UInteger) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function EnableWindow(hWnd As IntPtr, bEnable As Boolean) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SetFocus(hWnd As IntPtr) As IntPtr
    End Function

    Public Const VK_RETURN As Integer = &HD
    Public Const VK_ESCAPE As Integer = &H1B

    <DllImport("user32.dll")>
    Public Shared Function GetAsyncKeyState(vKey As Integer) As Short
    End Function

    Public Shared Sub WaitKeyRelease(vKey As Integer, Optional maxWaitMs As Integer = 500)
        Dim sw = Stopwatch.StartNew()
        While (GetAsyncKeyState(vKey) And &H8000) <> 0 AndAlso sw.ElapsedMilliseconds < maxWaitMs
            System.Threading.Thread.Sleep(10)
        End While
    End Sub

    Public Delegate Function EnumChildProc(hwnd As IntPtr, lParam As IntPtr) As Boolean

    <DllImport("user32.dll")>
    Public Shared Function EnumChildWindows(hWndParent As IntPtr, lpEnumFunc As EnumChildProc, lParam As IntPtr) As Boolean
    End Function

    Public Shared Function FindChildWindowsByClass(parentHwnd As IntPtr, className As String) As List(Of IntPtr)
        Dim list As New List(Of IntPtr)()
        If parentHwnd = IntPtr.Zero OrElse Not IsWindow(parentHwnd) Then Return list

        EnumChildWindows(parentHwnd, Function(childHwnd, lParam)
                                         Dim sb As New StringBuilder(256)
                                         GetClassName(childHwnd, sb, 256)
                                         If sb.ToString().Equals(className, StringComparison.OrdinalIgnoreCase) Then
                                             list.Add(childHwnd)
                                         End If
                                         Return True
                                     End Function, IntPtr.Zero)
        Return list
    End Function

    Public Const WM_KEYFIRST As UInteger = &H100
    Public Const WM_KEYLAST As UInteger = &H109
    Public Const PM_REMOVE As UInteger = &H1

    <StructLayout(LayoutKind.Sequential)>
    Public Structure MSG
        Public hwnd As IntPtr
        Public message As UInteger
        Public wParam As IntPtr
        Public lParam As IntPtr
        Public time As UInteger
        Public pt_x As Integer
        Public pt_y As Integer
    End Structure

    Public Shared Function PurgeKeyboardQueue() As Integer
        Dim msg As MSG
        Dim count As Integer = 0
        While PeekMessage(msg, IntPtr.Zero, WM_KEYFIRST, WM_KEYLAST, PM_REMOVE)
            count += 1
            Log(String.Format("Purged queued key message #{0}: msg=0x{1:X4}, wParam=0x{2:X}, hwnd={3}",
                              count, msg.message, msg.wParam.ToInt64(), GetWindowInfo(msg.hwnd)))
        End While
        Return count
    End Function

    Public Const GA_PARENT As UInteger = 1
    Public Const GA_ROOT As UInteger = 2
    Public Const GA_ROOTOWNER As UInteger = 3

    Public Shared Function GetWindowInfo(hwnd As IntPtr) As String
        If hwnd = IntPtr.Zero Then Return "IntPtr.Zero"
        If Not IsWindow(hwnd) Then Return String.Format("0x{0:X8} [Invalid HWND]", hwnd.ToInt64())

        Dim sbText As New StringBuilder(256)
        GetWindowText(hwnd, sbText, 256)
        Dim sbClass As New StringBuilder(256)
        GetClassName(hwnd, sbClass, 256)

        Dim procId As UInteger = 0
        Dim threadId As UInteger = GetWindowThreadProcessId(hwnd, procId)
        Dim enabled As Boolean = IsWindowEnabled(hwnd)
        Dim visible As Boolean = IsWindowVisible(hwnd)

        Return String.Format("0x{0:X8} [Class='{1}', Title='{2}', Enabled={3}, Vis={4}, TID={5}, PID={6}]",
                             hwnd.ToInt64(), sbClass.ToString(), sbText.ToString(), enabled, visible, threadId, procId)
    End Function

    Public Shared Sub Log(msg As String)
        GoServerInterop.LogToFile("[FI-DIAG] " & msg)
    End Sub

    Public Shared Sub LogSnapshot(context As String, Optional targetHwnd As IntPtr = Nothing)
        Try
            Dim curThread = GetCurrentThreadId()
            Dim curProc = CUInt(Process.GetCurrentProcess().Id)
            Dim fg = GetForegroundWindow()
            Dim act = GetActiveWindow()
            Dim foc = GetFocus()

            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("=== SNAPSHOT: {0} ===", context))
            sb.AppendLine(String.Format("  CurContext: TID={0}, PID={1}", curThread, curProc))
            sb.AppendLine(String.Format("  Foreground: {0}", GetWindowInfo(fg)))
            sb.AppendLine(String.Format("  ActiveWnd:  {0}", GetWindowInfo(act)))
            sb.AppendLine(String.Format("  FocusWnd:   {0}", GetWindowInfo(foc)))

            If targetHwnd <> IntPtr.Zero Then
                sb.AppendLine(String.Format("  TargetWnd:  {0}", GetWindowInfo(targetHwnd)))
            End If

            If fg <> IntPtr.Zero Then
                Dim parent = GetAncestor(fg, GA_PARENT)
                Dim rootOwner = GetAncestor(fg, GA_ROOTOWNER)
                If parent <> IntPtr.Zero AndAlso parent <> fg Then
                    sb.AppendLine(String.Format("  FG.Parent:  {0}", GetWindowInfo(parent)))
                End If
                If rootOwner <> IntPtr.Zero AndAlso rootOwner <> fg AndAlso rootOwner <> parent Then
                    sb.AppendLine(String.Format("  FG.RootOwn: {0}", GetWindowInfo(rootOwner)))
                End If
            End If

            Log(sb.ToString().TrimEnd())
        Catch ex As Exception
            Log("LogSnapshot Error: " & ex.Message)
        End Try
    End Sub

End Class

Public Class DialogFocusGuard
    Implements IDisposable

    Private disabledHwnds As New List(Of IntPtr)()
    Private restoreTargetHwnd As IntPtr = IntPtr.Zero
    Private ownerWrapper As IWin32Window

    Public ReadOnly Property Owner As IWin32Window
        Get
            Return ownerWrapper
        End Get
    End Property

    Public Sub New()
        Try
            ' 1. Purge any queued keyboard messages (e.g. premature Enter pressed during VBScript execution)
            Dim purged = FocusDiagnostics.PurgeKeyboardQueue()
            If purged > 0 Then
                FocusDiagnostics.Log(String.Format("DialogFocusGuard: Purged {0} stale keyboard message(s) from queue", purged))
            End If

            ' 2. Resolve owner window
            ownerWrapper = MyApp.GetActiveWindowOwner()
            Dim ownerHwnd = If(ownerWrapper IsNot Nothing, ownerWrapper.Handle, IntPtr.Zero)

            ' 3. Identify focused child in Busy (if any) and disable it during the modal dialog
            Dim foc = FocusDiagnostics.GetFocus()
            If foc <> IntPtr.Zero AndAlso foc <> ownerHwnd AndAlso FocusDiagnostics.IsWindow(foc) Then
                restoreTargetHwnd = foc
                If FocusDiagnostics.IsWindowEnabled(foc) Then
                    FocusDiagnostics.EnableWindow(foc, False)
                    disabledHwnds.Add(foc)
                    FocusDiagnostics.Log(String.Format("DialogFocusGuard: Disabled focused child control {0}", FocusDiagnostics.GetWindowInfo(foc)))
                End If
            End If

            ' 4. Explicitly guard Busy's report grid (ListView20WndClass) under owner window
            ' even if focus was on another control (e.g. checkbox or caption)
            If ownerHwnd <> IntPtr.Zero Then
                Dim grids = FocusDiagnostics.FindChildWindowsByClass(ownerHwnd, "ListView20WndClass")
                For Each gridHwnd In grids
                    If Not disabledHwnds.Contains(gridHwnd) AndAlso FocusDiagnostics.IsWindow(gridHwnd) Then
                        If restoreTargetHwnd = IntPtr.Zero Then
                            restoreTargetHwnd = gridHwnd
                        End If
                        If FocusDiagnostics.IsWindowEnabled(gridHwnd) Then
                            FocusDiagnostics.EnableWindow(gridHwnd, False)
                            disabledHwnds.Add(gridHwnd)
                            FocusDiagnostics.Log(String.Format("DialogFocusGuard: Explicitly disabled grid control {0}", FocusDiagnostics.GetWindowInfo(gridHwnd)))
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            FocusDiagnostics.Log("DialogFocusGuard Init Exception: " & ex.Message)
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            ' 1. Wait until user physically releases Enter and Escape so keystrokes cannot leak
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_RETURN, 500)
            FocusDiagnostics.WaitKeyRelease(FocusDiagnostics.VK_ESCAPE, 500)

            ' 2. Purge any stray key messages that may have arrived while dialog was closing
            Dim purged = FocusDiagnostics.PurgeKeyboardQueue()
            If purged > 0 Then
                FocusDiagnostics.Log(String.Format("DialogFocusGuard Dispose: Purged {0} trailing keyboard message(s)", purged))
            End If

            ' 3. Re-enable all child controls that were disabled
            For Each h In disabledHwnds
                If h <> IntPtr.Zero AndAlso FocusDiagnostics.IsWindow(h) Then
                    FocusDiagnostics.EnableWindow(h, True)
                    FocusDiagnostics.Log(String.Format("DialogFocusGuard: Re-enabled child control {0}", FocusDiagnostics.GetWindowInfo(h)))
                End If
            Next

            ' 4. Restore focus back to Busy
            MyApp.RestoreFocus(ownerWrapper, restoreTargetHwnd)

            ' 5. Final purge after restore focus to guarantee nothing leaked to the newly focused control
            Dim postPurged = FocusDiagnostics.PurgeKeyboardQueue()
            If postPurged > 0 Then
                FocusDiagnostics.Log(String.Format("DialogFocusGuard Dispose: Purged {0} post-restore keyboard message(s)", postPurged))
            End If
        Catch ex As Exception
            FocusDiagnostics.Log("DialogFocusGuard Dispose Exception: " & ex.Message)
        End Try
    End Sub
End Class
