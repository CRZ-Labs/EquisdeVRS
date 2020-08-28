Imports System.Runtime.Serialization.Formatters.Binary
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports System.Net
Imports System.Runtime.InteropServices

Public Class WebCamServer
    Dim YO As TcpListener
    Dim REMOTO As TcpClient
    Dim RECIBE As Thread
    Dim NS As NetworkStream
    Dim IMAGEN As Image
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
        'INICIA LA CAMARA
        'hHwnd = capCreateCaptureWindowA(iDevice, WS_VISIBLE Or WS_CHILD, 0, 0, 320, 240, PictureBoxVISOR.Handle.ToInt32, 0)
        Dim CONTADOR As Integer = 0
        For I = 1 To 10
            Dim CONECTADO As Integer = SendMessage(hHwnd, WM_CAP_DRIVER_CONNECT, iDevice, 0)
            If CONECTADO = 1 Then
                SendMessage(hHwnd, WM_CAP_SET_SCALE, True, 0)
                SendMessage(hHwnd, WM_CAP_SET_PREVIEWRATE, 66, 0)
                SendMessage(hHwnd, WM_CAP_SET_PREVIEW, True, 0)
                'SetWindowPos(hHwnd, HWND_BOTTOM, 0, 0, PictureBoxVISOR.Width, PictureBoxVISOR.Height, SWP_NOMOVE Or SWP_NOZORDER)
                Exit For
            End If
            CONTADOR = I
        Next
        If CONTADOR = 10 Then
            DestroyWindow(hHwnd)
            MsgBox("Error: No se puede Iniciar la Camara", MsgBoxStyle.Critical, "Worcome Security")
        End If
    End Sub

    Public Sub RECIBIR()
        'ESPERANDO IMAGENES DEL CLIENTE
        Dim BF As New BinaryFormatter
        Try
            REMOTO = YO.AcceptTcpClient()
            NS = REMOTO.GetStream
            Try
                PictureBoxREMOTO.Image = BF.Deserialize(NS)
            Catch ex As Exception
                Console.WriteLine("[VideoClient@RECIBIR(DrawImage)]Error: " & ex.Message)
            End Try
        Catch ex As Exception
            Console.WriteLine("[VideoClient@RECIBIR]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim BF As New BinaryFormatter
        'ENIANDO IMAGENES AL CLIENTE
        Try
            SendMessage(hHwnd, WM_CAP_EDIT_COPY, 0, 0)
            Dim DATOS = Clipboard.GetDataObject
            IMAGEN = CType(DATOS.GetData(GetType(Bitmap)), Image)
            Dim MS As New MemoryStream
            IMAGEN.Save(MS, Imaging.ImageFormat.Jpeg)
            IMAGEN = Image.FromStream(MS)
            Try
                NS = REMOTO.GetStream
                BF.Serialize(NS, IMAGEN)
            Catch ex As Exception
                Console.WriteLine("[VideoClient@Timer1_Tick(SendImage)]Error: " & ex.Message)
            End Try
        Catch ex As Exception
            Console.WriteLine("[VideoClient@Timer1_Tick]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub WebCamServer_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            NS.Dispose()
        Catch ex As Exception
        End Try
        Try
            YO.Stop()
        Catch ex As Exception
        End Try
        Try
            RECIBE.Abort()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub WebCam_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            YO = New TcpListener(IPAddress.Any, "21113")
            YO.Start()
            RECIBE = New Thread(AddressOf RECIBIR)
            RECIBE.Start()
            'Timer1.Interval = 1000
            'Timer1.Start()
        Catch ex As Exception
            MsgBox("Error: Imposible Conectar" & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Conexion Video")
        End Try
    End Sub
End Class
