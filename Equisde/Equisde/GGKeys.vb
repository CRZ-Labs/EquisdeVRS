Public Class GGKeys
    Dim StringTextMemory As New ArrayList
    Dim Programa As String = Nothing

    Private Sub GGKeys_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Sub GetStringText(ByVal Text As String, ByVal Program As String)
        Try
            Programa = Program
            For Each c As Char In Text
                StringTextMemory.Add(c)
            Next
            SendThatKeys()
        Catch ex As Exception
            Console.WriteLine("[GetStringText@GGKeys]Error: " & ex.Message)
        End Try
    End Sub

    Sub SendThatKeys()
        Try
            If Programa = "None" Then
            Else
                Dim ProcID As Integer
                ProcID = Shell(Programa, AppWinStyle.NormalFocus)
                AppActivate(ProcID)
            End If
            For Each item As String In StringTextMemory
                My.Computer.Keyboard.SendKeys(item, True)
            Next
            Me.Close()
        Catch ex As Exception
            Console.WriteLine("[SendThatKeys@GGKeys]Error: " & ex.Message)
        End Try
    End Sub
End Class