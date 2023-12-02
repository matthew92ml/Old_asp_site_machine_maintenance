Public Class DashboardClass
    '** Costanti icone
    Public Const SUMMARY_CITIES_ICON As String = "fa-institution"
    Public Const SUMMARY_STATIONS_ICON As String = "fa-gears"
    Public Const SUMMARY_GARBAGE_ICON As String = "fa-trash-o"
    Public Const SUMMARY_USERS_ICON As String = "fa-users"
    Public Const SUMMARY_INDEX_ICON As String = "fa-bar-chart-o"
    Public Const SUMMARY_LINK_ICON As String = "fa-chain"
    Public Const SUMMARY_RANK_ICON As String = "fa-list-ol"
    Public Const SUMMMARY_PRIZE_ICON As String = "fa-trophy"
    Public Const SUMMARY_BOX_ICON As String = "fa-archive"

    '** Costanti colori
    Public Const SUMMARY_COLOR_PURPLE As String = "purple"
    Public Const SUMMARY_COLOR_BLUE As String = "blue"
    Public Const SUMMARY_COLOR_GREEN As String = "green"
    Public Const SUMMARY_COLOR_RED As String = "red"
    Public Const SUMMARY_COLOR_ORANGE As String = "orange"
    Public Const SUMMARY_COLOR_AQUA As String = "aqua"

    '** Costanti colori font
    Public Const FORECOLOR_BLUE As String = "#348fe2"
    Public Const FORECOLOR_BLUELIGHT As String = "#5da5e8"
    Public Const FORECOLOR_BLUEDARK As String = "#1993E4"
    Public Const FORECOLOR_AQUA As String = "#49b6d6"
    Public Const FORECOLOR_AQUALIGHT As String = "#6dc5de"
    Public Const FORECOLOR_AQUADARK As String = "#3a92ab"
    Public Const FORECOLOR_GREEN As String = "#00acac"
    Public Const FORECOLOR_GREENLIGHT As String = "#33bdbd"
    Public Const FORECOLOR_GREENDARK As String = "#008a8a"
    Public Const FORECOLOR_ORANGE As String = "#f59c1a"
    Public Const FORECOLOR_ORANGELIGHT As String = "#f7b048"
    Public Const FORECOLOR_ORANGEDARK As String = "#c47d15"
    Public Const FORECOLOR_DARK As String = "#2d353c"
    Public Const FORECOLOR_GREY As String = "#b6c2c9"
    Public Const FORECOLOR_PURPLE As String = "#727cb6"
    Public Const FORECOLOR_PURPLELIGHT As String = "#8e96c5"
    Public Const FORECOLOR_PURPLEDARK As String = "#5b6392"
    Public Const FORECOLOR_WHITE As String = "#ffffff"
    Public Const FORECOLOR_YELLOW As String = "#ffdb31"
    Public Const FORECOLOR_BROWN As String = "#763c04"
    Public Const FORECOLOR_RED As String = "#ff5b57"

    '** Costanti classes
    Public Const SUMMARY_CLASS_XS_1 As String = "col-xs-1"
    Public Const SUMMARY_CLASS_XS_2 As String = "col-xs-2"
    Public Const SUMMARY_CLASS_XS_3 As String = "col-xs-3"
    Public Const SUMMARY_CLASS_XS_4 As String = "col-xs-4"
    Public Const SUMMARY_CLASS_XS_5 As String = "col-xs-5"
    Public Const SUMMARY_CLASS_XS_6 As String = "col-xs-6"
    Public Const SUMMARY_CLASS_XS_7 As String = "col-xs-7"
    Public Const SUMMARY_CLASS_XS_8 As String = "col-xs-8"
    Public Const SUMMARY_CLASS_XS_9 As String = "col-xs-9"
    Public Const SUMMARY_CLASS_XS_10 As String = "col-xs-10"
    Public Const SUMMARY_CLASS_XS_11 As String = "col-xs-11"
    Public Const SUMMARY_CLASS_XS_12 As String = "col-xs-12"

    Public Const SUMMARY_CLASS_SM_1 As String = "col-sm-1"
    Public Const SUMMARY_CLASS_SM_2 As String = "col-sm-2"
    Public Const SUMMARY_CLASS_SM_3 As String = "col-sm-3"
    Public Const SUMMARY_CLASS_SM_4 As String = "col-sm-4"
    Public Const SUMMARY_CLASS_SM_5 As String = "col-sm-5"
    Public Const SUMMARY_CLASS_SM_6 As String = "col-sm-6"
    Public Const SUMMARY_CLASS_SM_7 As String = "col-sm-7"
    Public Const SUMMARY_CLASS_SM_8 As String = "col-sm-8"
    Public Const SUMMARY_CLASS_SM_9 As String = "col-sm-9"
    Public Const SUMMARY_CLASS_SM_10 As String = "col-sm-10"
    Public Const SUMMARY_CLASS_SM_11 As String = "col-sm-11"
    Public Const SUMMARY_CLASS_SM_12 As String = "col-sm-12"

    Public Const SUMMARY_CLASS_MD_1 As String = "col-md-1"
    Public Const SUMMARY_CLASS_MD_2 As String = "col-md-2"
    Public Const SUMMARY_CLASS_MD_3 As String = "col-md-3"
    Public Const SUMMARY_CLASS_MD_4 As String = "col-md-4"
    Public Const SUMMARY_CLASS_MD_5 As String = "col-md-5"
    Public Const SUMMARY_CLASS_MD_6 As String = "col-md-6"
    Public Const SUMMARY_CLASS_MD_7 As String = "col-md-7"
    Public Const SUMMARY_CLASS_MD_8 As String = "col-md-8"
    Public Const SUMMARY_CLASS_MD_9 As String = "col-md-9"
    Public Const SUMMARY_CLASS_MD_10 As String = "col-md-10"
    Public Const SUMMARY_CLASS_MD_11 As String = "col-md-11"
    Public Const SUMMARY_CLASS_MD_12 As String = "col-md-12"

    Public Const SUMMARY_CLASS_LG_1 As String = "col-lg-1"
    Public Const SUMMARY_CLASS_LG_2 As String = "col-lg-2"
    Public Const SUMMARY_CLASS_LG_3 As String = "col-lg-3"
    Public Const SUMMARY_CLASS_LG_4 As String = "col-lg-4"
    Public Const SUMMARY_CLASS_LG_5 As String = "col-lg-5"
    Public Const SUMMARY_CLASS_LG_6 As String = "col-lg-6"
    Public Const SUMMARY_CLASS_LG_8 As String = "col-lg-8"
    Public Const SUMMARY_CLASS_LG_9 As String = "col-lg-9"
    Public Const SUMMARY_CLASS_LG_10 As String = "col-lg-10"
    Public Const SUMMARY_CLASS_LG_11 As String = "col-lg-11"
    Public Const SUMMARY_CLASS_LG_12 As String = "col-lg-12"

    Public Shared Function CreateSummaryItem(ByVal title As String, ByVal image As String, ByVal cssClass As String, Optional subtitle As String = "") As String
        Dim strResult As String = "<div class=""" & cssClass & """>" _
                                & "<div class=""widget widget-state bg-white"" style=""height: 149px; border: 1px solid #EAEAEA;"">" _
                                & "<div class=""state-icon"" style=""width: 80px;""><img alt="""" src=""" & image & """ style=""width: inherit;"" /></div>" _
                                & "<div class=""state-info"">" _
                                & "<h3 style=""color: #333333"">" & title & "</h3>"

        If subtitle <> String.Empty Then strResult &= "<p style=""color: #666666; font-size: 16px;"">" & subtitle & "</p>"

        strResult &= "</div></div></div>"

        Return strResult
    End Function

    Public Shared Function CreateSummaryItem(ByVal title As String, ByVal value As String, ByVal icon As String, ByVal color As String, ByVal cssClass As String, ByVal summaryDetailsType As SummaryDetailsEnum, ByVal summaryDetails As String) As String
        Dim strResult As String = "<div class=""" & cssClass & """>" _
                                & "<div class=""widget widget-state bg-" & color & """>" _
                                & "<div class=""state-icon""><i class=""fa " & icon & """></i></div>" _
                                & "<div class=""state-info"">" _
                                & "<h4>" & title & "</h4>" _
                                & "<p>" & value & "</p>" _
                                & "</div>"

        If summaryDetails <> String.Empty Then
            Select Case summaryDetailsType
                Case SummaryDetailsEnum.Link
                    strResult &= "<div class=""state-link pull-right text-right"">" _
                               & "<a href=""" & summaryDetails & """>" & HttpContext.GetGlobalResourceObject("controls", "details").ToString & "<i class=""fa fa-arrow-circle-o-right""></i></a>" _
                               & "</div>"
                Case SummaryDetailsEnum.Text
                    strResult &= "<div class=""state-link pull-right text-right"">" _
                               & summaryDetails _
                               & "</div>"
            End Select
        End If

        strResult &= "</div></div>"

        Return strResult
    End Function

    Public Shared Function CreateGarbageTable(ByRef garbageList As Hashtable, garbageValuesList As Hashtable) As String
        Dim strResult As String = String.Empty

        Dim totGenPeso As Double = 0.0
        Dim totGenPesate As Integer = 0

        If garbageList.Count > 0 Then
            Dim colors As Hashtable = FunctionsClass.GetColors()

            strResult = "<table class=""table table-valign-middle m-b-0"">" _
                      & "<thead>" _
                      & "<tr>" _
                      & "<th>" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_1").ToString _
                      & "</th>" _
                      & "<th class=""text-right"">" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_2").ToString _
                      & "</th>" _
                      & "<th class=""text-right"">" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_3").ToString _
                      & "</th>" _
                      & "<th class=""text-center"">" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_4").ToString _
                      & "</th>" _
                      & "</tr>" _
                      & "</thead>" _
                      & "<tbody>"

            For Each garbage As DictionaryEntry In garbageList
                Dim pesi As List(Of Double) = Nothing

                If garbageValuesList.Contains(garbage.Key.ToString) Then
                    pesi = CType(garbageValuesList(garbage.Key.ToString), List(Of Double))

                    Dim totPeso As Double = 0.0
                    Dim totPesate As Integer = 0
                    For Each peso As Double In pesi
                        totPeso += peso
                        totGenPeso += peso
                        totPesate += 1
                        totGenPesate += 1
                    Next

                    If totPeso > 0.0 Then
                        Dim infos As List(Of String) = CType(garbage.Value, List(Of String))

                        strResult &= "<tr>" _
                                   & "<td><label class=""label label-inverse"" style=""background: " & colors(infos(1)) & ";font-size: 12px;"">" _
                                   & infos(0) _
                                   & "</label></td>" _
                                   & "<td class=""text-right"">" _
                                   & totPeso.ToString("0.00") _
                                   & "</td>" _
                                   & "<td class=""text-right"">" _
                                   & totPesate.ToString _
                                   & "</td>" _
                                   & "<td>" _
                                   & "<div id=""sparkline-" & garbage.Key.ToString & """>" _
                                   & "<canvas width=""86"" height=""23"" style=""display: inline-block; width: 86px; height: 23px; vertical-align: top;""></canvas>" _
                                   & "</div>" _
                                   & "</td>" _
                                   & "</tr>"
                    End If
                End If
            Next

            If totGenPeso > 0.0 Then
                strResult &= "<tr>" _
                           & "<td><strong style=""font-size: 14px;"">" _
                           & HttpContext.GetGlobalResourceObject("controls", "total").ToString _
                           & "</strong></td>" _
                           & "<td class=""text-right"">" _
                           & totGenPeso.ToString("0.00") _
                           & "</td>" _
                           & "<td class=""text-right"">" _
                           & totGenPesate.ToString _
                           & "</td>" _
                           & "<div id=""sparkline-tot"">" _
                           & "<canvas width=""86"" height=""23"" style=""display: inline-block; width: 86px; height: 23px; vertical-align: top;""></canvas>" _
                           & "</div>" _
                           & "</td>" _
                           & "</tr>"
            End If

            strResult &= "</tbody>" _
                       & "</table>"
        End If

        Return strResult
    End Function

    Public Shared Function CreateGarbageTable2(ByRef garbageList As Hashtable, ByRef garbageValuesList As Hashtable) As String
        Dim strResult As String = String.Empty

        Dim totGenPeso As Double = 0.0
        Dim totGenPesate As Integer = 0

        If garbageList.Count > 0 Then
            Dim colors As Hashtable = FunctionsClass.GetColors()

            strResult = "<table class=""table table-valign-middle m-b-0"">" _
                      & "<thead>" _
                      & "<tr>" _
                      & "<th>" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_1").ToString _
                      & "</th>" _
                      & "<th class=""text-right"">" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_2").ToString _
                      & "</th>" _
                      & "<th class=""text-right"">" _
                      & HttpContext.GetGlobalResourceObject("controls", "garbage_types_header_3").ToString _
                      & "</th>" _
                      & "</tr>" _
                      & "</thead>" _
                      & "<tbody>"

            For Each garbage As DictionaryEntry In garbageList
                Dim pesi As List(Of Double) = Nothing

                If garbageValuesList.Contains(garbage.Key.ToString) Then
                    pesi = CType(garbageValuesList(garbage.Key.ToString), List(Of Double))

                    Dim totPeso As Double = 0.0
                    Dim totPesate As Integer = 0
                    For Each peso As Double In pesi
                        totPeso += peso
                        totGenPeso += peso
                        totPesate += 1
                        totGenPesate += 1
                    Next

                    If totPeso > 0.0 Then
                        Dim infos As List(Of String) = CType(garbage.Value, List(Of String))

                        strResult &= "<tr>" _
                                   & "<td><label class=""label label-inverse"" style=""background: " & colors(infos(1)) & ";font-size: 12px;"">" _
                                   & infos(0) _
                                   & "</label></td>" _
                                   & "<td class=""text-right"">" _
                                   & totPeso.ToString("0.00") _
                                   & "</td>" _
                                   & "<td class=""text-right"">" _
                                   & totPesate.ToString _
                                   & "</td>" _
                                   & "</tr>"
                    End If
                End If
            Next

            If totGenPeso > 0.0 Then
                strResult &= "<tr>" _
                           & "<td><strong style=""font-size: 14px;"">" _
                           & HttpContext.GetGlobalResourceObject("controls", "total").ToString _
                           & "</strong></td>" _
                           & "<td class=""text-right"">" _
                           & totGenPeso.ToString("0.00") _
                           & "</td>" _
                           & "<td class=""text-right"">" _
                           & totGenPesate.ToString _
                           & "</td>" _
                           & "</tr>"
            End If

            strResult &= "</tbody>" _
                       & "</table>"
        End If

        Return strResult
    End Function

    Public Shared Function LoadGarbageButtons(ByRef garbageTypes As Hashtable, ByVal garbageSelect As String) As String
        Dim strResult As String = String.Empty

        If garbageSelect = String.Empty Then
            strResult = "<span class=""btn btn-white m-5"" style=""color: #FFFFFF; border: solid " & FunctionsClass.GetColors("dark") & " 2px; background-color: " & FunctionsClass.GetColors("dark") & """ onclick="""">" & HttpContext.GetGlobalResourceObject("controls", "garbage_all").ToString & "</span>"
        Else
            strResult = "<span class=""btn btn-white m-5"" style=""border: solid " & FunctionsClass.GetColors("dark") & " 2px;"" onclick=""ComFun_SelectGarbage('');"">" & HttpContext.GetGlobalResourceObject("controls", "garbage_all").ToString & "</span>"
        End If

        For Each garbage As DictionaryEntry In garbageTypes
            If garbage.Key.ToString = garbageSelect Then
                strResult &= "<span class=""btn btn-white m-5"" style=""color: #FFFFFF; border: solid " & FunctionsClass.GetColors(CType(garbage.Value, List(Of String))(1)) & " 2px; background-color: " & FunctionsClass.GetColors(CType(garbage.Value, List(Of String))(1)) & """>" & CType(garbage.Value, List(Of String))(0) & "</span>"
            Else
                strResult &= "<span class=""btn btn-white m-5"" style=""border: solid " & FunctionsClass.GetColors(CType(garbage.Value, List(Of String))(1)) & " 2px;"" onclick=""ComFun_SelectGarbage('" & garbage.Key.ToString & "');"">" & CType(garbage.Value, List(Of String))(0) & "</span>"
            End If
        Next

        Return strResult
    End Function

    Public Enum SummaryDetailsEnum
        Hide
        Text
        Link
    End Enum
End Class
