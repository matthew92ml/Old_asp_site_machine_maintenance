Imports Npgsql
Imports System.Text.RegularExpressions

Public Class SigninClass
    Public Shared Function Signin(ByVal username As String, ByVal password As String, ByVal confirm As String, ByVal privacy As Boolean, ByVal lang As String, ByVal connectionString As String) As SigninData
        Dim datResponse As New SigninData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = SigninDataEnum.Null

        '** Controllo email
        If username.Trim = String.Empty Then
            datResponse.Message = SigninDataEnum.EmptyUserName
            Return datResponse
        End If
        If username.Length < 6 Then
            datResponse.Message = SigninDataEnum.WrongUserNameLength
            Return datResponse
        End If
        If username.Contains(" ") Then
            datResponse.Message = SigninDataEnum.WrongUserName
            Return datResponse
        End If

        '** Controllo password
        If password.Trim = String.Empty Then
            datResponse.Message = SigninDataEnum.EmptyPassword
            Return datResponse
        End If
        If password.Length < 8 Then
            datResponse.Message = SigninDataEnum.WrongPasswordLength
            Return datResponse
        End If
        If password.Contains(" ") Then
            datResponse.Message = SigninDataEnum.WrongPassword
            Return datResponse
        End If

        '** Controllo conferma password
        If confirm.Trim = String.Empty Then
            datResponse.Message = SigninDataEnum.EmptyConfirm
            Return datResponse
        End If
        If password.Trim <> confirm.Trim Then
            datResponse.Message = SigninDataEnum.WrongConfirm
            Return datResponse
        End If

        '** Controllo privacy
        If Not privacy Then
            datResponse.Message = SigninDataEnum.PrivacySelect
            Return datResponse
        End If

        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                Dim sqlString As String = String.Empty
                Dim userId As Long = 0L

                '** Controllo se utente già attivato
                sqlString = "SELECT id, attivato FROM utentiweb " _
                          & "WHERE UPPER(username) = '" & FunctionsClass.CheckString(username).ToUpper & "' " _
                          & "AND password = '" & FunctionsClass.EncryptValue(password) & "'"

                If pConnector.AddReader("utentiweb", sqlString) Then
                    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("utentiweb")
                    Dim attivato As Integer = CType(sqlReader("attivato").ToString, Integer)

                    If attivato = 1 Then
                        pConnector.Dispose()
                        datResponse.Message = SigninDataEnum.UserAlreadyActivated
                        Return datResponse
                    Else
                        '** Attivazione utente
                        sqlString = "UPDATE utentiweb " _
                                  & "SET attivato = 1 " _
                                  & "WHERE UPPER(username) = '" & FunctionsClass.CheckString(username).ToUpper & "'"

                        If pConnector.ExecuteCommand(sqlString) Then
                            datResponse.Message = SigninDataEnum.UserChecked
                        Else
                            datResponse.Message = SigninDataEnum.ErrorDataBase
                        End If
                    End If

                    pConnector.DelReader("utentiweb")
                Else
                    pConnector.Dispose()
                    datResponse.Message = SigninDataEnum.ErrorCheckUser
                    Return datResponse
                End If
            Else
                datResponse.Message = SigninDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = SigninDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Structure SigninData
        Public UserId As Integer
        Public RedirectLink As String
        Public Message As SigninDataEnum
    End Structure

    Public Enum SigninDataEnum
        UserChecked
        ErrorUpdateUser
        ErrorCheckUser
        ErrorDataBase
        ErrorGeneric
        EmptyUserName
        EmptyPassword
        EmptyConfirm
        WrongUserName
        WrongUserNameLength
        WrongPassword
        WrongPasswordLength
        WrongConfirm
        PrivacySelect
        UserAlreadyActivated
        Null
    End Enum
End Class
