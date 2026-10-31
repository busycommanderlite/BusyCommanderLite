Imports System.Diagnostics
Imports System.IO
Imports System.Runtime.InteropServices

Public Class GoServerInterop
    <DllImport("server.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub StartServer_Go()
    End Sub

    <DllImport("server.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Shared Sub StopServer_Go()
    End Sub

    Public Shared Function IsLoggingEnabled() As Boolean
        Try
            ' 1. Check for file trigger: LOG or LOG.txt inside WhatsApp folder
            If File.Exists(Log_File_Trigger_Path) OrElse File.Exists(Log_File_Trigger_Txt_Path) Then
                Return True
            End If

            ' 2. Check for DEBUG or DEBUG.txt file trigger
            If File.Exists(Debug_File_Path) OrElse File.Exists(Debug_File_Txt_Path) Then
                Return True
            End If

            ' 3. Check config.json toggle (EnableLogging = yes)
            If MyApp.KeyDict IsNot Nothing AndAlso MyApp.KeyDict.ContainsKey("EnableLogging") Then
                Return MyApp.KeyDict("EnableLogging").Equals("yes", StringComparison.OrdinalIgnoreCase)
            End If
        Catch
        End Try
        Return False
    End Function

    Public Shared Sub LogToFile(msg As String)
        Try
            If Not IsLoggingEnabled() Then Return

            Dim dir = Path.GetDirectoryName(Debug_Log_Path)
            If Not Directory.Exists(dir) Then Directory.CreateDirectory(dir)
            File.AppendAllText(Debug_Log_Path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " - " & msg & Environment.NewLine)
        Catch
        End Try
    End Sub

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function LoadLibrary(ByVal lpFileName As String) As IntPtr
    End Function

    Public Shared Sub StartServer()
        Try
            Dim myDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            Dim dllPath = Path.Combine(myDir, "server.dll")
            LogToFile("Loading DLL from: " & dllPath)
            
            Dim hModule As IntPtr = LoadLibrary(dllPath)
            If hModule = IntPtr.Zero Then
                LogToFile("LoadLibrary failed. Error code: " & Marshal.GetLastWin32Error())
            End If

            LogToFile("Calling StartServer from DLL...")
            StartServer_Go()
            LogToFile("StartServer called successfully.")
        Catch ex As Exception
            LogToFile("Error starting Go DLL: " & ex.Message)
        End Try
    End Sub

    Public Shared Sub StopServer()
        Try
            LogToFile("Calling StopServer from DLL...")
            StopServer_Go()
        Catch ex As Exception
            LogToFile("Error stopping Go DLL: " & ex.Message)
        End Try
    End Sub
End Class
