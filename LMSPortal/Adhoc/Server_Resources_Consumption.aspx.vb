
Partial Class Adhoc_Server_Resources_Consumption
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Server Space Consumption"

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
            Dim sqlStr As String = " EXEC Display_Consumption_Data 11 "

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
        GridView1.AutoGenerateColumns = True
        GridView1.AllowPaging = False
        GridView1.PageSize = 15
        GridView1.CellPadding = 4
        GridView1.Font.Size = 10
        GridView1.GridLines = GridLines.None
        GridView1.ShowFooter = False
        GridView1.ShowHeaderWhenEmpty = True
        GridView1.DataKeyNames = New String() {"ColName"}
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

    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 0 Then
                    e.Row.Cells(i).Width = 150
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                    e.Row.Cells(i).Style.Add("text-align", "right !important")
                Else
                    e.Row.Cells(i).Width = 200
                    e.Row.Cells(i).Text = ""
                End If
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 0 Then
                    Dim ColName As String() = {"EJournal Size", "PLU Count", "Transaction Total", "Transaction Item", "Billed Store", "DB Size", "Disk Space Used"}
                    Dim UnitType As String() = {"MB", "rows", "rows", "rows", "stores", "GB", "%"}
                    For j = 0 To ColName.Count - 1
                        If e.Row.Cells(0).Text = ColName(j) Then
                            If ColName(j) = "Disk Space Used" Then
                                e.Row.Cells(i).Text = String.Format("{0:0 " & UnitType(j) & "}", CInt(e.Row.Cells(i).Text) / 100)
                            Else
                                e.Row.Cells(i).Text = String.Format("{0:#,0 " & UnitType(j) & "}", CDbl(e.Row.Cells(i).Text))
                            End If
                        End If
                    Next
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                    e.Row.Cells(i).Wrap = False
                Else
                    e.Row.Cells(i).Style.Add("padding-left", "20px")
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Left
                End If
            Next
        End If

        For i = 0 To e.Row.Cells.Count - 1
            e.Row.Cells(i).Height = 40
            e.Row.Cells(i).VerticalAlign = VerticalAlign.Middle
            e.Row.Cells(i).Wrap = False
        Next
    End Sub

End Class
