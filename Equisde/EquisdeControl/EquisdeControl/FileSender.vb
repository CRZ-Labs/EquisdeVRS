Imports System.Threading
Imports System.Net.Sockets
Imports System.IO
Imports System.Net
Public Class FileSender

    Private Sub FileSender_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Sub ENVIA(ByVal DirFile As String)
        Me.Show()
        Dim TAMAÑOBUFFER As Integer = 1024
        Try
            Dim CLIENTE As New TcpClient(My.Settings.ServerIP, "21111")
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
            FS.Close()
            NS.Close()
            CLIENTE.Close()
        Catch ex As Exception
            Console.WriteLine("[FileSender@ENVIA]Error: " & ex.Message)
        End Try
        Me.Close()
    End Sub

    Sub RECIBE(ByVal FileName As String)
        Me.Show()
        SaveFileDialog1.FileName = FileName
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
                Dim ACEPTA As String = "Peticion para descargar (Recibir) un fichero remoto" & vbCrLf & "Fichero: " & FileName & vbCrLf & "¿Aceptar la descarga del archivo?"
                Dim TITULO As String = "FileSender"
                Dim BOTONES As MessageBoxButtons = MessageBoxButtons.YesNo
                Dim RESULTADO As DialogResult
                If SERVIDOR.Pending Then
                    CLIENTE = SERVIDOR.AcceptTcpClient
                    NS = CLIENTE.GetStream
                    RESULTADO = MessageBox.Show(ACEPTA, TITULO, BOTONES)
                    If RESULTADO = Windows.Forms.DialogResult.Yes Then
                        Dim FICHERORECIBIDO As String = Nothing
                        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                            FICHERORECIBIDO = SaveFileDialog1.FileName
                        End If
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
                        MsgBox("Descarga de Archivo remoto finalizado", MsgBoxStyle.Information, "FileSender")
                        FIN = 1
                    End If
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End While
        SERVIDOR.Stop()
        Me.Close()
    End Sub
    Private Shared Function AYUDAENLINEA(Of T)(ByRef OBJETIVO As T, VALOR As T)
        OBJETIVO = VALOR
        Return VALOR
    End Function
End Class