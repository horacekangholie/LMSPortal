
Partial Class Views_DMC_Account_Cross_Assigned
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "R&D DMC User Account Cross-assigned Overview"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
        End If
    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {"SELECT A.username AS [DMC User Account]
                                            , CAST(B.Headquarter_ID AS int) AS [Headquarter of Origin]
	                                        , CASE WHEN CAST(A.hqid AS int) = CAST(B.Headquarter_ID AS int) THEN NULL ELSE A.hqid END AS [Cross Assigned To Headquarter]
	                                        , H.Name AS [Headquarter Name]  
                                       FROM L_dmcuser A  
                                       INNER JOIN DMC_User B ON B.Username = A.username  
                                       INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = A.hqid  
                                       WHERE CAST(B.Headquarter_ID AS int) IN ('21')
                                       ORDER BY A.username "}

            BuildGridView(GridView1, "GridView1", "DMC User Account")
            GridView1.DataSource = GetDataTable(sqlStr(0))
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
                Dim ColData() As String = {"DMC User Account", "Headquarter of Origin", "Cross Assigned To Headquarter", "Headquarter Name"}
                Dim ColSize() As Integer = {50, 100, 50, 100}
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
        End Select
    End Sub


    '' Gridview controls
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
