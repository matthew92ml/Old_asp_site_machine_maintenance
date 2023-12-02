Imports System.IO

Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.Document
Imports iTextSharp.text.Rectangle
Imports iTextSharp.text.DocumentException
Imports iTextSharp.text.Phrase
Imports iTextSharp.text.Image
Imports iTextSharp.text.Font
Imports iTextSharp.text.Font.FontFamily
Imports iTextSharp.text.List
Imports iTextSharp.text.ListItem

Public Class PrintClass
    Private Shared PDFBasFont As pdf.BaseFont = pdf.BaseFont.CreateFont(pdf.BaseFont.TIMES_ROMAN, pdf.BaseFont.CP1252, False)

    Public Shared Function Print(ByVal printType As PrintTypeEnum, ByVal cityId As Integer, ByVal actionId As Integer, ByVal connectionString As String, ByVal password As String) As PrintData
        Dim sqlString As String = String.Empty

        Dim datResponse As New PrintData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = PrintDataEnum.Null

        Try
            '** Connessione al database
            Dim pConnector As New PostgreConnector(connectionString)

            If pConnector.Status Then
                Select Case printType
                    Case PrintTypeEnum.UnitWebInfo
                        PrintUnitWebInfo(pConnector, cityId, actionId, datResponse, password)
                End Select
            Else
                datResponse.Message = PrintDataEnum.ErrorDataBase
            End If
        Catch ex As Exception
            datResponse.Message = PrintDataEnum.ErrorGeneric
        End Try

        Return datResponse
    End Function

    Public Shared Function Print(ByVal printType As PrintTypeEnum, ByVal cityId As Integer, ByVal actionId As Integer, ByRef pConnector As PostgreConnector, ByVal password As String) As PrintData
        Dim sqlString As String = String.Empty

        Dim datResponse As New PrintData
        datResponse.RedirectLink = String.Empty
        datResponse.Message = PrintDataEnum.Null

        Try
            PrintUnitWebInfo(pConnector, cityId, actionId, datResponse, password)
        Catch ex As Exception
            datResponse.Message = PrintDataEnum.ErrorGeneric
        End Try

        Return datResponse
    End Function

    Private Shared Sub PrintUnitWebInfo(ByRef pConnector As PostgreConnector, ByVal cityId As Integer, ByVal actionId As Integer, ByRef datResponse As PrintData, ByVal password As String)
        Try
            Dim PDFPath As String = HttpContext.Current.Request.PhysicalApplicationPath

            '** Lettura dati nucleo
            Dim unitDataInfo As Hashtable = UnitsClass.GetUnitData(pConnector, cityId, actionId)

            '** Impostazioni documento
            Dim PDFDateNow As Date = Now
            Dim PdfDocument As New Document(iTextSharp.text.PageSize.A4)

            Dim PDFDirectoryName As String = PDFPath & "pdf\" & cityId.ToString
            If Not Directory.Exists(PDFDirectoryName) Then Directory.CreateDirectory(PDFDirectoryName)

            Dim PDFNomeFile As String = PDFPath & "pdf\" & cityId.ToString & "\USRCRE-" & actionId.ToString & ".pdf"
            If File.Exists(PDFNomeFile) Then File.Delete(PDFNomeFile)

            PdfWriter.GetInstance(PdfDocument, New FileStream(PDFNomeFile, FileMode.Create))

            PdfDocument.Open()
            PdfDocument.AddTitle("Credenziali Utente Ecofil")

            Dim PDFPage As Integer = 1
            Dim PDFTabella As pdf.PdfPTable

            PDFTabella = New pdf.PdfPTable({50, 50})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.IMAGE, PDFPath & "assets\img\comuni\" & unitDataInfo("city-id").ToString & ".jpg", 100.0F, ALIGN_MIDDLE, ALIGN_LEFT, False, 5))
            PDFTabella.AddCell(NewCell(CellTypeEnum.IMAGE, PDFPath & "assets\img\comuni\" & unitDataInfo("city-id").ToString & "-1.jpg", 60.0F, ALIGN_MIDDLE, ALIGN_CENTER, False, 5))
            PdfDocument.Add(PDFTabella)

            NewLineSpace(PdfDocument, 2)

            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, unitDataInfo("city-name").ToString, 0, ALIGN_TOP, ALIGN_CENTER, False, 5, 0, CellFontEnum.FONT_16_B_B))
            PdfDocument.Add(PDFTabella)

            NewLineSpace(PdfDocument, 1)

            NewLine(PdfDocument, 100, 5)
            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_header").ToString, 0, ALIGN_TOP, ALIGN_CENTER, False, 5, 0, CellFontEnum.FONT_14_B_B))
            PdfDocument.Add(PDFTabella)
            NewLine(PdfDocument, 100, 5)

            NewLineSpace(PdfDocument, 1)

            PDFTabella = New pdf.PdfPTable({30, 70})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_unit_code").ToString, 0, ALIGN_TOP, ALIGN_RIGHT, False, 5, 0, CellFontEnum.FONT_10_B_B))
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, unitDataInfo("unit-code").ToString, 0, ALIGN_TOP, ALIGN_LEFT, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            PDFTabella = New pdf.PdfPTable({30, 70})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_unit_infos").ToString, 0, ALIGN_TOP, ALIGN_RIGHT, False, 5, 0, CellFontEnum.FONT_10_B_B))
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, unitDataInfo("unit-first-name").ToString & " " & unitDataInfo("unit-last-name").ToString & vbCrLf _
                                                        & unitDataInfo("unit-address").ToString, 0, ALIGN_TOP, ALIGN_LEFT, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            NewLineSpace(PdfDocument, 1)

            NewLine(PdfDocument, 100, 5)
            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_body").ToString, 0, ALIGN_TOP, ALIGN_CENTER, False, 5, 0, CellFontEnum.FONT_14_B_B))
            PdfDocument.Add(PDFTabella)
            NewLine(PdfDocument, 100, 5)

            NewLineSpace(PdfDocument, 1)

            PDFTabella = New pdf.PdfPTable({30, 70})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_web").ToString, 0, ALIGN_TOP, ALIGN_RIGHT, False, 5, 0, CellFontEnum.FONT_10_B_B))
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "home_link").ToString, 0, ALIGN_TOP, ALIGN_LEFT, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            PDFTabella = New pdf.PdfPTable({30, 70})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_user_name").ToString, 0, ALIGN_TOP, ALIGN_RIGHT, False, 5, 0, CellFontEnum.FONT_10_B_B))
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, unitDataInfo("unit-username").ToString, 0, ALIGN_TOP, ALIGN_LEFT, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            PDFTabella = New pdf.PdfPTable({30, 70})
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_USRCRE_password").ToString, 0, ALIGN_TOP, ALIGN_RIGHT, False, 5, 0, CellFontEnum.FONT_10_B_B))
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, password, 0, ALIGN_TOP, ALIGN_LEFT, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            NewLineSpace(PdfDocument, 1)

            NewLine(PdfDocument, 100, 5)
            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_HOWTO_header").ToString, 0, ALIGN_TOP, ALIGN_CENTER, False, 5, 0, CellFontEnum.FONT_14_B_B))
            PdfDocument.Add(PDFTabella)
            NewLine(PdfDocument, 100, 5)

            NewLineSpace(PdfDocument, 1)

            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.TEXT, HttpContext.GetGlobalResourceObject("controls", "print_HOWTO_desc").ToString, 0, ALIGN_TOP, ALIGN_JUSTIFIED, False, 5, 0, CellFontEnum.FONT_10_N_B))
            PdfDocument.Add(PDFTabella)

            NewLineSpace(PdfDocument, 20)

            PDFTabella = New pdf.PdfPTable(1)
            PDFTabella.WidthPercentage = 100
            PDFTabella.AddCell(NewCell(CellTypeEnum.IMAGE, PDFPath & "assets\img\logo.jpg", 30.0F, ALIGN_MIDDLE, ALIGN_CENTER, False, 5))
            PdfDocument.Add(PDFTabella)

            PdfDocument.Close()
            PdfDocument.Dispose()

            datResponse.Message = PrintDataEnum.Created
            datResponse.RedirectLink = "pdf\" & cityId.ToString & "\USRCRE-" & actionId.ToString & ".pdf"
            datResponse.ServerPath = PDFPath
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Private Shared Function NewCell(ByVal type As CellTypeEnum, ByVal content As String, ByVal height As Single, ByVal verticalAlignment As Integer, _
                                    ByVal horizontalAlignment As Integer, Optional ByVal border As Boolean = False, Optional _
                                    ByVal padding As Integer = 0, Optional ByVal colSpan As Integer = 0, _
                                    Optional ByVal fontDef As CellFontEnum = CellFontEnum.FONT_NULL, _
                                    Optional ByVal foreColor As iTextSharp.text.BaseColor = Nothing, _
                                    Optional ByVal backgroundColor As iTextSharp.text.BaseColor = Nothing, _
                                    Optional ByVal borderColor As iTextSharp.text.BaseColor = Nothing) As iTextSharp.text.pdf.PdfPCell

        Dim cellResult As pdf.PdfPCell

        Dim cellFont As New Font(PDFBasFont, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)
        Dim cellFontParms() As String = CellFontEnum.FONT_12_N_B.ToString.Split("_")
        Dim cellFontWeight As Integer = iTextSharp.text.Font.NORMAL
        Dim cellFontUnderline As Integer = iTextSharp.text.Font.NORMAL
        If foreColor Is Nothing Then foreColor = BaseColor.BLACK
        Dim cellFontColor As BaseColor = foreColor

        If fontDef <> CellFontEnum.FONT_NULL Then
            cellFontParms = fontDef.ToString.Split("_")

            Select Case cellFontParms(2)
                Case "N"
                    cellFontWeight = iTextSharp.text.Font.NORMAL
                Case "B"
                    cellFontWeight = iTextSharp.text.Font.BOLD
                Case "U"
                    cellFontWeight = iTextSharp.text.Font.UNDERLINE
            End Select

            Select Case cellFontParms(3)
                Case "N"
                    cellFontColor = BaseColor.BLACK
                Case "W"
                    cellFontColor = BaseColor.WHITE
            End Select

            cellFont = New Font(PDFBasFont, CType(cellFontParms(1), Integer), cellFontWeight, cellFontColor)
        End If

        Select Case type
            Case CellTypeEnum.TEXT
                cellResult = New pdf.PdfPCell(New Phrase(content, cellFont))
            Case CellTypeEnum.IMAGE
                cellResult = New pdf.PdfPCell(iTextSharp.text.Image.GetInstance(content))
            Case Else
                cellResult = New pdf.PdfPCell(New Phrase("<ERROR>", cellFont))
        End Select

        cellResult.VerticalAlignment = verticalAlignment
        cellResult.HorizontalAlignment = horizontalAlignment

        If colSpan > 0 Then cellResult.Colspan = colSpan
        If Not border Then cellResult.Border = 0
        If padding > 0 Then cellResult.Padding = padding
        If backgroundColor IsNot Nothing Then cellResult.BackgroundColor = backgroundColor
        If borderColor IsNot Nothing Then cellResult.BorderColor = borderColor

        If height <> 0 Then cellResult.FixedHeight = height

        Return cellResult
    End Function

    Public Shared Sub NewLine(ByRef documento As Document, ByVal width As Integer, Optional ByVal padding As Integer = 0, Optional ByVal color As BaseColor = Nothing)
        Dim tabella As New pdf.PdfPTable(1)
        tabella.WidthPercentage = width

        Dim cell1 As pdf.PdfPCell
        cell1 = New pdf.PdfPCell(New Phrase(String.Empty))
        If color IsNot Nothing Then
            cell1.BorderColorTop = color
        Else
            cell1.BorderColorTop = BaseColor.DARK_GRAY
        End If
        cell1.BorderColorRight = BaseColor.WHITE
        cell1.BorderColorBottom = BaseColor.WHITE
        cell1.BorderColorLeft = BaseColor.WHITE
        If padding > 0 Then cell1.Padding = padding

        tabella.AddCell(cell1)

        documento.Add(tabella)
    End Sub

    Public Shared Sub NewLineSpace(ByRef documento As Document, Optional rows As Integer = 1, Optional padding As Integer = 0)
        Dim tabella As New pdf.PdfPTable(1)
        tabella.WidthPercentage = 100

        Dim strRows As String = String.Empty
        For i As Integer = 0 To rows - 1
            strRows &= vbCrLf
        Next

        tabella.AddCell(NewCell(CellTypeEnum.TEXT, strRows, 0.0F, ALIGN_MIDDLE, ALIGN_CENTER, False, padding, 0, CellFontEnum.FONT_NULL))

        documento.Add(tabella)
    End Sub

    Public Structure PrintData
        Public RedirectLink As String
        Public Message As PrintDataEnum
        Public ServerPath As String
    End Structure

    Public Enum PrintDataEnum
        Created
        ErrorCreatingPdf
        ErrorDataBase
        ErrorGeneric
        Null
    End Enum

    Public Enum PrintTypeEnum
        UnitWebInfo
    End Enum

    Public Enum CellTypeEnum
        IMAGE
        TEXT
    End Enum

    Public Enum CellFontEnum
        FONT_NULL
        FONT_6_N_B
        FONT_6_B_B
        FONT_6_U_B
        FONT_6_N_W
        FONT_6_B_W
        FONT_6_U_W
        FONT_7_N_B
        FONT_7_B_B
        FONT_7_U_B
        FONT_7_N_W
        FONT_7_B_W
        FONT_7_U_W
        FONT_8_N_B
        FONT_8_B_B
        FONT_8_U_B
        FONT_8_N_W
        FONT_8_B_W
        FONT_8_U_W
        FONT_9_N_B
        FONT_9_B_B
        FONT_9_U_B
        FONT_9_N_W
        FONT_9_B_W
        FONT_9_U_W
        FONT_10_N_B
        FONT_10_B_B
        FONT_10_U_B
        FONT_10_N_W
        FONT_10_B_W
        FONT_10_U_W
        FONT_11_N_B
        FONT_11_B_B
        FONT_11_U_B
        FONT_11_N_W
        FONT_11_B_W
        FONT_11_U_W
        FONT_12_N_B
        FONT_12_B_B
        FONT_12_U_B
        FONT_12_N_W
        FONT_12_B_W
        FONT_12_U_W
        FONT_14_N_B
        FONT_14_B_B
        FONT_14_U_B
        FONT_14_N_W
        FONT_14_B_W
        FONT_14_U_W
        FONT_16_N_B
        FONT_16_B_B
        FONT_16_U_B
        FONT_16_N_W
        FONT_16_B_W
        FONT_16_U_W
        FONT_18_N_B
        FONT_18_B_B
        FONT_18_U_B
        FONT_18_N_W
        FONT_18_B_W
        FONT_18_U_W
        FONT_20_N_B
        FONT_20_B_B
        FONT_20_U_B
        FONT_20_N_W
        FONT_20_B_W
        FONT_20_U_W
    End Enum
End Class
