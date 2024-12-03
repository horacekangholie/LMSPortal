
Partial Class Maintenance_Access_Map
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Module Access Map"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            ' Initial data loading
            PopulateGridViewData(TB_Search.Text)
        End If
    End Sub


    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT ID, ROW_NUMBER() OVER(ORDER BY Level_1, level_2) AS Seq_No, Module_Name, Sub_Module, Description, Admin, Tech, Sales, CtrAdmin " &
                                   "FROM DB_Access_Map WHERE Sub_Module NOT IN ('Access Map') " &
                                   " AND Sub_Module LIKE '%" & keyword & "%' " &
                                   "ORDER BY Level_1, Level_2 "

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


    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        CurrentPageIndex.Value = e.NewPageIndex.ToString()   ' Update the hidden field to reflect the new page index
        PopulateGridViewData(TB_Search.Text)
    End Sub


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"No", "Module", "Sub Module", "Description", "Admin", "Tech", "Sales", "CtrAdmin"}
        Dim ColSize() As Integer = {0, 300, 300, 600, 100, 100, 100, 100}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = ColSize(i)
                If i > 3 Then
                    e.Row.Cells(i).Style.Add("text-align", "center !important")
                End If
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
                If i > 3 Then
                    e.Row.Cells(i).Style.Add("text-align", "center !important")
                End If
            Next
        End If
    End Sub


    '' Checkboxes controls
    Protected Sub chk_CheckedChanged(sender As Object, e As EventArgs)
        Dim Sub_Module As String = (CType((CType(sender, CheckBox)).Parent.FindControl("LB_Sub_Module"), Label)).Text
        Dim Role As String = Replace(CType(sender, CheckBox).ID, "chk", "")
        Dim Status As String = IIf(CType(sender, CheckBox).Checked = "True", 1, 0)
        Dim sqlStr As String = " UPDATE DB_Access_Map SET " & Role & " = " & Status & " WHERE Sub_Module = '" & Sub_Module & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        ' Update hidden field instead of using session
        CurrentPageIndex.Value = GridView1.PageIndex.ToString()

        ' Re-bind the GridView data to reflect changes
        PopulateGridViewData(TB_Search.Text)

        ' Register a client-side script to click the BT_Search button
        Dim script As String = "document.getElementById('" & BT_Search.ClientID & "').click();"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "clickButtonScript", script, True)
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub


End Class
