
Partial Class Reports_SubReport
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)

        Dim GroupName As String = Replace(Request.QueryString("Group"), "_", " ")
        Dim AccountType As String = Replace(Request.QueryString("Account_Type"), "_", " ")
        Dim ColNo As Integer = Request.QueryString("ColNo")

        Try
            Dim sqlStr As String = "SELECT * FROM _AccountUsage WHERE [Group] = '" & GroupName & "' AND Account_Type = '" & AccountType & "' "

            Select Case ColNo
                Case 1
                    sqlStr += "AND DATEDIFF(d, Last_Used_Date, GETDATE()) <= 30 "
                Case 2
                    sqlStr += "AND DATEDIFF(d, Last_Used_Date, GETDATE()) > 30 AND DATEDIFF(d, Last_Used_Date, GETDATE()) <= 180 "
                Case 3
                    sqlStr += "AND DATEDIFF(d, Last_Used_Date, GETDATE()) > 180 AND DATEDIFF(d, Last_Used_Date, GETDATE()) <= 365 "
                Case 4
                    sqlStr += "AND DATEDIFF(d, Last_Used_Date, GETDATE()) > 365 "
                Case 5
                    sqlStr += "AND Last_Used_Date IS NULL "
                Case Else
                    sqlStr = sqlStr
            End Select

            sqlStr += "AND (Customer_Name LIKE N'%" & keyword & "%' OR Headquarter_Name LIKE N'%" & keyword & "%' OR Store_Name LIKE N'%" & keyword & "%') "

            sqlStr += "ORDER BY Customer_Name "

            '' Formatting GridView
            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView()
        '' GridView Properties
        GridView1.AutoGenerateColumns = False
        GridView1.AllowPaging = True
        GridView1.PageSize = 20
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"Group", "Account_Type"}
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

        Dim reportColName As String() = {"Customer_Name", "Group", "Headquarter_ID", "Headquarter_Name", "Store_Code", "Store_Name", "Created_Date", "Inactive_Date", "Status", "Account_Type", "Last_Used_Date"}
        Dim reportColSize As Integer() = {250, 100, 100, 250, 100, 250, 100, 100, 100, 100, 100}

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

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Style.Add("background-color", "#B8DAFF")
            Next
        End If

        For i = 0 To e.Row.Cells.Count - 1
            If i > 1 Then
                e.Row.Cells(i).Wrap = False
            End If
        Next
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent()
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Try
            RunSQL("EXEC SP_Sync_LMS_Licence")
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs)
        '' Close the browser
        Response.Write("<script>window.close();</script>")
    End Sub

End Class
