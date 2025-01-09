
Partial Class Maintenance_Lookup_Details
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Lookup Details"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            PopulateGridViewData()
            TB_Lookup_Name.Text = HttpUtility.HtmlDecode(Request.QueryString("Lookup_Name"))
        End If
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr As String = "SELECT * FROM DB_Lookup WHERE Lookup_Name ='" & Request.QueryString("Lookup_Name") & "' "

            If TB_Lookup_Name.Text <> "Production Used Licence Key" Then
                sqlStr += " AND (Value_1 LIKE '%" & keyword & "%' OR Value_2 LIKE '%" & keyword & "%' OR Value_3 LIKE '%" & keyword & "%' OR Value_4 LIKE '%" & keyword & "%') "
            Else
                '' Remove dash when searching by license key
                sqlStr += " AND (REPLACE(Value_1, '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR Value_2 LIKE '%" & keyword & "%' OR Value_3 LIKE '%" & keyword & "%' OR Value_4 LIKE '%" & keyword & "%') "
            End If

            If Request.QueryString("Lookup_Name") = "Bill Items" Then
                sqlStr += " ORDER BY Value_4, Value_1 "
            Else
                sqlStr += " ORDER BY Value_2 "
            End If

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


    '' Insert new record to DB_Lookup table
    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Lookup_Name As String = TB_Lookup_Name.Text
        Dim Value_1, Value_2, Value_3, Value_4 As String
        Value_1 = TB_Value_1.Text
        Value_2 = TB_Value_2.Text
        Value_3 = TB_Value_3.Text
        Value_4 = TB_Value_4.Text

        Dim isValid As Boolean = IIf(Get_Value("SELECT COUNT(ISNULL(Value_1, '')) AS NoOfRecord FROM DB_Lookup WHERE Value_1 = '" & Value_1 & "' ", "NoOfRecord") < 1, True, False)
        Dim toSkipValidate As Boolean = LookupNameToSkipValidate(Trim(TB_Lookup_Name.Text))

        If isValid Or toSkipValidate Then
            Try
                Dim sqlStr As String = " INSERT INTO DB_Lookup(Lookup_Name, Value_1, Value_2, Value_3, Value_4) " &
                                       " VALUES('" & Lookup_Name & "', '" & Value_1 & "', '" & Value_2 & "', '" & Value_3 & "', '" & Value_4 & "')"

                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("ERROR: " & ex.Message)
            End Try

            '' clear field after added
            TB_Value_1.Text = String.Empty
            TB_Value_2.Text = String.Empty
            TB_Value_3.Text = String.Empty
            TB_Value_4.Text = String.Empty
        Else
            AlertMessageMsgBox(Value_1 & " exists, please check and try again.")
        End If
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Function LookupNameToSkipValidate(ByVal lookupName As String) As Boolean
        Select Case lookupName
            Case "Bill Items"
                Return False
            Case Else
                Return True
        End Select
    End Function



    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"Lookup_Name", "Value_1", "Value_2", "Value_3", "Value_4", ""}
        Dim ColSize() As Integer = {200, 300, 300, 300, 300, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = ColSize(i)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
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
                DeleteCommandField.Attributes.Add("onclick", "return confirm('Delete record may cause data inconsistency,\nare you sure to proceed?');")
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
    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim Lookup_Name As String = (TryCast(row.FindControl("TB_E_Lookup_Name"), TextBox)).Text
        Dim Value_1 As String = (TryCast(row.FindControl("TB_E_Value_1"), TextBox)).Text
        Dim Value_2 As String = (TryCast(row.FindControl("TB_E_Value_2"), TextBox)).Text
        Dim Value_3 As String = (TryCast(row.FindControl("TB_E_Value_3"), TextBox)).Text
        Dim Value_4 As String = (TryCast(row.FindControl("TB_E_Value_4"), TextBox)).Text

        Dim sqlStr As String = " UPDATE DB_Lookup SET Lookup_Name = N'" & Lookup_Name & "', " &
                                 " Value_1 = N'" & Value_1 & "', " &
                                 " Value_2 = N'" & Value_2 & "', " &
                                 " Value_3 = N'" & Value_3 & "', " &
                                 " Value_4 = N'" & Value_4 & "' " &
                                 " WHERE ID = " & ID
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
        Dim sqlStr As String = " DELETE FROM DB_Lookup WHERE ID = " & ID
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


    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Maintenance/Lookup_Table.aspx")
    End Sub


End Class




