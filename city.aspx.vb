Imports System.Globalization
Imports System.Threading

Partial Class city
    Inherits System.Web.UI.Page

    '** Parametro lingua
    Private mLang As String

    '** Parametro city
    Private mCity As Integer

    '** Parametro action
    Private mAction As String

    '** Parmaetro label tabella
    Protected mTableLabels As String

    '** Parametri gestione nuclei
    Protected mStationsHeaders As String
    Protected mStationsData As String

    '** Parametro numero elementi iniziale tabella
    Protected mDisplayItems As Integer

    '** Parametro redirect
    Private mRedirect As FunctionsClass.RedirectEnum

    Protected Overrides Sub InitializeCulture()
        Try
            mLang = Session("lang").ToString

            If mLang = String.Empty Then mLang = "it"
        Catch ex As Exception
            mLang = "it"
        End Try

        Thread.CurrentThread.CurrentCulture = New CultureInfo(mLang)
        Thread.CurrentThread.CurrentUICulture = New CultureInfo(mLang)
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '** Controllo se richiamata pagina senza URL rewrite
        If Request.Url.AbsoluteUri.EndsWith(".aspx") Then Response.Redirect("~/home")

        '** Controllo stringa di connessione
        If Session("connection-string") Is Nothing Then Response.Redirect("~/home")

        '** Reperimento oggetto di sessione
        Dim oEcofilUser As EcofilUser = Nothing
        Try
            oEcofilUser = CType(Session("userinfos"), EcofilUser)

            If oEcofilUser.UserType <> EcofilUser.UserTypeEnum.Manager Then
                Response.Redirect("~/login")
            End If
        Catch ex As Exception
            Response.Redirect("~/login")
        End Try

        '** Parametri ricevuti
        mCity = 0
        If Request.QueryString("city") IsNot Nothing Then Integer.TryParse(HttpUtility.UrlDecode(Request.QueryString("city")), mCity)
        mAction = "NEW"
        If Request.QueryString("action") IsNot Nothing Then mAction = HttpUtility.UrlDecode(Request.QueryString("action"))

        '** Impostazione titolo pagina
        Page.Header.Title = GetLocalResourceObject("title").ToString

        '** Connessione al database
        Dim pConnector As New PostgreConnector(Session("connection-string").ToString)

        If pConnector.Status Then
            mRedirect = FunctionsClass.RedirectEnum.Null

            '** Controllo città selezionata
            If mCity > 0 AndAlso Not CityClass.CheckCityCode(pConnector, oEcofilUser, mCity) Then
                mRedirect = FunctionsClass.RedirectEnum.Dashboard
            End If

            '** Inizializzo le strutture jquery
            mTableLabels = "["""","""","""","""","""",""""]"
            mDisplayItems = 25
            mStationsHeaders = "null"
            mStationsData = "null"

            '** Impostazione dashboard        
            page_header.Text = "<h1 class=""page-header"">" & GetLocalResourceObject("city_info_base").ToString & "</h1>"

            label_user_info.Text = "<span>" & GetGlobalResourceObject("controls", "customer").ToString & ": <strong>" & oEcofilUser.IdUtente & "</strong></span> <b class=""caret""></b>"

            literal_menu_items_base.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "station_link").ToString.Replace("~/", "") & "?city=" & mCity.ToString & "&action=NEW"">" & GetGlobalResourceObject("pages", "station_new_text").ToString & "</a></li>" _
                                         & "<li class=""divider""></li>" _
                                         & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                         & "<li class=""divider""></li>"

            link_cancel.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted & "-" & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower

            If Not Page.IsPostBack Then
                '** Assegnazione script
                button_add.OnClientClick = "if (CheckTagCode('" & mLang & "')) {AssignTagToCity('" & mCity.ToString & "','" & mLang & "');}; return false;"
                button_check.OnClientClick = "if (CheckTagCode('" & mLang & "')) {CheckTag('" & mCity.ToString & "','" & mLang & "');}; return false;"

                '** Impostazione link modulo importazione tag
                link_download_csv_tags.NavigateUrl = "modules/UPL_TGS_" & mLang.ToLower & ".csv"

                '** Impostazione link modulo importazione tag
                link_download_csv_stations.NavigateUrl = "modules/UPL_STN_" & mLang.ToLower & ".csv"

                '** Impostazione campi form
                FunctionsClass.DisableSubmit(input_name)
                FunctionsClass.DisableSubmit(input_district)
                FunctionsClass.DisableSubmit(input_state)
                FunctionsClass.DisableSubmit(input_citizens)
                FunctionsClass.DisableSubmit(input_units)
                FunctionsClass.DisableSubmit(input_stations)
            End If

            If mRedirect = FunctionsClass.RedirectEnum.Null Or mAction = "NEW" Then
                '** Controllo azione
                Select Case mAction
                    Case "NEW"
                        link_reload.NavigateUrl = GetGlobalResourceObject("pages", "city_link").ToString & "?action=NEW"
                        literal_edit.Text = GetGlobalResourceObject("controls", "new_item")
                        panel_tags.Visible = False
                        panel_upload.Visible = False
                        panel_stations.Visible = False
                        link_delete.Attributes.Remove("onclick")

                        If Not Page.IsPostBack Then ClearFiels()
                    Case "UPD"
                        link_reload.NavigateUrl = GetGlobalResourceObject("pages", "city_link").ToString & "?city=" & mCity & "&action=UPD"
                        literal_edit.Text = GetGlobalResourceObject("controls", "edit_item")
                        panel_tags.Visible = True
                        panel_upload.Visible = True
                        panel_stations.Visible = True
                        link_delete.Attributes.Add("onclick", "SelectTagId('" & mCity.ToString & "','" & mLang & "');")

                        If Not LoadCityData(pConnector) Then mRedirect = FunctionsClass.RedirectEnum.Dashboard
                    Case "DEL"
                        If mCity > 0 Then
                            link_reload.NavigateUrl = GetGlobalResourceObject("pages", "city_link").ToString & "?city=" & mCity & "&action=UPD"
                            literal_edit.Text = GetGlobalResourceObject("controls", "edit_item")

                            Select Case CityClass.DeleteCity(pConnector, mCity, oEcofilUser)
                                Case CityClass.DeleteResultEnum.Deleted
                                    mRedirect = FunctionsClass.RedirectEnum.Dashboard
                                Case CityClass.DeleteResultEnum.CityNotDeleted
                                    literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_048").ToString)
                                Case CityClass.DeleteResultEnum.CityIdNotValid
                                    mRedirect = FunctionsClass.RedirectEnum.Dashboard
                            End Select
                        Else
                            mRedirect = FunctionsClass.RedirectEnum.Dashboard
                        End If
                    Case Else
                        mRedirect = FunctionsClass.RedirectEnum.Dashboard
                End Select

                '** Caricamento lista stazioni
                LoadStations(pConnector)
            End If            
        End If
        pConnector.Dispose()

        Select Case mRedirect
            Case FunctionsClass.RedirectEnum.Dashboard
                Response.Redirect(GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted & "-" & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower)
        End Select
    End Sub

    Private Function LoadCityData(ByRef pConnector As PostgreConnector) As Boolean
        '** Caricamento dati città
        Dim hashValues As Hashtable = CityClass.GetCityData(pConnector, mCity)

        If CType(hashValues("city-id").ToString, Integer) = 0 Then Return False

        '** Caricamento campi form
        input_name.Text = hashValues("city-name").ToString
        hidden_latitude.Value = hashValues("city-latitude").ToString
        hidden_longitude.Value = hashValues("city-longitude").ToString
        input_district.Text = hashValues("city-district").ToString
        input_state.Text = hashValues("city-state").ToString
        input_citizens.Text = hashValues("city-citizens").ToString
        input_units.Text = hashValues("city-units").ToString
        input_stations.Text = hashValues("city-stations").ToString
        input_username.Text = hashValues("city-email").ToString
        input_username.ReadOnly = True

        hashValues.Clear()

        Return True
    End Function

    Protected Sub button_confirm_Click(sender As Object, e As EventArgs) Handles button_confirm.Click
        Dim oEcofilUser As EcofilUser = CType(Session("userinfos"), EcofilUser)

        Dim hashFields As New Hashtable
        hashFields.Add("city-name", input_name.Text)
        hashFields.Add("city-district", input_district.Text)
        hashFields.Add("city-state", input_state.Text)
        hashFields.Add("city-citizens", input_citizens.Text)
        hashFields.Add("city-units", input_units.Text)
        hashFields.Add("city-stations", input_stations.Text)
        hashFields.Add("city-latitude", hidden_latitude.Value)
        hashFields.Add("city-longitude", hidden_longitude.Value)
        hashFields.Add("city-username", input_username.Text)

        Dim unitResult As CityClass.CityData = CityClass.Save(mCity, hashFields, Session("connection-string").ToString)

        ResultCity(unitResult, oEcofilUser)
    End Sub

    Private Sub ResultCity(ByRef cityResult As CityClass.CityData, ByRef oEcofilUser As EcofilUser)
        literal_alert.Text = String.Empty

        Select Case cityResult.Message
            Case UnitsClass.UnitsDataEnum.Created
                '** Aggiornamento sessione
                oEcofilUser.AddCity(cityResult.Id)
                Session("userinfos") = oEcofilUser

                '** Redirect alla dashboard
                Response.Redirect(cityResult.RedirectLink)
            Case CityClass.CityDataEnum.Updated
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.InfoAlert, GetGlobalResourceObject("messages", "message_012").ToString)
            Case CityClass.CityDataEnum.EmptyName
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_035").ToString)
            Case CityClass.CityDataEnum.EmptyDistrict
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_036").ToString)
            Case CityClass.CityDataEnum.EmptyState
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_046").ToString)
            Case CityClass.CityDataEnum.EmptyCitizens
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_045").ToString)
            Case CityClass.CityDataEnum.EmptyUserName
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_059").ToString)
            Case CityClass.CityDataEnum.WrongCitizens
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_047").ToString)
            Case CityClass.CityDataEnum.WrongUserName
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_061").ToString)
            Case CityClass.CityDataEnum.ErrorSavingCity
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_044").ToString)
            Case CityClass.CityDataEnum.ErrorSavingCityUserWeb
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_027").ToString)
            Case CityClass.CityDataEnum.ErrorDataBase
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_005").ToString)
            Case Else
                literal_alert.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_007").ToString)
        End Select
    End Sub

    Protected Sub button_upload_tags_Click(sender As Object, e As EventArgs) Handles button_upload_tags.Click
        If file_upload_tags.HasFile Then
            '** Controllo l'estensione del file
            If file_upload_tags.FileName.EndsWith(".csv") Then
                Try
                    Dim strFilePath = Request.PhysicalApplicationPath & "temp\" & mCity.ToString & "-" & file_upload_tags.FileName.Replace(" ", "-").Replace("_", "-")
                    file_upload_tags.SaveAs(strFilePath)

                    literal_alert_upload_tags.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.InfoAlert, GetGlobalResourceObject("messages", "message_013").ToString _
                                                                            & "<br />" _
                                                                            & "<a href=""" & CityClass.UploadCsvTags(mCity, strFilePath, mLang, Session("connection-string").ToString) & """ class=""btn btn-link p-0"" style=""color: #FFFFFF;"" target=""_blank"">" _
                                                                            & GetGlobalResourceObject("controls", "click_here").ToString & "</a>")
                Catch ex As Exception
                    literal_alert_upload_tags.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, HttpContext.GetGlobalResourceObject("errors", "error_007").ToString)
                End Try
            Else
                literal_alert_upload_tags.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_030").ToString)
            End If
        Else
            literal_alert_upload_tags.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_029").ToString)
        End If
    End Sub

    Protected Sub button_upload_stations_Click(sender As Object, e As EventArgs) Handles button_upload_stations.Click
        If file_upload_stations.HasFile Then
            '** Controllo l'estensione del file
            If file_upload_stations.FileName.EndsWith(".csv") Then
                Try
                    Dim strFilePath = Request.PhysicalApplicationPath & "temp\" & mCity.ToString & "-" & file_upload_stations.FileName.Replace(" ", "-").Replace("_", "-")
                    file_upload_stations.SaveAs(strFilePath)

                    literal_alert_upload_stations.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.InfoAlert, GetGlobalResourceObject("messages", "message_015").ToString _
                                                                               & "<br />" _
                                                                               & "<a href=""" & CityClass.UploadCsvStations(mCity, strFilePath, mLang, Session("connection-string").ToString) & """ class=""btn btn-link p-0"" style=""color: #FFFFFF;"" target=""_blank"">" _
                                                                               & GetGlobalResourceObject("controls", "click_here").ToString & "</a>")
                Catch ex As Exception
                    literal_alert_upload_stations.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, HttpContext.GetGlobalResourceObject("errors", "error_007").ToString)
                End Try
            Else
                literal_alert_upload_stations.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_030").ToString)
            End If
        Else
            literal_alert_upload_stations.Text = AlertsClass.CreateAlert(AlertsClass.AlertEnum.ErrorAlert, GetGlobalResourceObject("errors", "error_029").ToString)
        End If
    End Sub

    Private Sub ClearFiels()
        input_name.Text = String.Empty
        input_district.Text = String.Empty
        input_state.Text = String.Empty
        input_citizens.Text = String.Empty
        input_units.Text = String.Empty
        input_stations.Text = String.Empty
        hidden_latitude.Value = "0.0"
        hidden_longitude.Value = "0.0"
    End Sub

    Private Sub LoadStations(ByRef pConnector As PostgreConnector)
        Dim hashParms1 As New Hashtable
        hashParms1.Add("idcomune", mCity.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

        '** Caricamento stazioni
        mTableLabels = GetGlobalResourceObject("controls", "table_labels")
        mStationsHeaders = GetGlobalResourceObject("controls", "table_stations2_headers")
        mStationsData = StationsClass.GetStationsDataTable(pConnector, hashParms1, mLang)
    End Sub
End Class
