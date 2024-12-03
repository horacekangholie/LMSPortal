
Partial Class Reports_Licence_Details
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Licence Details"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)   '' default formview as readonly
        LB_PageTitle.Text = PageTitle

        If Not IsPostBack Then
            Try
                RunSQL("EXEC SP_Sync_LMS_Licence")
            Catch ex As Exception
                Response.Write("ERROR: " & ex.Message)
            End Try
            PopulateFormViewData()
        End If
    End Sub

    Protected Sub PopulateFormViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(Trim(TB_Search))
        keyword = Replace(keyword, "-", "")
        keyword = Replace(keyword, ":", "")

        Try
            Dim sqlStr As String

            If Len(keyword) > 0 Then
                sqlStr = " SELECT * FROM _LMS_Licence_Details " &
                         " WHERE Lower(Replace([Licence Code], '-', '')) LIKE '%" & LCase(keyword) & "%' " &
                         "    OR Lower(Replace([MAC Address], '-', '')) LIKE '%" & LCase(keyword) & "%' " &
                         "    OR Lower(Replace([MAC Address], ':', '')) LIKE '%" & LCase(keyword) & "%' " &
                         "    OR Lower([Serial No]) LIKE '%" & LCase(keyword) & "%' " &
                         "    OR Lower([Licensee]) LIKE '%" & LCase(keyword) & "%' " &
                         "    OR Lower(Replace([AI Device ID], '-', '')) LIKE '%" & LCase(keyword) & "%' " &
                         " ORDER BY [Created Date] DESC "
            Else
                sqlStr = " SELECT * FROM _LMS_Licence_Details WHERE [Licence Code] = '" & keyword & "'"
            End If

            ''Response.Write(sqlStr)
            BuildFormView()
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Display the message alert when user search and formview return no record
        NoRecordAlert.Visible = IIf(Len(Trim(TB_Search)) > 0 And FormView1.PageCount <= 0, True, False)


        '' Display or hide field based on application type
        If FormView1.CurrentMode = FormViewMode.ReadOnly Then
            Dim Application_Type As TextBox = FormView1.FindControl("TB_Application_Type")
            Dim AI_Device_ID_Row As Control = FormView1.FindControl("AI_Device_ID_Row")
            Dim DMC_Server_Licence_Row As Control = FormView1.FindControl("DMC_Server_Licence_Row")
            If Not Application_Type Is Nothing Then
                AI_Device_ID_Row.Visible = IIf(Application_Type.Text.Contains("PC Scale"), True, False)
                DMC_Server_Licence_Row.Visible = IIf(Application_Type.Text.Contains("DMC"), True, False)
            End If
        ElseIf FormView1.CurrentMode = FormViewMode.Edit Then
            Dim E_Application_Type As TextBox = FormView1.FindControl("TB_E_Application_Type")
            Dim E_AI_Device_ID_Row As Control = FormView1.FindControl("AI_E_Device_ID_Row")
            Dim E_DMC_Server_Licence_Row As Control = FormView1.FindControl("DMC_E_Server_Licence_Row")
            If Not E_Application_Type Is Nothing Then
                E_AI_Device_ID_Row.Visible = IIf(E_Application_Type.Text.Contains("PC Scale"), True, False)
                E_DMC_Server_Licence_Row.Visible = IIf(E_Application_Type.Text.Contains("DMC"), True, False)
            End If
        End If

    End Sub

    Protected Sub BuildFormView()
        '' Formview properties
        FormView1.DataKeyNames = {"Licence Code"}

        FormView1.PagerSettings.Mode = PagerButtons.NumericFirstLast
        FormView1.PagerSettings.FirstPageText = "First"
        FormView1.PagerSettings.LastPageText = "Last"
        FormView1.PagerSettings.PageButtonCount = 10
        FormView1.PagerSettings.Position = PagerPosition.Bottom

        FormView1.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        FormView1.PagerStyle.CssClass = "pagination-ys"
    End Sub



    '' FormView controls
    Protected Sub FormView1_ModeChanging(ByVal sender As Object, ByVal e As FormViewModeEventArgs)
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
    End Sub

    Protected Sub FormView1_PageIndexChanging(sender As Object, e As FormViewPageEventArgs)
        FormView1.PageIndex = e.NewPageIndex
        PopulateFormViewData(TB_Search.Text)
    End Sub



    '' Commnad button controls
    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        '' When edit button is clicked, change the mode to edit mode
        FormView1.ChangeMode(FormViewMode.Edit)
        PopulateFormViewData(TB_Search.Text)
    End Sub

    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs)
        '' When update button is clicked, populate those edit field data and update to DB
        Dim Licence_Code As String = FormView1.DataKey.Value.ToString
        Dim Serial_No As TextBox = FormView1.FindControl("TB_E_Serial_No")
        Dim AI_Device_ID As TextBox = FormView1.FindControl("TB_E_AI_Device_ID")
        Dim AI_Device_Serial_No As TextBox = FormView1.FindControl("TB_E_AI_Device_Serial_No")
        Dim Remarks As TextBox = FormView1.FindControl("TB_E_Remarks")

        Dim sqlStr As String = "UPDATE LMS_Licence " &
                               "SET Serial_No = '" & Serial_No.Text & "', AI_Device_ID = '" & AI_Device_ID.Text & "', AI_Device_Serial_No = '" & AI_Device_Serial_No.Text & "', Remarks = '" & Remarks.Text & "' " &
                               "WHERE Licence_Code = '" & Licence_Code & "' "

        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' Once update completed, change the mode to readonly
        FormView1.ChangeMode(FormViewMode.ReadOnly)
        PopulateFormViewData(TB_Search.Text)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        '' when cancel button is clicked, change back the mode readonly
        FormView1.ChangeMode(FormViewMode.ReadOnly)
        PopulateFormViewData(TB_Search.Text)
    End Sub


    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateFormViewData(TB_Search.Text)
    End Sub


End Class
