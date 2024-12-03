
Partial Class Services_Maintenance_Revenue
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Maintenance Contract Revenue"
    Dim MonthlyTotal, MonthlyAverage, MonthlyMedian, MonthlyNoOfContract As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        BuildContenctPage()
    End Sub

    Protected Sub BuildContenctPage(Optional ByVal ReportMonth As String = Nothing)
        Dim Services_Group As String = Left(Session("User_Group"), 1)

        '' if ReportMonth value is empty then use the default month
        ReportMonth = IIf(ReportMonth Is Nothing, DateSerial(Year(Now), Month(Now), 0).ToString("yyyy-MM-dd"), ReportMonth)

        '' Get the MonthlyTotal, MonthlyAverage, MonthlyNoOfStore and MonthlyMedian based on Report Month
        Dim dReader = RunSQLExecuteReader("SELECT SUM([Amount On Month]) AS [Monthly Total], CAST(AVG([Amount On Month]) AS decimal(10,2)) AS Average, COUNT([Reference No]) AS [Contract Count],  dbo.Get_Maintenance_Revenue_Median('" & ReportMonth & "', '" & Services_Group & "') AS Median FROM Maintenance_Monthly_Revenue('" & ReportMonth & "', '" & Services_Group & "')")
        While dReader.Read()
            MonthlyTotal = String.Format("{0:#,##0.00}", dReader("Monthly Total"))
            MonthlyAverage = String.Format("{0:#,##0.00}", dReader("Average"))
            MonthlyMedian = String.Format("{0:#,##0.00}", dReader("Median"))
            MonthlyNoOfContract = String.Format("{0:0}", dReader("Contract Count"))
        End While
        dReader.Close()

        Try
            '' Run store procedured to populate data to Temptable_Maintenance_Services_Monthly_Revenue_Summary in SQL
            Dim StartMonth As String = DateSerial(Year(Now) - 5, Month(Now), 1).ToString("yyyy-MM-dd")
            Dim EndMonth As String = GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd")

            RunSQL("EXEC dbo.SP_Insert_TempTable_Maintenance_Services_Monthly_Revenue_Summary '" & StartMonth & "', '" & EndMonth & "', '" & Services_Group & "' ")

            Dim sqlStr() As String = {"SELECT * FROM Maintenance_Monthly_Revenue('" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "', '" & Services_Group & "') ORDER BY [Customer Name]",
                                      "SELECT * FROM R_Maintenance_Services_Revenue_Overview ORDER BY [Year] DESC, CASE Col WHEN 'Amount' THEN 1 WHEN 'No of contract' THEN 2 ELSE 3 END "}

            ' Build and bind Gridview
            BuildGridView(GridView1, "GridView1", "Unique ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Year")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowHeaderWhenEmpty = True
        GridViewObj.DataKeyNames = New String() {DataKeyName}
        GridViewObj.CssClass = "table table-bordered"

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
                GridViewObj.AllowPaging = False
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Customer Name", "Store Name", "Reference No", "Start Date", "End Date", "Contract Value", "Period In Days", "Daily Rate", "Valid Days", "Amount On Month"}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    If Bfield.DataField.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                    End If
                    If Bfield.DataField.Contains("Amount") Or Bfield.DataField.Contains("Value") Then
                        Bfield.DataFormatString = "{0:C}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = True

            Case "GridView2"
                GridViewObj.AllowPaging = False
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Year", "COL", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Total"}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    If Not Bfield.DataField.Contains("Year") Then
                        Bfield.DataFormatString = "{0:C}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
        End Select
    End Sub


    ''Gridview controls
    Private Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated, GridView2.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Customer", "Store", "Contract Reference No", "Contract<br>From", "Contract<br>To", "Contract Value<br>(A)", "Duration (Days)<br>(B)", "Daily Rate<br>(C) = (A) / (B)", "Coverage (Days)<br>(D)", "Amount (SGD)<br>(E) = (C) * (D)"}
        Dim ColSize() As Integer = {300, 300, 200, 100, 100, 200, 0, 0, 0, 100}
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                e.Row.Cells(i).Height = 60
                e.Row.Cells(i).Width = ColSize(i)
                If i > 2 Then
                    e.Row.Cells(i).Style.Add("text-align", "right !important")
                End If
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 2 Then
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                End If
            Next

            '' Format the data row on refernce no column
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Text = drv("Reference No")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Style.Add("font-style", "italic")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Style.Add("color", "#808080")

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Text = "<br>No of contract: " & MonthlyNoOfContract

            e.Row.Cells(GetColumnIndexByName(e.Row, "Valid Days")).Text = "Monthly Total<br>Mean(Avg)<br>Median"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Valid Days")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Valid Days")).Style.Add("font-weight", "bold !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount On Month")).Text = String.Format("{0:c}", CDec(MonthlyTotal)) & "<br>" & String.Format("{0:c}", CDec(MonthlyAverage)) & "<br>" & String.Format("{0:c}", CDec(MonthlyMedian))
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount On Month")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Amount On Month")).Style.Add("font-weight", "bold !important")
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
        Dim ColName() As String = {"Year", "COL", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Yearly Total"}
        Dim ColSize() As Integer = {150, 150, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 200}
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", "")
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                e.Row.Cells(i).Width = ColSize(i)
                If i > 0 Then
                    e.Row.Cells(i).Style.Add("text-align", "right !important")
                End If
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = ""  '' remove the column header
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 0 Then
                    e.Row.Cells(i).Style.Add("text-align", "right !important")

                    If i > 1 Then
                        e.Row.Cells(i).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "No of contract", e.Row.Cells(i).Text, CInt(e.Row.Cells(i).Text))
                    End If

                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Amount" Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                    End If
                End If
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Amount", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text)
            e.Row.Cells(GetColumnIndexByName(e.Row, "Total")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Amount", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Total")).Text)
        End If
    End Sub


    '' Dropdownlist
    Protected Sub DDL_ReportMonth_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_ReportMonth.Load
        Dim DDL_ReportMonth As DropDownList = TryCast(sender, DropDownList)
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT MonthYearList, CAST(MonthYearList AS date) AS ReportMonth " &
                                       " FROM dbo.Get_MonthYearList(DATEADD(MONTH, -15, GETDATE()), DATEADD(MONTH, -1, GETDATE())) " &
                                       " ORDER BY CAST(EOMONTH(CAST(MonthYearList AS date), 0) AS Date) DESC "

                DDL_ReportMonth.DataSource = GetDataTable(sqlStr)
                DDL_ReportMonth.DataTextField = "MonthYearList"
                DDL_ReportMonth.DataValueField = "ReportMonth"
                DDL_ReportMonth.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_ReportMonth_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ReportMonth.SelectedIndexChanged
        BuildContenctPage(CDate(DDL_ReportMonth.SelectedValue).ToString("yyyy-MM-dd"))
    End Sub




    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Dim Page_Origin As String = Get_Value("SELECT TOP 1 Page_Origin FROM Maintenance_Services_Reports_List WHERE ID = " & Request.QueryString("ID"), "Page_Origin")
        Response.Redirect(Page_Origin)
    End Sub


End Class
