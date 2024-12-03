
Imports System.Globalization

Partial Class Views_CZL_Unregistered_Licence
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Licence w/o Registered Account"

    Dim reportColName As String() = {"Licensee", "Licence Code", "Created Date", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Activated Date", "Expired Date", "Requested By", "Remarks"}
    Dim reportColSize As Integer() = {150, 80, 50, 50, 50, 100, 100, 50, 50, 100, 300}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT * FROM R_Activated_AI_Licence " &
                                   "WHERE [AI Device ID] NOT IN (SELECT Device_ID FROM CZL_Licenced_Devices) " &
                                   "AND LEN([Serial No]) > 0 "
            sqlStr += "AND ([Licensee] LIKE '%" & keyword & "%' OR [Licence Code] LIKE '%" & keyword & "%' OR [Serial No] LIKE '%" & keyword & "%' OR [MAC Address] LIKE '%" & keyword & "%' OR [AI Device ID] LIKE '%" & keyword & "%' OR [AI Device Serial No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%') "
            sqlStr += "ORDER BY [Licensee], [Activated Date] DESC, [Serial No] "

            ''Response.Write(sqlStr)

            '' Formatting GridView
            BuildGridView()

            '' Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        ' Get the total record
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
        GridView1.DataKeyNames = New String() {"Customer ID"}
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
    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            'Dim drv As System.Data.DataRowView = e.Row.DataItem
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub



    '' Modal
    Protected Sub DDL_CZL_Client_ID_Load(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group " &
                             "FROM CZL_Account " &
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

    Protected Sub Assign_Unregistered_Device_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderUnregisteredLDevice.Text = "Assign Device to Account"
        btnAssignUnregisteredDevice.Text = "Assign"
        btnCancelUnregisteredDevice.Text = "Cancel"

        '' Get row command argument and populate to hidden field in modal
        Dim AssignLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim AssignLinkButtonCommandArgument As Array = Split(AssignLinkButton.CommandArgument, "|")

        TB_Hidden_Selected_Row_Index.Text = AssignLinkButtonCommandArgument(0)
        TB_Hidden_Selected_AI_Device_Serial_No.Text = AssignLinkButtonCommandArgument(1)
        TB_Hidden_Selected_AI_Device_ID.Text = AssignLinkButtonCommandArgument(2)
        TB_Hidden_Selected_Serial_No.Text = AssignLinkButtonCommandArgument(3)
        TB_Hidden_Selected_MAC_Address.Text = AssignLinkButtonCommandArgument(4)
        TB_Hidden_Selected_Customer_ID.Text = AssignLinkButtonCommandArgument(5)
        TB_Hidden_Selected_Licensee.Text = EscapeChar(AssignLinkButtonCommandArgument(6))


        '' Repopulate the dropdownlist option
        Dim DDL_CZL_Client_ID As DropDownList = pnlAssignUnregisteredLDevice.FindControl("DDL_CZL_Client_ID")

        Try
            Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group " &
                         "FROM CZL_Account " &
                         "WHERE By_Distributor ='" & TB_Hidden_Selected_Customer_ID.Text & "' " &
                         "ORDER BY CAST(Client_ID AS int) "

            '' Clear the dropdownlist item and append an bound item
            DDL_CZL_Client_ID.Items.Clear()
            DDL_CZL_Client_ID.Items.Add(New ListItem("Please select", "-1"))

            DDL_CZL_Client_ID.DataSource = GetDataTable(sqlStr)
            DDL_CZL_Client_ID.DataTextField = "Client_ID_User_Group"
            DDL_CZL_Client_ID.DataValueField = "Client_ID"
            DDL_CZL_Client_ID.DataBind()

            DDL_CZL_Client_ID.SelectedIndex = -1   '' Set dropdownlist to always select appended items "Please select"

            '' Displya guided message to user based on the available czl account
            If DDL_CZL_Client_ID.Items.Count > 1 Then
                DDL_CZL_Client_ID.Enabled = True
                AssignGuidedMessage.Text = "Please select an CZL account from following list to assign the device."
                AssignGuidedMessage.CssClass = "text-muted"
            Else
                DDL_CZL_Client_ID.Enabled = False
                AssignGuidedMessage.Text = "<b>" & TB_Hidden_Selected_Licensee.Text & "</b> has no registered CZL account."
                AssignGuidedMessage.CssClass = "text-danger"
            End If

        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        popupAssignUnregisteredLDevice.Show()
    End Sub

    Protected Sub Save_Assign_Unregistered_Device_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAssignUnregisteredDevice.Click
        Dim Device_Serial As TextBox = TB_Hidden_Selected_AI_Device_Serial_No
        Dim Device_ID As TextBox = TB_Hidden_Selected_AI_Device_ID
        Dim Model As String = Nothing
        Dim AI_Software_Version As String = Nothing
        Dim R_Version As String = Nothing
        Dim Scale_SN As TextBox = TB_Hidden_Selected_Serial_No
        Dim MAC_Addr As TextBox = TB_Hidden_Selected_MAC_Address
        Dim Production_Licence_No As String = Nothing
        Dim Location As String = Nothing
        Dim DDL_CZL_Client_ID As DropDownList = pnlAssignUnregisteredLDevice.FindControl("DDL_CZL_Client_ID")
        Dim Effective_Date As String = String.Format("{0:yyyy-MM-dd}", Today)
        Dim Unique_ID As String = Nothing
        Dim CZL_Account_Unique_ID As String = Get_Value("SELECT CZL_Account_Unique_ID FROM CZL_Account WHERE Client_ID = '" & DDL_CZL_Client_ID.SelectedValue & "'", "CZL_Account_Unique_ID")


        '' Check if the scale serial number exist in database
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Licenced_Devices WHERE Scale_SN = N'" & Scale_SN.Text & "'", "NoOfRecord")) > 0, True, False)
        If Not RecordExists Then
            Try
                '' 01. Assign unregistered device to account
                Dim sqlStr As String = "EXEC SP_CRUD_CZL_Licenced_Devices N'" & Device_Serial.Text &
                                                                      "', N'" & Device_ID.Text &
                                                                      "', N'" & Model &
                                                                      "', N'" & AI_Software_Version &
                                                                      "', N'" & R_Version &
                                                                      "', N'" & Scale_SN.Text &
                                                                      "', N'" & Replace(MAC_Addr.Text, "-", ":") &
                                                                      "', N'" & Production_Licence_No &
                                                                      "', N'" & Location &
                                                                      "', N'" & DDL_CZL_Client_ID.SelectedValue &
                                                                      "', N'" & Effective_Date &
                                                                      "', N'" & Unique_ID &
                                                                      "', N'" & CZL_Account_Unique_ID & "' "

                RunSQL(sqlStr)


                '' 02. Log the assign activity
                Dim CZLAccount_Unique_ID As String = Get_Value("SELECT TOP 1 Unique_ID FROM CZL_Licenced_Devices WHERE Scale_SN = '" & Scale_SN.Text & "' ORDER BY ID DESC", "Unique_ID")
                Dim By_Who As String = Session("User_Name")
                Dim sqlStr1 As String = "EXEC SP_CRUD_CZL_Log N'', N'Device first assign to CZL account#" & DDL_CZL_Client_ID.SelectedValue & "', N'" & CZLAccount_Unique_ID & "', N'SYS', N'" & By_Who & "' "

                RunSQL(sqlStr1)

                ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Device serial no " & Scale_SN.Text & " has been assigned to account " & DDL_CZL_Client_ID.SelectedItem.Text & "');", True)
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try
        Else
            Dim CZL_Client_Account As String = Get_Value("SELECT Client_ID + ' - ' + User_Group AS CZL_Client_Account FROM CZL_Account WHERE Client_ID = (SELECT Client_ID FROM CZL_Licenced_Devices WHERE Scale_SN = '" & Scale_SN.Text & "')", "CZL_Client_Account")
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Device serial no " & Scale_SN.Text & " exists in account " & CZL_Client_Account & "');", True)
        End If

        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub Cancel_Assign_UnregisteredDevice_Click(sender As Object, e As EventArgs) Handles btnCancelUnregisteredDevice.Click
        '' When cancel button is click, highlight the record that was being selected
        Dim Selected_Row_Index As String = TB_Hidden_Selected_Row_Index.Text
        Selected_Row_Index = IIf(Selected_Row_Index <> "", Selected_Row_Index, 0)

        For i = 0 To GridView1.Rows.Count - 1
            GridView1.Rows(i).BackColor = IIf(i = Selected_Row_Index, Drawing.ColorTranslator.FromHtml("#eeeeee"), Drawing.Color.Transparent)
        Next
    End Sub



    '' Search record
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub

End Class
