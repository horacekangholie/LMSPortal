
Partial Class Listings_CZL_Account_Setup
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Recover Invoice for CZL Account Setup Fee"

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
            Dim sqlStr = "SELECT * FROM I_CZL_Account_Setup_Fee " &
                         "WHERE [Account Name] LIKE '%" & keyword & "%' OR [By Distributor] LIKE '%" & keyword & "%' OR [PO No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%' " &
                         "ORDER BY [CZL Account ID] DESC "

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

        GridView1.Columns.Clear()

        Dim ColData() As String = {"CZL Client ID", "Account Name", "Created Date", "By Distributor", "PO No", "PO Date", "Invoice No", "Invoice Date", "Requested By", "Currency", "Amount"}
        Dim ColName() As String = {"Account ID", "Account Name", "Created Date", "By Distributor / Owner", "PO No", "PO Date", "Invoice No", "Invoice Date", "Requested By", "Currency", "Amount"}
        Dim ColSize() As Integer = {100, 80, 150, 50, 100, 100, 50, 100, 50, 100, 50, 80}

        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColName(i), "_", " ")
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
            Dim CZL_Account_Unique_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO Date")).Text

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
            EditLinkButton.CommandArgument = drv("CZL Account ID")
            EditLinkButton.CommandName = drv("Distributor ID") & "|" & drv("PO No")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_RecoverCZLAccountSetupFee_Click

            '' Lock record if invoice has been recovered
            If drv("Invoice No") = "" Then
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



    '' Modal control - Recover invoice
    Protected Sub Edit_RecoverCZLAccountSetupFee_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderRecoverCZLAccountSetupFee.Text = "Recover Invoice"
        btnUpdateRecoverCZLAccountSetupFee.Text = "Update"
        btnCancelRecoverCZLAccountSetupFee.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_CZL_Account_Unique_ID.Text = EditLinkButton.CommandArgument

        Dim EditCommandNameArray As String() = Split(EditLinkButton.CommandName, "|")
        TB_Distributor_ID.Text = EditCommandNameArray(0)
        TB_Hidden_PO_No.Text = EditCommandNameArray(1)

        TB_SO_No.Text = Get_Value("SELECT ISNULL(SO_No, '') AS SO_No FROM DB_SO_No_By_PO WHERE Customer_ID ='" & TB_Distributor_ID.Text & "' AND PO_No = '" & TB_Hidden_PO_No.Text & "' ", "SO_No")
        TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)
        RequiredField_TB_SO_No.Enabled = IIf(Len(TB_SO_No.Text) <= 0, True, False)

        '' Initialize the field
        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True

        popupRecoverCZLAccountSetupFee.Show()

    End Sub

    Protected Sub Update_RecoverCZLAccountSetupFee_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateRecoverCZLAccountSetupFee.Click
        Dim CZL_Account_Unique_ID As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_CZL_Account_Unique_ID")
        Dim Distributor_ID As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_Distributor_ID")
        Dim PO_No As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_Hidden_PO_No")
        Dim SO_NO As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_SO_No")
        Dim Invoice_No As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlRecoverCZLAccountSetupFee.FindControl("TB_Invoice_Date")

        Try
            Dim sqlStr As String = "UPDATE CZL_Account_Setup_Charge SET Invoice_No = '" & Invoice_No.Text & "', Invoice_Date = '" & Invoice_Date.Text & "' " &
                                   "WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID.Text & "' "

            '' insert the record into DB_Recovered_Invoice table
            Dim sqlStr1 As String = "EXEC SP_Insert_CZL_Account_Setup_Charge_Recovered_Invoice_Items '" & CZL_Account_Unique_ID.Text & "', '" & PO_No.Text & "' "

            Dim sqlStr2 As String = "UPDATE DB_SO_No_By_PO SET SO_No = '" & Trim(SO_NO.Text) & "' WHERE Customer_ID ='" & Distributor_ID.Text & "' AND PO_No = '" & PO_No.Text & "' "

            RunSQL(sqlStr)
            RunSQL(sqlStr1)
            RunSQL(sqlStr2)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_RecoverCZLAccountSetupFee_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelRecoverCZLAccountSetupFee.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub




    '' Modal control - Add new account setup fee record
    Protected Sub DDL_CZL_Client_ID_Load(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group FROM CZL_Account " &
                             "WHERE Client_ID NOT IN (SELECT Client_ID FROM CZL_Account_Setup_Charge) " &
                             "ORDER BY CAST(Client_ID AS int) "

                DDL_CZL_Client_ID.DataSource = GetDataTable(sqlStr)
                DDL_CZL_Client_ID.DataTextField = "Client_ID_User_Group"
                DDL_CZL_Client_ID.DataValueField = "Client_ID"
                DDL_CZL_Client_ID.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Sales_Representative_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = " SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "
                DDL_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_Sales_Representative.DataTextField = "Name"
                DDL_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
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
        Dim DDL_Currency As DropDownList = pnlAddNewCZLAccountSetupFee.FindControl("DDL_Currency")
        DDL_Currency.SelectedValue = DDL_Currency.Items.FindByText("SGD").Value  '' Default as SGD
    End Sub


    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        ModalHeaderAddNewCZLAccountSetupFee.Text = "Add New Account Setup Fee Record"
        btnAddNewCZLAccountSetupFee.Text = "Add"
        btnCancelAddNewCZLAccountSetupFee.Text = "Cancel"

        '' Initialize the fields
        DDL_CZL_Client_ID.SelectedIndex = 0
        DDL_Sales_Representative.SelectedIndex = 0
        TB_PO_No.Text = String.Empty
        TB_PO_Date.Text = String.Empty
        TB_Fee.Text = String.Empty

        popupAddNewCZLAccountSetupFee.Show()
    End Sub

    Protected Sub Add_NewCZLAccountSetupFee_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNewCZLAccountSetupFee.Click
        Dim Result As Integer = Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Account_Setup_Charge WHERE Client_ID = '" & DDL_CZL_Client_ID.SelectedValue & "' ", "NoOfRecord")
        If Result > 0 Then
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Client account " & DDL_CZL_Client_ID.SelectedItem.Text & " was charged setup fee before,\nPlease note that setup fee is one time charge for each client account.');", True)
        Else
            Dim CZL_Account_To_Charge_Setup_Fee As String = Get_Value("SELECT CZL_Account_Unique_ID FROM CZL_Account WHERE Client_ID = '" & DDL_CZL_Client_ID.SelectedValue & "' ", "CZL_Account_Unique_ID")
            Try
                Dim sqlStr As String = "INSERT INTO CZL_Account_Setup_Charge(CZL_Account_Unique_ID, Client_ID, PO_No, PO_Date, Invoice_No, Invoice_Date, Currency, Fee, Sales_Representative_ID) " &
                                       "SELECT '" & CZL_Account_To_Charge_Setup_Fee & "', " & DDL_CZL_Client_ID.SelectedValue & ", '" & TB_PO_No.Text & "', '" & TB_PO_Date.Text & "', '', NULL, '" & DDL_Currency.SelectedValue & "', " & TB_Fee.Text & ", '" & DDL_Sales_Representative.SelectedValue & "' "
                RunSQL(sqlStr)

                Dim sqlStr1 As String = "INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date) " &
                                        "SELECT TOP 1 [Distributor ID], [Requestor ID], [PO No], [PO Date] FROM I_CZL_Account_Setup_Fee WHERE [PO No] = '" & TB_PO_No.Text & "' "

                RunSQL(sqlStr1)
            Catch ex As Exception
                Response.Write("ERROR: " & ex.Message)
            End Try
        End If

        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Search button
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub


End Class
