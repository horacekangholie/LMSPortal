
Partial Class Maintenance_Lookup_Table
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Lookup Tables"
    Dim sqlStr As String = "SELECT ROW_NUMBER() OVER(ORDER BY Lookup_Name ASC) AS No, Lookup_Name AS Lookup_Table FROM DB_Lookup "
    Dim reportColName As String() = {"No", "Lookup_Table"}
    Dim reportColSize As Integer() = {0, 1000}
    Dim orderByCol As String = "[No]"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If

        UpdatePanel1.Visible = True   '' initialize update panel 1 as visible
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            sqlStr += " WHERE Lookup_Name LIKE '%" & keyword & "%' "
            sqlStr += " GROUP BY Lookup_Name "
            sqlStr += " ORDER BY " & orderByCol

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
        GridView1.DataKeyNames = New String() {"No"}
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
            Bfield.HeaderText = Replace(reportColName(i), "_", " ")
            Bfield.HeaderStyle.Width = reportColSize(i)
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next

    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            e.Row.ToolTip = "Click to view record"
            e.Row.Attributes.Add("OnClick", String.Format("window.location='/Maintenance/Lookup_Details.aspx?Lookup_Name={0}'", drv("Lookup_Table")))
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub BT_Create_Click(sender As Object, e As EventArgs) Handles BT_Create.Click
        ModalHeaderLookupName.Text = "Add Lookup Name"
        btnSaveLookupName.Text = "Save"
        btnCancelLookupName.Text = "Cancel"
        TB_LookupName.Text = String.Empty
        popupLookupName.Show()
    End Sub

    Protected Sub btnSaveLookupName_Click(sender As Object, e As EventArgs) Handles btnSaveLookupName.Click
        Dim LookupName As TextBox = pnlAddLookupName.FindControl("TB_LookupName")
        Dim RecordExist As Boolean = IIf(Get_Value("SELECT COUNT(*) AS NoOfRow FROM DB_Lookup WHERE Lookup_Name IN ('" & LookupName.Text & "')", "NoOfRow") > 0, True, False)
        If Not RecordExist Then
            Try
                Dim sqlStr As String = "INSERT INTO DB_Lookup(Lookup_Name, Value_1) VALUES('" & LookupName.Text & "', '(New Value)') "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            UpdatePanel1.Visible = False
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "msgBox1", "<script>alert('Lookup name exist.');</script>", False)
        End If
        BuildPageContent(LookupName.Text)  '' set newly created lookupname
    End Sub

    Protected Sub btnCancelLookupName_Click(sender As Object, e As EventArgs) Handles btnCancelLookupName.Click
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub





End Class
