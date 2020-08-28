Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Public Class FileSender

    Private Sub FileSender_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Sub SendFile(ByVal DirFile As String)
        Me.Show()
        Dim ServerIP As String = Nothing
        Try
            Dim Lineas = IO.File.ReadLines(Starter.DIRCommons & "\DataBase.ini")
            ServerIP = Lineas(1).Split(">"c)(1).Trim()
            TCPCMDReader.DiagnosticPosting("Server IP: " & ServerIP, "TCP.DGN.FileSender", True, False)
        Catch ex As Exception
            Console.WriteLine("[FileSender@SendFile(GetDataBaseIP)]Error: " & ex.Message)
        End Try
        Dim TAMAÑOBUFFER As Integer = 1024
        Try
            Dim CLIENTE As New TcpClient(ServerIP, "21111")
            Dim NS As NetworkStream = CLIENTE.GetStream
            Dim FS As New FileStream(DirFile, FileMode.Open, FileAccess.Read)
            Dim PAQUETES As Integer = CInt(Math.Ceiling(CDbl(FS.Length) / CDbl(TAMAÑOBUFFER)))
            Dim LONGITUDTOTAL As Integer = CInt(FS.Length)
            Dim LONGITUDPAQUETEACTUAL As Integer = 0
            Dim CONTADOR As Integer = 0
            For I As Integer = 0 To PAQUETES - 1
                If LONGITUDTOTAL > TAMAÑOBUFFER Then
                    LONGITUDPAQUETEACTUAL = TAMAÑOBUFFER
                    LONGITUDTOTAL = LONGITUDTOTAL - LONGITUDPAQUETEACTUAL
                Else
                    LONGITUDPAQUETEACTUAL = LONGITUDTOTAL
                End If
                Dim ENVIARBUFFER As Byte() = New Byte(LONGITUDPAQUETEACTUAL - 1) {}
                FS.Read(ENVIARBUFFER, 0, LONGITUDPAQUETEACTUAL)
                NS.Write(ENVIARBUFFER, 0, CInt(ENVIARBUFFER.Length))
            Next
            TCPCMDReader.DiagnosticPosting("Fichero enviado: " & DirFile, "TCP.DGN.FileSender", True, False)
            FS.Close()
            NS.Close()
            CLIENTE.Close()
        Catch ex As Exception
            Console.WriteLine("[FileSender@SendFile]Error: " & ex.Message)
        End Try
        Me.Close()
    End Sub

    Sub ReceiveFile(ByVal FileName As String)
        Try
            Dim CLIENTE As TcpClient
            Dim TAMAÑOBUFFER As Integer = 1024
            Dim ARCHIVORECIBIDO As Byte() = New Byte(TAMAÑOBUFFER - 1) {}
            Dim BYTESRECIBIDOS As Integer
            Dim FIN As Integer = 0
            Dim SERVIDOR As New TcpListener(IPAddress.Any, "21111")
            SERVIDOR.Start()
            While FIN = 0
                Dim NS As NetworkStream = Nothing
                Try
                    If SERVIDOR.Pending Then
                        CLIENTE = SERVIDOR.AcceptTcpClient
                        NS = CLIENTE.GetStream
                        Dim FICHERORECIBIDO As String = Nothing
                        FICHERORECIBIDO = Starter.DIRCommons & "\" & FileName
                        If FICHERORECIBIDO <> String.Empty Then
                            Dim TOTALBYTESRECIBIDOS As Integer = 0
                            Dim FS As New FileStream(FICHERORECIBIDO, FileMode.OpenOrCreate, FileAccess.Write)
                            While (AYUDAENLINEA(BYTESRECIBIDOS, NS.Read(ARCHIVORECIBIDO, 0, ARCHIVORECIBIDO.Length))) > 0
                                FS.Write(ARCHIVORECIBIDO, 0, BYTESRECIBIDOS)
                                TOTALBYTESRECIBIDOS = TOTALBYTESRECIBIDOS + BYTESRECIBIDOS
                            End While
                            FS.Close()
                        End If
                        NS.Close()
                        CLIENTE.Close()
                        SERVIDOR.Stop()
                        FIN = 1
                    End If
                Catch ex As Exception
                    Console.WriteLine("[FileSender@Receive(Recibir)]Error: " & ex.Message)
                End Try
            End While
            SERVIDOR.Stop()
            Me.Close()
        Catch ex As Exception
            Console.WriteLine("[FileSender@Receive]Error: " & ex.Message)
        End Try
    End Sub

    Private Shared Function AYUDAENLINEA(Of T)(ByRef OBJETIVO As T, VALOR As T)
        OBJETIVO = VALOR
        Return VALOR
    End Function
End Class