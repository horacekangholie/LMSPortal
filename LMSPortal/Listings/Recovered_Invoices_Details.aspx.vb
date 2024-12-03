
Partial Class Listings_Recovered_Invoices_Details
    Inherits LMSPortalBaseCode

    Dim Currency As String
    Dim TotalAmount As Decimal

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Request.QueryString("Invoice_No") <> "" And Not Request.QueryString("Invoice_No") Is Nothing Then
            LB_PageTitle.Text = "Billed Item Details"
            PopulateGridViewData()
        Else
            Response.Redirect("~/Listings/Recovered_Invoices.aspx")
        End If

        '' Postback link for downloading the invoice file
        RefLink.PostBackUrl = "~/Download/DownloadFile.aspx?Inv_Ref_No=" & Request.QueryString("Invoice_No")
    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {" SELECT Invoice_No, Invoice_Date, Item_Code, Description, Currency, SUM(Amount) As Amount " &
                                      " FROM I_DB_Recovered_Invoice WHERE Invoice_No = '" & Request.QueryString("Invoice_No") & "' " &
                                      " GROUP BY Invoice_No, Invoice_Date, Item_Code, Description, Currency "}

            BuildGridView(GridView1, "GridView1", "Invoice_No")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowFooter = True
        GridViewObj.ShowHeaderWhenEmpty = True
        GridViewObj.DataKeyNames = New String() {DataKeyName}
        GridViewObj.CssClass = "table table-bordered"
        GridViewObj.Style.Add("width", "99.3%")

        '' Header Style
        GridViewObj.HeaderStyle.CssClass = "table-secondary"
        GridViewObj.HeaderStyle.Font.Bold = True
        GridViewObj.HeaderStyle.VerticalAlign = VerticalAlign.Top

        '' Row Style
        GridViewObj.RowStyle.CssClass = "Default"
        GridViewObj.RowStyle.VerticalAlign = VerticalAlign.Middle

        '' Footer Style
        GridViewObj.FooterStyle.CssClass = "table-active"

        '' Pager Style
        GridViewObj.PagerSettings.Mode = PagerButtons.NumericFirstLast
        GridViewObj.PagerSettings.FirstPageText = "First"
        GridViewObj.PagerSettings.LastPageText = "Last"
        GridViewObj.PagerSettings.PageButtonCount = "5"
        GridViewObj.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewObj.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridViewObj.EmptyDataText = "No records found."

        '' Define each Gridview
        Select Case ControlName
            Case "GridView1"
                '' Build GridView Content
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 50
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Invoice_Date", "Item_Code", "Description", "Currency", "Amount"}
                Dim ColSize() As Integer = {50, 100, 400, 50, 50}

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
                    GridViewObj.Columns.Add(Bfield)
                Next

                '' Add templatefield for Edit icon
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

        End Select
    End Sub


    '' Gridview control
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount")).Style.Add("text-align", "right !important")

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Create Edit button at the last column
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(0), LinkButton)
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)

            '' Lock record if invoice has been recovered
            If drv("Item_Code") <> "DMC004" And drv("Item_Code") <> "DMC005" And drv("Item_Code") <> "DMC013" Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.Enabled = True

                DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
                DeleteLinkButton.Enabled = True
            Else
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Enabled = False

                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.Enabled = False
                DeleteLinkButton.Visible = False
            End If

            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_BilledItem_Click

            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_BilledItem_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            '' Total Amount
            TotalAmount += CDec(DataBinder.Eval(e.Row.DataItem, "Amount"))

            '' Currency
            Currency = drv("Currency")  '' Get currency from here to prevent heavy load using Get_Value from DB

            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount")).Style.Add("text-align", "right !important")

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(2).Text = "Total"
            e.Row.Cells(2).HorizontalAlign = HorizontalAlign.Right
            e.Row.Cells(2).Style.Add("padding-right", "30px")
            e.Row.Cells(3).Text = Currency
            e.Row.Cells(4).Text = TotalAmount.ToString("#,##0.00")

            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount")).Style.Add("text-align", "right !important")
        End If

    End Sub

    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData()
    End Sub


    '' Modal control
    Protected Sub DDL_Item_Code_Load(sender As Object, e As EventArgs) Handles DDL_Item_Code.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = " SELECT Value_1 As [Item Code], Value_1 + ' - ' + Value_2 AS [Item Description] " &
                             " FROM DB_Lookup " &
                             " WHERE Lookup_Name = 'Bill Items' " &
                             "   AND Value_1 NOT IN ('DMC004', 'DMC005', 'DMC013', '00000', '00002') " &
                             "   AND Value_4 NOT IN ('Module Licence Key') " &
                             " ORDER BY Value_4, Value_1 "

                DDL_Item_Code.DataSource = GetDataTable(sqlStr)
                DDL_Item_Code.DataTextField = "Item Description"
                DDL_Item_Code.DataValueField = "Item Code"
                DDL_Item_Code.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Currency_Load(sender As Object, e As EventArgs) Handles DDL_Currency.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "
                DDL_Currency.DataSource = GetDataTable(sqlStr)
                DDL_Currency.DataTextField = "Currency"
                DDL_Currency.DataValueField = "Currency"
                DDL_Currency.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub Add_BilledItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddBilledItem.Click
        ModalHeaderBilledItem.Text = "Add Bill Item"
        btnSaveBilledItem.Text = "Save"
        btnCancelBilledItem.Text = "Cancel"

        '' Initialize the field
        DDL_Item_Code.Enabled = True
        TB_Invoice_Date.Text = CDate(Get_Value("SELECT TOP 1 Invoice_Date FROM I_DB_Recovered_Invoice WHERE Invoice_No = '" & Request.QueryString("Invoice_No") & "' ", "Invoice_Date")).ToString("dd MMM yy")
        TB_Invoice_Date.Enabled = False
        TB_Invoice_Date.TextMode = TextBoxMode.SingleLine

        Dim CurrencySelected As String = Get_Value("SELECT TOP 1 Currency FROM I_DB_Recovered_Invoice WHERE Invoice_No = '" & Request.QueryString("Invoice_No") & "' ", "Currency")
        DDL_Currency.SelectedValue = IIf(CurrencySelected = "", "SGD", CurrencySelected)
        'DDL_Currency.Enabled = False

        TB_Amount.Text = String.Empty
        RequiredField_TB_Invoice_Date.Enabled = True
        RequiredField_TB_Amount.Enabled = True

        popupBilledItem.Show()
    End Sub

    Protected Sub Edit_BilledItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderBilledItem.Text = "Update Bill Item"
        btnSaveBilledItem.Text = "Update"
        btnCancelBilledItem.Text = "Cancel"

        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        TB_Old_Item_Code.Text = HttpUtility.HtmlDecode(row.Cells(1).Text)
        DDL_Item_Code.SelectedValue = HttpUtility.HtmlDecode(row.Cells(1).Text)
        DDL_Item_Code.Enabled = False

        Dim CurrencySelected As String = Get_Value("SELECT TOP 1 Currency FROM I_DB_Recovered_Invoice WHERE Invoice_No = '" & Request.QueryString("Invoice_No") & "' ", "Currency")
        DDL_Currency.SelectedValue = IIf(CurrencySelected = "", "SGD", CurrencySelected)
        'DDL_Currency.Enabled = False

        TB_Amount.Text = Trim(HttpUtility.HtmlDecode(row.Cells(4).Text))

        TB_Invoice_Date.Text = HttpUtility.HtmlDecode(row.Cells(0).Text)
        If TB_Invoice_Date.Text <> "" Then
            TB_Invoice_Date.TextMode = TextBoxMode.SingleLine
            TB_Invoice_Date.Enabled = False
            RequiredField_TB_Invoice_Date.Enabled = False
        Else
            TB_Invoice_Date.TextMode = TextBoxMode.Date
            TB_Invoice_Date.Enabled = True
            RequiredField_TB_Invoice_Date.Enabled = True
        End If

        popupBilledItem.Show()
    End Sub

    Protected Sub Save_BilledItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveBilledItem.Click
        Dim Invoice_No As String = Request.QueryString("Invoice_No")
        Dim Invoice_Date As TextBox = pnlAddEditBilledItem.FindControl("TB_Invoice_Date")
        Dim Old_Item_Code As TextBox = pnlAddEditBilledItem.FindControl("TB_Old_Item_Code")
        Dim Item_Code As DropDownList = pnlAddEditBilledItem.FindControl("DDL_Item_Code")
        Dim Currency As DropDownList = pnlAddEditBilledItem.FindControl("DDL_Currency")
        Dim Amount As TextBox = pnlAddEditBilledItem.FindControl("TB_Amount")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_Recovered_Invoice_Bill_Items '" & Invoice_No &
                                                                          "', '" & Invoice_Date.Text &
                                                                          "', '" & Old_Item_Code.Text &
                                                                          "', '" & Item_Code.SelectedValue &
                                                                          "', '" & Currency.SelectedValue &
                                                                          "', '" & Amount.Text & "' "

            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
        Response.Redirect("~/Listings/Recovered_Invoices_Details.aspx?Invoice_No=" & Request.QueryString("Invoice_No"))
        PopulateGridViewData()
    End Sub

    Protected Sub Delete_BilledItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim Invoice_No As String = Request.QueryString("Invoice_No")
        Dim Item_Code As String = HttpUtility.HtmlDecode(row.Cells(1).Text)
        Try
            Dim sqlStr As String = " DELETE FROM DB_Recovered_Invoice WHERE Invoice_No ='" & Invoice_No & "' AND Item_Code = '" & Item_Code & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
        Response.Redirect("~/Listings/Recovered_Invoices_Details.aspx?Invoice_No=" & Request.QueryString("Invoice_No"))
        PopulateGridViewData()
    End Sub

    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Listings/Recovered_Invoices.aspx")
    End Sub

End Class
