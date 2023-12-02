Imports Microsoft.VisualBasic

Public Class AlertsClass
    Public Shared Function CreateAlert(ByVal alertType As AlertEnum, ByVal message As String) As String
        Dim strResult As String = String.Empty

        Dim strAlertType As String = String.Empty

        Select Case alertType
            Case AlertEnum.SuccessAlert
                strAlertType = "alert-success"
            Case AlertEnum.InfoAlert
                strAlertType = "alert-info"
            Case AlertEnum.WarningAlert
                strAlertType = "alert-warning"
            Case AlertEnum.ErrorAlert
                strAlertType = "alert-danger"
        End Select

        strResult = "<div class=""alert " & strAlertType & " fade in m-b-20"">" _
                  & message _
                  & "<span class=""close"" data-dismiss=""alert"">×</span>" _
                  & "</div>"

        Return strResult
    End Function

    Public Enum AlertEnum
        SuccessAlert
        InfoAlert
        WarningAlert
        ErrorAlert
    End Enum
End Class
