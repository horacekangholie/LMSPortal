
Partial Class Reports_Activated_AI_Licence
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "AI Licences"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        BuildContenctPage(TB_Search.Text)

        '' Sync button with bootstrap icons
        AILicenceRefresh.Text = "Sync " & "<i class='bi bi-arrow-repeat align-middle' style='display:inline-block;font-size:1.8rem;'></i>"
    End Sub

    Protected Sub BuildContenctPage(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr() As String = {"SELECT * FROM R_Activated_AI_Licence WHERE ([Serial No] IS NOT NULL AND [Serial No] != '') AND [Status] NOT IN ('Renew', 'Expired') AND ([Licensee] LIKE '%" & keyword & "%' OR Replace([Licence Code], '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR [Serial No] LIKE '%" & keyword & "%' OR [MAC Address] LIKE '%" & keyword & "%' OR [AI Device ID] LIKE '%" & keyword & "%' OR [AI Device Serial No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%') ORDER BY [Expired Date], [Serial No] ",
                                      "SELECT * FROM R_Activated_AI_Licence WHERE ([Serial No] IS NULL OR [Serial No] = '') AND ([Licensee] LIKE '%" & keyword & "%' OR Replace([Licence Code], '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR [Serial No] LIKE '%" & keyword & "%' OR [MAC Address] LIKE '%" & keyword & "%' OR [AI Device ID] LIKE '%" & keyword & "%' OR [AI Device Serial No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%') AND (Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key')) ORDER BY [Expired Date] ",
                                      "SELECT * FROM R_Activated_AI_Licence WHERE [Status] IN ('Renew') AND ([Licensee] LIKE '%" & keyword & "%' OR Replace([Licence Code], '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR [Serial No] LIKE '%" & keyword & "%' OR [MAC Address] LIKE '%" & keyword & "%' OR [AI Device ID] LIKE '%" & keyword & "%' OR [AI Device Serial No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%') ORDER BY [Expired Date] ",
                                      "SELECT * FROM R_Activated_AI_Licence WHERE [Status] IN ('Expired') AND ([Licensee] LIKE '%" & keyword & "%' OR Replace([Licence Code], '-', '') LIKE '%" & Replace(keyword, "-", "") & "%' OR [Serial No] LIKE '%" & keyword & "%' OR [MAC Address] LIKE '%" & keyword & "%' OR [AI Device ID] LIKE '%" & keyword & "%' OR [AI Device Serial No] LIKE '%" & keyword & "%' OR [Requested By] LIKE '%" & keyword & "%') ORDER BY [Expired Date] DESC "}

            ' Build and bind Gridview
            BuildGridView(GridView1, "GridView1", "Customer ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Customer ID")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Customer ID")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "Customer ID")
            GridView4.DataSource = GetDataTable(sqlStr(3))
            GridView4.DataBind()

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        ' Get the total record of each tab
        Dim GridviewObj As GridView() = {GridView1, GridView2, GridView3, GridView4}
        Dim FooterObj As Label() = {FooterTotalCount1, FooterTotalCount2, FooterTotalCount3, FooterTotalCount4}

        ' Assign the no of record to the footer
        For i = 0 To GridviewObj.Count - 1
            If keyword <> "" AndAlso CInt(GridviewObj(i).Rows.Count.ToString()) > 0 Then
                FooterObj(i).Visible = True
                FooterObj(i).Text = "Record(s) found: " & GridviewObj(i).Rows.Count.ToString()
            Else
                FooterObj(i).Visible = False
            End If
        Next

    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.AllowPaging = True
        GridViewObj.PageSize = 15
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowHeaderWhenEmpty = True
        GridViewObj.DataKeyNames = New String() {DataKeyName}
        GridViewObj.CssClass = "table table-bordered"
        GridViewObj.Style.Add("width", "100%")   '' Stretch the Gridview to screen width

        '' Header Style
        GridViewObj.HeaderStyle.CssClass = "table-primary"
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
        GridViewObj.PagerSettings.PageButtonCount = "10"
        GridViewObj.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewObj.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridViewObj.EmptyDataText = "No records found."

        '' Define each Gridview
        Select Case ControlName
            Case "GridView1", "GridView2", "GridView3", "GridView4"
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Licensee", "Created Date", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Activated Date", "Expired Date", "Status", "Requested By", "Remarks"}
                Dim ColSize() As Integer = {100, 50, 100, 50, 80, 100, 120, 50, 50, 50, 100, 300}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    Bfield.DataField = ColData(i)
                    If Bfield.DataField.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                    End If
                    If Bfield.DataField.Contains("Remarks") Then
                        Bfield.ItemStyle.Wrap = True
                    Else
                        Bfield.ItemStyle.Wrap = False
                    End If
                    Bfield.HeaderStyle.Wrap = True
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False
        End Select
    End Sub


    ''Gridview controls
    Private Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated, GridView2.RowCreated, GridView3.RowCreated, GridView4.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            '' Set as unknown when the cell return dbnull value
            Dim AI_Tech_Details() As String = {"Serial No", "MAC Address", "AI Device ID", "AI Device Serial No"}
            For i = 0 To AI_Tech_Details.Length - 1
                Dim emptyField As String = IIf(IsDBNull(drv(AI_Tech_Details(i))), String.Empty, drv(AI_Tech_Details(i)))
                If Len(emptyField) <= 0 Then
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Text = "Unknown"
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("font-style", "italic")
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("color", "#999999")
                End If
            Next
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
        End If
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
        End If
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildContenctPage(TB_Search.Text)
    End Sub

    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        GridView2.PageIndex = e.NewPageIndex
        BuildContenctPage(TB_Search.Text)
    End Sub

    Protected Sub GridView3_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView3.PageIndexChanging
        GridView3.PageIndex = e.NewPageIndex
        BuildContenctPage(TB_Search.Text)
    End Sub

    Protected Sub GridView4_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView4.PageIndexChanging
        GridView3.PageIndex = e.NewPageIndex
        BuildContenctPage(TB_Search.Text)
    End Sub


    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildContenctPage(TB_Search.Text)
    End Sub

    Protected Sub AILicenceRefresh_Click(sender As Object, e As EventArgs) Handles AILicenceRefresh.Click
        Try
            RunSQL("EXEC SP_Sync_LMS_Licence")
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        'BuildContenctPage(TB_Search.Text)   '' no need to Build the content page here as button postback will trigger postback to build the build content page
    End Sub

End Class
