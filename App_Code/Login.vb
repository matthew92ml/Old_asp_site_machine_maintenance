Imports Npgsql

Public Class LoginClass
    Public Shared Function Login(ByVal username As String, ByVal password As String, ByVal lang As String, ByVal connectionString As String, Optional isFromCookie As Boolean = False) As LoginData
        Dim datResponse As New LoginData
        datResponse.EcofilUser = Nothing
        datResponse.RedirectLink = String.Empty
        datResponse.Message = LoginDataEnum.Null

        '** Controllo username
        If username = String.Empty Then
            datResponse.Message = LoginDataEnum.EmptyUsername
            Return datResponse
        End If
        If username.Contains(" ") Then
            datResponse.Message = LoginDataEnum.WrongUserName
            Return datResponse
        End If

        '** Controllo password
        If password = String.Empty Then
            datResponse.Message = LoginDataEnum.EmptyPassword
            Return datResponse
        End If
        If password.Contains(" ") Then
            datResponse.Message = LoginDataEnum.WrongPassword
            Return datResponse
        End If

        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                Dim sqlString As String = "SELECT utentiweb.id, " _
                                        & "utentiweb.password, " _
                                        & "utentiweb.username, " _
                                        & "utentiweb.idnucleo, " _
                                        & "datinuclei.nome, " _
                                        & "datinuclei.cognome, " _
                                        & "datinuclei.indirizzo, " _
                                        & "datinuclei.indirizzoutenza, " _
                                        & "datinuclei.indirizzoalternativo, " _
                                        & "datinuclei.flaginvio, " _
                                        & "datinuclei.telefono, " _
                                        & "datinuclei.cellulare, " _
                                        & "datinuclei.mail, " _
                                        & "datinuclei.codicenucleo, " _
                                        & "datinuclei.compostiera, " _
                                        & "datinuclei.pannolini, " _
                                        & "utentiweb.latitudine, " _
                                        & "utentiweb.longitudine, " _
                                        & "utentiweb.usertype, " _
                                        & "utentiweb.attivato " _
                                        & "FROM utentiweb " _
                                        & "LEFT JOIN datinuclei ON utentiweb.idnucleo = datinuclei.id " _
                                        & "WHERE UPPER(utentiweb.username) = '" & FunctionsClass.CheckString(username).ToUpper & "'"

                If pConnector.AddReader("utentiweb", sqlString) Then
                    Dim booFirstRec As Boolean = True
                    Dim oEcofilUser As EcofilUser = Nothing

                    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("utentiweb")

                    Dim encryptPassword As String = IIf(isFromCookie, password, FunctionsClass.EncryptValue(password)).ToString

                    If sqlReader("password").ToString = encryptPassword Then
                        If CType(sqlReader("attivato"), Integer) = 1 Then
                            oEcofilUser = New EcofilUser()
                            oEcofilUser.ConnectUser(sqlReader, HttpContext.Current.Session.SessionID)

                            If oEcofilUser.LoggedOn Then
                                If CType(sqlReader("attivato"), Integer) = 1 Then
                                    datResponse.Message = LoginDataEnum.LoggedOn
                                Else
                                    datResponse.Message = LoginDataEnum.ErrorConnectUser
                                End If
                            Else
                                datResponse.Message = LoginDataEnum.ErrorConnectUser
                            End If
                        Else
                            datResponse.Message = LoginDataEnum.ErrorConnectUser
                        End If
                    Else
                        datResponse.Message = LoginDataEnum.WrongPassword
                    End If

                    pConnector.DelReader("utentiweb")

                    If datResponse.Message = LoginDataEnum.LoggedOn Then
                        '** Reperimento comuni
                        sqlString = "SELECT idcomune FROM utentiwebcomuni " _
                                  & "WHERE id = " & oEcofilUser.IdUtente.ToString

                        If pConnector.AddReader("utentiwebcomuni", sqlString) Then
                            Do While True
                                Dim sqlReader2 As NpgsqlDataReader = pConnector.GetData("utentiwebcomuni")

                                If sqlReader2 IsNot Nothing Then
                                    oEcofilUser.AddCity(CType(sqlReader2("idcomune").ToString, Integer))
                                Else
                                    Exit Do
                                End If
                            Loop

                            pConnector.DelReader("utentiwebcomuni")
                        End If

                        '** Configurazione dati di sessione
                        datResponse.EcofilUser = oEcofilUser
                        Select Case oEcofilUser.UserType
                            Case EcofilUser.UserTypeEnum.User
                                '** Reperimento tag collegati
                                sqlString = "SELECT codicetag FROM associazionenuclei " _
                                          & "WHERE codicenucleo = '" & oEcofilUser.CodiceNucleo & "'"

                                If pConnector.AddReader("associazionenuclei", sqlString) Then
                                    Do While True
                                        Dim sqlReader2 As NpgsqlDataReader = pConnector.GetData("associazionenuclei")

                                        If sqlReader2 IsNot Nothing Then
                                            oEcofilUser.AddTag(sqlReader2("codicetag").ToString)
                                        Else
                                            Exit Do
                                        End If
                                    Loop

                                    pConnector.DelReader("associazionenuclei")
                                End If

                                datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted
                            Case EcofilUser.UserTypeEnum.Administration
                                If oEcofilUser.IdComune.Count = 1 Then
                                    datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted
                                Else
                                    datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted & "-" & HttpContext.GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower
                                End If
                            Case EcofilUser.UserTypeEnum.Manager
                                datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted & "-" & HttpContext.GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower
                        End Select

                        '** Aggiornamento data accesso
                        sqlString = "UPDATE utentiweb " _
                                  & "SET ultimoaccesso = '" & Now.ToString("s") & "' " _
                                  & "WHERE UPPER(username) = '" & FunctionsClass.CheckString(username).ToUpper & "'"

                        pConnector.ExecuteCommand(sqlString)
                    End If
                Else
                    datResponse.Message = LoginDataEnum.WrongUserName
                End If
            Else
                datResponse.Message = LoginDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = LoginDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Shared Sub CreateCookie(ByVal Request As System.Web.HttpRequest, ByVal Response As System.Web.HttpResponse, ByVal checkRemember As CheckBox, ByVal user As String, ByVal password As String)
        If checkRemember.Checked Then
            '** Controllo se il browser accetta i cookies
            If Request.Browser.Cookies Then
                Dim myCookie As HttpCookie = New HttpCookie("myCookie")
                myCookie("parm1") = user
                myCookie("parm2") = FunctionsClass.EncryptValue(password)
                myCookie.Expires = Now.AddDays(30)
                Response.Cookies.Add(myCookie)
            End If
        End If
    End Sub

    Public Shared Function CheckCookie(ByVal Request As System.Web.HttpRequest, ByVal lang As String, ByVal connectionString As String) As LoginData
        Dim datResponse As New LoginData

        '** Controllo se il browser accetta i cookies
        If Request.Browser.Cookies Then
            '** Controllo se esiste il cookie "myCookie" per la funzione "Alwais logged in"
            If Request.Cookies("myCookie") IsNot Nothing Then
                '** Controllo se sono presenti i parametri nel cookie "myCookie"                        
                If Request.Cookies("myCookie")("parm1") IsNot Nothing And Request.Cookies("myCookie")("parm2") IsNot Nothing Then
                    '** Verifica le credenziali presenti nel cookie per effettuare il login automatico
                    datResponse = Login(Request.Cookies("myCookie")("parm1").ToString, Request.Cookies("myCookie")("parm2").ToString, lang, connectionString, True)
                End If
            End If
        End If

        Return datResponse
    End Function

    Public Shared Sub ExitLogin(ByVal Request As System.Web.HttpRequest, ByVal Response As System.Web.HttpResponse)
        '** Eliminazione del cookie "myCookie" per la funzione "Alwais logged in"
        If Request.Cookies("myCookie") IsNot Nothing Then
            Dim myCookie As HttpCookie
            myCookie = New HttpCookie("myCookie")
            myCookie.Expires = Now.AddDays(-30)
            Response.Cookies.Add(myCookie)
        End If
    End Sub

    Public Structure LoginData
        Public EcofilUser As EcofilUser
        Public RedirectLink As String
        Public Message As LoginDataEnum
    End Structure

    Public Enum LoginDataEnum
        LoggedOn
        ErrorConnectUser
        ErrorDataBase
        ErrorGeneric
        EmptyUserName
        EmptyPassword
        WrongUserName
        WrongPassword
        Null
    End Enum
End Class
