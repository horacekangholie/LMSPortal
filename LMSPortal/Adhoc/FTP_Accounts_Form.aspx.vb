
Partial Class Adhoc_FTP_Accounts_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            If Request.QueryString("Code") <> "" Then
                FormView1.ChangeMode(FormViewMode.Edit)
                PageTitle = "Update Account"
                BT_Create.Text = "Update"
                BT_Create.CommandName = "Update"
                UpdatePanel1.Visible = True
                UpdatePanel2.Visible = True
            Else
                FormView1.ChangeMode(FormViewMode.Insert)
                PageTitle = "New Account"
                BT_Create.Text = "Create"
                BT_Create.CommandName = "Create"
                UpdatePanel1.Visible = False
                UpdatePanel2.Visible = False
            End If
            LB_PageTitle.Text = PageTitle
            PopulateFormViewData()
        End If
        PopulateGridViewData()


        '' Hide the Add access folder button when there is no FTP user account
        btnAddAccessFolder.Visible = IIf(GridView1.Rows.Count > 0 AndAlso Session("Login_Name") = "administrator", True, False)

        '' Hide the Add access folder button when there is no row selected
        btnAddAccessFolder.Visible = IIf(TB_Selected_Row_Index.Text <> "" AndAlso Session("Login_Name") = "administrator", True, False)


        '' Hide add ftp user and access folder button when it is not administrator
        btnAddFTPUser.Visible = IIf(Session("Login_Name") <> "administrator", False, True)

    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr = " SELECT * FROM FTP_Server_Distributor WHERE Code = '" & Request.QueryString("Code") & "' "
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error111:  " & ex.Message)
        End Try

        '' Disable Code field when it is in edit mode
        CType(FormView1.FindControl("TB_Code"), TextBox).Enabled = IIf(BT_Create.CommandName = "Update", False, True)

    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {"SELECT * FROM R_FTP_Server_Distributor_Account WHERE Code = '" & Request.QueryString("Code") & "' ORDER BY [Group], [Contact Person]"}

            BuildGridView(GridView1, "GridView1", "ID")
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
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 50
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Group", "Contact Person", "Email", "User ID", "Password", "Status", "Last Updated"}
                Dim ColSize() As Integer = {100, 200, 200, 100, 100, 50, 50}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Updated") Then
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
                GridView1.Columns.Add(TField)

            Case "GridView2"
                '' Build GridView Content
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 50
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Path", "Folder"}
                Dim ColSize() As Integer = {200, 50}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

        End Select
    End Sub


    '' Formview control
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
    End Sub


    '' Gridview control
    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Add badge style to ftp user status
            Dim UserStatus As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text
            If UserStatus <> "Active" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge rounded-pill bg-danger'>" & UserStatus & "</span>"
            Else
                e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge rounded-pill bg-success'>" & UserStatus & "</span>"
            End If

            ' Define the control button for FTP account
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' 01. Edit control
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The column where the edit and delete button are
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)

            If Session("Login_Name") <> "administrator" Then
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("User ID") & "|" & drv("Code") & "_" & drv("User ID") & "|" & drv("Contact Person") & "|" & drv("Status")
                EditLinkButton.CausesValidation = False
                EditLinkButton.ToolTip = "Locked"
                EditLinkButton.Enabled = False
            Else
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("User ID") & "|" & drv("Code") & "_" & drv("User ID") & "|" & drv("Contact Person") & "|" & drv("Status")
                EditLinkButton.CausesValidation = False
                EditLinkButton.Enabled = True
            End If
            AddHandler EditLinkButton.Click, AddressOf Edit_FTPUser_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button


            '' 02. View Path Control
            Dim ViewPathCellIndex As Integer = e.Row.Cells.Count - 1  '' The column where the edit and delete button are
            Dim ViewPathLink As LinkButton = TryCast(e.Row.Cells(ViewPathCellIndex).Controls(1), LinkButton)

            If Trim(drv("Status")) <> "Active" Then
                ViewPathLink.Text = "<i class='bi bi-list-ul'></i>"
                ViewPathLink.CssClass = "btn btn-xs btn-light disabled"
                ViewPathLink.CommandArgument = e.Row.RowIndex & "|" & drv("User ID") & "|" & drv("Code") & "_" & drv("User ID") & "|" & drv("Contact Person") & "|" & drv("Status")
                ViewPathLink.ToolTip = "Locked"
                ViewPathLink.Enabled = False
            Else
                ViewPathLink.Text = "<i class='bi bi-list-ul'></i>"
                ViewPathLink.CssClass = "btn btn-xs btn-info"
                ViewPathLink.CommandArgument = e.Row.RowIndex & "|" & drv("User ID") & "|" & drv("Code") & "_" & drv("User ID") & "|" & drv("Contact Person") & "|" & drv("Status")
            End If
            AddHandler ViewPathLink.Click, AddressOf View_PathLink_Click

        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        Dim eCommandArgument As Array = Split(e.CommandArgument, "|")  '' pass eCmmandArgument to array
        Dim Selected_row_of_FTP_User As Integer = Convert.ToInt32(eCommandArgument(0))
        Dim FTP_User_ID As String = eCommandArgument(1)
        Dim Access_List_UID As String = eCommandArgument(2)
        Dim Contact_Person As String = eCommandArgument(3)
        Dim Status As String = eCommandArgument(4)

        '' Change the row color of the selected row
        Dim rowIndex As Integer = Selected_row_of_FTP_User
        For Each row As GridViewRow In GridView1.Rows
            row.BackColor = If(row.RowIndex.Equals(rowIndex), Drawing.ColorTranslator.FromHtml("#eeeeee"), System.Drawing.Color.Transparent)
        Next

        '' Get the commandargument when command button is clicked
        TB_Selected_Row_Index.Text = eCommandArgument(0)          '' pass the selected row index to hidden field
        TB_Selected_FTP_User.Text = eCommandArgument(1)           '' pass the FTP User ID to hidden field
        TB_Selected_Access_List_UID.Text = eCommandArgument(2)    '' pass the Access_List_UID to hidden field
        TB_Selected_Contact_Person.Text = eCommandArgument(3)     '' pass the contact person to hidden field  

        '' When control button gridview1 row clicked, display the access folder in gridview2
        Dim sqlStr As String = "SELECT * FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & eCommandArgument(2) & "' ORDER BY Folder"
        BuildGridView(GridView2, "GridView2", "ID")
        GridView2.DataSource = GetDataTable(sqlStr)
        GridView2.DataBind()

        '' Hide the Add access folder button when it is an inactive FTP User
        btnAddAccessFolder.Visible = IIf(Status = "Active" AndAlso Session("Login_Name") = "administrator", True, False)
        GridView2.Visible = IIf(Status = "Active", True, False)

    End Sub



    '' Validate Code to prevent duplicated Code
    Protected Sub TB_Code_TextChanged(sender As Object, e As EventArgs)
        Dim Code As String = CType(FormView1.FindControl("TB_Code"), TextBox).Text
        Dim RecordCount As Integer = Get_Value("Select COUNT(Code) AS RecordCount FROM FTP_Server_Distributor WHERE Code = '" & Code & "'", "RecordCount")

        If RecordCount > 0 Then
            CType(FormView1.FindControl("TB_Code"), TextBox).Focus()
            Response.Write(AlertMessage("Distributor code " & Code & " exists, please use another code."))
        End If
    End Sub



    '' Modal control

    '' FTP User
    Protected Sub TB_Contact_Person_TextChanged(sender As Object, e As EventArgs) Handles TB_Contact_Person.TextChanged
        Dim User_Group As DropDownList = pnlAddEditFTPUser.FindControl("DDL_User_Group")
        Dim Code As TextBox = FormView1.FindControl("TB_Code")
        Dim Company_Abbreviation As TextBox = FormView1.FindControl("TB_Company_Abbreviation")
        Dim Contact_Person As TextBox = pnlAddEditFTPUser.FindControl("TB_Contact_Person")
        Dim Contact_Person_Name_Array As String() = Contact_Person.Text.Split(" "c)
        Dim NameInitial As String = ""

        For i = 0 To Contact_Person_Name_Array.Length - 1
            If (Contact_Person_Name_Array.Length - 1) < 1 Then
                NameInitial = Contact_Person_Name_Array(i).Substring(0, 2) '' if name only have one word, then take first 2 characters
            Else
                NameInitial += Contact_Person_Name_Array(i).Substring(0, 1)   '' Get initial of each word and join them
            End If
        Next

        '' Form the FTP user id format and assign to the field
        TB_FTP_User.Text = (User_Group.Text.Substring(0, 1) & Right(Code.Text, 2) & Company_Abbreviation.Text.Substring(0, 1) & NameInitial).ToLower()

        popupFTPUser.Show()
    End Sub

    Protected Sub DDL_User_Group_Load(sender As Object, e As EventArgs) Handles DDL_User_Group.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_1 AS [User Group] FROM DB_Lookup WHERE Lookup_Name = 'FTP User Group' "

                DDL_User_Group.DataSource = GetDataTable(sqlStr)
                DDL_User_Group.DataTextField = "User Group"
                DDL_User_Group.DataValueField = "User Group"
                DDL_User_Group.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_User_Group_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_User_Group.SelectedIndexChanged
        Dim User_Group As DropDownList = pnlAddEditFTPUser.FindControl("DDL_User_Group")
        Dim Code As TextBox = FormView1.FindControl("TB_Code")
        Dim Company_Abbreviation As TextBox = FormView1.FindControl("TB_Company_Abbreviation")
        Dim Contact_Person As TextBox = pnlAddEditFTPUser.FindControl("TB_Contact_Person")
        Dim Contact_Person_Name_Array As String() = Contact_Person.Text.Split(" "c)
        Dim NameInitial As String = ""

        For i = 0 To Contact_Person_Name_Array.Length - 1
            If (Contact_Person_Name_Array.Length - 1) < 1 Then
                NameInitial = Contact_Person_Name_Array(i).Substring(0, 2) '' if name only have one word, then take first 2 characters
            Else
                NameInitial += Contact_Person_Name_Array(i).Substring(0, 1)   '' Get initial of each word and join them
            End If
        Next

        '' Form the FTP user id format and assign to the field
        TB_FTP_User.Text = (User_Group.Text.Substring(0, 1) & Right(Code.Text, 2) & Company_Abbreviation.Text.Substring(0, 1) & NameInitial).ToLower()

        popupFTPUser.Show()
    End Sub


    Protected Sub Add_FTPUser_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddFTPUser.Click
        ModalHeaderFTPUser.Text = "Add FTP User"
        btnSaveFTPUser.Text = "Save"
        btnCancelFTPUser.Text = "Cancel"

        '' Initialize field
        TB_Contact_Person.Text = String.Empty
        TB_Email.Text = String.Empty
        TB_FTP_User.Text = String.Empty
        TB_FTP_Password.Text = GenerateFTPPassword()

        Dim DDL_Status As DropDownList = pnlAddEditFTPUser.FindControl("DDL_Status")
        DDL_Status.SelectedIndex = DDL_Status.Items.IndexOf(DDL_Status.Items.FindByText("Active"))

        DDL_User_Group.Enabled = True
        TB_Contact_Person.Enabled = True

        popupFTPUser.Show()
    End Sub

    Protected Sub Edit_FTPUser_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderFTPUser.Text = "Edit FTP User"
        btnSaveFTPUser.Text = "Update"
        btnCancelFTPUser.Text = "Cancel"

        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)

        '' User Group field
        Dim DDL_User_Group As DropDownList = pnlAddEditFTPUser.FindControl("DDL_User_Group")
        DDL_User_Group.SelectedIndex = DDL_User_Group.Items.IndexOf(DDL_User_Group.Items.FindByText(HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(0).Text))))
        DDL_User_Group.Enabled = False

        '' Contact Person field
        Dim Contact_Person As String = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(1).Text))
        TB_Contact_Person.Text = Contact_Person
        TB_Contact_Person.Enabled = False

        '' Email field
        Dim Email As String = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(2).Text))
        TB_Email.Text = Email

        '' FTP User field
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim eCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        ' TB_FTP_User.Text = HttpUtility.HtmlDecode(row.Cells(3).Text)     '' Get FTP User ID from Gridview1 cell 3
        TB_FTP_User.Text = eCommandArgument(1)                             '' Get FTP User ID from edit link button command argument (array)
        TB_FTP_User.Enabled = False

        '' FTP Password field
        Dim FTP_Password As String = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(4).Text))
        TB_FTP_Password.Text = FTP_Password

        '' Status dropdownlist
        Dim DDL_Status As DropDownList = pnlAddEditFTPUser.FindControl("DDL_Status")
        Dim Status As String = IIf(Trim(HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(5).Text))).Contains("Inactive"), "Inactive", "Active")
        DDL_Status.SelectedIndex = DDL_Status.Items.IndexOf(DDL_Status.Items.FindByText(Status))

        popupFTPUser.Show()
    End Sub

    Protected Sub btnSaveFTPUser_Click(sender As Object, e As EventArgs) Handles btnSaveFTPUser.Click
        Dim User_Group As DropDownList = pnlAddEditFTPUser.FindControl("DDL_User_Group")
        Dim Contact_Person As TextBox = pnlAddEditFTPUser.FindControl("TB_Contact_Person")
        Dim Email As TextBox = pnlAddEditFTPUser.FindControl("TB_Email")
        Dim User_ID As TextBox = pnlAddEditFTPUser.FindControl("TB_FTP_User")
        Dim User_Password As TextBox = pnlAddEditFTPUser.FindControl("TB_FTP_Password")
        Dim Status As DropDownList = pnlAddEditFTPUser.FindControl("DDL_Status")
        Dim Code As TextBox = FormView1.FindControl("TB_Code")

        Try
            Dim sqlStr As String = " EXEC SP_CRUD_FTP_Users '" & User_Group.Text &
                                                         "', N'" & Contact_Person.Text &
                                                         "', N'" & Email.Text &
                                                         "', N'" & User_ID.Text &
                                                         "', N'" & User_Password.Text &
                                                         "', N'" & Status.SelectedValue &
                                                         "', N'" & Code.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try


        '' Rebind the dropdownlist after a new FTP user created
        Try
            Dim sqlStr As String = " SELECT Contact_Person + ' - ' + User_ID AS [FTP User], Access_List_UID AS [UID] " &
                                       " FROM FTP_Server_Distributor_Account WHERE Code = '" & Request.QueryString("Code") & "' AND Is_Active = 1 " &
                                       " ORDER BY User_Group, Contact_Person "

            DDL_FTP_User.DataSource = GetDataTable(sqlStr)
            DDL_FTP_User.DataTextField = "FTP User"
            DDL_FTP_User.DataValueField = "UID"
            DDL_FTP_User.Items.Clear()  '' clear item and rebind again
            DDL_FTP_User.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()

        If btnSaveFTPUser.Text = "Update" Then
            '' Stay highligthed the row when click on Update FTP User
            Dim Selected_Row_Index As TextBox = pnlAddEditAccessFolder.FindControl("TB_Selected_Row_Index")
            GridView1.Rows(Selected_Row_Index.Text).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

            '' Pass Contract person name to access folder header
            FTP_User_Name.Text = " - " & Contact_Person.Text

            '' Hide the Add access folder button when it is an inactive FTP User
            btnAddAccessFolder.Visible = IIf(Status.SelectedValue = 0, False, True)
            GridView2.Visible = IIf(Status.SelectedValue = 0, False, True)
        Else
            '' Hide the Add access folder button when there is no row selected
            btnAddAccessFolder.Visible = False
            GridView2.Visible = False
            FTP_User_Name.Text = String.Empty
        End If
    End Sub

    Protected Sub btnCancelFTPUser_Click(sender As Object, e As EventArgs) Handles btnCancelFTPUser.Click
        '' hightlight row after click on cancel button in edit mode
        If btnSaveFTPUser.Text = "Update" Then
            '' Stay highlighted when click on Cancel button on Edit FTP User
            Dim Selected_Row_Index As TextBox = pnlAddEditAccessFolder.FindControl("TB_Selected_Row_Index")
            GridView1.Rows(CInt(Selected_Row_Index.Text)).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

            '' Pass Contract person name to access folder header
            Dim Contact_Person As TextBox = pnlAddEditAccessFolder.FindControl("TB_Selected_Contact_Person")
            FTP_User_Name.Text = " - " & Contact_Person.Text

            '' Hide the Add access folder button when it is an inactive FTP User
            Dim Status As DropDownList = pnlAddEditFTPUser.FindControl("DDL_Status")
            btnAddAccessFolder.Visible = IIf(Status.SelectedValue = 0, False, True)
            GridView2.Visible = IIf(Status.SelectedValue = 0, False, True)
        Else
            '' Hide the Add access folder button when there is no row selected
            btnAddAccessFolder.Visible = False
            GridView2.Visible = False
            FTP_User_Name.Text = String.Empty
        End If

    End Sub



    '' FTP Access Folder
    Protected Sub DDL_FTP_User_Load(sender As Object, e As EventArgs) Handles DDL_FTP_User.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Contact_Person + ' - ' + User_ID AS [FTP User], Access_List_UID AS [UID] " &
                                       " FROM FTP_Server_Distributor_Account WHERE Code = '" & Request.QueryString("Code") & "' AND Is_Active = 1 " &
                                       " ORDER BY User_Group, Contact_Person "

                DDL_FTP_User.DataSource = GetDataTable(sqlStr)
                DDL_FTP_User.DataTextField = "FTP User"
                DDL_FTP_User.DataValueField = "UID"
                DDL_FTP_User.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_FTP_User_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_FTP_User.SelectedIndexChanged
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")

        PopulateAccessFolder(DDL_FTP_User.SelectedValue)  '' repopulate the access folder list when each selected
        PopulateListbox(DDL_FTP_User.SelectedValue)
        popupAccessFolder.Show()
    End Sub

    Protected Sub DDL_Access_Folder_Load(sender As Object, e As EventArgs) Handles DDL_Access_Folder.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_2 AS [Folder], Value_1 AS [Path] FROM DB_Lookup WHERE Lookup_Name = 'Tech Support FTP Folder' ORDER BY Value_2 "

                DDL_Access_Folder.DataSource = GetDataTable(sqlStr)
                DDL_Access_Folder.DataTextField = "Folder"
                DDL_Access_Folder.DataValueField = "Path"
                DDL_Access_Folder.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub Add_AccessFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddAccessFolder.Click
        ModalHeaderAccessFolder.Text = "Add Access Folder"
        btnClosePanel.Text = "Close"

        '' Select the user in dropdownlist based on the row selected in gridview1
        Dim Selected_FTP_User As String = TB_Selected_Access_List_UID.Text   '' get the value from hidden field
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")
        Dim i = DDL_FTP_User.Items.IndexOf(DDL_FTP_User.Items.FindByValue(Selected_FTP_User))
        i = IIf(i < 0, 0, i)
        DDL_FTP_User.SelectedIndex = i

        PopulateAccessFolder(DDL_FTP_User.SelectedValue)   '' repopulate the access folder list when each selected
        PopulateListbox(DDL_FTP_User.SelectedValue)        '' repopulate Gridview2 (access folder list) in listbox
        popupAccessFolder.Show()
    End Sub

    Protected Sub AddAccessFolder_Click(sender As Object, e As EventArgs) Handles AddAccessFolder.Click
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")
        Dim CodeArray As String() = Split(DDL_FTP_User.SelectedValue, "_")
        Dim Code As String = CodeArray(0)
        Dim User_ID As String = CodeArray(1)
        Dim DDL_Access_Folder As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_Access_Folder")

        Try
            Dim sqlStr As String = " EXEC SP_CRUD_FTP_Folder_Access_List N'" & Trim(DDL_FTP_User.SelectedValue) &
                                                         "', N'" & Trim(User_ID) &
                                                         "', N'" & Code &
                                                         "', N'" & DDL_Access_Folder.SelectedValue &
                                                         "', N'" & DDL_Access_Folder.SelectedItem.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateAccessFolder(DDL_FTP_User.SelectedValue)   '' repopulate the access folder list when each selected
        PopulateListbox(DDL_FTP_User.SelectedValue)
        popupAccessFolder.Show()
    End Sub

    Protected Sub View_PathLink_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim ViewPathLink As LinkButton = TryCast(sender, LinkButton)
        Dim eCommandArgument As Array = Split(ViewPathLink.CommandArgument, "|")  '' pass eCmmandArgument to array
        Dim Selected_row_of_FTP_User As Integer = Convert.ToInt32(eCommandArgument(0))
        Dim FTP_User_ID As String = eCommandArgument(1)
        Dim Access_List_UID As String = eCommandArgument(2)
        Dim Contact_Person As String = eCommandArgument(3)

        FTP_User_Name.Text = " - " & eCommandArgument(3)   '' Get the FTP user name portion from array and display ftp user name based on the record clicked.
    End Sub

    Protected Sub btnClearAccessFolder_Click(sender As Object, e As EventArgs) Handles btnClearAccessFolder.Click
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")
        Try
            Dim sqlStr As String = " DELETE FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & DDL_FTP_User.SelectedValue & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateAccessFolder(DDL_FTP_User.SelectedValue)    '' repopulate the access folder list when each selected
        PopulateListbox(DDL_FTP_User.SelectedValue)
        popupAccessFolder.Show()
    End Sub

    Protected Sub btnClosePanel_Click(sender As Object, e As EventArgs) Handles btnClosePanel.Click
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")
        Dim sqlStr As String = "SELECT * FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & DDL_FTP_User.SelectedValue & "' ORDER BY Folder"

        '' When control button gridview1 row clicked, display the access folder in gridview2
        BuildGridView(GridView2, "GridView2", "ID")
        GridView2.DataSource = GetDataTable(sqlStr)
        GridView2.DataBind()

        '' Stay highlist the record when close the Access Folder panel
        Dim Selected_Row_Index As TextBox = pnlAddEditAccessFolder.FindControl("TB_Selected_Row_Index")
        If Selected_Row_Index.Text <> "" Then
            GridView1.Rows(Selected_Row_Index.Text).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
        End If

        Dim Contact_Person As TextBox = pnlAddEditAccessFolder.FindControl("TB_Selected_Contact_Person")
        FTP_User_Name.Text = " - " & Contact_Person.Text

    End Sub


    Protected Sub PopulateListbox(Optional ByVal Access_List_UID As String = Nothing)
        '' Populate the list of access folder in listbox
        Try
            Dim sqlStr As String = " SELECT * FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & Access_List_UID & "' "

            GridView_Access_Folder.DataSource = GetDataTable(sqlStr)
            GridView_Access_Folder.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateAccessFolder(Optional ByVal Access_List_UID As String = Nothing)
        '' Populate the list of available FTP folder in the dropdownlist
        Dim DDL_FTP_User As DropDownList = pnlAddEditAccessFolder.FindControl("DDL_FTP_User")
        Try
            Dim sqlStr As String = " SELECT Value_2 AS [Folder], Value_1 AS [Path] FROM DB_Lookup " &
                                   " WHERE Lookup_Name = 'Tech Support FTP Folder' " &
                                   "   AND Value_2 NOT IN (SELECT Folder FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & DDL_FTP_User.SelectedValue & "') ORDER BY Value_2 "

            DDL_Access_Folder.DataSource = GetDataTable(sqlStr)
            DDL_Access_Folder.DataTextField = "Folder"
            DDL_Access_Folder.DataValueField = "Path"
            DDL_Access_Folder.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub GridView_Access_Folder_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_Access_Folder.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim DeleteLineItem As LinkButton = e.Row.FindControl("lnkDeleteLineItem")
            DeleteLineItem.Text = "<i class='bi bi-trash'></i>"
            DeleteLineItem.CssClass = "btn btn-xs btn-danger"
            DeleteLineItem.CausesValidation = False
            AddHandler DeleteLineItem.Click, AddressOf DeleteLineItem_Click
        End If
    End Sub

    Protected Sub DeleteLineItem_Click(sender As Object, e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim Access_List_UID As String = DDL_FTP_User.SelectedValue
        Dim FolderName As String = HttpUtility.HtmlDecode(row.Cells(1).Text)

        Try
            Dim sqlStr As String = "DELETE FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = '" & Access_List_UID & "' AND Folder = '" & FolderName & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Record fail to delete.');", True)
        End Try

        PopulateAccessFolder(DDL_FTP_User.SelectedValue)   '' repopulate the access folder list when each selected
        PopulateListbox(DDL_FTP_User.SelectedValue)
        popupAccessFolder.Show()
    End Sub



    '' Control for creating or cancel FTP Account profile
    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Dim Code As String = IIf(Request.QueryString("Code") Is Nothing, "", CType(FormView1.FindControl("TB_Code"), TextBox).Text)
        Dim Distributor As String = CType(FormView1.FindControl("TB_Distributor"), TextBox).Text
        Dim Country As String = CType(FormView1.FindControl("TB_Country"), TextBox).Text
        Dim Currency As String = CType(FormView1.FindControl("TB_Currency"), TextBox).Text
        Dim Company_Abbreviation As String = CType(FormView1.FindControl("TB_Company_Abbreviation"), TextBox).Text
        Dim Region As String = CType(FormView1.FindControl("TB_Region"), TextBox).Text
        Dim Tech_Support_Representative As String = CType(FormView1.FindControl("TB_TS_Rep"), TextBox).Text
        Dim Marketing_Representative As String = CType(FormView1.FindControl("TB_MKT_Rep"), TextBox).Text
        Dim Nature As String = CType(FormView1.FindControl("TB_Nature"), TextBox).Text
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)

        Try
            Dim sqlStr As String = " EXEC SP_CRUD_FTP_Account_Users N'" & Code & "' " &
                                                ", N'" & EscapeChar(Distributor) & "' " &
                                                ", N'" & EscapeChar(Country) & "' " &
                                                ", N'" & Currency & "' " &
                                                ", N'" & Company_Abbreviation & "' " &
                                                ", N'" & Region & "' " &
                                                ", N'" & Tech_Support_Representative & "' " &
                                                ", N'" & Marketing_Representative & "' " &
                                                ", N'" & Nature & "' " &
                                                ", N'" & BtnCommand.CommandName & "' "

            RunSQL(sqlStr)
            Response.Write(AlertMessageRedirect("User account for " & Replace(Distributor, "'", "\'") & " " & BtnCommand.Text.ToLower & "d.", "/Adhoc/FTP_Accounts.aspx"))
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

    End Sub

    Protected Sub BT_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Cancel.Click
        Response.Redirect("~/Adhoc/FTP_Accounts.aspx")
    End Sub

End Class
