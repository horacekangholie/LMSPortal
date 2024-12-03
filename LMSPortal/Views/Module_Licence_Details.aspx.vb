
Partial Class Views_Module_Licence_Details
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)
        LB_PageTitle.Text = Get_Value("SELECT Name FROM R_Customer_List WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ", "Name")

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" And Not Request.QueryString("Customer_ID") Is Nothing Then
                PopulateFormViewData()
            Else
                Response.Redirect("~/Login.aspx")
            End If
        End If
        PopulateGridViewData()

    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr As String = " SELECT * FROM Master_Customer WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' "
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData()
        Try
            Dim sqlStr() As String = {"EXEC SP_Module_Licence_Order '" & Request.QueryString("Customer_ID") & "' ",
                                      "SELECT [Customer ID], [PO No], [PO Date], [Chargeable], [Invoice No], STRING_AGG([Requested By], ', ') AS [Requested By] " &
                                      "     , (SELECT CAST(COUNT(*) AS nvarchar) FROM R_LMS_Module_Licence WHERE [Customer ID] = TBL.[Customer ID] AND [PO No] = TBL.[PO No] AND Status = 'Activated') + ' / ' + CAST(SUM([No of Licence Key Issued]) AS nvarchar) AS [No of Licence Key Issued] " &
                                      "FROM (" &
                                      "   SELECT [Customer ID], [PO No], [PO Date], [Chargeable], [Invoice No], CASE WHEN [Invoice No] = 'NA' THEN '' ELSE [Requested By] END AS [Requested By], COUNT([Licence Code]) AS [No of Licence Key Issued] " &
                                      "   FROM R_LMS_Module_Licence " &
                                      "   WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                                      "   GROUP BY [Customer ID], [PO No], [PO Date], [Chargeable], [Invoice No], [Invoice Date], CASE WHEN [Invoice No] = 'NA' THEN '' ELSE [Requested By] END " &
                                      ") TBL " &
                                      "GROUP BY [Customer ID], [PO No], [PO Date], [Chargeable], [Invoice No] " &
                                      "ORDER BY [Chargeable] DESC, [PO Date] DESC",
                                      "SELECT * FROM R_LMS_Module_Licence_Pool WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' ORDER BY Customer_ID, No, Module_Type DESC",
                                      "SELECT * FROM I_AI_Licence_Renewal WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ORDER BY [Expired Date] ",
                                      "SELECT [UID], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], SUM(Fee) AS [Total Amount], [Renewal Date] FROM R_AI_Licence_Renewal WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' GROUP BY [UID], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], [Renewal Date] ORDER BY [UID] DESC ",
                                      "SELECT *, CASE WHEN DATEDIFF(D, Added_Date, GETDATE()) > 90 THEN 1 ELSE 0 END AS Is_Locked FROM DB_Account_Notes WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' AND Notes_For = 'Module Licence' ORDER BY Added_Date DESC, ID DESC "}

            BuildGridView(GridView1, "GridView1", "PO No")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Customer ID")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Customer_ID")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "Licence Code")
            GridView4.DataSource = GetDataTable(sqlStr(3))
            GridView4.DataBind()

            BuildGridView(GridView5, "GridView5", "UID")
            GridView5.DataSource = GetDataTable(sqlStr(4))
            GridView5.DataBind()

            BuildGridView(GridView6, "GridView6", "ID")
            GridView6.DataSource = GetDataTable(sqlStr(5))
            GridView6.DataBind()

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' display footer no of activated AI Licences
        If CInt(GridView4.Rows.Count.ToString()) > 0 Then
            FooterTotalCount.Visible = True
            FooterTotalCount.Text = "Record(s) found: " & GridView4.Rows.Count.ToString()
        Else
            FooterTotalCount.Visible = False
        End If

        '' Draw last line if page count less than 1
        If GridView6.PageCount < 2 Then
            GridView6.Style.Add("border-bottom", "1px solid #ddd")
        Else
            GridView6.Style.Add("border-bottom", "1px solid #fff !important")
        End If

    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        'GridViewObj.AutoGenerateColumns = False   '' move AutoGenerateColumns to each of Gridview
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowFooter = True
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
            Case "GridView1"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 5
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"PO No", "PO Date", "Invoice No", "Invoice Date", "Created Date", "Requested By", "e.Sense", "BYOC", "AI"}
                Dim ColData() As String = {"PO No", "PO Date", "Invoice No", "Invoice Date", "Created Date", "Requested By", "e.Sense", "BYOC", "AI"}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView2"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"PO No", "PO Date", "Invoice No", "Activated / No of Licence Key", "Requested By"}
                Dim ColData() As String = {"PO No", "PO Date", "Invoice No", "No of Licence Key Issued", "Requested By"}
                Dim ColSize() As Integer = {100, 50, 100, 50, 100}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New LicenceNestedGridViewItemTemplate()
                Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                GridViewObj.Columns.Add(Expandfield)

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColName(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView3"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = False
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Name", "Module_Type", "Balance", "Used"}
                Dim ColSize() As Integer = {200, 100, 50, 50}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Balance") Or Bfield.HeaderText.Contains("Used") Then

                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView4"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 20
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Activated Date", "Expired Date", "Status", "Requested By"}
                Dim ColSize() As Integer = {100, 50, 50, 100, 100, 50, 50, 50, 100}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView5"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"UID", "PO No", "PO Date", "Invoice No", "Invoice Date", "Currency", "Total Amount", "Renewal Date"}
                Dim ColSize() As Integer = {100, 100, 50, 100, 50, 50, 100, 50}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New AIRenewalNestedGridViewItemTemplate()
                Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                GridViewObj.Columns.Add(Expandfield)

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Width = ColSize(i)
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                    End If
                    If Bfield.HeaderText.Contains("Amount") Then
                        Bfield.DataFormatString = "{0:#,##0.00}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView6"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #ddd")
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Added_Date", "Notes"}
                Dim ColSize() As Unit = {Unit.Percentage(2), Unit.Percentage(95)}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.ItemStyle.Width = ColSize(i)   '' when GridViewObj.ShowHeader is false then use itemstyle to set width
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
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


    '' FormView control
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub DDL_Country_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Country As Label = FormView1.FindControl("LB_Country")
        Dim DDL_Country As DropDownList = FormView1.FindControl("DDL_Country")
        Dim i = DDL_Country.Items.IndexOf(DDL_Country.Items.FindByText(LB_Country.Text))
        i = IIf(i < 0, 0, i)
        DDL_Country.SelectedIndex = i
    End Sub

    Protected Sub DDL_Type_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Type As Label = FormView1.FindControl("LB_Type")
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim i = DDL_Type.Items.IndexOf(DDL_Type.Items.FindByText(LB_Type.Text))
        i = IIf(i < 0, 0, i)
        DDL_Type.Items(i).Selected = True
    End Sub

    Protected Sub DDL_Type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim DDL_Type As DropDownList = FormView1.FindControl("DDL_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(DDL_Type.SelectedItem.Text <> "Customer", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
    End Sub

    Protected Sub DDL_Group_ID_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim LB_Group As Label = FormView1.FindControl("LB_Group_ID")
        Dim DDL_Group As DropDownList = FormView1.FindControl("DDL_Group_ID")
        Dim i = DDL_Group.Items.IndexOf(DDL_Group.Items.FindByValue(LB_Group.Text))
        i = IIf(i < 0, 0, i)
        DDL_Group.Items(i).Selected = True
    End Sub

    Protected Sub DDL_By_Distributor_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim Type As Label = FormView1.FindControl("LB_Type")
        Dim To_Display_By_Distributor As Boolean = IIf(Type.Text <> "Customer" And Type.Text <> "", False, True)
        FormView1.FindControl("lbl_By_Distributor").Visible = To_Display_By_Distributor
        FormView1.FindControl("DDL_By_Distributor").Visible = To_Display_By_Distributor
        Dim By_Distributor As Label = FormView1.FindControl("LB_By_Distributor")
        Dim DDL_By_Distributor As DropDownList = FormView1.FindControl("DDL_By_Distributor")
        Dim i = DDL_By_Distributor.Items.IndexOf(DDL_By_Distributor.Items.FindByValue(By_Distributor.Text))
        DDL_By_Distributor.Items(i).Selected = True
    End Sub


    '' Gridview control
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" And drv("Invoice No") <> UCase("Cancelled") Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            ElseIf drv("Invoice No") = UCase("Cancelled") Then
                '' if the order is cancelled then display Cancelled
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = drv("Invoice No")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("color", "#999999")
            End If

            Dim ModuleLicenceColumnIndex As Integer = GetColumnIndexByName(e.Row, "e.Sense")
            For i = ModuleLicenceColumnIndex To e.Row.Cells.Count - 1
                e.Row.Cells(i).Width = 70
            Next

        End If
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim Licence_Code As GridView = TryCast(e.Row.FindControl("gvLicenceList"), GridView)
            Dim query As String = " SELECT [Customer ID] " &
                                  "      , ISNULL([Application Type] + ' (' + Activated_Module_Type + ') ', [Application Type]) AS [Application Type] " &
                                  "      , [OS Type], [Chargeable] " &
                                  "      , [Created Date], [Licence Code], [Status], [MAC Address], [Email] " &
                                  "      , [Activated Date], [Expired Date], [Remarks], [Requested By] " &
                                  " FROM R_LMS_Module_Licence " &
                                  " LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(R_LMS_Module_Licence.[Licence Code], '-', '') " &
                                  " WHERE [Customer ID] = '" & Customer_ID & "'" &
                                  "   AND [PO No] = '" & PO_No & "'" &
                                  " ORDER BY [Created Date] DESC "

            Try
                Licence_Code.DataSource = GetDataTable(query)
                Licence_Code.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' Set fixed column width
            Dim FixedColSize() As Integer = {18, 6, 5, 6, 1, 7, 5, 5, 7, 5, 12, 24}
            For i = 0 To FixedColSize.Length - 1
                Licence_Code.Columns(i).ItemStyle.Width = Unit.Percentage(FixedColSize(i))
                If i = GetColumnIndexByColumnName(Licence_Code, "Requested By") Then
                    '' display the Child Gridview Requested By column when the PO No is NA
                    Licence_Code.Columns(i).Visible = IIf(PO_No = "NA", True, False)
                    Licence_Code.Columns(i + 1).ItemStyle.Width = IIf(PO_No = "NA", Unit.Percentage(FixedColSize(i + 1)), Unit.Percentage(FixedColSize(i) + FixedColSize(i + 1)))
                End If
            Next

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" And drv("Invoice No") <> UCase("Cancelled") Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            ElseIf drv("Invoice No") = UCase("Cancelled") Then
                '' if the order is cancelled then display Cancelled
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = drv("Invoice No")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("color", "#999999")
            End If

            '' if PO is NA then requestor set to (multiple)
            If PO_No = "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Text = "(multiple)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("color", "#999999")
            End If

        End If
    End Sub

    Protected Sub GridView2_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        GridView2.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim LicenseStatus As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text
            Select Case LicenseStatus
                Case "Activated"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-success'>" & LicenseStatus & "</span>"
                Case "Renew"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-info'>" & LicenseStatus & "</span>"
                Case "Expired"
                    e.Row.Cells(GetColumnIndexByName(e.Row, "Status")).Text = "<span class='badge bg-danger'>" & LicenseStatus & "</span>"
            End Select
        End If
    End Sub

    Protected Sub GridView4_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView4.PageIndexChanging
        GridView4.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim UID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim AIRenewal As GridView = TryCast(e.Row.FindControl("gvAILicenceList"), GridView)
            Dim query As String = " SELECT * FROM R_AI_Licence_Renewal WHERE [UID] ='" & UID & "' "
            Try
                AIRenewal.DataSource = GetDataTable(query)
                AIRenewal.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

        End If
    End Sub

    Protected Sub GridView5_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView5.PageIndexChanging
        GridView4.PageIndex = e.NewPageIndex
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub



    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Views/Module_Licence_List.aspx")
    End Sub

End Class
