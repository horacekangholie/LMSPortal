
Partial Class Views_Subscription_Details
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)
        LB_PageTitle.Text = Get_Value("SELECT Name FROM R_Customer_List WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ", "Name")

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" And Not Request.QueryString("Customer_ID") Is Nothing Then
                PopulateFormViewData()
            Else
                Response.Redirect("~/Login.aspx")
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

            Case "GridView2"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 15
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Store No", "Store Name", "Banner", "Zone", "Headquarter ID", "Created Date", "Account Type", "Start Date", "End Date", "Public IP", "FTP Host", "FTP User", "FTP Password", "Status"}
                Dim ColSize() As Integer = {80, 200, 100, 80, 80, 120, 100, 120, 100, 100, 100, 100, 100, 120, 50}
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
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

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

    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If Session("User_Roles") <> "Sales" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).CssClass = "ftpFont"
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).CssClass = "ftpFont"
            Else
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).Text = "(hidden)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).Text = "(hidden)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).Style.Add("font-style", "italic")
            End If

            '' Highlight DMC store that is not active
            If e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text <> "Active" Then
                e.Row.Style.Add("color", "#a6a6a6")
            Else
                '' Loop through cell and set copiable cell value when status is not closed
                For i = 0 To e.Row.Cells.Count - 1
                    If i >= 9 And i <= 12 Then
                        e.Row.Cells(i).Attributes("onmousedown") = "this.style.backgroundColor='#eeeeee';"
                        e.Row.Cells(i).Attributes("onmouseup") = "this.style.backgroundColor='';"
                        e.Row.Cells(i).Attributes("onclick") = String.Format("copyToClipboard('{0}');", e.Row.Cells(i).Text)     '' Copy public ip, ftp host, ftp user, ftp password to clipboard
                        e.Row.Cells(i).Attributes("style") = "cursor:copy"
                    End If
                Next
            End If

        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If Session("User_Roles") <> "Sales" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).CssClass = "ftpFont"
            Else
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).Text = "(hidden)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).Style.Add("font-style", "italic")
            End If

            If e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text <> "Active" Then
                e.Row.Style.Add("color", "#a6a6a6")
            Else
                '' Loop through cell and set copiable cell value when status is not closed
                For i = 0 To e.Row.Cells.Count - 1
                    If i >= 1 And i <= 3 Then
                        e.Row.Cells(i).Attributes("onmousedown") = "this.style.backgroundColor='#eeeeee';"
                        e.Row.Cells(i).Attributes("onmouseup") = "this.style.backgroundColor='';"
                        e.Row.Cells(i).Attributes("onclick") = String.Format("copyToClipboard('{0}');", e.Row.Cells(i).Text)     '' Copy username, password and email to clipboard
                        e.Row.Cells(i).Attributes("style") = "cursor:copy"
                    End If
                Next
            End If

        End If
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim Subscription_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim Subscription As GridView = TryCast(e.Row.FindControl("gvContractStore"), GridView)
            Dim query As String = " SELECT * FROM R_DMC_Subscription_Detail WHERE Subscription_ID ='" & Subscription_ID & "' ORDER BY Store_No"
            Try
                Subscription.DataSource = GetDataTable(query)
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
                e.Row.Cells(2).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound

    End Sub

    Protected Sub GridView_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) _
        Handles GridView1.PageIndexChanging, GridView2.PageIndexChanging, GridView3.PageIndexChanging, GridView4.PageIndexChanging, GridView5.PageIndexChanging

        Dim CurrActiveGV As GridView = CType(sender, GridView)
        CurrActiveGV.PageIndex = e.NewPageIndex

        PopulateFormViewData()
        PopulateGridViewData(DDL_Store_Contract_List.SelectedValue)
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

            Case "DDL_Store_Headquarter", "DDL_User_Headquarter", "DDL_Subscription_Headquarter"
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

            Case "DDL_Subscription_Duration"
                sqlStr = "SELECT CAST(Value_2 AS int) AS Duration, Value_1 AS Duration_Option FROM DB_Lookup WHERE Lookup_Name = 'Payment Period' ORDER BY Duration "

            Case "DDL_Subscription_Currency"
                sqlStr = "SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "

            Case "DDL_Subscription_Payment_Method"
                sqlStr = "SELECT Value_1 AS Payment_Method FROM DB_Lookup WHERE Lookup_Name = 'Payment Method' "

            Case "DDL_Subscription_Payment_Mode"
                sqlStr = "SELECT Value_1 AS Payment_Mode FROM DB_Lookup WHERE Lookup_Name = 'Payment Mode' "

            Case "DDL_Subscription_Subscriber_Group"
                sqlStr = "SELECT Value_2 AS Code, Value_1 AS Subscriber_Group FROM DB_Lookup WHERE Lookup_Name = 'Subscriber Group' "

            Case "GridView_Store_List"
                sqlStr = " SELECT Headquarter_ID " &
                         "      , SUBSTRING(Store_ID, 8, 4) AS Store_No " &
                         "      , Duration, Currency, Fee, Payment_Method, Payment_Mode " &
                         "      , CASE WHEN Subscriber_Group = 'H' THEN 'Hospitality' " &
                         "             WHEN Subscriber_Group = 'R' THEN 'Retail' " &
                         "             END AS Subscriber_Group " &
                         " FROM DMC_Subscription_Staging " &
                         " ORDER BY ID DESC "

        End Select

        Return sqlStr
    End Function



    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        '' Return to original page of listing
        Dim Group_No As String = Request.QueryString("Group")
        Dim sqlStr As String = " SELECT Level_2, Path " &
                               " FROM DB_Access_Map " &
                               " WHERE Module_Name = 'DMC Accounts' AND (level_2 BETWEEN 1 And 5) AND Level_2 = '" & Group_No & "'"
        Dim Group_Path As String = Get_Value(sqlStr, "Path")
        Group_Path = IIf(Group_Path = "", "~/Form/Store_User.aspx", Group_Path)
        Response.Redirect(Group_Path)
    End Sub



End Class
