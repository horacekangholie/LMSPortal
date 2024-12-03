
Partial Class Adhoc_Diskspace_Reading
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Server Diskspace Reading"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
            TB_DB_Size.Text = Get_Value("SELECT LEFT(pg_size_pretty, Len(pg_size_pretty) - 3) AS DB_Size FROM L_database_size", "DB_Size")   '' Get the database size from DMC
        End If
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT * FROM Server_Space  "
            sqlStr += "WHERE " & IIf(Len(keyword) > 0, "", "YEAR(Reading_Date) <= YEAR(GETDATE()) AND ") & "(Reading_Date LIKE '%" & keyword & "%' OR Remarks LIKE '%" & keyword & "%') "
            sqlStr += "ORDER BY CAST(Reading_Date AS datetime) DESC, ID DESC "

            'Response.Write(sqlStr)
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
        Dim ColName() As String = {"Date", "Total Space", "Used", "Avail", "Percentage Used", "DB Size", "Remarks", ""}
        Dim ColSize() As Integer = {100, 100, 100, 100, 100, 100, 400, 0}

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
                'DeleteCommandField.Attributes.Add("onclick", "return confirm('Are you sure to delete record?');")
                DeleteCommandField.Attributes.Add("onclick", "javascript:if (confirm('Are you sure to delete record?')){return true;} else {return false;}")
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
    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Total_Space As String = TB_Total_Space.Text
        Dim Used As String = TB_Used.Text
        Dim Avail As String = TB_Avail.Text
        Dim Percentage_Used As String = TB_Percentage_Used.Text
        Dim DB_Size As String = Trim(TB_DB_Size.Text)
        Dim Remarks As String = EscapeChar(TB_Remarks.Text)

        Dim sqlStr As String = "INSERT INTO Server_Space(Reading_Date, Size, Used, Avail, Used_Percentage, DB_Size, Remarks) " &
                               "VALUES(GETDATE(), N'" & Total_Space & "', N'" & Used & "', N'" & Avail & "', N'" & Percentage_Used & "', N'" & DB_Size & "', N'" & Remarks & "') "

        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' clear field after added
        TB_Total_Space.Text = String.Empty
        TB_Used.Text = String.Empty
        TB_Avail.Text = String.Empty
        TB_Percentage_Used.Text = String.Empty
        'TB_DB_Size.Text = String.Empty
        TB_Remarks.Text = String.Empty

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim Reading_Date As String = Trim((TryCast(row.FindControl("TB_E_Reading_Date"), TextBox)).Text)
        Dim Size As String = Trim((TryCast(row.FindControl("TB_E_Size"), TextBox)).Text)
        Dim Used As String = Trim((TryCast(row.FindControl("TB_E_Used"), TextBox)).Text)
        Dim Avail As String = Trim((TryCast(row.FindControl("TB_E_Avail"), TextBox)).Text)
        Dim Used_Percentage As String = Trim((TryCast(row.FindControl("TB_E_Used_Percentage"), TextBox)).Text)
        Dim DB_Size As String = Trim((TryCast(row.FindControl("TB_E_DB_Size"), TextBox)).Text)
        Dim Remarks As String = EscapeChar(Trim((TryCast(row.FindControl("TB_E_Remarks"), TextBox)).Text))

        Dim sqlStr As String = " UPDATE Server_Space SET Size = N'" & Size &
                                                     "', Used = N'" & Used &
                                                    "', Avail = N'" & Avail &
                                          "', Used_Percentage = N'" & Used_Percentage &
                                                  "', DB_Size = N'" & DB_Size &
                                                  "', Remarks = N'" & Remarks &
                                                  "' WHERE ID = " & ID
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
        Dim sqlStr As String = " DELETE FROM Server_Space WHERE ID = " & ID
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




    '' Search box
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub


End Class
