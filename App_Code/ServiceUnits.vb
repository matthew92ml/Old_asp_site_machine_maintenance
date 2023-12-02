Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Npgsql

<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://www.ecofilgreen.it/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class ServiceUnits
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function CS_AssignTagToUnit(ByVal tagCode As String, ByVal cityId As String, ByVal unitId As String, ByVal unitCode As String, ByVal infos As String, ByVal lang As String) As String
        Dim strResult As String = "ok^"

        Dim pConnector As New PostgreConnector(ConfigurationManager.ConnectionStrings.Item("PostgreConnectionString").ToString)

        If pConnector.Status Then
            Dim sqlString As String = String.Empty

            '** Controllo se TAG assegnato all'ente
            sqlString = "SELECT id FROM infonuclei " _
                      & "WHERE idcomune = " & cityId & " " _
                      & "AND UPPER(codicetag) = '" & FunctionsClass.CheckString(tagCode).ToUpper & "'"

            If pConnector.ExecuteScalar(sqlString) = 0 Then
                '** Assegnazione TAG-COMUNE
                sqlString = "INSERT INTO infonuclei " _
                          & "(codicetag, idcomune) " _
                          & "VALUES('" & FunctionsClass.CheckString(tagCode).ToLower & "', " & cityId & ")"

                pConnector.ExecuteCommand(sqlString)
            End If

            '** Controllo se TAG libero
            sqlString = "SELECT COUNT(*) FROM associazionenuclei " _
                      & "WHERE idcomune = " & cityId & " " _
                      & "AND UPPER(codicetag) = '" & FunctionsClass.CheckString(tagCode).ToUpper & "'"

            If pConnector.ExecuteScalar(sqlString) = 0 Then
                '** Assegnazione TAG-NUCLEO
                sqlString = "INSERT INTO associazionenuclei " _
                          & "(codicetag, " _
                          & "codicenucleo, " _
                          & "idcomune, " _
                          & "info) " _
                          & "VALUES('" & FunctionsClass.CheckString(tagCode).ToLower & "', '" _
                          & FunctionsClass.CheckString(unitCode) & "', " _
                          & cityId & ", '" _
                          & FunctionsClass.CheckString(infos) & "')"

                If pConnector.ExecuteCommand(sqlString) Then                    
                    strResult &= FunctionsClass.GetTagsList(pConnector, CType(cityId, Integer), CType(unitId, Integer))
                Else
                    strResult = "null^" & lang & "^TAG_ASSIGN_ERROR"
                End If
            Else
                strResult = "null^" & lang & "^TAG_ASSIGNED_ERROR"
            End If
        Else
            strResult = "null^" & lang & "^DB_ERROR"
        End If

        Return strResult
    End Function

    <WebMethod()> _
    Public Function CS_DeleteTagFromUnit(ByVal tagCode As String, ByVal cityId As String, ByVal unitId As String, ByVal unitCode As String, ByVal lang As String) As String
        Dim strResult As String = "ok^"

        Dim pConnector As New PostgreConnector(ConfigurationManager.ConnectionStrings.Item("PostgreConnectionString").ToString)

        If pConnector.Status Then
            Dim sqlString As String = String.Empty

            '** Controllo se TAG assegnato al nucleo
            sqlString = "SELECT COUNT(*) FROM associazionenuclei " _
                      & "WHERE idcomune = " & cityId & " " _
                      & "AND codicenucleo = '" & unitCode & "' " _
                      & "AND UPPER(codicetag) = '" & FunctionsClass.CheckString(tagCode).ToUpper & "'"

            If pConnector.ExecuteScalar(sqlString) > 0 Then
                '** Eliminazione TAG
                sqlString = "DELETE FROM associazionenuclei " _
                          & "WHERE idcomune = " & cityId & " " _
                          & "AND codicenucleo = '" & unitCode & "' " _
                          & "AND UPPER(codicetag) = '" & FunctionsClass.CheckString(tagCode).ToUpper & "'"

                If pConnector.ExecuteCommand(sqlString) Then
                    strResult &= FunctionsClass.GetTagsList(pConnector, CType(cityId, Integer), CType(unitId, Integer))
                Else
                    strResult = "null^" & lang & "^TAG_DELETE_ERROR"
                End If
            Else
                strResult = "null^" & lang & "^TAG_ERROR"
            End If
        Else
            strResult = "null^" & lang & "^DB_ERROR"
        End If

        Return strResult
    End Function
End Class