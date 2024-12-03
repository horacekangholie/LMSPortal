
Partial Class Form_Customer_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" Then
                FormView1.ChangeMode(FormViewMode.Edit)
                PageTitle = "Update Customer"
                BT_Create.Text = "Update"
                BT_Create.CommandName = "Update"
            Else
                FormView1.ChangeMode(FormViewMode.Insert)
                PageTitle = "New Customer"
                BT_Create.Text = "Create"
                BT_Create.CommandName = "Create"
            End If
            LB_PageTitle.Text = PageTitle
            PopulateFormViewData()
        End If

    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr = " SELECT * FROM Master_Customer WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "'"
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

    End Sub

    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
    End Sub


    Protected Sub DDL_Country_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Country As Label = FormView1.FindControl("LB_Country")
        Dim DDL_Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByText(LB_Country.Text))
        i = IIf(i < 0, 0, i)
        DDL_Country.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Type_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Type As Label = FormView1.FindControl("LB_Type")
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim i = DDL_Type.Items.IndexOf(DDL_Type.Items.FindByText(LB_Type.Text))
        i = IIf(i < 0, 0, i)
        DDL_Type.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Group_ID_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Group As Label = FormView1.FindControl("LB_Group_ID")
        Dim DDL_Group As DropDownList = FormView1.FindControl("DDL_Group_ID")
        Dim i = DDL_Group.Items.IndexOf(DDL_Group.Items.FindByValue(LB_Group.Text))
        i = IIf(i < 0, 0, i)
        DDL_Group.Items(i).Selected = True
    End Sub

    Protected Sub DDL_By_Distributor_DataBound(sender As Object, e As EventArgs)
        Dim Type As Label = FormView1.FindControl("LB_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(Type.Text <> "Customer" And Type.Text <> "", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
        Dim By_Distributor As Label = FormView1.FindControl("LB_By_Distributor")
        Dim DDL_By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim i = DDL_By_Distributor.Items.IndexOf(DDL_By_Distributor.Items.FindByValue(By_Distributor.Text))
        DDL_By_Distributor.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Type_SelectedIndexChanged(sender As Object, e As EventArgs)
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(DDL_Type.SelectedItem.Text <> "Customer", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
    End Sub

    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Dim Customer_ID As String = IIf(Request.QueryString("Customer_ID") Is Nothing, "", Request.QueryString("Customer_ID"))
        Dim Name As TextBox = FormView1.FindControl("TB_Name")
        Dim Code As TextBox = FormView1.FindControl("TB_Code")
        Dim Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim Address As TextBox = FormView1.FindControl("TB_Address")
        Dim Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim Group_ID As DropDownList = FormView1.FindControl("DDL_Group_ID")
        Dim By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim Contact_Person As TextBox = FormView1.FindControl("TB_Contact_Person")
        Dim Phone As TextBox = FormView1.FindControl("TB_Phone")
        Dim Email As TextBox = FormView1.FindControl("TB_Email")
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)

        Try
            Dim sqlStr As String = " EXEC SP_CRUD_Customer N'" & Customer_ID & "' " &
                                            ", N'" & EscapeChar(Name.Text) & "' " &
                                            ", N'" & Code.Text & "' " &
                                            ", N'" & Country.SelectedItem.Text & "' " &
                                            ", N'" & EscapeChar(Address.Text) & "' " &
                                            ", N'" & Type.Text & "' " &
                                            ", N'" & Group_ID.Text & "' " &
                                            ", N'" & By_Distributor.SelectedValue & "' " &
                                            ", N'" & EscapeChar(Contact_Person.Text) & "' " &
                                            ", N'" & Phone.Text & "' " &
                                            ", N'" & EscapeChar(Email.Text) & "' " &
                                            ", N'" & BtnCommand.CommandName & "' "
            RunSQL(sqlStr)
            Response.Write(AlertMessageRedirect("Customer " & Replace(Name.Text, "'", "\'") & " " & BtnCommand.Text.ToLower & "d.", "/Form/Customer.aspx"))

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

    End Sub

    Protected Sub BT_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Cancel.Click
        Response.Redirect("~/Form/Customer.aspx")
    End Sub

End Class
