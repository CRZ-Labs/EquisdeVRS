Public Class ScreenLocker
    Dim SerialKey As String = Nothing
    Dim SolveSerial As String = Nothing
    Dim MaxTime As Integer = 120000
    Dim MaxTimeAction As String = Nothing

    Private Sub ScreenLocker_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Try
                Dim Lineas = IO.File.ReadLines(Starter.DIRCommons & "\[SL]ConfigFile.ini")
                SerialKey = Lineas(1).Split(">"c)(1).Trim()
                SolveSerial = Lineas(2).Split(">"c)(1).Trim()
                MaxTime = Lineas(3).Split(">"c)(1).Trim()
                MaxTimeAction = Lineas(4).Split(">"c)(1).Trim()
                TCPCMDReader.DiagnosticPosting("ScreenLocker Iniciado! (" & "SerialKey:" & SerialKey & " >SolveSerial:" & SolveSerial & " >MaxTime:" & MaxTimer.ToString & " >MaxTimeAction:" & MaxTimeAction & ")", "TCP.DGN.ScreenLocker", True, False)
            Catch ex As Exception
                Console.WriteLine("[ScreenLocker@ScreenLocker_Load(SetValues)]Error: " & ex.Message)
                End
            End Try
            LockScreen()
        Catch ex As Exception
            Console.WriteLine("[ScreenLocker@ScreenLocker_Load]Error: " & ex.Message)
        End Try
    End Sub

    Sub LockScreen()
        Try
            Label4.Text = SolveSerial
            MaxTimer.Interval = Val(MaxTime)
            MaxTimer.Start()
            MaxTimer.Enabled = True
            TCPCMDReader.DiagnosticPosting("Pantalla bloqueada!", "TCP.DGN.ScreenLocker", True, False)
        Catch ex As Exception
            Console.WriteLine("[ScreenLocker@LockScreen]Error: " & ex.Message)
        End Try
    End Sub

    Sub UnLock()
        Try
            TCPCMDReader.DiagnosticPosting("Pantalla desbloqueada + ScreenLocker Cerrado + dependencias!", "TCP.DGN.ScreenLocker", True, False)
            End
        Catch ex As Exception
            Console.WriteLine("[ScreenLocker@UnLock]Error: " & ex.Message)
        End Try
    End Sub

    Sub MaxTimeResearched()
        Try
            If MaxTimeAction = "BSODForce" Then
                'xD
            ElseIf MaxTimeAction = "UnLock" Then
                UnLock()
            End If
            TCPCMDReader.DiagnosticPosting("Tiempo maximo alcanzado! (" & MaxTimeAction & ")", "TCP.DGN.ScreenLocker", True, False)
        Catch ex As Exception
            Console.WriteLine("[ScreenLocker@MaxTimeResearched]Error: " & ex.Message)
        End Try
    End Sub

    Dim Counter As Integer = 0
    Private Sub MaxTimer_Tick(sender As Object, e As EventArgs) Handles MaxTimer.Tick
        If Counter < 100 Then
            Counter = Counter + 1
            ProgressBar1.Value = Counter
        Else
            Counter = 0
            MaxTimer.Stop()
            MaxTimer.Enabled = False
            ProgressBar1.Value = 100
            MaxTimeResearched()
        End If
    End Sub

    Private Sub Try_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text = SerialKey Then
            MsgBox("Correct SerialKey!", MsgBoxStyle.Information, "ScreenLocker")
            UnLock()
            TCPCMDReader.DiagnosticPosting("Llave correcta! (" & TextBox1.Text & ")", "TCP.DGN.ScreenLocker", True, False)
        Else
            TCPCMDReader.DiagnosticPosting("Llave incorrecta (" & TextBox1.Text & ")", "TCP.DGN.ScreenLocker", True, False)
            TextBox1.Clear()
            MsgBox("Incorrect SerialKey", MsgBoxStyle.Critical, "ScreenLocker")
            TextBox1.Focus()
        End If
    End Sub
End Class