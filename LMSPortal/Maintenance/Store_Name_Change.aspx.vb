
Partial Class Maintenance_Store_Name_Change
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Store Name Change"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
        End If

    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT ID, A.Store_ID, Old_Store_Name, New_Store_Name, Old_Banner_Name, New_Banner_Name " &
                                   "     , CONVERT(nvarchar, Effective_Date, 120) As Effective_Date " &
                                   "     , ROW_NUMBER() OVER(PARTITION BY A.Store_ID ORDER BY A.Store_ID DESC ) As SeqOfChange, B.MaxOfChange " &
                                   "FROM DMC_Store_Name_Change_History A " &
                                   "INNER JOIN (SELECT Store_ID, COUNT(Store_ID) As MaxOfChange FROM DMC_Store_Name_Change_History GROUP BY Store_ID) B On B.Store_ID = A.Store_ID "
            sqlStr += "WHERE A.Store_ID LIKE '%" & keyword & "%' OR Old_Store_Name Like '%" & keyword & "%' OR New_Store_Name LIKE '%" & keyword & "%' OR Old_Banner_Name LIKE '%" & keyword & "%' OR New_Banner_Name LIKE '%" & keyword & "%' "
            sqlStr += "ORDER BY Effective_Date DESC, SeqOfChange DESC "

            ''Response.Write(sqlStr)
            BuildGridView()
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

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
        GridView1.DataKeyNames = New String() {"ID"}
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
    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Store_ID", "Old_Store_Name", "Old_Banner_Name", "New_Store_Name", "New_Banner_Name", "Effective_Date", ""}
        Dim ColSize() As Integer = {100, 300, 300, 300, 300, 200, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = ColSize(i)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
                If i = CtrlCellIndex Then
                    e.Row.Cells(CtrlCellIndex).Width = 0
                    e.Row.Cells(CtrlCellIndex).Wrap = False
                    e.Row.Cells(CtrlCellIndex).HorizontalAlign = HorizontalAlign.Center
                End If
            Next

            '' Set confirmation for commandfield delete button before deleting
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            Dim EditCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 3
            Dim DeleteCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1
            Dim EditCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(EditCommandFieldIndex), LinkButton)
            Dim DeleteCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteCommandFieldIndex), LinkButton)
            If DeleteCommandField.CommandName = "Delete" Then
                If CInt(drv("MaxOfChange")) > CInt(drv("SeqOfChange")) Then
                    EditCommandField.Text = "<span class='btn btn-xs btn-light'><i class='bi bi-lock'></i></span>"
                    EditCommandField.Enabled = False
                    DeleteCommandField.Text = "<span class='btn btn-xs btn-light'><i class='bi bi-lock'></i></span>"
                    DeleteCommandField.Enabled = False
                Else
                    DeleteCommandField.Text = "<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>"
                    DeleteCommandField.Enabled = True
                    DeleteCommandField.Attributes.Add("onclick", "return confirm('Delete record may cause data inconsistency,\nare you sure to proceed?');")
                End If
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


    '' gridview row controls
    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNewChange.Click
        Dim Store_ID As String = DDL_Store.SelectedValue
        Dim Old_Store_Name As String = TB_Store_Name.Text
        Dim New_Store_Name As String = TB_New_Store_Name.Text
        Dim Old_Banner_Name As String = TB_Banner.Text
        Dim New_Banner_Name As String = TB_New_Banner_Name.Text
        Dim Effective_Date As String = TB_Effective_Date.Text

        Dim sqlStr As String = "INSERT INTO DMC_Store_Name_Change_History(Store_ID, Old_Store_Name, New_Store_Name, Old_Banner_Name, New_Banner_Name, Effective_Date) " &
                               "VALUES(N'" & Store_ID & "', N'" & Old_Store_Name & "', N'" & EscapeChar(New_Store_Name) & "', N'" & Old_Banner_Name & "', N'" & EscapeChar(New_Banner_Name) & "', N'" & Effective_Date & "') "

        Dim sqlStr1 As String = "UPDATE DMC_Store SET Name = N'" & New_Store_Name & "', Banner = N'" & New_Banner_Name & "' WHERE Store_ID = '" & Store_ID & "' "

        Try
            RunSQL(sqlStr)
            RunSQL(sqlStr1)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' clear field after added
        DDL_Headquarter.SelectedIndex = -1
        DDL_Store.SelectedIndex = -1
        TB_Store_Name.Text = String.Empty
        TB_Banner.Text = String.Empty
        TB_New_Store_Name.Text = String.Empty
        TB_New_Banner_Name.Text = String.Empty
        TB_Effective_Date.Text = String.Empty

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))

        Dim Store_ID As String = (TryCast(row.FindControl("TB_E_Store_ID"), TextBox)).Text
        Dim New_Store_Name As String = (TryCast(row.FindControl("TB_E_New_Store_Name"), TextBox)).Text
        Dim New_Banner_Name As String = (TryCast(row.FindControl("TB_E_New_Banner_Name"), TextBox)).Text
        Dim Effective_Date As String = (TryCast(row.FindControl("TB_E_Effective_Date"), TextBox)).Text

        Dim sqlStr As String = "UPDATE DMC_Store_Name_Change_History SET New_Store_Name = N'" & New_Store_Name & "', New_Banner_Name = N'" & New_Banner_Name & "', Effective_Date = N'" & Effective_Date & "' WHERE ID = " & ID
        Dim sqlStr1 As String = "UPDATE DMC_Store SET Name = N'" & New_Store_Name & "', Banner = N'" & New_Banner_Name & "' WHERE Store_ID = '" & Store_ID & "' "
        Try
            RunSQL(sqlStr)
            RunSQL(sqlStr1)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles GridView1.RowDeleting
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))

        Dim sqlStr As String = "DELETE FROM DMC_Store_Name_Change_History WHERE ID = " & ID
        Dim sqlStr1 As String = "UPDATE DMC_Store " &
                                "SET DMC_Store.Name = DMC_Store_Name_Change_History.Old_Store_Name, DMC_Store.Banner = DMC_Store_Name_Change_History.Old_Banner_Name " &
                                "FROM DMC_Store " &
                                "INNER JOIN DMC_Store_Name_Change_History ON DMC_Store_Name_Change_History.Store_ID = DMC_Store.Store_ID AND DMC_Store_Name_Change_History.ID = " & ID &
                                "WHERE DMC_Store.Store_ID = (SELECT Store_ID FROM DMC_Store_Name_Change_History WHERE ID = " & ID & ") "
        Try
            RunSQL(sqlStr1)
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub



    Protected Sub DDL_Headquarter_Load(sender As Object, e As EventArgs) Handles DDL_Headquarter.Load
        If Not IsPostBack Then
            'Dim sqlStr As String = "SELECT Headquarter_ID + ' - ' + Name AS Headquarter, Headquarter_ID FROM DMC_Headquarter ORDER BY Headquarter_ID"

            '' Populate active headquarter and headquarter with at least one active store
            Dim sqlStr As String = "SELECT Headquarter_ID + ' - ' + Name AS Headquarter, Headquarter_ID " &
                                   "FROM DMC_Headquarter H " &
                                   "WHERE Is_Active = CASE WHEN (SELECT COUNT(Store_ID) FROM DMC_Store WHERE Headquarter_ID = H.Headquarter_ID AND Is_Active = 1) > 0 THEN 1 ELSE 0 END " &
                                   "  AND H.Is_Active = 1 " &
                                   "ORDER BY H.Headquarter_ID "

            Try
                DDL_Headquarter.DataSource = GetDataTable(sqlStr)
                DDL_Headquarter.DataTextField = "Headquarter"
                DDL_Headquarter.DataValueField = "Headquarter_ID"
                DDL_Headquarter.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Headquarter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Headquarter.SelectedIndexChanged
        '' when DDL_Headquarter selected item changed, repopulate the selection in DDL_Store
        Dim Headquarter As String = DDL_Headquarter.SelectedValue
        Dim sqlStr As String = "SELECT ISNULL(Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) + ' - ' + Name AS Store, Store_ID FROM DMC_Store WHERE Headquarter_ID = '" & Headquarter & "' ORDER BY Store_ID"
        Try
            DDL_Store.DataSource = GetDataTable(sqlStr)
            DDL_Store.DataTextField = "Store"
            DDL_Store.DataValueField = "Store_ID"
            '' clear the dropdownlist and add back the first item
            DDL_Store.Items.Clear()
            DDL_Store.Items.Insert(0, New ListItem("Please select", "empty"))
            DDL_Store.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub DDL_Store_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Store.SelectedIndexChanged
        Dim Store_ID As String = DDL_Store.SelectedValue
        TB_Store_Name.Text = Get_Value("select Name from DMC_Store where Store_ID = '" & Store_ID & "'", "Name")
        TB_Banner.Text = Get_Value("select Banner from DMC_Store where Store_ID = '" & Store_ID & "'", "Banner")
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub



End Class
