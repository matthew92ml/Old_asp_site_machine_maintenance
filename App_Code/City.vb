Imports Npgsql
Imports System.IO

Public Class CityClass
    Private Const TAGS_DATA_HEADER_IT As String = "codice"
    Private Const TAGS_DATA_HEADER_EN As String = "tag_code"

    Private Const STATIONS_DATA_HEADER_IT As String = "codice_stazione;via;civico;estenzione_civico;cap;citta;provincia"
    Private Const STATIONS_DATA_HEADER_EN As String = "station_code;address;number;number_extension;Zip_code;city;district"

    Public Shared Function CheckCityCode(ByRef pConnector As PostgreConnector, ByVal oEcofilUser As EcofilUser, ByVal cityId As Integer) As Boolean
        Dim sqlstring As String = String.Empty

        If oEcofilUser Is Nothing Then
            sqlstring = "SELECT id FROM infocomune " _
                      & "WHERE id = " & cityId.ToString
        Else
            If oEcofilUser.UserType = EcofilUser.UserTypeEnum.Manager Then
                sqlstring = "SELECT id FROM infocomune " _
                          & "WHERE id = " & cityId.ToString
            Else
                sqlstring = "SELECT idcomune FROM utentiwebcomuni " _
                          & "WHERE id = " & oEcofilUser.IdUtente.ToString & " " _
                          & "AND idcomune = " & cityId.ToString
            End If
        End If

        If pConnector.ExecuteScalar(sqlstring) > 0 Then Return True

        Return False
    End Function

    Public Shared Function GetCitiesData(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, ByRef oEcofilUser As EcofilUser) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT infocomune.id AS idcomune, " _
                                & "infocomune.nome AS nome, " _
                                & "infocomune.numeroabitanti AS numeroabitanti, " _
                                & "infocomune.numerostazioni AS numerostazioni, " _
                                & "[PERCENTUALE_RD], " _
                                & "[PRO_CAPITE], " _
                                & "[TOTALE_PESO] " _
                                & "FROM [TAB_COMUNI] "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then
            FunctionsClass.InsertWhereConditions(sqlString, hashParms)

            sqlString = sqlString.Replace("[TAB_COMUNI]", "(SELECT idcomune FROM utentiwebcomuni WHERE id = " & oEcofilUser.IdUtente.ToString & ") AS utentiwebcomuni INNER JOIN infocomune ON utentiwebcomuni.idcomune = infocomune.id")
        Else
            sqlString = sqlString.Replace("[TAB_COMUNI]", "infocomune")
        End If

        sqlString = sqlString.Replace("[PERCENTUALE_RD]", "(SELECT AVG(rdpercentuale) FROM datinuclei INNER JOIN classifiche ON datinuclei.id = classifiche.idnucleo WHERE datinuclei.idcomune = infocomune.id) AS percentuale_rd")
        sqlString = sqlString.Replace("[PRO_CAPITE]", "(SELECT AVG(mediaprocapite) FROM datinuclei INNER JOIN classifiche ON datinuclei.id = classifiche.idnucleo WHERE datinuclei.idcomune = infocomune.id) AS media_procapite")
        sqlString = sqlString.Replace("[TOTALE_PESO]", "(SELECT SUM(peso) FROM conferimenti WHERE conferimenti.idcomune = infocomune.id) AS totalepeso")

        If pConnector.AddReader("infocomune", sqlString) Then
            Dim cities As New List(Of String)

            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

                If sqlReader IsNot Nothing Then
                    Dim numeroStazioni As Integer = 0
                    Integer.TryParse(sqlReader("numerostazioni").ToString, numeroStazioni)
                    Dim percentualeRD As Double = 0.0
                    Double.TryParse(sqlReader("percentuale_rd").ToString, percentualeRD)
                    Dim mediaProCapite As Double = 0.0
                    Double.TryParse(sqlReader("media_procapite").ToString, mediaProCapite)
                    Dim peso As Double = 0.0
                    Double.TryParse(sqlReader("totalepeso").ToString, peso)

                    Dim linkDetalis As String = "<a class=\""btn btn-sm btn-block btn-info\"" href=\""" _
                                              & HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                              & oEcofilUser.IdUtenteEncrypted & "-" _
                                              & HttpContext.GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower & "-" _
                                              & sqlReader("idcomune").ToString & "\"">" _
                                              & HttpContext.GetGlobalResourceObject("controls", "details").ToString & "</a>"

                    Dim linkEdit As String = "<a class=\""btn btn-sm btn-block btn-success\"" href=\""" _
                                           & HttpContext.GetGlobalResourceObject("pages", "city_link").ToString.Replace("~/", "") & "?city=" & sqlReader("idcomune").ToString & "&action=UPD\"">" _
                                           & HttpContext.GetGlobalResourceObject("pages", "city_text").ToString & "</a>"

                    'cities.Add("[""" & sqlReader("nome").ToString & """,""" _
                    '                 & sqlReader("numeroabitanti").ToString & """,""" _
                    '                 & numeroStazioni.ToString & """,""" _
                    '                 & (percentualeRD * 100).ToString("0.00") & """,""" _
                    '                 & peso.ToString("0.00") & """,""" _
                    '                 & mediaProCapite.ToString("0.00") & """,""" _
                    '                 & (peso / numeroStazioni).ToString("0.00") & """,""" _
                    '                 & linkDetalis & """,""" _
                    '                 & linkEdit & """]")

                    '** Elenco senza gestione
                    cities.Add("[""" & sqlReader("nome").ToString & """,""" _
                                     & sqlReader("numeroabitanti").ToString & """,""" _
                                     & numeroStazioni.ToString & """,""" _
                                     & (percentualeRD * 100).ToString("0.00") & """,""" _
                                     & peso.ToString("0.00") & """,""" _
                                     & mediaProCapite.ToString("0.00") & """,""" _
                                     & (peso / numeroStazioni).ToString("0.00") & """,""" _
                                     & linkDetalis & """]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("infocomune")

            strResult = "[" & String.Join(",", cities) & "]"
        End If

        Return strResult
    End Function

    Public Shared Function GetCitiesMap(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, ByRef oEcofilUser As EcofilUser) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT latitudine, " _
                                & "longitudine, " _
                                & "nome " _
                                & "FROM [TAB_COMUNI] "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then
            FunctionsClass.InsertWhereConditions(sqlString, hashParms)

            sqlString = sqlString.Replace("[TAB_COMUNI]", "(SELECT idcomune FROM utentiwebcomuni WHERE id = " & oEcofilUser.IdUtente.ToString & ") AS utentiwebcomuni INNER JOIN infocomune ON utentiwebcomuni.idcomune = infocomune.id")
        Else
            sqlString = sqlString.Replace("[TAB_COMUNI]", "infocomune")
        End If

        If pConnector.AddReader("infocomune", sqlString) Then
            Dim points As New List(Of String)
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

                If sqlReader IsNot Nothing Then
                    Dim latitudine As Double = 0.0
                    Double.TryParse(sqlReader("latitudine").ToString, latitudine)
                    Dim longitudine As Double = 0.0
                    Double.TryParse(sqlReader("longitudine").ToString, longitudine)

                    points.Add("{""latitudine"":" & latitudine.ToString().Replace(",", ".") & "," _
                              & """longitudine"":" & longitudine.ToString.Replace(",", ".") & "," _
                              & """title"":""" & sqlReader("nome").ToString & """}")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("infocomune")

            strResult = "[" & String.Join(",", points) & "]"
        End If

        Return strResult
    End Function

    Public Shared Function GetCityData(ByRef pConnector As PostgreConnector, ByVal cityId As Integer) As Hashtable
        Dim hashResult As New Hashtable

        hashResult.Add("city-id", "0")
        hashResult.Add("city-name", String.Empty)
        hashResult.Add("city-citizens", String.Empty)
        hashResult.Add("city-units", String.Empty)
        hashResult.Add("city-stations", String.Empty)
        hashResult.Add("city-latitude", "0.0")
        hashResult.Add("city-longitude", "0.0")
        hashResult.Add("city-district", String.Empty)
        hashResult.Add("city-state", String.Empty)
        hashResult.Add("city-username", String.Empty)

        Try
            Dim sqlString As String = "SELECT id, " _
                                    & "nome, " _
                                    & "numeroabitanti, " _
                                    & "(SELECT COUNT(*) FROM datinuclei WHERE idcomune = infocomune.id) AS tot_nuclei, " _
                                    & "(SELECT COUNT(*) FROM infostazioni WHERE idcomune = infocomune.id) AS tot_stazioni, " _
                                    & "latitudine, " _
                                    & "longitudine, " _
                                    & "provincia, " _
                                    & "regione, " _
                                    & "(SELECT username FROM utentiwebcomuni INNER JOIN utentiweb ON utentiwebcomuni.id = utentiweb.id WHERE utentiwebcomuni.idcomune = infocomune.id AND utentiweb.usertype= 2) AS username " _
                                    & "FROM infocomune " _
                                    & "WHERE id = " & cityId.ToString

            If pConnector.AddReader("infocomune", sqlString) Then
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

                hashResult("city-id") = sqlReader("id").ToString
                hashResult("city-name") = sqlReader("nome").ToString
                hashResult("city-citizens") = sqlReader("numeroabitanti").ToString
                hashResult("city-units") = sqlReader("tot_nuclei").ToString
                hashResult("city-stations") = sqlReader("tot_stazioni").ToString
                hashResult("city-latitude") = sqlReader("latitudine").ToString
                hashResult("city-longitude") = sqlReader("longitudine").ToString
                hashResult("city-district") = sqlReader("provincia").ToString
                hashResult("city-state") = sqlReader("regione").ToString
                hashResult("city-username") = sqlReader("username").ToString

                pConnector.DelReader("infocomune")
            End If
        Catch ex As Exception
            '...
        End Try

        Return hashResult
    End Function

    Public Shared Function Save(ByVal cityId As Integer, ByRef hashValues As Hashtable, ByVal connectionString As String) As CityData
        Dim sqlString As String = String.Empty

        Dim datResponse As New CityData
        datResponse.Id = 0
        datResponse.RedirectLink = String.Empty
        datResponse.Message = CityDataEnum.Null

        '** Controllo nome
        If hashValues("city-name").ToString = String.Empty Then
            datResponse.Message = CityDataEnum.EmptyName
            Return datResponse
        End If

        '** Controllo provincia
        If hashValues("city-district").ToString = String.Empty Then
            datResponse.Message = CityDataEnum.EmptyDistrict
            Return datResponse
        End If

        '** Controllo regione
        If hashValues("city-state").ToString = String.Empty Then
            datResponse.Message = CityDataEnum.EmptyState
            Return datResponse
        End If

        '** Controllo abitanti
        If hashValues("city-citizens").ToString = String.Empty Then
            datResponse.Message = CityDataEnum.EmptyCitizens
            Return datResponse
        End If
        Dim intCitizens As Integer = 0
        If Not Integer.TryParse(hashValues("city-citizens").ToString, intCitizens) Then
            datResponse.Message = CityDataEnum.WrongCitizens
            Return datResponse
        End If

        '** Controllo user name
        If hashValues("city-username").ToString.Trim = String.Empty Then
            datResponse.Message = CityDataEnum.EmptyUserName
            Return datResponse
        End If
        If hashValues("city-username").contains(" ") = String.Empty Then
            datResponse.Message = CityDataEnum.WrongUserName
            Return datResponse
        End If


        '** Reperimento nuclei
        Dim intUnits As Integer = 0
        Integer.TryParse(hashValues("city-units").ToString, intUnits)

        '** Reperimento stazioni
        Dim intStations As Integer = 0
        Integer.TryParse(hashValues("city-stations").ToString, intStations)

        '** Reperimento latitudine
        Dim dblLatitude As Double = 0.0
        Double.TryParse(hashValues("city-latitude").ToString, dblLatitude)

        '** Reperimento longitudine
        Dim dblLongitude As Double = 0.0
        Double.TryParse(hashValues("city-longitude").ToString, dblLongitude)

        '** Controllo coordinate
        Dim strSearch = hashValues("city-name").ToString.ToUpper.Replace(" ", "%20") & "%20" _
                      & hashValues("city-district").ToString.ToUpper.Replace(" ", "%20") & "%20" _
                      & hashValues("city-state").ToString.ToUpper.Replace(" ", "%20")

        Dim infosResult As FunctionsClass.GeoInfos = FunctionsClass.GetLocationLatLon("address=" & strSearch)
        If infosResult.Latitudine > 0.0 And infosResult.Longitudine > 0.0 Then
            dblLatitude = infosResult.Latitudine
            dblLongitude = infosResult.Longitudine
        End If

        '** Connessione al database
        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                If cityId = 0 Then
                    '** Creazione nuovo ente
                    sqlString = "INSERT INTO infocomune " _
                              & "(nome, " _
                              & "numeroabitanti, " _
                              & "numeronuclei, " _
                              & "numerostazioni, " _
                              & "latitudine, " _
                              & "longitudine, " _
                              & "provincia, " _
                              & "regione) " _
                              & "VALUES ('" & FunctionsClass.CheckString(hashValues("city-name").ToString).ToUpper & "', " _
                              & intCitizens.ToString & ", " _
                              & intUnits.ToString & ", " _
                              & intStations.ToString & ", " _
                              & dblLatitude.ToString("0.00").Replace(",", ".") & ", " _
                              & dblLongitude.ToString("0.00").Replace(",", ".") & ", '" _
                              & FunctionsClass.CheckString(hashValues("city-district").ToString).ToUpper & "', '" _
                              & FunctionsClass.CheckString(hashValues("city-state").ToString).ToUpper & "') RETURNING id"

                    Dim intCityId As Integer = pConnector.ExecuteCommandWithIdBack(sqlString)

                    If intCityId > 0 Then
                        Dim intUserId As Integer = 0

                        '** Controllo se utente già inserito
                        sqlString = "SELECT id FROM utentiweb " _
                                  & "WHERE username = '" & hashValues("city-username").ToString.ToLower & "'"

                        If pConnector.AddReader("utentiweb", sqlString) Then
                            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("utentiweb")

                            Integer.TryParse(sqlReader("utentiweb").ToString, intUserId)

                            pConnector.DelReader("utentiweb")
                        End If

                        '** Reperimento coordinate medie
                        Dim geoInfos As FunctionsClass.GeoInfos = FunctionsClass.GetWebUserCoords(pConnector, intCityId)

                        If intUserId = 0 Then
                            '** Creazione utente web
                            sqlString = "INSERT INTO utentiweb " _
                                      & "(username, " _
                                      & "password, " _
                                      & "usertype, " _
                                      & "latitudine, " _
                                      & "longitudine, " _
                                      & "idnucleo, " _
                                      & "recoverycode, " _
                                      & "attivato, " _
                                      & "ultimoaccesso) " _
                                      & "VALUES('" & FunctionsClass.CheckString(hashValues("city-username").ToString).ToLower & "', '', 1, " _
                                      & geoInfos.Latitudine.ToString("0.00").Replace(",", ".") & ", " _
                                      & geoInfos.Longitudine.ToString("0.00").Replace(",", ".") & " 0, '', 0, '" _
                                      & Now.ToString("s") & "') RETURNING id"

                            intUserId = pConnector.ExecuteCommandWithIdBack(sqlString)
                        Else
                            '** Aggiornamento coordinate utente web
                            sqlString = "UPDATE utentiweb " _
                                      & "SET latitudine = " & geoInfos.Latitudine.ToString("0.00").Replace(",", ".") & ", " _
                                      & "longitudine = " & geoInfos.Longitudine.ToString("0.00").Replace(",", ".") & " " _
                                      & "WHERE id = " & intUserId.ToString

                            pConnector.ExecuteCommand(sqlString)
                        End If

                        '** Creazione nuova relazione utente web - ente
                        sqlString = "INSERT INTO utentiwebcomuni " _
                                  & "(id," _
                                  & "idcomune) " _
                                  & "VALUES(" & intUserId.ToString & ", " _
                                  & intCityId.ToString & ")"

                        If Not pConnector.ExecuteCommand(sqlString) Then
                            datResponse.Id = intCityId
                            datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "city_link").ToString & "?city=" & intCityId.ToString & "&action=UPD"
                            datResponse.Message = CityDataEnum.Created
                        Else
                            datResponse.Message = CityDataEnum.ErrorSavingCityUserWeb
                        End If
                    Else
                        datResponse.Message = CityDataEnum.ErrorSavingCity
                    End If
                Else
                    '** Aggiornamento ente
                    sqlString = "UPDATE infocomune " _
                              & "SET nome = '" & FunctionsClass.CheckString(hashValues("city-name").ToString).ToUpper & "', " _
                              & "numeroabitanti = " & intCitizens.ToString & ", " _
                              & "numeronuclei = " & intUnits.ToString & ", " _
                              & "numerostazioni = " & intStations.ToString & ", " _
                              & "latitudine = " & dblLatitude.ToString("0.00").Replace(",", ".") & ", " _
                              & "longitudine = " & dblLongitude.ToString("0.00").Replace(",", ".") & ", " _
                              & "provincia = '" & FunctionsClass.CheckString(hashValues("city-district").ToString).ToUpper & "', " _
                              & "regione = '" & FunctionsClass.CheckString(hashValues("city-state").ToString).ToUpper & "' " _
                              & "WHERE id = " & cityId.ToString

                    If pConnector.ExecuteCommand(sqlString) Then
                        datResponse.Message = CityDataEnum.Updated
                    Else
                        datResponse.Message = CityDataEnum.ErrorSavingCity
                    End If
                End If
            Else
                datResponse.Message = CityDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = CityDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Shared Function UploadCsvTags(ByVal cityId As Integer, ByVal filePath As String, ByVal lang As String, ByVal connectionString As String) As String
        Dim strResult As String = String.Empty

        Dim sqlString As String = String.Empty

        '** Apertura file importazione
        Dim sr As New StreamReader(filePath)

        '** Creazione file di log
        Dim logFileName As String = filePath.Replace(".csv", ".txt")
        Dim slog As New StreamWriter(logFileName)

        '** Contatori
        Dim row As Integer = 0
        Dim imp As Integer = 0
        Dim err As Integer = 0

        '** Connessione al database
        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                '** Controllo intestazioni file

                Dim strHeader As String = sr.ReadLine
                If strHeader <> TAGS_DATA_HEADER_IT And strHeader <> TAGS_DATA_HEADER_EN Then
                    slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_032").ToString)
                Else
                    row = 1

                    Do While Not sr.EndOfStream
                        Dim values() As String = sr.ReadLine.Split(";")

                        If CheckRowTag(values, row, slog, pConnector) Then
                            '** Controllo se tag già presente
                            sqlString = "SELECT COUNT(*) FROM infonuclei " _
                                      & "WHERE codicetag = '" & values(0).ToLower & "'"

                            If pConnector.ExecuteScalar(sqlString) = 0 Then
                                '** Inserimento tag
                                sqlString = "INSERT INTO infonuclei " _
                                          & "(codicetag, " _
                                          & "idcomune) " _
                                          & "VALUES('" & values(0).ToLower & "', " _
                                          & cityId.ToString & ")"

                                If pConnector.ExecuteCommand(sqlString) Then
                                    slog.WriteLine("MSG-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("messages", "message_017").ToString)
                                    imp += 1
                                Else
                                    slog.WriteLine("ERR-R: " & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_051").ToString)
                                    err += 1
                                End If
                            Else
                                slog.WriteLine("ERR-R: " & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_050").ToString)
                                err += 1
                            End If
                        Else
                            err += 1
                        End If

                        row += 1
                    Loop
                End If
            Else
                slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_005").ToString)
            End If
        Catch ex As Exception
            slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_007").ToString & " - " & ex.Message)
        Finally
            slog.WriteLine(HttpContext.GetGlobalResourceObject("messages", "message_014").ToString.Replace("{p1}", (row - 1).ToString).Replace("{p2}", imp.ToString).Replace("{p3}", err.ToString))

            sr.Close()
            sr.Dispose()

            slog.Close()
            slog.Dispose()

            pConnector.Dispose()
        End Try

        Return "temp/" & logFileName.Substring(logFileName.LastIndexOf("\") + 1)
    End Function

    Private Shared Function CheckRowTag(ByRef values() As String, ByVal row As Integer, ByRef sLog As StreamWriter, ByRef pConnector As PostgreConnector) As Boolean
        Dim booResult As Boolean = True

        '** Controllo codice
        If values(0) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_049").ToString)
            booResult = False
        End If

        Return booResult
    End Function

    Public Shared Function UploadCsvStations(ByVal cityId As Integer, ByVal filePath As String, ByVal lang As String, ByVal connectionString As String) As String
        Dim strResult As String = String.Empty

        Dim sqlString As String = String.Empty

        '** Apertura file importazione
        Dim sr As New StreamReader(filePath)

        '** Creazione file di log
        Dim logFileName As String = filePath.Replace(".csv", ".txt")
        Dim slog As New StreamWriter(logFileName)

        '** Contatori
        Dim row As Integer = 0
        Dim imp As Integer = 0
        Dim err As Integer = 0

        '** Connessione al database
        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                '** Controllo intestazioni file

                Dim strHeader As String = sr.ReadLine
                If strHeader <> STATIONS_DATA_HEADER_IT And strHeader <> STATIONS_DATA_HEADER_EN Then
                    slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_032").ToString)
                Else
                    row = 1

                    Do While Not sr.EndOfStream
                        Dim values() As String = sr.ReadLine.Split(";")

                        If CheckRowStation(values, row, slog, pConnector) Then
                            '** Controllo se stazione già presente
                            sqlString = "SELECT COUNT(*) FROM infostazione " _
                                      & "WHERE codicestazione = " & values(0).ToLower

                            If pConnector.ExecuteScalar(sqlString) = 0 Then
                                '** Costruzione indirizzo
                                Dim strAddress As String = values(6) & ", " _
                                                         & values(7) _
                                                         & IIf(values(8) <> String.Empty, " " & values(8), String.Empty).ToString & ", " _
                                                         & values(9) & " " _
                                                         & values(10) & " (" & values(11) & ")"

                                '** Controllo coordinate
                                Dim infosResult As FunctionsClass.GeoInfos = FunctionsClass.GetLocationLatLon("address=" & strAddress)

                                '** Inserimento stazione
                                sqlString = "INSERT INTO infostazioni " _
                                          & "(codicestazione, " _
                                          & "idcomune, " _
                                          & "latitudine, " _
                                          & "lognitudine, " _
                                          & "via) " _
                                          & "VALUES('" & values(0).ToLower & "', " _
                                          & cityId.ToString & ", " _
                                          & infosResult.Latitudine.ToString("0.00").Replace(",", ".") & ", " _
                                          & infosResult.Longitudine.ToString("0.00").Replace(",", ".") & ", '" _
                                          & strAddress & "')"

                                If pConnector.ExecuteCommand(sqlString) Then
                                    slog.WriteLine("MSG-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("messages", "message_018").ToString)
                                    imp += 1
                                Else
                                    slog.WriteLine("ERR-R: " & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_054").ToString)
                                    err += 1
                                End If
                            Else
                                slog.WriteLine("ERR-R: " & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_053").ToString)
                                err += 1
                            End If
                        Else
                            err += 1
                        End If

                        row += 1
                    Loop
                End If
            Else
                slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_005").ToString)
            End If
        Catch ex As Exception
            slog.WriteLine("ERR: " & HttpContext.GetGlobalResourceObject("errors", "error_007").ToString & " - " & ex.Message)
        Finally
            slog.WriteLine(HttpContext.GetGlobalResourceObject("messages", "message_016").ToString.Replace("{p1}", (row - 1).ToString).Replace("{p2}", imp.ToString).Replace("{p3}", err.ToString))

            sr.Close()
            sr.Dispose()

            slog.Close()
            slog.Dispose()

            pConnector.Dispose()
        End Try

        Return "temp/" & logFileName.Substring(logFileName.LastIndexOf("\") + 1)
    End Function

    Private Shared Function CheckRowStation(ByRef values() As String, ByVal row As Integer, ByRef sLog As StreamWriter, ByRef pConnector As PostgreConnector) As Boolean
        Dim booResult As Boolean = True

        '** Controllo codice
        If values(0) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_052").ToString)
            booResult = False
        End If

        '** Controllo via
        If values(1) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_022").ToString)
            booResult = False
        End If

        '** Controllo numero civico
        Dim intNumber As Integer = Integer.MinValue
        Integer.TryParse(values(2), intNumber)
        If intNumber = Integer.MinValue Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_033").ToString)
            booResult = False
        End If

        '** Controllo cap
        If values(4) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_034").ToString)
            booResult = False
        End If

        '** Controllo city
        If values(5) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_035").ToString)
            booResult = False
        End If

        '** Controllo provincia
        If values(6) = String.Empty Then
            sLog.WriteLine("ERR-R-" & row.ToString("00000") & ": " & HttpContext.GetGlobalResourceObject("errors", "error_036").ToString)
            booResult = False
        End If

        Return booResult
    End Function

    Public Shared Function DeleteCity(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByRef oEcofilUser As EcofilUser) As DeleteResultEnum
        Dim sqlString As String = "DELETE FROM utentiwebcomuni " _
                                & "WHERE idcomune = " & cityId.ToString & " " _
                                & "AND id = " & oEcofilUser.IdUtente.ToString

        If pConnector.ExecuteCommand(sqlString) Then
            Return DeleteResultEnum.Deleted
        Else
            Return DeleteResultEnum.CityNotDeleted
        End If
    End Function

    Public Structure CityData
        Public Id As Integer
        Public RedirectLink As String
        Public Message As CityDataEnum
    End Structure

    Public Enum CityDataEnum
        Created
        Updated
        EmptyName
        EmptyDistrict
        EmptyState
        EmptyCitizens
        EmptyUserName
        WrongCitizens
        WrongUserName
        ErrorSavingCity
        ErrorSavingCityUserWeb
        ErrorDataBase
        ErrorGeneric
        Null
    End Enum

    Public Enum DeleteResultEnum
        Deleted
        CityIdNotValid
        CityNotDeleted
    End Enum
End Class
