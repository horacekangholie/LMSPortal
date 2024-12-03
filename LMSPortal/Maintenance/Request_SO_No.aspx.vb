
Partial Class Maintenance_Request_SO_No
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Recover Order SO No"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
        End If
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            'If chk_empty_account.Checked Then
            Dim sqlStr As String = "SELECT A.[Customer ID], A.[Name], A.[Requestor ID], A.[Requested By], B.[Category], A.[PO No], A.[PO Date], A.[SO No] " &
                                   "FROM I_DB_SO_No_By_PO A " &
                                   "INNER JOIN _PO_No_Ref_Invoice_For_All_Type_Of_Request B ON B.PO_No = A.[PO No] " &
                                   "WHERE LEN(B.Invoice_No) <= 0 AND (A.Name LIKE '%" & keyword & "%' OR A.[Requested By] LIKE '%" & keyword & "%' OR A.[PO No] LIKE '%" & keyword & "%') " & IIf(chk_empty_so_no.Checked, "AND [SO No] IS NULL ", "") &
                                   "ORDER BY [PO Date] DESC"

            ''Response.Write(sqlStr)
            BuildGridView()
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

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
        GridView1.DataKeyNames = New String() {"Customer ID", "Category", "PO No"}
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

    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub


    '' Gridview row commands events
    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim Customer_ID As String = GridView1.DataKeys(e.RowIndex).Values(0)
        Dim Category As String = GridView1.DataKeys(e.RowIndex).Values(1)
        Dim Old_PO_No As String = GridView1.DataKeys(e.RowIndex).Values(2)
        Dim New_PO_No As String = Trim((TryCast(row.FindControl("TB_E_PO_No"), TextBox)).Text)
        Dim SO_No As String = Trim((TryCast(row.FindControl("TB_E_SO_No"), TextBox)).Text)

        Try
            Dim sqlStr As String = "UPDATE DB_SO_No_By_PO SET SO_No = '" & SO_No & "' WHERE Customer_ID = '" & Customer_ID & "' AND PO_No = '" & Old_PO_No & "' "
            RunSQL(sqlStr)

            '' If PO No is changed, then update the PO No in table accordingly
            If Trim(New_PO_No) <> Trim(Old_PO_No) Then
                Dim sqlStr1 As String = "EXEC SP_Change_Order_PO_No '" & Customer_ID & "', '" & Category & "', '" & Old_PO_No & "', '" & New_PO_No & "' "
                RunSQL(sqlStr1)
            End If

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles GridView1.RowDeleting
        '' code here
    End Sub

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Checkbox for those SO No not recovered order
    Protected Sub chk_empty_so_no_CheckedChanged(sender As Object, e As EventArgs)
        TB_Search.Text = Nothing
        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Search button
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
