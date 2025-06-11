Imports System.Data
Imports System.Data.SqlClient

Partial Class Views_DMC_Account_Revenue_By_Account_Type_Base_USD
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Revenue By Account Type (USD)"
    Dim HeadquarterCount, StoreCount, TotalAmount As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        '' Set the start and end of yearly report month
        Dim StartMonth As String = New Date(DateSerial(Year(Now) - 5, 1, 1).Year, 1, 1).ToString("yyyy-MM-dd")    '' One year early to track those contract start at one year earlier
        Dim EndMonth As String = DateSerial(Year(Now), Month(Now), 0).ToString("yyyy-MM-dd")

        '' Bind the country dropdownlist
        If Not IsPostBack Then
            Try
                '' Bind Country Dropdownlist
                Dim sqlStr As String = "SELECT DISTINCT Country FROM R_DMC_Subscription_Detail  " &
                                       "WHERE Start_Date >= '" & StartMonth & "' AND End_Date <= '" & EndMonth & "' " &
                                       "ORDER BY Country "

                BindDropDownList_Custom_Default_Value(DDL_Country, sqlStr, "Country", "Country", "ALL", "ALL", True)

                Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByValue(DDL_Country.SelectedValue))   '' default to select as singapore
                i = IIf(i < 0, 0, i)
                DDL_Country.SelectedIndex = i


                '' Bind Account Type Dropdownlist
                Dim sqlStr1 As String = "SELECT DISTINCT Device_Type FROM R_DMC_Subscription_Detail " &
                                        "WHERE Start_Date >= '" & StartMonth & "' AND End_Date <= '" & EndMonth & "' "

                If DDL_Country.SelectedValue <> "ALL" Then
                    sqlStr1 += "  AND Country = '" & DDL_Country.SelectedValue & "' "
                End If

                BindDropDownList_Custom_Default_Value(DDL_Account_Type, sqlStr1, "Device_Type", "Device_Type", "ALL (ALL, POS and RETAIL)", "ALLOFALL", True)

                Dim j = DDL_Account_Type.Items.IndexOf(DDL_Account_Type.Items.FindByValue(DDL_Account_Type.SelectedValue))   '' default to select as singapore
                j = IIf(j < 0, 0, j)
                DDL_Account_Type.SelectedIndex = j


                '' Populate Gridview
                BuildContencPage(Nothing, DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)

            Catch ex As Exception
                Response.Write("Error - Country Dropdownlist: " & ex.Message)
            End Try
        End If


        LB_Country.Text = DDL_Country.SelectedValue & " - " & DDL_Account_Type.SelectedItem.Text

    End Sub

    Protected Sub BuildContencPage(Optional ByVal ReportMonth As String = Nothing, Optional ByVal Country As String = Nothing, Optional DeviceType As String = Nothing)
        '' if ReportMonth value is empty then use the default month
        ReportMonth = IIf(ReportMonth Is Nothing, DateSerial(Year(Now), Month(Now) - 1, 1).ToString("yyyy-MM-dd"), ReportMonth)

        '' Get the Headquarter_Count, Store_Count, Total_Amount_Per_Month on Report Month
        Dim dReader = RunSQLExecuteReader("SELECT COUNT(Headquarter_ID) AS Headquarter_Count, SUM(Owned_Store) AS Store_Count, SUM(Total_Amount_Per_Month) AS Total_Amount FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ")
        While dReader.Read()
            HeadquarterCount = String.Format("{0:0}", dReader("Headquarter_Count"))
            StoreCount = String.Format("{0:0}", dReader("Store_Count"))
            TotalAmount = String.Format("{0:#,##0.00}", dReader("Total_Amount"))
        End While
        dReader.Close()

        Try
            '' Run store procedured to populate data to Temptable_DMC_Monthly_Revenue_Summary in SQL
            Dim StartMonth As String = DateSerial(Year(Now) - 5, Month(Now), 1).ToString("yyyy-MM-dd")
            Dim EndMonth As String = DateSerial(Year(Now), Month(Now), 0).ToString("yyyy-MM-dd")

            '' Insert data to temp table
            RunSQL("EXEC dbo.SP_Insert_TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary '" & StartMonth & "', '" & EndMonth & "', '" & Country & "', '" & DeviceType & "' ")

            Dim sqlStr() As String = {"SELECT * FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ",
                                      "SELECT * FROM R_DMC_Subscription_Revenue_By_Account_Type_Base_USD_Overview ORDER BY [Year] DESC, CASE Col WHEN 'Amount' THEN 1 WHEN 'No of Store' THEN 2 ELSE 3 END ",
                                      "SELECT Category AS Country, Stores, Total, Average FROM DMC_Monthly_Subscription_Statistics_USD ('ByCountry', '" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ORDER BY CASE Category WHEN 'Total' THEN 1 ELSE 0 END ",
                                      "SELECT Category AS Customer, Stores, Total, Average FROM DMC_Monthly_Subscription_Statistics_USD ('ByCustomer', '" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ORDER BY CASE Category WHEN 'Total' THEN 2 WHEN 'Others' THEN 1 ELSE 0 END ",
                                      "SELECT Category AS Segment, Stores, Total, Average FROM DMC_Monthly_Subscription_Statistics_USD ('BySegment', '" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ORDER BY CASE Category WHEN 'Total' THEN 1 ELSE 0 END "}

            ' Build and bind Gridview
            BuildGridView(GridView1, "GridView1", "Headquarter_ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Year")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Country")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "Customer")
            GridView4.DataSource = GetDataTable(sqlStr(3))
            GridView4.DataBind()

            BuildGridView(GridView5, "GridView5", "Segment")
            GridView5.DataSource = GetDataTable(sqlStr(4))
            GridView5.DataBind()

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
                GridViewObj.AllowSorting = True
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

            Case "GridView3"
                GridViewObj.AllowPaging = False
                GridViewObj.AllowSorting = True
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"Country", "Stores", "Total", "Average"}
                Dim ColData() As String = {"Country", "Stores", "Total", "Average"}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.SortExpression = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    If Bfield.HeaderText.Contains("Total") Or Bfield.HeaderText.Contains("Average") Then
                        Bfield.DataFormatString = "{0:#,##0}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next

            Case "GridView4"
                GridViewObj.AllowPaging = False
                GridViewObj.AllowSorting = True
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"Customer", "Stores", "Total", "Average"}
                Dim ColData() As String = {"Customer", "Stores", "Total", "Average"}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.SortExpression = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    If Bfield.HeaderText.Contains("Total") Or Bfield.HeaderText.Contains("Average") Then
                        Bfield.DataFormatString = "{0:#,##0}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next

            Case "GridView5"
                GridViewObj.AllowPaging = False
                GridViewObj.AllowSorting = True
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"Segment", "Stores", "Total", "Average"}
                Dim ColData() As String = {"Segment", "Stores", "Total", "Average"}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.SortExpression = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    If Bfield.HeaderText.Contains("Total") Or Bfield.HeaderText.Contains("Average") Then
                        Bfield.DataFormatString = "{0:#,##0}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next

        End Select
    End Sub



    ''Gridview controls
    Private Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated, GridView2.RowCreated, GridView3.RowCreated, GridView4.RowCreated, GridView5.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView_PreRender(sender As Object, e As EventArgs) Handles GridView1.PreRender, GridView3.PreRender, GridView4.PreRender, GridView5.PreRender
        Dim GridViewObj As GridView = CType(sender, GridView)
        RemoveSortArrowsFromGridView(GridViewObj)  ' Remove sorting arrow whenever the page is postback
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)

        If e.Row.RowType = DataControlRowType.Header Then
            ' Use the reusable method to insert the sort arrow and set default column index
            ApplySortArrow(GridViewObj, e.Row, 4)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' For each DataRow, right-align columns after index 3
            For i = 0 To e.Row.Cells.Count - 1
                If i > 3 Then
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                End If
            Next
            ' Highlight the cell in the currently sorted column
            e.Row.Cells(currentSortedColumnIndex).BackColor = Drawing.ColorTranslator.FromHtml("#ffffe6")
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Headquarter_Name")).Text = "Total Headquarter: " & HeadquarterCount

            e.Row.Cells(GetColumnIndexByName(e.Row, "Owned_Store")).Text = "Total Store: " & StoreCount
            e.Row.Cells(GetColumnIndexByName(e.Row, "Owned_Store")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Owned_Store")).Style.Add("font-weight", "bold !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Owned_Store")).Wrap = False

            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Text = "USD"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Currency")).Style.Add("font-weight", "bold !important")

            e.Row.Cells(GetColumnIndexByName(e.Row, "Total_Amount_Per_Month")).Text = TotalAmount
            e.Row.Cells(GetColumnIndexByName(e.Row, "Total_Amount_Per_Month")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Total_Amount_Per_Month")).Style.Add("font-weight", "bold !important")
        End If
    End Sub

    Protected Sub GridView1_Sorting(sender As Object, e As GridViewSortEventArgs) Handles GridView1.Sorting
        Dim GridViewObj As GridView = CType(sender, GridView)

        ' Read selected month and load footer summary counts
        Dim reportMonth = GetSelectedReportMonth()

        ' Repopulate the footer count when table is sorted
        LoadFooterCounts(reportMonth)

        ' Rebuild grid definition (columns, styles, etc.)
        BuildGridView(GridViewObj, "GridView1", "Headquarter_ID")

        ' Fetch the full dataset for that month
        Dim eom As String = GetEndOfMonthDate(reportMonth).ToString("yyyy-MM-dd")
        Dim sqlStr As String = "SELECT * FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & eom & "') "
        Dim dt As DataTable = GetDataTable(sqlStr)
        Dim dataView As New DataView(dt)

        ' Toggle/view‐state logic for sorting, then apply to DataView
        Dim defaultFirstExpr As String = GridViewObj.Columns(0).SortExpression
        Dim fullSortExpr As String = BuildSortExpression(e.SortExpression, defaultFirstExpr)
        dataView.Sort = fullSortExpr

        ' Bind sorted DataView back to the GridView
        GridViewObj.DataSource = dataView
        GridViewObj.DataBind()
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
        Dim ColName() As String = {"Year", "COL", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "Yearly Total"}
        Dim ColSize() As Integer = {150, 150, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 200}
        If e.Row.RowType = DataControlRowType.Header Then
            ApplySimpleHeaderFormatting(e.Row, ColName, ColSize, rightAlignFromIndex:=1)
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = ""  '' remove the column header

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 0 Then
                    e.Row.Cells(i).Style.Add("text-align", "right !important")

                    If i > 1 Then
                        e.Row.Cells(i).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "No of store", e.Row.Cells(i).Text, CInt(e.Row.Cells(i).Text))
                    End If

                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Amount" Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6f2ff !important")
                    End If
                End If
            Next
            '' Hide the year for row that does not display amount
            If Not String.Equals(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text, "Amount", StringComparison.OrdinalIgnoreCase) Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = ""
                e.Row.Cells(GetColumnIndexByName(e.Row, "Total")).Text = ""
            End If
        End If
    End Sub

    Protected Sub GridView3_4_5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound, GridView4.RowDataBound, GridView5.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        ' Set defaults; we’ll override in the Select Case below
        Dim keyFieldName As String = ""
        Dim colNames() As String = {}
        Dim colSizes() As Integer = {}
        Const rightAlignIndex As Integer = 1

        Select Case GridViewObj.ID
            Case "GridView3"
                keyFieldName = "Country"
                colNames = {"Country", "Stores", "Total", "Average"}
                colSizes = {150, 100, 100, 100}

            Case "GridView4"
                keyFieldName = "Customer"
                colNames = {"Customer", "Stores", "Total", "Average"}
                colSizes = {150, 100, 100, 100}

            Case "GridView5"
                keyFieldName = "Segment"
                colNames = {"Segment", "Stores", "Total", "Average"}
                colSizes = {150, 100, 100, 100}

            Case Else
                Return  ' shouldn’t happen, but guard anyway
        End Select

        If e.Row.RowType = DataControlRowType.Header Then
            ' Insert sort arrow based on ViewState(“SortExpression”/“SortDirection”)
            ApplySortArrow(GridViewObj, e.Row, 1)

            ' Then update LinkButton.Text & styling—do NOT overwrite the cell entirely:
            ApplySimpleHeaderFormatting(e.Row, colNames, colSizes, rightAlignIndex)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Apply the “Total or not” styling (table-active for Total, right-align others)
            ApplySimpleDataRowFormatting(e.Row, e.Row.DataItem, keyFieldName, rightAlignIndex)

            ' Highlight the cell in the currently sorted column, but skip the Total row
            Dim thisCategory As String = DataBinder.Eval(e.Row.DataItem, keyFieldName).ToString()
            If Not String.Equals(thisCategory, "Total", StringComparison.OrdinalIgnoreCase) Then
                e.Row.Cells(currentSortedColumnIndex).BackColor = Drawing.ColorTranslator.FromHtml("#ffffe6")
            End If
        End If
    End Sub

    Protected Sub GridView3_4_5_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs) Handles GridView3.Sorting, GridView4.Sorting, GridView5.Sorting
        Dim GridViewObj As GridView = CType(sender, GridView)

        ' Re‐fetch ReportMonth
        Dim reportMonth As String = GetSelectedReportMonth()
        Dim eom As String = GetEndOfMonthDate(reportMonth).ToString("yyyy-MM-dd")

        ' Pick the correct SQL string & data‐key name based on which GridView called
        Dim sqlStr As String = ""
        Dim dataKeyName As String = ""
        Select Case GridViewObj.ID
            Case "GridView3"
                sqlStr = "SELECT Category AS Country, Stores, Total, Average " &
                         "FROM DMC_Monthly_Subscription_Statistics_USD('ByCountry','" & eom & "') " &
                         "ORDER BY CASE Category WHEN 'Total' THEN 1 ELSE 0 END"
                dataKeyName = "Country"

            Case "GridView4"
                sqlStr = "SELECT Category AS Customer, Stores, Total, Average " &
                         "FROM DMC_Monthly_Subscription_Statistics_USD('ByCustomer','" & eom & "') " &
                         "ORDER BY CASE Category WHEN 'Total' THEN 2 WHEN 'Others' THEN 1 ELSE 0 END"
                dataKeyName = "Customer"

            Case "GridView5"
                sqlStr = "SELECT Category AS Segment, Stores, Total, Average " &
                         "FROM DMC_Monthly_Subscription_Statistics_USD('BySegment','" & eom & "') " &
                         "ORDER BY CASE Category WHEN 'Total' THEN 1 ELSE 0 END"
                dataKeyName = "Segment"

            Case Else
                Return
        End Select

        ' Re‐build the grid’s columns & styling exactly as in BuildContencPage
        BuildGridView(GridViewObj, GridViewObj.ID, dataKeyName)   ' use "GridView3"/"GridView4"/"GridView5" for name

        ' Load DataTable
        Dim dt As DataTable = GetDataTable(sqlStr)

        ' Add an “IsTotal” column to each row: 1 if Category="Total", else 0
        dt.Columns.Add("IsTotal", GetType(Integer))
        For Each row As DataRow In dt.Rows
            If String.Equals(row(dataKeyName).ToString(), "Total", StringComparison.OrdinalIgnoreCase) Then
                row("IsTotal") = 1
            Else
                row("IsTotal") = 0
            End If
        Next

        ' Wrap DataView
        Dim dataView As New DataView(dt)

        ' Toggle the sort expression in ViewState & get full "ColName ASC/DESC"
        Dim defaultFirstExpr As String = GridViewObj.Columns(0).SortExpression
        Dim userSortExpr As String = BuildSortExpression(e.SortExpression, defaultFirstExpr)

        ' Prepend “IsTotal ASC” so that non-Total rows (IsTotal=0) come first,
        ' then apply the user's column sort on the remainder.
        dataView.Sort = "IsTotal ASC, " & userSortExpr

        ' Bind the sorted DataView back
        GridViewObj.DataSource = dataView
        GridViewObj.DataBind()
    End Sub


    '' Dropdownlist
    Protected Sub DDL_ReportMonth_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_ReportMonth.Load
        Dim DDL_ReportMonth As DropDownList = TryCast(sender, DropDownList)
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT MonthYearList, EOMONTH(MonthYearList) AS ReportMonth " &
                                       " FROM dbo.Get_MonthYearList(DATEADD(MONTH, -15, GETDATE()), DATEADD(MONTH, -1, GETDATE())) " &
                                       " ORDER BY CAST(EOMONTH(CAST(MonthYearList AS date), 0) AS Date) DESC "

                DDL_ReportMonth.DataSource = GetDataTable(sqlStr)
                DDL_ReportMonth.DataTextField = "MonthYearList"
                DDL_ReportMonth.DataValueField = "ReportMonth"
                DDL_ReportMonth.DataBind()
            Catch ex As Exception
                Response.Write("Error - ReportMonth Dropdownlist: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_ReportMonth_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_ReportMonth.SelectedIndexChanged
        RefreshAllGridViews()   '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
    End Sub

    Protected Sub DDL_Country_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Country.SelectedIndexChanged
        '' Set the start and end of yearly report month
        Dim StartMonth As String = New Date(DateSerial(Year(Now) - 5, 1, 1).Year, 1, 1).ToString("yyyy-MM-dd")    '' One year early to track those contract start at one year earlier
        Dim EndMonth As String = DateSerial(Year(Now), Month(Now), 0).ToString("yyyy-MM-dd")

        Dim sqlStr1 As String = "SELECT DISTINCT Device_Type FROM R_DMC_Subscription_Detail " &
                                "WHERE Start_Date >= '" & StartMonth & "' AND End_Date <= '" & EndMonth & "' "

        If DDL_Country.SelectedValue <> "ALL" Then
            sqlStr1 += "  AND Country = '" & DDL_Country.SelectedValue & "' "
        End If

        DDL_Account_Type.DataSource = GetDataTable(sqlStr1)
        DDL_Account_Type.DataTextField = "Device_Type"
        DDL_Account_Type.DataValueField = "Device_Type"
        DDL_Account_Type.Items.Clear()
        DDL_Account_Type.Items.Insert(0, New ListItem("ALL (ALL, POS and RETAIL)", "ALLOFALL"))
        DDL_Account_Type.DataBind()

        Dim j = DDL_Account_Type.Items.IndexOf(DDL_Account_Type.Items.FindByValue(DDL_Account_Type.SelectedValue))   '' default to select as singapore
        j = IIf(j < 0, 0, j)
        DDL_Account_Type.SelectedIndex = j

        RefreshAllGridViews()   '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
    End Sub

    Protected Sub DDL_Account_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Account_Type.SelectedIndexChanged
        RefreshAllGridViews()   '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
    End Sub


    '' Common events
    Private Sub RefreshAllGridViews()
        ' Clear the stored sort column/direction
        ViewState("SortExpression") = String.Empty

        ' Call BuildContencPage with the currently selected dropdown values
        Dim reportMonth As String = GetEndOfMonthDate(DDL_ReportMonth.SelectedValue).ToString("yyyy-MM-dd")
        BuildContencPage(reportMonth, DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)
    End Sub

    Private Function GetSelectedReportMonth() As String
        ' DDL_ReportMonth.SelectedValue in EOMONTH string (e.g. "2025-05-31").
        ' still CDate→ToString to ensure the correct format.
        Return CDate(DDL_ReportMonth.SelectedValue).ToString("yyyy-MM-dd")
    End Function

    Private Sub LoadFooterCounts(reportMonth As String)
        Dim eom As String = GetEndOfMonthDate(reportMonth).ToString("yyyy-MM-dd")
        Dim sql As String = "SELECT COUNT(Headquarter_ID) AS Headquarter_Count, " &
                            "       SUM(Owned_Store)      AS Store_Count,      " &
                            "       SUM(Total_Amount_Per_Month) AS Total_Amount " &
                            "FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & eom & "')"

        Dim dr = RunSQLExecuteReader(sql)
        If dr.Read() Then
            HeadquarterCount = String.Format("{0:0}", dr("Headquarter_Count"))
            StoreCount = String.Format("{0:0}", dr("Store_Count"))
            TotalAmount = String.Format("{0:#,##0.00}", dr("Total_Amount"))
        End If
        dr.Close()
    End Sub




    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Dim Page_Origin As String = Get_Value("SELECT TOP 1 Page_Origin FROM DMC_Account_Reports_List WHERE ID = " & Request.QueryString("ID"), "Page_Origin")
        Response.Redirect(Page_Origin)
    End Sub



End Class
