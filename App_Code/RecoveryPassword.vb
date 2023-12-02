Imports Npgsql

Public Class RecoveryPasswordClass
    Public Shared Function RequestCode(ByVal email As String, ByVal lang As String, ByVal connectionString As String) As Boolean
        Dim booResult As Boolean = False

        Dim sqlString As String = String.Empty
        Dim pConnector As New PostgreConnector(connectionString)

        Try
            '** Controllo email
            If email <> String.Empty Then
                '** Controllo se user code presente
                Dim userId As Long = 0L

                sqlString = "SELECT utentiweb.email AS email " _
                          & "FROM utentiweb " _
                          & "INNER JOIN datinuclei ON utentiweb.idnucleo = datinuclei.id " _
                          & "WHERE UPPER(utentiweb.email) = '" & FunctionsClass.CheckString(email).ToUpper & "'"

                If pConnector.AddReader("utentiweb", sqlString) Then
                    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("utentiweb")
                    Long.TryParse(sqlReader("id").ToString, userId)
                    pConnector.DelReader("utentiweb")

                    '** Creazione codice cambio password
                    Dim recoveryCode As String = FunctionsClass.EncryptValue(userId.ToString & Now.ToString("yyyyMMssHHmmss"))

                    sqlString = "UPDATE utentiweb " _
                              & "SET recoveycode = '" & recoveryCode & "' " _
                              & "WHERE id = '" & userId.ToString & "'"

                    If pConnector.ExecuteCommand(sqlString) Then
                        '** Invio email cambio password
                        booResult = EmailClass.SendEmail(email, "RPS", lang, String.Empty, pConnector, New List(Of String) From {HttpContext.GetGlobalResourceObject("pages", "recovery_password_link").ToString, recoveryCode})
                    End If
                End If
            End If
        Catch ex As Exception
            '...
        Finally
            pConnector.Dispose()
        End Try

        Return booResult
    End Function

    Public Shared Function ChangePassword(ByVal recoveryCode As String, ByVal password As String, ByVal confirm As String, ByVal lang As String, ByVal connectionString As String) As PasswordRecoveryData
        Dim datResponse As New PasswordRecoveryData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = PasswordRecoveryDataEnum.Null

        '** Controllo password
        If password = String.Empty Then
            datResponse.Message = PasswordRecoveryDataEnum.EmptyPassword
            Return datResponse
        End If
        If password.Length < 8 Then
            datResponse.Message = PasswordRecoveryDataEnum.WrongPasswordLength
            Return datResponse
        End If

        '** Controllo conferma password
        If confirm = String.Empty Then
            datResponse.Message = PasswordRecoveryDataEnum.EmptyConfirm
            Return datResponse
        End If
        If password <> confirm Then
            datResponse.Message = PasswordRecoveryDataEnum.WrongConfirm
            Return datResponse
        End If

        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                Dim sqlString As String = String.Empty

                '** Controllo keycode
                sqlString = "SELECT infonuclei.id " _
                          & "FROM utentiweb " _
                          & "INNER JOIN infonuclei ON utentiweb.codicetag = infonuclei.codicetag " _
                          & "WHERE utentiweb.recoverycode = '" & recoveryCode & "'"

                If pConnector.ExecuteScalar(sqlString) > 0 Then
                    '** Aggiornamento nuova password
                    sqlString = "UPDATE utentiweb " _
                              & "SET password = '" & FunctionsClass.EncryptValue(password) & "', " _
                              & "recoverycode = '' " _
                              & "WHERE recoverycode = '" & recoveryCode & "'"

                    If pConnector.ExecuteCommand(sqlString) Then
                        datResponse.RedirectLink = "~/login"
                        datResponse.Message = PasswordRecoveryDataEnum.PasswordChanged
                    Else
                        datResponse.Message = PasswordRecoveryDataEnum.ErrorUpdatePassword
                    End If
                Else
                    datResponse.Message = PasswordRecoveryDataEnum.WrongKeycode
                End If
            Else
                datResponse.Message = PasswordRecoveryDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "recovery_password_link")
            datResponse.Message = PasswordRecoveryDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Structure PasswordRecoveryData
        Public RedirectLink As String
        Public Message As PasswordRecoveryDataEnum
    End Structure

    Public Enum PasswordRecoveryDataEnum
        PasswordChanged
        ErrorUpdatePassword
        ErrorDataBase
        ErrorGeneric
        EmptyPassword
        EmptyConfirm
        WrongPasswordLength
        WrongConfirm
        WrongKeycode
        Null
    End Enum
End Class
