
Partial Class Form_CZL_Account
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "AI Account Listings"
    Dim sqlStr As String = "SELECT * FROM R_CZL_Account"
    Dim reportCol As String() = {"CZL Account ID", "CZL Client ID", "User Group", "Model", "Created Date", "By Distributor", "Country", "Gen Version", "No of Registered Device"}
    Dim reportColName As String() = {"CZL Account ID", "Account ID", "Account Name", "Account Model", "Created Date", "By Distributor / Owner", "Country", "Gen Version", "No of Registered Device"}
    Dim reportColSize As Integer() = {100, 100, 300, 100, 100, 250, 150, 100, 200}
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
            sqlStr += " WHERE [CZL Client ID] LIKE '%" & keyword & "%' OR [User Group] LIKE '%" & keyword & "%' OR ([By Distributor] LIKE '%" & keyword & "%' AND [By Distributor] != '') OR [Gen Version] LIKE '%" & keyword & "%' OR [Country] LIKE '%" & keyword & "%' "
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

        '' Add templatefield for Edit icon
        'Dim TField As TemplateField = New TemplateField()
        'TField.HeaderStyle.Width = Unit.Percentage(2)
        'TField.ItemStyle.Wrap = False
        'TField.ItemTemplate = New GridViewItemTemplateControl()
        'GridView1.Columns.Add(TField)


        '' Hide the 2 column after gridview is created
        GridView1.Columns(0).Visible = False                              '' Hide Account Unique ID column
        GridView1.Columns(GridView1.Columns.Count - 1).Visible = False    '' Hide No of Registered Device column

    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.ToolTip = "Click to view record"
            e.Row.Attributes.Add("OnClick", String.Format("window.location='/Form/CZL_Account_Form.aspx?Account_Unique_ID={0}'", drv("CZL Account ID")))

            ' Define navigate link to Customer details page
            'Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Click on edit icon to view edit page
            'Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The column where the edit and delete button are
            'Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            'EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            'EditLinkButton.CssClass = "btn btn-xs btn-info"
            'EditLinkButton.PostBackUrl = "~/Form/CZL_Account_Form.aspx?Account_Unique_ID=" & drv("CZL Account ID")
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


    '' Button create new account
    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Response.Redirect(String.Format("/Form/CZL_Account_Form.aspx?Account_Unique_ID={0}", ""))
    End Sub

End Class
