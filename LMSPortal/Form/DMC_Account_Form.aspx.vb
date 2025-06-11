
Imports System.Data

Partial Class Form_DMC_Account_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Register DMC Account"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)
        LB_PageTitle.Text = PageTitle

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" And Not Request.QueryString("Customer_ID") Is Nothing Then
                PopulateFormViewData()
            Else
                Response.Redirect("~/Form/DMC_Account.aspx")
            End If
        End If
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub

    Protected Sub PopulateFormViewData()
        Try
            FormView1.DataSource = GetDataTable(GetSQL(FormView1, Nothing, Nothing))
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("FormView1 - Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal filter As String = Nothing)
        Try
            BuildGridView(GridView1, "GridView1", "Headquarter ID")
            GridView1.DataSource = GetDataTable(GetSQL(GridView1, Nothing, Nothing))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Customer ID")
            GridView2.DataSource = GetDataTable(GetSQL(GridView2, Nothing, Nothing))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Customer ID")
            GridView3.DataSource = GetDataTable(GetSQL(GridView3, Nothing, Nothing))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "Subscription ID")
            GridView4.DataSource = GetDataTable(GetSQL(GridView4, Nothing, filter))
            GridView4.DataBind()

            BuildGridView(GridView5, "GridView5", "ID")
            GridView5.DataSource = GetDataTable(GetSQL(GridView5, Nothing, Nothing))
            GridView5.DataBind()

        Catch ex As Exception
            Response.Write("PopulateGridViewData - Error:  " & ex.Message)
        End Try

        '' Draw last line if page count less than 1
        GridView5.Style.Add("border-bottom", If(GridView5.PageCount < 2, "1px solid #ddd", "1px solid #fff !important"))

        '' Enable Add subscription batch button when number of active store is more than 10
        btnAddSubscriptionBatch.Visible = IIf(GridView2.Rows.Count > 10, True, False)
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
                GridViewObj.PageSize = 15
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Headquarter ID", "Headquarter Name", "Created Date", "Requested By", "Effective Date", "Status"}
                Dim ColSize() As Integer = {50, 300, 50, 300, 50, 50}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add templatefield for Edit icon
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView2"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 15
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Store No", "Store Name", "Banner", "Zone", "Headquarter ID", "Created Date", "Account Type", "Start Date", "End Date", "Public IP", "FTP Host", "FTP User", "FTP Password", "Status"}
                Dim ColSize() As Integer = {80, 200, 200, 80, 80, 120, 100, 120, 100, 100, 100, 100, 100, 120, 25}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    If Bfield.HeaderText.Contains("Banner") Then
                        Bfield.ItemStyle.Wrap = True
                    Else
                        Bfield.ItemStyle.Wrap = False
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add templatefield for Edit icon
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView3"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 15
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Headquarter ID", "Username", "Password", "Email", "Created Date", "Inactive Date", "Status", "Device Type"}
                Dim ColSize() As Integer = {80, 100, 100, 200, 100, 100, 100, 100}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add templatefield for Edit icon
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

                GridViewObj.Columns(0).Visible = False   '' Hide Headquarter_ID column

            Case "GridView4"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 15
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Subscription ID", "Invoice No", "Currency", "Fee", "Payment Method", "Payment Mode", "Status"}
                Dim ColSize() As Integer = {100, 100, 50, 50, 50, 50, 50}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New SubscriptionNestedGridViewItemTemplate()
                Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                GridViewObj.Columns.Add(Expandfield)

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Fee") Then
                        Bfield.DataFormatString = "{0:#,##0.00}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add template field for the delete button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView5"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #ddd")
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Added_Date", "Notes"}
                Dim ColSize() As Unit = {Unit.Percentage(2), Unit.Percentage(95)}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.ItemStyle.Width = ColSize(i)   '' when GridViewObj.ShowHeader is false then use itemstyle to set width
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                        Bfield.ItemStyle.Wrap = False
                        Bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Justify
                    Else
                        Bfield.ItemStyle.Wrap = True
                    End If
                    Bfield.HtmlEncode = False '' to render as html
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add template field for the delete button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

        End Select
    End Sub





    '' FormView control
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub

    Protected Sub DDL_Country_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Country As Label = FormView1.FindControl("LB_Country")
        Dim DDL_Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByText(LB_Country.Text))
        i = IIf(i < 0, 0, i)
        DDL_Country.SelectedIndex = i
    End Sub

    Protected Sub DDL_Type_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Type As Label = FormView1.FindControl("LB_Type")
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim i = DDL_Type.Items.IndexOf(DDL_Type.Items.FindByText(LB_Type.Text))
        i = IIf(i < 0, 0, i)
        DDL_Type.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(DDL_Type.SelectedItem.Text <> "Customer", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
    End Sub

    Protected Sub DDL_Group_ID_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Group As Label = FormView1.FindControl("LB_Group_ID")
        Dim DDL_Group As DropDownList = FormView1.FindControl("DDL_Group_ID")
        Dim i = DDL_Group.Items.IndexOf(DDL_Group.Items.FindByValue(LB_Group.Text))
        i = IIf(i < 0, 0, i)
        DDL_Group.Items(i).Selected = True
    End Sub

    Protected Sub DDL_By_Distributor_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim Type As Label = FormView1.FindControl("LB_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(Type.Text <> "Customer" And Type.Text <> "", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
        Dim By_Distributor As Label = FormView1.FindControl("LB_By_Distributor")
        Dim DDL_By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim i = DDL_By_Distributor.Items.IndexOf(DDL_By_Distributor.Items.FindByValue(By_Distributor.Text))
        DDL_By_Distributor.Items(i).Selected = True
    End Sub





    '' Gridview control
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Create Edit and Delete button at the last column
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview

            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton
            EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            EditLinkButton.CssClass = "btn btn-xs btn-info"
            EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Headquarter ID") & "|" & drv("Headquarter Name") & "|" & drv("Status") & "|" & drv("Requested By")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_Headquarter_Click

            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)
            DeleteLinkButton.Text = IIf(drv("In Used") > 0, "<i class='bi bi-lock'></i>", "<i class='bi bi-trash'></i>")
            DeleteLinkButton.CssClass = IIf(drv("In Used") > 0, "btn btn-xs btn-light disabled", "btn btn-xs btn-danger")
            DeleteLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Headquarter ID")
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Headquarter_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Create Edit and delete button at the last column
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview

            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton
            EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            EditLinkButton.CssClass = "btn btn-xs btn-info"
            EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Headquarter ID") & "|" & drv("Store No") & "|" & drv("Store Name") & "|" & drv("Banner") & "|" & drv("Zone") & "|" & drv("Public IP") & "|" & drv("FTP User") & "|" & drv("FTP Password") & "|" & drv("Type Code") & "|" & drv("Status") & "|" & drv("End Date") & "|" & drv("Store ID")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_Store_Click

            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)
            DeleteLinkButton.Text = IIf(drv("In Used") > 0, "<i class='bi bi-lock'></i>", "<i class='bi bi-trash'></i>")
            DeleteLinkButton.CssClass = IIf(drv("In Used") > 0, "btn btn-xs btn-light disabled", "btn btn-xs btn-danger")
            DeleteLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("ID")
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Store_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            '' Format the ftp user and password font type
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).CssClass = "ftpFont"
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).CssClass = "ftpFont"

            '' Highlight DMC store that is not active
            If e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text <> "Active" Then
                e.Row.Style.Add("color", "#a6a6a6")
                EditLinkButton.Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "Closed", "<i class='bi bi-lock'></i>", "<i class='bi bi-pencil-fill'></i>")
                EditLinkButton.CssClass = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "Closed", "btn btn-xs btn-light disabled", "btn btn-xs btn-info")
            End If
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Create Edit button at the last column
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview

            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton
            EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Headquarter ID") & "|" & drv("Username") & "|" & drv("Password") & "|" & drv("Email") & "|" & drv("Device Type") & "|" & drv("Status") & "|" & drv("Inactive Date")
            EditLinkButton.CssClass = "btn btn-xs btn-info"
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_User_Click

            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)
            DeleteLinkButton.Text = IIf(drv("In Used") > 0, "<i class='bi bi-lock'></i>", "<i class='bi bi-trash'></i>")
            DeleteLinkButton.CssClass = IIf(drv("In Used") > 0, "btn btn-xs btn-light disabled", "btn btn-xs btn-danger")
            DeleteLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("ID")
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_User_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            '' Format password font type
            e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).CssClass = "ftpFont"
        End If
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim Subscription_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim Subscription As GridView = TryCast(e.Row.FindControl("gvContractStore"), GridView)
            Try
                Subscription.DataSource = GetDataTable(GetSQL(Subscription, Nothing, Subscription_ID))
                Subscription.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If drv("Invoice No") <> "" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

            '' Control Button
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(0), LinkButton)
            EditLinkButton.Text = If(Len(drv("Invoice No")) <= 0, "<i class='bi bi-pencil-fill'></i>", "<i class='bi bi-lock'></i>")
            EditLinkButton.CssClass = If(Len(drv("Invoice No")) <= 0, "btn btn-xs btn-info", "btn btn-xs btn-light disabled")
            EditLinkButton.ToolTip = If(Len(drv("Invoice No")) <= 0, "", "Item Locked")
            EditLinkButton.Enabled = Len(drv("Invoice No")) <= 0
            EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Subscription ID")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_Subscription_Click

            Dim AdjustSubscription As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)
            AdjustSubscription.Text = If(Trim(drv("Status")) <> "Expired" And Len(drv("Invoice No")) <= 0, "<i class='bi bi-arrow-left-right'></i>", "<i class='bi bi-lock'></i>")
            AdjustSubscription.CssClass = If(Trim(drv("Status")) <> "Expired" And Len(drv("Invoice No")) <= 0, "btn btn-xs btn-success", "btn btn-xs btn-light disabled")
            AdjustSubscription.ToolTip = If(Trim(drv("Status")) <> "Expired" And Len(drv("Invoice No")) <= 0, "", "Item Locked")
            AdjustSubscription.Enabled = Trim(drv("Status")) <> "Expired" And Len(drv("Invoice No")) <= 0   '' Lock/disable the button when the subscription is in-force
            AdjustSubscription.CommandArgument = e.Row.RowIndex & "|" & drv("Subscription ID")
            AdjustSubscription.CausesValidation = False
            AddHandler AdjustSubscription.Click, AddressOf Adjust_Subscription_Click

            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(2), LinkButton)
            DeleteLinkButton.Text = If(Trim(drv("Status")) = "New", "<i class='bi bi-trash'></i>", "<i class='bi bi-lock'></i>")
            DeleteLinkButton.CssClass = If(Trim(drv("Status")) = "New", "btn btn-xs btn-danger", "btn btn-xs btn-light disabled")
            DeleteLinkButton.ToolTip = If(Trim(drv("Status")) = "New", "", "Item Locked")
            DeleteLinkButton.Enabled = Trim(drv("Status")) = "New"    '' Lock/disable the button when the subscription is in-force
            DeleteLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Subscription ID")
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Subscription_Click


            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button
            AdjustSubscription.Style.Add("margin-right", "5px")   '' add separator between button
        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton

            ' Calculate the difference in days
            Dim NotesAddedDate = CDate(drv("Added_date"))
            Dim CurrentDate = Now()
            Dim DaysDifference As Integer = (CurrentDate.Date - NotesAddedDate.Date).Days

            DeleteLinkButton.Text = IIf(DaysDifference >= 365, "", "<i class='bi bi-trash'></i>")
            DeleteLinkButton.CssClass = IIf(DaysDifference >= 365, "btn btn-xs btn-light disabled", "btn btn-xs btn-danger")
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.CommandArgument = drv("ID")
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Notes_Click
        End If
    End Sub

    Protected Sub GridView_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) _
        Handles GridView1.PageIndexChanging, GridView2.PageIndexChanging, GridView3.PageIndexChanging, GridView4.PageIndexChanging, GridView5.PageIndexChanging

        Dim CurrActiveGV As GridView = CType(sender, GridView)
        CurrActiveGV.PageIndex = e.NewPageIndex

        PopulateFormViewData()
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub



    '' Gridview of listbox
    Protected Sub GridView_Store_List_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView_Store_List.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' click on a row to hightlight and postback to populate value
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridView_Store_List, "Select$" & e.Row.RowIndex)
            e.Row.Attributes("style") = "cursor:pointer"
        End If
    End Sub

    Protected Sub GridView_Store_List_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_Store_List.RowCommand
        If e.CommandName = "Select" Then
            btnAddLineItems.Text = "Update"

            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView_Store_List.Rows(rowIndex)

            '' Get the value from gridviewrow
            Dim Headquarter_ID As String = selectedRow.Cells(0).Text
            Dim Duration As String = selectedRow.Cells(3).Text
            Dim Currency As String = selectedRow.Cells(4).Text
            Dim Subscription_Fee As String = selectedRow.Cells(5).Text
            Dim Subscriber_Group As String = selectedRow.Cells(8).Text

            '' Get the Store_ID from Delete button CommandArgument
            Dim DeleteLinkButton As LinkButton = TryCast(selectedRow.FindControl("DeleteLinkButton"), LinkButton)
            Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
            Dim Store_ID As String = DeleteLinkButtonCommandArgument(2)


            '' Get value of each cell of the selected row
            For i = 0 To selectedRow.Cells.Count - 2
                '' Set the selected Headquarter
                DDL_Subscription_Headquarter.SelectedIndex = DDL_Subscription_Headquarter.Items.IndexOf(DDL_Subscription_Headquarter.Items.FindByValue(selectedRow.Cells(0).Text))

                '' Bind store based on selected headquarter and set the selected value
                Shared_Dropdownlist_Bind(DDL_Subscription_Store, GetSQL(DDL_Subscription_Store, Nothing, Headquarter_ID), "Store_Name", "Store_ID", "Please select", True)
                DDL_Subscription_Store.SelectedIndex = DDL_Subscription_Store.Items.IndexOf(DDL_Subscription_Store.Items.FindByValue(Store_ID))

                '' Bind and set the selected duration and currency
                DDL_Subscription_Duration.SelectedIndex = DDL_Subscription_Duration.Items.IndexOf(DDL_Subscription_Duration.Items.FindByValue(Duration))
                DDL_Subscription_Currency.SelectedIndex = DDL_Subscription_Currency.Items.IndexOf(DDL_Subscription_Currency.Items.FindByValue(Currency))
                TB_Subscription_Fee.Text = Subscription_Fee

                '' Bind the subscriber group
                DDL_Subscription_Subscriber_Group.SelectedIndex = DDL_Subscription_Subscriber_Group.Items.IndexOf(DDL_Subscription_Subscriber_Group.Items.FindByText(Subscriber_Group))
            Next

            '' Highlight the selected row with color
            For Each row As GridViewRow In GridView_Store_List.Rows
                row.BackColor = If(row.RowIndex.Equals(rowIndex), Drawing.ColorTranslator.FromHtml("#eeeeee"), Drawing.Color.Transparent)
            Next
        Else
            btnAddLineItems.Text = "Add"
        End If
        popupSubscription.Show()
    End Sub

    Protected Sub GridView_Store_List_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_Store_List.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Call javascript function for GridView Row highlight effect
            If e.Row.RowType = DataControlRowType.DataRow Then
                e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
                e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
            End If
        End If
    End Sub





    ' Modal Controls

    '' DMC Headquarter
    Protected Sub Add_Headquarter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddHQ.Click
        ModalHeaderHQ.Text = "Add DMC Headquarter"
        btnSaveHQ.Text = "Save"
        btnCancelHQ.Text = "Cancel"

        Try
            ' Determine next Headquarter ID
            Dim sqlStr As String = "SELECT TOP 1 FORMAT(CAST(Headquarter_ID as int) + 1, 'd6') AS Next_HQ_Code FROM DMC_Headquarter ORDER BY Next_HQ_Code DESC "
            Dim dReader = RunSQLExecuteReader(sqlStr)
            Dim NextHQCode As String = "000001"  '' First Headquarter_ID when there is no Headquarter record
            While dReader.Read()
                NextHQCode = dReader("Next_HQ_Code").ToString()
            End While
            TB_Headquarter_ID.Text = NextHQCode
            TB_Headquarter_ID.Enabled = False
            dReader.Close()
        Catch ex As Exception
            Response.Write("Add_Headquarter_Click - Error: " & ex.Message)
        End Try

        ' Initialize fields
        TB_Name.Text = String.Empty
        DDL_HQ_Status.SelectedIndex = 0  '' Select 'Active' when adding new record
        DDL_HQ_Status.Enabled = False

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Sales_Representative, GetSQL(DDL_Sales_Representative, Nothing, Nothing), "Name", "Sales_Representative_ID", "Please select", True)
        DDL_Sales_Representative.SelectedIndex = 0    '' reset the selection when adding new record

        ' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_HQ_Row_Index, TB_Selected_HQ_ID, TB_Selected_HQ_Name, TB_Selected_HQ_Status, TB_Selected_HQ_Requested_By}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupHQ.Show()
    End Sub

    Protected Sub Edit_Headquarter_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderHQ.Text = "Update DMC Headquarter"
        btnSaveHQ.Text = "Update"
        btnCancelHQ.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Sales_Representative, GetSQL(DDL_Sales_Representative, Nothing, Nothing), "Name", "Sales_Representative_ID", "Please select", True)

        ' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_HQ_Row_Index, TB_Selected_HQ_ID, TB_Selected_HQ_Name, TB_Selected_HQ_Status, TB_Selected_HQ_Requested_By}

        ' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        ' populate value to fields
        TB_Headquarter_ID.Text = TB_Selected_HQ_ID.Text
        TB_Headquarter_ID.Enabled = False
        TB_Name.Text = TB_Selected_HQ_Name.Text
        DDL_HQ_Status.SelectedIndex = DDL_HQ_Status.Items.IndexOf(DDL_HQ_Status.Items.FindByText(TB_Selected_HQ_Status.Text))
        DDL_HQ_Status.Enabled = True
        DDL_Sales_Representative.SelectedIndex = DDL_Sales_Representative.Items.IndexOf(DDL_Sales_Representative.Items.FindByText(TB_Selected_HQ_Requested_By.Text))

        popupHQ.Show()
    End Sub

    Protected Sub Save_Headquarter_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveHQ.Click
        Dim Headquarter_ID As TextBox = pnlAddEditHQ.FindControl("TB_Headquarter_ID")
        Dim Name As TextBox = pnlAddEditHQ.FindControl("TB_Name")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Status As DropDownList = pnlAddEditHQ.FindControl("DDL_HQ_Status")
        Dim Sales_Representative As DropDownList = pnlAddEditHQ.FindControl("DDL_Sales_Representative")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_DMC_Headquarter '" & Headquarter_ID.Text &
                                                            "', N'" & EscapeChar(Name.Text) &
                                                             "', '" & Customer_ID &
                                                             "', '" & Status.SelectedValue &
                                                             "', '" & Sales_Representative.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Save_Headquarter_Click - Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub

    Protected Sub Delete_Headquarter_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
        Try
            Dim sqlStr As String = "BEGIN TRANSACTION; " &
                                   "DELETE FROM DMC_Headquarter WHERE Headquarter_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; " &
                                   "DELETE FROM DMC_Headquarter_Sales_Representative WHERE Headquarter_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; " &
                                   "COMMIT;"
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub





    '' DMC Store
    Protected Sub DDL_Store_Headquarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Store_Headquarter.SelectedIndexChanged
        Dim sqlStr As String = " SELECT TOP 1 CAST(SUBSTRING(Store_ID, 8, 4) AS int) + 1 AS Next_Store_Code FROM DMC_Store " &
                               " WHERE Headquarter_ID = (SELECT Headquarter_ID FROM DMC_Headquarter WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' AND Headquarter_ID = '" & DDL_Store_Headquarter.SelectedValue & "') " &
                               " ORDER BY Next_Store_Code DESC "

        Dim dReader = RunSQLExecuteReader(sqlStr)
        Dim NextStoreCode As String = "1"
        While dReader.Read()
            NextStoreCode = dReader("Next_Store_Code")
        End While
        dReader.Close()

        TB_Store_No.Text = IIf(DDL_Store_Headquarter.SelectedValue <> "-1", NextStoreCode, String.Empty)
        TB_Store_No.Enabled = False
        popupStore.Show()
    End Sub

    Protected Sub DDL_Type_Code_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Type_Code.SelectedIndexChanged
        Dim isAccountTypeTrial As Boolean = (DDL_Type_Code.SelectedItem.Text = "Trial")
        trTrialAccountStatusRow.Visible = isAccountTypeTrial
        DDL_Store_Status.Visible = isAccountTypeTrial
        TB_Store_End_Date.Visible = isAccountTypeTrial
        TB_Store_End_Date.Enabled = isAccountTypeTrial

        popupStore.Show()
    End Sub

    Protected Sub DDL_Store_Status_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Store_Status.SelectedIndexChanged
        Dim isAccountStatusActive As Boolean = (DDL_Store_Status.SelectedItem.Text = "Active")
        TB_Store_End_Date.Text = IIf(isAccountStatusActive, String.Empty, CDate(TB_Selected_Store_Account_End_Date.Text).ToString("yyyy-MM-dd"))
        popupStore.Show()
    End Sub

    Protected Sub Add_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddStore.Click
        ModalHeaderStore.Text = "Add DMC Store"
        btnSaveStore.Text = "Save"
        btnCancelStore.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Store_Headquarter, GetSQL(DDL_Store_Headquarter, Nothing, "New"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        DDL_Store_Headquarter.SelectedIndex = 0    '' reset the selection when adding new record
        DDL_Store_Headquarter.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Type_Code, GetSQL(DDL_Type_Code, Nothing, Nothing), "Name", "Code", "Please select", True)
        DDL_Type_Code.SelectedIndex = 0
        DDL_Type_Code.Enabled = True

        ' Initialize fields
        TB_Store_No.Text = String.Empty
        TB_Store_No.Enabled = False
        TB_Store_Name.Text = String.Empty
        TB_Banner.Text = String.Empty
        TB_Zone.Text = String.Empty
        TB_Public_IP.Text = String.Empty
        TB_FTP_User.Text = String.Empty
        TB_FTP_Password.Text = String.Empty

        trTrialAccountStatusRow.Visible = False
        DDL_Store_Status.SelectedIndex = 0
        TB_Store_End_Date.Text = String.Empty

        ' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Store_Row_Index, TB_Selected_Store_HQ_ID, TB_Selected_Store_No, TB_Selected_Store_Name, TB_Selected_Store_Banner, TB_Selected_Store_Zone, TB_Selected_Store_Public_IP, TB_Selected_Store_FTP_User, TB_Selected_Store_FTP_Password, TB_Selected_Store_Account_Type_Code, TB_Selected_Store_Account_Status, TB_Selected_Store_Account_End_Date, TB_Selected_Store_ID}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupStore.Show()
    End Sub

    Protected Sub Edit_Store_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderStore.Text = "Update DMC Store"
        btnSaveStore.Text = "Update"
        btnCancelStore.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Store_Headquarter, GetSQL(DDL_Store_Headquarter, Nothing, "Edit"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Type_Code, GetSQL(DDL_Type_Code, Nothing, Nothing), "Name", "Code", "Please select", True)

        ' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Store_Row_Index, TB_Selected_Store_HQ_ID, TB_Selected_Store_No, TB_Selected_Store_Name, TB_Selected_Store_Banner, TB_Selected_Store_Zone, TB_Selected_Store_Public_IP, TB_Selected_Store_FTP_User, TB_Selected_Store_FTP_Password, TB_Selected_Store_Account_Type_Code, TB_Selected_Store_Account_Status, TB_Selected_Store_Account_End_Date, TB_Selected_Store_ID}

        ' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        ' Populate fields
        DDL_Store_Headquarter.SelectedIndex = DDL_Store_Headquarter.Items.IndexOf(DDL_Store_Headquarter.Items.FindByValue(TB_Selected_Store_HQ_ID.Text))
        DDL_Store_Headquarter.Enabled = False
        TB_Store_No.Text = CInt(TB_Selected_Store_No.Text).ToString()
        TB_Store_No.Enabled = False
        TB_Store_Name.Text = TB_Selected_Store_Name.Text
        TB_Banner.Text = TB_Selected_Store_Banner.Text
        TB_Zone.Text = TB_Selected_Store_Zone.Text
        TB_Public_IP.Text = TB_Selected_Store_Public_IP.Text
        TB_FTP_User.Text = TB_Selected_Store_FTP_User.Text
        TB_FTP_Password.Text = TB_Selected_Store_FTP_Password.Text
        DDL_Type_Code.SelectedIndex = DDL_Type_Code.Items.IndexOf(DDL_Type_Code.Items.FindByValue(TB_Selected_Store_Account_Type_Code.Text))
        DDL_Type_Code.Enabled = IIf(DDL_Type_Code.SelectedValue <> "03", True, False)


        Dim selectedStatus As String = IIf(TB_Selected_Store_Account_Status.Text = "Closed", "Suspended", TB_Selected_Store_Account_Status.Text)
        DDL_Store_Status.SelectedIndex = DDL_Store_Status.Items.IndexOf(DDL_Store_Status.Items.FindByText(selectedStatus))
        DDL_Store_Status.Enabled = IIf(selectedStatus = "Active", False, True)  '' Active does not need to edit, changing of status is handled by code.

        '' Hide / show store status and end date based on the account type
        Dim isAccountTypeTrial As Boolean = (DDL_Type_Code.SelectedItem.Text = "Trial")
        If isAccountTypeTrial Then
            trTrialAccountStatusRow.Visible = isAccountTypeTrial
            DDL_Store_Status.Visible = isAccountTypeTrial
            TB_Store_End_Date.Visible = isAccountTypeTrial
            TB_Store_End_Date.Enabled = isAccountTypeTrial
        Else
            trTrialAccountStatusRow.Visible = (selectedStatus <> "Active")
        End If


        Dim selectedDate As String
        If TB_Selected_Store_Account_End_Date.Text <> "" Then
            selectedDate = CDate(TB_Selected_Store_Account_End_Date.Text).ToString("yyyy-MM-dd")
        Else
            selectedDate = String.Empty
        End If
        TB_Store_End_Date.Text = selectedDate
        TB_Store_End_Date.Enabled = IIf(DDL_Type_Code.SelectedValue <> "01", False, True)

        popupStore.Show()
    End Sub

    Protected Sub Save_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveStore.Click
        Dim Store_No As TextBox = pnlAddEditStore.FindControl("TB_Store_No")
        Dim Name As TextBox = pnlAddEditStore.FindControl("TB_Store_Name")
        Dim Banner As TextBox = pnlAddEditStore.FindControl("TB_Banner")
        Dim Zone As TextBox = pnlAddEditStore.FindControl("TB_Zone")
        Dim Public_IP As TextBox = pnlAddEditStore.FindControl("TB_Public_IP")
        Dim FTP_User As TextBox = pnlAddEditStore.FindControl("TB_FTP_User")
        Dim FTP_Password As TextBox = pnlAddEditStore.FindControl("TB_FTP_Password")

        Dim Status As DropDownList = pnlAddEditStore.FindControl("DDL_Store_Status")
        Dim Is_Active = Status.SelectedValue

        Dim Type_Code As DropDownList = pnlAddEditStore.FindControl("DDL_Type_Code")
        Dim Account_Type = Type_Code.SelectedValue

        Dim End_Date As TextBox = pnlAddEditStore.FindControl("TB_Store_End_Date")

        Dim Headquarter_ID As DropDownList = pnlAddEditStore.FindControl("DDL_Store_Headquarter")

        Dim Store_ID As String = TB_Selected_Store_ID.Text

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_DMC_Store '" & Store_No.Text &
                                                       "', N'" & EscapeChar(Name.Text) &
                                                       "', N'" & EscapeChar(Banner.Text) &
                                                       "', N'" & EscapeChar(Zone.Text) &
                                                       "', '" & Account_Type &
                                                       "', '" & EscapeChar(Public_IP.Text) &
                                                       "', '" & EscapeChar(FTP_User.Text) &
                                                       "', '" & EscapeChar(FTP_Password.Text) &
                                                       "',  " & Is_Active &
                                                        ", '" & End_Date.Text &
                                                       "', '" & Headquarter_ID.SelectedValue &
                                                       "', '" & Store_ID & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Save_Store_Click - Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub

    Protected Sub Delete_Store_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
        Try
            Dim sqlStr As String = "DELETE FROM DMC_Store WHERE ID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Delete_Store_Click - Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub





    '' DMC User
    Protected Sub DDL_User_Headquarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_User_Headquarter.SelectedIndexChanged
        Dim DDL_Headquarter As DropDownList = pnlAddEditStore.FindControl("DDL_User_Headquarter")
        Dim Headquarter_ID = DDL_Headquarter.SelectedValue

        Dim sqlStr As String = "SELECT TOP 1 FORMAT(Cast(Username + 1 AS int), 'd10') AS NextUsername FROM R_DMC_User_Licence " &
                               "WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                               "  AND [Headquarter ID] = '" & DDL_User_Headquarter.SelectedValue & "' ORDER BY Username DESC "

        Dim dReader = RunSQLExecuteReader(sqlStr)
        Dim NextUsername As String = Headquarter_ID & CInt("1").ToString("D4")  '' get the first username
        While dReader.Read()
            NextUsername = dReader("NextUsername")
        End While
        dReader.Close()
        TB_Username.Text = IIf(DDL_Headquarter.SelectedIndex <> 0, NextUsername, String.Empty)

        popupUser.Show()
    End Sub

    Protected Sub DDL_User_Status_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_User_Status.SelectedIndexChanged
        Dim isUserStatusActive As Boolean = (DDL_User_Status.SelectedItem.Text = "Active")
        TB_User_End_Date.Text = IIf(isUserStatusActive, String.Empty, CDate(TB_Selected_User_Account_End_Date.Text).ToString("yyyy-MM-dd"))
        Required_TB_User_End_Date.Enabled = IIf(isUserStatusActive, False, True)
        popupUser.Show()
    End Sub

    Protected Sub Add_User_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddUser.Click
        ModalHeaderUser.Text = "Add DMC User"
        btnSaveUser.Text = "Save"
        btnCancelUser.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_User_Headquarter, GetSQL(DDL_User_Headquarter, Nothing, "New"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        DDL_User_Headquarter.SelectedIndex = 0    '' reset the selection when adding new record
        DDL_User_Headquarter.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Device_Type, GetSQL(DDL_Device_Type, Nothing, Nothing), "Device_Type", "Value_No", "Please select", True)
        DDL_Device_Type.SelectedIndex = 0    '' reset the selection when adding new record
        DDL_Device_Type.Enabled = True

        ' Initialize field
        TB_Username.Text = String.Empty
        TB_Username.Enabled = False
        TB_Password.Text = String.Empty
        TB_Email.Text = String.Empty

        trUserAccountStatusRow.Visible = False '' Hide the row for user account status
        DDL_User_Status.SelectedIndex = 0
        TB_User_End_Date.Text = String.Empty
        TB_User_End_Date.Enabled = True
        Required_TB_User_End_Date.Enabled = False

        ' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_User_Row_Index, TB_Selected_User_HQ_ID, TB_Selected_User_Username, TB_Selected_User_Password, TB_Selected_User_Email, TB_Selected_User_Device_Type, TB_Selected_User_Account_Status, TB_Selected_User_Account_End_Date}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupUser.Show()
    End Sub

    Protected Sub Edit_User_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderUser.Text = "Update DMC User"
        btnSaveUser.Text = "Update"
        btnCancelUser.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_User_Headquarter, GetSQL(DDL_User_Headquarter, Nothing, "Edit"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Device_Type, GetSQL(DDL_Device_Type, Nothing, Nothing), "Device_Type", "Value_No", "Please select", True)

        ' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_User_Row_Index, TB_Selected_User_HQ_ID, TB_Selected_User_Username, TB_Selected_User_Password, TB_Selected_User_Email, TB_Selected_User_Device_Type, TB_Selected_User_Account_Status, TB_Selected_User_Account_End_Date}

        ' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        ' Populate fields
        DDL_User_Headquarter.SelectedIndex = DDL_User_Headquarter.Items.IndexOf(DDL_User_Headquarter.Items.FindByValue(TB_Selected_User_HQ_ID.Text))
        DDL_User_Headquarter.Enabled = False
        TB_Username.Text = TB_Selected_User_Username.Text
        TB_Username.Enabled = False
        TB_Password.Text = TB_Selected_User_Password.Text
        TB_Email.Text = TB_Selected_User_Email.Text

        trUserAccountStatusRow.Visible = True
        DDL_Device_Type.SelectedIndex = DDL_Device_Type.Items.IndexOf(DDL_Device_Type.Items.FindByText(TB_Selected_User_Device_Type.Text))

        Dim selectedStatus As String = IIf(TB_Selected_User_Account_Status.Text = "Closed", "Suspended", TB_Selected_User_Account_Status.Text)
        DDL_User_Status.SelectedIndex = DDL_User_Status.Items.IndexOf(DDL_User_Status.Items.FindByText(selectedStatus))
        DDL_User_Status.Enabled = IIf(TB_Selected_User_Account_Status.Text = "Active", False, True)  '' Active does not need to edit, changing of status is handle by code.

        Dim selectedDate As String
        If TB_Selected_User_Account_End_Date.Text <> "" Then
            selectedDate = CDate(TB_Selected_User_Account_End_Date.Text).ToString("yyyy-MM-dd")
        Else
            selectedDate = String.Empty
        End If
        TB_User_End_Date.Text = selectedDate
        TB_User_End_Date.Enabled = IIf(CDate(Now()).AddMonths(2).ToString("yyyy-MM-dd") > selectedDate, True, False)
        Required_TB_User_End_Date.Enabled = IIf(TB_Selected_User_Account_Status.Text <> "Active", True, False)

        popupUser.Show()
    End Sub

    Protected Sub Save_User_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveUser.Click
        Dim Username As TextBox = pnlAddEditUser.FindControl("TB_Username")
        Dim Password As TextBox = pnlAddEditUser.FindControl("TB_Password")
        Dim Email As TextBox = pnlAddEditUser.FindControl("TB_Email")
        Dim Device As DropDownList = pnlAddEditUser.FindControl("DDL_Device_Type")
        Dim Status As DropDownList = pnlAddEditUser.FindControl("DDL_User_Status")
        Dim Is_Active = Status.SelectedValue
        Dim End_Date As TextBox = pnlAddEditUser.FindControl("TB_User_End_Date")
        Dim Headquarter_ID As DropDownList = pnlAddEditUser.FindControl("DDL_User_Headquarter")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_DMC_User '" & Username.Text &
                                                      "', '" & EscapeChar(Password.Text) &
                                                      "', '" & Email.Text &
                                                      "', '" & Device.SelectedValue &
                                                      "',  " & Is_Active &
                                                       ", '" & End_Date.Text &
                                                      "', '" & Headquarter_ID.SelectedValue & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub

    Protected Sub Delete_User_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
        Try
            Dim sqlStr As String = "DELETE FROM DMC_User WHERE ID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Delete_Store_Click - Error:  " & ex.Message)
        End Try

        PopulateGridViewData()
    End Sub





    '' DMC Subscription
    Protected Sub DDL_Subscription_Headquarter_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Subscription_Headquarter.SelectedIndexChanged
        ' Bind dropdownlist
        Dim selected_Headquarter As String = DDL_Subscription_Headquarter.SelectedValue
        Shared_Dropdownlist_Bind(DDL_Subscription_Store, GetSQL(DDL_Subscription_Store, Nothing, selected_Headquarter), "Store_Name", "Store_ID", "Please select", True)
        popupSubscription.Show()
    End Sub

    Protected Sub DDL_Subscription_Store_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Subscription_Store.SelectedIndexChanged
        btnAddLineItems.Text = "Add"
        TB_Subscription_Fee.Text = String.Empty
        PopulateListbox()
        popupSubscription.Show()
    End Sub

    Protected Sub Add_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddSubscription.Click
        ModalHeaderSubscription.Text = "Add DMC Subscription"
        btnSaveSubscription.Text = "Save"
        btnCancelSubscription.Text = "Cancel"

        btnAddLineItems.Text = "Add"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Subscription_Headquarter, GetSQL(DDL_Subscription_Headquarter, Nothing, "New"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        DDL_Subscription_Headquarter.SelectedIndex = 0    '' reset the selection when adding new record
        DDL_Subscription_Headquarter.Enabled = True

        Dim selected_Headquarter As String = DDL_Subscription_Headquarter.SelectedValue   '' populate store based on headquarter selected
        Shared_Dropdownlist_Bind(DDL_Subscription_Store, GetSQL(DDL_Subscription_Store, Nothing, selected_Headquarter), "Store_Name", "Store_ID", "Please select", True)
        DDL_Subscription_Store.SelectedIndex = 0
        DDL_Subscription_Store.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Duration, GetSQL(DDL_Subscription_Duration, Nothing, Nothing), "Duration_Option", "Duration", "Please select", True)
        DDL_Subscription_Duration.SelectedIndex = 0
        DDL_Subscription_Duration.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Currency, GetSQL(DDL_Subscription_Currency, Nothing, Nothing), "Currency", "Currency", "Please select", True)
        DDL_Subscription_Currency.SelectedIndex = 0
        DDL_Subscription_Currency.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Payment_Method, GetSQL(DDL_Subscription_Payment_Method, Nothing, Nothing), "Payment_Method", "Payment_Method", "Please select", False)
        DDL_Subscription_Payment_Method.SelectedIndex = DDL_Subscription_Payment_Method.Items.IndexOf(DDL_Subscription_Payment_Method.Items.FindByValue("By Store"))
        DDL_Subscription_Payment_Method.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Payment_Mode, GetSQL(DDL_Subscription_Payment_Mode, Nothing, Nothing), "Payment_Mode", "Payment_Mode", "Please select", False)
        DDL_Subscription_Payment_Mode.SelectedIndex = DDL_Subscription_Payment_Mode.Items.IndexOf(DDL_Subscription_Payment_Mode.Items.FindByValue("Pre-paid"))
        DDL_Subscription_Payment_Mode.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Subscriber_Group, GetSQL(DDL_Subscription_Subscriber_Group, Nothing, Nothing), "Subscriber_Group", "Code", "Please select", True)
        DDL_Subscription_Subscriber_Group.SelectedIndex = 0
        DDL_Subscription_Subscriber_Group.Enabled = True

        TB_Subscription_Fee.Text = String.Empty
        subscriptionlistboxerrormsg.Visible = False

        ' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Subscription_Row_Index, TB_Selected_Subscription_ID}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        PopulateListbox()
        popupSubscription.Show()
    End Sub

    Protected Sub Edit_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderSubscription.Text = "Edit DMC Subscription"
        btnSaveSubscription.Text = "Update"
        btnCancelSubscription.Text = "Cancel"

        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Subscription_Headquarter, GetSQL(DDL_Subscription_Headquarter, Nothing, "Edit"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        DDL_Subscription_Headquarter.SelectedIndex = 0
        DDL_Subscription_Headquarter.Enabled = True

        Dim selected_Headquarter As String = DDL_Subscription_Headquarter.SelectedValue   '' populate store based on headquarter selected
        Shared_Dropdownlist_Bind(DDL_Subscription_Store, GetSQL(DDL_Subscription_Store, Nothing, selected_Headquarter), "Store_Name", "Store_ID", "Please select", True)
        DDL_Subscription_Store.SelectedIndex = 0
        DDL_Subscription_Store.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Duration, GetSQL(DDL_Subscription_Duration, Nothing, Nothing), "Duration_Option", "Duration", "Please select", True)
        DDL_Subscription_Duration.SelectedIndex = 0
        DDL_Subscription_Duration.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Currency, GetSQL(DDL_Subscription_Currency, Nothing, Nothing), "Currency", "Currency", "Please select", True)
        DDL_Subscription_Currency.SelectedIndex = 0
        DDL_Subscription_Currency.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Payment_Method, GetSQL(DDL_Subscription_Payment_Method, Nothing, Nothing), "Payment_Method", "Payment_Method", "Please select", False)
        DDL_Subscription_Payment_Method.SelectedIndex = DDL_Subscription_Payment_Method.Items.IndexOf(DDL_Subscription_Payment_Method.Items.FindByValue("By Store"))
        DDL_Subscription_Payment_Method.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Payment_Mode, GetSQL(DDL_Subscription_Payment_Mode, Nothing, Nothing), "Payment_Mode", "Payment_Mode", "Please select", False)
        DDL_Subscription_Payment_Mode.SelectedIndex = DDL_Subscription_Payment_Mode.Items.IndexOf(DDL_Subscription_Payment_Mode.Items.FindByValue("Pre-paid"))
        DDL_Subscription_Payment_Mode.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Subscriber_Group, GetSQL(DDL_Subscription_Subscriber_Group, Nothing, Nothing), "Subscriber_Group", "Code", "Please select", True)
        DDL_Subscription_Subscriber_Group.SelectedIndex = 0
        DDL_Subscription_Subscriber_Group.Enabled = True


        TB_Subscription_Fee.Text = String.Empty
        subscriptionlistboxerrormsg.Visible = False

        ' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Subscription_Row_Index, TB_Selected_Subscription_ID}

        ' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' Get record from existing table and insert to staging table
        Try
            DeleteStaging()  '' Clean the staging table

            Dim sqlStr = "INSERT INTO DMC_Subscription_Staging(Headquarter_ID, Store_ID, Synced_dmcstore_userstoreid, Duration, Currency, Fee, Payment_Method, Payment_Mode, Subscriber_Group) " &
                         "SELECT SUBSTRING(DMC_Subscription.Store_ID, 2, 6)
                               , DMC_Subscription.Store_ID
                               , ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(DMC_Subscription.Store_ID, 8, 4))
	                           , DATEDIFF(MONTH, Start_Date, DATEADD(d, 1, End_Date))
	                           , Currency
	                           , Fee
	                           , Payment_Method
	                           , Payment_Mode
	                           , Subscriber_Group
                          FROM DMC_Subscription
                          INNER JOIN DMC_Store ON DMC_Store.Store_ID = DMC_Subscription.Store_ID
                          WHERE Subscription_ID = '" & EditLinkButtonCommandArgument(1) & "' " &
                          "ORDER BY Synced_dmcstore_userstoreid, DMC_Subscription.Store_ID "

            RunSQL(sqlStr)   '' Insert into staging table with existing data
        Catch ex As Exception
            Response.Write("Error: " & ex.Message & "\nEdit_Tags_Deployment_Click")
        End Try

        '' Populate listbox in edit mode
        PopulateListbox()
        popupSubscription.Show()
    End Sub

    Protected Sub Save_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveSubscription.Click
        Dim Subscription_ID As TextBox = pnlAddEditSubscription.FindControl("TB_Selected_Subscription_ID")
        Dim GridView_Store_List As GridView = pnlAddEditSubscription.FindControl("GridView_Store_List")
        Dim UploadedRecordCount As Integer = GridView_Store_List.Rows.Count

        Dim sqlStr As String = "EXEC SP_CRUD_DMC_Subscription '" & Subscription_ID.Text & "', '" & Request.QueryString("Customer_ID") & "' "
        If UploadedRecordCount > 0 Then
            If btnAddLineItems.Text <> "Update" Then
                Try
                    RunSQL(sqlStr)
                Catch ex As Exception
                    Response.Write("Error: " & ex.Message)
                End Try
                PopulateGridViewData()
                DeleteStaging()
            Else
                AlertMessageMsgBox("You have not updated the selected row.")
                popupSubscription.Show()
            End If
        Else
            subscriptionlistboxerrormsg.Visible = True
            popupSubscription.Show()
        End If
    End Sub

    Protected Sub Delete_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
        Dim sqlStr As String = "DELETE FROM DMC_Subscription WHERE Subscription_ID ='" & DeleteLinkButtonCommandArgument(1) & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        PopulateGridViewData()
    End Sub

    Protected Sub Adjust_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderAdjustSubscriptionPeriod.Text = "Adjust Subscription Period"
        btnSaveAdjustSubscriptionPeriod.Text = "Save"
        btnCancelAdjustSubscriptionPeriod.Text = "Cancel"

        ' Initialiaze the hidden fields
        TB_New_Start_Date.Text = String.Empty
        DDL_Adjust_Subscription_Duration.SelectedIndex = 0

        ' Get row command argument, get the value and pass them to hidden fields
        Dim AdjustLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim AdjustLinkButtonCommandArgument As Array = Split(AdjustLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Adjust_Subscription_Row_Index, TB_Selected_Adjust_Subscription_ID}

        ' Loop through to assign value to hidden fields
        For i = 0 To AdjustLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = AdjustLinkButtonCommandArgument(i)
        Next

        ' Build dropdownlist for the adjust subscription period duration
        Shared_Dropdownlist_Bind(DDL_Adjust_Subscription_Duration, GetSQL(DDL_Adjust_Subscription_Duration, Nothing, Nothing), "Duration_Option", "Duration", "Please select", True)
        DDL_Adjust_Subscription_Duration.SelectedIndex = 0
        DDL_Adjust_Subscription_Duration.Enabled = True

        popupAdjustSubscriptionPeriod.Show()
    End Sub

    Protected Sub Save_Adjust_Subscription_Period_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveAdjustSubscriptionPeriod.Click
        Dim Adjusted_Start_Date As String = TB_New_Start_Date.Text
        Dim Duration As String = DDL_Adjust_Subscription_Duration.Text
        Dim Subscription_ID As String = TB_Selected_Adjust_Subscription_ID.Text
        Dim sqlStr As String = "UPDATE DMC_Subscription SET Start_Date = '" & Adjusted_Start_Date & "', End_Date = DATEADD(DAY, -1, DATEADD(MONTH, " & Duration & ", '" & Adjusted_Start_Date & "')) WHERE Subscription_ID = '" & Subscription_ID & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        PopulateGridViewData()
    End Sub

    Protected Sub Cancel_Subscription_Click(sender As Object, e As EventArgs) Handles btnCancelSubscription.Click
        DeleteStaging()
    End Sub



    '' Filter Subscription by Store
    Protected Sub DDL_Store_Contract_List_Load(sender As Object, e As EventArgs) Handles DDL_Store_Contract_List.Load
        If Not IsPostBack Then
            Try
                Shared_Dropdownlist_Bind(DDL_Store_Contract_List, GetSQL(DDL_Store_Contract_List, Nothing, Nothing), "Name", "Store_ID", "", True)

                '' Enable store list dropdownlist when billed store is more than 1
                LB_Store_Contract_List.Visible = ((DDL_Store_Contract_List.Items.Count - 1) > 1)
                DDL_Store_Contract_List.Visible = ((DDL_Store_Contract_List.Items.Count - 1) > 1)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Store_Contract_List_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Store_Contract_List.SelectedIndexChanged
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub




    '' DMC Subscription in batch
    Protected Sub Add_Subscription_Batch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddSubscriptionBatch.Click
        ModalHeaderSubscriptionBatch.Text = "Add DMC Subscription (Batch)"
        btnSaveSubscriptionBatch.Text = "Save"
        btnCancelSubscriptionBatch.Text = "Cancel"


        ' Bind dropdownlist
        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Headquarter, GetSQL(DDL_Subscription_Batch_Headquarter, Nothing, "New"), "Headquarter_Name", "Headquarter_ID", "Please select", True)
        DDL_Subscription_Batch_Headquarter.SelectedIndex = 0    '' reset the selection when adding new record
        DDL_Subscription_Batch_Headquarter.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Duration, GetSQL(DDL_Subscription_Batch_Duration, Nothing, Nothing), "Duration_Option", "Duration", "Please select", True)
        DDL_Subscription_Batch_Duration.SelectedIndex = 0
        DDL_Subscription_Batch_Duration.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Currency, GetSQL(DDL_Subscription_Batch_Currency, Nothing, Nothing), "Currency", "Currency", "Please select", True)
        DDL_Subscription_Batch_Currency.SelectedIndex = 0
        DDL_Subscription_Batch_Currency.Enabled = True

        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Payment_Method, GetSQL(DDL_Subscription_Batch_Payment_Method, Nothing, Nothing), "Payment_Method", "Payment_Method", "Please select", False)
        DDL_Subscription_Batch_Payment_Method.SelectedIndex = DDL_Subscription_Batch_Payment_Method.Items.IndexOf(DDL_Subscription_Batch_Payment_Method.Items.FindByValue("By Store"))
        DDL_Subscription_Batch_Payment_Method.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Payment_Mode, GetSQL(DDL_Subscription_Batch_Payment_Mode, Nothing, Nothing), "Payment_Mode", "Payment_Mode", "Please select", False)
        DDL_Subscription_Batch_Payment_Mode.SelectedIndex = DDL_Subscription_Batch_Payment_Mode.Items.IndexOf(DDL_Subscription_Batch_Payment_Mode.Items.FindByValue("Pre-paid"))
        DDL_Subscription_Batch_Payment_Mode.Enabled = False

        Shared_Dropdownlist_Bind(DDL_Subscription_Batch_Subscriber_Group, GetSQL(DDL_Subscription_Batch_Subscriber_Group, Nothing, Nothing), "Subscriber_Group", "Code", "Please select", True)
        DDL_Subscription_Batch_Subscriber_Group.SelectedIndex = 0
        DDL_Subscription_Batch_Subscriber_Group.Enabled = True

        TB_Subscription_Batch_Fee.Text = String.Empty


        popupSubscriptionBatch.Show()
    End Sub

    Protected Sub Save_Subscription_Batch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveSubscriptionBatch.Click
        Dim DDL_Subscription_Batch_Headquarter As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Headquarter")
        Dim DDL_Subscription_Batch_Duration As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Duration")
        Dim DDL_Subscription_Batch_Currency As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Currency")
        Dim TB_Subscription_Batch_Fee As TextBox = pnlAddEditSubscriptionBatch.FindControl("TB_Subscription_Batch_Fee")
        Dim DDL_Subscription_Batch_Payment_Method As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Payment_Method")
        Dim DDL_Subscription_Batch_Payment_Mode As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Payment_Mode")
        Dim DDL_Subscription_Batch_Subscriber_Group As DropDownList = pnlAddEditSubscriptionBatch.FindControl("DDL_Subscription_Batch_Subscriber_Group")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_DMC_Subscription_Batch N'" & DDL_Subscription_Batch_Headquarter.SelectedValue &
                                                                    "', N'" & DDL_Subscription_Batch_Duration.SelectedValue &
                                                                    "', N'" & DDL_Subscription_Batch_Currency.SelectedValue &
                                                                    "', N'" & TB_Subscription_Batch_Fee.Text &
                                                                    "', N'" & DDL_Subscription_Batch_Payment_Method.SelectedValue &
                                                                    "', N'" & DDL_Subscription_Batch_Payment_Mode.SelectedValue &
                                                                    "', N'" & DDL_Subscription_Batch_Subscriber_Group.SelectedValue & "' "

            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Save Subscription Batch - Error: " & ex.Message)
        End Try

        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub






    '' Subscription Line Items
    Protected Sub AddLineItems_Click(sender As Object, e As EventArgs) Handles btnAddLineItems.Click
        Dim DDL_Subscription_Headquarter As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Headquarter")
        Dim DDL_Subscription_Store As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Store")
        Dim DDL_Subscription_Duration As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Duration")
        Dim DDL_Subscription_Currency As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Currency")
        Dim TB_Subscription_Fee As TextBox = pnlAddEditSubscription.FindControl("TB_Subscription_Fee")
        Dim DDL_Subscription_Payment_Method As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Payment_Method")
        Dim DDL_Subscription_Payment_Mode As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Payment_Mode")
        Dim DDL_Subscription_Subscriber_Group As DropDownList = pnlAddEditSubscription.FindControl("DDL_Subscription_Subscriber_Group")

        '' If no billed store, prevent record to be added into line item
        If DDL_Subscription_Store.SelectedValue.ToString() <> "" Then
            Dim sqlStr As String = " EXEC SP_Insert_Subscription_Staging '" & DDL_Subscription_Headquarter.SelectedValue &
                                                                     "', '" & DDL_Subscription_Store.SelectedValue &
                                                                     "', '" & DDL_Subscription_Duration.SelectedValue &
                                                                     "', '" & DDL_Subscription_Currency.Text &
                                                                     "', '" & TB_Subscription_Fee.Text &
                                                                     "', '" & DDL_Subscription_Payment_Method.SelectedValue &
                                                                     "', '" & DDL_Subscription_Payment_Mode.SelectedValue &
                                                                     "', '" & DDL_Subscription_Subscriber_Group.SelectedValue & "'"
            RunSQL(sqlStr)

            ' Reset field when item added
            'DDL_Subscription_Headquarter.SelectedIndex = 0
            DDL_Subscription_Store.SelectedIndex = 0
            'DDL_Subscription_Duration.SelectedIndex = 0
            'DDL_Subscription_Currency.SelectedIndex = 0
            TB_Subscription_Fee.Text = String.Empty
            'DDL_Subscription_Subscriber_Group.SelectedIndex = 0


            '' Change the button to Add
            btnAddLineItems.Text = "Add"
        Else
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('" & DDL_Subscription_Headquarter.SelectedItem.ToString & " currently has no billed store available to be added to contract.');", True)
        End If

        subscriptionlistboxerrormsg.Visible = False

        PopulateListbox()
        popupSubscription.Show()
    End Sub

    Protected Sub ClearLineItems_Click(sender As Object, e As EventArgs) Handles btnClearLineItems.Click
        ' Reset field click on clear line items button
        DDL_Subscription_Headquarter.SelectedIndex = 0
        DDL_Subscription_Store.SelectedIndex = 0
        DDL_Subscription_Duration.SelectedIndex = 0
        DDL_Subscription_Currency.SelectedIndex = 0
        TB_Subscription_Fee.Text = String.Empty
        DDL_Subscription_Subscriber_Group.SelectedIndex = 0

        DeleteStaging()
        subscriptionlistboxerrormsg.Visible = False
        PopulateListbox()
        popupSubscription.Show()
    End Sub

    Protected Sub PopulateListbox()
        Try
            GridView_Store_List.DataSource = GetDataTable(GetSQL(GridView_Store_List, Nothing, Nothing))
            GridView_Store_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub DeleteLineItem_Click(sender As Object, e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = " DELETE FROM DMC_Subscription_Staging " &
                                   " WHERE Headquarter_ID = '" & DeleteLinkButtonCommandArgument(1) & "' " &
                                   "   AND Store_ID = '" & DeleteLinkButtonCommandArgument(2) & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Record fail to delete.');", True)
        End Try

        PopulateListbox()
        popupSubscription.Show()
    End Sub


    '' Data View for In-memory subcription store update
    Private Property SubscriptionItems As DataTable
        Get
            If ViewState("SubscriptionItems") Is Nothing Then
                Dim dt As New DataTable()
                dt.Columns.Add("Headquarter_ID", GetType(String))
                dt.Columns.Add("Store_ID", GetType(String))
                dt.Columns.Add("Store_No", GetType(String))
                dt.Columns.Add("Duration", GetType(String))
                dt.Columns.Add("Currency", GetType(String))
                dt.Columns.Add("Fee", GetType(Decimal))
                dt.Columns.Add("Payment_Method", GetType(String))
                dt.Columns.Add("Payment_Mode", GetType(String))
                dt.Columns.Add("Subscriber_Group", GetType(String))

                ViewState("SubscriptionItems") = dt
            End If

            Return CType(ViewState("SubscriptionItems"), DataTable)
        End Get
        Set(value As DataTable)
            ViewState("SubscriptionItems") = value
        End Set
    End Property





    '' Account Notes
    Protected Sub Add_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNotes.Click
        ModalHeaderNotes.Text = "Add Notes"
        btnSaveNotes.Text = "Save"
        btnCancelNotes.Text = "Cancel"

        TB_Notes.Text = String.Empty
        popupNotes.Show()
    End Sub

    Protected Sub Save_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveNotes.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Notes As TextBox = pnlAddEditNotes.FindControl("TB_Notes")
        Dim Notes_For As String = "DMC Account"
        Dim BtnCommand As Button = TryCast(sender, Button)

        Dim sqlStr As String = " EXEC SP_CRUD_Notes '" & Customer_ID & "', N'" & Replace(Notes.Text, "'", "''") & "', '" & Notes_For & "', '" & BtnCommand.CommandArgument & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub

    Protected Sub Delete_Notes_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)
        Dim sqlStr As String = " DELETE FROM DB_Account_Notes WHERE ID = " & BtnCommand.CommandArgument
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
    End Sub





    '' Common procedure
    Protected Function GetSQL(ByVal controlObj As Object, Optional ByVal keyword As String = Nothing, Optional ByVal filter As String = Nothing) As String
        Dim sqlStr As String = Nothing
        Dim objID As String = String.Empty
        Dim Search_Keyword As String = EscapeChar(keyword)

        ' Check if the object has an ID property or field
        Dim idProperty = controlObj.GetType().GetProperty("ID")
        If idProperty IsNot Nothing AndAlso idProperty.PropertyType Is GetType(String) Then
            objID = idProperty.GetValue(controlObj)?.ToString()
        End If

        ' Form SQL String based on control object ID
        Select Case objID
            Case "FormView1"
                sqlStr = "SELECT * FROM Master_Customer WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' "

            Case "GridView1"
                sqlStr = "SELECT * FROM R_Headquarter_Sales_Representative WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ORDER BY [Headquarter ID] "

            Case "GridView2"
                sqlStr = "SELECT * FROM R_DMC_Store_Licence WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ORDER BY [Headquarter ID], [Store No] "

            Case "GridView3"
                sqlStr = "SELECT * FROM R_DMC_User_Licence WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ORDER BY [Username] "

            Case "GridView4"
                sqlStr = "SELECT DISTINCT * FROM R_DMC_Subscription WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' AND [Payment Status] NOT IN ('Cancelled') AND [Subscription ID] IN (SELECT Subscription_ID FROM R_DMC_Subscription_Detail WHERE Store_ID LIKE '%" & IIf(filter = "-1", Nothing, filter) & "%') ORDER BY [Subscription ID] DESC "

            Case "GridView5"
                sqlStr = "SELECT * FROM DB_Account_Notes WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' AND Notes_For = 'DMC Account' ORDER BY Added_Date DESC, ID DESC "

            Case "DDL_Sales_Representative"
                sqlStr = "SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "

            Case "DDL_Store_Headquarter", "DDL_User_Headquarter", "DDL_Subscription_Headquarter", "DDL_Subscription_Batch_Headquarter"
                sqlStr = "SELECT Headquarter_ID, Headquarter_ID + ' ' + Name AS Headquarter_Name, Customer_ID FROM DMC_Headquarter " &
                         "WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' " & IIf(filter = "New", "AND Is_Active = 1 ", "") &
                         "ORDER BY Headquarter_ID "
            Case "DDL_Type_Code"
                sqlStr = "SELECT Code, Name FROM Master_Account_Type "

            Case "DDL_Device_Type"
                sqlStr = "SELECT Value_2 As Value_No, Value_1 As Device_Type FROM DB_Lookup WHERE Lookup_Name = 'Device Type' ORDER BY Value_2 "

            Case "gvContractStore"
                sqlStr = "SELECT * FROM R_DMC_Subscription_Detail WHERE Subscription_ID ='" & filter & "' ORDER BY Store_No "

            Case "DDL_Subscription_Store"
                sqlStr = "SELECT Store_ID, ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' ' + S.Name AS Store_Name FROM DMC_Store S " &
                         "LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID " &
                         "WHERE S.Is_Active = 1 AND Customer_ID = '" & Request.QueryString("Customer_ID") & "' " &
                         "  AND S.Headquarter_ID = '" & filter & "' AND S.Account_Type IN ('03') " &
                         "ORDER BY ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' ' + S.Name "
            Case "DDL_Store_Contract_List"
                sqlStr = "SELECT Store_ID, ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' - ' + Name AS Name FROM DMC_Store WHERE Headquarter_ID IN (SELECT Headquarter_ID FROM DMC_Headquarter WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "') AND Account_Type = '03' " &
                         "ORDER BY ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' - ' + Name "

            Case "DDL_Subscription_Duration", "DDL_Subscription_Batch_Duration", "DDL_Adjust_Subscription_Duration"
                sqlStr = "SELECT CAST(Value_2 AS int) AS Duration, Value_1 AS Duration_Option FROM DB_Lookup WHERE Lookup_Name = 'Payment Period' ORDER BY Duration "

            Case "DDL_Subscription_Currency", "DDL_Subscription_Batch_Currency"
                sqlStr = "SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "

            Case "DDL_Subscription_Payment_Method", "DDL_Subscription_Batch_Payment_Method"
                sqlStr = "SELECT Value_1 AS Payment_Method FROM DB_Lookup WHERE Lookup_Name = 'Payment Method' "

            Case "DDL_Subscription_Payment_Mode", "DDL_Subscription_Batch_Payment_Mode"
                sqlStr = "SELECT Value_1 AS Payment_Mode FROM DB_Lookup WHERE Lookup_Name = 'Payment Mode' "

            Case "DDL_Subscription_Subscriber_Group", "DDL_Subscription_Batch_Subscriber_Group"
                sqlStr = "SELECT Value_2 AS Code, Value_1 AS Subscriber_Group FROM DB_Lookup WHERE Lookup_Name = 'Subscriber Group' "

            Case "GridView_Store_List"
                sqlStr = "SELECT Headquarter_ID " &
                         "     , Store_ID " &
                         "     , CASE WHEN LEN(Synced_dmcstore_userstoreid) > 0 THEN Synced_dmcstore_userstoreid ELSE SUBSTRING(Store_ID, 8, 4) END AS Store_No " &
                         "     , Duration, Currency, Fee, Payment_Method, Payment_Mode " &
                         "     , CASE WHEN Subscriber_Group = 'H' THEN 'Hospitality' " &
                         "            WHEN Subscriber_Group = 'R' THEN 'Retail' " &
                         "            END AS Subscriber_Group " &
                         "FROM DMC_Subscription_Staging " &
                         "ORDER BY ID DESC "

        End Select

        Return sqlStr
    End Function

    Protected Sub DeleteStaging()
        Try
            Dim sqlStr As String = "EXEC SP_Reset_Staging_Table 'DMC_Subscription_Staging' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("DeleteStaging - Error: " & ex.Message)
        End Try
    End Sub





    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/DMC_Account.aspx")
    End Sub




End Class
