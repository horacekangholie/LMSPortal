
Partial Class Maintenance_AssignBillEntity
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Re-assign Bill Entity"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        PopulateGridViewData()
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT * FROM (" &
                                   "                SELECT A.Subscription_ID, B.Customer AS Company " &
                                   "                     , (SELECT Name FROM Master_Customer WHERE Customer_ID = A.Arranged_Bill_Entity) AS Invoice_Bill_To, B.[Invoice No] " &
                                   "                FROM DB_Bill_Entity_Special_Arranged A " &
                                   "                INNER JOIN I_DMC_Subscription B On B.[Subscription ID] = A.Subscription_ID " &
                                   "              ) TBL "
            sqlStr += " WHERE Company LIKE '%" & keyword & "%' OR Invoice_Bill_To LIKE '%" & keyword & "%' "
            sqlStr += " ORDER BY Subscription_ID DESC "

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
        GridView1.DataKeyNames = New String() {"Subscription_ID"}
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

        GridView1.Columns.Clear()  '' Clear each time page index changed otherwise gridview will keep populating column

        Dim ColData() As String = {"Subscription_ID", "Company", "Invoice_Bill_To"}
        Dim ColSize() As Integer = {100, 300, 300}

        '' Create column boundfield
        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColData(i), "_", " ")
            Bfield.HeaderStyle.Width = ColSize(i)
            Bfield.HeaderStyle.Wrap = False
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next

        '' Add templatefield for Edit icon
        Dim TField As TemplateField = New TemplateField()
        TField.HeaderStyle.Width = Unit.Percentage(2)
        TField.ItemStyle.Wrap = False
        TField.ItemTemplate = New GridViewItemTemplateControl()
        GridView1.Columns.Add(TField)

    End Sub


    Protected Sub AssignBillEntity(ByVal sender As Object, ByVal e As EventArgs) Handles BN_Assign.Click
        Dim Subscription_ID As String = DDL_Subscription_ID.SelectedValue
        Dim Customer_ID As String = DDL_New_Bill_Entity.SelectedValue
        Dim sqlStr As String = "EXEC SP_ReAssignBillEntity '" & Subscription_ID & "', '" & Customer_ID & "'"
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim DeleteLinkButtonIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1

            '' Add delete button
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteLinkButtonIndex), LinkButton)

            '' Disabled button if a subscription's invoice has been recovered
            If Len(drv("Invoice No")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
                DeleteLinkButton.CommandArgument = drv("Subscription_ID")
                DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
                DeleteLinkButton.Enabled = True
            End If
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            DeleteLinkButton.CausesValidation = False
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Click

            '' format the control button
            For i = 0 To e.Row.Cells.Count - 1
                    e.Row.Cells(i).Wrap = False
                    If i = CtrlCellIndex Then
                        e.Row.Cells(CtrlCellIndex).Width = 0
                        e.Row.Cells(CtrlCellIndex).Wrap = False
                        e.Row.Cells(CtrlCellIndex).HorizontalAlign = HorizontalAlign.Center
                    End If
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

    Protected Sub Delete_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim Subscription_ID As String = TryCast(sender, LinkButton).CommandArgument
        Dim sqlStr As String = "DELETE FROM DB_Bill_Entity_Special_Arranged WHERE Subscription_ID = '" & Subscription_ID & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub



    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
