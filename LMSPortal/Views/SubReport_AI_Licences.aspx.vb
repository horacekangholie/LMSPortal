
Partial Class Views_SubReport_AI_Licences
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If

        If Request.QueryString("ColNo") = 4 Then
            For i = 0 To GridView1.Columns.Count - 1
                GridView1.Columns(i).Visible = IIf(GridView1.Columns(i).HeaderText.Contains("Licence"), True, False)
            Next
        End If

        voidlicensekeymessage.Visible = IIf(Request.QueryString("ColNo") <> 4, False, True)
    End Sub

    Protected Sub BuildPageContent(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Dim Customer_ID As String = Replace(Request.QueryString("Customer_ID"), "_", " ")
        Dim ColNo As Integer = Request.QueryString("ColNo")

        Try
            Dim sqlStr As String = "SELECT [Serial No], [MAC Address], [Licence Code], [AI Device Serial No], [AI Device ID], [Activated Date] " &
                                   "     , CASE WHEN [Activated Date] IS NULL THEN NULL ELSE [Expired Date] END [Expired Date] " &
                                   "FROM _AllRegisteredAILicence " &
                                   "WHERE [Customer ID] = '" & Customer_ID & "' "

            Select Case ColNo
                Case 1
                    sqlStr += "AND [Status] = 'Activated' "
                Case 2
                    sqlStr += "AND [Status] = 'Expired' "
                Case 3
                    sqlStr += "AND [Status] = 'Renew' "
                Case 4
                    sqlStr += "AND [Status] = 'Blocked' "
                Case Else
                    sqlStr = sqlStr
            End Select

            sqlStr += "AND ([Serial No] LIKE N'%" & keyword & "%' OR [MAC Address] LIKE N'%" & keyword & "%' OR [Licence Code] LIKE N'%" & keyword & "%' OR [AI Device Serial No] LIKE N'%" & keyword & "%' OR [AI Device ID] LIKE N'%" & keyword & "%') "
            sqlStr += "ORDER BY [Expired Date] "

            '' Formatting GridView
            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub BuildGridView()
        '' GridView Properties
        GridView1.AutoGenerateColumns = False
        GridView1.AllowPaging = True
        GridView1.PageSize = 20
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"Serial No"}
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

        GridView1.Columns.Clear()  '' Clear each time page index changed

        Dim reportColName As String() = {"Serial No", "MAC Address", "Licence Code", "AI Device Serial No", "AI Device ID", "Activated Date", "Expired Date"}
        Dim reportColSize As Integer() = {50, 100, 100, 100, 250, 100, 250}

        '' Create column boundfield
        For i = 0 To reportColName.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = reportColName(i)
            Bfield.HeaderText = Replace(reportColName(i), "_", " ")
            Bfield.HeaderStyle.Width = reportColSize(i)
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next
    End Sub


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Style.Add("background-color", "#B8DAFF")
            Next
        End If

        For i = 0 To e.Row.Cells.Count - 1
            e.Row.Cells(i).Wrap = False
        Next
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        BuildPageContent()
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BuildPageContent(TB_Search.Text)
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Try
            RunSQL("EXEC SP_Sync_LMS_Licence")
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub btnClose_Click(sender As Object, e As EventArgs)
        '' Close the browser
        Response.Write("<script>window.close();</script>")
    End Sub

End Class
