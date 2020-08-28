Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Runtime.InteropServices
Public Class WebCamClient
    Dim YO As New TcpClient
    Dim NS As NetworkStream
    Dim INICIADO As Boolean = False

    Public Const WM_CAP As Short = &H400S
    Public Const WM_CAP_DLG_VIDEOFORMAT As Integer = WM_CAP + 41
    Public Const WM_CAP_DRIVER_CONNECT As Integer = WM_CAP + 10
    Public Const WM_CAP_DRIVER_DISCONNECT As Integer = WM_CAP + 11
    Public Const WM_CAP_EDIT_COPY As Integer = WM_CAP + 30
    Public Const WM_CAP_SEQUENCE As Integer = WM_CAP + 62
    Public Const WM_CAP_FILE_SAVEAS As Integer = WM_CAP + 23
    Public Const WM_CAP_SET_PREVIEW As Integer = WM_CAP + 50
    Public Const WM_CAP_SET_PREVIEWRATE As Integer = WM_CAP + 52
    Public Const WM_CAP_SET_SCALE As Integer = WM_CAP + 53
    Public Const WS_CHILD As Integer = &H40000000
    Public Const WS_VISIBLE As Integer = &H10000000
    Public Const SWP_NOMOVE As Short = &H2S
    Public Const SWP_NOSIZE As Short = 1
    Public Const SWP_NOZORDER As Short = &H4S
    Public Const HWND_BOTTOM As Short = 1
    Public Const WM_CAP_STOP As Integer = WM_CAP + 68
    Public iDevice As Integer = 0
    Public hHwnd As Integer
    Public Declare Function SendMessage Lib "user32" Alias "SendMessageA" _
        (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, _
        <MarshalAs(UnmanagedType.AsAny)> ByVal lParam As Object) As Integer
    Public Declare Function SetWindowPos Lib "user32" Alias "SetWindowPos" (ByVal hwnd As Integer, _
        ByVal hWndInsertAfter As Integer, ByVal x As Integer, ByVal y As Integer, _
        ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
    Public Declare Function DestroyWindow Lib "user32" (ByVal hndw As Integer) As Boolean
    Public Declare Function capCreateCaptureWindowA Lib "avicap32.dll" _
        (ByVal lpszWindowName As String, ByVal dwStyle As Integer, _
        ByVal x As Integer, ByVal y As Integer, ByVal nWidth As Integer, _
        ByVal nHeight As Short, ByVal hWndParent As Integer, _
        ByVal nID As Integer) As Integer
    Public Declare Function capGetDriverDescriptionA Lib "avicap32.dll" (ByVal wDriver As Short, _
        ByVal lpszName As String, ByVal cbName As Integer, ByVal lpszVer As String, _
        ByVal cbVer As Integer) As Boolean

    Public Sub OpenPreviewWindow()
        Try
            'INICIA LA WEBCAM
            hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 640, 480, PictureBoxVISOR.Handle.ToInt32, 0)
            Dim CONTADOR As Integer = 0
            For I = 1 To 10
                Dim CONECTADO As Integer = SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0)
                If CONECTADO = 1 Then
                    SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)
                    SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 66, 0)
                    SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)
                    SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, PictureBoxVISOR.Width, PictureBoxVISOR.Height, SWP_NOMOVE Or SWP_NOZORDER)
                    Exit For
                End If
                CONTADOR = I
                TCPCMDReader.DiagnosticPosting("WebCam Iniciada y lista", "TCP.DGN.VideoTCP", True, False)
            Next
            If CONTADOR = 10 Then
                DestroyWindow(hHwnd)
                Console.WriteLine("No se pudo iniciar la camara")
                TCPCMDReader.DiagnosticPosting("WebCam no iniciada", "TCP.DGN.VideoTCP", True, False)
            End If
            TCPCMDReader.DiagnosticPosting("WebCam (" & CONTADOR & ")", "TCP.DGN.VideoTCP", True, False)
        Catch ex As Exception
            Console.WriteLine("[WebCamServer@OpenPreviewWindow]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim BF As New BinaryFormatter
        'ENVIA LAS IMAGENES AL SERVIDOR
        Try
            SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)
            Dim DATOS = Clipboard.GetDataObject
            Dim IMAGEN As Image = CType(DATOS.GetData(GetType(Bitmap)), Image)
            Dim MS As New MemoryStream
            IMAGEN.Save(MS, Imaging.ImageFormat.Png)
            IMAGEN = Image.FromStream(MS)
            NS = YO.GetStream
            BF.Serialize(NS, IMAGEN)
        Catch ex As Exception
            Console.WriteLine("[WebCamServer@Timer1_Tick]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub WebCam_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Try
            NS.Dispose()
        Catch ex As Exception
        End Try
        Try
            YO.Close()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub WebCam_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            OpenPreviewWindow()
            INICIADO = True
        Catch ex As Exception
            Console.WriteLine("[WebCamServer@WebCam_Load(CallConnectCamera)]Error: " & ex.Message)
        End Try
        Dim ServerIP As String = Nothing
        Try
            Dim Lineas = IO.File.ReadLines(Starter.DIRCommons & "\DataBase.ini")
            ServerIP = Lineas(1).Split(">"c)(1).Trim()
        Catch ex As Exception
            Console.WriteLine("[WebCamServer@WebCam_Load(GetDataBaseIP)]Error: " & ex.Message)
        End Try
        Try
            YO.Connect(ServerIP, "21113")
        Catch ex As Exception
            Console.WriteLine("[WebCamServer@WebCam_Load(Connect)]Error: " & ex.Message)
        End Try
        Timer1.Interval = 3550
        Timer1.Start()
        Timer1.Enabled = True
        TCPCMDReader.DiagnosticPosting("VideoTCP Iniciado! (" & "Status: " & INICIADO & " >ServerIP:" & ServerIP & " >Port: 21113" & ")", "TCP.DGN.VideoTCP", True, False)
    End Sub
End Class