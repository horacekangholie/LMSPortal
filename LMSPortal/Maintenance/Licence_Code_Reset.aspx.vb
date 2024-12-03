
Partial Class Maintenance_Licence_Code_Reset
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Licence Code Reset"

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
            Dim sqlStr As String = "SELECT A.ID, B.Name, A.Licence_Code, A.MAC_Address, A.Reset_On, A.Remarks FROM LMS_Licence_Reset_History A "
            sqlStr += "INNER JOIN Master_Customer B ON B.Customer_ID = A.Customer_ID "
            sqlStr += "WHERE B.Name LIKE '%" & keyword & "%' OR REPLACE(A.Licence_Code, '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR REPLACE(A.MAC_Address, ':', '') LIKE '%" & Replace(keyword, ":", "") & "%' OR Remarks LIKE '%" & keyword & "%' "
            sqlStr += "ORDER BY A.Reset_On DESC  "

            ''Response.Write(sqlStr)
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
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Licensee", "Licence_Code", "MAC_Address / UUID", "Reset_On", "Remarks", ""}
        Dim ColSize() As Integer = {200, 200, 0, 100, 500, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = ColSize(i)
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            If (e.Row.RowState And DataControlRowState.Edit) = 0 Then
                ' The row is in ItemTemplate mode
                '' Truncated the value and set tooltip to display value in full length
                Dim LB_MAC_Address As Label = DirectCast(e.Row.Cells(2).FindControl("LB_MAC_Address"), Label)
                Dim maxLength As Integer = 20
                If LB_MAC_Address.Text.Length > maxLength Then
                    LB_MAC_Address.ToolTip = LB_MAC_Address.Text
                    LB_MAC_Address.Text = LB_MAC_Address.Text.Substring(0, maxLength) & "<font style='color:#6585A1'>...</font>"
                End If
            ElseIf (e.Row.RowState And DataControlRowState.Edit) <> 0 Then
                ' The row is in EditTemplate mode
            End If

            '' Arrange the edit / delete control button to center
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
                If i = CtrlCellIndex Then
                    e.Row.Cells(CtrlCellIndex).Width = 0
                    e.Row.Cells(CtrlCellIndex).Wrap = False
                    e.Row.Cells(CtrlCellIndex).HorizontalAlign = HorizontalAlign.Center
                End If
            Next

            '' Set confirmation before deleting
            Dim DeleteCommandFieldIndex As Integer = e.Row.Cells(CtrlCellIndex).Controls.Count - 1
            Dim DeleteCommandField As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(DeleteCommandFieldIndex), LinkButton)
            If DeleteCommandField.CommandName = "Delete" Then
                DeleteCommandField.Attributes.Add("onclick", "return confirm('Are you sure to delete record?');")
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



    '' Gridview row commands events
    Protected Sub Check(ByVal sender As Object, ByVal e As EventArgs) Handles BN_Check.Click
        Dim Licence_Code As String = CleanSymbolInString(Trim(TB_Licence_Code.Text))
        Dim MAC_Address As String = CleanSymbolInString(Trim(TB_MAC_Address.Text))

        Dim sqlStr As String = "SELECT LMS_Licence.Customer_ID + '|' + Master_Customer.Name + '|' + Licence_Code + '|' + Synced_dmcmobiletoken_unique_id + '|' + Synced_dmcmobiletoken_status AS LicenceInfo " &
                               "FROM LMS_Licence " &
                               "INNER JOIN Master_Customer  On Master_Customer.Customer_ID = LMS_Licence.Customer_ID " &
                               "WHERE REPLACE(Licence_Code, '-', '') = '" & Licence_Code & "' AND REPLACE(Synced_dmcmobiletoken_unique_id, ':', '') = '" & MAC_Address & "' "
        Dim LicenceInfo() As String = Split(Get_Value(sqlStr, "LicenceInfo"), "|")

        '' if record exist then show licence is valid
        LicenceValidityTr.Visible = True
        LicenceValidityDiv.Visible = True
        If CInt(LicenceInfo.Length - 1) > 0 Then
            LicenceValidityDiv.Attributes.Add("class", "alert alert-dismissible alert-success")
            LicenceValidity.Text = "Licence is valid"
            TB_Customer_ID.Text = LicenceInfo(0)
            TB_Customer_Name.Text = LicenceInfo(1)
            RequiredField_TB_Remarks.Enabled = IIf(LicenceValidityTr.Visible, True, False)  '' Enable to valid Remarks field after license verified valid
        Else
            LicenceValidityDiv.Attributes.Add("class", "alert alert-dismissible alert-danger")
            LicenceValidity.Text = "Licence is invalid"
            TB_Customer_ID.Text = String.Empty
            TB_Customer_Name.Text = String.Empty
        End If
    End Sub

    Protected Sub Clear(ByVal sender As Object, ByVal e As EventArgs) Handles BN_Clear.Click
        TB_Licence_Code.Text = String.Empty
        TB_MAC_Address.Text = String.Empty
        TB_Customer_ID.Text = String.Empty
        TB_Customer_Name.Text = String.Empty
        LicenceValidityTr.Visible = False
        LicenceValidityDiv.Visible = False
    End Sub

    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Customer_ID As String = TB_Customer_ID.Text
        Dim Licence_Code As String = FormatLicenceCode(TB_Licence_Code.Text)
        Dim MAC_Address As String = FormatMACAddress(TB_MAC_Address.Text)
        Dim Remarks As String = TB_Remarks.Text

        Dim sqlStr As String = "INSERT INTO LMS_Licence_Reset_History(Customer_ID, Licence_Code, MAC_Address, Counter, Reset_On, Remarks) " &
                               "VALUES(N'" & Customer_ID & "', N'" & Licence_Code & "', N'" & MAC_Address & "', 1, GETDATE(), N'" & Remarks & "') "

        If Len(TB_Customer_ID.Text) > 0 AndAlso LicenceValidityTr.Visible = True Then
            If Not LicenceValidity.Text.Contains("invalid") Then
                Try
                    RunSQL(sqlStr)

                    '' clear field after added
                    TB_Licence_Code.Text = String.Empty
                    TB_MAC_Address.Text = String.Empty
                    TB_Customer_ID.Text = String.Empty
                    TB_Customer_Name.Text = String.Empty
                    TB_Remarks.Text = String.Empty

                    '' hide the message when click add new button
                    LicenceValidityTr.Visible = False

                Catch ex As Exception
                    Response.Write("Error: " & ex.Message)
                End Try
            Else
                AlertMessageMsgBox("Licence Code and MAC Address does not match.")
            End If
        ElseIf LicenceValidityTr.Visible = True Then
            AlertMessageMsgBox("Please ensure license is valid before reset.")
        Else
            AlertMessageMsgBox("Please click Check Button to verify the license validity first.")
        End If

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim Remarks As String = Trim((TryCast(row.FindControl("TB_E_Remarks"), TextBox)).Text)

        Dim sqlStr As String = " UPDATE LMS_Licence_Reset_History SET Remarks = N'" & EscapeChar(Remarks) & "' WHERE ID = " & ID
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
        Dim sqlStr As String = " DELETE FROM LMS_Licence_Reset_History WHERE ID = " & ID
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




    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
