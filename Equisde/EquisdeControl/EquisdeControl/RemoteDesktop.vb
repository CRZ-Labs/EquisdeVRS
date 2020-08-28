Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Runtime.Serialization.Formatters.Binary
Public Class RemoteDesktop
    Dim YO As TcpListener
    Dim REMOTO As TcpClient
    Dim RECIBE As Thread
    Dim NS As NetworkStream

    Dim RESOLUCIONX As Integer
    Dim RESOLUCIONY As Integer
    Dim POSICIONX As Integer
    Dim POSICIONY As Integer
    Dim ENVIO As Byte()

    Public Sub RECIBIR()
        Dim BF As New BinaryFormatter
        Try
            While True
                REMOTO = YO.AcceptTcpClient()
                NS = REMOTO.GetStream
                While REMOTO.Connected = True
                    PictureBoxREMOTO.Image = BF.Deserialize(NS)
                    RESOLUCIONX = PictureBoxREMOTO.Image.Width
                    RESOLUCIONY = PictureBoxREMOTO.Image.Height
                End While
            End While
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@RECIBIR]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PictureBoxREMOTO_Click(sender As System.Object, e As System.EventArgs) Handles PictureBoxREMOTO.Click
        Try
            POSICIONX = (Cursor.Position.X - Me.Location.X - 10) * RESOLUCIONX / PictureBoxREMOTO.Width
            POSICIONY = (Cursor.Position.Y - Me.Location.Y - 30) * RESOLUCIONY / PictureBoxREMOTO.Height
            Dim MENSAJE As String = "IZQUIERDO:" & POSICIONX & ":" & POSICIONY
            ENVIO = System.Text.Encoding.UTF7.GetBytes(MENSAJE)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@PictureBoxREMOTO_Click]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PictureBoxREMOTO_DoubleClick(sender As Object, e As System.EventArgs) Handles PictureBoxREMOTO.DoubleClick
        Try
            POSICIONX = (Cursor.Position.X - Me.Location.X - 10) * RESOLUCIONX / PictureBoxREMOTO.Width
            POSICIONY = (Cursor.Position.Y - Me.Location.Y - 30) * RESOLUCIONY / PictureBoxREMOTO.Height
            Dim MENSAJE As String = "DOBLE:" & POSICIONX & ":" & POSICIONY
            ENVIO = System.Text.Encoding.UTF7.GetBytes(MENSAJE)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@PictureBoxREMOTO_DoubleClick]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub ButtonTAREAS_Click(sender As System.Object, e As System.EventArgs) Handles ButtonTAREAS.Click
        Try
            Dim MENSAJE As String = "TAREAS:500:" & RESOLUCIONY
            ENVIO = System.Text.Encoding.UTF7.GetBytes(MENSAJE)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@ButtonTAREAS_Click]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        Dim BF As New BinaryFormatter
        Try
            NS = REMOTO.GetStream
            BF.Serialize(NS, ENVIO)
            ENVIO = Nothing
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@Timer1_Tick]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub RemoteDesktop_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Timer1.Interval = 2555
            Timer1.Start()
        Catch ex As Exception
            MsgBox("Error al Conectar: " & ex.Message)
        End Try
        Try
            YO = New TcpListener(IPAddress.Any, "21112")
            YO.Start()
            RECIBE = New Thread(AddressOf RECIBIR)
            RECIBE.Start()
        Catch ex As Exception
            MsgBox("Error al Conectar al Servidor: " & ex.Message)
        End Try
    End Sub

    Private Sub RemoteDesktop_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            NS.Dispose()
        Catch ex As Exception
        End Try
        Try
            YO.Stop()
            RECIBE.Abort()
        Catch ex As Exception
            Me.Close()
        End Try
    End Sub
End Class
