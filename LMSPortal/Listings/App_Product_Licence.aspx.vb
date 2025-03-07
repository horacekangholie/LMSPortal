
Partial Class Listings_App_Product_Licence
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "App / Product Licence"

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
            Dim sqlStr = " SELECT [Customer ID], [Invoice Bill To], [Licensee], [PO No], [PO Date], [Invoice No], [Invoice Date], SUM([No of Licence]) AS [No of Licence], [Requested By], [Currency], SUM([Amount]) AS [Amount] " &
                         " FROM ( " &
                         "                  SELECT [Customer ID], [Invoice Bill To], [Licensee] " &
                         "                       , [PO No], [PO Date] " &
                         "                       , CASE WHEN [Is Cancelled] = 1 THEN 'CANCELLED' ELSE [Invoice No] END AS [Invoice No], [Invoice Date]  " &
                         "                       , COUNT([Licence Code]) AS [No of Licence] " &
                         "                       , [Requested By] " &
                         "                       , dbo.Get_Licence_Inv_Currency([Invoice No]) AS [Currency] " &
                         "                       , dbo.Get_Licence_Inv_Amount([Invoice No], [PO No]) AS [Amount] " &
                         "                  FROM I_LMS_Licence " &
                         "                  WHERE [PO No] != 'NA' " &
                         "                  GROUP BY [Customer ID], [Invoice Bill To], [Licensee], [PO No], [PO Date], [Invoice No], [Invoice Date], [Requested By], [Is Cancelled] " &
                         "                  UNION ALL " &
                         "                  SELECT [Customer ID], [Invoice Bill To], [Licensee] " &
                         "                       , [PO No], [PO Date] " &
                         "                       , CASE WHEN [Is Cancelled] = 1 THEN 'CANCELLED' ELSE [Invoice No] END AS [Invoice No], [Invoice Date]  " &
                         "                       , COUNT([Licence Code]) AS [No of Licence] " &
                         "                       , '' AS [Requested By] " &
                         "                       , dbo.Get_Licence_Inv_Currency([Invoice No]) AS [Currency] " &
                         "                       , dbo.Get_Licence_Inv_Amount([Invoice No], [PO No]) AS [Amount] " &
                         "                  FROM I_LMS_Licence " &
                         "                  WHERE [PO No] = 'NA' " &
                         "                  GROUP BY [Customer ID], [Invoice Bill To], [Licensee], [PO No], [PO Date], [Invoice No], [Invoice Date], [Requested By], [Is Cancelled] " &
                         "               ) TBL " &
                         " WHERE [Licensee] LIKE '%" & keyword & "%' OR [PO No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%' OR [Invoice Bill To] LIKE '%" & keyword & "%' " &
                         " GROUP BY [Customer ID], [Invoice Bill To], [Licensee], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], [Requested By] "

            sqlStr += " ORDER BY [PO Date] DESC"

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
        GridView1.PageSize = 15
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

        Dim ColData() As String = {"Invoice Bill To", "Licensee", "PO No", "PO Date", "Invoice No", "Invoice Date", "Requested By", "No of Licence", "Currency", "Amount"}
        Dim ColSize() As Integer = {100, 100, 50, 20, 50, 20, 100, 10, 20, 10}

        '' add template field for the nested gridview
        Dim Expandfield As TemplateField = New TemplateField()
        Expandfield.ItemTemplate = New LicenceNestedGridViewItemTemplate()
        Expandfield.HeaderStyle.Width = Unit.Percentage(1)
        GridView1.Columns.Add(Expandfield)

        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColData(i), "_", " ")
            Bfield.HeaderStyle.Width = ColSize(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            If Bfield.HeaderText.Contains("Amount") Then
                Bfield.DataFormatString = "{0:#,##0.00}"
            End If
            Bfield.HeaderStyle.Wrap = False
            Bfield.ItemStyle.Wrap = False
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

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO Date")).Text
            Dim Requested_By As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Text
            Dim AppProductLicence As GridView = TryCast(e.Row.FindControl("gvLicenceList"), GridView)

            If Replace(PO_Date, "&nbsp;", "") = "" Then
                PO_Date = ""
            Else
                PO_Date = CDate(PO_Date).ToString("yyyy-MM-dd")
            End If

            Dim query As String = " SELECT * FROM I_LMS_Licence " &
                                  " WHERE [Customer ID] = '" & Customer_ID & "'  AND [PO No] = '" & PO_No & "' "

            If PO_No <> "NA" Then
                query += " AND [PO Date] = '" & PO_Date & "' "
            End If

            '' Separated record based requestor
            If Len(RemoveHTMLWhiteSpace(Requested_By)) > 0 Then
                query += " AND [Requested By] = '" & Requested_By & "'"
            End If

            query += " ORDER BY [Created Date] DESC "

            Try
                AppProductLicence.DataSource = GetDataTable(query)
                AppProductLicence.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try



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
            EditLinkButton.CommandName = drv("Customer ID")
            EditLinkButton.CommandArgument = drv("PO No")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_AppProductLicence_Click

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

            '' if PO is NA then requestor set to (multiple)
            If PO_No = "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Text = "(multiple)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("color", "#999999")
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
    Protected Sub Edit_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderAppProductLicence.Text = "Recover Invoice"
        btnUpdateAppProductLicence.Text = "Update"
        btnCancelAppProductLicence.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_PO_No.Text = EditLinkButton.CommandArgument
        TB_Customer_ID.Text = EditLinkButton.CommandName

        TB_SO_No.Text = Get_Value("SELECT ISNULL(SO_No, '') AS SO_No FROM DB_SO_No_By_PO WHERE Customer_ID ='" & TB_Customer_ID.Text & "' AND PO_No = '" & TB_PO_No.Text & "' ", "SO_No")
        TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)
        RequiredField_TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)

        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True

        popupAppProductLicence.Show()
    End Sub

    Protected Sub Update_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateAppProductLicence.Click
        Dim Customer_ID As TextBox = pnlUpdateAppProductLicence.FindControl("TB_Customer_ID")
        Dim PO_No As TextBox = pnlUpdateAppProductLicence.FindControl("TB_PO_No")

        Dim SO_NO As TextBox = pnlUpdateAppProductLicence.FindControl("TB_SO_No")
        Dim Invoice_No As TextBox = pnlUpdateAppProductLicence.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlUpdateAppProductLicence.FindControl("TB_Invoice_Date")

        Try
            Dim sqlStr As String = "UPDATE LMS_Licence SET Invoice_No = N'" & Invoice_No.Text & "', Invoice_Date = N'" & Invoice_Date.Text & "' " &
                                   "WHERE Customer_ID = '" & Customer_ID.Text & "' AND PO_No = N'" & PO_No.Text & "' "

            Dim sqlStr1 As String = "EXEC SP_Insert_App_Product_Licence_Order_Recovered_Invoice_Items '" & Customer_ID.Text & "', N'" & PO_No.Text & "' "

            Dim sqlStr2 As String = "UPDATE DB_SO_No_By_PO SET SO_No = N'" & Trim(SO_NO.Text) & "' WHERE Customer_ID ='" & Customer_ID.Text & "' AND PO_No = N'" & PO_No.Text & "' "

            RunSQL(sqlStr)
            RunSQL(sqlStr1)
            RunSQL(sqlStr2)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelAppProductLicence.Click
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
