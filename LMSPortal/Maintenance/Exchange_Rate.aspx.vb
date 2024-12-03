Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Class Maintenance_Exchange_Rate
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Upload Exchange Rate"

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
            Dim sqlStr As String = "SELECT ID, CONVERT(nvarchar(50),CAST(Date as date), 23) AS Date, Currency, Rate " &
                                   "FROM DB_Exchange_History " &
                                   "WHERE Date LIKE '%" & keyword & "%' OR Currency LIKE '%" & keyword & "%' " &
                                   "ORDER BY Date DESC, Currency DESC "

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

        'GridView1.Columns.Clear()  '' Clear each time page index changed otherwise gridview will keep populating column

        'Dim ColData() As String = {"Date", "Currency", "Rate"}
        'Dim ColSize() As Integer = {100, 100, 100}

        '' Create column boundfield
        'For i = 0 To ColData.Length - 1
        '    Dim Bfield As BoundField = New BoundField()
        '    Bfield.DataField = ColData(i)
        '    Bfield.HeaderText = Replace(ColData(i), "_", " ")
        '    Bfield.HeaderStyle.Width = ColSize(i)
        '    Bfield.HeaderStyle.Wrap = False
        '    Bfield.ItemStyle.Wrap = False
        '    GridView1.Columns.Add(Bfield)
        'Next

        '' Add templatefield for Edit icon
        'Dim TField As TemplateField = New TemplateField()
        'TField.HeaderStyle.Width = Unit.Percentage(2)
        'TField.ItemStyle.Wrap = False
        'TField.ItemTemplate = New GridViewItemTemplateControl()
        'GridView1.Columns.Add(TField)
    End Sub



    '' Gridview events
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Edit Button
            'Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1
            'Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            'EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            'EditLinkButton.CssClass = "btn btn-xs btn-info"
            'EditLinkButton.CommandArgument = drv("ID") & "|" & drv("Date") & "|" & drv("Currency") & "|" & drv("Rate")
            'EditLinkButton.CausesValidation = False
            'AddHandler EditLinkButton.Click, AddressOf Edit_ExchangeRate_Click

            'Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(1), LinkButton)
            'DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
            'DeleteLinkButton.CommandArgument = drv("ID")
            'DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
            'DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            'AddHandler DeleteLinkButton.Click, AddressOf Delete_ExchangeRate_Click

            'EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub




    '' Modal control
    Protected Sub DDL_Currency_Load(sender As Object, e As EventArgs) Handles DDL_Currency.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "
                DDL_Currency.DataSource = GetDataTable(sqlStr)
                DDL_Currency.DataTextField = "Currency"
                DDL_Currency.DataValueField = "Currency"
                DDL_Currency.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub Edit_ExchangeRate_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderExchangeRate.Text = "Update Exchange Rate"
        btnUpdateExchangeRate.Text = "Update"
        btnCancelExchangeRate.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditStringArray As String() = Split(EditLinkButton.CommandArgument, "|")

        '' Assign value to hidden fields
        TB_Selected_ID.Text = EditStringArray(1)
        TB_Selected_Date.Text = EditStringArray(2)
        TB_Selected_Currency.Text = EditStringArray(3)
        TB_Selected_Rate.Text = EditStringArray(4)

        '' Asisgn value to modal fields
        TB_Date.Text = TB_Selected_Date.Text
        DDL_Currency.SelectedIndex = DDL_Currency.Items.IndexOf(DDL_Currency.Items.FindByValue(TB_Selected_Currency.Text))
        TB_Rate.Text = TB_Selected_Rate.Text

        RequiredField_TB_Rate.Enabled = True
        RegEx_TB_Rate.Enabled = True

        popupExchangeRate.Show()
    End Sub

    Protected Sub Update_ExchangeRate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateExchangeRate.Click
        Dim ID As TextBox = pnlUpdateExchangeRate.FindControl("TB_Selected_ID")
        Dim ExchangeDate As TextBox = pnlUpdateExchangeRate.FindControl("TB_Date")
        Dim Currency As DropDownList = pnlUpdateExchangeRate.FindControl("DDL_Currency")
        Dim Rate As TextBox = pnlUpdateExchangeRate.FindControl("TB_Rate")

        Try
            Dim sqlStr As String = "UPDATE DB_Exchange_History SET Rate = " & Rate.Text & " WHERE ID = " & ID.Text
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_ExchangeRate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelExchangeRate.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Delete_ExchangeRate_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteStringArray As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = " DELETE FROM DB_Exchange_History WHERE ID =" & DeleteStringArray(1)
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
        PopulateGridViewData(TB_Search.Text)
    End Sub


    Protected Sub UploadFile(ByVal sender As Object, ByVal e As EventArgs) Handles BN_Upload.Click
        Dim filename As String = Path.GetFileName(FileUpload1.PostedFile.FileName)
        Dim csvPath As String = Server.MapPath("~/Uploads/") + AppendDatetime() + "_" + filename
        Dim dt As New DataTable()

        FileUpload1.SaveAs(csvPath)
        dt.Columns.AddRange(New DataColumn(3) {New DataColumn("ID", GetType(Integer)),
                                               New DataColumn("Date", GetType(String)),
                                               New DataColumn("Currency", GetType(String)),
                                               New DataColumn("Rate", GetType(Decimal))})

        Dim csvData As String = File.ReadAllText(csvPath)
        Try
            For Each row As String In csvData.Split(ControlChars.Lf)
                If Not String.IsNullOrEmpty(row) Then
                    row = Replace(row, "/", "-")
                    dt.Rows.Add()
                    Dim i As Integer = 1   ' 1 means start writting data from Date column
                    For Each cell As String In row.Split(","c)
                        dt.Rows(dt.Rows.Count - 1)(i) = cell
                        i += 1
                    Next
                Else
                    Exit For
                End If
            Next

            Dim consString As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString
            Using con As New SqlConnection(consString)
                Using sqlBulkCopy As New SqlBulkCopy(con)
                    'Set the database table name
                    sqlBulkCopy.DestinationTableName = "dbo.DB_Exchange_History"
                    con.Open()
                    sqlBulkCopy.WriteToServer(dt)
                    con.Close()
                End Using
            End Using

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('" & dt.Rows.Count & " record(s) uploaded.');</script>")
        Catch ex As Exception
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('Upload template error.\nPlease check template content.');</script>")
        End Try

        TB_Search.Text = String.Empty
        PopulateGridViewData(TB_Search.Text)
    End Sub


End Class
