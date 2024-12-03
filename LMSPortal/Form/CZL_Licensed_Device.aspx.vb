Imports System.Data

Partial Class Form_CZL_Licensed_Device
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "CZL Licenced Device by Account"
    Dim sqlStr As String = "SELECT * FROM R_CZL_Account"
    Dim reportCol As String() = {"CZL Account ID", "CZL Client ID", "User Group", "Model", "Created Date", "By Distributor", "Country", "Gen Version", "No of Registered Device"}
    Dim reportColName As String() = {"CZL Account ID", "Account ID", "Account Name", "Account Model", "Created Date", "By Distributor / Owner", "Country", "Gen Version", "No of Registered Device"}
    Dim reportColSize As Integer() = {100, 100, 300, 100, 100, 250, 150, 100, 200}
    Dim orderByCol As String = "[CZL Account ID]"

    Dim currentSortedColumnIndex As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
            chk_empty_account.Checked = False   '' uncheck checkbox when first loaded
        End If

        LB_No_Of_Registered_Account.Text = Get_Value("SELECT COUNT(*) AS NoOfAccount FROM R_CZL_Account", "NoOfAccount")
        LB_No_Of_Registered_Devices.Text = Get_Value("SELECT SUM([No of Registered Device]) AS Device_Count FROM R_CZL_Account", "Device_Count")
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "", Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            If chk_empty_account.Checked Then
                sqlStr += " WHERE [No of Registered Device] < 1 "
            Else
                sqlStr += " WHERE [CZL Client ID] LIKE '%" & keyword & "%' OR [User Group] LIKE '%" & keyword & "%' OR ([By Distributor] LIKE '%" & keyword & "%' AND [By Distributor] != '') OR [Gen Version] LIKE '%" & keyword & "%' OR [Country] LIKE '%" & keyword & "%' OR [CZL Account ID] IN (SELECT CZL_Account_Unique_ID FROM CZL_Licenced_Devices WHERE Device_ID LIKE '%" & keyword & "%' OR Scale_SN LIKE '%" & keyword & "%' OR REPLACE(MAC_Addr, ':', '') LIKE '" & Replace(keyword, ":", "") & "' OR REPLACE(MAC_Addr, ':', '') LIKE '" & Replace(keyword, "-", "") & "') "
            End If
            sqlStr += " ORDER BY " & orderByCol & " DESC "

            '' Formatting GridView
            BuildGridView()

            Dim dt As DataTable = GetDataTable(sqlStr)
            Dim dataView As New DataView(dt)
            dataView.Sort = currentSortedExpressionDirection

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = dataView
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
        GridView1.AllowSorting = True
        GridView1.PageSize = 15
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"CZL Account ID"}
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
            Bfield.DataField = reportCol(i)
            Bfield.SortExpression = reportCol(i)
            Bfield.HeaderText = reportColName(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
                Bfield.ItemStyle.Wrap = False
            End If
            Bfield.HeaderStyle.Width = reportColSize(i)
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next

        '' Add templatefield for Edit icon
        'Dim TField As TemplateField = New TemplateField()
        'TField.HeaderStyle.Width = Unit.Percentage(2)
        'TField.ItemStyle.Wrap = False
        'TField.ItemTemplate = New GridViewItemTemplateControl()
        'GridView1.Columns.Add(TField)


        '' Hide the 2 column after gridview is created
        GridView1.Columns(0).Visible = False                              '' Hide Account Unique ID column
        'GridView1.Columns(GridView1.Columns.Count - 2).Visible = False    '' Hide No of Registered Device column

    End Sub



    ' Gridview events
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text, IIf(ViewState("SortExpression") <> "", ViewState("SortExpression") & " " & ViewState("SortDirection"), GetDataFieldNameByHeaderText(GridView1, "Account ID") & " DESC"))
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.ToolTip = "Click to view record"
            e.Row.Attributes.Add("OnClick", String.Format("window.location='/Form/CZL_Licensed_Device_Form.aspx?Account_Unique_ID={0}'", drv("CZL Account ID")))

            '' Highlight the row in red to mark account which has no registered device
            For i = 0 To e.Row.Cells.Count - 1
                If CInt(drv("No of Registered Device")) <= 0 Then
                    e.Row.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#fdf5f4")
                End If
            Next

            '' Change the color for sorted column
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).Style.Add("background-color", "#ffffe6")
            Next
        ElseIf e.Row.RowType = DataControlRowType.Footer Then

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Private Sub GridView1_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs) Handles GridView1.Sorting
        Dim sortExpression As String = e.SortExpression
        If ViewState("SortDirection") IsNot Nothing AndAlso ViewState("SortExpression") IsNot Nothing Then
            Dim previousSortExpression As String = ViewState("SortExpression").ToString()
            Dim previousSortDirection As String = ViewState("SortDirection").ToString()
            ViewState("SortDirection") = If(previousSortExpression = sortExpression, If(previousSortDirection = "DESC", "ASC", "DESC"), "ASC")
        Else
            ViewState("SortDirection") = "ASC"
            ViewState("SortExpression") = GridView1.Columns(1).SortExpression
        End If

        ViewState("SortExpression") = sortExpression
        Dim sortDirection As String = If(ViewState("SortDirection").ToString() = "ASC", " ASC", " DESC")
        sortExpression += sortDirection

        '' populate gridview
        BuildPageContent(TB_Search.Text, sortExpression)
    End Sub

    Private Sub CustomizeSortedHeaderRow(ByVal gridView As GridView, ByVal headerRow As GridViewRow)
        Dim sortExpression As String = ViewState("SortExpression")?.ToString()
        Dim sortDirection As String = ViewState("SortDirection")?.ToString()

        ' If sortExpression is empty, set it to the first column's SortExpression
        If String.IsNullOrEmpty(sortExpression) Then
            sortExpression = gridView.Columns(1).SortExpression
            sortDirection = "DESC"
            currentSortedColumnIndex = 1
        End If

        ' Loop through the headerrow control field to find which is the current selected column
        For Each field As DataControlField In gridView.Columns
            If field.SortExpression = sortExpression Then
                Dim cellIndex As Integer = gridView.Columns.IndexOf(field)
                Dim sortArrow As New Label()
                sortArrow.CssClass = "sort-arrow " & If(sortDirection = "DESC", "desc", "asc")

                ' Add the sorting arrow inside a <span> element
                Dim span As New HtmlGenericControl("span")
                span.Controls.Add(sortArrow)

                ' Append the <span> to the header cell
                headerRow.Cells(cellIndex).Controls.Add(span)

                ' Get current sorted column index
                currentSortedColumnIndex = cellIndex
            End If
        Next
    End Sub



    ' Search button

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        chk_empty_account.Checked = False
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub chk_empty_account_CheckedChanged(sender As Object, e As EventArgs)
        TB_Search.Text = Nothing
        BuildPageContent(TB_Search.Text)
    End Sub

End Class
