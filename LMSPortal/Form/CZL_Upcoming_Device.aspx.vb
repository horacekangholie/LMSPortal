
Imports System.Data

Partial Class Form_CZL_Upcoming_Device
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Add AI Device"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
        End If
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT * FROM CZL_Licenced_Devices WHERE Client_ID IS NULL "
            sqlStr += "AND (Device_Serial LIKE '%" & keyword & "%' OR Device_ID LIKE '%" & keyword & "%' OR Scale_SN LIKE '%" & keyword & "%' OR Location LIKE '%" & keyword & "%') "
            sqlStr += "ORDER BY ID DESC "

            BuildGridView()
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        If keyword <> "" AndAlso CInt(GridView1.Rows.Count.ToString()) > 0 Then
            FooterTotalCount.Visible = True
            FooterTotalCount.Text = "Record(s) found: " & GridView1.Rows.Count.ToString()
        Else
            FooterTotalCount.Visible = False
        End If

        '' Hide header checkbox when row record found and more than one record
        Dim chkHeader As CheckBox = CType(GridView1.HeaderRow.FindControl("chkHeader"), CheckBox)
        chkHeader.Visible = IIf(GridView1.Rows.Count > 0, True, False)

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
        GridView1.DataKeyNames = New String() {"ID"}
        GridView1.CssClass = "table table-bordered"

        '' Header Style
        GridView1.HeaderStyle.CssClass = "table-secondary"
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

        GridView1.Columns(1).Visible = False   '' hide the unique_id column
    End Sub


    '' Insert new record to DB_Users table
    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Device_Serial As String = TB_Device_Serial.Text
        Dim Device_ID As String = TB_Device_ID.Text
        Dim Model As String = TB_Model.Text
        Dim AI_Software_Version As String = TB_AI_Software_Version.Text
        Dim R_Version As String = TB_R_Version.Text
        Dim Scale_SN As String = TB_Scale_Serial_No.Text
        Dim Location As String = TB_Location.Text
        Dim MAC_Address As String = Replace(TB_MAC_Address.Text, "-", ":")
        Dim Production_Licence_No As String = TB_Production_Licence_No.Text


        '' Check database if the scale serial no exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM CZL_Licenced_Devices WHERE Scale_SN = N'" & Scale_SN & "'", "NoOfRecord")) > 0, True, False)

        If RecordExists Then
            czldeviceserialexisterrormsg.Visible = True
            czldeviceserialexisterrormsg.Style.Add("display", "inline")
            TB_Scale_Serial_No.Focus()
        Else
            czldeviceserialexisterrormsg.Visible = False
            Try
                Dim sqlStr As String = "INSERT INTO CZL_Licenced_Devices(Unique_ID, Device_Serial, Device_ID, Model, AI_Software_Version, R_Version, Scale_SN, MAC_Addr, Production_Licence_No, Location, Created_Date, Last_Updated, Client_ID, CZL_Account_Unique_ID, Effective_Date) " &
                                       "VALUES(dbo.Get_New_CZL_Licenced_Device_Unique_ID(), N'" & Device_Serial & "', N'" & Device_ID & "', N'" & Model & "', N'" & AI_Software_Version & "', N'" & R_Version & "', N'" & Scale_SN & "', N'" & MAC_Address & "', N'" & Production_Licence_No & "', N'" & EscapeChar(Location) & "', GETDATE(), GETDATE(), NULL, NULL, NULL) "

                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try

            '' clear field after added
            TB_Device_Serial.Text = String.Empty
            TB_Device_ID.Text = String.Empty
            TB_Model.Text = String.Empty
            TB_AI_Software_Version.Text = String.Empty
            TB_R_Version.Text = String.Empty
            TB_Scale_Serial_No.Text = String.Empty
            TB_Location.Text = String.Empty
            TB_MAC_Address.Text = String.Empty
            TB_Production_Licence_No.Text = String.Empty

            PopulateGridViewData(TB_Search.Text)
        End If

    End Sub

    '' CZL client dropdownlist
    Protected Sub DDL_CZL_Client_ID_Load(sender As Object, e As EventArgs) Handles DDL_CZL_Client_ID.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr = "SELECT Client_ID, Client_ID + ' - ' + User_Group AS Client_ID_User_Group FROM CZL_Account ORDER BY CAST(Client_ID AS int) "
                DDL_CZL_Client_ID.DataSource = GetDataTable(sqlStr)
                DDL_CZL_Client_ID.DataTextField = "Client_ID_User_Group"
                DDL_CZL_Client_ID.DataValueField = "Client_ID"
                DDL_CZL_Client_ID.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Set confirmation before deleting
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim DeleteCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1
            Dim DeleteCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteCommandFieldIndex), LinkButton)
            If DeleteCommandField.CommandName = "Delete" Then
                DeleteCommandField.Attributes.Add("onclick", "return confirm('Are you sure to delete?');")
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


    '' Gridview row commands events
    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))

        Dim Device_Serial As String = (TryCast(row.FindControl("TB_E_Device_Serial"), TextBox)).Text
        Dim Device_ID As String = (TryCast(row.FindControl("TB_E_Device_ID"), TextBox)).Text
        Dim Model As String = (TryCast(row.FindControl("TB_E_Model"), TextBox)).Text
        Dim AI_Software_Version As String = (TryCast(row.FindControl("TB_E_AI_Software_Version"), TextBox)).Text
        Dim R_Version As String = (TryCast(row.FindControl("TB_E_R_Version"), TextBox)).Text
        Dim Scale_SN As String = (TryCast(row.FindControl("TB_E_Scale_SN"), TextBox)).Text
        Dim Location As String = (TryCast(row.FindControl("TB_E_Location"), TextBox)).Text
        Dim MAC_Address As String = (TryCast(row.FindControl("TB_E_MAC_Addr"), TextBox)).Text
        Dim Production_Licence_No As String = (TryCast(row.FindControl("TB_E_Production_Licence_No"), TextBox)).Text
        Dim Effective_Date As String = (TryCast(row.FindControl("TB_E_Effective_Date"), TextBox)).Text

        Dim sqlStr As String = "UPDATE CZL_Licenced_Devices SET Device_Serial = N'" & Device_Serial &
                                                                 "', Device_ID = N'" & Device_ID &
                                                                 "', Model = N'" & Model &
                                                                 "', AI_Software_Version = N'" & AI_Software_Version &
                                                                 "', R_Version = N'" & R_Version &
                                                                 "', Location = N'" & Location &
                                                                 "', MAC_Addr = N'" & MAC_Address &
                                                                 "', Production_Licence_No = N'" & Production_Licence_No &
                                                                 "', Effective_Date = N'" & Effective_Date & "'  WHERE ID = " & ID
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles GridView1.RowDeleting
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim sqlStr As String = "DELETE FROM CZL_Licenced_Devices WHERE ID = " & ID
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Search button and assign button controls
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub BT_Assign_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Assign.Click
        Dim Selected_Client_ID As String = DDL_CZL_Client_ID.SelectedValue
        Dim Selected_Account_Nama As String = DDL_CZL_Client_ID.SelectedItem.Text
        Dim Selected_Account_Unique_ID As String = Get_Value("SELECT TOP 1 CZL_Account_Unique_ID FROM CZL_Account WHERE Client_ID = " & Selected_Client_ID, "CZL_Account_Unique_ID")
        Dim Selected_Unique_ID As String = Nothing
        Dim CheckedCount As Integer = 0
        Dim IsProceed As Boolean = True

        For Each row As GridViewRow In GridView1.Rows
            If row.RowType = DataControlRowType.DataRow Then
                Dim chkRow As CheckBox = TryCast(row.Cells(0).FindControl("chkRow"), CheckBox)
                If chkRow.Checked Then
                    Selected_Unique_ID += "'" & TryCast(row.Cells(0).FindControl("LB_Unique_ID"), Label).Text & "' "
                    CheckedCount += 1

                    '' Check if any device in the selected of devices does not have effective date
                    If Len(TryCast(row.Cells(0).FindControl("LB_Effective_Date"), Label).Text) = 0 Then
                        IsProceed = False
                    End If
                End If
            End If
        Next

        If CheckedCount > 0 Then
            If IsProceed Then
                Dim sqlStr As String = "UPDATE CZL_Licenced_Devices " &
                                       "SET Client_ID = " & Selected_Client_ID & ", CZL_Account_Unique_ID = '" & Selected_Account_Unique_ID & "' " &
                                       "WHERE Unique_ID IN (" & Replace(Trim(Selected_Unique_ID), " ", ",") & ") "
                Try
                    RunSQL(sqlStr)
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('" & CheckedCount & " devices(s) assigned to Account - " & Selected_Account_Nama & "');</script>")

                    TB_Search.Text = String.Empty          '' Clear the search box
                    PopulateGridViewData(TB_Search.Text)
                    DDL_CZL_Client_ID.SelectedIndex = 0    '' Reset the client id dropdownlist
                Catch ex As Exception
                    Response.Write("Error: " & ex.Message)
                End Try
            Else
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('You have one/more device(s) does not have an effective date.\nPlease set check again.');</script>")
            End If
        Else
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('Please select device to assign.');</script>")
        End If
    End Sub


End Class
