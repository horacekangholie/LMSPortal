
Partial Class Adhoc_Suspended_Stores
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Suspended Stores"
    Dim sqlStr As String = "SELECT ROW_NUMBER() OVER(ORDER BY [Headquarter ID] ASC) AS No, * FROM R_suspended_stores "
    Dim reportColName As String() = {"Headquarter ID", "Headquarter Name", "Store No", "Store Name", "Account Type", "Created Date", "Expiry Date", "Suspended Date", "Status", "Requestor", "Reason of suspension"}
    Dim reportColSize As Integer() = {50, 200, 50, 200, 50, 50, 50, 50, 50, 100, 300}
    Dim orderByCol As String = "[Suspended Date] DESC, [Headquarter ID]"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            sqlStr += " WHERE [Headquarter ID] LIKE '%" & keyword & "%' OR [Headquarter Name] LIKE '%" & keyword & "%' OR [Store Name] LIKE '%" & keyword & "%' OR [Requestor] LIKE '%" & keyword & "%' "
            sqlStr += " ORDER BY " & orderByCol

            '' Formatting GridView
            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        ' Get the total record
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
        GridView1.DataKeyNames = New String() {"No"}
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

        GridView1.Columns.Clear()  '' Clear each time page index changed

        '' Create column boundfield
        For i = 0 To reportColName.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = reportColName(i)
            Bfield.HeaderText = Replace(reportColName(i), "_", " ")
            Bfield.HeaderStyle.Width = reportColSize(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next
    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub


    '' Model controls - Add suspended store record
    Protected Sub DDL_Headquarter_Load(sender As Object, e As EventArgs) Handles DDL_Headquarter.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = "SELECT H.Headquarter_ID, H.Headquarter_ID + ' ' + H.Name AS Headquarter_Name " &
                             "FROM DMC_Store S " &
                             "LEFT JOIN DMC_Headquarter H On H.Headquarter_ID = S.Headquarter_ID " &
                             "WHERE S.Is_Active = 1 And S.Account_Type In ('03') " &
                             "GROUP BY H.Headquarter_ID, H.Name " &
                             "ORDER BY H.Headquarter_ID "

                DDL_Headquarter.DataSource = GetDataTable(sqlStr)
                DDL_Headquarter.DataTextField = "Headquarter_Name"
                DDL_Headquarter.DataValueField = "Headquarter_ID"
                DDL_Headquarter.DataBind()

            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Headquarter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Headquarter.SelectedIndexChanged
        '' Enable the Store dropdownlist if a headquarter is selected
        DDL_Store.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)
        CompareValidator_Store.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)
        DDL_Suspend_From_Month.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)
        CompareValidator_Suspend_From_Month.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)
        TB_Reason.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)
        RequiredField_TB_Reason.Enabled = IIf(DDL_Headquarter.SelectedValue <> "-1", True, False)

        Dim sqlStr As String = " SELECT Store_ID, S.Name AS Store_Name " &
                               " FROM DMC_Store S " &
                               " LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID " &
                               " WHERE S.Is_Active = 1 AND S.Headquarter_ID = '" & DDL_Headquarter.Text & "' AND S.Account_Type IN ('03') " &
                               " ORDER BY ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' ' + S.Name "

        DDL_Store.DataSource = GetDataTable(sqlStr)
        DDL_Store.DataTextField = "Store_Name"
        DDL_Store.DataValueField = "Store_ID"
        DDL_Store.Items.Clear()   '' clear the dropdownlist then bind
        DDL_Store.Items.Insert(0, New ListItem("Please select", "-1"))   '' appenddatebounditem
        DDL_Store.DataBind()

        popupAddSuspendedStore.Show()
    End Sub

    Protected Sub DDL_Suspend_From_Month_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Suspend_From_Month.Load
        Dim DDL_Suspend_From_Month As DropDownList = TryCast(sender, DropDownList)
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT MonthYearList, CAST(MonthYearList AS date) AS ReportMonth " &
                                       "FROM dbo.Get_MonthYearList(DATEADD(MONTH, 0, GETDATE()), DATEADD(MONTH, 11, GETDATE())) " &
                                       "ORDER BY CAST(EOMONTH(CAST(MonthYearList AS date), 0) AS Date) "

                DDL_Suspend_From_Month.DataSource = GetDataTable(sqlStr)
                DDL_Suspend_From_Month.DataTextField = "MonthYearList"
                DDL_Suspend_From_Month.DataValueField = "ReportMonth"
                DDL_Suspend_From_Month.DataBind()

            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub


    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        ModalHeaderAddSuspendedStore.Text = "Add Suspended Store"
        btnAddSuspendedStore.Text = "Add"
        btnCancelAddSuspendedStore.Text = "Cancel"

        '' Initialize the fields
        DDL_Headquarter.SelectedIndex = 0
        DDL_Store.SelectedIndex = 0
        DDL_Store.Enabled = False
        DDL_Suspend_From_Month.SelectedIndex = 0
        DDL_Suspend_From_Month.Enabled = False
        TB_Reason.Text = String.Empty
        TB_Reason.Enabled = False

        popupAddSuspendedStore.Show()
    End Sub

    Protected Sub Add_SuspendedStore_Click(sender As Object, e As EventArgs) Handles btnAddSuspendedStore.Click
        Dim DDL_Headquarter As DropDownList = pnlAddSuspendedStore.FindControl("DDL_Headquarter")
        Dim DDL_Store As DropDownList = pnlAddSuspendedStore.FindControl("DDL_Store")

        Try
            Dim sqlStr As String = "INSERT INTO Suspended_Store(Store_ID, Store_Name, Suspended_Date, Reason) " &
                                   "SELECT '" & DDL_Store.SelectedValue & "', '" & EscapeChar(DDL_Store.SelectedItem.Text) & "', '" & DDL_Suspend_From_Month.SelectedValue & "', '" & TB_Reason.Text & "'; " &
                                   "UPDATE DMC_Store SET Is_Active = 0 WHERE Store_ID = '" & DDL_Store.SelectedValue & "'; " &
                                   "UPDATE DMC_User " &
                                   "   SET Is_Active = (CASE WHEN (SELECT COUNT(*) FROM DMC_Store WHERE Headquarter_ID = '" & DDL_Headquarter.SelectedValue & "' AND Is_Active = 1) = 0 THEN 0 ELSE Is_Active END) " &
                                   " , Inactive_Date = GETDATE() " &
                                   "WHERE Headquarter_ID = '" & DDL_Headquarter.SelectedValue & "' AND Is_Active = 1; "
            'RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' Re-bind the Headquarter dropdownlist
        Dim sqlStr1 = "SELECT H.Headquarter_ID, H.Headquarter_ID + ' ' + H.Name AS Headquarter_Name " &
                      "FROM DMC_Store S " &
                      "LEFT JOIN DMC_Headquarter H On H.Headquarter_ID = S.Headquarter_ID " &
                      "WHERE S.Is_Active = 1 And S.Account_Type In ('03') " &
                      "GROUP BY H.Headquarter_ID, H.Name " &
                      "ORDER BY H.Headquarter_ID "

        DDL_Headquarter.DataSource = GetDataTable(sqlStr1)
        DDL_Headquarter.DataTextField = "Headquarter_Name"
        DDL_Headquarter.DataValueField = "Headquarter_ID"
        DDL_Headquarter.DataBind()

        BuildPageContent(TB_Search.Text)
    End Sub


    '' Search button
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub


End Class
