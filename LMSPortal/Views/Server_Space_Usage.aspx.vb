
Partial Class Views_Server_Space_Usage
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Server Space Consumption"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        BuildContenctPage()
    End Sub

    Protected Sub BuildContenctPage(Optional ByVal ReportMonth As String = Nothing)
        Try
            Dim sqlStr() As String = {"EXEC SP_Server_Space_Consumption_Week ",
                                      "EXEC dbo.SP_Generate_Server_Consumption_Statistics '_Server_Space_Week_Growth', 'Used Growth', 'DB Growth' ",
                                      "EXEC SP_Server_Space_Consumption_Month ",
                                      "EXEC dbo.SP_Generate_Server_Consumption_Statistics '_Server_Space_Month_Growth', 'Used Growth', 'DB Growth' ",
                                      "EXEC SP_Server_Space_Consumption_Quarter ",
                                      "EXEC dbo.SP_Generate_Server_Consumption_Statistics '_Server_Space_Quarter_Growth', 'Used Growth', 'DB Growth' ",
                                      "EXEC SP_Server_Space_Consumption_Semiannual ",
                                      "EXEC dbo.SP_Generate_Server_Consumption_Statistics '_Server_Space_Semiannual_Growth', 'Used Growth', 'DB Growth' ",
                                      "EXEC SP_Server_Space_Consumption_Year ",
                                      "EXEC dbo.SP_Generate_Server_Consumption_Statistics '_Server_Space_Year_Growth', 'Used Growth', 'DB Growth' ",
                                      "SELECT CAST(Reading_Date AS date) AS [Date], Remarks AS [Events] FROM Server_Space WHERE LEN(Remarks) > 0 ORDER BY ID DESC "}

            ' Build and bind Gridview
            BuildGridView(GridView1, "GridView1", "Year")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Statistics")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Year")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "Statistics")
            GridView4.DataSource = GetDataTable(sqlStr(3))
            GridView4.DataBind()

            BuildGridView(GridView5, "GridView5", "Year")
            GridView5.DataSource = GetDataTable(sqlStr(4))
            GridView5.DataBind()

            BuildGridView(GridView6, "GridView6", "Statistics")
            GridView6.DataSource = GetDataTable(sqlStr(5))
            GridView6.DataBind()

            BuildGridView(GridView7, "GridView7", "Year")
            GridView7.DataSource = GetDataTable(sqlStr(6))
            GridView7.DataBind()

            BuildGridView(GridView8, "GridView8", "Statistics")
            GridView8.DataSource = GetDataTable(sqlStr(7))
            GridView8.DataBind()

            BuildGridView(GridView9, "GridView9", "COL")
            GridView9.DataSource = GetDataTable(sqlStr(8))
            GridView9.DataBind()

            BuildGridView(GridView10, "GridView10", "Statistics")
            GridView10.DataSource = GetDataTable(sqlStr(9))
            GridView10.DataBind()

            BuildGridView(GridView11, "GridView11", "Date")
            GridView11.DataSource = GetDataTable(sqlStr(10))
            GridView11.DataBind()

        Catch ex As Exception
            Response.Write("BuildContenctPage - Error:  " & ex.Message)
        End Try

        '' Draw last line if page count less than 1
        GridView11.Style.Add("border-bottom", If(GridView11.PageCount < 2, "1px solid #ddd", "1px solid #fff !important"))
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        'GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowHeader = True
        'GridViewObj.ShowFooter = True
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
            Case "GridView1", "GridView3", "GridView5", "GridView7", "GridView9"
                GridViewObj.AutoGenerateColumns = True
                GridViewObj.AllowPaging = False
                GridViewObj.ShowFooter = False
            Case "GridView2", "GridView4", "GridView6", "GridView8", "GridView10"
                GridViewObj.AutoGenerateColumns = True
                GridViewObj.AllowPaging = False
                GridViewObj.ShowFooter = False
                GridViewObj.Style.Add("width", "50%")
            Case "GridView11"
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #ddd")
                GridViewObj.Columns.Clear()

                Dim ColData() As String = {"Date", "Events"}
                Dim ColSize() As Unit = {Unit.Percentage(5), Unit.Percentage(90)}
                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.ItemStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                        Bfield.ItemStyle.Wrap = False
                        Bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Justify
                    Else
                        Bfield.ItemStyle.Wrap = True
                    End If
                    Bfield.HtmlEncode = False '' to render as html
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False
        End Select

    End Sub



    ''Gridview controls
    Private Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs) _
        Handles GridView1.RowCreated, GridView2.RowCreated, GridView3.RowCreated, GridView4.RowCreated, GridView5.RowCreated, GridView6.RowCreated, GridView7.RowCreated, GridView8.RowCreated, GridView9.RowCreated, GridView10.RowCreated

        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            'e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            'e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = "Week"  '' Week No
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If

                If i > 0 Then
                    e.Row.Cells(i).Wrap = False
                    If i > 1 Then
                        e.Row.Cells(i).Text = CInt(e.Row.Cells(i).Text)  '' Convert row value to integer for Growth value
                    End If

                    '' hightlight color and add percentage symbol for those of growth value
                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Growth") Or e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Diff") Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                    End If
                End If
            Next

            '' Empty the row for year value if repeated
            e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Used", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text)
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Usage"), e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (%)", e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (GB)")
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = "Month"  '' Month Name
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If

                If i > 0 Then
                    e.Row.Cells(i).Wrap = False
                    If i > 1 Then
                        e.Row.Cells(i).Text = CInt(e.Row.Cells(i).Text)   '' Convert row value to integer for Growth value
                    End If

                    '' hightlight color and add percentage symbol for those of growth value
                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Growth") Or e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Diff") Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                    End If
                End If
            Next

            '' Empty the row for year value if repeated
            e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Used", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text)
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Usage"), e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (%)", e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (GB)")
        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                e.Row.Cells(i).Text = IIf(i > 1, "Q", "") & e.Row.Cells(i).Text
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = "Quarter"  '' Quarter No
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If

                If i > 0 Then
                    e.Row.Cells(i).Wrap = False
                    If i > 1 Then
                        e.Row.Cells(i).Text = CInt(e.Row.Cells(i).Text)   '' Convert row value to integer for Growth value
                    End If

                    '' hightlight color and add percentage symbol for those of growth value
                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Growth") Or e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Diff") Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                    End If
                End If
            Next

            '' Empty the row for year value if repeated
            e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Used", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text)
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Usage"), e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (%)", e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (GB)")
        End If
    End Sub

    Protected Sub GridView7_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView7.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                If i > 1 Then
                    e.Row.Cells(i).Text = IIf(e.Row.Cells(i).Text.Contains("1"), e.Row.Cells(i).Text & "-st", e.Row.Cells(i).Text & "-nd")
                End If
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = "Semiannual"  '' Semiannual
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If

                If i > 0 Then
                    e.Row.Cells(i).Wrap = False
                    If i > 1 Then
                        e.Row.Cells(i).Text = CInt(e.Row.Cells(i).Text) '' Convert row value to integer for Growth value
                    End If

                    '' hightlight color and add percentage symbol for those of growth value
                    If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Growth") Or e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Diff") Then
                        e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                    End If
                End If
            Next

            '' Empty the row for year value if repeated
            e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text <> "Used", "", e.Row.Cells(GetColumnIndexByName(e.Row, "Year")).Text)
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Usage"), e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (%)", e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (GB)")
        End If
    End Sub

    Protected Sub GridView9_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView9.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
            Next
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = "Year"  '' Semiannual
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If

                If i > 0 Then
                    e.Row.Cells(i).Wrap = False
                    e.Row.Cells(i).Text = CInt(e.Row.Cells(i).Text) '' Convert row value to integer for Growth value
                End If

                '' hightlight color and add percentage symbol for those of growth value
                If e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Growth") Or e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Diff") Then
                    e.Row.Cells(i).Style.Add("background-color", "#e6ffe6 !important")
                End If
            Next

            '' Empty the row for year value if repeated
            e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text = IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text.Contains("Usage"), e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (%)", e.Row.Cells(GetColumnIndexByName(e.Row, "COL")).Text & " (GB)")
        End If
    End Sub

    Protected Sub GridView11_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView11.PageIndexChanging
        GridView11.PageIndex = e.NewPageIndex
        BuildContenctPage()
    End Sub

    Protected Sub GridView_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) _
        Handles GridView2.RowDataBound, GridView4.RowDataBound, GridView6.RowDataBound, GridView8.RowDataBound, GridView10.RowDataBound

        Dim GridViewObj As GridView = CType(sender, GridView)
        Dim maxColumnWidth As Integer
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).VerticalAlign = VerticalAlign.Top
                e.Row.Cells(i).Text = IIf(e.Row.Cells(i).Text.Contains("Mean"), "Moving Average (GB)", e.Row.Cells(i).Text)
                e.Row.Cells(i).Text = IIf(e.Row.Cells(i).Text.Contains("Median"), e.Row.Cells(i).Text & " (GB)", e.Row.Cells(i).Text)
                e.Row.Cells(i).Text = IIf(e.Row.Cells(i).Text.Contains("Standard Deviation"), e.Row.Cells(i).Text & " (Scores)", e.Row.Cells(i).Text)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                '' Assuming an average character width of 10 pixels
                Dim columnWidth As Integer = e.Row.Cells(i).Text.Length * 10
                If columnWidth > maxColumnWidth Then
                    maxColumnWidth = columnWidth
                End If
            Next
        End If
    End Sub



    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Dim Page_Origin As String = Get_Value("SELECT TOP 1 Page_Origin FROM DMC_Account_Reports_List WHERE ID = " & Request.QueryString("ID"), "Page_Origin")
        Response.Redirect(Page_Origin)
    End Sub


End Class
