
Partial Class Form_CZL_Model_Update
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "CZL Account Model Update"
    Dim sqlStr As String = "SELECT * FROM R_CZL_Account"
    Dim reportCol As String() = {"CZL Account ID", "CZL Client ID", "User Group", "Created Date", "Model", "Grace Period Model Update", "After Grace Period Model Update", "By Distributor", "Country", "Gen Version"}
    Dim reportColName As String() = {"CZL Account ID", "Account ID", "Account Name", "Created Date", "Current Model", "Grace Period Model Update", "After Grace Period Model Update", "By Distributor / Owner", "Country", "Gen Version"}
    Dim reportColSize As Integer() = {100, 100, 300, 100, 100, 100, 100, 300, 150, 100}
    Dim orderByCol As String = "[CZL Account ID]"

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
            sqlStr += " WHERE [CZL Client ID] LIKE '%" & keyword & "%' OR [User Group] LIKE '%" & keyword & "%' OR ([By Distributor] LIKE '%" & keyword & "%' AND [By Distributor] != '') OR [Gen Version] LIKE '%" & keyword & "%' OR [Country] LIKE '%" & keyword & "%' OR [CZL Account ID] IN (SELECT CZL_Account_Unique_ID FROM CZL_Licenced_Devices WHERE Device_ID LIKE '%" & keyword & "%') "
            sqlStr += " ORDER BY " & orderByCol & " DESC "

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
            Bfield.HeaderText = reportColName(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
                Bfield.ItemStyle.Wrap = False
            End If
            Bfield.HeaderStyle.Width = reportColSize(i)
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next


        '' Hide column after gridview is created
        GridView1.Columns(0).Visible = False                              '' Hide Account Unique ID column

    End Sub


    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            '' Add additional header row
            Dim HeaderGrid As GridView = DirectCast(sender, GridView)
            Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)

            For i = 0 To e.Row.Cells.Count - 1
                Dim HeaderCell = New TableHeaderCell()
                Select Case i
                    Case 0
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.ColumnSpan = 4
                    Case 4
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "Model Update"
                        HeaderCell.ColumnSpan = 2
                        HeaderCell.Style.Add("text-align", "center")
                    Case 6
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.ColumnSpan = 3
                End Select
            Next
            HeaderGrid.Controls(0).Controls.AddAt(0, HeaderGridRow)

            e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Style.Add("text-align", "center")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Grace Period Model Update")).Text = "Grace Period<br><i style='color:grey'>(Max 4x)</i>"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Grace Period Model Update")).Style.Add("text-align", "center")
            e.Row.Cells(GetColumnIndexByName(e.Row, "After Grace Period Model Update")).Text = "After Grace Period<br><i style='color:grey'>(Chargeable)</i>"
            e.Row.Cells(GetColumnIndexByName(e.Row, "After Grace Period Model Update")).Style.Add("text-align", "center")

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.ToolTip = "Click to view record"
            e.Row.Attributes.Add("OnClick", String.Format("window.location='/Form/CZL_Model_Update_Form.aspx?Account_Unique_ID={0}'", drv("CZL Account ID")))

            '' Highlight the row if model update more than 4 times
            'For i = 0 To e.Row.Cells.Count - 1
            '    If CInt(drv("Grace Period Model Update")) > 4 Then
            '        e.Row.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#fdf5f4")
            '    End If
            'Next

            '' Highlight the cell if model update more than 4 times
            If CInt(drv("Grace Period Model Update")) > 4 Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Grace Period Model Update")).Style.Add("background-color", "#fbeee6")
            End If

            'e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Style.Add("background-color", " #fef9e7 ")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Style.Add("text-align", "center")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Grace Period Model Update")).Style.Add("text-align", "center")
            e.Row.Cells(GetColumnIndexByName(e.Row, "After Grace Period Model Update")).Style.Add("text-align", "center")

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

End Class
