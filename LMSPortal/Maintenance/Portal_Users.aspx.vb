
Partial Class Maintenance_Portal_Users
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Portal Users"

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
            Dim sqlStr As String = "SELECT * FROM DB_Users "
            sqlStr += " WHERE Display_Name LIKE '%" & keyword & "%' "
            sqlStr += " ORDER BY ID DESC "

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



    '' Insert new record to DB_Users table
    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Username As String = TB_Username.Text
        Dim Display_Name As String = TB_Display_Name.Text
        Dim Email As String = TB_Email.Text
        Dim Role As String = DDL_Role.SelectedValue

        Dim sqlStr As String = "INSERT INTO DB_Users(Username, Password, Email, Roles, Display_Name) " &
                               "VALUES(N'" & Username & "', N'" & Encrypt("password123") & "', N'" & Email & "', N'" & Role & "', N'" & Display_Name & "')"

        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' clear field after added
        TB_Username.Text = String.Empty
        TB_Display_Name.Text = String.Empty
        TB_Email.Text = String.Empty
        DDL_Role.SelectedIndex = 0

        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Gridview controls

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Display Name", "Username", "Password", "Email", "Role", "Group", ""}
        Dim ColSize() As Integer = {300, 200, 200, 300, 200, 200, 0}

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

            '' Set confirmation before deleting
            Dim DeleteCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1
            Dim DeleteCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteCommandFieldIndex), LinkButton)
            If DeleteCommandField.CommandName = "Delete" Then
                DeleteCommandField.Attributes.Add("onclick", "return confirm('Are you sure to proceed?');")
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


    '' Gridview row commands events
    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim Display_Name As String = (TryCast(row.FindControl("TB_E_Display_Name"), TextBox)).Text
        Dim Username As String = (TryCast(row.FindControl("TB_E_Username"), TextBox)).Text
        Dim Password As String = Encrypt((TryCast(row.FindControl("TB_E_Password"), TextBox)).Text)
        Dim Email As String = (TryCast(row.FindControl("TB_E_Email"), TextBox)).Text
        Dim Role As String = (TryCast(row.FindControl("TB_E_Role"), TextBox)).Text
        Dim Group As String = (TryCast(row.FindControl("TB_E_Group"), TextBox)).Text

        Dim sqlStr As String = "UPDATE DB_Users SET Username = N'" & Username & "', Password = N'" & Password & "', Email = N'" & Email & "', Roles = N'" & Role & "', Display_Name = N'" & Display_Name & "', User_Group = N'" & Group & "'  WHERE ID = " & ID
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles GridView1.RowDeleting
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim sqlStr As String = " DELETE FROM DB_Users WHERE ID = " & ID
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
