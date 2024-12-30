
Partial Class Views_DMC_Maintenance_Downtime
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Maintenance Planned Downtime Report"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        Dim currentDate As DateTime = DateTime.Now
        Dim firstDayOfYear As New DateTime(currentDate.Year, 1, 1)
        Dim firstDayOfYearString As String = firstDayOfYear.ToString("yyyy-MM-dd")

        If Not IsPostBack Then
            PopulateGridViewData(firstDayOfYearString)
        End If
    End Sub

    Protected Sub PopulateGridViewData(ByVal year As String)
        Try
            Dim sqlStr As String = "SELECT * FROM DMC_Maintenance_History_Report('" & year & "') ORDER BY [Maintenance Date] DESC, [Down Time From] DESC "

            BuildGridView(GridView1, "GridView1", "Maintenance Date")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub


    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.AllowPaging = False
        GridViewObj.PageSize = 15
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowHeaderWhenEmpty = True
        GridViewObj.DataKeyNames = New String() {DataKeyName}
        GridViewObj.CssClass = "table table-bordered"
        GridViewObj.Style.Add("width", "70%")

        '' Header Style
        GridViewObj.HeaderStyle.CssClass = "table-primary"
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
        GridViewObj.PagerSettings.PageButtonCount = "10"
        GridViewObj.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewObj.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridViewObj.EmptyDataText = "No records found."

        '' Define each Gridview
        Select Case ControlName
            Case "GridView1"
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Maintenance Date", "Work Type", "Description", "Down Time From", "Down Time To", "Duration"}
                Dim ColName() As String = {"Maintenance Date", "Work Type", "Description", "Down Time From", "Down Time To", "Duration (Minutes)"}
                Dim ColSize() As Integer = {50, 100, 200, 50, 50, 0}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    Bfield.HeaderStyle.Wrap = False
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                    End If
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = True
        End Select
    End Sub





    Protected Sub DDL_Maintenance_Year_Load(sender As Object, e As EventArgs) Handles DDL_Maintenance_Year.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT DISTINCT CONVERT(VARCHAR, YEAR(Maintenance_Date)) AS [Year], DATEFROMPARTS(CONVERT(VARCHAR, YEAR(Maintenance_Date)), 1, 1) AS [FirstOfYear] FROM DMC_Maintenance_History WHERE Duration > 0 ORDER BY CONVERT(VARCHAR, YEAR(Maintenance_Date)) DESC "

                DDL_Maintenance_Year.DataSource = GetDataTable(sqlStr)
                DDL_Maintenance_Year.DataTextField = "Year"
                DDL_Maintenance_Year.DataValueField = "FirstOfYear"
                DDL_Maintenance_Year.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Maintenance_Year_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Maintenance_Year.SelectedIndexChanged
        PopulateGridViewData(DDL_Maintenance_Year.SelectedValue)
    End Sub



    '' Gridview controla
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim currentDate As DateTime = DateTime.Now
        Dim firstDayOfYear As New DateTime(currentDate.Year, 1, 1)
        Dim firstDayOfYearString As String = firstDayOfYear.ToString("yyyy-MM-dd")

        Dim reportYear As String = IIf(Len(DDL_Maintenance_Year.SelectedValue) > 0, DDL_Maintenance_Year.SelectedValue, firstDayOfYearString)
        Dim totalDuration As Integer = Get_Value("SELECT SUM(ISNULL(Duration, 0)) AS Total_Duration FROM DMC_Maintenance_History_Report('" & reportYear & "') ", "Total_Duration")

        Dim percentageDownTime As Double = totalDuration / (60 * 24 * 365)
        Dim percentageUpTime As Double = 1 - percentageDownTime

        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Down Time To")).Text = "Total Down Time (mins)<br>HH:mm<br><br>Down Time<br>Up Time"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Down Time To")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Duration")).Text = totalDuration & "<br>" & ConvertMinutesToHHmm(totalDuration) & "<br><br>" & percentageDownTime.ToString("P2") & "<br>" & percentageUpTime.ToString("P2")
        End If
        e.Row.Cells(GetColumnIndexByName(e.Row, "Duration")).Style.Add("text-align", "right !important")
        e.Row.Cells(GetColumnIndexByName(e.Row, "Duration")).Style.Add("padding-right", "15px !important")
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub





    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Dim Page_Origin As String = Get_Value("SELECT TOP 1 Page_Origin FROM DMC_Account_Reports_List WHERE ID = " & Request.QueryString("ID"), "Page_Origin")
        Response.Redirect(Page_Origin)
    End Sub


End Class
