Imports Npgsql
Imports Newtonsoft.Json

Public Class FunctionsClass
    Public Const DATATYPE_STRING = "S"
    Public Const DATATYPE_NUMERIC = "N"
    Public Const DATATYPE_DATE = "D"

    Private Const PASSWORD_CHARS_LCASE As String = "abcdefghijklmnopqrstuvwxyz"
    Private Const PASSWORD_CHARS_UCASE As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    Private Const PASSWORD_CHARS_NUMERIC As String = "0123456789"

    Public Shared Function EncryptValue(ByVal value As String) As String
        Dim EncryptedValue As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim ByteValue() As Byte = System.Text.Encoding.UTF8.GetBytes(value)
        ByteValue = EncryptedValue.ComputeHash(ByteValue)
        Dim sb As New System.Text.StringBuilder
        For Each b As Byte In ByteValue
            sb.Append(b.ToString("x2").ToLower())
        Next
        Return sb.ToString
    End Function

    Public Shared Function GeneratePassword(ByVal length As Integer) As String
        Dim strResult As String = String.Empty

        If length < 1 Or length > 128 Then length = 6

        Dim rndGroup As New Random
        Dim rndItem As New Random
        For i As Integer = 1 To length
            Select Case rndGroup.Next(3)
                Case 0
                    strResult &= PASSWORD_CHARS_LCASE.Substring(rndItem.Next(26), 1)
                Case 1
                    strResult &= PASSWORD_CHARS_UCASE.Substring(rndItem.Next(26), 1)
                Case 2
                    strResult &= PASSWORD_CHARS_NUMERIC.Substring(rndItem.Next(10), 1)
            End Select
        Next

        Return strResult
    End Function


    Public Shared Function CheckString(ByVal str As String) As String
        Return str.Replace("'", "''").Replace("*", " ").Replace("%", " ").Replace(";", " ").Replace("--", " ").Replace("<", " ").Replace(">", " ").Replace("#", "").Replace("=", "")
    End Function

    Public Shared Function CleanStringURL(ByVal str As String, ByVal replacePoint As Boolean) As String
        If replacePoint Then str = str.Replace(".", "")

        Return str.Replace(";", "").Replace("/", "").Replace("?", "").Replace(":", "").Replace("@", "").Replace("&", "").Replace("=", "").Replace("+", "").Replace("$", "") _
                  .Replace(" ", "").Replace("<", "").Replace(">", "").Replace("#", "").Replace("%", "").Replace("""", "").Replace(",", "").Replace("{", "").Replace("}", "") _
                  .Replace("|", "").Replace("\", "").Replace("^", "").Replace("[", "").Replace("]", "").Replace("'", "")
    End Function

    Public Shared Sub DisableSubmit(ByRef textboxObj As TextBox)
        textboxObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); if (ComFun_DisableSubmit()) { return false; }")
    End Sub

    Public Shared Sub DisableSubmit(ByRef dropdownObj As DropDownList)
        dropdownObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); if (ComFun_DisableSubmit()) { return false; }")
    End Sub

    Public Shared Sub DisableSubmit(ByRef checkboxObj As CheckBox)
        checkboxObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); if (ComFun_DisableSubmit()) { return false; }")
    End Sub

    Public Shared Sub LaunchSubmit(ByRef textboxObj As TextBox, ByVal buttonId As String)
        textboxObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); ComFun_LaunchSubmit('" & buttonId & "');")
    End Sub

    Public Shared Sub LaunchSubmit(ByRef dropdownObj As DropDownList, ByVal buttonId As String)
        dropdownObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); ComFun_LaunchSubmit('" & buttonId & "');")
    End Sub

    Public Shared Sub LaunchSubmit(ByRef checkboxObj As CheckBox, ByVal buttonId As String)
        checkboxObj.Attributes.Add("onkeydown", "ComFun_GetEvent(event); ComFun_LaunchSubmit('" & buttonId & "');")
    End Sub

    Public Shared Sub CreateFormField(ByRef fieldType As FormFieldTypeEnum, ByVal fieldId As String, ByVal fieldValue As String, ByVal fieldLabel As String, ByVal fieldClass As String, ByVal fieldLabelClass As String, ByRef fieldContainer As Panel)
        Select Case fieldType
            Case FormFieldTypeEnum.TextField
                Dim panelContainer As New Panel
                panelContainer.CssClass = "form-group"

                If fieldLabel <> String.Empty Then
                    Dim literalLabel As New Literal
                    literalLabel.Text = "<label for=""" & fieldId & """ class=""" & fieldLabelClass & " control-label"">" & fieldLabel & "</asp:Literal></label>"

                    panelContainer.Controls.Add(literalLabel)
                End If

                Dim panelGroup As New Panel
                panelGroup.CssClass = fieldClass

                Dim textObj As New TextBox
                textObj.ID = fieldId
                textObj.Text = fieldValue
                textObj.CssClass = "form-control"
                FunctionsClass.DisableSubmit(textObj)

                panelGroup.Controls.Add(textObj)

                panelContainer.Controls.Add(panelGroup)

                fieldContainer.Controls.Add(panelContainer)
            Case FormFieldTypeEnum.HiddenField
                Dim hiddenObj As New HiddenField
                hiddenObj.ID = fieldId
                hiddenObj.Value = fieldValue

                fieldContainer.Controls.Add(hiddenObj)
        End Select
    End Sub

    Public Shared Function GetTotalStation(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim sqlString As String = "SELECT COUNT(*) FROM infostazioni WHERE idcomune > 0"
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        Return pConnector.ExecuteScalar(sqlString)
    End Function

    Public Shared Function GetTotalStationInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_stations").ToString & "</h5>"

        Dim intStations As Integer = GetTotalStation(pConnector, hashParms)

        If intStations > 0 Then
            strResult &= "<p>" & intStations.ToString & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetActiveStation(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim intResult As Integer = 0

        Dim sqlString As String = "SELECT COUNT(DISTINCT codicestazione) AS stazioni_attive FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Integer.TryParse(sqlReader("stazioni_attive").ToString, intResult)

            pConnector.DelReader("conferimenti")
        End If

        Return intResult
    End Function

    Public Shared Function GetTotalCities(ByRef pConnector As PostgreConnector, ByVal userId As Integer) As Integer
        Dim sqlString As String = "SELECT COUNT([DISTINCT] idcomune) FROM utentiwebcomuni" _
                                & "INNER JOIN infocomune ON utentiwebcomuni.idcomune = infocomune.id" _

        If userId > 0 Then
            sqlString = sqlString.Replace("[DISTINCT]", String.Empty)
            sqlString &= " WHERE utentiwebcomuni.id = " & userId.ToString
        Else
            sqlString = sqlString.Replace("[DISTINCT]", "DISTINCT")
        End If


        Return pConnector.ExecuteScalar(sqlString)
    End Function

    Public Shared Function GetTotalUsers(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim sqlString As String = "SELECT COUNT(*) FROM datinuclei "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        Return pConnector.ExecuteScalar(sqlString)
    End Function

    Public Shared Function GetTotalCitizen(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim sqlString As String = "SELECT SUM(numeroabitanti) FROM infocomune "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        Return pConnector.ExecuteScalar(sqlString)
    End Function

    Public Shared Function GetTotalContributions(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim intResult As Integer = 0

        Dim sqlString As String = "SELECT COUNT(*) AS totale_conferimenti FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Integer.TryParse(sqlReader("totale_conferimenti").ToString, intResult)

            pConnector.DelReader("conferimenti")
        End If

        Return intResult
    End Function

    Public Shared Function GetTotalGarbage(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT SUM(peso) AS totale_peso FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Double.TryParse(sqlReader("totale_peso").ToString, dblResult)

            pConnector.DelReader("conferimenti")
        End If

        Return dblResult
    End Function

    Public Shared Function GetTotalRD(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlstring1 As String = "SELECT 'tc' AS flag_tot, SUM(peso) AS totale_peso FROM conferimenti " _
                                 & "INNER JOIN associazionenuclei ON conferimenti.idcomune = associazionenuclei.idcomune AND UPPER(conferimenti.codicetag) = UPPER(associazionenuclei.codicetag) " _
                                 & "INNER JOIN datinuclei ON associazionenuclei.idcomune = datinuclei.idcomune AND associazionenuclei.codicenucleo = datinuclei.codicenucleo "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlstring1, hashParms)

        Dim sqlstring2 As String = "SELECT 'tci' AS flag_tot, SUM(peso) AS totale_peso FROM conferimenti " _
                                 & "INNER JOIN associazionenuclei ON conferimenti.idcomune = associazionenuclei.idcomune AND UPPER(conferimenti.codicetag) = UPPER(associazionenuclei.codicetag) " _
                                 & "INNER JOIN datinuclei ON associazionenuclei.idcomune = datinuclei.idcomune AND associazionenuclei.codicenucleo = datinuclei.codicenucleo "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlstring2, hashParms)
        If Not sqlstring2.Contains("WHERE") Then
            sqlstring2 &= " WHERE"
        Else
            sqlstring2 &= " AND"
        End If
        sqlstring2 &= " tiporifiuto = 0"

        Dim tc As Double = 0.0
        Dim tci As Double = 0.0

        If pConnector.AddReader("conferimenti", sqlstring1 & " UNION " & sqlstring2) Then
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    Select Case sqlReader("flag_tot").ToString
                        Case "tc"
                            Double.TryParse(sqlReader("totale_peso").ToString, tc)
                        Case "tci"
                            Double.TryParse(sqlReader("totale_peso").ToString, tci)
                    End Select
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")
        End If

        If tc > 0 Then dblResult = ((tc - tci) / tc) * 100

        'Dim sqlString As String = "SELECT rdpercentuale FROM classifiche "
        'If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        'If pConnector.AddReader("classifiche", sqlString) Then
        '    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

        '    Double.TryParse(sqlReader("rdpercentuale").ToString, dblResult)
        '    dblResult = dblResult * 100

        '    pConnector.DelReader("classifiche")
        'End If

        Return dblResult
    End Function

    Public Shared Function GetTotalRDByCity(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlstring1 As String = "SELECT 'tc' AS flag_tot, SUM(peso) AS totale_peso FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlstring1, hashParms)

        Dim sqlstring2 As String = "SELECT 'tci' AS flag_tot, SUM(peso) AS totale_peso FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlstring2, hashParms)
        If Not sqlstring2.Contains("WHERE") Then
            sqlstring2 &= " WHERE"
        Else
            sqlstring2 &= " AND"
        End If
        sqlstring2 &= " tiporifiuto = 0"

        Dim tc As Double = 0.0
        Dim tci As Double = 0.0

        If pConnector.AddReader("conferimenti", sqlstring1 & " UNION " & sqlstring2) Then
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    Select Case sqlReader("flag_tot").ToString
                        Case "tc"
                            Double.TryParse(sqlReader("totale_peso").ToString, tc)
                        Case "tci"
                            Double.TryParse(sqlReader("totale_peso").ToString, tci)
                    End Select
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")
        End If

        If tc > 0 Then dblResult = ((tc - tci) / tc) * 100

        'Dim sqlString As String = "SELECT AVG(rdpercentuale) AS media_rdpercentuale FROM datinuclei " _
        '                        & "INNER JOIN classifiche ON datinuclei.id = classifiche.idnucleo "
        'If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        'If pConnector.AddReader("classifiche", sqlString) Then
        '    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

        '    Double.TryParse(sqlReader("media_rdpercentuale").ToString, dblResult)
        '    dblResult = dblResult * 100

        '    pConnector.DelReader("classifiche")
        'End If

        Return dblResult
    End Function

    Public Shared Function GetTotalGarbagesType(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Hashtable
        Dim hashResult As New Hashtable

        Dim sqlString As String = "SELECT SUM(peso) AS peso, tiporifiuto FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "GROUP BY tiporifiuto"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    hashResult.Add(sqlReader("tiporifiuto").ToString, CType(sqlReader("peso"), Double))
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")
        End If

        Return hashResult
    End Function

    Public Shared Function GetValuesGarbagesType(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Hashtable
        Dim hashResult As New Hashtable

        Dim sqlString As String = "SELECT peso, tiporifiuto FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "ORDER BY tiporifiuto, dataora"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim checkRifiuto As String = String.Empty
            Dim pesi As List(Of Double) = Nothing

            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    If checkRifiuto <> sqlReader("tiporifiuto").ToString Then
                        If checkRifiuto <> String.Empty Then hashResult(checkRifiuto) = pesi

                        hashResult.Add(sqlReader("tiporifiuto").ToString, Nothing)
                        pesi = New List(Of Double)

                        checkRifiuto = sqlReader("tiporifiuto").ToString
                    End If

                    pesi.Add(CType(sqlReader("peso"), Double))
                Else
                    Exit Do
                End If
            Loop

            If checkRifiuto <> String.Empty Then hashResult(checkRifiuto) = pesi

            pConnector.DelReader("conferimenti")
        End If

        Return hashResult
    End Function

    Public Shared Function GetLastGarbageWeight(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT conferimenti.peso AS peso FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Double.TryParse(sqlReader("peso"), dblResult)

            pConnector.DelReader("conferimenti")
        End If

        Return dblResult
    End Function

    Public Shared Function GetLastGarbageUser(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT AVG(peso) AS media_peso FROM (SELECT SUM(peso) AS peso, codicetag FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "GROUP BY codicetag) AS media_rifiuti_nucleo"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Double.TryParse(sqlReader("media_peso"), dblResult)

            pConnector.DelReader("conferimenti")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageByUser(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT mediaprocapite FROM classifiche "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("classifiche", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

            Double.TryParse(sqlReader("mediaprocapite").ToString, dblResult)

            pConnector.DelReader("classifiche")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageByUserCity(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT AVG(mediaprocapite) AS mediaprocapite " _
                                & "FROM datinuclei " _
                                & "INNER JOIN classifiche ON datinuclei.id = classifiche.idnucleo "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("classifiche", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

            Double.TryParse(sqlReader("mediaprocapite").ToString, dblResult)

            pConnector.DelReader("classifiche")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageByCity(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT AVG(peso) AS media_peso FROM (SELECT SUM(peso) AS peso, idcomune FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "GROUP BY idcomune) AS media_peso_comune"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Double.TryParse(sqlReader("media_peso").ToString, dblResult)

            pConnector.DelReader("conferimenti")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageByStation(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT AVG(peso) AS media_peso FROM (SELECT SUM(peso) AS peso, codicestazione FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "GROUP BY codicestazione) AS media_peso_stazione"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Double.TryParse(sqlReader("media_peso").ToString, dblResult)

            pConnector.DelReader("conferimenti")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageGarbagePerc(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double
        Dim dblResult As Double = 0.0

        Dim sqlString As String = "SELECT AVG(rdpercentuale) AS media_differenziata FROM datinuclei " _
                                & "INNER JOIN classifiche ON datinuclei.id = classifiche.idnucleo "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("datinuclei", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("datinuclei")

            Double.TryParse(sqlReader("media_differenziata").ToString, dblResult)
            dblResult = dblResult * 100

            pConnector.DelReader("datinuclei")
        End If

        Return dblResult
    End Function

    Public Shared Function GetAverageProCapite(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Double()
        Dim dblResult() As Double = {0.0, 0.0}

        Dim sqlString As String = "SELECT SUM(peso_nuclei) AS totale_peso, SUM(componenti_nuclei) AS totale_componenti FROM " _
                                & "(SELECT SUM(conferimenti.peso) AS peso_nuclei, datinuclei.ncomponenti AS componenti_nuclei " _
                                & "FROM conferimenti " _
                                & "INNER JOIN associazionenuclei ON UPPER(conferimenti.codicetag) = UPPER(associazionenuclei.codicetag) " _
                                & "INNER JOIN datinuclei ON associazionenuclei.codicenucleo = datinuclei.codicenucleo "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        sqlString &= "GROUP BY associazionenuclei.codicenucleo, datinuclei.ncomponenti) AS tabella_nuclei"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Dim mpc As Double = 0.0

            Dim tp As Double = 0.0
            Double.TryParse(sqlReader("totale_peso").ToString, tp)
            Dim tc As Double = 0.0
            Double.TryParse(sqlReader("totale_componenti").ToString, tc)

            If tc > 0 Then mpc = tp / tc

            pConnector.DelReader("conferimenti")

            Dim days As Integer = GetActiveDays(pConnector, hashParms)

            If days > 0 Then
                dblResult(0) = mpc / days
                dblResult(1) = dblResult(0) * 365
            End If
        End If

        Return dblResult
    End Function

    Public Shared Function GetActiveDays(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim intResult As Integer = 0

        Dim sqlString As String = "SELECT MAX(dataora) AS datafine, MIN(dataora) AS datainizio FROM conferimenti "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            Dim datStart As Date = Date.MinValue
            Date.TryParse(sqlReader("datainizio").ToString, datStart)

            Dim datEnd As Date = Date.MinValue
            Date.TryParse(sqlReader("datafine").ToString, datEnd)

            If datStart <> Date.MinValue Then intResult = datEnd.Subtract(datStart).Days

            pConnector.DelReader("conferimenti")
        End If

        Return intResult
    End Function

    Public Shared Function GetGarbages(ByRef pConnector As PostgreConnector, ByVal idComune As Integer, ByVal livello As Integer) As Hashtable
        Dim hashResult As New Hashtable

        Dim sqlString As String = "SELECT * FROM inforifiuti " _
                                & "WHERE idcomune = " & idComune.ToString & " " _
                                & "AND livello = " & livello.ToString & " " _
                                & "ORDER BY descrizione"

        If pConnector.AddReader("inforifiuti", sqlString) Then
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("inforifiuti")

                If sqlReader IsNot Nothing Then
                    Dim infos As New List(Of String)
                    infos.Add(sqlReader("descrizione").ToString)
                    infos.Add(sqlReader("colore").ToString)

                    hashResult.Add(sqlReader("codicerifiuto").ToString, infos)
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("inforifiuti")
        End If

        Return hashResult
    End Function

    Public Shared Function GetPoints(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim intResult As Double = 0

        Dim sqlString As String = "SELECT puntiacquisiti FROM classifiche "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("classifiche", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

            Integer.TryParse(sqlReader("puntiacquisiti").ToString, intResult)

            pConnector.DelReader("classifiche")
        End If

        Return intResult
    End Function

    Public Shared Function GetRank(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As Integer
        Dim intResult As Integer = 0

        Dim sqlString As String = "SELECT posizione FROM classifiche "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If pConnector.AddReader("classifiche", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("classifiche")

            Integer.TryParse(sqlReader("posizione").ToString, intResult)

            pConnector.DelReader("classifiche")
        End If

        Return intResult
    End Function

    Public Shared Function GetLastConfInfos(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "last_conferment_infos").ToString & "</h5>"

        Dim sqlString As String = "SELECT infostazioni.via AS via, " _
                                & "conferimenti.dataora AS dataora " _
                                & "FROM conferimenti " _
                                & "INNER JOIN infostazioni ON conferimenti.codicestazione = infostazioni.codicestazione "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= "ORDER BY conferimenti.dataora DESC " _
                   & "LIMIT 1"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

            strResult &= "<p>" & sqlReader("via").ToString & " " & CType(sqlReader("dataora"), Date).ToString("dd/MM/yyyy HH:mm:ss") & "</p>"

            pConnector.DelReader("conferimenti")
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetMonthContributionsInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "average_days").ToString & "</h5>"

        Dim intAverage As Integer = GetTotalContributions(pConnector, hashParms) / 12

        strResult &= "<p>" & intAverage.ToString() & "</p>"

        Return strResult
    End Function

    Public Shared Function GetMonthGarbageInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "average_days").ToString & "</h5>"

        Dim dblAverage As Double = GetTotalGarbage(pConnector, hashParms) / 12

        If dblAverage > 0.0 Then
            strResult &= "<p>" & dblAverage.ToString("0.00") & " Kg</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetTotalGarbageInfos(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_garbage").ToString & "</h5>"

        Dim dblWeight As Double = GetTotalGarbage(pConnector, hashParms)

        If dblWeight > 0.0 Then
            strResult &= "<p>" & dblWeight.ToString("0.00") & " Kg" & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetAverageInfoByUserCity(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_user_average").ToString & "</h5>"

        Dim dblAverage As Double = GetAverageByUserCity(pConnector, hashParms)

        If dblAverage > 0.0 Then
            strResult &= "<p>" & dblAverage.ToString("0.00") & " Kg" & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetAverageInfoByCity(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_city_average").ToString & "</h5>"

        Dim dblAverage As Double = GetAverageByCity(pConnector, hashParms)

        If dblAverage > 0.0 Then
            strResult &= "<p>" & dblAverage.ToString("0.00") & " Kg" & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetAverageInfoByStation(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_station_average").ToString & "</h5>"

        Dim dblAverage As Double = GetAverageByStation(pConnector, hashParms)

        If dblAverage > 0.0 Then
            strResult &= "<p>" & dblAverage.ToString("0.00") & " Kg" & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetRankInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_position").ToString & "</h5>"

        Dim intRank As Integer = GetRank(pConnector, hashParms)

        If intRank > 0 Then
            strResult &= "<p>" & intRank.ToString & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetActiveStationInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "active_stations").ToString & "</h5>"

        Dim intActiveStations As Integer = GetActiveStation(pConnector, hashParms)

        If intActiveStations > 0 Then
            strResult &= "<p>" & intActiveStations.ToString & "</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetAverageProCapiteInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_user_average").ToString & "</h5>"

        Dim dblAverageProCapite() As Double = GetAverageProCapite(pConnector, hashParms)

        strResult &= "<p>" _
                   & dblAverageProCapite(0).ToString("0.00") & " " & HttpContext.GetGlobalResourceObject("controls", "summary_user_average_day").ToString _
                   & " | " _
                   & dblAverageProCapite(1).ToString("0.00") & " " & HttpContext.GetGlobalResourceObject("controls", "summary_user_average_day").ToString _
                   & "</p>"

        Return strResult
    End Function

    Public Shared Function GetRDByCityInfo(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_rd_city").ToString & "</h5>"

        Dim dblTotalRDByCity As Double = GetTotalRDByCity(pConnector, hashParms)

        If dblTotalRDByCity > 0.0 Then
            strResult &= "<p>" & dblTotalRDByCity.ToString("0") & " %</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetRDGlobalInfo(ByRef pConnector As PostgreConnector) As String
        Dim strResult As String = "<h5>" & HttpContext.GetGlobalResourceObject("controls", "summary_rd_global").ToString & "</h5>"

        Dim dblTotalRDByCity As Double = GetTotalRDByCity(pConnector, Nothing)

        If dblTotalRDByCity > 0.0 Then
            strResult &= "<p>" & dblTotalRDByCity.ToString("0") & " %</p>"
        Else
            strResult &= "<p>...</p>"
        End If

        Return strResult
    End Function

    Public Shared Function GetTimelineData(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable) As String
        Dim strResult As String = "[]"

        Dim sqlString As String = "SELECT peso, dataora FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)
        sqlString &= " ORDER BY dataora"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim points As New List(Of String)
            Dim pointValue As Double = 0.0
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    Dim timeValue As New TimeSpan(CType(sqlReader("dataora"), Date).Ticks)
                    Dim xValue As Long = timeValue.TotalMilliseconds

                    pointValue = CType(sqlReader("peso"), Double)

                    points.Add("[" & xValue.ToString & "," & pointValue.ToString("0.00").Replace(",", ".") & "]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")

            strResult = "[{data: [" & String.Join(",", points) & "], color: dark}]"
        End If

        Return strResult
    End Function

    Public Shared Function GetTimelineGarbageTypesData(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, ByRef garbageList As Hashtable, ByVal garbageSelect As String, Optional sumValues As Boolean = False) As String
        Dim strResult As String = "[]"

        Dim listResult As New List(Of String)

        Dim garbageLabel As String = "tot"
        Dim garbageColor As String = "dark"

        Dim sqlString As String = "SELECT peso, dataora FROM conferimenti "
        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms)

        If garbageSelect <> String.Empty And garbageList.Contains(garbageSelect) Then
            If Not sqlString.Contains("WHERE") Then
                sqlString &= " WHERE"
            Else
                sqlString &= " AND"
            End If
            sqlString &= " tiporifiuto = " & garbageSelect

            garbageLabel = CType(garbageList(garbageSelect), List(Of String))(0)
            garbageColor = CType(garbageList(garbageSelect), List(Of String))(1)
        End If
        sqlString &= " ORDER BY dataora"

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim points As New List(Of String)
            Dim pointValue As Double = 0.0
            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    Dim timeValue As New TimeSpan(CType(sqlReader("dataora"), Date).Ticks)
                    Dim xValue As Long = timeValue.TotalMilliseconds

                    If sumValues Then
                        pointValue += CType(sqlReader("peso"), Double)
                    Else
                        pointValue = CType(sqlReader("peso"), Double)
                    End If

                    points.Add("[" & xValue.ToString & "," & pointValue.ToString("0.00").Replace(",", ".") & "]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")

            listResult.Add("{label: """ & garbageLabel & """, data: [" & String.Join(",", points) & "], color: " & garbageColor & "}")
        End If

        If listResult.Count > 0 Then strResult = "[" & String.Join(",", listResult) & "]"

        Return strResult
    End Function

    Public Shared Function GetContributionsData(ByRef pConnector As PostgreConnector, ByRef hashParms As Hashtable, Optional exceptions As List(Of String) = Nothing) As String
        Dim strResult As String = "null"

        Dim sqlString As String = "SELECT conferimenti.dataora AS dataora, " _
                                & "infostazioni.via AS via, " _
                                & "[DESCRIZIONE_RIFIUTO], " _
                                & "conferimenti.peso AS peso " _
                                & "FROM conferimenti " _
                                & "INNER JOIN infostazioni ON conferimenti.idcomune = infostazioni.idcomune " _
                                & "AND conferimenti.codicestazione = infostazioni.codicestazione "

        If hashParms IsNot Nothing AndAlso hashParms.Count > 0 Then InsertWhereConditions(sqlString, hashParms, exceptions)
        sqlString &= "ORDER BY conferimenti.dataora"

        sqlString = sqlString.Replace("[DESCRIZIONE_RIFIUTO]", "(SELECT descrizione FROM inforifiuti WHERE idcomune = conferimenti.idcomune AND codicerifiuto = conferimenti.tiporifiuto) AS descrizione")

        If pConnector.AddReader("conferimenti", sqlString) Then
            Dim contributions As New List(Of String)

            Do While True
                Dim sqlReader As NpgsqlDataReader = pConnector.GetData("conferimenti")

                If sqlReader IsNot Nothing Then
                    contributions.Add("[""" & CType(sqlReader("dataora"), Date).ToString("yyyy-MM-dd, HH:mm:ss") & """,""" _
                                            & sqlReader("via").ToString & """,""" _
                                            & sqlReader("descrizione").ToString & """,""" _
                                            & CType(sqlReader("peso"), Double).ToString("0.00") & """]")
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("conferimenti")

            strResult = "[" & String.Join(",", contributions) & "]"
        End If

        Return strResult
    End Function

    Public Shared Sub InsertWhereConditions(ByRef sqlString As String, ByRef hashParms As Hashtable, Optional exceptions As List(Of String) = Nothing)
        For Each hashParm As DictionaryEntry In hashParms
            Dim fieldName As String = String.Empty

            If exceptions Is Nothing OrElse (exceptions IsNot Nothing And Not exceptions.Contains(hashParm.Key)) Then
                If hashParm.Key.ToString.Contains("#") Then
                    fieldName = hashParm.Key.ToString.Split("#")(0)
                Else
                    fieldName = hashParm.Key.ToString
                End If

                If sqlString.Contains("WHERE") Then
                    sqlString &= "AND "
                Else
                    sqlString &= "WHERE "
                End If

                Dim valueParms() As String
                If hashParm.Value.ToString.Contains(";") Then
                    valueParms = hashParm.Value.ToString.Split(";")
                Else
                    valueParms = {hashParm.Value.ToString, "S"}
                End If

                If valueParms(0).Contains("#") Then
                    Dim parmsItems() As String = valueParms(0).Split("#")

                    sqlString &= "("

                    For Each parmsItem As String In parmsItems
                        If Not sqlString.EndsWith("(") Then sqlString &= "OR "

                        sqlString &= fieldName & " " & valueParms(2) & " "
                        Select Case valueParms(1)
                            Case FunctionsClass.DATATYPE_STRING, FunctionsClass.DATATYPE_DATE
                                sqlString &= "'" & parmsItem & "' "
                            Case FunctionsClass.DATATYPE_NUMERIC
                                sqlString &= parmsItem & " "
                        End Select
                    Next

                    sqlString &= ") "
                Else
                    sqlString &= fieldName & " " & valueParms(2) & " "
                    Select Case valueParms(1)
                        Case FunctionsClass.DATATYPE_STRING, FunctionsClass.DATATYPE_DATE
                            sqlString &= "'" & valueParms(0) & "' "
                        Case FunctionsClass.DATATYPE_NUMERIC
                            sqlString &= valueParms(0) & " "
                    End Select
                End If
            End If
        Next
    End Sub

    Public Shared Function HashToString(ByRef hashArray As Hashtable, ByVal valuesType As ValuesTypeEnum) As String
        Dim strResult As String = "null"

        Dim listValue As New List(Of String)

        For Each itemArray As DictionaryEntry In hashArray
            Select Case valuesType
                Case ValuesTypeEnum.STRING_VALUE
                    listValue.Add("[" & itemArray.Key.ToString & ",""" & itemArray.Value.ToString & """]")
                Case ValuesTypeEnum.STRING_ARRAY
                    Dim stringArray As New List(Of String)

                    For Each stringValue As String In CType(itemArray.Value, List(Of String))
                        stringArray.Add("""" & stringValue & """")
                    Next

                    listValue.Add("[" & itemArray.Key.ToString & "," & "[" & String.Join(",", stringArray) & "]]")
                Case ValuesTypeEnum.INTEGER_VALUE
                    listValue.Add("[" & itemArray.Key.ToString & "," & itemArray.Value.ToString & "]")
                Case ValuesTypeEnum.INTEGER_ARRAY
                    listValue.Add("[" & itemArray.Key.ToString & "," & "[" & String.Join(",", CType(itemArray.Value, List(Of Integer))) & "]]")
                Case ValuesTypeEnum.DOUBLE_VALUE
                    listValue.Add("[" & itemArray.Key.ToString & "," & CType(itemArray.Value, Double).ToString("0.00").Replace(",", ".") & "]")
                Case ValuesTypeEnum.DOUBLE_ARRAY
                    Dim doubleArray As New List(Of String)

                    For Each doubleValue As Double In CType(itemArray.Value, List(Of Double))
                        doubleArray.Add(doubleValue.ToString("0.00").Replace(",", "."))
                    Next

                    listValue.Add("[" & itemArray.Key.ToString & "," & "[" & String.Join(",", doubleArray) & "]]")
                Case ValuesTypeEnum.DATA_VALUE
                    Dim timeValue As New TimeSpan(CType(itemArray.Value, Date).Ticks)
                    Dim millisecondsValue As Long = timeValue.TotalMilliseconds

                    listValue.Add("[" & itemArray.Key.ToString & "," & millisecondsValue.ToString & "]")
                Case ValuesTypeEnum.DATA_ARRAY
                    Dim millisecondsArray As New List(Of Long)

                    For Each dataValue As Date In CType(itemArray.Value, List(Of Date))
                        Dim timeValue As New TimeSpan(CType(dataValue, Date).Ticks)
                        Dim millisecondsValue As Long = timeValue.TotalMilliseconds

                        millisecondsArray.Add(millisecondsValue)
                    Next

                    listValue.Add("[" & itemArray.Key.ToString & "," & "[" & String.Join(",", millisecondsArray) & "]]")
            End Select
        Next

        strResult = "[" & String.Join(",", listValue) & "]"

        Return strResult
    End Function

    Public Shared Function HashToStructure(ByRef hashLabelArray As Hashtable, ByRef hashArray As Hashtable) As String
        Dim strResult As String = "null"

        Dim listValue As New List(Of String)

        Dim totalValue As Double = 0.0
        Dim valueItems As New Hashtable

        For Each itemArray As DictionaryEntry In hashArray
            Dim totValueItem As Double = 0.0

            For Each doubleValue As Double In CType(itemArray.Value, List(Of Double))
                totValueItem += doubleValue
            Next

            totalValue += totValueItem
            valueItems.Add(itemArray.Key.ToString, totValueItem)
        Next

        Dim checkTotal As Integer = 100
        Dim percentageItems As New Hashtable

        For Each valueItem As DictionaryEntry In valueItems
            Dim percentage As Integer = (CType(valueItem.Value, Double) / totalValue) * 100
            checkTotal -= percentage
            percentageItems.Add(valueItem.Key.ToString, percentage)
        Next

        If checkTotal > 0 Then percentageItems(0) += checkTotal

        For Each itemArray As DictionaryEntry In hashArray
            If hashLabelArray.Contains(itemArray.Key.ToString) Then
                Dim infos As List(Of String) = CType(hashLabelArray(itemArray.Key.ToString), List(Of String))
                If infos(1) = String.Empty Then infos(1) = "dark"

                listValue.Add("{ label: """ & infos(0) & " (" & percentageItems(itemArray.Key.ToString).ToString & "%)"", data: " & percentageItems(itemArray.Key.ToString).ToString & ", color: " & infos(1) & " }")
            End If
        Next

        strResult = "[" & String.Join(",", listValue) & "]"

        Return strResult
    End Function

    Public Shared Function GetCityName(ByRef pConnector As PostgreConnector, ByVal cityId As Integer) As String
        Dim strResult As String = String.Empty

        Dim sqlString As String = "SELECT nome FROM infocomune WHERE id = " & cityId.ToString

        If pConnector.AddReader("infocomune", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

            strResult = sqlReader("nome").ToString

            pConnector.DelReader("infocomune")
        End If

        Return strResult
    End Function

    Public Shared Function GetCityInfos(ByRef pConnector As PostgreConnector, ByVal cityId As Integer) As CityInfos
        Dim cityInfosResult As New CityInfos

        Dim sqlString As String = "SELECT * FROM infocomune WHERE id = " & cityId.ToString

        If pConnector.AddReader("infocomune", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

            cityInfosResult.Nome = sqlReader("nome").ToString
            Double.TryParse(sqlReader("latitudine").ToString, cityInfosResult.Latitudine)
            Double.TryParse(sqlReader("longitudine").ToString, cityInfosResult.Longitudine)

            pConnector.DelReader("infocomune")
        End If

        Return cityInfosResult
    End Function

    Public Shared Function CheckUserName(ByRef pConnector As PostgreConnector, ByVal username As String, Optional ByVal userId As Integer = 0) As Boolean
        Dim sqlstring As String = "SELECT id FROM utentiweb " _
                                & "WHERE UPPER(username) = '" & username.ToUpper & "'" _
                                & IIf(userId > 0, " AND id <> " & userId.ToString, String.Empty).ToString
    
       If pConnector.ExecuteScalar(sqlstring) > 0 Then Return True
    
       Return False
    End Function

    Public Shared Function GetTagsList(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal unitId As Integer) As String
        Dim strResult As String = "<div class=""table-responsive"">" _
                                & "<table class=""table"">" _
                                & "<thead>" _
                                & "<tr>" _
                                & "<th>" & HttpContext.GetGlobalResourceObject("controls", "tags_header_1").ToString & "</th>" _
                                & "<th>" & HttpContext.GetGlobalResourceObject("controls", "tags_header_2").ToString & "</th>" _
                                & "</tr>" _
                                & "</thead>" _
                                & "<tbody>"

        Dim sqlstring As String = "SELECT * FROM datinuclei " _
                                & "INNER JOIN associazionenuclei ON datinuclei.codicenucleo = associazionenuclei.codicenucleo " _
                                & "WHERE datinuclei.idcomune = " & cityId.ToString & " " _
                                & "AND datinuclei.id = " & unitId.ToString & " " _
                                & "ORDER BY codicetag"

        If pConnector.AddReader("associazionenuclei", sqlstring) Then
            Do While True
                Dim sqlReader As Npgsql.NpgsqlDataReader = pConnector.GetData("associazionenuclei")

                If sqlReader IsNot Nothing Then
                    strResult &= "<tr>" _
                               & "<td>" & sqlReader("codicetag").ToString & "</td>" _
                               & "<td>" & sqlReader("info").ToString & "</td>" _
                               & "</tr>"
                Else
                    Exit Do
                End If
            Loop

            pConnector.DelReader("associazionenuclei")
        Else
            strResult &= "<tr>" _
                       & "<td colspan=""3"">" & HttpContext.GetGlobalResourceObject("messages", "message_002").ToString & "</td>" _
                       & "</tr>"
        End If

        strResult &= "</tbody>" _
                   & "</table>" _
                   & "</div>"

        Return strResult
    End Function

    Public Shared Function GetWebUserCoords(ByRef pConnector As PostgreConnector, ByVal userId As Integer) As GeoInfos
        Dim infosResult As New GeoInfos

        Dim sqlString As String = "SELECT AVG(Latitudine) AS acg_lat, AVG(Longitudine) AS avg_lon FROM utentiwebcomuni " _
                                & "INNER JOIN infocomune ON utentiwebcomuni.idcomune = infocomune.id " _
                                & "WHERE id = " & userId.ToString & " " _
                                & "AND idcomune > 0 " _
                                & "AND latitudine > 0 " _
                                & "AND longitudine > 0"

        If pConnector.AddReader("utentiwebcomuni", sqlString) Then
            Dim sqlReader As NpgsqlDataReader = pConnector.GetData("utentiwebcomuni")

            Double.TryParse(sqlReader("acg_lat").ToString, infosResult.Latitudine)
            Double.TryParse(sqlReader("avg_lon").ToString, infosResult.Longitudine)

            pConnector.DelReader("utentiwebcomuni")
        End If

        Return infosResult
    End Function

    Public Shared Function GetLocationLatLon(ByVal parameters As String) As GeoInfos
        Dim infosResult As New GeoInfos
        infosResult.Latitudine = 0.0
        infosResult.Longitudine = 0.0

        Dim requestGoogleLatLon As String = HttpRequestClass.httpGetRequest("http://maps.googleapis.com/maps/api/geocode/json?", parameters & "&sensor=false")

        Dim jsonLatLon As GoogleGeoCodeResponse = JsonConvert.DeserializeObject(Of GoogleGeoCodeResponse)(requestGoogleLatLon)
        Dim status As String = jsonLatLon.status

        If status = "OK" Then
            Double.TryParse(jsonLatLon.results(0).geometry.location.lat, infosResult.Latitudine)
            Double.TryParse(jsonLatLon.results(0).geometry.location.lng, infosResult.Longitudine)
        End If

        Return infosResult
    End Function

    Public Shared Function GetColors() As Hashtable
        Dim hashResult As New Hashtable

        hashResult.Add("blue", "#348fe2")
        hashResult.Add("blueLight", "#5da5e8")
        hashResult.Add("blueDark", "#003596")
        hashResult.Add("aqua", "#49b6d6")
        hashResult.Add("aquaLight", "#6dc5de")
        hashResult.Add("aquaDark", "#3a92ab")
        hashResult.Add("green", "#00acac")
        hashResult.Add("greenLight", "#33bdbd")
        hashResult.Add("greenDark", "#016b10")
        hashResult.Add("orange", "#f59c1a")
        hashResult.Add("orangeLight", "#f7b048")
        hashResult.Add("orangeDark", "#c47d15")
        hashResult.Add("dark", "#2d353c")
        hashResult.Add("grey", "#b6c2c9")
        hashResult.Add("purple", "#727cb6")
        hashResult.Add("purpleLight", "#8e96c5")
        hashResult.Add("purpleDark", "#5b6392")
        hashResult.Add("white", "#ffffff")
        hashResult.Add("yellow", "#ffdb31")
        hashResult.Add("brown", "#763c04")
        hashResult.Add("red", "#ff5b57")
        hashResult.Add("beige", "#f5f5dc")

        Return hashResult
    End Function

    Public Structure CityInfos
        Dim Nome As String
        Dim Latitudine As Double
        Dim Longitudine As Double
    End Structure

    Public Structure GeoInfos
        Dim Latitudine As Double
        Dim Longitudine As Double
    End Structure

    Public Enum ValuesTypeEnum
        STRING_VALUE
        STRING_ARRAY
        DOUBLE_VALUE
        DOUBLE_ARRAY
        INTEGER_VALUE
        INTEGER_ARRAY
        DATA_VALUE
        DATA_ARRAY
    End Enum

    Public Enum RedirectEnum
        Dashboard
        Units
        Points
        City
        Null
    End Enum

    Public Enum FormFieldTypeEnum
        TextField
        HiddenField
    End Enum
End Class
