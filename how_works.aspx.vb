Imports System.Globalization
Imports System.Threading

Partial Class how_works
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

        '** Controllo se richiamata pagina senza URL rewrite
        If Request.Url.AbsoluteUri.EndsWith(".aspx") Then Response.Redirect("~/home")

        '** Impostazione titolo pagina
        Page.Header.Title = GetLocalResourceObject("title").ToString
    End Sub
End Class
