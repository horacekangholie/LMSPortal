
Partial Class Form_CZL_Model_Update_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "CZL Account Model Update"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        LB_PageTitle.Text = PageTitle
        FormView1.ChangeMode(FormViewMode.ReadOnly)  '' Formview1 always set as read only mode

        If Not IsPostBack Then
            If Request.QueryString("Account_Unique_ID") <> "" And Not Request.QueryString("Account_Unique_ID") Is Nothing Then
                PopulateFormViewData()
                PopulateGridViewData()
            Else
                Response.Redirect("~/Form/CZL_Model_Update.aspx")
            End If
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
    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr As String = "SELECT *, ROW_NUMBER() OVER(ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC) AS rownum " &
                                   "FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' " &
                                   "ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC "

            BuildGridView(GridView1, "GridView1", "UID")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        Update_Model_Stats()
    End Sub

    Sub Update_Model_Stats()
        Dim sqlStr As String() = {"SELECT TOP 1 To_Model AS Current_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC ",
                                  "SELECT TOP 1 Effective_Date AS Current_Effective_Date FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC ",
                                  "SELECT COUNT(*) AS No_Of_Update_Within_Grace_Period FROM CZL_Account_Model_Upgrade_Trail A INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID WHERE A.Effective_Date < DATEADD(Year, 1, B.Effective_Date) AND A.CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' ",
                                  "SELECT COUNT(*) AS No_Of_Update_After_Grace_Period FROM CZL_Account_Model_Upgrade_Trail A INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID WHERE A.Effective_Date >= DATEADD(Year, 1, B.Effective_Date) AND A.CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' ",
                                  "SELECT FORMAT(Effective_Date, 'dd MMM yy') + ' - ' + FORMAT(One_Year_Period_End, 'dd MMM yy') AS Grace_Period FROM CZL_Account WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' ",
                                  "SELECT One_Year_Period_End AS Grace_Period_End FROM CZL_Account WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' "}

        Dim ColName As String() = {"Current_Model", "Current_Effective_Date", "No_Of_Update_Within_Grace_Period", "No_Of_Update_After_Grace_Period", "Grace_Period", "Grace_Period_End"}


        Dim Account_Model As TextBox = FormView1.FindControl("TB_Model")
        LB_Current_Model.Text = Get_Value(sqlStr(0), ColName(0))
        LB_Current_Model.Text = IIf(LB_Current_Model.Text <> "", LB_Current_Model.Text, Account_Model.Text)

        LB_Current_Effective_Date.Text = Get_Value(sqlStr(1), ColName(1))
        LB_Current_Effective_Date.Text = IIf(LB_Current_Effective_Date.Text <> "", LB_Current_Effective_Date.Text, "1900-01-01")

        LB_Update_Within_Grace_Period.Text = Get_Value(sqlStr(2), ColName(2))
        LB_Update_After_Grace_Period.Text = Get_Value(sqlStr(3), ColName(3))

        LB_Grace_Period.Text = Get_Value(sqlStr(4), ColName(4))
        LB_Grace_Period_End_Date.Text = Format(CDate(Get_Value(sqlStr(5), ColName(5))), "yyyy-MM-dd")

    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
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
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 5
                GridViewObj.ShowHeader = True
                GridViewObj.ShowFooter = False
        End Select

    End Sub






    '' FormView controls
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
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






    '' Gridview control
    Protected Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Set confirmation before deleting
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim DeleteCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1
            Dim DeleteCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteCommandFieldIndex), LinkButton)

            '' display lock icon to prevent deletion when it is not latest model update but still allow to edit.
            If drv("rownum") > 1 AndAlso DeleteCommandField.Text.Contains("trash") Then
                DeleteCommandField.Text = "<span class='btn btn-xs btn-light disabled'><i class='bi bi-lock'></i></span>"
                DeleteCommandField.Enabled = False
            Else
                If DeleteCommandField.CommandName = "Delete" Then
                    DeleteCommandField.Attributes.Add("onclick", "return confirm('This will also CZL Account Notes for model upgrade.\nAre you sure to delete?');")
                End If
            End If
        End If

        '' Unwrap of all column
        For i = 0 To e.Row.Cells.Count - 1
            e.Row.Cells(i).Wrap = False
        Next
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub



    Protected Sub BN_AddNew_Click(sender As Object, e As EventArgs) Handles BN_AddNew.Click
        Dim CZL_Account_Unique_ID As String = Request.QueryString("Account_Unique_ID")
        Dim Client_ID As String = TryCast(FormView1.FindControl("TB_CZL_Client_ID"), TextBox).Text
        Dim By_Who As String = Session("User_Name")

        If validateRecord(CInt(TB_New_Model.Text), CInt(LB_Current_Model.Text), CDate(TB_Effective_Date.Text), CDate(LB_Current_Effective_Date.Text), Trim(TB_Bind_Key.Text.ToUpper)) Then
            Try
                Dim sqlStr As String = "EXEC SP_CRUD_CZL_Model_Update N'" & CZL_Account_Unique_ID & "', N'" & TB_New_Model.Text & "', N'" & TB_Effective_Date.Text & "', N'" & TB_Bind_Key.Text & "', N'" & TB_Remarks.Text & "', N'" & By_Who & "' "
                RunSQL(sqlStr)

                '' Model Update chargeable section
                Dim sqlStr1 As String = "EXEC SP_CRUD_CZL_Model_Update_Charge N'" & CZL_Account_Unique_ID & "', N'" & Client_ID & "', N'" & TB_New_Model.Text & "', N'" & EscapeChar(TB_PO_No.Text) & "', N'" & TB_PO_Date.Text & "', N'" & TB_Bind_Key.Text.ToUpper & "' "

                '' if model update after grace period or else model update request already more than 4 times
                '' then it is chargeable and need to provide PO No.
                Dim shouldCharge As Boolean = CDate(TB_Effective_Date.Text) > CDate(LB_Grace_Period_End_Date.Text) OrElse CInt(LB_Update_Within_Grace_Period.Text) > 3

                If shouldCharge Then
                    RunSQL(sqlStr1)
                End If
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try

            '' Initialized fields after record saved
            TB_New_Model.Text = String.Empty
            TB_Effective_Date.Text = String.Empty
            TB_PO_No.Text = String.Empty
            TB_PO_Date.Text = String.Empty
            TB_Bind_Key.Text = String.Empty
            TB_Remarks.Text = String.Empty
        End If

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Function validateRecord(ByVal newModel As String, ByVal currentModel As String, ByVal newEffectiveDate As Date, ByVal currentEffective_date As Date, ByVal bindKey As String) As Boolean
        Dim IsRecordValid As Boolean = True
        If CInt(newModel) <= CInt(currentModel) Then
            newmodelupdateerrormsg.Visible = True
            newmodelupdateerrormsg.Style.Add("display", "inline")
            TB_New_Model.Focus()
            IsRecordValid = False
        Else
            newmodelupdateerrormsg.Visible = False
        End If

        If CDate(newEffectiveDate) < CDate(currentEffective_date) Then
            neweffectivedateerrormsg.Visible = True
            neweffectivedateerrormsg.Style.Add("display", "inline")
            TB_Effective_Date.Focus()
            IsRecordValid = False
        Else
            neweffectivedateerrormsg.Visible = False
        End If

        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Account_Model_Upgrade_Trail WHERE Bind_Key ='" & Trim(bindKey) & "'", "NoOfRecord")) > 0, True, False)
        If RecordExists Then
            bindkeyexisterrmsg.Visible = True
            bindkeyexisterrmsg.Style.Add("display", "inline")
            TB_Bind_Key.Focus()
            IsRecordValid = False
        Else
            bindkeyexisterrmsg.Visible = False
        End If

        Return IsRecordValid
    End Function





    '' Gridview row controls event
    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData()
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)

        Dim UID As String = GridView1.DataKeys(e.RowIndex).Values(0)
        Dim From_Model As String = (TryCast(row.FindControl("TB_E_From_Model"), TextBox)).Text
        Dim To_Model As String = (TryCast(row.FindControl("TB_E_To_Model"), TextBox)).Text
        Dim Effective_Date As String = (TryCast(row.FindControl("TB_E_Effective_Date"), TextBox)).Text
        Dim Bind_Key As String = (TryCast(row.FindControl("TB_E_Bind_Key"), TextBox)).Text
        Dim Remarks As String = EscapeChar((TryCast(row.FindControl("TB_E_Remarks"), TextBox)).Text)
        Dim By_Who As String = Session("User_Name")

        Try
            Dim sqlStr As String = "UPDATE CZL_Account_Model_Upgrade_Trail SET From_Model = '" & From_Model & "', To_Model = '" & To_Model & "', Effective_Date = N'" & Effective_Date & "', Bind_Key = N'" & Bind_Key & "', Last_Update = GETDATE(), Remarks = N'" & Remarks & "' WHERE UID = '" & UID & "' "
            'Dim sqlStr1 As String = "UPDATE CZL_Device_Status_Log SET Log_Description = N'Updated from model-" & From_Model & " to model-" & To_Model & "', Last_Update = GETDATE(), By_Who = N'" & By_Who & "' WHERE UID = '" & UID & "' "

            RunSQL(sqlStr)
            'RunSQL(sqlStr1)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        GridView1.EditIndex = -1
        PopulateGridViewData()
    End Sub

    Protected Sub GridView1_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles GridView1.RowDeleting
        Dim CZL_Account_Unique_ID As String = Request.QueryString("Account_Unique_ID")
        Dim UID As String = GridView1.DataKeys(e.RowIndex).Values(0)
        Dim LatestModelBeforeDeletion As String = Get_Value("SELECT TOP 1 To_Model AS Current_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "' ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC", "Current_Model")

        '' Delete a model trail record
        Dim sqlStr As String = "DELETE FROM CZL_Account_Notes WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "' AND Notes LIKE '%model-" & LatestModelBeforeDeletion & "%'; " &
                               "DELETE FROM CZL_Account_Model_Upgrade_Trail WHERE UID = '" & UID & "' "


        '' Update the account model to previous model after the deletion
        Dim sqlStr1 As String = "UPDATE CZL_Account
                                 SET Account_Model = (SELECT TOP 1 To_Model AS Current_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "' ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC )
                                 WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "'  "

        '' To delete the upgrade charge record along
        Dim sqlStr2 As String = "DELETE FROM CZL_Account_Model_Upgrade_Charge WHERE UID = '" & UID & "' "

        'Dim sqlStr3 As String = "DELETE FROM CZL_Device_Status_Log WHERE UID = '" & UID & "' "
        'Dim sqlStr4 As String = "UPDATE CZL_Licenced_Devices
        '                         SET Model = (SELECT TOP 1 To_Model AS Currenct_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "' ORDER BY CAST(ISNULL(To_Model, 0) AS int) DESC )
        '                         WHERE CZL_Account_Unique_ID = '" & CZL_Account_Unique_ID & "'  "

        Try
            RunSQL(sqlStr)
            RunSQL(sqlStr1)
            RunSQL(sqlStr2)
            'RunSQL(sqlStr3)
            'RunSQL(sqlStr4)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        PopulateGridViewData()
    End Sub




    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/CZL_Model_Update.aspx")
    End Sub


End Class
