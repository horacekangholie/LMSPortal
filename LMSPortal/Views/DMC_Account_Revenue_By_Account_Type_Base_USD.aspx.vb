Imports System.Data
Imports System.Data.SqlClient

Partial Class Views_DMC_Account_Revenue_By_Account_Type_Base_USD
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Revenue By Account Type (USD)"
    Dim HeadquarterCount, StoreCount, TotalAmount As String
    Dim currentSortedColumnIndex As Integer


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
                BuildContenctPage(Nothing, DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)

            Catch ex As Exception
                Response.Write("Error - Country Dropdownlist: " & ex.Message)
            End Try
        End If


        LB_Country.Text = DDL_Country.SelectedValue & " - " & DDL_Account_Type.SelectedItem.Text

    End Sub

    Protected Sub BuildContenctPage(Optional ByVal ReportMonth As String = Nothing, Optional ByVal Country As String = Nothing, Optional DeviceType As String = Nothing)
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
                                      "SELECT * FROM R_DMC_Subscription_Revenue_By_Account_Type_Base_USD_Overview ORDER BY [Year] DESC, CASE Col WHEN 'Amount' THEN 1 WHEN 'No of Store' THEN 2 ELSE 3 END "}

            ' Build and bind Gridview
            BuildGridView(GridView1, "GridView1", "Headquarter_ID")
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

    Protected Sub GridView1_PreRender(sender As Object, e As EventArgs) Handles GridView1.PreRender
        ' Remove sorting arrow whenever the page is postback
        If GridView1.HeaderRow IsNot Nothing Then
            For Each cell As TableCell In GridView1.HeaderRow.Cells
                Dim span As Control = cell.Controls.OfType(Of HtmlGenericControl)().FirstOrDefault(Function(c) c.Attributes("class") = "sort-arrow")
                If span IsNot Nothing Then
                    cell.Controls.Remove(span)
                End If
            Next
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            Dim sortExpression As String = ViewState("SortExpression")?.ToString()
            Dim sortDirection As String = ViewState("SortDirection")?.ToString()

            ' If sortExpression is empty, set it to the first column's SortExpression
            If String.IsNullOrEmpty(sortExpression) Then
                sortExpression = GridView1.Columns(0).SortExpression
                sortDirection = "ASC"
                currentSortedColumnIndex = 0
            End If

            ' Loop through the headerrow control field to find which is the current selected column
            For Each field As DataControlField In GridView1.Columns
                If field.SortExpression = sortExpression Then
                    Dim cellIndex As Integer = GridView1.Columns.IndexOf(field)
                    Dim sortArrow As New Label()
                    sortArrow.CssClass = "sort-arrow " & If(sortDirection = "ASC", "asc", "desc")

                    ' Add the sorting arrow inside a <span> element
                    Dim span As New HtmlGenericControl("span")
                    span.Controls.Add(sortArrow)

                    ' Append the <span> to the header cell
                    e.Row.Cells(cellIndex).Controls.Add(span)

                    ' Get current sorted column index
                    currentSortedColumnIndex = cellIndex
                End If
            Next

            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                e.Row.Cells(i).Height = 60
                If i > 3 Then
                    e.Row.Cells(i).Style.Add("text-align", "right !important")
                End If
            Next

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 3 Then
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                End If
                e.Row.Cells(currentSortedColumnIndex).BackColor = Drawing.ColorTranslator.FromHtml("#ffffe6")
            Next
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
        '' if ReportMonth value is empty then use the default month
        Dim ReportMonth As String = CDate(DDL_ReportMonth.SelectedValue).ToString("yyyy-MM-dd")
        Dim sqlStr As String = "SELECT * FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') "

        '' Get the Headquarter_Count, Store_Count, Total_Amount_Per_Month on Report Month
        '' This is to repopulate the footer count when table is sorted
        Dim dReader = RunSQLExecuteReader("SELECT COUNT(Headquarter_ID) AS Headquarter_Count, SUM(Owned_Store) AS Store_Count, SUM(Total_Amount_Per_Month) AS Total_Amount FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD('" & GetEndOfMonthDate(ReportMonth).ToString("yyyy-MM-dd") & "') ")
        While dReader.Read()
            HeadquarterCount = String.Format("{0:0}", dReader("Headquarter_Count"))
            StoreCount = String.Format("{0:0}", dReader("Store_Count"))
            TotalAmount = String.Format("{0:#,##0.00}", dReader("Total_Amount"))
        End While
        dReader.Close()

        BuildGridView(GridView1, "GridView1", "Headquarter_ID")
        Dim dt As DataTable = GetDataTable(sqlStr)
        Dim dataView As New DataView(dt)
        Dim sortExpression As String = e.SortExpression

        If ViewState("SortDirection") IsNot Nothing AndAlso ViewState("SortExpression") IsNot Nothing Then
            Dim previousSortExpression As String = ViewState("SortExpression").ToString()
            Dim previousSortDirection As String = ViewState("SortDirection").ToString()

            If previousSortExpression = sortExpression Then
                ' If the same column is clicked again, toggle the sort direction
                If previousSortDirection = "ASC" Then
                    ViewState("SortDirection") = "DESC"
                Else
                    ViewState("SortDirection") = "ASC"
                End If
            Else
                ' If a new column is clicked, default to ascending order
                ViewState("SortDirection") = "DESC"
            End If
        Else
            ' If ViewState is empty, default to ascending order for the first column
            ViewState("SortDirection") = "DESC"
            ViewState("SortExpression") = GridView1.Columns(0).SortExpression
        End If

        ViewState("SortExpression") = sortExpression
        Dim sortDirection As String = If(ViewState("SortDirection").ToString() = "ASC", " ASC", " DESC")
        sortExpression += sortDirection

        dataView.Sort = sortExpression

        GridView1.DataSource = dataView
        GridView1.DataBind()
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
                        e.Row.Cells(i).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "No of store", e.Row.Cells(i).Text, CInt(e.Row.Cells(i).Text))
                    End If

                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Amount" Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6f2ff !important")
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
        '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
        ViewState("SortExpression") = String.Empty
        BuildContenctPage(CDate(GetEndOfMonthDate(DDL_ReportMonth.SelectedValue)).ToString("yyyy-MM-dd"), DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)
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


        '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
        ViewState("SortExpression") = String.Empty
        BuildContenctPage(CDate(GetEndOfMonthDate(DDL_ReportMonth.SelectedValue)).ToString("yyyy-MM-dd"), DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)
    End Sub

    Protected Sub DDL_Account_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Account_Type.SelectedIndexChanged
        '' Reset sortExpression whenever the ReportMonth dropdownlist selectedindex is changed
        ViewState("SortExpression") = String.Empty
        BuildContenctPage(CDate(GetEndOfMonthDate(DDL_ReportMonth.SelectedValue)).ToString("yyyy-MM-dd"), DDL_Country.SelectedValue, DDL_Account_Type.SelectedValue)
    End Sub





    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Dim Page_Origin As String = Get_Value("SELECT TOP 1 Page_Origin FROM DMC_Account_Reports_List WHERE ID = " & Request.QueryString("ID"), "Page_Origin")
        Response.Redirect(Page_Origin)
    End Sub



End Class
