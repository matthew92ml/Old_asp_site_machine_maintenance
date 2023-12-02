Imports Npgsql

Public Class ProfileClass
    Public Shared Function Save(ByVal oEcofilUser As EcofilUser, ByRef hashValues As Hashtable, ByVal lang As String, ByVal connectionString As String) As ProfileData
        Dim sqlString As String = String.Empty

        Dim datResponse As New ProfileData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = ProfileDataEnum.Null

        '** Controllo phone
        'If oEcofilUser.UserType = EcofilUser.UserTypeEnum.User AndAlso hashValues("profile-phone").ToString = String.Empty Then
        '    datResponse.Message = ProfileDataEnum.EmptyPhone
        '    Return datResponse
        'End If

        '** Controllo mobile
        If oEcofilUser.UserType = EcofilUser.UserTypeEnum.User AndAlso hashValues("profile-mobile").ToString = String.Empty Then
            datResponse.Message = ProfileDataEnum.EmptyMobile
            Return datResponse
        End If

        '** Controllo email
        'If hashValues("profile-email").ToString = String.Empty Then
        '    datResponse.Message = ProfileDataEnum.EmptyEmail
        '    Return datResponse
        'End If
        Dim emailExpression As New Regex("^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})$")
        If hashValues("profile-email").ToString <> String.Empty AndAlso Not emailExpression.IsMatch(hashValues("profile-email").ToString) Then
            datResponse.Message = ProfileDataEnum.WrongEmail
            Return datResponse
        End If

        '** Controllo user name
        If hashValues("profile-username").ToString = String.Empty Then
            datResponse.Message = ProfileDataEnum.EmptyUserName
            Return datResponse
        End If
        If hashValues("profile-username").ToString.Contains(" ") Then
            datResponse.Message = ProfileDataEnum.WrongUserName
            Return datResponse
        End If
        If hashValues("profile-username").ToString.Length < 6 Then
            datResponse.Message = ProfileDataEnum.WrongUserNameLength
            Return datResponse
        End If

        '** Controllo password
        If hashValues("profile-password").ToString <> String.Empty AndAlso hashValues("profile-password").ToString.Contains(" ") Then
            datResponse.Message = ProfileDataEnum.WrongPassword
            Return datResponse
        End If
        If hashValues("profile-password").ToString <> String.Empty AndAlso hashValues("profile-password").ToString.Length < 8 Then
            datResponse.Message = ProfileDataEnum.WrongPasswordLength
            Return datResponse
        End If

        '** Controllo conferma password
        If hashValues("profile-password").ToString <> String.Empty AndAlso hashValues("profile-confirm").ToString = String.Empty Then
            datResponse.Message = ProfileDataEnum.EmptyConfirm
            Return datResponse
        End If
        If hashValues("profile-password").ToString <> String.Empty AndAlso hashValues("profile-password").ToString <> hashValues("profile-confirm").ToString Then
            datResponse.Message = ProfileDataEnum.WrongConfirm
            Return datResponse
        End If

        '** Controllo abitanti
        Dim intAbitanti As Integer = -1
        If oEcofilUser.UserType = EcofilUser.UserTypeEnum.Administration AndAlso hashValues.Contains("profile-city") Then
            If hashValues("profile-city-citizens").ToString <> String.Empty Then
                If Integer.TryParse(hashValues("profile-city-citizens").ToString, intAbitanti) AndAlso intAbitanti < 1 Then
                    datResponse.Message = ProfileDataEnum.WrongCitizens
                    Return datResponse
                End If
            End If
        End If

        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                Dim booCompostiera As Boolean = False
                Dim booPannolini As Boolean = False
                If oEcofilUser.UserType = EcofilUser.UserTypeEnum.User Then
                    Boolean.TryParse(hashValues("profile-compostiera").ToString, booCompostiera)
                    Boolean.TryParse(hashValues("profile-pannolini").ToString, booPannolini)
                End If

                If oEcofilUser.NomeUtente <> hashValues("profile-username").ToString Then
                    '** Controllo se nome utente modificato già usato da un altro utente
                    If Not FunctionsClass.CheckUserName(pConnector, FunctionsClass.CheckString(hashValues("profile-username").ToString), oEcofilUser.IdUtente) Then
                        '** Aggiornamento utente
                        sqlString = "UPDATE utentiweb " _
                                  & "SET username = '" & FunctionsClass.CheckString(hashValues("profile-username").ToString).ToLower & "'"

                        If hashValues("profile-password").ToString <> String.Empty Then
                            sqlString = ", password = '" & FunctionsClass.EncryptValue(hashValues("profile-password").ToString) & "'"
                        End If

                        sqlString &= " WHERE id = " & oEcofilUser.IdUtente.ToString

                        If pConnector.ExecuteCommand(sqlString) Then
                            '** Aggiornamento dati nucleo
                            sqlString = "UPDATE datinuclei " _
                                      & "SET mail = '" & FunctionsClass.CheckString(hashValues("profile-email").ToString).ToLower & "'"

                            If oEcofilUser.UserType = EcofilUser.UserTypeEnum.User Then
                                Dim flagInvio As String = "0"
                                If hashValues("profile-option-address").ToString = "1" Then flagInvio = "0"
                                If hashValues("profile-option-address-user").ToString = "1" Then flagInvio = "1"
                                If hashValues("profile-option-address-other").ToString = "1" Then flagInvio = "2"
                                If hashValues("profile-option-email").ToString = "1" Then flagInvio = "3"

                                sqlString &= ", telefono = '" & FunctionsClass.CheckString(hashValues("profile-phone").ToString).ToLower & "', " _
                                           & "cellulare = '" & FunctionsClass.CheckString(hashValues("profile-mobile").ToString).ToLower & "', " _
                                           & "indirizzoalternativo = '" & FunctionsClass.CheckString(hashValues("profile-address-other").ToString).ToLower & "', " _
                                           & "flaginvio = '" & flagInvio & "', " _
                                           & "compostiera = '" & IIf(booCompostiera, "1", "0").ToString & "', " _
                                           & "pannolini = '" & IIf(booPannolini, "1", "0").ToString & "'"
                            End If

                            sqlString &= " WHERE id = " & oEcofilUser.IdNucleo.ToString

                            If pConnector.ExecuteCommand(sqlString) Then
                                datResponse.Message = ProfileDataEnum.Updated
                            Else
                                datResponse.Message = ProfileDataEnum.ErrorSavingUnit
                            End If
                        Else
                            datResponse.Message = ProfileDataEnum.ErrorSavingProfile
                        End If
                    Else
                        datResponse.Message = ProfileDataEnum.UserNameAlreadyUsed
                    End If
                Else
                    If hashValues("profile-password").ToString <> String.Empty Then
                        '** Aggiornamento utente
                        sqlString = "UPDATE utentiweb " _
                                  & "SET password = '" & FunctionsClass.EncryptValue(hashValues("profile-password").ToString) & "' " _
                                  & "WHERE id = " & oEcofilUser.IdUtente.ToString

                        If pConnector.ExecuteCommand(sqlString) Then
                            datResponse.Message = ProfileDataEnum.Updated
                        Else
                            datResponse.Message = ProfileDataEnum.ErrorSavingProfile
                        End If
                    Else
                        datResponse.Message = ProfileDataEnum.Updated
                    End If

                    If oEcofilUser.UserType = EcofilUser.UserTypeEnum.User Then
                        Dim flagInvio As String = "0"
                        If hashValues("profile-option-address").ToString = "1" Then flagInvio = "0"
                        If hashValues("profile-option-address-user").ToString = "1" Then flagInvio = "1"
                        If hashValues("profile-option-address-other").ToString = "1" Then flagInvio = "2"
                        If hashValues("profile-option-email").ToString = "1" Then flagInvio = "3"

                        '** Aggiornamento dati nucleo
                        sqlString = "UPDATE datinuclei " _
                                  & "SET telefono = '" & FunctionsClass.CheckString(hashValues("profile-phone").ToString).ToLower & "', " _
                                  & "cellulare = '" & FunctionsClass.CheckString(hashValues("profile-mobile").ToString).ToLower & "', " _
                                  & "mail = '" & FunctionsClass.CheckString(hashValues("profile-email").ToString).ToLower & "', " _
                                  & "indirizzoalternativo = '" & FunctionsClass.CheckString(hashValues("profile-address-other").ToString).ToLower & "', " _
                                  & "flaginvio = '" & flagInvio & "', " _
                                  & "compostiera = '" & IIf(booCompostiera, "1", "0").ToString & "', " _
                                  & "pannolini = '" & IIf(booPannolini, "1", "0").ToString & "' " _
                                  & "WHERE id = " & oEcofilUser.IdNucleo.ToString

                        If pConnector.ExecuteCommand(sqlString) Then
                            datResponse.Message = ProfileDataEnum.Updated
                        Else
                            datResponse.Message = ProfileDataEnum.ErrorSavingUnit
                        End If
                    End If
                End If

                '** Attivazione tag nucleo    
                Dim tagList As New List(Of String)
                sqlString = "SELECT codicetag FROM associazionenuclei " _
                          & "WHERE codicenucleo = (SELECT codicenucleo FROM datinuclei WHERE id = " & oEcofilUser.IdNucleo.ToString & ")"

                If pConnector.AddReader("associazionenuclei", sqlString) Then
                    Do While True
                        Dim sqlReader As NpgsqlDataReader = pConnector.GetData("associazionenuclei")

                        If sqlReader IsNot Nothing Then
                            tagList.Add(sqlReader("codicetag").ToString)
                        Else
                            Exit Do
                        End If
                    Loop

                    pConnector.DelReader("associazionenuclei")
                End If

                If tagList.Count > 0 Then
                    For Each tag As String In tagList
                        sqlString = "INSERT INTO infonuclei " _
                                  & "(codicetag, idcomune) " _
                                  & "SELECT '" & tag & "', " & oEcofilUser.IdComune(0).ToString & " " _
                                  & "WHERE NOT EXISTS (SELECT id FROM infonuclei WHERE codicetag = '" & tag & "' AND idcomune = " & oEcofilUser.IdComune(0).ToString & ")"

                        pConnector.ExecuteCommand(sqlString)
                    Next
                End If

                '** Aggiornamento abitanti comune
                If intAbitanti > 0 AndAlso datResponse.Message = ProfileDataEnum.Updated Then
                    sqlString = "UPDATE infocomune " _
                              & "SET numeroabitanti = " & intAbitanti.ToString & " " _
                              & "WHERE id = " & hashValues("profile-city").ToString

                    If Not pConnector.ExecuteCommand(sqlString) Then datResponse.Message = ProfileDataEnum.ErrorSavingCitizens
                End If
            Else
                datResponse.Message = ProfileDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = ProfileDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Structure ProfileData
        Public RedirectLink As String
        Public Message As ProfileDataEnum
    End Structure

    Public Enum ProfileDataEnum
        Updated
        EmptyPhone
        EmptyMobile
        EmptyEmail
        EmptyUserName
        EmptyConfirm
        WrongEmail
        WrongUserName
        WrongUserNameLength
        WrongPassword
        WrongPasswordLength
        WrongConfirm
        WrongCitizens
        UserNameAlreadyUsed
        ErrorSavingProfile
        ErrorSavingUnit
        ErrorSavingCitizens
        ErrorDataBase
        ErrorGeneric
        Null
    End Enum
End Class
