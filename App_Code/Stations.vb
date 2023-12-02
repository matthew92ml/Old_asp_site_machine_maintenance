Imports System.IO
Imports Npgsql
'verifica stazione
Public Class StationsClass
    Public Shared Function CheckStationCode(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal stationId As Integer) As Boolean
        Dim sqlstring As String = "SELECT codicestazione FROM infostazioni " _
                                & "WHERE idcomune = " & cityId.ToString & " " _
                                & "AND codicestazione = " & stationId.ToString

        If pConnector.ExecuteScalar(sqlstring) > 0 Then Return True

        Return False
    End Function

    Public Shared Function GetStationInfos(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal stationId As Integer) As StationsInfos
        Dim stationsInfosResult As New StationsInfos

        Dim sqlString As String = "SELECT nome, via FROM infostazioni WHERE codicestazione = " & stationId.ToString & " AND idcomune = " & cityId.ToString

        If pConnector.AddReader("infostazioni", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infostazioni")

            stationsInfosResult.Nome = sqlReader("nome").ToString
            stationsInfosResult.Via = sqlReader("via").ToString

            pConnector.DelReader("infostazioni")
        End If

        Return stationsInfosResult
    End Function

    Public Shared Function GetStationsData(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, ByRef oEcofilUser As EcofilUser, ByVal singleCity As Boolean, ByVal dateStart As Date, ByVal dateEnd As Date) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT infostazioni.codicestazione AS idStazione, " _
                                & "infostazioni.idcomune AS idComune, " _
                                & "infostazioni.nome AS nome, " _
                                & "infostazioni.via AS via, " _
                                & "[NUMERO_NUCLEI], " _
                                & "[TOTALE_PESO] " _
                                & "FROM infostazioni "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then FunctionsClass.InsertWhereConditions(sqlString, hashParms)

        Dim sqlStringUnits As String = "SELECT COUNT(DISTINCT codicetag) FROM conferimenti WHERE idcomune = infostazioni.idcomune AND codicestazione = infostazioni.codicestazione"
        Dim sqlStringWeight As String = "SELECT SUM(peso) FROM conferimenti WHERE idcomune = infostazioni.idcomune AND codicestazione = infostazioni.codicestazione"

        If dateStart <> Date.MinValue Then
            sqlStringUnits &= " AND conferimenti.dataora >= '" & dateStart.ToString("s") & "'"
            sqlStringWeight &= " AND conferimenti.dataora >= '" & dateStart.ToString("s") & "'"
        End If

        If dateEnd <> Date.MinValue Then
            sqlStringUnits &= " AND conferimenti.dataora <= '" & dateEnd.ToString("s") & "'"
            sqlStringWeight &= " AND conferimenti.dataora <= '" & dateEnd.ToString("s") & "'"
        End If

        sqlString = sqlString.Replace("[NUMERO_NUCLEI]", "(" & sqlStringUnits & ") AS numeronuclei")
        sqlString = sqlString.Replace("[TOTALE_PESO]", "(" & sqlStringWeight & ") AS tot_peso")

        If pConnector.AddReader("infostazioni", sqlString) Then
            Dim stations As New List(Of String)

            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infostazioni")

                If sqlReader IsNot Nothing Then
                    Dim numeroNuclei As Integer = 0
                    Integer.TryParse(sqlReader("numeronuclei").ToString, numeroNuclei)
                    Dim peso As Double = 0.0
                    Double.TryParse(sqlReader("tot_peso").ToString, peso)

                    Dim linkDetail As String = String.Empty
                    If Not singleCity Then
                        linkDetail = "<a class=\""btn btn-sm btn-block btn-info\"" href=\""" _
                                   & HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                   & oEcofilUser.IdUtenteEncrypted & "-" _
                                   & HttpContext.GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower & "-" _
                                   & sqlReader("idComune").ToString & "-" _
                                   & HttpContext.GetGlobalResourceObject("controls", "summary_stations").ToString.ToLower & "-" _
                                   & sqlReader("idStazione").ToString & "\"">" _
                                   & HttpContext.GetGlobalResourceObject("controls", "details").ToString & "</a>"
                    Else
                        linkDetail = "<a class=\""btn btn-sm btn-block btn-info\"" href=\""" _
                                   & HttpContext.GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                   & oEcofilUser.IdUtenteEncrypted & "-" _
                                   & HttpContext.GetGlobalResourceObject("controls", "summary_stations").ToString.ToLower & "-" _
                                   & sqlReader("idStazione").ToString & "\"">" _
                                   & HttpContext.GetGlobalResourceObject("controls", "details").ToString & "</a>"
                    End If

                    stations.Add("[""" & sqlReader("idStazione").ToString & """,""" _
                                       & sqlReader("nome").ToString & """,""" _
                                       & sqlReader("via").ToString & """,""" _
                                       & numeroNuclei.ToString & """,""" _
                                       & peso.ToString("0.00") & """,""" _
                                       & linkDetail & """]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("infostazioni")

            strResult = "[" & String.Join(",", stations) & "]"
        End If

        Return strResult
    End Function

    Public Shared Function GetStationsMap(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT latitudine, " _
                                & "longitudine, " _
                                & "via " _
                                & "FROM infostazioni "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then FunctionsClass.InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("infostazioni", sqlString) Then
            Dim points As New List(Of String)
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infostazioni")

                If sqlReader IsNot Nothing Then
                    Dim latitudine As Double = 0.0
                    Double.TryParse(sqlReader("latitudine").ToString, latitudine)
                    Dim longitudine As Double = 0.0
                    Double.TryParse(sqlReader("longitudine").ToString, longitudine)

                    points.Add("{""latitudine"":" & latitudine.ToString.Replace(",", ".") & "," _
                              & """longitudine"":" & longitudine.ToString.Replace(",", ".") & "," _
                              & """title"":""" & sqlReader("via").ToString & """}")
                Else
                    Exit Do
        End If
        Loop

            pConnector.DelReader("infostazioni")

            strResult = "[" & String.Join(",", points) & "]"
        End If

        Return strResult
    End Function

    Public Shared Function GetStationData(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal stationId As Integer) As Hashtable
        Dim hashResult As New Hashtable

        hashResult.Add("station-id", "0")
        hashResult.Add("station-code", String.Empty)
        hashResult.Add("station-address", String.Empty)
        hashResult.Add("station-latitude", "0.0")
        hashResult.Add("station-longitude", "0.0")
        Try
            Dim sqlString As String = "SELECT * FROM infostazioni " _
                                    & "WHERE idcomune = " & cityId.ToString & " " _
                                    & "AND id = " & stationId.ToString

            If pConnector.AddReader("infostazioni", sqlString) Then
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infostazioni")

                hashResult("station-id") = sqlReader("id").ToString
                hashResult("station-code") = sqlReader("codicestazione").ToString
                hashResult("station-address") = sqlReader("via").ToString
                hashResult("station-latitude") = sqlReader("latitudine").ToString
                hashResult("station-longitude") = sqlReader("longitudine").ToString

                pConnector.DelReader("infostazioni")
            End If
        Catch ex As Exception
            '...
        End Try

        Return hashResult
    End Function
    'Funzione che ricava i dati della stazione per la dashboard del sito ecofilgreen
    Public Shared Function GetStationsDataTable(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, ByVal lang As String) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT * FROM infostazioni "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then FunctionsClass.InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("infostazioni", sqlString) Then
            Dim stations As New List(Of String)
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infostazioni")

                If sqlReader IsNot Nothing Then
                    Dim linkDetail As String = "<a class=\""btn btn-xs btn-block btn-info\"" " _
                                             & "href=\""" & HttpContext.GetGlobalResourceObject("pages", "station_link").ToString.Replace("~/", "") _
                                                                                             & "?city=" & HttpContext.Current.Server.UrlEncode(sqlReader("idcomune").ToString) _
                                                                                             & "&action=" & HttpContext.Current.Server.UrlEncode("UPD") _
                                                                                             & "&id=" & HttpContext.Current.Server.UrlEncode(sqlReader("id").ToString) & "\"">" _
                                             & "<i class=\""fa fa-edit\""></i>" _
                                             & "</a>"

                    Dim linkDelete As String = "<a class=\""btn btn-xs btn-block btn-danger\"" " _
                                             & "data-toggle=\""modal\"" " _
                                             & "href=\""#modal_message_delete\"" " _
                                             & "onclick=\""SelectStationId('" & HttpContext.GetGlobalResourceObject("pages", "station_link").ToString.Replace("~/", "") _
                                                                                                                 & "?city=" & HttpContext.Current.Server.UrlEncode(sqlReader("idcomune").ToString) _
                                                                                                                 & "&action=" & HttpContext.Current.Server.UrlEncode("DEL") _
                                                                                                                 & "&id=" & HttpContext.Current.Server.UrlEncode(sqlReader("id").ToString) & "','" _
                                                                                                                 & lang & "');\"">" _
                                             & "<i class=\""fa fa-trash-o\""></i>" _
                                             & "</a>"

                    stations.Add("[""" & sqlReader("codicestazione").ToString & """,""" _
                                       & sqlReader("nome").ToString & """,""" _
                                       & sqlReader("via").ToString & """,""" _
                                       & linkDetail & linkDelete & """]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("infostazioni")

            strResult = "[" & String.Join(",", stations) & "]"
        End If

        Return strResult
    End Function

    Public Shared Function Save(ByVal cityId As Integer, ByVal stationId As Integer, ByRef hashValues As Hashtable, ByVal connectionString As String) As StationsData
        Dim sqlString As String = String.Empty

        Dim datResponse As New StationsData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = StationsDataEnum.Null

        '** Controllo codice
        Dim intCode As Integer = 0
        If Not Integer.TryParse(hashValues("station-code").ToString, intCode) Then
            datResponse.Message = StationsDataEnum.EmptyCode
            Return datResponse
        End If

        '** Controllo indirizzo
        If hashValues("station-address").ToString = String.Empty Then
            datResponse.Message = StationsDataEnum.EmptyAddress
            Return datResponse
        End If

        '** Reperimento latitudine
        Dim dblLatitude As Double = 0.0
        Double.TryParse(hashValues("station-latitude").ToString, dblLatitude)

        '** Reperimento longitudine
        Dim dblLongitude As Double = 0.0
        Double.TryParse(hashValues("station-longitude").ToString, dblLongitude)

        '** Controllo coordinate
        Dim infosResult As FunctionsClass.GeoInfos = FunctionsClass.GetLocationLatLon("address=" & hashValues("station-address").ToString)
        If infosResult.Latitudine > 0.0 And infosResult.Longitudine > 0.0 Then
            dblLatitude = infosResult.Latitudine
            dblLongitude = infosResult.Longitudine
        End If

        '** Connessione al database
        Dim pConnector As New PostgreConnector(connectionString)

        Try
            If pConnector.Status Then
                If stationId = 0 Then
                    '** Controllo se il codice stazione è già utilizzato
                    If CheckStationCode(pConnector, cityId, FunctionsClass.CheckString(hashValues("station-code").ToString)) Then
                        pConnector.Dispose()

                        datResponse.Message = StationsDataEnum.StationCodeAlreadyExists

                        Return datResponse
                    End If

                    '** Creazione nuova stazione
                    sqlString = "INSERT INTO infostazioni " _
                              & "(codicestazione, " _
                              & "idcomune, " _
                              & "latitudine, " _
                              & "longitudine, " _
                              & "via) " _
                              & "VALUES(" & intCode.ToString & ", " _
                              & cityId.ToString & ", '" _
                              & dblLatitude.ToString("0.00").Replace(",", ".") & ", " _
                              & dblLongitude.ToString("0.00").Replace(",", ".") & ", '" _
                              & FunctionsClass.CheckString(hashValues("station-address").ToString) & "')"

                    If pConnector.ExecuteCommand(sqlString) Then
                        datResponse.RedirectLink = HttpContext.GetGlobalResourceObject("pages", "city_link").ToString & "?city=" & cityId.ToString
                        datResponse.Message = StationsDataEnum.Created
                    Else
                        datResponse.Message = StationsDataEnum.ErrorSavingStation
                    End If
                Else
                    '** Aggioramento dati stazione
                    sqlString = "UPDATE infostazioni " _
                              & "SET latitudine = " & dblLatitude.ToString("0.00").Replace(",", ".") & ", " _
                              & "longitudine = " & dblLongitude.ToString("0.00").Replace(",", ".") & ", " _
                              & "via = '" & FunctionsClass.CheckString(hashValues("station-address").ToString) & "' " _
                              & "WHERE idcomune = " & cityId.ToString & " " _
                              & "AND id = " & stationId.ToString

                    If pConnector.ExecuteCommand(sqlString) Then
                        datResponse.Message = StationsDataEnum.Updated
                    Else
                        datResponse.Message = StationsDataEnum.ErrorSavingStation
                    End If
                End If
            Else
                datResponse.Message = StationsDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = StationsDataEnum.ErrorGeneric
        Finally
            pConnector.Dispose()
        End Try

        Return datResponse
    End Function

    Public Shared Function DeleteStation(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal stationId As Integer) As DeleteResultEnum
        Dim sqlString As String = "DELETE FROM infostazioni " _
                                & "WHERE idcomune = " & cityId.ToString & " " _
                                & "AND id = " & stationId

        If pConnector.ExecuteCommand(sqlString) Then
            Return DeleteResultEnum.DeletedAll
        Else
            Return DeleteResultEnum.StationIdNotValid
        End If
    End Function

    Public Structure StationsInfos
        Dim Nome As String
        Dim Via As String
    End Structure

    Public Structure StationsData
        Public RedirectLink As String
        Public Message As StationsDataEnum
    End Structure

    Public Enum StationsDataEnum
        Created
        Updated
        EmptyCode
        EmptyAddress
        StationCodeAlreadyExists
        ErrorSavingStation
        ErrorDataBase
        ErrorGeneric
        Null
    End Enum

    Public Enum DeleteResultEnum
        DeletedAll
        StationIdNotValid
    End Enum
End Class
