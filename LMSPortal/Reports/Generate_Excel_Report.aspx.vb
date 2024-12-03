Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Class Reports_Generate_Excel_Report
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Excel Reports"
    Dim sqlStr As String = "SELECT ROW_NUMBER() OVER(ORDER BY Report_Name ASC) AS No, * FROM Excel_Reports_List "
    Dim reportColName As String() = {"Report_Name", "Description"}
    Dim reportColSize As Integer() = {200, 300}
    Dim orderByCol As String = "Report_Name"

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
            sqlStr += " WHERE Report_Name LIKE '%" & keyword & "%' "
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

    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Report_Name", "Description", ""}
        Dim ColSize() As Integer = {600, 1400, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Width = ColSize(i)
                e.Row.Cells(i).Wrap = True
                If i = CtrlCellIndex Then
                    e.Row.Cells(CtrlCellIndex).Width = 0
                    e.Row.Cells(CtrlCellIndex).Wrap = False
                    e.Row.Cells(CtrlCellIndex).HorizontalAlign = HorizontalAlign.Center
                End If
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

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub


    '' Button click to open modals
    Protected Sub BN_AddNew_Report_Click(sender As Object, e As EventArgs) Handles BN_AddNew.Click
        Try
            Dim sqlStr As String = " EXEC SP_Create_Excel_Report " &
                                   "  N'" & EscapeChar(TB_Report_Name.Text) & "'" &
                                   ", N'" & EscapeChar(TB_Description.Text) & "'" &
                                   ", N'" & TB_Sheet_Name.Text & "'" &
                                   ", N'" & EscapeChar(TB_Sql_Strings.Text) & "'"
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' Initialize field once record added
        TB_Report_Name.Text = String.Empty
        TB_Description.Text = String.Empty
        TB_Sheet_Name.Text = String.Empty
        TB_Sql_Strings.Text = String.Empty

        BuildPageContent(TB_Search.Text)
    End Sub


    Protected Sub DownloadReport(sender As Object, e As EventArgs)
        Dim GenerateBtn As LinkButton = CType(sender, LinkButton)
        Dim ReportID As Integer = GenerateBtn.CommandArgument
        Dim ReportName As String = Get_Value("SELECT Report_Name FROM Excel_Reports_List WHERE ID = " & ReportID, "Report_Name")
        Dim ReportSQL As String = Get_Value("SELECT SQLString FROM Excel_Reports_List WHERE ID = " & ReportID, "SQLString")
        DownloadExcel(ReportSQL, ReportName, "Excel", "Excel Reports")
    End Sub

    Protected Sub DeleteReport(sender As Object, e As EventArgs)
        Dim ID As Integer = Integer.Parse(TryCast(sender, LinkButton).CommandArgument)
        Dim query As String = " DELETE FROM Excel_Reports_List WHERE ID = @ID "
        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString
        Using con As SqlConnection = New SqlConnection(constr)
            Using cmd As SqlCommand = New SqlCommand(query)
                cmd.Parameters.AddWithValue("@ID", ID)
                cmd.Connection = con
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
        BuildPageContent(TB_Search.Text)
    End Sub

End Class
