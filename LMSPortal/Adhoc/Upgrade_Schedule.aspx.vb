
Partial Class Adhoc_Upgrade_Schedule
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "DMC Maintenance Schedule"

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
            Dim sqlStr As String = "SELECT * FROM DMC_Maintenance_History "
            sqlStr += "WHERE Maintenance_Date LIKE '%" & keyword & "%' OR Work_Type LIKE '%" & keyword & "%' OR Description LIKE '%" & keyword & "%' OR Remarks LIKE '%" & keyword & "%' "
            sqlStr += "ORDER BY Maintenance_Date DESC, ID DESC "

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
        GridView1.PageSize = 10
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
        Dim ColName() As String = {"Date", "Type", "Description", "Status", "From", "To", "Duration", "Remarks", ""}
        Dim ColSize() As Integer = {100, 200, 300, 100, 100, 100, 100, 300, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = ColSize(i)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = True
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
    Protected Sub TB_Downtime_From_To_TextChanged(sender As Object, e As EventArgs) Handles TB_Downtime_From.TextChanged, TB_Downtime_To.TextChanged
        UpdateDuration()
    End Sub

    Private Sub UpdateDuration()
        ' Use IsNullOrWhiteSpace to check for null, empty, or whitespace strings
        If String.IsNullOrWhiteSpace(TB_Downtime_From.Text) AndAlso Not String.IsNullOrWhiteSpace(TB_Downtime_To.Text) Then
            TB_Downtime_From.Focus()
        ElseIf String.IsNullOrWhiteSpace(TB_Downtime_To.Text) AndAlso Not String.IsNullOrWhiteSpace(TB_Downtime_From.Text) Then
            TB_Downtime_To.Focus()
        End If

        ' Only update duration if both fields are non-empty and non-null
        TB_Duration.Text = If(Not String.IsNullOrWhiteSpace(TB_Downtime_From.Text) AndAlso Not String.IsNullOrWhiteSpace(TB_Downtime_To.Text), GetTimeDifference(TB_Downtime_From.Text, TB_Downtime_To.Text).ToString(), "")
    End Sub


    Protected Sub Add(ByVal sender As Object, ByVal e As EventArgs) Handles BN_AddNew.Click
        Dim Maintenance_Date As String = TB_Maintenance_Date.Text
        Dim Work_Type As String = TB_Work_Type.Text
        Dim Description As String = EscapeChar(TB_Description.Text)
        Dim Downtime_From As String = TB_Downtime_From.Text
        Dim Donwtime_To As String = TB_Downtime_To.Text
        Dim Duration As String = TB_Duration.Text
        Dim Status As String = DDL_Status.SelectedValue
        Dim Remarks As String = EscapeChar(TB_Remarks.Text)

        Dim sqlStr As String = "INSERT INTO DMC_Maintenance_History(Maintenance_Date, Work_Type, Description, Status, Downtime_From, Downtime_To, Duration, Remarks) " &
                               "VALUES(N'" & Maintenance_Date & "', N'" & Work_Type & "', N'" & Description & "', N'" & Status & "', N'" & Downtime_From & "', N'" & Donwtime_To & "', N'" & Duration & "', N'" & Remarks & "') "

        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' clear field after added
        TB_Maintenance_Date.Text = String.Empty
        TB_Work_Type.Text = String.Empty
        TB_Description.Text = String.Empty
        TB_Downtime_From.Text = String.Empty
        TB_Downtime_To.Text = String.Empty
        TB_Duration.Text = String.Empty
        DDL_Status.SelectedIndex = -1
        TB_Remarks.Text = String.Empty

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)
        Dim ID As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim Maintenance_Date As String = Trim((TryCast(row.FindControl("TB_E_Maintenance_Date"), TextBox)).Text)
        Dim Work_Type As String = Trim((TryCast(row.FindControl("TB_E_Work_Type"), TextBox)).Text)
        Dim Description As String = Trim((TryCast(row.FindControl("TB_E_Description"), TextBox)).Text)
        Dim Status As String = Trim((TryCast(row.FindControl("TB_E_Status"), TextBox)).Text)
        Dim Downtime_From As String = Trim((TryCast(row.FindControl("TB_E_Downtime_From"), TextBox)).Text)
        Dim Downtime_To As String = Trim((TryCast(row.FindControl("TB_E_Downtime_To"), TextBox)).Text)
        Dim Duration As String = Trim((TryCast(row.FindControl("TB_E_Duration"), TextBox)).Text)
        Dim Remarks As String = Trim((TryCast(row.FindControl("TB_E_Remarks"), TextBox)).Text)

        Dim sqlStr As String = " UPDATE DMC_Maintenance_History SET Maintenance_Date = N'" & Maintenance_Date &
                                                                       "', Work_Type = N'" & Work_Type &
                                                                     "', Description = N'" & Description &
                                                                          "', Status = N'" & Status &
                                                                   "', Downtime_From = N'" & Downtime_From &
                                                                     "', Downtime_To = N'" & Downtime_To &
                                                                        "', Duration = N'" & Duration &
                                                                         "', Remarks = N'" & Remarks &
                                                                         "' WHERE ID = " & ID
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
        Dim sqlStr As String = " DELETE FROM DMC_Maintenance_History WHERE ID = " & ID
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



    Protected Sub DDL_Status_Load(sender As Object, e As EventArgs) Handles DDL_Status.Load
        If Not IsPostBack Then
            Dim sqlStr As String = "SELECT Value_1 AS Status FROM DB_Lookup WHERE Lookup_Name = 'Scheduled Maintenance Status' ORDER BY Value_2"
            Try
                DDL_Status.DataSource = GetDataTable(sqlStr)
                DDL_Status.DataTextField = "Status"
                DDL_Status.DataValueField = "Status"
                DDL_Status.DataBind()
            Catch ex As Exception
                Response.Write("ERROR: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Status_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Status.SelectedIndexChanged
        ' Convert DDL_Status.SelectedValue to lowercase before comparison
        If DDL_Status.SelectedValue.ToLower() = "scheduled" Then
            ' Set defaults for scheduled status
            TB_Downtime_From.Text = "00:00"
            TB_Downtime_To.Text = "00:00"
            TB_Duration.Text = "0"
        End If
    End Sub


    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub


End Class
