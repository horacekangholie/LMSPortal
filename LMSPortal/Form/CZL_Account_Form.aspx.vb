
Partial Class Form_CZL_Account_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            If Request.QueryString("Account_Unique_ID") <> "" Then
                FormView1.ChangeMode(FormViewMode.Edit)
                PageTitle = "Update AI Account"
                BT_Create.Text = "Update"
                BT_Create.CommandName = "Update"
            Else
                FormView1.ChangeMode(FormViewMode.Insert)
                PageTitle = "New AI Account"
                BT_Create.Text = "Create"
                BT_Create.CommandName = "Create"
            End If
            LB_PageTitle.Text = PageTitle
            PopulateFormViewData()
        End If
    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr = "SELECT * FROM CZL_Account WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "'"
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Disabled Model field when it is on edit mode and account model field has a value
        Dim Account_Model As TextBox = FormView1.FindControl("TB_Account_Model")
        Account_Model.Enabled = IIf(Request.QueryString("Account_Unique_ID") <> "" AndAlso Account_Model.Text <> "", False, True)

    End Sub

    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
    End Sub


    '' Account details section
    Protected Sub TB_CZL_Client_ID_TextChanged(sender As Object, e As EventArgs)
        Dim CZL_Client_ID As TextBox = FormView1.FindControl("TB_CZL_Client_ID")
        Dim User_Group As TextBox = FormView1.FindControl("TB_User_Group")

        '' Check database if the Client ID exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Account WHERE Client_ID ='" & CZL_Client_ID.Text & "'", "NoOfRecord")) > 0, True, False)
        '' Format the Client ID validation message
        Dim czlclientidexisterrormsg As Label = FormView1.FindControl("czlclientidexisterrormsg")
        czlclientidexisterrormsg.Visible = False   '' Default is hidden

        '' Display alert message a client ID is exists
        If RecordExists Then
            czlclientidexisterrormsg.Visible = True
            czlclientidexisterrormsg.Style.Add("display", "inline")    '' required this line to display message
            CZL_Client_ID.Focus()
        Else
            User_Group.Focus()
        End If
    End Sub

    Protected Sub DDL_By_Distributor_DataBound(sender As Object, e As EventArgs)
        Dim By_Distributor As Label = FormView1.FindControl("LB_By_Distributor")
        Dim DDL_By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim i = DDL_By_Distributor.Items.IndexOf(DDL_By_Distributor.Items.FindByValue(By_Distributor.Text))
        i = IIf(i < 0, 0, i)
        DDL_By_Distributor.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Country_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Country As Label = FormView1.FindControl("LB_Country")
        Dim DDL_Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByText(LB_Country.Text))
        i = IIf(i < 0, 0, i)
        DDL_Country.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Gen_Version_DataBound(sender As Object, e As EventArgs)
        Dim LB_Gen_Version As Label = FormView1.FindControl("LB_Gen_Version")
        Dim DDL_Gen_Version As DropDownList = FormView1.FindControl("DDL_Gen_Version")
        Dim i = DDL_Gen_Version.Items.IndexOf(DDL_Gen_Version.Items.FindByText(LB_Gen_Version.Text))
        i = IIf(i < 0, 0, i)
        DDL_Gen_Version.Items(i).Selected = True
    End Sub


    '' Account setup billable section
    Protected Sub DDL_Account_Setup_Billable_DataBound(sender As Object, e As EventArgs)
        Dim DDL_Account_Setup_Billable As DropDownList = FormView1.FindControl("DDL_Account_Setup_Billable")
        Dim i = DDL_Account_Setup_Billable.Items.IndexOf(DDL_Account_Setup_Billable.Items.FindByText("Yes"))
        i = IIf(i < 0, 0, i)
        DDL_Account_Setup_Billable.Items(i).Selected = True   '' default select NO
    End Sub

    Protected Sub DDL_Currency_DataBound(sender As Object, e As EventArgs)
        Dim DDL_Currency As DropDownList = FormView1.FindControl("DDL_Currency")
        DDL_Currency.SelectedValue = DDL_Currency.Items.FindByText("SGD").Value  '' Default as SGD
    End Sub

    Protected Sub DDL_Sales_Representative_DataBound(sender As Object, e As EventArgs)
        Dim DDL_Sales_Representative As DropDownList = FormView1.FindControl("DDL_Sales_Representative")
        DDL_Sales_Representative.Items(0).Selected = True
    End Sub

    Protected Sub DDL_Account_Setup_Billable_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim DDL_Account_Setup_Billable As DropDownList = FormView1.FindControl("DDL_Account_Setup_Billable")
        Dim lbl_po_no As HtmlGenericControl = FormView1.FindControl("lbl_po_no")
        Dim TB_PO_No As TextBox = FormView1.FindControl("TB_PO_No")
        Dim RequiredField_TB_PO_No As RequiredFieldValidator = FormView1.FindControl("RequiredField_TB_PO_No")

        Dim lbl_po_date As HtmlGenericControl = FormView1.FindControl("lbl_po_date")
        Dim TB_PO_Date As TextBox = FormView1.FindControl("TB_PO_Date")
        Dim RequiredField_TB_PO_Date As RequiredFieldValidator = FormView1.FindControl("RequiredField_TB_PO_Date")

        Dim lbl_currency As HtmlGenericControl = FormView1.FindControl("lbl_currency")
        Dim DDL_Currency As DropDownList = FormView1.FindControl("DDL_Currency")

        Dim lbl_fee As HtmlGenericControl = FormView1.FindControl("lbl_fee")
        Dim TB_Fee As TextBox = FormView1.FindControl("TB_Fee")
        Dim RequiredField_TB_Fee As RequiredFieldValidator = FormView1.FindControl("RequiredField_TB_Fee")
        Dim RegEx_TB_Fee As RegularExpressionValidator = FormView1.FindControl("RegEx_TB_Fee")

        Dim lbl_sales_representative As HtmlGenericControl = FormView1.FindControl("lbl_sales_representative")
        Dim DDL_Sales_Representative As DropDownList = FormView1.FindControl("DDL_Sales_Representative")
        Dim CompareValidator_I_DDL_Sales_Representative As CompareValidator = FormView1.FindControl("CompareValidator_I_DDL_Sales_Representative")


        '' Toggle controls
        lbl_po_no.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        TB_PO_No.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        RequiredField_TB_PO_No.Enabled = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)

        lbl_po_date.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        TB_PO_Date.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        RequiredField_TB_PO_Date.Enabled = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)

        lbl_currency.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        DDL_Currency.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)

        lbl_fee.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        TB_Fee.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        RequiredField_TB_Fee.Enabled = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        RegEx_TB_Fee.Enabled = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)

        lbl_sales_representative.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        DDL_Sales_Representative.Visible = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)
        CompareValidator_I_DDL_Sales_Representative.Enabled = IIf(DDL_Account_Setup_Billable.SelectedValue = 1, True, False)


    End Sub



    '' Command buttons
    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Dim CZL_Account_Unique_ID As String = IIf(Request.QueryString("Account_Unique_ID") Is Nothing, "", Request.QueryString("Account_Unique_ID"))
        Dim CZL_Client_ID As TextBox = FormView1.FindControl("TB_CZL_Client_ID")
        Dim User_Group As TextBox = FormView1.FindControl("TB_User_Group")
        Dim Account_Model As TextBox = FormView1.FindControl("TB_Account_Model")
        Dim By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim Gen_Version As DropDownList = FormView1.FindControl("DDL_Gen_Version")
        Dim Effective_Date As TextBox = FormView1.FindControl("TB_Effective_Date")
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)


        '' Check database if the Client ID exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Account WHERE Client_ID ='" & CZL_Client_ID.Text & "'", "NoOfRecord")) > 0, True, False)

        '' Format the Client ID validation message
        Dim czlclientidexisterrormsg As Label = FormView1.FindControl("czlclientidexisterrormsg")
        czlclientidexisterrormsg.Visible = False   '' Default is hidden

        '' Define the sqlstr
        Dim sqlStr As String = "EXEC SP_CRUD_CZL_Account N'" & CZL_Account_Unique_ID & "' " &
                                                      ", N'" & CZL_Client_ID.Text & "' " &
                                                      ", N'" & EscapeChar(User_Group.Text) & "' " &
                                                      ", N'" & Account_Model.Text & "' " &
                                                      ", N'" & By_Distributor.SelectedValue & "' " &
                                                      ", N'" & Country.SelectedValue & "' " &
                                                      ", N'" & Gen_Version.SelectedValue & "' " &
                                                      ", N'" & Effective_Date.Text & "' " &
                                                      ", N'" & BtnCommand.CommandName & "' "

        Try
            If Not RecordExists AndAlso BtnCommand.Text = "Create" Then
                RunSQL(sqlStr)
                Response.Write(AlertMessageRedirect("CZL Account No. " & CZL_Client_ID.Text & " - " & Replace(User_Group.Text, "'", "\'") & " " & BtnCommand.Text.ToLower & "d.", "/Form/CZL_Account.aspx"))

                '' Account billable section
                Dim DDL_Account_Setup_Billable As DropDownList = FormView1.FindControl("DDL_Account_Setup_Billable")
                If DDL_Account_Setup_Billable.SelectedValue <> "0" Then   '' if account is set to non-billable, then set values to ""
                    Dim PO_No As TextBox = FormView1.FindControl("TB_PO_No")
                    Dim PO_Date As TextBox = FormView1.FindControl("TB_PO_Date")
                    Dim Currency As DropDownList = FormView1.FindControl("DDL_Currency")
                    Dim Fee As TextBox = FormView1.FindControl("TB_Fee")
                    Dim Sales_Representative As DropDownList = FormView1.FindControl("DDL_Sales_Representative")

                    Dim sqlStr1 As String = "EXEC SP_CRUD_CZL_Account_Setup_Charge N'" & CZL_Account_Unique_ID & "' " &
                                                                                ", N'" & CZL_Client_ID.Text & "' " &
                                                                                ", N'" & PO_No.Text & "' " &
                                                                                ", N'" & PO_Date.Text & "' " &
                                                                                ", N'" & Currency.SelectedValue & "' " &
                                                                                ", N'" & Fee.Text & "' " &
                                                                                ", N'" & Sales_Representative.SelectedValue & "' "

                    RunSQL(sqlStr1)  '' create open case for account setup fee billing
                End If
            Else
                If BtnCommand.Text = "Update" Then
                    RunSQL(sqlStr)
                    Response.Write(AlertMessageRedirect("CZL Account No. " & CZL_Client_ID.Text & " - " & Replace(User_Group.Text, "'", "\'") & " " & BtnCommand.Text.ToLower & "d.", "/Form/CZL_Account.aspx"))
                Else
                    czlclientidexisterrormsg.Visible = True
                    czlclientidexisterrormsg.Style.Add("display", "inline")
                End If
            End If
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

    End Sub

    Protected Sub BT_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Cancel.Click
        Response.Redirect("~/Form/CZL_Account.aspx")
    End Sub


End Class
