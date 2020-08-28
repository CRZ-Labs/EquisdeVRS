Imports System.Net
Imports System.Threading
Imports System.IO
Public Class HttpReport
    Dim SERVIDOR As New HttpListener
    Dim HEBRA As Thread
    Dim ARCHIVO As String = Starter.DIRCommons & "\"

    Private Sub HttpReport_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub HttpReport_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            SERVIDOR.Stop()
            HEBRA.Abort()
        Catch ex As Exception
        End Try
    End Sub

    Sub StartReporting()
        Try
            SERVIDOR.Start()
            SERVIDOR.Prefixes.Add("http://*:1337/")
            HEBRA = New Thread(AddressOf RESPONDE)
            HEBRA.Start()
            TCPCMDReader.DiagnosticPosting("Starting Reporting... (" & "http://*:1337" & ")", "TCP.DGN.HttpReport", True, False)
        Catch ex As Exception
            Console.WriteLine("[HttpReport@StartReporting]Error: " & ex.Message)
        End Try
    End Sub

    Sub StopReporting()
        Try
            SERVIDOR.Stop()
            HEBRA.Abort()
            TCPCMDReader.DiagnosticPosting("Stop Reporting", "TCP.DGN.HttpReport", True, False)
        Catch ex As Exception
            Console.WriteLine("[HttpReport@StopReporting]Error: " & ex.Message)
        End Try
    End Sub

    Public Sub RESPONDE()
        While True
            Try

                Dim CONTEXTO As HttpListenerContext = SERVIDOR.GetContext
                Dim FICHERO As String = Path.GetFileName(CONTEXTO.Request.RawUrl)
                Dim CAMINO = Path.Combine(ARCHIVO, FICHERO)
                Dim MENSAJE As Byte()
                If File.Exists(CAMINO) Then
                    CONTEXTO.Response.StatusCode = HttpStatusCode.OK
                    MENSAJE = File.ReadAllBytes(CAMINO)
                Else
                    CONTEXTO.Response.StatusCode = HttpStatusCode.NotFound
                    MENSAJE = File.ReadAllBytes(ARCHIVO + "EquisdeVRSWebReport.html")
                End If
                CONTEXTO.Response.ContentLength64 = MENSAJE.Length
                Dim MISTREAM As Stream = CONTEXTO.Response.OutputStream
                Using (MISTREAM)
                    MISTREAM.Write(MENSAJE, 0, MENSAJE.Length)
                End Using
                TCPCMDReader.DiagnosticPosting("Request Responded!", "TCP.DGN.HttpReport", True, False)
            Catch ex As Exception
                Console.WriteLine("[HttpReport@RESPONDE]Error: " & ex.Message)
            End Try
        End While
    End Sub

    Sub CreateWebReport()
        Try
            If My.Computer.FileSystem.FileExists(Starter.DIRCommons & "\EquisdeVRSWebReport.html") = True Then
                My.Computer.FileSystem.DeleteFile(Starter.DIRCommons & "\EquisdeVRSWebReport.html")
            End If
            'My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\EquisdeVRSWebReport.html", "<html><head><title>Equisde VRS Web Report</title></head><body><div align=" & """" & "center" & """" & ">This feature is not available</div></body></html>", False)
            My.Computer.FileSystem.WriteAllText(Starter.DIRCommons & "\EquisdeVRSWebReport.html", "Equide VRS Web Report" &
                                                vbCrLf & vbCrLf & vbCrLf &
                                                vbCrLf & "Status: " & "Running" &
                                                vbCrLf & "Start: " & Starter.StartedData &
                                                vbCrLf & vbCrLf & vbCrLf & vbCrLf & "Equisde VRS was created by CRZNetworks.", False)
            TCPCMDReader.DiagnosticPosting("HTML Information File created!", "TCP.DGN.HttpReport", True, False)
            StartReporting()
        Catch ex As Exception
            Console.WriteLine("[HttpReport@CreateWebReport]Error: " & ex.Message)
        End Try
    End Sub
End Class