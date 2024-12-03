Imports System.Data

Partial Class Adhoc_FTP_Accounts
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "FTP Account Users"
    Dim sqlStr As String = "SELECT Code, Distributor, Country FROM FTP_Server_Distributor"
    Dim reportColName As String() = {"Code", "Distributor", "Country"}
    Dim reportColSize As Integer() = {0, 500, 300}
    Dim orderByCol As String = "[Code]"

    Dim currentSortedColumnIndex As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If

        '' Hide create new FTP account when not administrator login
        BT_Create.Visible = IIf(Session("Login_Name") <> "administrator", False, True)
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "", Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            sqlStr += " WHERE [Code] LIKE '%" & keyword & "%' OR [Distributor] LIKE '%" & keyword & "%' OR [Country] LIKE '%" & keyword & "%' OR Code IN (SELECT Code FROM R_FTP_Server_Distributor_Account WHERE [User ID] = '" & keyword & "' OR [Contact Person] LIKE '%" & keyword & "%') OR (COY_ABBR LIKE '%" & keyword & "%' AND Len(ISNULL(COY_ABBR, '')) > 0 ) "
            sqlStr += " ORDER BY " & orderByCol

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
        GridView1.DataKeyNames = New String() {"Code"}
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
            Bfield.SortExpression = reportColName(i)    '' Enable SortExpression
            Bfield.HeaderText = reportColName(i)
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

    End Sub



    '' Gridview events 
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text, IIf(ViewState("SortExpression") <> "", ViewState("SortExpression") & " " & ViewState("SortDirection"), GetDataFieldNameByHeaderText(GridView1, "Code") & " ASC"))
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.ToolTip = "Click to view record"
            e.Row.Attributes.Add("OnClick", String.Format("window.location='/Adhoc/FTP_Accounts_Form.aspx?Code={0}'", drv("Code")))

            '' Click on edit icon to view edit page
            'Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The column where the edit and delete button are
            'Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            'EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            'EditLinkButton.CssClass = "btn btn-xs btn-info"
            'EditLinkButton.PostBackUrl = "~/Adhoc/FTP_Accounts_Form.aspx?Code=" & drv("Code")

            '' Change the color for sorted column
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).Style.Add("background-color", "#ffffe6")
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

    Private Sub GridView1_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs) Handles GridView1.Sorting
        Dim sortExpression As String = e.SortExpression
        If ViewState("SortDirection") IsNot Nothing AndAlso ViewState("SortExpression") IsNot Nothing Then
            Dim previousSortExpression As String = ViewState("SortExpression").ToString()
            Dim previousSortDirection As String = ViewState("SortDirection").ToString()

            ViewState("SortDirection") = If(previousSortExpression = sortExpression, If(previousSortDirection = "ASC", "DESC", "ASC"), "DESC")
        Else
            ViewState("SortDirection") = "DESC"
            ViewState("SortExpression") = GridView1.Columns(0).SortExpression
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
            sortExpression = gridView.Columns(0).SortExpression
            sortDirection = "ASC"
            currentSortedColumnIndex = 0
        End If

        ' Loop through the headerrow control field to find which is the current selected column
        For Each field As DataControlField In gridView.Columns
            If field.SortExpression = sortExpression Then
                Dim cellIndex As Integer = gridView.Columns.IndexOf(field)
                Dim sortArrow As New Label()
                sortArrow.CssClass = "sort-arrow " & If(sortDirection = "ASC", "asc", "desc")

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




    '' Search box
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Response.Redirect(String.Format("/Adhoc/FTP_Accounts_Form.aspx?Code={0}", ""))
    End Sub

End Class
