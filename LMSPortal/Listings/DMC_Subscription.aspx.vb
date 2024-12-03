
Partial Class Listings_DMC_Subscription
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Subscription"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr = "SELECT * FROM I_DMC_Subscription "
            sqlStr += "WHERE [Invoice Bill To] LIKE '%" & keyword & "%' OR [Customer] LIKE '%" & keyword & "%' OR [Headquarter ID] LIKE '%" & keyword & "%' OR [Headquarter Name] LIKE '%" & keyword & "%' OR [Subscription ID] LIKE '%" & keyword & "%' "
            sqlStr += "ORDER BY [Subscription ID] DESC "

            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' Get the total record
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
        GridView1.DataKeyNames = New String() {"Subscription ID"}
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

        GridView1.Columns.Clear()

        Dim ColData() As String = {"Subscription ID", "Invoice Bill To", "Headquarter ID", "Headquarter Name", "Invoice No", "Invoice Date", "Currency", "Total Amount", "Status"}
        Dim ColSize() As Integer = {50, 200, 50, 100, 100, 50, 50, 50, 50}

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
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            If Bfield.HeaderText.Contains("Amount") Then
                Bfield.DataFormatString = "{0:#,##0.00}"
            End If
            Bfield.HeaderStyle.Wrap = False
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next

        '' Add template field for the delete button
        Dim TField As TemplateField = New TemplateField()
        TField.HeaderStyle.Width = Unit.Percentage(2)
        TField.ItemStyle.Wrap = False
        TField.ItemTemplate = New GridViewItemTemplateControl()
        GridView1.Columns.Add(TField)

    End Sub


    '' Gridview control
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

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

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"

            If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.Text = drv("Invoice No")
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

            '' Edit Button
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            EditLinkButton.CssClass = "btn btn-xs btn-info"
            EditLinkButton.CommandArgument = drv("Subscription ID")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_Subscription_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            '' Void Button
            Dim VoidLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(1), LinkButton)
            VoidLinkButton.CommandArgument = drv("Subscription ID")
            VoidLinkButton.Attributes.Add("onclick", "javascript:if (confirm('You are about to cancel DMC Subscription " & drv("Subscription ID") & "\nClick OK to proceed.\nOtherwise, click Cancel')){return true;} else {return false;}")
            VoidLinkButton.CausesValidation = False
            AddHandler VoidLinkButton.Click, AddressOf Void_Subscription_Click

            '' Lock record if invoice has been recovered
            If drv("Invoice No") = "" Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.Enabled = True

                VoidLinkButton.Text = "<i class='bi bi-x-circle'></i>"
                VoidLinkButton.CssClass = "btn btn-xs btn-warning"
                VoidLinkButton.Enabled = True
            Else
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Enabled = False
            End If

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub DDL_Status_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Status.SelectedIndexChanged
        Dim Status As DropDownList = pnlUpdateSubscription.FindControl("DDL_Status")
        If Status.Text = "Cancelled" Then
            TB_Invoice_No.Enabled = False
            TB_Invoice_Date.Enabled = False
            RequiredField_TB_Invoice_No.Enabled = False
            RequiredField_TB_Invoice_Date.Enabled = False
        Else
            TB_Invoice_No.Enabled = True
            TB_Invoice_Date.Enabled = True
            RequiredField_TB_Invoice_No.Enabled = True
            RequiredField_TB_Invoice_Date.Enabled = True
        End If
        popupSubscription.Show()
    End Sub


    '' record edit control
    Protected Sub Edit_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderSubscription.Text = "Recover Invoice"
        btnUpdateSubscription.Text = "Update"
        btnCancelSubscription.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_Subscription_ID.Text = EditLinkButton.CommandArgument   '' pass the subscription id from link button command argument

        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty

        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True

        popupSubscription.Show()
    End Sub

    Protected Sub Update_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateSubscription.Click
        Dim Subscription_ID As TextBox = pnlUpdateSubscription.FindControl("TB_Subscription_ID")
        Dim Invoice_No As TextBox = pnlUpdateSubscription.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlUpdateSubscription.FindControl("TB_Invoice_Date")
        Dim Status As DropDownList = pnlUpdateSubscription.FindControl("DDL_Status")

        Try
            Dim sqlStr As String = "UPDATE DMC_Subscription SET Ref_Invoice_No = N'" & Invoice_No.Text &
                                                              "', Invoiced_Date = '" & Invoice_Date.Text &
                                                              "', Payment_Status = '" & Status.Text &
                                                              "' WHERE Subscription_ID = '" & Subscription_ID.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelSubscription.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Void_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim VoidLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim Subscription_ID As String = VoidLinkButton.CommandArgument
        MsgBox(Subscription_ID)
        Try
            Dim sqlStr As String = "EXEC SP_Void_DMC_Subscription '" & Subscription_ID & "' "
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
