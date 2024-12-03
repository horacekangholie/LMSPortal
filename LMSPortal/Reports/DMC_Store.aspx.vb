
Partial Class Reports_DMC_Store
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Store FTP Info"
    Dim sqlStr As String = "SELECT ROW_NUMBER() OVER(ORDER BY [Headquarter Name] ASC) AS No, * FROM R_DMC_Store_Licence "

    Dim reportColName As String() = {"Headquarter ID", "Headquarter Name", "Store No", "Store Name", "Public IP", "FTP Host", "FTP User", "FTP Password", "Status", "Customer"}
    Dim reportColSize As Integer() = {100, 300, 100, 200, 100, 100, 100, 100, 100, 300}

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
            sqlStr += " WHERE [Headquarter ID] LIKE '%" & keyword & "%' OR [Headquarter Name] LIKE '%" & keyword & "%' OR [Store Name] LIKE '%" & keyword & "%' OR [Customer] LIKE '%" & keyword & "%' "
            sqlStr += " ORDER BY [Headquarter ID], [Store No] "

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

        '' SQL for excel report download
        '' ReportSQL is hidden asp label in markup page
        ReportSQL.Text = " SELECT ROW_NUMBER() OVER(ORDER BY [Headquarter ID] ASC) AS No, [Headquarter ID], [Headquarter Name], [Store No], [Store Name], [Public IP], [FTP Host], [FTP User], [FTP Password], [Status], [Customer] " &
                         " FROM R_DMC_Store_Licence " &
                         " WHERE [Headquarter ID] LIKE '%" & keyword & "%' OR [Headquarter Name] LIKE '%" & keyword & "%' OR [Store Name] LIKE '%" & keyword & "%' OR [Customer] LIKE '%" & keyword & "%' " &
                         " ORDER BY [Headquarter ID], [Store No] "
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

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).Style.Add("font-family", "Menlo,Monaco,Consolas,Liberation Mono,courier New,monospace")
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).Style.Add("background-color", "#fbfcfc")
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).Style.Add("font-family", "Menlo,Monaco,Consolas,Liberation Mono,courier New,monospace")
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).Style.Add("background-color", "#fbfcfc")
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub


    '' Download excel report button
    Protected Sub BT_Download_Excel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Download_Excel.Click
        DownloadExcel(ReportSQL.Text, "DMC Store FTP Info", "Excel", "General")
    End Sub

End Class
