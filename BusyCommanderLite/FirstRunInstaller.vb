Imports System.Configuration.Install
Imports System.IO

Public Class FirstRunInstaller
    Inherits Installer

    Public Overrides Sub Install(stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)

        Try
            Dim regForm = New RegistrationForm
            regForm.BringToFront()
            regForm.TopMost = True
            Dim result As DialogResult = regForm.ShowDialog()

            If result = DialogResult.Cancel Then
                Throw New InstallException("User cancelled registration.")
            ElseIf result = DialogResult.Abort Then
                Throw New InstallException("Installation Aborted.")
            End If
        Catch ex As Exception
            Try
                Dim installDir = "C:\BusyWin\Whatsapp"
                If Not Directory.Exists(installDir) Then
                    Directory.CreateDirectory(installDir)
                End If
                File.AppendAllText(Path.Combine(installDir, "install_error.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & " - " & ex.ToString() & vbCrLf & vbCrLf)
            Catch
            End Try
            Throw
        End Try
    End Sub

End Class