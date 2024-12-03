Imports System.Data

Partial Class Views_CZL_Devices_List
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Devices by Country"
    Dim ExcelColData As String = "Status, Expiry_Date, Licence_Key, Device_Serial, Device_ID, Model, FORMAT(Created_Date, 'dd MMM yy'), AI_Software_Version, R_Version, Scale_SN, Location, MAC_Addr, Production_Licence_No"

    Dim currentSortedColumnIndex As Integer

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
            Dim query As String = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' ", Selected_Country)
            Session("SearchQuery") = query               '' Pass current query to session SearchQuery without the ORDER BY
            'query += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
            query += "ORDER BY [Status] "
            BindGridview(GridView1, query)

            '' Disable transfer link button when country dropdownlist selection changed
            GridView1.Columns(GridView1.Columns.Count - 1).Visible = IIf(DDL_By_Distributor.SelectedIndex <> 0, True, False)

            '' Pass query to download excel button
            ReportSQL.Text = Replace(query, "*", ExcelColData)
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
            Dim query As String = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' AND Distributor_Code = '{1}' ", Selected_Country, Selected_Distributor)
            Session("SearchQuery") = query            '' Pass current query to session SearchQuery without the ORDER BY
            'query += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
            query += "ORDER BY [Status] "
            BindGridview(GridView1, query)

            '' Pass query to download excel button
            ReportSQL.Text = Replace(query, "*", ExcelColData)
            BT_Download_Excel.Visible = True
        Else
            DDL_CZL_Client_ID.Enabled = False
            DDL_CZL_Client_ID.SelectedIndex = 0

            Dim query As String = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' ", Selected_Country)
            'query += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
            query += "ORDER BY [Status] "
            BindGridview(GridView1, query)
        End If

        TB_Search.Text = String.Empty
    End Sub

    Protected Sub DDL_CZL_Client_ID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.SelectedIndexChanged
        Dim Selected_Country As String = DDL_Country.SelectedValue
        Dim Selected_Distributor As String = DDL_By_Distributor.SelectedValue
        Dim Selected_Client_ID As String = DDL_CZL_Client_ID.SelectedValue
        Dim query As String

        '' Bind Gridview by country, distributor and client id
        If Selected_Client_ID <> "0" Then
            query = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' AND Distributor_Code = '{1}' AND Account_ID = '{2}' ", Selected_Country, Selected_Distributor, Selected_Client_ID)
        Else
            query = String.Format("SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '{0}' AND Distributor_Code = '{1}' ", Selected_Country, Selected_Distributor)
        End If
        Session("SearchQuery") = query        '' Pass current query to session SearchQuery
        'query += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
        query += "ORDER BY [Status] "
        BindGridview(GridView1, query)

        '' Pass query to download excel button
        ReportSQL.Text = Replace(query, "*", ExcelColData)
        BT_Download_Excel.Visible = True

        TB_Search.Text = String.Empty
    End Sub



    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        Dim Selected_Country As String = DDL_Country.SelectedValue
        Dim Selected_Distributor As String = DDL_By_Distributor.SelectedValue
        Dim Selected_Client_ID As String = DDL_CZL_Client_ID.SelectedValue

        Dim query As String = "SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '" & Selected_Country & "' "
        If Selected_Distributor <> "0" Then
            query += "AND Distributor_Code = '" & Selected_Distributor & "' "
        End If
        If Selected_Client_ID <> "0" Then
            query += "AND Account_ID = '" & Selected_Client_ID & "' "
        End If

        Session("SearchQuery") = query         '' Pass current query to session SearchQuery
        'query += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
        query += "ORDER BY [Status] "
        BindGridview(GridView1, query)
    End Sub

    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

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
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).Style.Add("background-color", "#ffffe6")
            Next
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
        Dim sortExpression As String = e.SortExpression
        If ViewState("SortDirection") IsNot Nothing AndAlso ViewState("SortExpression") IsNot Nothing Then
            Dim previousSortExpression As String = ViewState("SortExpression").ToString()
            Dim previousSortDirection As String = ViewState("SortDirection").ToString()

            ViewState("SortDirection") = If(previousSortExpression = sortExpression, If(previousSortDirection = "ASC", "DESC", "ASC"), "DESC")
        Else
            ViewState("SortDirection") = "DESC"
            ViewState("SortExpression") = GridView1.Columns(0).SortExpression
        End If

        ViewState("SortExpression") = sortExpression
        Dim sortDirection As String = If(ViewState("SortDirection").ToString() = "ASC", " ASC", " DESC")
        sortExpression += sortDirection

        '' populate gridview
        Dim keyword As String = EscapeChar(TB_Search.Text)
        Dim Selected_Country As String = DDL_Country.SelectedValue
        Dim Selected_Distributor As String = DDL_By_Distributor.SelectedValue
        Dim Selected_Client_ID As String = DDL_CZL_Client_ID.SelectedValue

        Dim query As String = "SELECT * FROM R_CZL_Licenced_Device_With_Unassigned_Device WHERE Country = '" & Selected_Country & "' "
        If Selected_Distributor <> "0" Then
            query += "AND Distributor_Code = '" & Selected_Distributor & "' "
        End If
        If Selected_Client_ID <> "0" Then
            query += "AND Account_ID = '" & Selected_Client_ID & "' "
        End If

        '' Form the query and bind gridview
        Session("SearchQuery") = query         '' Pass current query to session SearchQuery
        query += "AND (Device_Serial LIKE '%" & keyword & "%' OR Device_ID LIKE '%" & keyword & "%' OR Scale_SN LIKE '%" & keyword & "%' OR Location LIKE '%" & keyword & "%') "
        BindGridview(GridView1, query, sortExpression)

        '' Form the query for excel download
        query += "ORDER BY " & sortExpression
        ReportSQL.Text = Replace(query, "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub

    Private Sub CustomizeSortedHeaderRow(ByVal gridView As GridView, ByVal headerRow As GridViewRow)
        Dim sortExpression As String = ViewState("SortExpression")?.ToString()
        Dim sortDirection As String = ViewState("SortDirection")?.ToString()

        ' If sortExpression is empty, set it to the first column's SortExpression
        If String.IsNullOrEmpty(sortExpression) Then
            sortExpression = gridView.Columns(0).SortExpression
            sortDirection = "ASC"
            currentSortedColumnIndex = 0
        End If

        ' Loop through the headerrow control field to find which is the current selected column
        For Each field As DataControlField In gridView.Columns
            If field.SortExpression = sortExpression Then
                Dim cellIndex As Integer = gridView.Columns.IndexOf(field)
                Dim sortArrow As New Label()
                sortArrow.CssClass = "sort-arrow " & If(sortDirection = "ASC", "asc", "desc")

                ' Add the sorting arrow inside a <span> element
                Dim span As New HtmlGenericControl("span")
                span.Controls.Add(sortArrow)

                ' Append the <span> to the header cell
                headerRow.Cells(cellIndex).Controls.Add(span)

                ' Get current sorted column index
                currentSortedColumnIndex = cellIndex
            End If
        Next
    End Sub



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
        Dim keyword As String = EscapeChar(TB_Search.Text)
        Dim SearchQuery As String = Session("SearchQuery")
        SearchQuery += "AND (Licence_Key LIKE '%" & keyword & "%' OR Device_Serial LIKE '%" & keyword & "%' OR Device_ID LIKE '%" & keyword & "%' OR Scale_SN LIKE '%" & keyword & "%' OR Location LIKE '%" & keyword & "%') "
        'SearchQuery += "ORDER BY CAST(Activated_Date AS date), Expiry_Date "
        SearchQuery += "ORDER BY [Status] "
        BindGridview(GridView1, SearchQuery)

        '' Pass query to download excel button and replace * with specified column name for excel
        ReportSQL.Text = Replace(SearchQuery, "*", ExcelColData)
        BT_Download_Excel.Visible = True
    End Sub



    '' Download excel report button
    Protected Sub BT_Download_Excel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Download_Excel.Click
        DownloadExcel(ReportSQL.Text, "AI Device List", "Excel", "General")
    End Sub


End Class
