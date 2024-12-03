
Partial Class Form_Hardkey_Licence_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Register Hardkey Licence"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)
        LB_PageTitle.Text = PageTitle

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" And Not Request.QueryString("Customer_ID") Is Nothing Then
                PopulateFormViewData()
            Else
                Response.Redirect("~/Form/Hardkey_Licence.aspx")
            End If
        End If
        PopulateGridViewData()
    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr As String = " SELECT * FROM Master_Customer WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' "
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {"SELECT * FROM R_Hardkey_Licence WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "'"}

            BuildGridView(GridView1, "GridView1", "Licence No")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowFooter = True
        GridViewObj.ShowHeaderWhenEmpty = True
        GridViewObj.DataKeyNames = New String() {DataKeyName}
        GridViewObj.CssClass = "table table-bordered"
        GridViewObj.Style.Add("width", "99.3%")

        '' Header Style
        GridViewObj.HeaderStyle.CssClass = "table-secondary"
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
        GridViewObj.PagerSettings.PageButtonCount = "5"
        GridViewObj.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewObj.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridViewObj.EmptyDataText = "No records found."

        '' Define each Gridview
        Select Case ControlName
            Case "GridView1"
                '' Build GridView Content
                GridViewObj.AllowPaging = False
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"PLU Code", "PO No", "PO Date", "SO No", "SO Date", "Invoice No", "Invoice Date", "Licence No", "Prepared By", "Requested By"}
                Dim ColSize() As Integer = {50, 100, 50, 100, 50, 100, 50, 200, 50, 200}
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

                '' Add templatefield for Edit icon
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)
        End Select
    End Sub


    '' FormView control
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub DDL_Country_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Country As Label = FormView1.FindControl("LB_Country")
        Dim DDL_Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByText(LB_Country.Text))
        i = IIf(i < 0, 0, i)
        DDL_Country.SelectedIndex = i
    End Sub

    Protected Sub DDL_Type_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Type As Label = FormView1.FindControl("LB_Type")
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim i = DDL_Type.Items.IndexOf(DDL_Type.Items.FindByText(LB_Type.Text))
        i = IIf(i < 0, 0, i)
        DDL_Type.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(DDL_Type.SelectedItem.Text <> "Customer", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
    End Sub

    Protected Sub DDL_Group_ID_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Group As Label = FormView1.FindControl("LB_Group_ID")
        Dim DDL_Group As DropDownList = FormView1.FindControl("DDL_Group_ID")
        Dim i = DDL_Group.Items.IndexOf(DDL_Group.Items.FindByValue(LB_Group.Text))
        i = IIf(i < 0, 0, i)
        DDL_Group.Items(i).Selected = True
    End Sub

    Protected Sub DDL_By_Distributor_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim Type As Label = FormView1.FindControl("LB_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(Type.Text <> "Customer" And Type.Text <> "", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
        Dim By_Distributor As Label = FormView1.FindControl("LB_By_Distributor")
        Dim DDL_By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim i = DDL_By_Distributor.Items.IndexOf(DDL_By_Distributor.Items.FindByValue(By_Distributor.Text))
        DDL_By_Distributor.Items(i).Selected = True
    End Sub


    '' Gridview control
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim DeletectrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(DeletectrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton
            DeleteLinkButton.CommandArgument = drv("Licence No")  '' pass the licence no as command argument to delete button
            DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
            DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
            DeleteLinkButton.CausesValidation = False
            AddHandler DeleteLinkButton.Click, AddressOf Delete_Headquarter_Click

            '' Lock record if invoice has been recovered
            If Len(drv("Invoice No").ToString) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
                DeleteLinkButton.Enabled = True
            End If

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If Len(drv("Invoice No").ToString) > 0 Then
                e.Row.Cells(5).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

        End If
    End Sub


    '' Modal control
    Protected Sub DDL_Licence_Type_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Licence_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Value_1 AS PLU_Code, Value_2 AS Description FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_4 = 'Hardkey Licence'"
                DDL_Licence_Type.DataSource = GetDataTable(sqlStr)
                DDL_Licence_Type.DataTextField = "Description"
                DDL_Licence_Type.DataValueField = "PLU_Code"
                DDL_Licence_Type.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Licence_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Licence_Type.SelectedIndexChanged
        Dim DDL_Licence_Type As DropDownList = pnlAddEditHardkey.FindControl("DDL_Licence_Type")

        Select Case DDL_Licence_Type.Text
            Case "64150", "64392"
                TB_Start_Date.Enabled = False
                RequiredField_TB_Start_Date.Enabled = False
                TB_End_Date.Enabled = False
                RequiredField_TB_End_Date.Enabled = False
            Case Else
                TB_Start_Date.Enabled = True
                RequiredField_TB_Start_Date.Enabled = True
                TB_End_Date.Enabled = True
                RequiredField_TB_End_Date.Enabled = True
        End Select

        popupHardkey.Show()
    End Sub

    Protected Sub DDL_Sales_Representative_Load(ByVal sender As Object, ByVal e As EventArgs) Handles DDL_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name"
                DDL_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_Sales_Representative.DataTextField = "Name"
                DDL_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub Add_Hardkey_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddHardkey.Click
        ModalHeaderHardkey.Text = "Add Hardkey Licence"
        btnSaveHardkey.Text = "Save"
        btnCancelHardkey.Text = "Cancel"

        TB_PO_No.Text = String.Empty
        TB_PO_Date.Text = String.Empty
        TB_Licence_No.Text = String.Empty
        TB_Start_Date.Text = String.Empty
        TB_End_Date.Text = String.Empty
        TB_Prepared_By.Text = String.Empty

        Dim DDL_Licence_Type As DropDownList = pnlAddEditHardkey.FindControl("DDL_Licence_Type")

        '' disable start date and end date field when first load if Demo licence type is pre selected
        Select Case DDL_Licence_Type.Text
            Case "64150", "64392"
                TB_Start_Date.Enabled = False
                RequiredField_TB_Start_Date.Enabled = False
                TB_End_Date.Enabled = False
                RequiredField_TB_End_Date.Enabled = False
            Case Else
                TB_Start_Date.Enabled = True
                RequiredField_TB_Start_Date.Enabled = True
                TB_End_Date.Enabled = True
                RequiredField_TB_End_Date.Enabled = True
        End Select

        popupHardkey.Show()
    End Sub

    Protected Sub Save_Hardkey_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveHardkey.Click
        Dim PO_No As TextBox = pnlAddEditHardkey.FindControl("TB_PO_No")
        Dim PO_Date As TextBox = pnlAddEditHardkey.FindControl("TB_PO_Date")
        Dim Licence_No As TextBox = pnlAddEditHardkey.FindControl("TB_Licence_No")
        Dim Licence_Type As DropDownList = pnlAddEditHardkey.FindControl("DDL_Licence_Type")
        Dim Start_Date As TextBox = pnlAddEditHardkey.FindControl("TB_Start_Date")
        Dim End_Date As TextBox = pnlAddEditHardkey.FindControl("TB_End_Date")
        Dim Prepared_By As TextBox = pnlAddEditHardkey.FindControl("TB_Prepared_By")
        Dim Sales_Representative As DropDownList = pnlAddEditHardkey.FindControl("DDL_Sales_Representative")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_LMS_Hardkey_Licence N'" & EscapeChar(PO_No.Text) &
                                                                  "', '" & PO_Date.Text &
                                                                 "', N'" & Licence_No.Text &
                                                                  "', '" & Licence_Type.Text &
                                                                  "', '" & Start_Date.Text &
                                                                  "', '" & End_Date.Text &
                                                                  "', '" & Prepared_By.Text &
                                                                  "', '" & Sales_Representative.Text &
                                                                  "', '" & Customer_ID & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub Delete_Headquarter_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim sqlStr As String = " DELETE FROM LMS_Hardkey_Licence WHERE Licence_No ='" & DeleteLinkButton.CommandArgument & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub


    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/Hardkey_Licence.aspx")
    End Sub


End Class
