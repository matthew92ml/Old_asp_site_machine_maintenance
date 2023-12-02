Imports System.Globalization
Imports System.Threading

Partial Class dashboard
    Inherits System.Web.UI.Page

    '** Parametro lingua
    Private mLang As String

    '** Parametro filtro
    Private mFilter1 As String
    Private mFilter2 As Integer    
    Private mFilter3 As String
    Private mFilter4 As Integer

    '** Parametri dati grafico timeline
    Protected mTimelineGarbageMinYDay As String
    Protected mTimelineGarbageMaxYDay As String
    Protected mTimelineGarbageTypesDataDay As String
    Protected mTimelineGarbageTypesDataSum As String
    Protected mTimeLineColor As String

    '** Parametri dati grafico rifiuti
    Protected mGarbageData As String

    '** Parametri dati grafico percentuali rifiuti
    Protected mGarbageDataDonut As String

    '** Parametro label tabella
    Protected mTableLabels As String

    '** Parametro numero elementi iniziale tabella
    Protected mDisplayItems As Integer

    '** Parametri dati tabella conferimenti
    Protected mContributionsHeaders As String
    Protected mContributionsData As String

    '** Parametri dati tabella comuni
    Protected mCitiesHeaders As String
    Protected mCitiesData As String

    '** Parametri dati tabella stazioni
    Protected mStationsHeaders As String
    Protected mStationsData As String

    '** Parametri mappa
    Protected mMapCityLan As String
    Protected mMapCityLon As String
    Protected mMapCityZoom As String
    Protected mMapData As String

    '** Costanti
    Private Const CITIES_LIST_IT As String = "cities"
    Private Const CITIES_LIST_EN As String = "comuni"
    Private Const STATIONS_LIST_IT As String = "stations"
    Private Const STATIONS_LIST_EN As String = "stazioni"
    Private Const UNIT_DETAIL_IT As String = "nucleo"
    Private Const UNIT_DETAIL_EN As String = "unit"

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

            If Page.RouteData.Values.ContainsKey("profile") Then
                If Page.RouteData.Values("profile").ToString <> oEcofilUser.IdUtenteEncrypted Then
                    Response.Redirect("~/home?logoff=1")
                End If
            Else
                Response.Redirect("~/home?logoff=1")
            End If

            mFilter1 = String.Empty
            If Page.RouteData.Values.ContainsKey("filter1") Then mFilter1 = Page.RouteData.Values("filter1").ToString
            mFilter2 = 0
            If Page.RouteData.Values.ContainsKey("filter2") Then Integer.TryParse(Page.RouteData.Values("filter2").ToString, mFilter2)
            mFilter3 = String.Empty
            If Page.RouteData.Values.ContainsKey("filter3") Then mFilter3 = Page.RouteData.Values("filter3").ToString
            mFilter4 = 0
            If Page.RouteData.Values.ContainsKey("filter4") Then Integer.TryParse(Page.RouteData.Values("filter4").ToString, mFilter4)

            Select Case oEcofilUser.UserType
                Case EcofilUser.UserTypeEnum.User
                    If mFilter1 <> String.Empty Then mFilter1 = String.Empty
                    If mFilter2 > 0 Then mFilter2 = 0
                    If mFilter3 <> String.Empty Then mFilter3 = String.Empty
                    If mFilter4 > 0 Then mFilter4 = 0
                Case EcofilUser.UserTypeEnum.Administration
                    If oEcofilUser.IdComune.Count = 1 Then
                        If (mFilter1 = CITIES_LIST_IT Or mFilter1 = CITIES_LIST_EN) Then
                            mFilter1 = String.Empty
                            mFilter2 = 0
                        End If

                        If mFilter3 <> String.Empty Then mFilter3 = String.Empty
                        If mFilter4 > 0 Then mFilter4 = 0
                    End If
            End Select
        Catch ex As Exception
            Response.Redirect("~/login")
        End Try

        '** Impostazione titolo pagina
        Page.Header.Title = GetLocalResourceObject("title").ToString

        '** Connessione al database
        Dim pConnector As New PostgreConnector(Session("connection-string").ToString)

        If pConnector.Status Then
            '** Controllo parametro filter 1
            Select Case mFilter1
                Case CITIES_LIST_IT, CITIES_LIST_EN
                    If mFilter2 > 0 AndAlso Not CityClass.CheckCityCode(pConnector, oEcofilUser, mFilter2) Then
                        pConnector.Dispose()

                        Response.Redirect("~/home?logoff=1")
                    End If

                    '** Controllo parametri filter 2 e filter 4
                    Select Case mFilter3
                        Case STATIONS_LIST_IT, STATIONS_LIST_EN
                            If mFilter2 > 0 AndAlso mFilter4 > 0 AndAlso Not StationsClass.CheckStationCode(pConnector, mFilter2, mFilter4) Then
                                pConnector.Dispose()

                                Response.Redirect("~/home?logoff=1")
                            End If
                        Case UNIT_DETAIL_IT, UNIT_DETAIL_EN
                            If mFilter2 > 0 AndAlso mFilter4 > 0 AndAlso Not UnitsClass.CheckUnitCode(pConnector, mFilter4) Then
                                pConnector.Dispose()

                                Response.Redirect("~/home?logoff=1")
                            End If
                    End Select                    
                Case STATIONS_LIST_IT, STATIONS_LIST_EN
                    '** Controllo parametro filter 2
                    If mFilter2 > 0 AndAlso Not StationsClass.CheckStationCode(pConnector, oEcofilUser.IdComune(0), mFilter2) Then
                        pConnector.Dispose()

                        Response.Redirect("~/home?logoff=1")
                    End If
            End Select
            
            '** Inizializzo le strutture jquery
            mTableLabels = "["""","""","""","""","""",""""]"
            mDisplayItems = 50
            mTimelineGarbageMinYDay = "-10"
            mTimelineGarbageMaxYDay = "150"
            mTimelineGarbageTypesDataDay = "null"
            mTimelineGarbageTypesDataSum = "null"
            mGarbageData = "null"
            mGarbageDataDonut = "null"
            mContributionsHeaders = "null"
            mContributionsData = "null"
            mCitiesHeaders = "null"
            mCitiesData = "null"
            mStationsHeaders = "null"
            mStationsData = "null"
            mMapCityLan = oEcofilUser.Latitudine.ToString().Replace(",", ".")
            mMapCityLon = oEcofilUser.Longitudine.ToString().Replace(",", ".")
            mMapCityZoom = "9"
            mMapData = "null"

            '** Inizializzo le variabili di sessione
            Session("city-return-page") = String.Empty

            Select Case oEcofilUser.UserType
                Case EcofilUser.UserTypeEnum.User
                    panel_date_range.Visible = False
                    label_user_info.Text = "<span>" & GetGlobalResourceObject("controls", "user").ToString & ": <strong>" & oEcofilUser.NominativoNucleo & " (" & oEcofilUser.IdUtente & ")</strong></span> <b class=""caret""></b>"
                    literal_panel_4.Text = GetLocalResourceObject("panel_contributions_list").ToString
                    literal_menu_items_base.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "profile_user_link").ToString.Replace("~/", "") & """>" & GetGlobalResourceObject("pages", "profile_text").ToString & "</a></li>"

                    LoadUserDashboard(pConnector, oEcofilUser)
                Case EcofilUser.UserTypeEnum.Administration
                    panel_date_range.Visible = True
                    input_start_date.Attributes.Add("placeholder", GetGlobalResourceObject("controls", "date_start").ToString)
                    input_end_date.Attributes.Add("placeholder", GetGlobalResourceObject("controls", "date_end").ToString)
                    label_user_info.Text = "<span>" & GetGlobalResourceObject("controls", "customer").ToString & ": <strong>" & oEcofilUser.IdUtente & "</strong></span> <b class=""caret""></b>"
                    If oEcofilUser.IdComune.Count = 1 Or (oEcofilUser.IdComune.Count > 1 And mFilter2 > 0) Then
                        If mFilter2 > 0 Then Session("city-return-page") = HttpContext.GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower

                        Dim linkUnits As String = GetGlobalResourceObject("pages", "units_link").ToString.Replace("~/", "") & "?city=" & Server.UrlEncode(IIf(oEcofilUser.IdComune.Count = 1, oEcofilUser.IdComune(0), mFilter2).ToString)
                        'Dim linkPoints As String = GetGlobalResourceObject("pages", "points_link").ToString.Replace("~/", "") & "?city=" & Server.UrlEncode(IIf(oEcofilUser.IdComune.Count = 1, oEcofilUser.IdComune(0), mFilter2).ToString)

                        literal_menu_items.Text = "<li><a href=""" & linkUnits & """>" & GetGlobalResourceObject("pages", "units_text").ToString & "</a></li>" '_
                        '& "<li><a href=""" & linkPoints & """>" & GetGlobalResourceObject("pages", "points_text").ToString & "</a></li>"
                    End If

                    literal_menu_items_base.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "profile_administration_link").ToString.Replace("~/", "") & """>" & GetGlobalResourceObject("pages", "profile_text").ToString & "</a></li>"

                    LoadAdministrationDashboard(pConnector, oEcofilUser)
                Case EcofilUser.UserTypeEnum.Manager
                    panel_date_range.Visible = True
                    input_start_date.Attributes.Add("placeholder", GetGlobalResourceObject("controls", "date_start").ToString)
                    input_end_date.Attributes.Add("placeholder", GetGlobalResourceObject("controls", "date_end").ToString)
                    label_user_info.Text = "<span>Ecofil</span> <b class=""caret""></b>"
                    literal_panel_4.Text = GetLocalResourceObject("panel_cities_map").ToString

                    'literal_menu_items.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "city_link").ToString.Replace("~/", "") & "?action=NEW"">" & GetGlobalResourceObject("pages", "city_new_text").ToString & "</a></li>"

                    literal_menu_items_base.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "profile_administration_link").ToString.Replace("~/", "") & """>" & GetGlobalResourceObject("pages", "profile_text").ToString & "</a></li>"

                    LoadManagerDashboard(pConnector, oEcofilUser)
                Case Else
                    Response.Redirect("~/login")
            End Select
        End If
        pConnector.Dispose()
    End Sub

    Private Sub LoadUserDashboard(ByRef pConnector As PostgreConnector, ByRef oEcofilUser As EcofilUser)
        Dim hashParms1 As New Hashtable
        Dim hashParms2 As New Hashtable
        Dim hashParms3 As New Hashtable
        Dim hashParms4 As New Hashtable
        Dim hashParms5 As New Hashtable

        '** Codice comune
        hashParms1.Add("idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms3.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms4.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms5.Add("conferimenti.idcomune", mFilter1.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

        '** Codice nucleo
        hashParms2.Add("datinuclei.id", oEcofilUser.IdNucleo.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

        '** Codice TAG
        hashParms3.Add("conferimenti.codicetag", String.Join("#", oEcofilUser.CodiceTag) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
        hashParms4.Add("conferimenti.codicetag", String.Join("#", oEcofilUser.CodiceTag) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
        hashParms5.Add("conferimenti.codicetag", String.Join("#", oEcofilUser.CodiceTag) & ";" & FunctionsClass.DATATYPE_STRING & ";=")

        '** Peso
        hashParms3.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms4.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms5.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")

        '** Data
        hashParms3.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
        hashParms4.Add("conferimenti.dataora", Now.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
        hashParms5.Add("conferimenti.dataora", Now.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")

        '** Tipo rifiuto
        hashParms5.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")


        literal_summary.Text = DashboardClass.CreateSummaryItem(oEcofilUser.NominativoNucleo.Replace(" ", "<br />"), "assets/img/user.png", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRD(pConnector, hashParms2).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                FunctionsClass.GetRDByCityInfo(pConnector, hashParms1)) _
                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms4).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms4)) _
                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms5).ToString & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms5))

        TogglePanels(False)

        '** Caricamento grafico per rifiuto
        Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, oEcofilUser.IdComune(0), 1)
        Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms3)
        literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

        '** Caricamento grafico donut per rifiuto
        mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

        '** Caricamento grafici timeline      
        literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
        mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms3, hashGarbageType, hidden_garbage_type.Value)
        mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms3, hashGarbageType, hidden_garbage_type.Value, True)

        '** Caricamento lista dei conferimenti
        mTableLabels = GetGlobalResourceObject("controls", "table_labels")
        mDisplayItems = 10
        mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
        mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms3, New List(Of String) From {"conferimenti.peso"})

        '** Impostazione dashboard        
        link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted

        page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & "</small>" & "</h1>"

        '** Impostazione briciole di pane
        literal_breadcrumb.Text = "<li class=""active"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString & "</li>"
    End Sub

    Private Sub LoadAdministrationDashboard(ByRef pConnector As PostgreConnector, ByRef oEcofilUser As EcofilUser)
        '** Controllo se selezionato filtro
        Dim booProcapite As Boolean = False
        Dim datStart As Date = Date.MinValue
        Date.TryParse(input_start_date.Text, datStart)
        Dim datEnd As Date = Date.MinValue
        Date.TryParse(input_end_date.Text, datEnd)
        If datEnd <> Date.MinValue Then datEnd = datEnd.AddHours(23).AddMinutes(59).AddSeconds(59)
        If datStart <> Date.MinValue Or datEnd <> Date.MinValue Then booProcapite = True

        Dim hashParms1 As New Hashtable
        Dim hashParms2 As New Hashtable
        Dim hashParms3 As New Hashtable
        Dim hashParms4 As New Hashtable
        Dim hashParms5 As New Hashtable
        Dim hashParms6 As New Hashtable
        Dim hashParms7 As New Hashtable
        Dim hashParms8 As New Hashtable
        Dim hashParms9 As New Hashtable
        Dim hashParms10 As New Hashtable
        Dim hashParms11 As New Hashtable

        '** Codice comune
        If mFilter2 > 0 And (mFilter1 = CITIES_LIST_IT Or mFilter1 = CITIES_LIST_EN) Then
            hashParms1.Add("idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms2.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms3.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms4.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms5.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms9.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms10.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms11.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        Else
            hashParms1.Add("idcomune", String.Join("#", oEcofilUser.IdComune) & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms2.Add("conferimenti.idcomune", String.Join("#", oEcofilUser.IdComune) & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms3.Add("conferimenti.idcomune", String.Join("#", oEcofilUser.IdComune) & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms6.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms7.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms9.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms10.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms11.Add("conferimenti.idcomune", oEcofilUser.IdComune(0).ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        End If

        '** Codice stazione        
        If mFilter4 > 0 Then
            hashParms4.Add("conferimenti.codicestazione", mFilter4.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms5.Add("conferimenti.codicestazione", mFilter4.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        End If
        If mFilter2 > 0 Then
            hashParms6.Add("conferimenti.codicestazione", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms7.Add("conferimenti.codicestazione", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        End If        

        '** Peso
        hashParms2.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms3.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms4.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms5.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms6.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms7.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms9.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms10.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms11.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")

        '** Data
        If datStart <> Date.MinValue Or datEnd <> Date.MinValue Then
            If datStart <> Date.MinValue Then
                hashParms2.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms3.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms4.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms5.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms6.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms7.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms9.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms10.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms11.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            End If

            If datEnd <> Date.MinValue Then
                hashParms2.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms3.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms4.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms5.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms6.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms7.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms9.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms10.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms11.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
            End If
        Else
            hashParms2.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms3.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms4.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms5.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms6.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms7.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms9.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms10.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms11.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
        End If

        '** Tipo rifiuto
        hashParms3.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms5.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms7.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms10.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

        If mFilter1 = UNIT_DETAIL_IT Or mFilter1 = UNIT_DETAIL_EN Then
            '** Codice nucleo
            hashParms8.Add("datinuclei.id", mFilter2 & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

            '** Codice Tag
            Dim tagList As List(Of String) = UnitsClass.GetUnitTags(pConnector, mFilter2)
            hashParms9.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
            hashParms10.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
            hashParms11.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
        End If

        If mFilter3 = UNIT_DETAIL_IT Or mFilter3 = UNIT_DETAIL_EN Then
            '** Codice nucleo
            hashParms8.Add("datinuclei.id", mFilter4 & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

            '** Codice Tag
            Dim tagList As List(Of String) = UnitsClass.GetUnitTags(pConnector, mFilter4)
            hashParms9.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
            hashParms10.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
            hashParms11.Add("conferimenti.codicetag", String.Join("#", tagList) & ";" & FunctionsClass.DATATYPE_STRING & ";=")
        End If
        
        Dim cityId As Integer = 0
        Dim cityInfosData As New FunctionsClass.CityInfos
        Dim stationInfosData As New StationsClass.StationsInfos
        Dim nominativoNucleo As String = String.Empty
        If oEcofilUser.IdComune.Count = 1 Or mFilter2 > 0 Then
            Select Case mFilter1
                Case String.Empty, CITIES_LIST_IT, CITIES_LIST_EN
                    If mFilter2 > 0 Then
                        cityId = mFilter2
                    Else
                        cityId = oEcofilUser.IdComune(0)
                    End If

                    cityInfosData = FunctionsClass.GetCityInfos(pConnector, cityId)

                    Select Case mFilter3
                        Case STATIONS_LIST_IT, STATIONS_LIST_EN
                            stationInfosData = StationsClass.GetStationInfos(pConnector, mFilter2, mFilter4)

                            literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & cityId.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_6 & " " & DashboardClass.SUMMARY_CLASS_SM_6, stationInfosData.Nome & "<br />" & stationInfosData.Via) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms4).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms4), String.Empty).ToString) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms5).ToString, DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms5), String.Empty).ToString)
                        Case UNIT_DETAIL_IT, UNIT_DETAIL_EN
                            '** Decodifica nucleo selezionato
                            nominativoNucleo = UnitsClass.GetUnitUserName(pConnector, mFilter4)

                            literal_summary.Text = DashboardClass.CreateSummaryItem(nominativoNucleo.Replace(" ", "<br />"), "assets/img/user.png", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRD(pConnector, hashParms8).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    String.Empty) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms9).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms9), String.Empty).ToString) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms10).ToString & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms10), String.Empty).ToString)
                        Case Else
                            literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & cityId.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRDByCity(pConnector, hashParms2).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    String.Empty) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms2).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms2), String.Empty).ToString) _
                                                 & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms3).ToString & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                    DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                    IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms3), String.Empty).ToString)
                    End Select
                Case STATIONS_LIST_IT, STATIONS_LIST_EN
                    cityId = oEcofilUser.IdComune(0)

                    cityInfosData = FunctionsClass.GetCityInfos(pConnector, cityId)

                    If mFilter2 > 0 Then
                        stationInfosData = StationsClass.GetStationInfos(pConnector, cityId, mFilter2)

                        literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & cityId.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_6 & " " & DashboardClass.SUMMARY_CLASS_SM_6, stationInfosData.Nome & "<br />" & stationInfosData.Via) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms6).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms6), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms7).ToString, DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms7), String.Empty).ToString)
                    Else
                        literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & cityId.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRDByCity(pConnector, hashParms2).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                String.Empty) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms2).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms2), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms3).ToString, DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text,
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms3), String.Empty).ToString)
                    End If
                Case UNIT_DETAIL_IT, UNIT_DETAIL_EN
                    cityId = oEcofilUser.IdComune(0)

                    cityInfosData = FunctionsClass.GetCityInfos(pConnector, cityId)

                    If mFilter2 > 0 Then
                        '** Decodifica nucleo selezionato
                        nominativoNucleo = UnitsClass.GetUnitUserName(pConnector, mFilter2)

                        literal_summary.Text = DashboardClass.CreateSummaryItem(nominativoNucleo.Replace(" ", "<br />"), "assets/img/user.png", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRD(pConnector, hashParms8).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                String.Empty) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms9).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms9), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms10).ToString & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms10), String.Empty).ToString)
                    Else
                        literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & cityId.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRDByCity(pConnector, hashParms2).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                String.Empty) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms2).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms2), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms3).ToString, DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text,
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms3), String.Empty).ToString)
                    End If
            End Select
        End If

        Select Case mFilter1
            Case CITIES_LIST_IT, CITIES_LIST_EN
                If mFilter2 = 0 Then
                    TogglePanels(True)

                    '** Caricamento lista dei comuni
                    mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                    mCitiesHeaders = GetGlobalResourceObject("controls", "table_cities_headers_noedit")
                    mCitiesData = CityClass.GetCitiesData(pConnector, hashParms1, oEcofilUser)

                    '** Caricamento della mappa dei comuni
                    mMapData = CityClass.GetCitiesMap(pConnector, hashParms1, oEcofilUser)

                    '** Impostazioni titolo lista
                    literal_list_1.Text = GetLocalResourceObject("panel_cities_list").ToString
                    literal_list_2.Text = GetLocalResourceObject("panel_cities_map").ToString

                    '** Impostazione dashboard        
                    link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                            & oEcofilUser.IdUtenteEncrypted & "-" _
                                            & mFilter1

                    link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                    page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("panel_cities_list").ToString & "</small>" & "</h1>"

                    '** Impostazione briciole di pane
                    literal_breadcrumb.Text = "<li class=""active"">" & GetLocalResourceObject("panel_cities_list").ToString & "</li>"
                Else
                    Select Case mFilter3
                        Case STATIONS_LIST_IT, STATIONS_LIST_EN
                            If mFilter4 = 0 Then
                                TogglePanels(True)

                                '** Caricamento lista delle 
                                mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                                mStationsHeaders = GetGlobalResourceObject("controls", "table_stations_headers")
                                mStationsData = StationsClass.GetStationsData(pConnector, hashParms1, oEcofilUser, False, datStart, datEnd)

                                '** Caricamento della mappa delle stazioni
                                mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                                mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                                mMapCityZoom = "15"
                                mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                                '** Impostazioni titolo lista
                                literal_list_1.Text = GetLocalResourceObject("panel_stations_list").ToString
                                literal_list_2.Text = GetLocalResourceObject("panel_stations_map").ToString

                                '** Impostazione dashboard        
                                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                        & oEcofilUser.IdUtenteEncrypted & "-" _
                                                        & mFilter1 & "-" _
                                                        & mFilter2.ToString & "-" _
                                                        & mFilter3

                                link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("panel_stations_list").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                                '** Impostazione briciole di pane
                                literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & """>" & cityInfosData.Nome & "</a></li>" _
                                                        & "<li class=""active"">" & GetLocalResourceObject("panel_stations_list").ToString & "</li>"
                            Else
                                TogglePanels(False)

                                '** Caricamento grafico per rifiuto
                                Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, CType(IIf(mFilter2 > 0, mFilter2, oEcofilUser.IdComune(0)), Integer), 1)
                                Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms4)
                                literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                                '** Caricamento grafico donut per rifiuto
                                mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                                '** Caricamento grafico timeline
                                literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                                mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value)
                                mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value, True)

                                '** Caricamento lista dei conferimenti
                                mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                                mDisplayItems = 5
                                mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
                                mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms4, New List(Of String) From {"conferimenti.peso"})

                                '** Impostazione dashboard        
                                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                        & oEcofilUser.IdUtenteEncrypted & "-" _
                                                        & mFilter1 & "-" _
                                                        & mFilter2.ToString & "-" _
                                                        & mFilter3 & "-" _
                                                        & mFilter4.ToString

                                link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetGlobalResourceObject("controls", "station").ToString & " " & stationInfosData.Nome & " " & stationInfosData.Via & "</small>" & "</h1>"

                                '** Impostazione briciole di pane
                                literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & """>" & cityInfosData.Nome & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & "-" & mFilter3 & """>" & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>" _
                                                        & "<li class=""active"">" & stationInfosData.Nome & "</li>"

                                '** Panello 4
                                literal_panel_4.Text = GetLocalResourceObject("panel_contributions_list").ToString
                            End If
                        Case UNIT_DETAIL_IT, UNIT_DETAIL_EN
                            TogglePanels(False)

                            '** Caricamento grafico per rifiuto
                            Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, CType(IIf(mFilter2 > 0, mFilter2, oEcofilUser.IdComune(0)), Integer), 1)
                            Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms11)
                            literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                            '** Caricamento grafico donut per rifiuto
                            mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                            '** Caricamento grafici timeline      
                            literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms11, hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms11, hashGarbageType, hidden_garbage_type.Value, True)

                            '** Caricamento lista dei conferimenti
                            mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                            mDisplayItems = 10
                            mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
                            mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms11, New List(Of String) From {"conferimenti.peso"})

                            '** Impostazione dashboard        
                            link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                    & oEcofilUser.IdUtenteEncrypted & "-" _
                                                    & mFilter1 & "-" _
                                                    & mFilter2.ToString & "-" _
                                                    & mFilter3 & "-" _
                                                    & mFilter4.ToString

                            page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & "</small>" & "</h1>"

                            '** Impostazione briciole di pane
                            literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "units_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                    & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & """>" & cityInfosData.Nome & "</a></li>" _
                                                    & "<li><a href=""" & GetGlobalResourceObject("pages", "units_link").ToString.Replace("~/", "") & "?city=" & mFilter2.ToString & """>" & GetGlobalResourceObject("pages", "units_text").ToString & "</a></li>" _
                                                    & "<li class=""active"">" & nominativoNucleo & "</li>"
                        Case Else
                            TogglePanels(False, True)

                            '** Caricamento grafico per rifiuto
                            Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, CType(IIf(mFilter2 > 0, mFilter2, oEcofilUser.IdComune(0)), Integer), 1)
                            Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms2)
                            literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                            '** Caricamento grafico donut per rifiuto
                            mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                            '** Caricamento grafico timeline
                            literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value, True)

                            '** Caricamento della mappa delle stazioni
                            mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                            mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                            mMapCityZoom = "15"
                            mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                            '** Impostazione voci id menù
                            literal_menu_items.Text &= "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                                     & oEcofilUser.IdUtenteEncrypted & "-" _
                                                     & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower & "-" _
                                                     & mFilter2 & "-" _
                                                     & GetGlobalResourceObject("controls", "summary_stations").ToString.ToLower & """>" _
                                                     & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>"

                            '** Impostazione dashboard        
                            link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                    & oEcofilUser.IdUtenteEncrypted & "-" _
                                                    & mFilter1 & "-" _
                                                    & mFilter2.ToString

                            link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                            page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                            '** Impostazione briciole di pane
                            literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                    & "<li class=""active"">" & cityInfosData.Nome & "</li>"

                            '** Panello 4
                            literal_panel_4.Text = GetLocalResourceObject("panel_stations_map").ToString
                    End Select
                End If
            Case STATIONS_LIST_IT, STATIONS_LIST_EN
                If mFilter2 = 0 Then
                    TogglePanels(True)

                    '** Caricamento lista delle stazioni
                    mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                    mStationsHeaders = GetGlobalResourceObject("controls", "table_stations_headers")
                    mStationsData = StationsClass.GetStationsData(pConnector, hashParms1, oEcofilUser, True, datStart, datEnd)

                    '** Caricamento della mappa delle stazioni
                    mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                    mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                    mMapCityZoom = "15"
                    mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                    '** Impostazioni titolo lista
                    literal_list_1.Text = GetLocalResourceObject("panel_stations_list").ToString
                    literal_list_2.Text = GetLocalResourceObject("panel_stations_map").ToString

                    '** Impostazione dashboard        
                    link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                            & oEcofilUser.IdUtenteEncrypted & "-" _
                                            & mFilter1

                    link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                    page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("panel_stations_list").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                    '** Impostazione briciole di pane
                    literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & """>" & cityInfosData.Nome & "</a></li>" _
                                            & "<li class=""active"">" & GetLocalResourceObject("panel_stations_list").ToString & "</li>"
                Else
                    TogglePanels(False)

                    '** Caricamento grafico per rifiuto
                    Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, oEcofilUser.IdComune(0), 1)
                    Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms6)
                    literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                    '** Caricamento grafico donut per rifiuto
                    mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                    '** Caricamento grafico timeline
                    literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                    mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value)
                    mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value, True)

                    '** Caricamento lista dei conferimenti
                    mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                    mDisplayItems = 5
                    mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
                    mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms6, New List(Of String) From {"conferimenti.peso"})

                    '** Impostazione dashboard        
                    link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                            & oEcofilUser.IdUtenteEncrypted & "-" _
                                            & mFilter1 & "-" _
                                            & mFilter2.ToString

                    link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                    page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetGlobalResourceObject("controls", "station").ToString & " " & stationInfosData.Nome & " " & stationInfosData.Via & "</small>" & "</h1>"

                    '** Impostazione briciole di pane
                    literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & """>" & cityInfosData.Nome & "</a></li>" _
                                            & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>" _
                                            & "<li class=""active"">" & stationInfosData.Nome & "</li>"

                    '** Panello 4
                    literal_panel_4.Text = GetLocalResourceObject("panel_contributions_list").ToString
                End If
            Case UNIT_DETAIL_IT, UNIT_DETAIL_EN
                TogglePanels(False)

                '** Caricamento grafico per rifiuto
                Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, oEcofilUser.IdComune(0), 1)
                Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms11)
                literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                '** Caricamento grafico donut per rifiuto
                mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                '** Caricamento grafici timeline      
                literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms11, hashGarbageType, hidden_garbage_type.Value)
                mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms11, hashGarbageType, hidden_garbage_type.Value, True)

                '** Caricamento lista dei conferimenti
                mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                mDisplayItems = 10
                mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
                mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms11, New List(Of String) From {"conferimenti.peso"})

                '** Impostazione dashboard        
                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                        & oEcofilUser.IdUtenteEncrypted & "-" _
                                        & mFilter1 & "-" _
                                        & mFilter2.ToString

                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & "</small>" & "</h1>"

                '** Impostazione briciole di pane
                literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & """>" & cityInfosData.Nome & "</a></li>" _
                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "units_link").ToString.Replace("~/", "") & "?city=" & oEcofilUser.IdComune(0).ToString & """>" & GetGlobalResourceObject("pages", "units_text").ToString & "</a></li>" _
                                        & "<li class=""active"">" & nominativoNucleo & "</li>"
            Case Else
                TogglePanels(False, True)

                '** Caricamento grafico per rifiuto
                Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, CType(IIf(mFilter2 > 0, mFilter2, oEcofilUser.IdComune(0)), Integer), 1)
                Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms2)
                literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                '** Caricamento grafico donut per rifiuto
                mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                '** Caricamento grafico timeline
                literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value)
                mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value, True)

                '** Caricamento della mappa delle stazioni
                mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                mMapCityZoom = "15"
                mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                '** Impostazione voci id menù
                literal_menu_items.Text &= "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                         & oEcofilUser.IdUtenteEncrypted & "-" _
                                         & GetGlobalResourceObject("controls", "summary_stations").ToString.ToLower & """>" _
                                         & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>"

                '** Impostazione dashboard        
                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                        & oEcofilUser.IdUtenteEncrypted

                link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                '** Impostazione briciole di pane
                literal_breadcrumb.Text = "<li class=""active"">" & cityInfosData.Nome & "</li>"

                '** Panello 4
                literal_panel_4.Text = GetLocalResourceObject("panel_stations_map").ToString
        End Select
    End Sub

    Private Sub LoadManagerDashboard(ByRef pConnector As PostgreConnector, ByRef oEcofilUser As EcofilUser)
        '** Controllo se selezionato filtro
        Dim booProcapite As Boolean = False
        Dim datStart As Date = Date.MinValue
        Date.TryParse(input_start_date.Text, datStart)
        Dim datEnd As Date = Date.MinValue
        Date.TryParse(input_end_date.Text, datEnd)
        If datEnd <> Date.MinValue Then datEnd = datEnd.AddHours(23).AddMinutes(59).AddSeconds(59)
        If datStart <> Date.MinValue Or datEnd <> Date.MinValue Then booProcapite = True

        Dim hashParms1 As New Hashtable
        Dim hashParms2 As New Hashtable
        Dim hashParms3 As New Hashtable
        Dim hashParms4 As New Hashtable
        Dim hashParms5 As New Hashtable

        '** Codice comune
        If mFilter2 > 0 Then
            hashParms1.Add("idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms2.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms3.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms4.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
            hashParms5.Add("conferimenti.idcomune", mFilter2.ToString & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        Else
            hashParms1.Add("idcomune", Nothing & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        End If
        
        '** Peso
        hashParms2.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms3.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms4.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")
        hashParms5.Add("conferimenti.peso", "0;" & FunctionsClass.DATATYPE_NUMERIC & ";>")

        '** Data
        If datStart <> Date.MinValue Or datEnd <> Date.MinValue Then
            If datStart <> Date.MinValue Then
                hashParms2.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms3.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms4.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
                hashParms5.Add("conferimenti.dataora#1", datStart.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            End If

            If datEnd <> Date.MinValue Then
                hashParms2.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms3.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms4.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
                hashParms5.Add("conferimenti.dataora#2", datEnd.ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";<=")
            End If
        Else
            hashParms2.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms3.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms4.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
            hashParms5.Add("conferimenti.dataora", Now.AddDays(-30).ToString("s") & ";" & FunctionsClass.DATATYPE_STRING & ";>=")
        End If

        '** Tipo rifiuto
        hashParms3.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")
        hashParms5.Add("conferimenti.tiporifiuto", "0" & ";" & FunctionsClass.DATATYPE_NUMERIC & ";=")

        Dim cityInfosData As New FunctionsClass.CityInfos
        Dim stationInfosData As New StationsClass.StationsInfos
        If mFilter2 > 0 Then
            cityInfosData = FunctionsClass.GetCityInfos(pConnector, mFilter2)
            If mFilter4 > 0 Then
                stationInfosData = StationsClass.GetStationInfos(pConnector, mFilter2, mFilter4)

                literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & mFilter2.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_6 & " " & DashboardClass.SUMMARY_CLASS_SM_6, stationInfosData.Nome & "<br />" & stationInfosData.Via) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms4).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms4), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms5).ToString, DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms5), String.Empty).ToString)
            Else
                literal_summary.Text = DashboardClass.CreateSummaryItem(cityInfosData.Nome, "assets/img/comuni/" & mFilter2.ToString & ".jpg", DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_rd").ToString, FunctionsClass.GetTotalRDByCity(pConnector, hashParms2).ToString("0") & " %", DashboardClass.SUMMMARY_PRIZE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_BLUE, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                String.Empty) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms2).ToString("0.00") & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_GREEN, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms2), String.Empty).ToString) _
                                             & DashboardClass.CreateSummaryItem(GetGlobalResourceObject("controls", "summary_garbage_undefined").ToString, FunctionsClass.GetTotalGarbage(pConnector, hashParms3).ToString & " Kg", DashboardClass.SUMMARY_GARBAGE_ICON, _
                                                                                DashboardClass.SUMMARY_COLOR_RED, DashboardClass.SUMMARY_CLASS_MD_3 & " " & DashboardClass.SUMMARY_CLASS_SM_6, DashboardClass.SummaryDetailsEnum.Text, _
                                                                                IIf(booProcapite, FunctionsClass.GetAverageProCapiteInfo(pConnector, hashParms3), String.Empty).ToString)
            End If
        End If

        Select Case mFilter1
            Case CITIES_LIST_IT, CITIES_LIST_EN
                If mFilter2 = 0 Then
                    TogglePanels(True)

                    '** Caricamento lista dei comuni
                    mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                    mCitiesHeaders = GetGlobalResourceObject("controls", "table_cities_headers_noedit")
                    mCitiesData = CityClass.GetCitiesData(pConnector, Nothing, oEcofilUser)

                    '** Caricamento della mappa dei comuni
                    mMapData = CityClass.GetCitiesMap(pConnector, Nothing, oEcofilUser)

                    '** Impostazioni titolo lista
                    literal_list_1.Text = GetLocalResourceObject("panel_cities_list").ToString
                    literal_list_2.Text = GetLocalResourceObject("panel_cities_map").ToString

                    '** Impostazione dashboard        
                    link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                            & oEcofilUser.IdUtenteEncrypted & "-" _
                                            & mFilter1

                    link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                    page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("panel_cities_list").ToString & "</small>" & "</h1>"

                    '** Impostazione briciole di pane
                    literal_breadcrumb.Text = "<li class=""active"">" & GetLocalResourceObject("panel_cities_list").ToString & "</li>"
                Else
                    Select Case mFilter3
                        Case STATIONS_LIST_IT, STATIONS_LIST_EN
                            If mFilter4 = 0 Then
                                TogglePanels(True)

                                '** Caricamento lista delle stazioni
                                mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                                mStationsHeaders = GetGlobalResourceObject("controls", "table_stations_headers")
                                mStationsData = StationsClass.GetStationsData(pConnector, hashParms1, oEcofilUser, False, datStart, datEnd)

                                '** Caricamento della mappa delle stazioni    
                                mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                                mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                                mMapCityZoom = "15"
                                mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                                '** Impostazioni titolo lista
                                literal_list_1.Text = GetLocalResourceObject("panel_stations_list").ToString
                                literal_list_2.Text = GetLocalResourceObject("panel_stations_map").ToString

                                '** Impostazione dashboard        
                                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                        & oEcofilUser.IdUtenteEncrypted & "-" _
                                                        & mFilter1 & "-" _
                                                        & mFilter2.ToString & "-" _
                                                        & mFilter3

                                link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("panel_stations_list").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                                '** Impostazione briciole di pane
                                literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & """>" & cityInfosData.Nome & "</a></li>" _
                                                        & "<li class=""active"">" & GetLocalResourceObject("panel_stations_list").ToString & "</li>"
                            Else
                                TogglePanels(False)

                                '** Caricamento grafico per rifiuto
                                Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, mFilter2, 1)
                                Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms4)
                                literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                                '** Caricamento grafico donut per rifiuto
                                mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                                '** Caricamento grafico timeline
                                literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                                mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value)
                                mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms4, hashGarbageType, hidden_garbage_type.Value, True)

                                ''** Caricamento lista dei conferimenti
                                mTableLabels = GetGlobalResourceObject("controls", "table_labels")
                                mDisplayItems = 5
                                mContributionsHeaders = GetGlobalResourceObject("controls", "table_contributions_headers")
                                mContributionsData = FunctionsClass.GetContributionsData(pConnector, hashParms4, New List(Of String) From {"conferimenti.peso"})

                                '** Impostazione dashboard        
                                link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                        & oEcofilUser.IdUtenteEncrypted & "-" _
                                                        & mFilter1 & "-" _
                                                        & mFilter2.ToString & "-" _
                                                        & mFilter3 & "-" _
                                                        & mFilter4.ToString

                                link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                                page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetGlobalResourceObject("controls", "station").ToString & " " & stationInfosData.Nome & " " & stationInfosData.Via & "</small>" & "</h1>"

                                '** Impostazione briciole di pane
                                literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & """>" & cityInfosData.Nome & "</a></li>" _
                                                        & "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & "-" & mFilter2.ToString & "-" & mFilter3 & """>" & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>" _
                                                        & "<li class=""active"">" & stationInfosData.Nome & "</li>"

                                '** Panello 4
                                literal_panel_4.Text = GetLocalResourceObject("panel_contributions_list").ToString
                            End If
                        Case Else
                            TogglePanels(False, True)

                            '** Caricamento grafico per rifiuto
                            Dim hashGarbageType As Hashtable = FunctionsClass.GetGarbages(pConnector, mFilter2, 1)
                            Dim hashGarbageData As Hashtable = FunctionsClass.GetValuesGarbagesType(pConnector, hashParms2)
                            literal_garbage_summary.Text = DashboardClass.CreateGarbageTable2(hashGarbageType, hashGarbageData)

                            '** Caricamento grafico donut per rifiuto
                            mGarbageDataDonut = FunctionsClass.HashToStructure(hashGarbageType, hashGarbageData)

                            '** Caricamento grafico timeline
                            literal_garbage_buttons.Text = DashboardClass.LoadGarbageButtons(hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataDay = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value)
                            mTimelineGarbageTypesDataSum = FunctionsClass.GetTimelineGarbageTypesData(pConnector, hashParms2, hashGarbageType, hidden_garbage_type.Value, True)

                            '** Caricamento della mappa delle stazioni    
                            mMapCityLan = cityInfosData.Latitudine.ToString.Replace(",", ".")
                            mMapCityLon = cityInfosData.Longitudine.ToString.Replace(",", ".")
                            mMapCityZoom = "15"
                            mMapData = StationsClass.GetStationsMap(pConnector, hashParms1)

                            '** Impostazione voci id menù
                            If Not Page.IsPostBack Then
                                literal_menu_items.Text &= "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" _
                                                         & oEcofilUser.IdUtenteEncrypted & "-" _
                                                         & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower & "-" _
                                                         & mFilter2 & "-" _
                                                         & GetGlobalResourceObject("controls", "summary_stations").ToString.ToLower & """>" _
                                                         & GetLocalResourceObject("panel_stations_list").ToString & "</a></li>"
                            End If
                            
                            '** Impostazione dashboard        
                            link_reload.NavigateUrl = GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" _
                                                    & oEcofilUser.IdUtenteEncrypted & "-" _
                                                    & mFilter1 & "-" _
                                                    & mFilter2.ToString

                            link_clear_filter.NavigateUrl = link_reload.NavigateUrl

                            page_header.Text = "<h1 class=""page-header"">" & GetGlobalResourceObject("pages", "dashboard_text").ToString.ToUpper & " <small>" & GetLocalResourceObject("dashboard_info_base").ToString & " " & cityInfosData.Nome & "</small>" & "</h1>"

                            '** Impostazione briciole di pane
                            literal_breadcrumb.Text = "<li><a href=""" & GetGlobalResourceObject("pages", "dashboard_link").ToString.Replace("~/", "") & "-" & oEcofilUser.IdUtenteEncrypted & "-" & mFilter1 & """>" & GetLocalResourceObject("panel_cities_list").ToString & "</a></li>" _
                                                    & "<li class=""active"">" & cityInfosData.Nome & "</li>"

                            '** Panello 4
                            literal_panel_4.Text = GetLocalResourceObject("panel_stations_map").ToString
                    End Select
                End If
            Case Else
                Response.Redirect(GetGlobalResourceObject("pages", "dashboard_link").ToString & "-" & oEcofilUser.IdUtenteEncrypted & "-" & GetGlobalResourceObject("controls", "summary_cities").ToString.ToLower)
        End Select
    End Sub

    Private Sub TogglePanels(ByVal visible As Boolean, Optional ByVal panel3IsMap As Boolean = False)
        panel_list_1.Visible = visible
        panel_list_2.Visible = visible
        panel_1.Visible = Not visible
        panel_2.Visible = Not visible
        panel_3.Visible = Not visible
        panel_4.Visible = Not visible

        If visible Then
            literal_body_list_2.Text = "<div id=""panel_body_map"" class=""panel-body""><div id=""panel_map"" class=""map""><div id=""google_map""></div></div></div>"
        Else
            If panel3IsMap Then
                literal_panel_body_4.Text = "<div id=""panel_body_map"" class=""panel-body""><div id=""panel_map"" class=""map""><div id=""google_map""></div></div></div>"
            Else
                literal_panel_body_4.Text = "<div class=""panel-body""><div id=""panel_table_3""></div></div>"
            End If
        End If
    End Sub
End Class
