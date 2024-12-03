
Partial Class Listings_Module_Licence
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Module Licence"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr = " SELECT ML.Customer_ID AS [Customer ID], C.Name AS [Licensee], ML.PO_No AS [PO No], ML.PO_Date AS [PO Date] " &
                         "      , CASE WHEN Is_Cancelled = 1 THEN 'CANCELLED' ELSE ML.Invoice_No END AS [Invoice No] " &
                         "      , ML.Invoice_Date AS [Invoice Date], ML.Created_Date AS [Created Date] " &
                         "      , S.Name As [Requested By], Case When ML.Chargeable = 1 Then 'Yes' ELSE 'No' END AS [Chargeable] " &
                         "      , dbo.Get_Licence_Inv_Currency([Invoice_No]) AS [Currency] " &
                         "      , dbo.Get_Licence_Inv_Amount([Invoice_No], [PO_No]) AS [Amount] " &
                         "      , Remarks, I.[e.Sense], I.BYOC, I.AI, ML.UID " &
                         " FROM LMS_Module_Licence_Order ML " &
                         " INNER JOIN Master_Customer C ON C.Customer_ID = ML.Customer_ID " &
                         " INNER JOIN Master_Sales_Representative S ON S.Sales_Representative_ID = ML.Sales_Representative_ID " &
                         " INNER JOIN ( Select * FROM (Select UID, Module_Type, Quantity FROM LMS_Module_Licence_Order_Item ) As SourceTable " &
                         "                             PIVOT " &
                         "                             ( " &
                         "		                          SUM(Quantity) " &
                         " 		                          FOR Module_Type IN ([e.Sense], [BYOC], [AI]) " &
                         "	                           ) AS PivotTable ) I ON I.UID = ML.UID " &
                         " WHERE C.Name LIKE '%" & keyword & "%' OR ML.PO_No LIKE '%" & keyword & "%' OR S.Name LIKE '%" & keyword & "%' " &
                         " ORDER BY CASE ML.PO_No WHEN 'NA' THEN 2 ELSE 1 END, ML.Created_Date DESC "

            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' Get the total record
        If keyword <> "" AndAlso CInt(GridView1.Rows.Count.ToString()) > 0 Then
            FooterTotalCount.Visible = True
            FooterTotalCount.Text = "Record(s) found: " & GridView1.Rows.Count.ToString()
        Else
            FooterTotalCount.Visible = False
        End If
    End Sub

    Protected Sub BuildGridView()
        '' GridView Properties
        GridView1.AutoGenerateColumns = False
        GridView1.AllowPaging = True
        GridView1.PageSize = 14
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"Customer ID"}
        GridView1.CssClass = "table table-bordered"

        '' Header Style
        GridView1.HeaderStyle.CssClass = "table-primary"
        GridView1.HeaderStyle.Font.Bold = True
        GridView1.HeaderStyle.VerticalAlign = VerticalAlign.Top

        '' Row Style
        GridView1.RowStyle.CssClass = "Default"
        GridView1.RowStyle.VerticalAlign = VerticalAlign.Middle

        '' Footer Style
        GridView1.FooterStyle.CssClass = "table-active"

        '' Pager Style
        GridView1.PagerSettings.Mode = PagerButtons.NumericFirstLast
        GridView1.PagerSettings.FirstPageText = "First"
        GridView1.PagerSettings.LastPageText = "Last"
        GridView1.PagerSettings.PageButtonCount = "10"
        GridView1.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridView1.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridView1.EmptyDataText = "No records found."

        GridView1.Columns.Clear()

        Dim ColData() As String = {"Licensee", "PO No", "PO Date", "Invoice No", "Invoice Date", "Created Date", "Chargeable", "Currency", "Amount", "e.Sense", "BYOC", "AI", "Requested By"}
        Dim ColSize() As Integer = {200, 100, 50, 100, 50, 50, 50, 20, 80, 50, 50, 50, 200, 100}

        '' add template field for the nested gridview
        Dim Expandfield As TemplateField = New TemplateField()
        Expandfield.ItemTemplate = New LicenceNestedGridViewItemTemplate()   ''LicenceNestedGridViewItemTemplate() common for both App / Product Licence and Module Licence
        Expandfield.HeaderStyle.Width = Unit.Percentage(1)
        GridView1.Columns.Add(Expandfield)

        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColData(i), "_", " ")
            Bfield.HeaderStyle.Width = ColSize(i)
            Bfield.HeaderStyle.Wrap = False
            Bfield.ItemStyle.Wrap = False
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            If Bfield.HeaderText.Contains("Amount") Then
                Bfield.DataFormatString = "{0:#,##0.00}"
            End If
            If Bfield.HeaderText.Contains("e.Sense") Or Bfield.HeaderText.Contains("BYOC") Or Bfield.HeaderText.Contains("AI") Then
                Bfield.HeaderStyle.Width = 65
                Bfield.ItemStyle.Width = 65
            End If
            If Bfield.HeaderText.Contains("Remarks") Then
                Bfield.HeaderStyle.Width = 250
                Bfield.ItemStyle.Wrap = True
            End If
            GridView1.Columns.Add(Bfield)
        Next

        '' Add template field for the delete button
        Dim TField As TemplateField = New TemplateField()
        TField.HeaderStyle.Width = Unit.Percentage(2)
        TField.ItemStyle.Wrap = False
        TField.ItemTemplate = New GridViewItemTemplateControl()
        GridView1.Columns.Add(TField)

    End Sub


    '' Gridview control
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.Header Then
            '' Add additional header row
            Dim HeaderGrid As GridView = DirectCast(sender, GridView)
            Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim HeaderCell = New TableCell()

            For i = 0 To e.Row.Cells.Count - 1
                HeaderCell = New TableCell()
                Select Case i
                    Case 1
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.ColumnSpan = 10
                    Case 10
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "Module Licence"
                        HeaderCell.ColumnSpan = 3
                    Case 13
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.ColumnSpan = 2
                End Select
            Next
            GridViewObj.Controls(0).Controls.AddAt(0, HeaderGridRow)
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Add nested module licence record
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO Date")).Text
            Dim Licence_Code As GridView = TryCast(e.Row.FindControl("gvLicenceList"), GridView)   '' gvLicenceList shared common both App / Product Licence and Module Licence
            Dim Created_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Created Date")).Text

            If Replace(PO_Date, "&nbsp;", "") = "" Then
                PO_Date = ""
            Else
                CDate(PO_Date).ToString("yyyy-MM-dd")
            End If

            Dim query As String = " SELECT [Customer ID] " &
                                  "      , ISNULL([Application Type] + ' (' + Activated_Module_Type + ') ', [Application Type]) AS [Application Type] " &
                                  "      , [OS Type], [Chargeable] " &
                                  "      , [Created Date], [Licence Code], [Status], [MAC Address], [Email] " &
                                  "      , [Activated Date], [Expired Date], [Remarks], [Requested By] " &
                                  " FROM I_LMS_Module_Licence " &
                                  " LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(I_LMS_Module_Licence.[Licence Code], '-', '') " &
                                  " WHERE [Customer ID] = '" & Customer_ID & "'" &
                                  "   AND [PO No] = '" & PO_No & "'"

            If PO_No <> "NA" Then
                query += " AND [PO Date] = '" & PO_Date & "' "
            Else
                query += " AND [Created Date] = '" & Created_Date & "' "
            End If
            query += " ORDER BY [Created Date] DESC "

            Try
                Licence_Code.DataSource = GetDataTable(query)
                Licence_Code.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            Licence_Code.Columns(9).Visible = False  '' hide column Requested By



            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"

            If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" And drv("Invoice No") <> UCase("Cancelled") Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.Text = drv("Invoice No")
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            ElseIf drv("Invoice No") = UCase("Cancelled") Then
                '' if the order is cancelled then display Cancelled
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = drv("Invoice No")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("color", "#999999")
            End If

            '' Edit Button
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            EditLinkButton.CommandName = drv("UID") & "|" & drv("Customer ID")
            EditLinkButton.CommandArgument = drv("PO No")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_ModuleLicence_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            '' Void Button
            Dim VoidLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(1), LinkButton)
            VoidLinkButton.CommandName = drv("Customer ID")
            VoidLinkButton.CommandArgument = drv("PO No")
            VoidLinkButton.Attributes.Add("onclick", "javascript:if (confirm('You are about to void PO No. " & drv("PO No") & "\nClick OK to proceed.\nOtherwise, click Cancel')){return true;} else {return false;}")
            VoidLinkButton.CausesValidation = False
            AddHandler VoidLinkButton.Click, AddressOf VoidLicenceRequest_Click


            '' Lock record if invoice has been recovered or the already cancelled
            If drv("Invoice No") = "" And drv("Invoice No") <> UCase("Cancelled") Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.Enabled = True

                VoidLinkButton.Text = "<i class='bi bi-x-circle'></i>"
                VoidLinkButton.CssClass = "btn btn-xs btn-warning"
                VoidLinkButton.Enabled = True
            Else
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Enabled = False
            End If

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub


    '' record edit control
    Protected Sub Edit_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderModuleLicence.Text = "Recover Invoice"
        btnUpdateModuleLicence.Text = "Update"
        btnCancelModuleLicence.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_PO_No.Text = EditLinkButton.CommandArgument

        Dim EditCommandNameArray As String() = Split(EditLinkButton.CommandName, "|")
        TB_UID.Text = EditCommandNameArray(0)
        TB_Customer_ID.Text = EditCommandNameArray(1)

        TB_SO_No.Text = Get_Value("SELECT ISNULL(SO_No, '') AS SO_No FROM DB_SO_No_By_PO WHERE Customer_ID ='" & TB_Customer_ID.Text & "' AND PO_No = '" & TB_PO_No.Text & "' ", "SO_No")
        TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)
        RequiredField_TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)

        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True

        popupModuleLicence.Show()
    End Sub

    Protected Sub Update_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateModuleLicence.Click
        Dim PO_No As TextBox = pnlUpdateModuleLicence.FindControl("TB_PO_No")
        Dim UID As TextBox = pnlUpdateModuleLicence.FindControl("TB_UID")
        Dim Customer_ID As TextBox = pnlUpdateModuleLicence.FindControl("TB_Customer_ID")

        Dim SO_NO As TextBox = pnlUpdateModuleLicence.FindControl("TB_SO_No")
        Dim Invoice_No As TextBox = pnlUpdateModuleLicence.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlUpdateModuleLicence.FindControl("TB_Invoice_Date")

        Try
            Dim sqlStr As String = " UPDATE LMS_Module_Licence_Order SET Invoice_No = '" & Invoice_No.Text & "', Invoice_Date = '" & Invoice_Date.Text & "' " &
                                   " WHERE UID = '" & UID.Text & "' AND PO_No = '" & PO_No.Text & "' "

            Dim sqlStr1 As String = " UPDATE LMS_Licence SET Invoice_No = '" & Invoice_No.Text & "', Invoice_Date = '" & Invoice_Date.Text & "' " &
                                    " WHERE Customer_ID = '" & Customer_ID.Text & "' AND PO_No = '" & PO_No.Text & "' "

            Dim sqlStr2 As String = " EXEC SP_Insert_Module_Licence_Order_Recovered_Invoice_Items '" & UID.Text & "', '" & Invoice_No.Text & "', '" & Invoice_Date.Text & "', '" & PO_No.Text & "' "

            Dim sqlStr3 As String = " UPDATE DB_SO_No_By_PO SET SO_No = '" & Trim(SO_NO.Text) & "' WHERE Customer_ID ='" & Customer_ID.Text & "' AND PO_No = '" & PO_No.Text & "' "

            RunSQL(sqlStr)
            RunSQL(sqlStr1)
            RunSQL(sqlStr2)
            RunSQL(sqlStr3)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelModuleLicence.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub VoidLicenceRequest_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim VoidLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim PO_No As String = VoidLinkButton.CommandArgument
        Dim Customer_ID As String = VoidLinkButton.CommandName

        Try
            Dim sqlStr As String = "EXEC SP_Void_Licence_Request '" & PO_No & "', '" & Customer_ID & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub


    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
