
Partial Class Reports_New_Licence_By_PO
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Licence Activation Keys By PO No"
    Dim sqlStr As String = "SELECT [Licensee], [PO No], FORMAT([PO Date], 'yyyy-MM-dd') AS [PO Date], [Application Type], [OS Type], [Chargeable], [Licence Code], FORMAT([Created Date], 'yyyy-MM-dd') AS [Created Date], [Status], [Requested By], [Remarks]  " &
                           "FROM _LMS_Licence_Details "

    Dim reportColName As String() = {"Licensee", "PO No", "PO Date", "Application Type", "OS Type", "Chargeable", "Licence Code", "Created Date", "Status", "Requested By", "Remarks"}
    Dim reportColSize As Integer() = {300, 100, 100, 200, 100, 50, 200, 150, 100, 200, 300}

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
            Dim LatestPO As String = Get_Value("SELECT TOP 1 PO_No FROM LMS_Licence ORDER BY ID DESC", "PO_No")
            Dim LatestCustomerID As String = Get_Value("SELECT TOP 1 Customer_ID FROM LMS_Licence ORDER BY ID DESC", "Customer_ID")
            Dim LatestCreatedDate As String = Get_Value("SELECT TOP 1 Created_Date FROM LMS_Licence ORDER BY [ID] DESC", "Created_Date")

            If Len(keyword) > 0 Then
                sqlStr += " WHERE [PO No] LIKE '%" & keyword & "%' "
            Else
                sqlStr += " WHERE [PO No] LIKE '%" & LatestPO & "%' AND [Customer ID] LIKE '%" & LatestCustomerID & "%' AND [Created Date] LIKE '%" & CDate(LatestCreatedDate).ToString("yyyy-MM-dd") & "%' "
            End If

            sqlStr += " ORDER BY [PO Date] DESC, [Application Type] "

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
        ReportSQL.Text = sqlStr

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
        GridView1.DataKeyNames = New String() {"Licence Code"}
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
        End If
    End Sub


    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub


    '' Download excel report button
    Protected Sub BT_Download_Excel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Download_Excel.Click
        DownloadExcel(ReportSQL.Text, "Licence Order", "Excel", "General")
    End Sub


End Class
