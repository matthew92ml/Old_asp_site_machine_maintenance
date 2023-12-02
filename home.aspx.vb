Imports System.Globalization
Imports System.Threading
Imports Npgsql

Partial Class home
    Inherits System.Web.UI.Page

    '** Parametro lingua
    Private mLang As String

    Protected Overrides Sub InitializeCulture()
        If Request.QueryString("lang") IsNot Nothing Then
            Select Case Request.QueryString("lang")
                Case "IT"
                    mLang = "it"
                Case "EN"
                    mLang = "en"
                Case Else
                    mLang = "en"
            End Select
        Else
            Try
                mLang = Session("lang").ToString

                If mLang = String.Empty Then
                    mLang = "it"
                End If
            Catch ex As Exception
                mLang = "it"
            End Try
        End If

        Thread.CurrentThread.CurrentCulture = New CultureInfo(mLang)
        Thread.CurrentThread.CurrentUICulture = New CultureInfo(mLang)
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            '** Creazione connection string di sessione
            If Session("connection-string") Is Nothing Then
                Session("connection-string") = ConfigurationManager.ConnectionStrings.Item("PostgreConnectionString").ToString
            End If
        End If

        '** Controllo se richiamata pagina senza URL rewrite
        If Request.Url.AbsoluteUri.EndsWith(".aspx") Then Response.Redirect("~/home")

        '** Controllo logout
        If Request.QueryString("logout") IsNot Nothing AndAlso Request.QueryString("logout").ToString = "1" Then
            LoginClass.ExitLogin(Request, Response)

            Session("userinfos") = Nothing
        End If

        '** Caricamento enti
        Dim pConnector As New PostgreConnector(Session("connection-string").ToString)

        If pConnector.Status Then
            Dim sqlString As String = "SELECT * FROM infocomune WHERE id > 0 ORDER BY id ASC"

            If pConnector.AddReader("infocomune", sqlString) Then
                literal_cities.Text = "<div class=""col-md-12"">"

                Do While True
                    Dim sqlReader As NpgsqlDataReader = pConnector.GetData("infocomune")

                    If sqlReader IsNot Nothing Then
                        literal_cities.Text &= "<a href=""" & GetGlobalResourceObject("pages", "infos_link").ToString.Replace("~/", "") & "-" & sqlReader("id").ToString & """>" _
                                             & "<div class=""col-md-3"">" _
                                             & "<div class=""service text-center"">" _
                                             & "<div class=""serv-block"">" _
                                             & "<img src=""assets/img/comuni/" & sqlReader("id").ToString & ".jpg"" height=""150"" />" _
                                             & "</div>" _
                                             & "<h6>" & sqlReader("nome").ToString & "</h6>" _
                                             & "</div>" _
                                             & "</div>" _
                                             & "</a>"
                    Else
                        Exit Do
                    End If
                Loop

                literal_cities.Text &= "</div>"

                pConnector.DelReader("infocomune")
            End If
        End If
        pConnector.Dispose()
    End Sub

    Private Sub RedirectLogin(ByRef loginResult As LoginClass.LoginData)
        Select Case loginResult.Message
            Case LoginClass.LoginDataEnum.LoggedOn
                Dim oEcofilUser As EcofilUser = loginResult.EcofilUser

                If oEcofilUser IsNot Nothing Then
                    '** Creo oggetto di sessione
                    Session("userinfos") = oEcofilUser

                    '** Redirect alla dashboard
                    Response.Redirect(loginResult.RedirectLink)
                End If
            Case Else
                '...
        End Select
    End Sub
End Class
