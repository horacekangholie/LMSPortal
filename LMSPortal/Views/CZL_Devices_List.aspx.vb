Imports System.Data

Partial Class Views_CZL_Devices_List
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Devices by Country"
    Dim ExcelColData As String = "Status, Expiry_Date, Licence_Term, Licence_Key, Device_Serial, Device_ID, Model, FORMAT(Created_Date, 'dd MMM yy'), AI_Software_Version, R_Version, Scale_SN, Location, MAC_Addr, Production_Licence_No"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            Dim sqlStr As String = "SELECT DISTINCT [Country] FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country != '' ORDER BY [Country] "
            BindDropDownList(DDL_Country, sqlStr, "Country", "Country", "Please select")

            '' Default distributor and czl account (client id) dropdownlist are disabled
            DDL_By_Distributor.Enabled = False
            DDL_By_Distributor.Items.Insert(0, New ListItem("Please select", "0"))
            DDL_CZL_Client_ID.Enabled = False
            DDL_CZL_Client_ID.Items.Insert(0, New ListItem("Please select", "0"))
        End If

        '' Hide / display control when dropdownlist has value selected
        GridView1.Visible = IIf(DDL_Country.SelectedValue <> "0", True, False)
        GridView1.Columns(GridView1.Columns.Count - 1).Visible = IIf(DDL_By_Distributor.SelectedValue <> "0", True, False)
        FooterTotalCount.Visible = IIf(DDL_Country.SelectedValue <> "0", True, False)
        TB_Search.Visible = IIf(DDL_Country.SelectedValue <> "0", True, False)
        BT_Search.Visible = IIf(DDL_Country.SelectedValue <> "0", True, False)
    End Sub

    Protected Sub BindGridview(ByVal gv As GridView, ByVal query As String, Optional currentSortedExpressionDirection As String = Nothing)
        Try
            BuildGridView(gv, "GridView1", "Unique_ID")

            Dim dt As DataTable = GetDataTable(query)
            Dim dataView As New DataView(dt)
            dataView.Sort = currentSortedExpressionDirection

            gv.DataSource = dataView
            gv.DataBind()
        Catch ex As Exception
            Response.Write("BindGridview - Error:  " & ex.Message)
        End Try
        FooterTotalCount.Text = "Record(s) found: " & gv.Rows.Count.ToString()   '' display no of record
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.AllowSorting = True
        GridViewObj.ShowHeader = True
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
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
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 15
                GridViewObj.ShowFooter = False
        End Select

    End Sub



    '' Dropdownlist controls
    Protected Sub DDL_Country_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Country.SelectedIndexChanged
        Dim Selected_Country As String = DDL_Country.SelectedValue
        If Selected_Country <> "0" Then
            DDL_By_Distributor.Enabled = True
            DDL_By_Distributor.Items.Clear()
            DDL_By_Distributor.Items.Insert(0, New ListItem("Please select", "0"))

            '' Bind distributor dropdownlist
            Dim sqlStr As String = String.Format("SELECT DISTINCT [Distributor_Code], [Distributor] FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Distributor != '' AND Country = '{0}' ORDER BY [Distributor] ", Selected_Country)
            BindDropDownList(DDL_By_Distributor, sqlStr, "Distributor", "Distributor_Code", "Please select")

            '' Bind Gridview by country
            Dim baseQuery = BuildBaseQuery()
            'Session("SearchQuery") = baseQuery
            BindGridview(GridView1, baseQuery & " ORDER BY [Status]")

            '' Disable transfer link button when country dropdownlist selection changed
            GridView1.Columns(GridView1.Columns.Count - 1).Visible = IIf(DDL_By_Distributor.SelectedIndex <> 0, True, False)

            '' Pass query to download excel button
            ReportSQL.Text = Replace(baseQuery & " ORDER BY [Status]", "*", ExcelColData)
            BT_Download_Excel.Visible = True
        Else
            DDL_By_Distributor.Enabled = False
            DDL_By_Distributor.SelectedIndex = 0
            BT_Download_Excel.Visible = False
        End If

        '' CZL account dropdownlist remained disabled
        DDL_CZL_Client_ID.Enabled = False
        DDL_CZL_Client_ID.SelectedIndex = 0
        DDL_CZL_Client_ID.Items.Insert(0, New ListItem("Please select", "0"))

        TB_Search.Text = String.Empty
    End Sub

    Protected Sub DDL_By_Distributor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_By_Distributor.SelectedIndexChanged
        Dim Selected_Country As String = DDL_Country.SelectedValue
        Dim Selected_Distributor As String = DDL_By_Distributor.SelectedValue
        If Selected_Distributor <> "0" Then
            DDL_CZL_Client_ID.Enabled = True
            DDL_CZL_Client_ID.Items.Clear()
            DDL_CZL_Client_ID.Items.Insert(0, New ListItem("Please select", "0"))

            '' Bind czl account (client id) dropdownlist
            Dim sqlStr As String = String.Format("SELECT Account_ID, Account_Name FROM (SELECT DISTINCT Account_ID, Account_ID + ' - ' + Account_Name AS Account_Name FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' AND Distributor_Code = '{1}') TBL ORDER BY CAST(Account_ID AS int) ", Selected_Country, Selected_Distributor)
            BindDropDownList(DDL_CZL_Client_ID, sqlStr, "Account_Name", "Account_ID", "Please select")

            '' Bind Gridview by country and distributor
            Dim baseQuery = BuildBaseQuery()
            'Session("SearchQuery") = baseQuery
            BindGridview(GridView1, baseQuery & " ORDER BY [Status]")

            '' Pass query to download excel button
            ReportSQL.Text = Replace(baseQuery & " ORDER BY [Status]", "*", ExcelColData)
            BT_Download_Excel.Visible = True
        Else
            DDL_CZL_Client_ID.Enabled = False
            DDL_CZL_Client_ID.SelectedIndex = 0

            Dim query As String = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' ", Selected_Country)
            query += "ORDER BY [Status] "
            BindGridview(GridView1, query)
        End If
    End Sub

    Protected Sub DDL_CZL_Client_ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.SelectedIndexChanged
        '' Bind Gridview by country, distributor and client id
        Dim baseQuery = BuildBaseQuery()
        Session("SearchQuery") = baseQuery
        BindGridview(GridView1, baseQuery & " ORDER BY [Status]")

        '' Pass query to download excel button
        ReportSQL.Text = Replace(baseQuery & " ORDER BY [Status]", "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub



    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex

        Dim baseQuery = BuildBaseQuery()
        Dim sortExpr As String = If(ViewState("SortExpression") IsNot Nothing,
                                ViewState("SortExpression").ToString(),
                                GridView1.Columns(0).SortExpression)
        Dim sortDir As String = If(ViewState("SortDirection")?.ToString() = "ASC", "ASC", "DESC")
        Dim fullSort As String = sortExpr & " " & sortDir
        BindGridview(GridView1, baseQuery & " ORDER BY " & fullSort, fullSort)

        ReportSQL.Text = Replace(baseQuery & " ORDER BY " & fullSort, "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub

    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim gv As GridView = DirectCast(sender, GridView)

        If e.Row.RowType = DataControlRowType.Header Then
            ApplySortArrow(gv, e.Row, rightAlignFromIndex:=14)
            Dim colNames = {
                "Status", "Expiry Date", "Term", "Binding Key", "Device Serial",
                "Device ID", "Model", "Created On", "AI Soft. Ver.", "R Vers",
                "Scale SN", "Location", "MAC Address", "Prod. Licence No", ""
            }
            Dim colSizes = {5, 4, 4, 4, 10, 15, 4, 4, 4, 4, 5, 25, 10, 10, 15}
            ApplySimpleHeaderFormatting(e.Row, colNames, colSizes, rightAlignFromIndex:=14)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim TransferLinkButton As LinkButton = CType((e.Row.FindControl("TransferLinkButton")), LinkButton)       '' Edit Link button object

            '' Display status column as badge
            Dim LicenseStatus As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text
            Select Case LicenseStatus
                Case "Activated"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-success'>" & LicenseStatus & "</span>"
                Case "Renew"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-info'>" & LicenseStatus & "</span>"
                Case "Expired"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-danger'>" & LicenseStatus & "</span>"
                Case "New"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-primary'>" & LicenseStatus & "</span>"
                Case "Blocked"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-dark'>" & LicenseStatus & "</span>"
                Case Else
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-light' style='color:#b2babb'>Unknown</span>"
            End Select

            '' If a device has comments then disable the delete button
            If drv("Status") = "Expired" Then
                TransferLinkButton.Text = "<i class='bi bi-lock'></i>"
                TransferLinkButton.CssClass = "btn btn-xs btn-light disabled"
                TransferLinkButton.ToolTip = "Item Locked"
                TransferLinkButton.Enabled = False
            Else
                TransferLinkButton.Enabled = True
            End If

            '' Change the color for sorted column
            e.Row.Cells(currentSortedColumnIndex).BackColor = Drawing.ColorTranslator.FromHtml("#ffffe6")
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView1_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView1.Sorting
        ' Toggle ASC/DESC on the new column
        Dim defaultExpr = GridView1.Columns(0).SortExpression
        Dim fullExpr = BuildSortExpression(e.SortExpression, defaultExpr)

        Dim baseQuery = BuildBaseQuery()
        Dim sortedQuery = baseQuery & " ORDER BY " & fullExpr

        ' Bind the grid (and pass the sort expression so the DataView highlights correctly)
        BindGridview(GridView1, sortedQuery, fullExpr)

        ' Update your Excel‐export query
        ReportSQL.Text = Replace(sortedQuery, "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub


    '' Common function
    Private Function BuildBaseQuery() As String
        Dim sb As New Text.StringBuilder()
        sb.Append("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device ")
        sb.Append("WHERE Country = '").Append(DDL_Country.SelectedValue).Append("' ")

        If DDL_By_Distributor.SelectedValue <> "0" Then
            sb.Append("AND Distributor_Code = '").Append(DDL_By_Distributor.SelectedValue).Append("' ")
        End If

        If DDL_CZL_Client_ID.SelectedValue <> "0" Then
            sb.Append("AND Account_ID = '").Append(DDL_CZL_Client_ID.SelectedValue).Append("' ")
        End If

        Dim kw As String = TB_Search.Text.Trim()
        If kw <> "" Then
            kw = EscapeChar(kw)
            ' Add column name to filter
            sb.Append("AND (Device_Serial LIKE '%").Append(kw) _
              .Append("%' OR Device_ID LIKE '%").Append(kw) _
              .Append("%' OR Licence_Key LIKE '%").Append(kw) _
              .Append("%' OR Scale_SN LIKE '%").Append(kw) _
              .Append("%' OR Location LIKE '%").Append(kw).Append("%') ")
        End If

        Return sb.ToString()
    End Function



    '' Modal
    Protected Sub DDL_Transfer_CZL_Client_ID_Load(sender As Object, e As EventArgs) Handles DDL_Transfer_CZL_Client_ID.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group " &
                             "FROM CZL_Account " &
                             "ORDER BY CAST(Client_ID AS int) "

                DDL_Transfer_CZL_Client_ID.DataSource = GetDataTable(sqlStr)
                DDL_Transfer_CZL_Client_ID.DataTextField = "Client_ID_User_Group"
                DDL_Transfer_CZL_Client_ID.DataValueField = "Client_ID"
                DDL_Transfer_CZL_Client_ID.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub TransferLinkButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderTransferLDevice.Text = "Move Device to Other Account"
        btnTransferDevice.Text = "Move"
        btnCancelTransferDevice.Text = "Cancel"

        '' Get row command argument and populate to hidden field in modal
        Dim TransferLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim TransferLinkButtonCommandArgument As Array = Split(TransferLinkButton.CommandArgument, "|")
        TB_Hidden_Selected_Row_Index.Text = TransferLinkButtonCommandArgument(0)
        TB_Hidden_Selected_Unique_ID.Text = TransferLinkButtonCommandArgument(1)
        TB_Hidden_Selected_Distributor_Code.Text = TransferLinkButtonCommandArgument(2)
        TB_Hidden_Selected_Existing_Client_ID.Text = TransferLinkButtonCommandArgument(3)
        TB_Hidden_Selected_Existing_Client_Name.Text = TransferLinkButtonCommandArgument(4)

        '' Repopulate the dropdownlist option
        Dim DDL_Transfer_CZL_Client_ID As DropDownList = pnlTransferLDevice.FindControl("DDL_Transfer_CZL_Client_ID")
        Try
            Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group " &
                         "FROM CZL_Account " &
                         "WHERE By_Distributor ='" & TB_Hidden_Selected_Distributor_Code.Text & "' " &
                         "  AND Client_ID != " & TB_Hidden_Selected_Existing_Client_ID.Text &
                         " ORDER BY CAST(Client_ID AS int) "

            '' Clear the dropdownlist item and append an bound item
            DDL_Transfer_CZL_Client_ID.Items.Clear()
            DDL_Transfer_CZL_Client_ID.Items.Add(New ListItem("Please select", "-1"))

            DDL_Transfer_CZL_Client_ID.DataSource = GetDataTable(sqlStr)
            DDL_Transfer_CZL_Client_ID.DataTextField = "Client_ID_User_Group"
            DDL_Transfer_CZL_Client_ID.DataValueField = "Client_ID"
            DDL_Transfer_CZL_Client_ID.DataBind()

            DDL_Transfer_CZL_Client_ID.SelectedIndex = -1   '' Set dropdownlist to always select appended items "Please select"

            '' Displya guided message to user based on the available czl account
            DDL_Transfer_CZL_Client_ID.Enabled = True
            TransferGuidedMessage.Text = "Please select CZL account from following to move the device to."
            TransferGuidedMessage.CssClass = "text-muted"

        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        popupTransferDevice.Show()
    End Sub

    Protected Sub Save_Transfer_Device_Click(sender As Object, e As EventArgs) Handles btnTransferDevice.Click
        Dim Device_Unique_ID As TextBox = TB_Hidden_Selected_Unique_ID
        Dim Existing_Client_ID As TextBox = TB_Hidden_Selected_Existing_Client_ID
        Dim Existing_Client_Name As TextBox = TB_Hidden_Selected_Existing_Client_Name
        Dim DDL_Transfer_CZL_Client_ID As DropDownList = pnlTransferLDevice.FindControl("DDL_Transfer_CZL_Client_ID")
        Dim By_Who As String = Session("User_Name")

        Try
            Dim sqlStr As String = "UPDATE CZL_Licenced_Devices " &
                                   "SET Client_ID = " & DDL_Transfer_CZL_Client_ID.SelectedValue &
                                   "  , CZL_Account_Unique_ID = (SELECT TOP 1 CZL_Account_Unique_ID FROM CZL_Account WHERE Client_ID = " & DDL_Transfer_CZL_Client_ID.SelectedValue & ") " &
                                   "WHERE Unique_ID = '" & Device_Unique_ID.Text & "' "
            RunSQL(sqlStr)

            Dim sqlStr1 As String = "EXEC SP_CRUD_CZL_Log N'', N'Moved from account #" & Existing_Client_ID.Text & " - " & Existing_Client_Name.Text & " to #" & DDL_Transfer_CZL_Client_ID.SelectedItem.Text & "', N'" & Device_Unique_ID.Text & "', N'SYS', N'" & By_Who & "' "
            RunSQL(sqlStr1)

            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Device is moved to account #" & DDL_Transfer_CZL_Client_ID.SelectedItem.Text & "');", True)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try


        '' Repopulate gridview
        Dim keyword As String = EscapeChar(TB_Search.Text)
        Dim SearchQuery As String = Session("SearchQuery")
        SearchQuery += "AND (Device_Serial LIKE '%" & keyword & "%' OR Device_ID LIKE '%" & keyword & "%' OR Scale_SN LIKE '%" & keyword & "%' OR Location LIKE '%" & keyword & "%') "
        'SearchQuery += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
        SearchQuery += "ORDER BY [Status] "
        BindGridview(GridView1, SearchQuery)

        '' Pass query to download excel button and replace * with specified column name for excel
        ReportSQL.Text = Replace(SearchQuery, "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub

    Protected Sub Cancel_Transfer_Device_Click(sender As Object, e As EventArgs) Handles btnCancelTransferDevice.Click
        '' When cancel button is click, highlight the record that was being selected
        Dim Selected_Row_Index As String = TB_Hidden_Selected_Row_Index.Text
        Selected_Row_Index = IIf(Selected_Row_Index <> "", Selected_Row_Index, 0)

        For i = 0 To GridView1.Rows.Count - 1
            GridView1.Rows(i).BackColor = IIf(i = Selected_Row_Index, Drawing.ColorTranslator.FromHtml("#eeeeee"), Drawing.Color.Transparent)
        Next
    End Sub



    '' Search record
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        Dim baseQuery = BuildBaseQuery()
        'Session("SearchQuery") = baseQuery
        BindGridview(GridView1, baseQuery & " ORDER BY [Status]")

        '' Pass query to download excel button and replace * with specified column name for excel
        ReportSQL.Text = Replace(baseQuery & " ORDER BY [Status]", "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub



    '' Download excel report button
    Protected Sub BT_Download_Excel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Download_Excel.Click
        DownloadExcel(ReportSQL.Text, "AI Device List", "Excel", "General")
    End Sub


End Class
