Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Web

Public Class HttpRequestClass
    Public Shared Function httpPostRequest(ByVal uri As String, ByVal parameters As String) As String
        Dim httpRequest As HttpWebRequest
        Dim responseFromServer As String = String.Empty
        Dim dataStream As Stream = Nothing

        Try
            httpRequest = DirectCast(WebRequest.Create(uri), HttpWebRequest)

            httpRequest.Method = "POST"
            httpRequest.ContentType = "application/x-www-form-urlencoded"

            Dim encoding As New Text.ASCIIEncoding()
            Dim postByteArray() As Byte = encoding.GetBytes(parameters)
            httpRequest.ContentLength = postByteArray.Length

            dataStream = httpRequest.GetRequestStream()
            dataStream.Write(postByteArray, 0, postByteArray.Length)
            dataStream.Close()

            Dim response As HttpWebResponse = httpRequest.GetResponse()

            If response.StatusCode = HttpStatusCode.OK Then
                dataStream = response.GetResponseStream()

                Dim reader As New StreamReader(dataStream)
                responseFromServer = reader.ReadToEnd()
                reader.Close()
            End If

            dataStream.Close()
            response.Close()
        Catch ex As Exception
            responseFromServer = "Err#" & ex.Message
        End Try

        Return responseFromServer
    End Function

    Public Shared Function httpGetRequest(ByVal uri As String, ByVal parameters As String) As String
        Dim httpRequest As HttpWebRequest
        Dim responseFromServer As String = String.Empty

        Try
            If Not uri.ToUpper.StartsWith("HTTP") Then uri = "http://" & uri
            If Not uri.Contains("?") Then uri = uri & "?"

            httpRequest = DirectCast(WebRequest.Create(uri & parameters), HttpWebRequest)
            httpRequest.Method = "GET"
            httpRequest.Credentials = CredentialCache.DefaultCredentials

            Dim response As WebResponse = httpRequest.GetResponse()

            Dim dataStream As Stream = response.GetResponseStream()

            Dim reader As New StreamReader(dataStream)

            responseFromServer = reader.ReadToEnd()

            reader.Close()
            response.Close()
        Catch ex As Exception
            responseFromServer = "Err#" & ex.Message
        End Try

        Return responseFromServer
    End Function
End Class
