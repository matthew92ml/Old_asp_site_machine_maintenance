Imports Npgsql
Imports System.Net.Mail

Public Class EmailClass
    Public Shared Function SendEmail(ByVal email As String, ByVal emailType As String, ByVal emailLang As String, ByVal connectionString As String, Optional pConnector As PostgreConnector = Nothing, Optional parms As List(Of String) = Nothing) As Boolean
        Dim booDispose As Boolean = False

        If pConnector Is Nothing Then
            pConnector = New PostgreConnector(connectionString)
            booDispose = True
        End If

        Try
            If pConnector.Status Then
                Dim sqlString As String = "SELECT * FROM email " _
                                        & "WHERE tipo = '" & emailType & "' " _
                                        & "AND lingua = '" & emailLang & "'"

                If pConnector.AddReader("email", sqlString) Then
                    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("email")

                    Dim mail As New MailMessage
                    mail.From = New MailAddress(sqlReader("smtpuser").ToString)
                    For Each emailItem As String In email
                        mail.To.Add(New MailAddress(emailItem.ToLower))
                    Next
                    mail.Subject = sqlReader("oggetto").ToString
                    Dim strBody As String = sqlReader("corpo").ToString
                    If strBody <> String.Empty Then
                        If parms IsNot Nothing Then
                            Dim i As Integer = 0
                            For Each parm As String In parms
                                i += 1

                                strBody = strBody.Replace("{p" & i.ToString & "}", parm)
                            Next
                        End If
                    Else
                        If parms IsNot Nothing Then
                            For Each parm As String In parms
                                strBody &= parm
                            Next
                        End If
                    End If
                    mail.Body = sqlReader("template").ToString.Replace("{body_email}", strBody)
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
                    mail.IsBodyHtml = True

                    Dim smtp As New SmtpClient(sqlReader("smtpserver").ToString)
                    smtp.Credentials = New System.Net.NetworkCredential(sqlReader("smtpuser").ToString, sqlReader("smtppassword").ToString)
                    smtp.Send(mail)

                    pConnector.DelReader("email")
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            If booDispose Then pConnector.Dispose()
        End Try

        Return True
    End Function
End Class
