Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Partial Class Form_App_Product_Licence_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Register App / Product Licence"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        FormView1.ChangeMode(FormViewMode.ReadOnly)
        LB_PageTitle.Text = PageTitle

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" And Not Request.QueryString("Customer_ID") Is Nothing Then
                PopulateFormViewData()
            Else
                Response.Redirect("~/Form/App_Product_Licence.aspx")
            End If
            hiddenModalVisible.Value = False
        Else
            hiddenModalVisible.Value = True
        End If
        PopulateGridViewData()

        '' Sync button with bootstrap icons
        AILicenceRefresh.Text = "Sync " & "<i class='bi bi-arrow-repeat align-middle' style='display:inline-block;font-size:1.8rem;'></i>"

        '' correct modal behavior
        If hiddenModalVisible.Value Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ScrollPage", "window.scrollTo(0, document.body.scrollHeight);", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ScrollPage", "window.scrollTo(0, 0);", True)
        End If
    End Sub

    Protected Sub PopulateFormViewData()
        Try
            Dim sqlStr As String = " Select * FROM Master_Customer WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' "
            FormView1.DataSource = GetDataTable(sqlStr)
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr() As String = {"SELECT * FROM R_LMS_Licence_Order WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' AND [PO No] IN (SELECT PO_No FROM LMS_Licence WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' AND Licence_Code LIKE '%" & keyword & "%') ORDER BY CASE [PO No] WHEN 'NA' THEN 2 ELSE 1 END, [PO Date] DESC ",
                                      "SELECT * FROM _LicenceUsage WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ",
                                      "SELECT * FROM I_Termed_Licence_Renewal WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ",
                                      "SELECT [UID], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], SUM(Fee) AS [Total Amount], [Renewal Date] FROM R_Termed_Licence_Renewal WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' GROUP BY [UID], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], [Renewal Date] ORDER BY [UID] DESC ",
                                      "SELECT *, CASE WHEN DATEDIFF(D, Added_Date, GETDATE()) > 90 THEN 1 ELSE 0 END AS Is_Locked FROM DB_Account_Notes WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' AND Notes_For = 'App Product Licence' ORDER BY Added_Date DESC, ID DESC ",
                                      "SELECT * FROM R_AI_Gateway_Licence WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' AND ([Token] LIKE '%" & keyword & "%' OR [Licence Code] LIKE '%" & keyword & "%')  ORDER BY [Created Date] DESC "}

            BuildGridView(GridView1, "GridView1", "Customer ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()

            BuildGridView(GridView2, "GridView2", "Customer ID")
            GridView2.DataSource = GetDataTable(sqlStr(1))
            GridView2.DataBind()

            BuildGridView(GridView3, "GridView3", "Customer ID")
            GridView3.DataSource = GetDataTable(sqlStr(2))
            GridView3.DataBind()

            BuildGridView(GridView4, "GridView4", "UID")
            GridView4.DataSource = GetDataTable(sqlStr(3))
            GridView4.DataBind()

            BuildGridView(GridView5, "GridView5", "ID")
            GridView5.DataSource = GetDataTable(sqlStr(4))
            GridView5.DataBind()

            BuildGridView(GridView6, "GridView6", "Token")
            GridView6.DataSource = GetDataTable(sqlStr(5))
            GridView6.DataBind()

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Draw last line if page count less than 1
        If GridView5.PageCount < 2 Then
            GridView5.Style.Add("border-bottom", "1px solid #ddd")
        Else
            GridView5.Style.Add("border-bottom", "1px solid #fff !important")
        End If
    End Sub

    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
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

                '' Add template field for the delete button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView2"
                '' Build GridView Content
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Licence Type", "New", "Used"}
                Dim ColSize() As Integer = {300, 50, 50}

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

            Case "GridView3"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = False
                GridViewObj.PageSize = 20
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"Licence Code", "Application Type", "MAC Address", "Activated Date", "Expired Date", "Status", "PO No", "PO Date", "Requested By"}
                Dim ColSize() As Integer = {100, 100, 50, 50, 50, 50, 50, 50, 100}

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

            Case "GridView4"
                '' Build GridView Content
                GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"UID", "PO No", "PO Date", "Invoice No", "Invoice Date", "Currency", "Total Amount", "Renewal Date"}
                Dim ColSize() As Integer = {100, 100, 50, 100, 50, 50, 100, 50}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New TermedLicenceRenewalNestedGridViewItemTemplate()
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

                '' Add template field for the delete button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView5"
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

                '' Add template field for the delete button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

            Case "GridView6"
                '' Build GridView Content
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.Columns.Clear()
                Dim ColName() As String = {"PO No", "PO Date", "Licence Code", "Token", "Created Date", "AI Account No", "Activated Date", "Expired Date", "Status"}
                Dim ColData() As String = {"PO No", "PO Date", "Licence Code", "Token", "Created Date", "AI Account No", "Activated Date", "Expired Date", "Status"}
                Dim ColSize() As Integer = {50, 50, 100, 10, 50, 50, 50, 50, 100}

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
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim Requested_By As String = e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Text
            Dim Licence_Code As GridView = TryCast(e.Row.FindControl("gvLicenceList"), GridView)

            Dim query As String = " SELECT [Customer ID], [Application Type], [OS Type], [Chargeable] " &
                                  "      , [Created Date], [Licence Code], [Status], [MAC Address], [Email] " &
                                  "      , [Activated Date], [Expired Date], [Remarks], [Requested By] " &
                                  " FROM R_LMS_Licence " &
                                  " WHERE [Customer ID] = '" & Customer_ID & "'" &
                                  "   AND [PO No] = N'" & PO_No & "'"

            '' Separated record based requestor
            'If Len(RemoveHTMLWhiteSpace(Requested_By)) > 0 Then
            '    query += " AND [Requested By] = '" & Requested_By & "'"
            'End If
            query += " ORDER BY [Created Date] DESC "

            Try
                Licence_Code.DataSource = GetDataTable(query)
                Licence_Code.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' display the Child Gridview Requested By column when the PO No is NA
            Licence_Code.Columns(GetColumnIndexByColumnName(Licence_Code, "Requested By")).Visible = IIf(PO_No = "NA", True, False)

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            'Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            'InvoiceDownloadLink.ID = "lnkDownload"
            'InvoiceDownloadLink.Text = drv("Invoice No")
            'If drv("Invoice No") <> "" And drv("Invoice No") <> "NA" And drv("Invoice No") <> UCase("Cancelled") Then
            '    e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
            '    InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
            '    InvoiceDownloadLink.Target = "_blank"
            'ElseIf drv("Invoice No") = UCase("Cancelled") Then
            '    '' if the order is cancelled then display Cancelled
            '    e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = drv("Invoice No")
            '    e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("font-style", "italic")
            '    e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Style.Add("color", "#999999")
            'End If


            '' Invoice Download Link - Handling multiple invoice numbers
            Dim InvoiceCell As TableCell = e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No"))
            Dim InvoiceNumbers As String() = drv("Invoice No").ToString().Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)

            For i As Integer = 0 To InvoiceNumbers.Length - 1
                Dim invoiceNo As String = InvoiceNumbers(i).Trim()
                If invoiceNo <> "" And invoiceNo <> "NA" And invoiceNo <> UCase("Cancelled") Then
                    Dim InvoiceDownloadLink As New HyperLink()
                    InvoiceDownloadLink.Text = invoiceNo
                    InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", invoiceNo)
                    InvoiceDownloadLink.Target = "_blank"
                    InvoiceCell.Controls.Add(InvoiceDownloadLink)

                    ' Add a separator if there are more invoices after the current one
                    If i < InvoiceNumbers.Length - 1 Then
                        InvoiceCell.Controls.Add(New LiteralControl(", "))
                    End If
                ElseIf invoiceNo = UCase("Cancelled") Then
                    ' if the order is cancelled then display Cancelled
                    InvoiceCell.Text = "Cancelled"
                    InvoiceCell.Style.Add("font-style", "italic")
                    InvoiceCell.Style.Add("color", "#999999")
                End If
            Next



            '' if PO is NA then requestor set to (multiple)
            If PO_No = "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Text = "(multiple)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Requested By")).Style.Add("color", "#999999")
            End If



            '' Check if any license within the same PO order activated (being used)
            Dim Activated_vs_Total_Licence As Array = Split(Replace(drv("No of Licence Key Issued"), " ", ""), "/")
            Dim toLockDelete As Boolean = IIf(CInt(Activated_vs_Total_Licence(0)) > 0, True, False)

            '' Control Button
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(CtrlCellIndex).Controls(1), LinkButton)
            DeleteLinkButton.Text = If(Len(Trim(drv("Invoice No"))) <= 0 And Not toLockDelete, "<i class='bi bi-trash'></i>", "<i class='bi bi-lock'></i>")
            DeleteLinkButton.CssClass = If(Len(Trim(drv("Invoice No"))) <= 0 And Not toLockDelete, "btn btn-xs btn-danger", "btn btn-xs btn-light disabled")
            DeleteLinkButton.ToolTip = If(Len(Trim(drv("Invoice No"))) <= 0 And Not toLockDelete, "", "Item Locked")
            DeleteLinkButton.Enabled = Len(Trim(drv("Invoice No"))) <= 0 And Not toLockDelete   '' Lock/disable the button if the license order is billed
            DeleteLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("Customer ID") & "|" & drv("PO No")
            DeleteLinkButton.CausesValidation = False
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Click, AddressOf Delete_AppProductLicence_Click

        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound

    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound

    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim UID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim TermedLicenceRenewal As GridView = TryCast(e.Row.FindControl("gvTermedLicenceList"), GridView)
            Dim query As String = "SELECT * FROM R_Termed_Licence_Renewal WHERE [UID] ='" & UID & "' "
            Try
                TermedLicenceRenewal.DataSource = GetDataTable(query)
                TermedLicenceRenewal.DataBind()
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


            '' Delete Button
            Dim DeletectrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(DeletectrlCellIndex).Controls(0), LinkButton)
            DeleteLinkButton.CommandArgument = drv("UID")

            '' Lock record if status is not 'New'
            If Trim(drv("Invoice No")) <> "" Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
                DeleteLinkButton.Enabled = True
            End If

            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            DeleteLinkButton.CausesValidation = False
            AddHandler DeleteLinkButton.Click, AddressOf Delete_TermedLicenceRenewal_Click
        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview
            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton

            '' Disable delete button when notes is_locked status is 1
            If drv("Is_Locked") = 1 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
                DeleteLinkButton.Enabled = True
                DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
                DeleteLinkButton.CausesValidation = False
                DeleteLinkButton.CommandArgument = drv("ID")
                AddHandler DeleteLinkButton.Click, AddressOf Delete_Notes_Click
            End If
        End If
    End Sub

    Protected Sub GridView_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) _
        Handles GridView1.PageIndexChanging, GridView2.PageIndexChanging, GridView3.PageIndexChanging, GridView4.PageIndexChanging, GridView5.PageIndexChanging, GridView6.PageIndexChanging

        Dim CurrActiveGV As GridView = CType(sender, GridView)
        CurrActiveGV.PageIndex = e.NewPageIndex

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub



    '' Modal control

    '' App / Product Licence

    Protected Sub TB_PO_No_TextChanged(sender As Object, e As EventArgs) Handles TB_PO_No.TextChanged
        Dim PO_No As TextBox = pnlAddEditAppProductLicence.FindControl("TB_PO_No")
        Dim DDL_Chargeable As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Chargeable")

        '' when the PO No is NA, then do not need to input PO_Date
        If PO_No.Text.ToUpper <> "NA" Then
            TB_PO_Date.Enabled = True
            RequiredField_TB_PO_Date.Enabled = True
            Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
            DDL_Chargeable.SelectedIndex = i
        Else
            TB_PO_Date.Text = String.Empty
            TB_PO_Date.Enabled = False
            RequiredField_TB_PO_Date.Enabled = False
            Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("No"))
            DDL_Chargeable.SelectedIndex = i
        End If
        popupAppProductLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub DDL_Application_Type_Load(sender As Object, e As EventArgs) Handles DDL_Application_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_3 AS Application_Type " &
                                       " FROM DB_Lookup " &
                                       " WHERE Lookup_Name = 'Bill Items' AND Value_4 IN ('App Licence', 'DMC Server Licence Key') " &
                                       " ORDER BY Application_Type "

                DDL_Application_Type.DataSource = GetDataTable(sqlStr)
                DDL_Application_Type.DataTextField = "Application_Type"
                DDL_Application_Type.DataValueField = "Application_Type"
                DDL_Application_Type.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Application_Type_DataBound(sender As Object, e As EventArgs) Handles DDL_Application_Type.DataBound
        Dim DDL_Application_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Application_Type")
        Dim i = DDL_Application_Type.Items.IndexOf(DDL_Application_Type.Items.FindByText("TPOP IB"))
        DDL_Application_Type.SelectedIndex = i
    End Sub

    Protected Sub DDL_Application_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Application_Type.SelectedIndexChanged
        Dim Application_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Application_Type")
        Dim OS_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_OS_Type")

        '' Change OS type based on application type selected
        If Trim(Application_Type.Text).ToLower.Contains("queue printer") Or
            Trim(Application_Type.Text).ToLower.Contains("delious mobile order") Or
            Trim(Application_Type.Text).ToLower.Contains("tpop pda - trk - sd") Or
            Trim(Application_Type.Text).ToLower.Contains("tpop infopad") Or
            Trim(Application_Type.Text).ToLower.Contains("tpop pick right") Then
            OS_Type.SelectedIndex = OS_Type.Items.IndexOf(OS_Type.Items.FindByText("Android"))
        Else
            OS_Type.SelectedIndex = OS_Type.Items.IndexOf(OS_Type.Items.FindByText("Web"))
        End If

        '' Check to turn on / off AI Account Selection based on Licence Type
        If Trim(Application_Type.Text).ToLower.Contains("ai gateway") Then
            aiaccountno.Visible = True
            CompareValidator_DDL_AI_Account_No.Enabled = True
            DDL_AI_Account_No.SelectedIndex = -1   '' Default the selection
        Else
            aiaccountno.Visible = False
            CompareValidator_DDL_AI_Account_No.Enabled = False
        End If

        popupAppProductLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub DDL_Sales_Representative_Load(sender As Object, e As EventArgs) Handles DDL_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "
                DDL_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_Sales_Representative.DataTextField = "Name"
                DDL_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_OS_Type_Load(sender As Object, e As EventArgs) Handles DDL_OS_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_1 AS OS_Type FROM DB_Lookup WHERE Lookup_Name = 'OS Type' "
                DDL_OS_Type.DataSource = GetDataTable(sqlStr)
                DDL_OS_Type.DataTextField = "OS_Type"
                DDL_OS_Type.DataValueField = "OS_Type"
                DDL_OS_Type.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_OS_Type_DataBound(sender As Object, e As EventArgs) Handles DDL_OS_Type.DataBound
        Dim DDL_OS_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_OS_Type")
        Dim i = DDL_OS_Type.Items.IndexOf(DDL_OS_Type.Items.FindByValue("Web"))
        DDL_OS_Type.SelectedIndex = i
    End Sub

    Protected Sub DDL_Chargeable_Load(sender As Object, e As EventArgs) Handles DDL_Chargeable.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_2, Value_1 FROM DB_Lookup WHERE Lookup_Name = 'YesNo' "
                DDL_Chargeable.DataSource = GetDataTable(sqlStr)
                DDL_Chargeable.DataTextField = "Value_1"
                DDL_Chargeable.DataValueField = "Value_2"
                DDL_Chargeable.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Chargeable_DataBound(sender As Object, e As EventArgs) Handles DDL_Chargeable.DataBound
        Dim DDL_Chargeable As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Chargeable")
        Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
        DDL_Chargeable.SelectedIndex = i
    End Sub

    Protected Sub DDL_AI_Account_No_Load(sender As Object, e As EventArgs) Handles DDL_AI_Account_No.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Client_ID AS [Account No], Client_ID + ' ' + User_Group AS [Account Name] " &
                                       " FROM CZL_Account " &
                                       " WHERE LEN(AI_Gateway_Key) < 1 " &
                                       " ORDER BY CAST(Client_ID AS int) DESC "

                DDL_AI_Account_No.DataSource = GetDataTable(sqlStr)
                DDL_AI_Account_No.DataTextField = "Account Name"
                DDL_AI_Account_No.DataValueField = "Account No"
                DDL_AI_Account_No.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub


    Protected Sub Add_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddAppProductLicence.Click
        ModalHeaderAppProductLicence.Text = "Add App / Product Licence"
        btnSaveAppProductLicence.Text = "Save"
        btnCancelAppProductLicence.Text = "Cancel"

        TB_PO_No.Text = String.Empty
        TB_PO_Date.Text = String.Empty
        TB_PO_Date.Enabled = True
        RequiredField_TB_PO_Date.Enabled = True
        DDL_Application_Type.SelectedIndex = -1
        DDL_Sales_Representative.SelectedIndex = -1

        Dim DDL_Chargeable As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Chargeable")
        Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
        DDL_Chargeable.SelectedIndex = i
        TB_Email.Text = String.Empty
        TB_Remarks.Text = String.Empty

        '' AI Account Selection Dropdownlist set to invisible
        aiaccountno.Visible = False
        CompareValidator_DDL_AI_Account_No.Enabled = False
        DDL_AI_Account_No.SelectedIndex = -1

        '' hide the tr row when the error message is.
        licencelistboxerrormsg.Visible = False

        PopulateListbox()
        popupAppProductLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub UploadLineItems_Click(sender As Object, e As EventArgs) Handles UploadLineItems.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditAppProductLicence.FindControl("TB_PO_No")
        Dim Application_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Application_Type")
        Dim OS_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_OS_Type")
        Dim Email As TextBox = pnlAddEditAppProductLicence.FindControl("TB_Email")
        Dim Sales_Representative_ID As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Sales_Representative")
        Dim Chargeable As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Chargeable")
        Dim Remarks As TextBox = pnlAddEditAppProductLicence.FindControl("TB_Remarks")

        '' Prepare data upload to LMS_Licence_Staging table
        Dim filename As String = Path.GetFileName(FileUpload1.PostedFile.FileName)
        Dim csvPath As String = Server.MapPath("~/Uploads/") + AppendDatetime() + "_" + filename
        Dim dt As New DataTable()

        FileUpload1.SaveAs(csvPath)
        dt.Columns.AddRange(New DataColumn(9) {New DataColumn("ID", GetType(Integer)),
                                               New DataColumn("Customer_ID", GetType(String)),
                                               New DataColumn("PO_No", GetType(String)),
                                               New DataColumn("Application_Type", GetType(String)),
                                               New DataColumn("OS_Type", GetType(String)),
                                               New DataColumn("Licence_Code", GetType(String)),
                                               New DataColumn("Email", GetType(String)),
                                               New DataColumn("Sales_Rep_ID", GetType(String)),
                                               New DataColumn("Chargeable", GetType(String)),
                                               New DataColumn("Remarks", GetType(String))
                                               })
        Dim csvData As String = File.ReadAllText(csvPath)
        Try
            Dim ColumnValue As String() = {Nothing,
                                           Customer_ID,
                                           PO_No.Text,
                                           Application_Type.Text,
                                           OS_Type.Text,
                                           Nothing,
                                           Email.Text,
                                           Sales_Representative_ID.SelectedValue,
                                           CBool(Chargeable.SelectedValue),
                                           EscapeChar(Remarks.Text)}

            For Each row As String In csvData.Split(ControlChars.Lf)
                If Not String.IsNullOrEmpty(row) Then
                    row = Replace(row, "/", "-")
                    dt.Rows.Add()
                    Dim i As Integer = 1
                    For Each cell As String In row.Split(","c)
                        For j = 1 To dt.Columns.Count - 1
                            If j = 5 Then
                                dt.Rows(dt.Rows.Count - 1)(j) = FormatLicenceCode(cell.Replace(vbCrLf, "")) ' insert dashes into licencecode
                            Else
                                dt.Rows(dt.Rows.Count - 1)(j) = ColumnValue(j)
                            End If
                        Next
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
                    sqlBulkCopy.DestinationTableName = "dbo.LMS_Licence_staging"
                    con.Open()
                    sqlBulkCopy.WriteToServer(dt)
                    con.Close()
                End Using
            End Using

        Catch ex As Exception
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('Import template error.\nPlease check template content.');</script>")
        End Try

        licencelistboxerrormsg.Visible = False

        PopulateListbox()
        popupAppProductLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub btnClearLineItems_Click(sender As Object, e As EventArgs) Handles btnClearLineItems.Click
        DeleteStaging()
        licencelistboxerrormsg.Visible = False
        PopulateListbox()
        popupAppProductLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub PopulateListbox()
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditAppProductLicence.FindControl("TB_PO_No")

        Try
            Dim sqlStr As String = " SELECT * FROM LMS_Licence_staging " &
                                   " WHERE Customer_ID = '" & Customer_ID & "'" &
                                   "   AND PO_No = N'" & PO_No.Text & "'"

            GridView_Licence_List.DataSource = GetDataTable(sqlStr)
            GridView_Licence_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub Save_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveAppProductLicence.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditAppProductLicence.FindControl("TB_PO_No")
        Dim PO_Date As TextBox = pnlAddEditAppProductLicence.FindControl("TB_PO_Date")

        If PO_No.Text <> "NA" Then
            Dim Existing_PO_Date As String = Get_Value("SELECT TOP 1 CONVERT(varchar, PO_Date, 23) AS PO_Date FROM LMS_Licence WHERE PO_No = '" & PO_No.Text & "' ORDER BY PO_Date", "PO_Date")
            PO_Date.Text = IIf(Existing_PO_Date <> "", Existing_PO_Date, PO_Date.Text)    '' If there is existing Licence PO record, get the PO Date from existing record
        End If


        '' Get these value from modal control instead of staging table
        Dim Application_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Application_Type")
        Dim Sales_Representative_ID As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Sales_Representative")
        Dim Chargeable As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_Chargeable")
        Dim OS_Type As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_OS_Type")
        Dim Email As TextBox = pnlAddEditAppProductLicence.FindControl("TB_Email")
        Dim Remarks As TextBox = pnlAddEditAppProductLicence.FindControl("TB_Remarks")

        Dim AI_Account_No As DropDownList = pnlAddEditAppProductLicence.FindControl("DDL_AI_Account_No")
        Dim Selected_AI_Account_No As String = IIf(AI_Account_No.SelectedIndex < 0, 0, AI_Account_No.SelectedValue)

        Dim GridView_Licence_List As GridView = pnlAddEditAppProductLicence.FindControl("GridView_Licence_List")
        Dim UploadedRecordCount As Integer = GridView_Licence_List.Rows.Count


        If Application_Type.SelectedValue = "AI Gateway" AndAlso UploadedRecordCount <> 1 Then
            If UploadedRecordCount = 0 Then
                AlertMessage("Please upload licence file.")
                licencelistboxerrormsg.Visible = True
                popupAppProductLicence.Show()
                hiddenModalVisible.Value = True
            Else
                AlertMessage("Please upload AI Gateway Licence one at time.\nOne AI Gateway Licence Key bind to one AI Account.")
            End If
        ElseIf UploadedRecordCount > 0 Then
            Try
                Dim sqlStr As String = " EXEC SP_CRUD_LMS_Licence '" & Customer_ID &
                                                                 "', N'" & PO_No.Text &
                                                                  "', '" & PO_Date.Text &
                                                                  "', '" & Application_Type.Text &
                                                                  "', '" & Sales_Representative_ID.Text &
                                                                  "', '" & Chargeable.SelectedValue &
                                                                  "', '" & OS_Type.Text &
                                                                  "', '" & Email.Text &
                                                                 "', N'" & EscapeChar(Remarks.Text) &
                                                                  "', '" & Trim(Selected_AI_Account_No) & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            licencelistboxerrormsg.Visible = True
            popupAppProductLicence.Show()
            hiddenModalVisible.Value = True
        End If

        DeleteStaging()
        PopulateFormViewData()
        PopulateGridViewData()
        hiddenModalVisible.Value = False
    End Sub

    Protected Sub Cancel_AppProductLicence_Click(sender As Object, e As EventArgs) Handles btnCancelAppProductLicence.Click
        hiddenModalVisible.Value = False
    End Sub

    Protected Sub Delete_AppProductLicence_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Dim sqlStr As String = "DELETE FROM LMS_Licence WHERE Customer_ID = '" & DeleteLinkButtonCommandArgument(1) & "' AND PO_No = '" & DeleteLinkButtonCommandArgument(2) & "' "
        Try
            RunSQL(sqlStr)
            AlertMessageMsgBox("Record deleted.")
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub



    '' Renew Term Licence
    Protected Sub TB_Termed_Licence_PO_No_TextChanged(sender As Object, e As EventArgs) Handles TB_Termed_Licence_PO_No.TextChanged
        Dim Termed_Licence_PO_No As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_PO_No")
        Dim DDL_Termed_Licence_Chargeable As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Chargeable")

        '' when the PO No is NA, then do not need to input PO_Date
        If Termed_Licence_PO_No.Text.ToUpper <> "NA" Then
            TB_Termed_Licence_PO_Date.Enabled = True
            RequiredField_TB_Termed_Licence_PO_Date.Enabled = True
            Dim i = DDL_Termed_Licence_Chargeable.Items.IndexOf(DDL_Termed_Licence_Chargeable.Items.FindByText("Yes"))
            DDL_Termed_Licence_Chargeable.SelectedIndex = i
        Else
            TB_Termed_Licence_PO_Date.Text = String.Empty
            TB_Termed_Licence_PO_Date.Enabled = False
            RequiredField_TB_Termed_Licence_PO_Date.Enabled = False
            Dim i = DDL_Termed_Licence_Chargeable.Items.IndexOf(DDL_Termed_Licence_Chargeable.Items.FindByText("No"))
            DDL_Termed_Licence_Chargeable.SelectedIndex = i
        End If
        popupTermedLicenceRenewal.Show()
    End Sub

    Protected Sub DDL_Termed_Licence_Chargeable_Load(sender As Object, e As EventArgs) Handles DDL_Termed_Licence_Chargeable.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Value_2, Value_1 FROM DB_Lookup WHERE Lookup_Name = 'YesNo' "
                DDL_Termed_Licence_Chargeable.DataSource = GetDataTable(sqlStr)
                DDL_Termed_Licence_Chargeable.DataTextField = "Value_1"
                DDL_Termed_Licence_Chargeable.DataValueField = "Value_2"
                DDL_Termed_Licence_Chargeable.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Termed_Licence_Chargeable_DataBound(sender As Object, e As EventArgs) Handles DDL_Termed_Licence_Chargeable.DataBound
        Dim DDL_Termed_Licence_Chargeable As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Chargeable")
        Dim i = DDL_Termed_Licence_Chargeable.Items.IndexOf(DDL_Termed_Licence_Chargeable.Items.FindByText("Yes"))
        DDL_Termed_Licence_Chargeable.SelectedIndex = i
    End Sub

    Protected Sub DDL_Termed_Licence_Sales_Representative_Load(sender As Object, e As EventArgs) Handles DDL_Termed_Licence_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "
                DDL_Termed_Licence_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_Termed_Licence_Sales_Representative.DataTextField = "Name"
                DDL_Termed_Licence_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_Termed_Licence_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Termed_Licence_Currency_Load(sender As Object, e As EventArgs) Handles DDL_Termed_Licence_Currency.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "
                DDL_Termed_Licence_Currency.DataSource = GetDataTable(sqlStr)
                DDL_Termed_Licence_Currency.DataTextField = "Currency"
                DDL_Termed_Licence_Currency.DataValueField = "Currency"
                DDL_Termed_Licence_Currency.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Termed_Licence_Currency_DataBound(sender As Object, e As EventArgs) Handles DDL_Termed_Licence_Currency.DataBound
        Dim DDL_Termed_Licence_Currency As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Currency")
        DDL_Termed_Licence_Currency.SelectedValue = DDL_Termed_Licence_Currency.Items.FindByText("SGD").Value  '' Default as SGD
    End Sub

    Protected Sub DDL_Termed_Licence_Load(sender As Object, e As EventArgs) Handles DDL_Termed_Licence.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT [Licence Code] FROM I_Termed_Licence_Renewal " &
                                       "WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                                       "  AND [Status] NOT IN ('Renew') AND [Expired Date] NOT IN ('No Expiry') " &
                                       "  AND [Expired Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) - 12, 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 6, 0)) " &
                                       "ORDER BY [Expired Date] "

                DDL_Termed_Licence.DataSource = GetDataTable(sqlStr)
                DDL_Termed_Licence.DataTextField = "Licence Code"
                DDL_Termed_Licence.DataValueField = "Licence Code"
                DDL_Termed_Licence.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Termed_Licence_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Termed_Licence.SelectedIndexChanged
        PopulateTermedLicenceListbox()
        popupTermedLicenceRenewal.Show()
    End Sub


    Protected Sub Add_TermedLicenceRenewal_Click(sender As Object, e As EventArgs) Handles btnAddTermedLicenceRenewal.Click
        ModalHeaderTermedLicenceRenewal.Text = "Add Termed Licence Renewal"
        btnSaveTermedLicenceRenewal.Text = "Save"
        btnCancelTermedLicenceRenewal.Text = "Cancel"

        TB_Termed_Licence_PO_No.Text = String.Empty
        TB_Termed_Licence_PO_Date.Text = String.Empty
        TB_Termed_Licence_PO_Date.Enabled = True
        RequiredField_TB_Termed_Licence_PO_Date.Enabled = True
        Dim DDL_Termed_Licence_Chargeable As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Chargeable")
        Dim i = DDL_Termed_Licence_Chargeable.Items.IndexOf(DDL_Termed_Licence_Chargeable.Items.FindByText("Yes"))
        DDL_Termed_Licence_Chargeable.SelectedIndex = i
        TB_Termed_Licence_Fee.Text = String.Empty
        TB_Termed_Licence_Remarks.Text = String.Empty

        '' hide the tr row when the error message is
        TermedLicencelistboxerrormsg.Visible = False

        PopulateTermedLicenceListbox()
        popupTermedLicenceRenewal.Show()
    End Sub

    Protected Sub AddTermedLicenceLineItems_Click(sender As Object, e As EventArgs) Handles btnAddTermedLicenceLineItems.Click
        Dim Licence_Code As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence")
        Dim PO_NO As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_PO_No")
        Dim PO_Date As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_PO_Date")
        Dim Chargeable As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Chargeable")
        Dim Currency As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Currency")
        Dim Fee As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_Fee")
        Dim Remarks As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_Remarks")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Sales_Representative_ID As DropDownList = pnlAddTermedLicenceRenewal.FindControl("DDL_Termed_Licence_Sales_Representative")

        PO_Date.Text = IIf(PO_Date.Text Is Nothing, "", PO_Date.Text)

        Try
            Dim sqlStr = "EXEC SP_Insert_Termed_Licence_Renewal_Staging '" & Licence_Code.SelectedValue &
                                                                    "', '" & EscapeChar(PO_NO.Text) &
                                                                    "', '" & PO_Date.Text &
                                                                    "', '" & Chargeable.SelectedValue &
                                                                    "', '" & Currency.SelectedValue &
                                                                    "', '" & Fee.Text &
                                                                    "', '" & EscapeChar(Remarks.Text) &
                                                                    "', '" & Customer_ID &
                                                                    "', '" & Sales_Representative_ID.SelectedValue & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        TermedLicencelistboxerrormsg.Visible = False

        PopulateTermedLicenceListbox()
        popupTermedLicenceRenewal.Show()
    End Sub

    Protected Sub btnClearTermedLicenceLineItems_Click(sender As Object, e As EventArgs) Handles btnClearTermedLicenceLineItems.Click
        DeleteStaging()
        TermedLicencelistboxerrormsg.Visible = False
        PopulateTermedLicenceListbox()
        popupTermedLicenceRenewal.Show()
    End Sub

    Protected Sub PopulateTermedLicenceListbox()
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_PO_No")
        Try
            Dim sqlStr As String = "SELECT * FROM LMS_Termed_Licence_Renewal_Staging " &
                                   "WHERE Customer_ID = '" & Customer_ID & "'" &
                                   "  AND PO_No = '" & PO_No.Text & "'"

            GridView_Termed_Licence_List.DataSource = GetDataTable(sqlStr)
            GridView_Termed_Licence_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub Save_TermedLicenceRenewal_Click(sender As Object, e As EventArgs) Handles btnSaveTermedLicenceRenewal.Click
        Dim PO_No As TextBox = pnlAddTermedLicenceRenewal.FindControl("TB_Termed_Licence_PO_No")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")

        Dim GridView_Termed_Licence_List As GridView = pnlAddTermedLicenceRenewal.FindControl("GridView_Termed_Licence_List")
        Dim UploadedRecordCount As Integer = GridView_Termed_Licence_List.Rows.Count

        If UploadedRecordCount > 0 Then
            Try
                Dim sqlStr As String = "EXEC SP_CRUD_Termed_Licence_Renewal N'" & EscapeChar(PO_No.Text) & "', '" & Customer_ID & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            TermedLicencelistboxerrormsg.Visible = True
            popupTermedLicenceRenewal.Show()
        End If

        DeleteStaging()
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub Delete_TermedLicenceRenewal_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim UID As String = CType(sender, LinkButton).CommandArgument
        Try
            Dim sqlStr As String = "DELETE FROM LMS_Termed_Licence_Renewal WHERE Renewal_UID = '" & UID & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub



    '' Licence Notes
    Protected Sub Add_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNotes.Click
        ModalHeaderNotes.Text = "Add Notes"
        btnSaveNotes.Text = "Save"
        btnCancelNotes.Text = "Cancel"

        TB_Notes.Text = String.Empty
        popupNotes.Show()
    End Sub

    Protected Sub Save_Notes_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveNotes.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Notes As TextBox = pnlAddNotes.FindControl("TB_Notes")
        Dim Notes_For As String = "App Product Licence"
        Dim BtnCommand As Button = TryCast(sender, Button)

        Dim sqlStr As String = " EXEC SP_CRUD_Notes '" & Customer_ID & "', N'" & Replace(Notes.Text, "'", "''") & "', '" & Notes_For & "', '" & BtnCommand.CommandArgument & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub Delete_Notes_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)
        Dim sqlStr As String = " DELETE FROM DB_Account_Notes WHERE ID = " & BtnCommand.CommandArgument
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        PopulateFormViewData()
        PopulateGridViewData()
    End Sub



    '' Cancel button event shared for both modal with staging table invoived
    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancelAppProductLicence.Click, btnCancelTermedLicenceRenewal.Click
        DeleteStaging()
    End Sub


    '' DeleteStaging event shared for both modal, this event reset both stagig table
    Protected Sub DeleteStaging()
        Try
            '' In module licence page, it is using 2 staging table, reset both table after used
            Dim TableToReset() As String = {"LMS_Licence_Staging", "LMS_Termed_Licence_Renewal_Staging"}
            For i = 0 To TableToReset.Length - 1
                RunSQL("EXEC SP_Reset_Staging_Table '" & TableToReset(i) & "'")
            Next
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub


    '' Click Sync button to sync license status
    Protected Sub AILicenceRefresh_Click(sender As Object, e As EventArgs) Handles AILicenceRefresh.Click
        Try
            RunSQL("EXEC SP_Sync_LMS_Licence")
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub



    '' Searchbox
    Protected Sub BT_Search_Click(sender As Object, e As EventArgs) Handles BT_Search.Click
        PopulateFormViewData()
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub BT_Search_Token_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Token.Click
        PopulateFormViewData()
        PopulateGridViewData(TB_Search_Token.Text)
    End Sub


    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/App_Product_Licence.aspx")
    End Sub


End Class
