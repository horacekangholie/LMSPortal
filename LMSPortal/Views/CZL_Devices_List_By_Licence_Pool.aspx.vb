
Partial Class Views_CZL_Devices_List_By_Licence_Pool
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Device List By Licence Pool"
    Dim sqlStr As String = "SELECT A.Customer_ID AS [Customer ID], B.Name AS [Name], B.Country AS [Country], B.Type AS [Licensee Group], Module_Type AS [Licence Type] " &
                           "    , (A.Balance + A.Used) AS [Total], A.Balance AS [Available], A.Used AS [Used] " &
                           "    , C.[Activated], C.[Expired], C.[Renew], C.[Blocked] " &
                           "FROM R_LMS_Module_Licence_Pool A  " &
                           "INNER JOIN _AllRegisteredAILicencePivot C On C.[Customer ID] = A.Customer_ID " &
                           "LEFT JOIN Master_Customer B On B.Customer_ID = A.Customer_ID  " &
                           "WHERE Module_Type = 'AI' AND B.Type NOT IN ('DIGI') " &
                           "ORDER BY B.Country "
    Dim reportColName As String() = {"Country", "Name", "Licensee Group", "Licence Type", "Total", "Available", "Used", "Activated", "Expired", "Renew", "Blocked"}
    Dim reportColSize As Integer() = {150, 250, 100, 250, 80, 80, 200, 80, 80, 80, 80}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BuildPageContent()
        End If
    End Sub

    Protected Sub BuildPageContent()
        Try
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
        GridView1.AllowPaging = False
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

        GridView1.Columns.Clear()  '' Clear each time page index changed

        '' Create column boundfield
        For i = 0 To reportColName.Length - 1
            If i > 6 Then
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderText = Replace(reportColName(i), "_", " ")
                TField.HeaderStyle.Width = reportColSize(i)
                TField.HeaderStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateHyperlinkControl()
                GridView1.Columns.Add(TField)
            Else
                Dim Bfield As BoundField = New BoundField()
                Bfield.DataField = reportColName(i)
                Bfield.HeaderText = Replace(reportColName(i), "_", " ")
                Bfield.HeaderStyle.Width = reportColSize(i)
                Bfield.ItemStyle.Wrap = False
                GridView1.Columns.Add(Bfield)
            End If
        Next
    End Sub


    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            '' Add additional header row
            Dim HeaderGrid As GridView = DirectCast(sender, GridView)
            Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)

            For i = 0 To e.Row.Cells.Count - 1
                Dim HeaderCell = New TableHeaderCell()
                Select Case i
                    Case 0
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.ColumnSpan = 4
                    Case 4
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "Licence Pool"
                        HeaderCell.ColumnSpan = 3
                        HeaderCell.Style.Add("text-align", "left")
                    Case 7
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "Licence Status"
                        HeaderCell.ColumnSpan = 3
                        HeaderCell.Style.Add("text-align", "center")
                    Case 10
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = ""
                        HeaderCell.ColumnSpan = 1
                        HeaderCell.Style.Add("text-align", "center")
                End Select
            Next
            HeaderGrid.Controls(0).Controls.AddAt(0, HeaderGridRow)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            For i = 0 To e.Row.Cells.Count - 1
                If i > 6 Then
                    Dim Customer_ID As String = Replace(DataBinder.Eval(e.Row.DataItem, "Customer ID"), " ", "_")
                    Dim HyperLinkValue As HyperLink = TryCast(e.Row.Cells(i).Controls(0), HyperLink)
                    HyperLinkValue.Text = IIf(drv(reportColName(i)) <= 0, "", drv(reportColName(i)))
                    HyperLinkValue.Attributes.Add("Onclick", "window.open('/Views/SubReport_AI_Licences.aspx?Customer_ID=" & Customer_ID & "&ColNo=" & i - 6 & "', '_blank', 'titlebar=no', 'status=no', 'directories=no', 'width=800px, height=600px');")
                        HyperLinkValue.Target = "_blank"
                        HyperLinkValue.Style.Add("cursor", "pointer")
                    HyperLinkValue.Style.Add("color", "blue")
                End If
                If i = e.Row.Cells.Count - 1 Then
                    e.Row.Cells(i).Style.Add("background-color", "#f9ebea")
                End If
            Next
        End If

        '' format value columns
        For i = 0 To e.Row.Cells.Count - 1
            If i > 6 Then
                e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                e.Row.Cells(i).Style.Add("text-align", "center !important")
            End If
        Next
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

End Class
