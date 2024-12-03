
Partial Class Reports_New_DMC_Account
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "New / Renewed DMC Account"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
        End If

        '' Hide the section when there is no record
        DMCSubscriptionSection.Visible = IIf(GridView1.Rows.Count > 0, True, False)
        StoreFTPInfoSection.Visible = IIf(GridView2.Rows.Count > 0, True, False)
        DMCUserAccountSection.Visible = IIf(GridView3.Rows.Count > 0, True, False)

    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {"SELECT TOP 7 * FROM D_DMC_Subscription_Outstanding_Invoice ORDER BY [Subscription ID] DESC ",
                                      "SELECT * FROM R_DMC_Store_Licence WHERE DATEDIFF(d, CAST([Created Date] AS date), CAST(GETDATE() AS date)) <= 7 ORDER BY [Created Date] DESC, ID ",
                                      "SELECT * FROM R_DMC_User_Licence WHERE DATEDIFF(d, CAST([Created Date] AS date), CAST(GETDATE() AS date)) <= 7 ORDER BY [Created Date] DESC, ID "}

            BuildGridView(GridView1, "GridView1", "Subscription ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Store ID")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Username")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

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
        GridViewObj.ShowFooter = False
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
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Subscription ID", "Invoice Bill To", "Headquarter ID", "Headquarter Name", "Currency", "Total Amount", "Status", "Invoice No"}
                Dim ColSize() As Integer = {50, 200, 50, 100, 50, 50, 50, 50}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New SubscriptionNestedGridViewItemTemplate()
                Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                GridView1.Columns.Add(Expandfield)

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Amount") Then
                        Bfield.DataFormatString = "{0:#,##0.00}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView2"
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Headquarter ID", "Store No", "Store Name", "Banner", "Public IP", "FTP Host", "FTP User", "FTP Password", "Status"}
                Dim ColSize() As Integer = {50, 50, 200, 200, 100, 100, 100, 100, 100}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView3"
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Username", "Password", "Email", "Created Date", "Status", "Device Type"}
                Dim ColSize() As Integer = {100, 100, 200, 100, 100, 100}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

        End Select
    End Sub


    '' Gridview controls
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim Subscription_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim Subscription As GridView = TryCast(e.Row.FindControl("gvContractStore"), GridView)
            Dim query As String = " SELECT * FROM R_DMC_Subscription_Detail WHERE Subscription_ID ='" & Subscription_ID & "' ORDER BY Store_No"

            Try
                Subscription.DataSource = GetDataTable(query)
                Subscription.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Format the ftp user and password font type
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP User")).CssClass = "ftpFont"
            e.Row.Cells(GetColumnIndexByName(e.Row, "FTP Password")).CssClass = "ftpFont"
        End If
    End Sub

    Private Sub GridView2_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView2.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim Customer_Country As String = Get_Value("SELECT Country FROM Master_Customer WHERE Customer_ID = (SELECT TOP 1 Customer_ID FROM R_DMC_User_Licence WHERE [Headquarter ID] = (SELECT TOP 1 [Headquarter ID] FROM I_DMC_Subscription ORDER BY [Subscription ID] DESC) AND CAST([Created Date] AS date) = CAST(GETDATE() AS date) ORDER BY Created_Date)", "Country")
            If Not Customer_Country.Contains("Singapore") Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).Text = "(hidden)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Password")).Style.Add("color", "#999999")
                UserAccountNotes.Text = "<b>Notes: </b>Please obtain login credentials from customer when setting up store."
            End If
        End If
    End Sub

    Private Sub GridView3_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView3.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

End Class
