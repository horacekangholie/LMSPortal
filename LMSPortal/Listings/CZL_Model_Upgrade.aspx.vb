
Partial Class Listings_CZL_Model_Upgrade
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Recover Invoice for CZL Account Model Upgrade"

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
            Dim sqlStr = "SELECT * FROM I_CZL_Account_Model_Upgrade_Charge " &
                         "WHERE [Account Name] LIKE '%" & keyword & "%' OR [Distributor] LIKE '%" & keyword & "%' OR [PO No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%' " &
                         "ORDER BY ID DESC "

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
        GridView1.DataKeyNames = New String() {"UID"}
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

        Dim ColData() As String = {"Distributor", "CZL Client ID", "Account Name", "Upgraded Model", "Upgrade Date", "PO No", "PO Date", "Invoice No", "Invoice Date", "Chargeable", "Currency", "Fee", "Requested By"}
        Dim ColName() As String = {"Distributor", "Account ID", "Account Name", "Upgraded Model", "Upgrade Date", "PO No", "PO Date", "Invoice No", "Invoice Date", "Chargeable", "Currency", "Fee", "Requested By"}
        Dim ColSize() As Integer = {100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100}

        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColName(i), "_", " ")
            Bfield.HeaderStyle.Width = ColSize(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            If Bfield.HeaderText.Contains("Fee") Then
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
            'Dim UID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO Date")).Text

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            Dim Invoice_No As String = IIf(IsDBNull(drv("Invoice No")), "", drv("Invoice No"))
            If Invoice_No <> "" And Invoice_No <> "NA" Then
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
            EditLinkButton.CommandArgument = drv("CZL Account ID") & "|" & drv("UID")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_RecoverCZLModelUpgradeFee_Click

            '' Lock record if invoice has been recovered
            If Invoice_No = "" Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.Enabled = True
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


    Protected Sub DDL_Chargeable_Load(sender As Object, e As EventArgs) Handles DDL_Chargeable.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Value_2, Value_1 FROM DB_Lookup WHERE Lookup_Name = 'YesNo' "
                DDL_Chargeable.DataSource = GetDataTable(sqlStr)
                DDL_Chargeable.DataTextField = "Value_1"
                DDL_Chargeable.DataValueField = "Value_2"
                DDL_Chargeable.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Chargeable_DataBound(sender As Object, e As EventArgs) Handles DDL_Chargeable.DataBound
        Dim DDL_Chargeable As DropDownList = pnlRecoverCZLModelUpgradeFee.FindControl("DDL_Chargeable")
        Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
        DDL_Chargeable.SelectedIndex = i
    End Sub

    Protected Sub DDL_Currency_Load(sender As Object, e As EventArgs) Handles DDL_Currency.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "
                DDL_Currency.DataSource = GetDataTable(sqlStr)
                DDL_Currency.DataTextField = "Currency"
                DDL_Currency.DataValueField = "Currency"
                DDL_Currency.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Currency_DataBound(sender As Object, e As EventArgs) Handles DDL_Currency.DataBound
        Dim DDL_Currency As DropDownList = pnlRecoverCZLModelUpgradeFee.FindControl("DDL_Currency")
        DDL_Currency.SelectedValue = DDL_Currency.Items.FindByText("SGD").Value  '' Default as SGD
    End Sub





    '' control button events
    Protected Sub Edit_RecoverCZLModelUpgradeFee_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderRecoverCZLModelUpgradeFee.Text = "Recover Invoice"
        btnUpdateRecoverCZLModelUpgradeFee.Text = "Update"
        btnCancelRecoverCZLModelUpgradeFee.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")  '' pass eCmmandArgument to array
        TB_CZL_Account_Unique_ID.Text = EditLinkButtonCommandArgument(0)
        TB_UID.Text = EditLinkButtonCommandArgument(1)

        TB_PO_No.Text = String.Empty
        TB_PO_Date.Text = String.Empty
        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty
        TB_Fee.Text = String.Empty
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True
        RequiredField_TB_Fee.Enabled = True

        popupRecoverCZLModelUpgradeFee.Show()

    End Sub

    Protected Sub Update_RecoverCZLModelUpgradeFee_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateRecoverCZLModelUpgradeFee.Click
        Dim CZL_Account_Unique_ID As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_CZL_Account_Unique_ID")
        Dim UID As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_UID")
        Dim PO_No As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_PO_No")
        Dim PO_Date As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_PO_Date")
        Dim Invoice_No As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_Invoice_Date")
        Dim Chargeable As DropDownList = pnlRecoverCZLModelUpgradeFee.FindControl("DDL_Chargeable")
        Dim Currency As DropDownList = pnlRecoverCZLModelUpgradeFee.FindControl("DDL_Currency")
        Dim Fee As TextBox = pnlRecoverCZLModelUpgradeFee.FindControl("TB_Fee")

        Try
            Dim sqlStr As String = "UPDATE CZL_Account_Model_Upgrade_Charge SET PO_No = N'" & PO_No.Text & "', PO_Date = N'" & PO_Date.Text & "', Invoice_No = '" & Invoice_No.Text & "', Invoice_Date = '" & Invoice_Date.Text & "', Chargeable = N'" & Chargeable.SelectedValue & "', Currency = N'" & Currency.SelectedValue & "', Fee = N'" & Fee.Text & "' " &
                                   "WHERE UID = '" & UID.Text & "' "

            '' insert the record into DB_Recovered_Invoice table
            Dim sqlStr1 As String = "EXEC SP_Insert_CZL_Model_Update_Charge_Recovered_Invoice_Items N'" & CZL_Account_Unique_ID.Text & "', N'" & UID.Text & "' "

            RunSQL(sqlStr)
            RunSQL(sqlStr1)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_RecoverCZLModelUpgradeFee_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelRecoverCZLModelUpgradeFee.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub



    '' Search button
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
