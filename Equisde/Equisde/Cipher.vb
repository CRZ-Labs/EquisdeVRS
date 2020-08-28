Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Security.Principal
Imports System.Security.AccessControl
Public Class Cipher
    Dim CryptographyKey As String = My.Settings.CryptographyKey 'llave por defecto (por razon obvia XD)
    Public Shared des As New TripleDESCryptoServiceProvider
    Public Shared hashmd5 As New MD5CryptoServiceProvider

    Private Sub Cipher_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

#Region "Cipher"

    Sub CreatePrivateKey()
        Try
            Dim obj As New Random()
            Dim Posible As String = "abcdefghijklmnñopqrstuvwxyzABCDEFGHIJKLMNÑOPQRSTUVWXYZ1234567890"
            Dim Longitud As Integer = Posible.Length
            Dim Letra As Char
            Dim LongitudNuevaCadena As Integer = 30
            Dim NuevaCadena As String = Nothing
            For i As Integer = 0 To LongitudNuevaCadena - 1
                Letra = Posible(obj.Next(Longitud))
                NuevaCadena += Letra.ToString
            Next
            CryptographyKey = NuevaCadena
            My.Settings.CryptographyKey = TextEncrypt(NuevaCadena)
            My.Settings.Save()
            My.Settings.Reload()
            Console.WriteLine("Llave criptografica creada: " & NuevaCadena)
            TCPCMDReader.DiagnosticPosting("Llave criptografica creada: " & NuevaCadena, "TCP.DGN.Cipher", True, True)
        Catch ex As Exception
            Console.WriteLine("[Cipher@CreatePrivateKey]Error: " & ex.Message)
        End Try
    End Sub

    Function TextEncrypt(ByVal Texto As String) As String
        If Trim(Texto) = "" Then
            TextEncrypt = ""
        Else
            des.Key = hashmd5.ComputeHash((New UnicodeEncoding).GetBytes(CryptographyKey))
            des.Mode = CipherMode.ECB
            Dim Encrypt As ICryptoTransform = des.CreateEncryptor
            Dim Buff() As Byte = UnicodeEncoding.UTF7.GetBytes(Texto)
            TextEncrypt = Convert.ToBase64String(Encrypt.TransformFinalBlock(Buff, 0, Buff.Length))
        End If
        Return TextEncrypt
    End Function
    Function TextDecrypt(ByVal Texto As String) As String
        If Trim(Texto) = "" Then
            TextDecrypt = ""
        Else
            des.Key = hashmd5.ComputeHash((New UnicodeEncoding).GetBytes(CryptographyKey))
            des.Mode = CipherMode.ECB
            Dim Decrypt As ICryptoTransform = des.CreateDecryptor
            Dim Buff() As Byte = UnicodeEncoding.UTF7.GetBytes(Texto)
            TextDecrypt = Convert.ToBase64String(Decrypt.TransformFinalBlock(Buff, 0, Buff.Length))
        End If
        Return TextDecrypt
    End Function

    Sub LockDirectory(ByVal Dir As String, ByVal Hide As Boolean)
        Try
            If Hide = True Then
                Dim atributo As System.IO.FileAttributes = FileAttributes.Normal
                File.SetAttributes(Dir, atributo)
            End If
            TCPCMDReader.DiagnosticPosting("Directorio ocultado (" & Dir & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@LockDirectory(HideFolder)]Error: " & ex.Message)
        End Try
        Try
            Dim Acceso As FileSystemSecurity = File.GetAccessControl(Dir)
            Dim Usuario As New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing)
            Acceso.AddAccessRule(New FileSystemAccessRule(Usuario, FileSystemRights.FullControl, AccessControlType.Deny))
            File.SetAccessControl(Dir, Acceso)
            TCPCMDReader.DiagnosticPosting("Directorio protegido (" & Dir & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@LockDirectory(AccessFolder)]Error: " & ex.Message)
        End Try
    End Sub
    Sub UnlockDirectory(ByVal Dir As String, ByVal Show As Boolean)
        Try
            If Show = True Then
                Dim atributo As System.IO.FileAttributes = FileAttributes.Normal
                File.SetAttributes(Dir, atributo)
            End If
            TCPCMDReader.DiagnosticPosting("Directorio mostrado (" & Dir & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@UnlockDirectory(ShowFolder)]Error: " & ex.Message)
        End Try
        Try
            Dim Acceso As FileSystemSecurity = File.GetAccessControl(Dir)
            Dim Usuario As New SecurityIdentifier(WellKnownSidType.WorldSid, Nothing)
            Acceso.RemoveAccessRule(New FileSystemAccessRule(Usuario, FileSystemRights.FullControl, AccessControlType.Deny))
            File.SetAccessControl(Dir, Acceso)
            TCPCMDReader.DiagnosticPosting("Directorio desprotegido (" & Dir & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@UnlockDirectory(AccessFolder)]Error: " & ex.Message)
        End Try
    End Sub

    Sub CallFileEncrypt(ByVal FileIN As String, ByVal FileOUT As String)
        Try
            Dim Buffer As Byte()
            Using fs As New FileStream(FileIN, FileMode.Open, FileAccess.Read)
                Using ms As New MemoryStream()
                    FileEncrypt(fs, ms, CryptographyKey)
                    Buffer = ms.ToArray()
                End Using
            End Using
            Using fs As New FileStream(FileOUT, FileMode.CreateNew, FileAccess.Write)
                fs.Write(Buffer, 0, Buffer.Length)
            End Using
            TCPCMDReader.DiagnosticPosting("Cifrado de archivo (" & FileIN & ">" & FileOUT & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@CallFileEncrypt]Error: " & ex.Message)
        End Try
    End Sub
    Sub CallFileDecrypt(ByVal FileIN As String, ByVal FileOUT As String)
        Try
            Dim Buffer As Byte() = Nothing
            Using fs As New FileStream(FileIN, FileMode.Open, FileAccess.Read)
                Using ms As New MemoryStream()
                    FileDecrypt(fs, ms, CryptographyKey)
                    Buffer = ms.ToArray()
                End Using
            End Using
            Using fs As New FileStream(FileOUT, FileMode.CreateNew, FileAccess.Write)
                fs.Write(Buffer, 0, Buffer.Length)
            End Using
            TCPCMDReader.DiagnosticPosting("Decifrado de archivo (" & FileIN & ">" & FileOUT & ")", "TCP.DGN.Cipher", True, False)
        Catch ex As Exception
            Console.WriteLine("[Cipher@CallFileDecrypt]Error: " & ex.Message)
        End Try
    End Sub
    Function FileEncrypt(inStream As Stream, outStream As Stream, Password As String)
        Try
            Dim aes As New AesCryptoServiceProvider()
            aes.Mode = CipherMode.CFB
            aes.Key() = GetDeriveBytes(Password, 32)
            aes.IV = GetDeriveBytes(Password, 16)
            Dim Stream As New CryptoStream(outStream, aes.CreateEncryptor(), CryptoStreamMode.Write)
            Dim Length As Integer = 2048
            Dim Buffer As Byte() = New Byte(Length - 1) {}
            Try
                Dim i As Integer = inStream.Read(Buffer, 0, Length)
                Do While (i > 0)
                    Stream.Write(Buffer, 0, i)
                    i = inStream.Read(Buffer, 0, Length)
                Loop
            Finally
                Stream.FlushFinalBlock()
                aes.Dispose()
                Buffer = Nothing
            End Try
        Catch ex As Exception
            Console.WriteLine("[Cipher@FileEncrypt]Error: " & ex.Message)
        End Try
    End Function
    Function FileDecrypt(inStream As Stream, outStream As Stream, Password As String)
        Try
            Dim aes As New AesCryptoServiceProvider()
            aes.Mode = CipherMode.CFB
            aes.Key() = GetDeriveBytes(Password, 32)
            aes.IV = GetDeriveBytes(Password, 16)
            Dim stream As New CryptoStream(inStream, aes.CreateDecryptor(), CryptoStreamMode.Read)
            Dim length As Integer = 2048
            Dim buffer As Byte() = New Byte(length - 1) {}
            Try
                Dim i As Integer = stream.Read(buffer, 0, length)
                Do While (i > 0)
                    outStream.Write(buffer, 0, i)
                    i = stream.Read(buffer, 0, length)
                Loop
            Finally
                aes.Dispose()
                buffer = Nothing
            End Try
        Catch ex As Exception
            Console.WriteLine("[Cipher@FileEncrypt]Error: " & ex.Message)
        End Try
    End Function
    Function GetDeriveBytes(ByVal Password As String, ByVal Size As Integer) As Byte()
        Try
            If ((String.IsNullOrWhiteSpace(Password)) OrElse (Password.Length < 8)) Then
                Console.WriteLine("[Cipher@GetDeriveBytes]La llave criptografia no debe tener espacios y debe ser mayor a ocho caracteres")
            End If
            If ((Size < 1) OrElse (Size > 128)) Then
                Console.WriteLine("[Cipher@GetDeriveBytes]El tamaño debe estar entre 1 y 128")
            End If
            Dim pwd As Byte() = UTF8Encoding.UTF8.GetBytes(Password)
            Dim AzUcAr As Byte() = UTF8Encoding.UTF8.GetBytes(Convert.ToBase64String(pwd))
            Using bytes As New Rfc2898DeriveBytes(pwd, AzUcAr, 1000)
                Return bytes.GetBytes(Size)
            End Using
        Catch ex As Exception
            Console.WriteLine("[Cipher@GetDeriveBytes]Error: " & ex.Message)
        End Try
    End Function
#End Region
End Class