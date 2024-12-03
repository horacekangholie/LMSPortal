
Partial Class Reports_DMC_Account_Usage
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Account Usage Overview"
    Dim sqlStr As String = "SELECT * FROM _AccountUsage_Overview ORDER BY CASE [Group] WHEN 'Singapore Outlets' THEN 1 WHEN 'Overseas Customers'THEN 2 WHEN 'Distributors' THEN 3 ELSE 4 END, Account_Type "
    Dim reportColName As String() = {"Group", "Account_Type", "Active_Within_30_Days", "Active_Within_180_Days", "Active_Within_365_Days", "Last_Active_More_Than_365_Days", "Account_No_Activity"}
    Dim reportColSize As Integer() = {250, 150, 200, 200, 200, 200, 200, 200}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If
    End Sub

    Protected Sub BuildPageContent()
        Try
            '' Formatting GridView
            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView()
        '' GridView Properties
        GridView1.AutoGenerateColumns = False
        GridView1.AllowPaging = False
        GridView1.PageSize = 15
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"Group", "Account_Type"}
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
            If i > 1 Then
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderText = Replace(reportColName(i), "_", " ")
                TField.HeaderStyle.Width = reportColSize(i)
                TField.HeaderStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateHyperlinkControl()
                GridView1.Columns.Add(TField)
            Else
                Dim Bfield As BoundField = New BoundField()
                Bfield.DataField = reportColName(i)
                Bfield.HeaderText = Replace(reportColName(i), "_", " ")
                Bfield.HeaderStyle.Width = reportColSize(i)
                Bfield.ItemStyle.Wrap = False
                GridView1.Columns.Add(Bfield)
            End If
        Next
    End Sub


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            For i = 0 To e.Row.Cells.Count - 1
                If i > 1 Then
                    Dim GroupName As String = Replace(DataBinder.Eval(e.Row.DataItem, "Group"), " ", "_")
                    Dim AccountType As String = Replace(DataBinder.Eval(e.Row.DataItem, "Account_Type"), " ", "_")

                    Dim HyperLinkValue As HyperLink = TryCast(e.Row.Cells(i).Controls(0), HyperLink)
                    HyperLinkValue.Text = IIf(IsDBNull(drv(reportColName(i))), "", drv(reportColName(i)))
                    HyperLinkValue.Attributes.Add("Onclick", "window.open('/Reports/SubReport.aspx?Group=" & GroupName & "&Account_Type=" & AccountType & "&ColNo=" & i - 1 & "', '_blank', 'titlebar=no', 'status=no', 'directories=no', 'width=800px, height=600px');")
                    HyperLinkValue.Target = "_blank"
                    HyperLinkValue.Style.Add("cursor", "pointer")
                    HyperLinkValue.Style.Add("color", "blue")
                End If
            Next
        End If

        '' format value columns
        For i = 0 To e.Row.Cells.Count - 1
            If i > 1 Then
                e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(i).Style.Add("text-align", "right !important")
                e.Row.Cells(i).Style.Add("padding-right", "20px")
            End If
        Next
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub


End Class
