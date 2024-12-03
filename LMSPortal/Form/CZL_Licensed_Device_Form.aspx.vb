
Partial Class Form_CZL_Licensed_Device_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "CZL Licenced Devices"

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
                PopulateGridViewData1()
                PopulateGridViewData2()
            Else
                Response.Redirect("~/Form/CZL_Licensed_Device.aspx")
            End If
        End If

        GridView1.Columns(1).Visible = False
    End Sub

    Protected Sub PopulateFormViewData()
        Try
            FormView1.DataSource = GetDataTable(GetSQL(FormView1, Nothing, Nothing, Nothing))
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            BuildGridView(GridView1, "GridView1", "Unique_ID")
            GridView1.DataSource = GetDataTable(GetSQL(GridView1, Nothing, keyword, Nothing))
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData1(Optional ByVal Unique_ID As String = Nothing)
        Try
            BuildGridView(GridView2, "GridView2", "Unique_ID")
            GridView2.DataSource = GetDataTable(GetSQL(GridView2, Nothing, Unique_ID, Nothing))
            GridView2.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' hide /show Change log section controls
        DeviceLogSection.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        ChangeLog_Header.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        LB_Header_Serial_No.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        LB_Header_Licence_Key.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        GridView2.Visible = IIf(Not Unique_ID Is Nothing, True, False)

        btnAddCZLChangeLog.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        btnLogBatchUpdate.Visible = IIf(Not Unique_ID Is Nothing And GridView1.Rows.Count > 1, True, False)

        '' Change the behavior of border line
        DeviceListingSection.Style.Add("border", "solid 1px")
        DeviceListingSection.Style.Add("border-color", IIf(Not Unique_ID Is Nothing, "#fff #dee2e6 #fff", "#fff #dee2e6 #dee2e6"))
        DeviceLogSection.Style.Add("border", IIf(Not Unique_ID Is Nothing, "solid 1px", "solid 0px"))
        DeviceLogSection.Style.Add("border-color", "#fff #dee2e6 #dee2e6")

        '' Draw last line if page count less than 1
        GridView2.Style.Add("border-bottom", "1px solid " & IIf(GridView2.PageCount < 2, "#ddd", "#fff"))
    End Sub

    Protected Sub PopulateGridViewData2()
        Try
            BuildGridView(GridView3, "GridView3", "ID")
            GridView3.DataSource = GetDataTable(GetSQL(GridView3, Nothing, Nothing, Nothing))
            GridView3.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Draw last line if page count less than 1
        GridView3.Style.Add("border-bottom", "1px solid " & IIf(GridView3.PageCount < 2, "#ddd", "#fff !important"))
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
            Case "GridView2"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.ShowFooter = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #fff")
            Case "GridView3"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.ShowFooter = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #fff")
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
        PopulateGridViewData1()  '' when Gridview 1 page changed, populate Gridview 2 without passing the Unique ID, when no value passed, device change log (comments) section will be hidden
        PopulateGridViewData2()
    End Sub

    Protected Sub GridView2_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        GridView2.PageIndex = e.NewPageIndex

        Dim Selected_CZLChangeLogRow_Parent_Row_Index As TextBox = pnlAddEditCZLChangeLog.FindControl("TB_Selected_CZLChangeLog_Parent_Row_Index")  '' Get the value to determine which row to highlight in gray
        Dim Selected_CZLChangeLogRow_Unique_ID As TextBox = pnlAddEditCZLChangeLog.FindControl("TB_Selected_CZLChangeLog_Unique_ID")  '' Get the value to determine the comment of which device to display

        '' Populate formview and gridview
        PopulateFormViewData()
        PopulateGridViewData()
        PopulateGridViewData1(Selected_CZLChangeLogRow_Parent_Row_Index.Text) '' pass the unique_id (device unique id) to display it's comments
        PopulateGridViewData2()

        '' Highlight current selected device
        GridView1.Rows(Selected_CZLChangeLogRow_Parent_Row_Index.Text).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
    End Sub

    Protected Sub GridView3_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView3.PageIndexChanging
        GridView3.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
        PopulateGridViewData1()
        PopulateGridViewData2()
    End Sub


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim Account_Model As TextBox = FormView1.FindControl("TB_Model")

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
                e.Row.Attributes("style") = "cursor:default"
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' click on a row to hightlight and postback to populate comment section
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridView1, "Select$" & e.Row.RowIndex)  ''Indicate a row index selected then trigger PopulateGridViewData1(Unique_ID)
            e.Row.Attributes("style") = "cursor:pointer"

            '' Mark model if device model is different from the account model
            If e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Text <> Account_Model.Text Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Text = e.Row.Cells(GetColumnIndexByName(e.Row, "Model")).Text & " <i style='display:inline-block;float:right;color:#cb4335' class='fa fa-exclamation-triangle fa-sm' aria-hidden='true' title='Device model mismatch with account model'></i>"
            End If

            If e.Row.Cells(GetColumnIndexByName(e.Row, "Licence_Key")).Text = "No Binding Key" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Licence_Key")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Licence_Key")).Style.Add("color", "#808080")
            End If

            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If a device has comments then disable the delete button
            If CInt(drv("Comment_Count")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i = e.Row.Cells.Count - 1 Then
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right   '' right align the control button
                End If
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
            Next

            '' Format the edited_by column
            e.Row.Cells(GetColumnIndexByName(e.Row, "Edited_By")).Style.Add("padding-right", "30px")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Edited_By")).Style.Add("font-size", "13px")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Edited_By")).Style.Add("font-style", "italic")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Edited_By")).Style.Add("color", "#566573")

            '' Get the gridview row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' Disable edit and delete button when the comment / log is created by SYSTEM
            If drv("Log_Type") = "SYS" Then
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Visible = False

                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                EditLinkButton.Enabled = True
                DeleteLinkButton.Enabled = True
            End If

        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i = e.Row.Cells.Count - 1 Then
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right   '' right align the control button
                End If
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
            Next

            '' Get the gridview row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' Disable edit and delete button when notes is_locked status is 1
            If drv("Is_Locked") = 1 Then
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Visible = False

                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                EditLinkButton.Enabled = True
                DeleteLinkButton.Enabled = True
            End If

        End If
    End Sub


    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        Dim eCommandArgument As Array = Split(e.CommandArgument, "|")  '' pass eCmmandArgument to array
        Dim Selected_Row_Of_CZL_Device As Integer = Convert.ToInt32(eCommandArgument(0))

        '' Change the row color of the selected row for the View (select command)
        Dim rowIndex As Integer = Convert.ToInt32(Selected_Row_Of_CZL_Device)
        For Each row As GridViewRow In GridView1.Rows
            row.BackColor = If(row.RowIndex.Equals(rowIndex), Drawing.ColorTranslator.FromHtml("#eeeeee"), Drawing.Color.Transparent)
        Next
    End Sub

    Protected Sub GridView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView1.SelectedIndexChanged
        Dim Selected_Row_Index As Integer = GridView1.SelectedRow.RowIndex
        Dim Selected_Device_ID As String = GridView1.SelectedRow.Cells(2).Text
        Dim Selected_Unique_ID As String = Get_Value("SELECT Unique_ID FROM CZL_Licenced_Devices WHERE Device_ID = '" & Selected_Device_ID & "'", "Unique_ID")  '' Get updated unique id
        Dim Selected_Serial_No As String = GridView1.SelectedRow.Cells(9).Text
        Dim Selected_Licence_Key As String = GridView1.SelectedRow.Cells(3).Text

        '' Populate comment/log list when a row is clicked
        PopulateFormViewData()
        PopulateGridViewData(TB_Search.Text)
        PopulateGridViewData1(Selected_Unique_ID)

        '' Stay highligthed the row when click on update device record
        GridView1.Rows(Selected_Row_Index).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

        '' display the device id as header
        ChangeLog_Header.Visible = True
        LB_Header_Serial_No.Visible = True
        LB_Header_Serial_No.Text = Selected_Serial_No
        LB_Header_Licence_Key.Visible = True
        LB_Header_Licence_Key.Text = Selected_Licence_Key

        '' enable add log and batch add log button
        btnAddCZLChangeLog.Visible = True
        btnLogBatchUpdate.Visible = IIf(GridView1.Rows.Count > 1, True, False)   '' Only enable this batch comment update button when there is nore that 1 device

        '' pass value hidden field in change log panel
        TB_Selected_CZLChangeLog_Parent_Row_Index.Text = Selected_Row_Index  '' row index of device selected in gridview 1
        TB_Selected_CZLChangeLog_Unique_ID.Text = Selected_Unique_ID  '' unique id of the device selected in gridview 1

        '' Draw last line if page count less than 1
        GridView2.Style.Add("border-bottom", If(GridView2.PageCount < 2, "1px solid #ddd", "1px solid #fff"))
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub





    '' Modal controls - CZL Licenced Device
    Protected Sub DDL_CZL_Client_ID_Load(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.Load
        If Not IsPostBack Then
            Shared_Dropdownlist_Bind(DDL_CZL_Client_ID, GetSQL(DDL_CZL_Client_ID, Nothing, Nothing, Nothing), "Client_ID_User_Group", "Client_ID", "Please select", False)
        End If
    End Sub

    Protected Sub Add_CZL_Device_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddCZLDevice.Click
        ModalHeaderCZLDevice.Text = "Add CZL Licenced Device"
        btnSaveCZLDevice.Text = "Save"
        btnCancelCZLDevice.Text = "Cancel"

        '' Initialize the fields
        TB_Device_Serial.Text = String.Empty
        TB_Device_ID.Text = String.Empty
        TB_Model.Text = String.Empty
        TB_AI_Software_Version.Text = String.Empty
        TB_R_Version.Text = String.Empty
        TB_Scale_Serial_No.Text = String.Empty
        TB_Scale_Serial_No.Enabled = IIf(Len(TB_Scale_Serial_No.Text) > 0, False, True)
        TB_Location.Text = String.Empty
        TB_MAC_Address.Text = String.Empty
        TB_Production_Licence_No.Text = String.Empty
        TB_Effective_Date.Text = String.Empty
        DDL_CZL_Client_ID.SelectedIndex = 0  '' select the default value in dropdownlist
        DDL_CZL_Client_ID.Visible = False    '' hide the Client ID dropdownlist when in new mode

        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Row_Index,
                                     TB_Selected_Unique_ID,
                                     TB_Selected_Device_Serial,
                                     TB_Selected_Device_ID,
                                     TB_Selected_Model,
                                     TB_Selected_AI_Software_Version,
                                     TB_Selected_R_Version,
                                     TB_Selected_Location,
                                     TB_Selected_Scale_Serial_No,
                                     TB_Selected_MAC_Address,
                                     TB_Selected_Production_Licence_No,
                                     TB_Selected_Effective_Date,
                                     TB_Selected_Client_ID,
                                     TB_Selected_CZL_Account_Unique_ID,
                                     TB_Selected_Bind_Licence_Key}

        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        TB_Selected_CZLChangeLog_Parent_Row_Index.Text = String.Empty
        TB_Selected_CZLChangeLog_Unique_ID.Text = String.Empty

        '' Hide / show controls
        lbl_client_id.Visible = False
        p_client_id_instruction.Visible = False
        czldeviceserialexisterrormsg.Visible = False   '' hide Serial No field error message

        popupCZLDevice.Show()
    End Sub

    Protected Sub Edit_CZL_Device_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderCZLDevice.Text = "Edit CZL Licenced Device"
        btnSaveCZLDevice.Text = "Update"
        btnCancelCZLDevice.Text = "Cancel"

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Row_Index,
                                     TB_Selected_Unique_ID,
                                     TB_Selected_Device_Serial,
                                     TB_Selected_Device_ID,
                                     TB_Selected_Model,
                                     TB_Selected_AI_Software_Version,
                                     TB_Selected_R_Version,
                                     TB_Selected_Location,
                                     TB_Selected_Scale_Serial_No,
                                     TB_Selected_MAC_Address,
                                     TB_Selected_Production_Licence_No,
                                     TB_Selected_Effective_Date,
                                     TB_Selected_Client_ID,
                                     TB_Selected_CZL_Account_Unique_ID,
                                     TB_Selected_Bind_Licence_Key}

        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' Populate the fields based on the selected hidden fields
        TB_Device_Serial.Text = TB_Selected_Device_Serial.Text
        TB_Device_ID.Text = TB_Selected_Device_ID.Text
        TB_Model.Text = TB_Selected_Model.Text
        TB_AI_Software_Version.Text = TB_Selected_AI_Software_Version.Text
        TB_R_Version.Text = TB_Selected_R_Version.Text
        TB_Scale_Serial_No.Text = TB_Selected_Scale_Serial_No.Text
        TB_Scale_Serial_No.Enabled = IIf(Len(TB_Scale_Serial_No.Text) > 0, False, True) '' disable field in edit mode when field not empty
        TB_Location.Text = TB_Selected_Location.Text
        TB_MAC_Address.Text = TB_Selected_MAC_Address.Text
        TB_Production_Licence_No.Text = TB_Selected_Production_Licence_No.Text
        '' Handle effective date with dbnull value
        If TB_Selected_Effective_Date.Text <> "" Then
            TB_Effective_Date.Text = CDate(TB_Selected_Effective_Date.Text).ToString("yyyy-MM-dd")
        Else
            TB_Effective_Date.Text = String.Empty
        End If
        DDL_CZL_Client_ID.SelectedIndex = DDL_CZL_Client_ID.Items.IndexOf(DDL_CZL_Client_ID.Items.FindByValue(TB_Selected_Client_ID.Text))
        DDL_CZL_Client_ID.Visible = True    '' display the client id dropdownlist when in edit mode

        '' Hide / show controls
        lbl_client_id.Visible = True
        p_client_id_instruction.Visible = True
        czldeviceserialexisterrormsg.Visible = False   '' hide Serial No field error message


        '' Pass the selected row and the selected device unique id to change log hidden fields
        TB_Selected_CZLChangeLog_Parent_Row_Index.Text = TB_Selected_Row_Index.Text
        TB_Selected_CZLChangeLog_Unique_ID.Text = TB_Selected_Unique_ID.Text

        popupCZLDevice.Show()
    End Sub

    Protected Sub Save_CZL_Device_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveCZLDevice.Click
        Dim Device_Serial As TextBox = pnlAddEditCZLDevice.FindControl("TB_Device_Serial")
        Dim Device_ID As TextBox = pnlAddEditCZLDevice.FindControl("TB_Device_ID")
        Dim Model As TextBox = pnlAddEditCZLDevice.FindControl("TB_Model")
        Dim AI_Software_Version As TextBox = pnlAddEditCZLDevice.FindControl("TB_AI_Software_Version")
        Dim R_Version As TextBox = pnlAddEditCZLDevice.FindControl("TB_R_Version")
        Dim Scale_SN As TextBox = pnlAddEditCZLDevice.FindControl("TB_Scale_Serial_No")
        Dim MAC_Addr As TextBox = pnlAddEditCZLDevice.FindControl("TB_MAC_Address")
        Dim Production_Licence_No As TextBox = pnlAddEditCZLDevice.FindControl("TB_Production_Licence_No")
        Dim Location As TextBox = pnlAddEditCZLDevice.FindControl("TB_Location")
        Dim DDL_CZL_Client_ID As DropDownList = pnlAddEditCZLDevice.FindControl("DDL_CZL_Client_ID")
        Dim Effective_Date As TextBox = pnlAddEditCZLDevice.FindControl("TB_Effective_Date")
        Dim Unique_ID As TextBox = pnlAddEditCZLDevice.FindControl("TB_Selected_Unique_ID")
        Dim CZL_Account_Unique_ID As String = Request.QueryString("Account_Unique_ID")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_CZL_Licenced_Devices N'" & Device_Serial.Text &
                                                                  "', N'" & Device_ID.Text &
                                                                  "', N'" & Model.Text &
                                                                  "', N'" & AI_Software_Version.Text &
                                                                  "', N'" & R_Version.Text &
                                                                  "', N'" & Scale_SN.Text &
                                                                  "', N'" & Replace(MAC_Addr.Text, "-", ":") &
                                                                  "', N'" & Production_Licence_No.Text &
                                                                  "', N'" & EscapeChar(Location.Text) &
                                                                  "', N'" & DDL_CZL_Client_ID.SelectedValue &
                                                                  "', N'" & Effective_Date.Text &
                                                                  "', N'" & Unique_ID.Text &
                                                                  "', N'" & CZL_Account_Unique_ID & "' "

            '' Check database if the scale serial no exists
            Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Licenced_Devices WHERE Scale_SN = N'" & Scale_SN.Text & "'", "NoOfRecord")) > 0, True, False)
            If RecordExists And btnSaveCZLDevice.Text <> "Update" Then
                czldeviceserialexisterrormsg.Visible = True
                czldeviceserialexisterrormsg.Style.Add("display", "inline")
                popupCZLDevice.Show()
                TB_Scale_Serial_No.Focus()
            Else
                '' bypass the check when in edit mode
                '' As serial no field is disabled in edit mode, do not have to further validate the serial no if user change serial no which same as exsiting one.
                czldeviceserialexisterrormsg.Visible = False
                RunSQL(sqlStr)
            End If
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try


        '' Populate Formview
        PopulateFormViewData()

        '' When edit a device record
        If btnSaveCZLDevice.Text = "Update" Then
            '' Check and log the license properties properties change
            Dim IsDeviceAccountTransfered As Boolean = Log_Licence_Properties_Change(Unique_ID.Text, TB_Selected_Client_ID.Text, DDL_CZL_Client_ID.SelectedValue, TB_Selected_Model.Text, Model.Text)
            If Not IsDeviceAccountTransfered Then
                '' When editing existing device
                '' populate gridvide 1 (device list) based on current search
                '' populate gridview 2 (log list) based on selected device (unique id) in gridview 1
                PopulateGridViewData(TB_Search.Text)
                PopulateGridViewData1(Unique_ID.Text)

                '' Stay highligthed the row when click on Update
                Dim Selected_Row_Index As String = TB_Selected_Row_Index.Text
                Selected_Row_Index = IIf(Selected_Row_Index <> "", Selected_Row_Index, 0)
                GridView1.Rows(Selected_Row_Index).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

                '' display the device id as header
                ChangeLog_Header.Visible = True
                LB_Header_Serial_No.Visible = True
                'LB_Header_Serial_No.Text = Get_Value("SELECT ISNULL(Scale_SN, 'Unknown') AS Scale_Serial_No FROM CZL_Licenced_Devices WHERE Unique_ID = '" & Unique_ID.Text & "'", "Scale_Serial_No")
                LB_Header_Licence_Key.Visible = True
                'LB_Header_Licence_Key.Text = Get_Value("SELECT Licence_Code FROM LMS_Licence WHERE AI_Device_ID = '" & Device_ID.Text & "'", "Licence_Code")
            Else
                '' when device being transferred, then reset both gridview 1 and gridview 2
                PopulateGridViewData()
                PopulateGridViewData1()
            End If
        Else
            PopulateGridViewData()
            PopulateGridViewData1()
        End If

    End Sub

    Protected Sub Delete_CZL_Device_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "BEGIN TRANSACTION; " &
                                   "DELETE FROM CZL_Licenced_Devices WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; " &
                                   "DELETE FROM CZL_Device_Status_Log WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; " &
                                   "COMMIT; "

            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-populate all 3 databind when a device is deleted
        PopulateFormViewData()
        PopulateGridViewData()
        PopulateGridViewData1()
    End Sub




    '' Modal control - CZL Licenced Change Log (change log refer as comment in UI)
    Protected Sub Add_Change_Log_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddCZLChangeLog.Click
        ModalHeaderCZLChangeLog.Text = "Add Comment"
        btnSaveCZLChangeLog.Text = "Save"
        btnCancelCZLChangeLog.Text = "Cancel"

        TB_Selected_Button_Command_Name.Text = btnAddCZLChangeLog.CommandName   '' pass the button command name to hidden field
        TB_CZLChangeLog.Text = String.Empty
        TB_Selected_CZLChangeLog_ID.Text = String.Empty    '' Clear this hidden field when adding new
        popupCZLChangeLog.Show()
    End Sub

    Protected Sub Add_Change_Log_Batch_Click(sender As Object, e As EventArgs) Handles btnLogBatchUpdate.Click
        ModalHeaderCZLChangeLog.Text = "Add Comments (Batch)"
        btnSaveCZLChangeLog.Text = "Save"
        btnCancelCZLChangeLog.Text = "Cancel"

        TB_Selected_Button_Command_Name.Text = btnLogBatchUpdate.CommandName   '' pass the button command name to hidden field
        TB_CZLChangeLog.Text = String.Empty
        popupCZLChangeLog.Show()
    End Sub

    Protected Sub Edit_Change_Log_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderCZLChangeLog.Text = "Update Comment"
        btnSaveCZLChangeLog.Text = "Update"
        btnCancelCZLChangeLog.Text = "Cancel"

        '' Get the gridview row details
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        TB_CZLChangeLog.Text = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(1).Text))   '' Add the change log / comment to textbox

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")

        '' Pass value to hidden field when row command button is clicked
        TB_Selected_CZLChangeLog_ID.Text = EditLinkButtonCommandArgument(1)  '' Get change log ID from EditLinkButtonCommandArgument array index 1
        TB_Selected_Button_Command_Name.Text = "Single_Log"      '' when click on edit button should always be in Single_Log mode and update only 1 record

        popupCZLChangeLog.Show()
    End Sub

    Protected Sub Save_Change_Log_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveCZLChangeLog.Click
        Dim CZLChangeLog As TextBox = pnlAddEditCZLChangeLog.FindControl("TB_CZLChangeLog")
        Dim CZLChangeLog_Unique_ID As String = TB_Selected_CZLChangeLog_Unique_ID.Text
        Dim CZLChangeLog_ID As String = TB_Selected_CZLChangeLog_ID.Text
        Dim CZLAccount_Unique_ID As String = Request.QueryString("Account_Unique_ID")
        Dim By_Who As String = Session("User_Name")
        Dim SelectedButton As String = TB_Selected_Button_Command_Name.Text

        Try
            Dim sqlStr As String = Nothing

            '' Determine the store to process by Selected button (commandName)
            If SelectedButton = "Multiple_Log" Then
                sqlStr = "EXEC SP_CRUD_CZL_Log_Batch N'" & EscapeChar(CZLChangeLog.Text) & "', N'" & CZLAccount_Unique_ID & "', N'USR', N'" & By_Who & "' "
            Else
                sqlStr = "EXEC SP_CRUD_CZL_Log N'" & CZLChangeLog_ID & "', N'" & EscapeChar(CZLChangeLog.Text) & "', N'" & CZLChangeLog_Unique_ID & "', N'USR', N'" & By_Who & "' "
            End If

            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        popupCZLChangeLog.Hide()

        'PopulateFormViewData()   this cause modal of save change log/comment cannot close
        PopulateGridViewData(TB_Search.Text)
        PopulateGridViewData1(CZLChangeLog_Unique_ID)

        '' Selected row stay highlighted
        Dim Selected_Row_Index As TextBox = pnlAddEditCZLChangeLog.FindControl("TB_Selected_CZLChangeLog_Parent_Row_Index")
        GridView1.Rows(Selected_Row_Index.Text).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

    End Sub

    Protected Sub Delete_Change_Log_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)  '' Command button from Gridview2
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")
        Dim sqlStr As String = "DELETE FROM CZL_Device_Status_Log WHERE ID = " & DeleteLinkButtonCommandArgument(1)

        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' When a device log is delete, repopulate device with currenct seleted device and current selected log list
        PopulateFormViewData()
        PopulateGridViewData(TB_Search.Text)
        PopulateGridViewData1(TB_Selected_CZLChangeLog_Unique_ID.Text)

        '' Stay highligthed the row when click on Update contract
        Dim Selected_Row_Index As String = TB_Selected_CZLChangeLog_Parent_Row_Index.Text   '' Get the value from hidden field as the row index for CZL License devie (gridview1)
        Selected_Row_Index = IIf(Selected_Row_Index <> "", Selected_Row_Index, 0)
        GridView1.Rows(Selected_Row_Index).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
    End Sub



    '' Account Notes
    Protected Sub Add_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNotes.Click
        ModalHeaderNotes.Text = "Add Notes"
        btnSaveNotes.Text = "Save"
        btnCancelNotes.Text = "Cancel"

        TB_Notes.Text = String.Empty
        TB_Selected_Notes_ID.Text = "0"
        popupNotes.Show()
    End Sub

    Protected Sub Edit_Notes_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderNotes.Text = "Update Notes"
        btnSaveNotes.Text = "Update"
        btnCancelNotes.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_Selected_Notes_ID.Text = EditLinkButton.CommandArgument   '' Pass the value to hidden field

        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        TB_Notes.Text = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(1).Text))   '' Populate the value to textbox in edit mode

        popupNotes.Show()
    End Sub

    Protected Sub Save_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveNotes.Click
        Dim CZL_Account_Unique_ID As String = Request.QueryString("Account_Unique_ID")
        Dim Notes As TextBox = pnlAddEditNotes.FindControl("TB_Notes")
        Dim Notes_For As String = "CZL Account"
        Dim ID As String = TB_Selected_Notes_ID.Text

        Dim sqlStr As String = "EXEC SP_CRUD_CZL_Account_Notes N'" & CZL_Account_Unique_ID & "', N'" & Replace(Notes.Text, "'", "''") & "', N'" & Notes_For & "', '" & ID & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
        PopulateGridViewData1()   '' when no value passed, comment section will be hidden
        PopulateGridViewData2()
    End Sub

    Protected Sub Delete_Notes_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim sqlStr As String = "DELETE FROM CZL_Account_Notes WHERE ID = " & DeleteLinkButton.CommandArgument
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
        PopulateGridViewData1()   '' when no value passed, comment section will be hidden
        PopulateGridViewData2()
    End Sub



    '' Searchbox
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateFormViewData()
        PopulateGridViewData(TB_Search.Text)
        PopulateGridViewData1()
        PopulateGridViewData2()
    End Sub


    '' Function
    Protected Function Log_Licence_Properties_Change(ByVal Unique_ID As String, ByVal Old_Client_ID As String, ByVal New_Client_ID As String, ByVal Old_Model As String, ByVal New_Model As String) As Boolean
        Dim IsDeviceAccountTransfered As Boolean = False
        Dim By_Who As String = Session("User_Name")

        '' Check Client ID
        Dim Old_Client_Account As String = Get_Value("SELECT Client_ID + ' - ' + User_Group AS Client_Account FROM CZL_Account WHERE Client_ID = '" & Old_Client_ID & "'", "Client_Account")
        Dim New_Client_Account As String = Get_Value("SELECT Client_ID + ' - ' + User_Group AS Client_Account FROM CZL_Account WHERE Client_ID = '" & New_Client_ID & "'", "Client_Account")
        If New_Client_ID <> Old_Client_ID Then
            RunSQL("EXEC SP_CRUD_CZL_Log N'', N'Moved from account #" & EscapeChar(Old_Client_Account) & " to #" & EscapeChar(New_Client_Account) & "', N'" & Unique_ID & "', N'SYS', N'" & By_Who & "' ")
            IsDeviceAccountTransfered = True
        End If

        '' other checking properties

        Return IsDeviceAccountTransfered
    End Function

    Protected Function GetSQL(ByVal controlObj As Object, Optional ByVal Services_Group As String = Nothing, Optional ByVal keyword As String = Nothing, Optional ByVal filter As String = Nothing) As String
        Dim sqlStr As String = Nothing
        Dim objID As String = String.Empty
        Dim Search_Keyword As String = EscapeChar(keyword)

        ' Check if the object has an ID property or field
        Dim idProperty = controlObj.GetType().GetProperty("ID")
        If idProperty IsNot Nothing AndAlso idProperty.PropertyType Is GetType(String) Then
            objID = idProperty.GetValue(controlObj)?.ToString()
        End If

        ' Form SQL String based on control object ID
        Select Case objID
            Case "FormView1"
                sqlStr = "SELECT * FROM CZL_Account WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' "

            Case "GridView1"
                sqlStr = "SELECT A.Unique_ID, Device_Serial, Device_ID " &
                         "     , Scale_SN, Model, AI_Software_Version, R_Version, MAC_Addr " &
                         "     , ISNULL(dbo.Get_AI_Licence_Activation_Key(A.Device_ID), 'No Binding Key') AS Licence_Key " &
                         "     , Production_Licence_No " &
                         "     , Location, A.Created_Date, Last_Updated, Client_ID, CZL_Account_Unique_ID " &
                         "     , Effective_Date, COUNT(B.Unique_ID) AS Comment_Count " &
                         "FROM CZL_Licenced_Devices A " &
                         "LEFT JOIN CZL_Device_Status_Log B ON B.Unique_ID = A.Unique_ID " &
                         "WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' " &
                         "  AND Client_ID IN (SELECT Client_ID FROM CZL_Account WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "') " &
                         "  AND (Device_Serial Like '%" & Search_Keyword & "%' OR Replace(Device_ID, '-', '') LIKE '%" & Replace(Search_Keyword, "-", "") & "%' OR Scale_SN LIKE '%" & Search_Keyword & "%' OR Location LIKE '%" & Search_Keyword & "%') " &
                         "GROUP BY A.Unique_ID, Device_Serial, Device_ID, Scale_SN, Model, AI_Software_Version, R_Version, MAC_Addr, Production_Licence_No, Location, A.Created_Date, Last_Updated, Client_ID, CZL_Account_Unique_ID, Effective_Date "

            Case "GridView2"
                sqlStr = "SELECT * FROM R_CZL_Device_Status_Log WHERE Unique_ID = '" & Search_Keyword & "' ORDER BY Created_Date DESC, ID DESC "

            Case "GridView3"
                sqlStr = "SELECT *, CASE WHEN DATEDIFF(D, Added_Date, GETDATE()) > 90 THEN 1 ELSE 0 END AS Is_Locked " &
                         "FROM CZL_Account_Notes " &
                         "WHERE CZL_Account_Unique_ID = '" & Request.QueryString("Account_Unique_ID") & "' " &
                         "  AND Notes_For = 'CZL Account' ORDER BY Added_Date DESC, ID DESC "
            Case "DDL_CZL_Client_ID"
                sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group FROM CZL_Account ORDER BY CAST(Client_ID AS int) "

        End Select

        Return sqlStr
    End Function



    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/CZL_Licensed_Device.aspx")
    End Sub


End Class
