Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports NPOI.OpenXmlFormats.Spreadsheet
Imports NPOI.HSSF.Record
Imports System.Drawing
'Imports NPOI.SS.Formula.Functions

Partial Class Form_Module_Licence_Form
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Register Module Licence"

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
                Response.Redirect("~/Form/Module_Licence.aspx")
            End If
            hiddenModalVisible.Value = False
        Else
            hiddenModalVisible.Value = True
        End If
        PopulateGridViewData()

        UpdatePanel1.Attributes.CssStyle.Add("width", "60%")
        UpdatePanel1.Attributes.CssStyle.Add("display", "inline-block")
        UpdatePanel1.Attributes.CssStyle.Add("margin-left", "5px")
        UpdatePanel3.Attributes.CssStyle.Add("width", "39.6%")
        UpdatePanel3.Attributes.CssStyle.Add("float", "left")

        '' Display the notes when login user is administrator
        AddLicencePoolGuide.Visible = IIf(Session("Login_Name") = "administrator", True, False)

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
                                      "ORDER BY [Chargeable] DESC, [PO Date] DESC ",
                                      "SELECT * FROM R_LMS_Module_Licence_Pool WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' ORDER BY Customer_ID, No, Module_Type DESC ",
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
        'GridViewObj.AutoGenerateColumns = False
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
                    Bfield.HeaderText = ColName(i).Replace("_", " ")
                    If Bfield.HeaderText.Contains("Date") Then
                        Bfield.DataFormatString = "{0:dd MMM yy}"
                    End If
                    Bfield.HeaderStyle.Wrap = False
                    Bfield.ItemStyle.Wrap = False
                    GridViewObj.Columns.Add(Bfield)
                Next
                GridViewObj.ShowFooter = False

                '' Add template field for the edit button
                Dim TField As TemplateField = New TemplateField()
                TField.HeaderStyle.Width = Unit.Percentage(2)
                TField.ItemStyle.Wrap = False
                TField.ItemTemplate = New GridViewItemTemplateControl()
                GridViewObj.Columns.Add(TField)

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
                    Bfield.HeaderText = ColName(i).Replace("_", " ")
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
                    Bfield.HeaderText = ColData(i).Replace("_", " ")
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
                    Bfield.HeaderText = ColData(i).Replace("_", " ")
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
                    Bfield.HeaderText = ColData(i).Replace("_", " ")
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
                    Bfield.HeaderText = ColData(i).Replace("_", " ")
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


            '' Edit Button (to add module license quantity)
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            If DateDiff(DateInterval.Day, CDate(drv("Created Date")), Date.Now) <= 365 Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
            Else
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
            End If
            EditLinkButton.CommandArgument = e.Row.RowIndex & "|" & drv("UID")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_ModuleLicenceCount_Click

        End If
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

            '' display the Child Gridview Requested By column when the PO No is NA
            Licence_Code.Columns(GetColumnIndexByColumnName(Licence_Code, "Requested By")).Visible = IIf(PO_No = "NA", True, False)

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

            '' if Invoice No is empty then set 'TBA'
            'If Replace(e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text, "&nbsp;", "") = "" Then
            '    e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "TBA"
            'End If

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
            AddHandler DeleteLinkButton.Click, AddressOf Delete_ModuleLicence_Click

        End If
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
            AddHandler DeleteLinkButton.Click, AddressOf Delete_AIRenewal_Click

        End If
    End Sub

    Protected Sub GridView6_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView6.RowDataBound
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

    '' 01. Register Module Licence Order 
    Protected Sub TB_Order_PO_No_TextChanged(sender As Object, e As EventArgs) Handles TB_Order_PO_No.TextChanged
        Dim Order_PO_No As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_No")
        Dim DDL_Order_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Order_Chargeable")

        '' when the PO No is NA, then do not need to input PO_Date
        If Order_PO_No.Text.ToUpper <> "NA" Then
            TB_Order_PO_Date.Enabled = True
            RequiredField_TB_Order_PO_Date.Enabled = True
            Dim i = DDL_Order_Chargeable.Items.IndexOf(DDL_Order_Chargeable.Items.FindByText("Yes"))
            DDL_Order_Chargeable.SelectedIndex = i
        Else
            TB_Order_PO_Date.Text = String.Empty
            TB_Order_PO_Date.Enabled = False
            RequiredField_TB_Order_PO_Date.Enabled = False
            Dim i = DDL_Order_Chargeable.Items.IndexOf(DDL_Order_Chargeable.Items.FindByText("No"))
            DDL_Order_Chargeable.SelectedIndex = i
        End If
        popupModuleLicenceOrder.Show()
    End Sub

    Protected Sub DDL_Order_Chargeable_Load(sender As Object, e As EventArgs) Handles DDL_Order_Chargeable.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_2, Value_1 FROM DB_Lookup WHERE Lookup_Name = 'YesNo' "
                DDL_Order_Chargeable.DataSource = GetDataTable(sqlStr)
                DDL_Order_Chargeable.DataTextField = "Value_1"
                DDL_Order_Chargeable.DataValueField = "Value_2"
                DDL_Order_Chargeable.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Order_Chargeable_DataBound(sender As Object, e As EventArgs) Handles DDL_Order_Chargeable.DataBound
        Dim DDL_Order_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Order_Chargeable")
        Dim i = DDL_Order_Chargeable.Items.IndexOf(DDL_Order_Chargeable.Items.FindByText("Yes"))
        DDL_Order_Chargeable.SelectedIndex = i
    End Sub

    Protected Sub DDL_Order_Sales_Representative_Load(sender As Object, e As EventArgs) Handles DDL_Order_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "
                DDL_Order_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_Order_Sales_Representative.DataTextField = "Name"
                DDL_Order_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_Order_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Order_Module_Type_Load(sender As Object, e As EventArgs) Handles DDL_Order_Module_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Value_3 AS Module_Type FROM DB_Lookup WHERE Value_4 = 'SM Module Licence' AND Value_3 IN ('AI', 'BYOC', 'e.Sense') "

                DDL_Order_Module_Type.DataSource = GetDataTable(sqlStr)
                DDL_Order_Module_Type.DataTextField = "Module_Type"
                DDL_Order_Module_Type.DataValueField = "Module_Type"
                DDL_Order_Module_Type.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Order_Module_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Order_Module_Type.SelectedIndexChanged
        Dim Quantity As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_Quantity")
        Quantity.Text = String.Empty
        popupModuleLicenceOrder.Show()
    End Sub


    Protected Sub Add_ModuleLicenceOrder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddModuleLicenceOrder.Click
        ModalHeaderModuleLicenceOrder.Text = "Add Module Licence Order"
        btnSaveModuleLicenceOrder.Text = "Save"
        btnCancelModuleLicenceOrder.Text = "Cancel"

        '' Initialize field
        TB_Order_PO_No.Text = String.Empty
        TB_Order_PO_Date.Text = String.Empty
        TB_Order_PO_Date.Enabled = True
        RequiredField_TB_Order_PO_Date.Enabled = True
        Dim DDL_Order_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Order_Chargeable")
        Dim i = DDL_Order_Chargeable.Items.IndexOf(DDL_Order_Chargeable.Items.FindByText("Yes"))
        DDL_Order_Chargeable.SelectedIndex = i
        TB_Order_Remarks.Text = String.Empty
        TB_Order_Quantity.Text = String.Empty

        '' hide the tr row when the error message is.
        licenceorderlistboxerrormsg.Visible = False

        PopulateOrderListbox()
        popupModuleLicenceOrder.Show()
    End Sub

    Protected Sub AddOrderLineItems_Click(sender As Object, e As EventArgs) Handles AddOrderLineItems.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_No")
        Dim Module_Type As DropDownList = pnlAddEditModuleLicenceOrder.FindControl("DDL_Order_Module_Type")
        Dim Quantity As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_Quantity")

        Try
            Dim sqlStr = " EXEC SP_Insert_Module_Licence_Staging '" & Customer_ID & "', '" & EscapeChar(PO_No.Text) & "', '" & Module_Type.Text & "', '" & Quantity.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        licenceorderlistboxerrormsg.Visible = False

        PopulateOrderListbox()
        popupModuleLicenceOrder.Show()
    End Sub

    Protected Sub btnClearOrderLineItems_Click(sender As Object, e As EventArgs) Handles btnClearOrderLineItems.Click
        Dim Quantity As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_Quantity")
        Quantity.Text = String.Empty

        DeleteStaging()
        licenceorderlistboxerrormsg.Visible = False
        PopulateOrderListbox()
        popupModuleLicenceOrder.Show()
    End Sub

    Protected Sub PopulateOrderListbox()
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_No")

        Try
            Dim sqlStr As String = " SELECT * FROM LMS_Module_Licence_Staging " &
                                   " WHERE Customer_ID = '" & Customer_ID & "'" &
                                   "   AND PO_No = '" & PO_No.Text & "'"

            GridView_Order_List.DataSource = GetDataTable(sqlStr)
            GridView_Order_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub Save_ModuleLicenceOrder_Click(sender As Object, e As EventArgs) Handles btnSaveModuleLicenceOrder.Click
        Dim PO_No As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_No")
        Dim PO_Date As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_Date")
        Dim Chargeable As DropDownList = pnlAddEditModuleLicenceOrder.FindControl("DDL_Order_Chargeable")
        Dim Remarks As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_Remarks")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Sales_Representative_ID As DropDownList = pnlAddEditModuleLicenceOrder.FindControl("DDL_Order_Sales_Representative")

        Dim Module_Type As DropDownList = pnlAddEditModuleLicenceOrder.FindControl("DDL_Order_Module_Type")
        Dim Quantity As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_Quantity")


        Dim GridView_Order_List As GridView = pnlAddEditModuleLicenceOrder.FindControl("GridView_Order_List")
        Dim UploadedRecordCount As Integer = GridView_Order_List.Rows.Count

        If UploadedRecordCount > 0 Then
            '' Check database if PO No exists
            Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecords FROM LMS_Module_Licence_Order WHERE Customer_ID = N'" & Customer_ID & "' AND PO_No = N'" & PO_No.Text.Trim() & "'", "NoOfRecords")) > 0, True, False)
            If RecordExists Then
                AlertMessageMsgBox("PO No. " & PO_No.Text & " exists, please check the record")
            Else
                Try
                    Dim sqlStr As String = " EXEC SP_CRUD_LMS_Module_Licence N'" & EscapeChar(PO_No.Text) &
                                                                          "', '" & PO_Date.Text &
                                                                          "', '" & Chargeable.SelectedValue &
                                                                          "', '" & EscapeChar(Remarks.Text) &
                                                                          "', '" & Customer_ID &
                                                                          "', '" & Sales_Representative_ID.SelectedValue & "' "

                    RunSQL(sqlStr)
                Catch ex As Exception
                    Response.Write("Error: " & ex.Message)
                End Try
            End If
        Else
            licenceorderlistboxerrormsg.Visible = True
            popupModuleLicenceOrder.Show()
        End If

        DeleteStaging()
        'PopulateFormViewData()
        PopulateGridViewData()
    End Sub




    Protected Sub DDL_Module_Licence_Type_Load(sender As Object, e As EventArgs) Handles DDL_Module_Licence_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT Value_3 AS Module_Type FROM DB_Lookup WHERE Value_4 = 'SM Module Licence' AND Value_3 IN ('AI', 'BYOC', 'e.Sense') ORDER BY Value_3 "

                DDL_Module_Licence_Type.DataSource = GetDataTable(sqlStr)
                DDL_Module_Licence_Type.DataTextField = "Module_Type"
                DDL_Module_Licence_Type.DataValueField = "Module_Type"
                DDL_Module_Licence_Type.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Module_Licence_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Module_Licence_Type.SelectedIndexChanged
        Dim Module_Licence_Type As DropDownList = pnlUpdateModuleLicenceCount.FindControl("DDL_Module_Licence_Type")
        Dim Module_Licence_Quantity As TextBox = pnlUpdateModuleLicenceCount.FindControl("TB_Module_Licence_Quantity")

        '' get the quantity from db and put to put to tb_quantity
        TB_Selected_Quantity_By_Module_Type.Text = Get_Value("SELECT Quantity FROM LMS_Module_Licence_Order_Item WHERE UID = '" & TB_Selected_UID.Text & "' AND Module_Type = '" & Module_Licence_Type.SelectedValue & "'", "Quantity")
        Module_Licence_Quantity.Text = TB_Selected_Quantity_By_Module_Type.Text

        licenceorderquantityerrormsg.Visible = False

        popupUpdateModuleLicenceCount.Show()
    End Sub

    Protected Sub Edit_ModuleLicenceCount_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderModuleLicenceCount.Text = "Update Module Licence Order"
        btnSaveModuleLicenceCount.Text = "Update"
        btnCancelModuleLicenceCount.Text = "Cancel"

        '' Reinitialize the field
        DDL_Module_Licence_Type.SelectedIndex = -1
        TB_Module_Licence_Quantity.Text = String.Empty
        licenceorderquantityerrormsg.Visible = False

        ' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Row_Index, TB_Selected_UID}

        ' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        popupUpdateModuleLicenceCount.Show()
    End Sub

    Protected Sub Update_ModuleLicenceCount_Click(sender As Object, e As EventArgs) Handles btnSaveModuleLicenceCount.Click
        Dim Selected_Row_Index As TextBox = pnlUpdateModuleLicenceCount.FindControl("TB_Selected_Row_Index")
        Dim Selected_Selected_UID As TextBox = pnlUpdateModuleLicenceCount.FindControl("TB_Selected_UID")
        Dim Module_Licence_Type As DropDownList = pnlUpdateModuleLicenceCount.FindControl("DDL_Module_Licence_Type")
        Dim Module_Licence_Quantity As TextBox = pnlUpdateModuleLicenceCount.FindControl("TB_Module_Licence_Quantity")

        Dim Quantity_By_Module_Type As String = IIf(Len(TB_Selected_Quantity_By_Module_Type.Text) > 0, TB_Selected_Quantity_By_Module_Type.Text, "0")

        If CInt(Module_Licence_Quantity.Text) >= CInt(Quantity_By_Module_Type) Then
            Try
                Dim sqlStr As String = " EXEC SP_CRUD_LMS_Module_Licence_Order_Count N'" & Selected_Selected_UID.Text &
                                                                                 "', N'" & Module_Licence_Type.SelectedValue &
                                                                                 "', N'" & Module_Licence_Quantity.Text & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            AlertMessageMsgBox("Cannot be less than existing quantity")
        End If


        'PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub CustomValidator_TB_Module_Licence_Quantity_ServerValidate(source As Object, args As ServerValidateEventArgs)
        Dim quantity As Integer
        If Integer.TryParse(args.Value, quantity) Then
            args.IsValid = (quantity <> 0)
        Else
            args.IsValid = False ' Fallback in case of non-integer input
        End If
        popupUpdateModuleLicenceCount.Show()
        licenceorderquantityerrormsg.Visible = True
    End Sub





    '' 02. Register Module Licence Key

    Protected Sub TB_PO_No_TextChanged(sender As Object, e As EventArgs) Handles TB_PO_No.TextChanged
        Dim PO_No As TextBox = pnlAddEditModuleLicence.FindControl("TB_PO_No")
        Dim DDL_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Chargeable")

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
        popupModuleLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub Custom_Validator_TB_Order_PO_No_ServerValidate(source As Object, args As ServerValidateEventArgs)
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicenceOrder.FindControl("TB_Order_PO_No")
        Dim RecordCount As Integer = Get_Value("SELECT COUNT(*) RecordCount FROM LMS_Module_Licence_Order WHERE Customer_ID = '" & Customer_ID & "' AND PO_No = '" & PO_No.Text & "' ", "RecordCount")

        If RecordCount > 0 Then
            args.IsValid = False
        Else
            args.IsValid = True
        End If
    End Sub

    Protected Sub DDL_Application_Type_Load(sender As Object, e As EventArgs) Handles DDL_Application_Type.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_3 AS Application_Type " &
                                       " FROM DB_Lookup " &
                                       " WHERE Lookup_Name = 'Bill Items' AND Value_4 IN ('Module Licence Key') " &
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
        Dim DDL_Application_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Application_Type")
        Dim i = DDL_Application_Type.Items.IndexOf(DDL_Application_Type.Items.FindByText("PC Scale"))
        DDL_Application_Type.SelectedIndex = i
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
                Dim sqlStr As String = " SELECT Value_1 AS OS_Type FROM DB_Lookup WHERE Lookup_Name = 'OS Type' AND Value_1 IN ('SM') "
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
        Dim DDL_OS_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_OS_Type")
        Dim i = DDL_OS_Type.Items.IndexOf(DDL_OS_Type.Items.FindByValue("SM"))
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
        Dim DDL_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Chargeable")
        Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
        DDL_Chargeable.SelectedIndex = i
    End Sub


    Protected Sub Add_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddModuleLicence.Click
        ModalHeaderModuleLicence.Text = "Add Module Licence"
        btnSaveModuleLicence.Text = "Save"
        btnCancelModuleLicence.Text = "Cancel"

        TB_PO_No.Text = String.Empty
        TB_PO_Date.Text = String.Empty
        TB_PO_Date.Enabled = True
        RequiredField_TB_PO_Date.Enabled = True
        Dim DDL_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Chargeable")
        Dim i = DDL_Chargeable.Items.IndexOf(DDL_Chargeable.Items.FindByText("Yes"))
        DDL_Chargeable.SelectedIndex = i
        TB_Email.Text = String.Empty
        TB_Remarks.Text = String.Empty

        '' hide the tr row when the error message is.
        licencekeylistboxerrormsg.Visible = False

        PopulateLicenceListbox()
        popupModuleLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub UploadLineItems_Click(sender As Object, e As EventArgs) Handles UploadLineItems.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicence.FindControl("TB_PO_No")
        Dim Application_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Application_Type")
        Dim OS_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_OS_Type")
        Dim Email As TextBox = pnlAddEditModuleLicence.FindControl("TB_Email")
        Dim Sales_Representative_ID As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Sales_Representative")
        Dim Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Chargeable")
        Dim Remarks As TextBox = pnlAddEditModuleLicence.FindControl("TB_Remarks")

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

        licencekeylistboxerrormsg.Visible = False

        PopulateLicenceListbox()
        popupModuleLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub btnClearLineItems_Click(sender As Object, e As EventArgs) Handles btnClearLineItems.Click
        DeleteStaging()
        licencekeylistboxerrormsg.Visible = False
        PopulateLicenceListbox()
        popupModuleLicence.Show()
        hiddenModalVisible.Value = True
    End Sub

    Protected Sub PopulateLicenceListbox()
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicence.FindControl("TB_PO_No")

        Try
            Dim sqlStr As String = " SELECT * FROM LMS_Licence_staging " &
                                   " WHERE Customer_ID = '" & Customer_ID & "'" &
                                   "   AND PO_No = '" & PO_No.Text & "'"

            GridView_Licence_List.DataSource = GetDataTable(sqlStr)
            GridView_Licence_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub Save_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveModuleLicence.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddEditModuleLicence.FindControl("TB_PO_No")
        Dim PO_Date As TextBox = pnlAddEditModuleLicence.FindControl("TB_PO_Date")

        '' Get these value from modal control instead of staging table
        Dim Application_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Application_Type")
        Dim Sales_Representative_ID As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Sales_Representative")
        Dim Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_Chargeable")
        Dim OS_Type As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_OS_Type")
        Dim Email As TextBox = pnlAddEditModuleLicence.FindControl("TB_Email")
        Dim Remarks As TextBox = pnlAddEditModuleLicence.FindControl("TB_Remarks")

        Dim GridView_Licence_List As GridView = pnlAddEditModuleLicence.FindControl("GridView_Licence_List")
        Dim UploadedRecordCount As Integer = GridView_Licence_List.Rows.Count

        If UploadedRecordCount > 0 Then
            Try
                Dim sqlStr As String = " EXEC SP_CRUD_LMS_Licence '" & Customer_ID &
                                                             "', N'" & PO_No.Text &
                                                              "', '" & PO_Date.Text &
                                                              "', '" & Application_Type.Text &
                                                              "', '" & Sales_Representative_ID.Text &
                                                              "', '" & Chargeable.SelectedValue &
                                                              "', '" & OS_Type.Text &
                                                              "', '" & Email.Text &
                                                              "', N'" & EscapeChar(Remarks.Text) & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            licencekeylistboxerrormsg.Visible = True
            popupModuleLicence.Show()
            hiddenModalVisible.Value = True
        End If

        DeleteStaging()
        PopulateFormViewData()
        PopulateGridViewData()
        hiddenModalVisible.Value = False
    End Sub

    Protected Sub Cancel_AppProductLicence_Click(sender As Object, e As EventArgs) Handles btnCancelModuleLicence.Click
        hiddenModalVisible.Value = False
    End Sub

    Protected Sub Delete_ModuleLicence_Click(ByVal sender As Object, ByVal e As EventArgs)
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





    '' Renew AI Licence
    Protected Sub TB_AI_PO_No_TextChanged(sender As Object, e As EventArgs) Handles TB_AI_PO_No.TextChanged
        Dim AI_PO_No As TextBox = pnlAddAIRenewal.FindControl("TB_AI_PO_No")
        Dim DDL_AI_Chargeable As DropDownList = pnlAddEditModuleLicence.FindControl("DDL_AI_Chargeable")

        '' when the PO No is NA, then do not need to input PO_Date
        If AI_PO_No.Text.ToUpper <> "NA" Then
            TB_AI_PO_Date.Enabled = True
            RequiredField_TB_AI_PO_Date.Enabled = True
            Dim i = DDL_AI_Chargeable.Items.IndexOf(DDL_AI_Chargeable.Items.FindByText("Yes"))
            DDL_AI_Chargeable.SelectedIndex = i
        Else
            TB_AI_PO_Date.Text = String.Empty
            TB_AI_PO_Date.Enabled = False
            RequiredField_TB_AI_PO_Date.Enabled = False
            Dim i = DDL_AI_Chargeable.Items.IndexOf(DDL_AI_Chargeable.Items.FindByText("No"))
            DDL_AI_Chargeable.SelectedIndex = i
        End If
        popupAIRenewal.Show()
    End Sub

    Protected Sub DDL_AI_Chargeable_Load(sender As Object, e As EventArgs) Handles DDL_AI_Chargeable.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_2, Value_1 FROM DB_Lookup WHERE Lookup_Name = 'YesNo' "
                DDL_AI_Chargeable.DataSource = GetDataTable(sqlStr)
                DDL_AI_Chargeable.DataTextField = "Value_1"
                DDL_AI_Chargeable.DataValueField = "Value_2"
                DDL_AI_Chargeable.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_AI_Chargeable_DataBound(sender As Object, e As EventArgs) Handles DDL_AI_Chargeable.DataBound
        Dim DDL_AI_Chargeable As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Chargeable")
        Dim i = DDL_AI_Chargeable.Items.IndexOf(DDL_AI_Chargeable.Items.FindByText("Yes"))
        DDL_AI_Chargeable.SelectedIndex = i
    End Sub

    Protected Sub DDL_AI_Sales_Representative_Load(sender As Object, e As EventArgs) Handles DDL_AI_Sales_Representative.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name "
                DDL_AI_Sales_Representative.DataSource = GetDataTable(sqlStr)
                DDL_AI_Sales_Representative.DataTextField = "Name"
                DDL_AI_Sales_Representative.DataValueField = "Sales_Representative_ID"
                DDL_AI_Sales_Representative.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_AI_Currency_Load(sender As Object, e As EventArgs) Handles DDL_AI_Currency.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "
                DDL_AI_Currency.DataSource = GetDataTable(sqlStr)
                DDL_AI_Currency.DataTextField = "Currency"
                DDL_AI_Currency.DataValueField = "Currency"
                DDL_AI_Currency.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_AI_Currency_DataBound(sender As Object, e As EventArgs) Handles DDL_AI_Currency.DataBound
        Dim DDL_AI_Currency As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Currency")
        DDL_AI_Currency.SelectedValue = DDL_AI_Currency.Items.FindByText("SGD").Value  '' Default as SGD
    End Sub

    Protected Sub DDL_AI_Licence_Load(sender As Object, e As EventArgs) Handles DDL_AI_Licence.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = "SELECT [Licence Code], CASE WHEN [Expired Date] != 'No Expiry' THEN [Licence code] + ' - (' + FORMAT(CAST([Expired Date] AS date), 'dd MMM yy') + ')' ELSE 'No Expiry' END AS [Licence Code Expiry] FROM I_AI_Licence_Renewal " &
                                       "WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                                       "  AND [Status] NOT IN ('Renew') AND [Expired Date] NOT IN ('No Expiry') " &
                                       "  AND [Expired Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) - 12, 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 6, 0)) " &
                                       "ORDER BY [Expired Date] "

                DDL_AI_Licence.DataSource = GetDataTable(sqlStr)
                DDL_AI_Licence.DataTextField = "Licence Code Expiry"
                DDL_AI_Licence.DataValueField = "Licence Code"
                DDL_AI_Licence.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_AI_Licence_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_AI_Licence.SelectedIndexChanged
        PopulateAIListbox()
        popupAIRenewal.Show()
    End Sub

    Protected Sub Add_AIRenewal_Click(sender As Object, e As EventArgs) Handles btnAddAIRenewal.Click
        ModalHeaderAIRenewal.Text = "Add AI Licence Renewal"
        btnSaveAIRenewal.Text = "Save"
        btnCancelAIRenewal.Text = "Cancel"

        TB_AI_PO_No.Text = String.Empty
        TB_AI_PO_Date.Text = String.Empty
        TB_AI_PO_Date.Enabled = True
        RequiredField_TB_AI_PO_Date.Enabled = True
        Dim DDL_AI_Chargeable As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Chargeable")
        Dim i = DDL_AI_Chargeable.Items.IndexOf(DDL_AI_Chargeable.Items.FindByText("Yes"))
        DDL_AI_Chargeable.SelectedIndex = i
        TB_AI_Fee.Text = String.Empty
        TB_AI_Remarks.Text = String.Empty

        '' hide the tr row when the error message is.
        AILicencelistboxerrormsg.Visible = False

        PopulateAIListbox()
        popupAIRenewal.Show()
    End Sub

    Protected Sub AddAILineItems_Click(sender As Object, e As EventArgs) Handles btnAddAILineItems.Click
        Dim Licence_Code As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Licence")
        Dim PO_NO As TextBox = pnlAddAIRenewal.FindControl("TB_AI_PO_No")
        Dim PO_Date As TextBox = pnlAddAIRenewal.FindControl("TB_AI_PO_Date")
        Dim Chargeable As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Chargeable")
        Dim Currency As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Currency")
        Dim Fee As TextBox = pnlAddAIRenewal.FindControl("TB_AI_Fee")
        Dim Remarks As TextBox = pnlAddAIRenewal.FindControl("TB_AI_Remarks")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Sales_Representative_ID As DropDownList = pnlAddAIRenewal.FindControl("DDL_AI_Sales_Representative")

        PO_Date.Text = IIf(PO_Date.Text Is Nothing, "", PO_Date.Text)

        Try
            Dim sqlStr = "EXEC SP_Insert_AI_Licence_Renewal_Staging '" & Licence_Code.SelectedValue &
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

        AILicencelistboxerrormsg.Visible = False

        PopulateAIListbox()
        popupAIRenewal.Show()
    End Sub

    Protected Sub btnClearAILineItems_Click(sender As Object, e As EventArgs) Handles btnClearAILineItems.Click
        DeleteStaging()
        AILicencelistboxerrormsg.Visible = False
        PopulateAIListbox()
        popupAIRenewal.Show()
    End Sub

    Protected Sub PopulateAIListbox()
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim PO_No As TextBox = pnlAddAIRenewal.FindControl("TB_AI_PO_No")
        Try
            Dim sqlStr As String = " SELECT * FROM LMS_AI_Licence_Renewal_Staging " &
                                   " WHERE Customer_ID = '" & Customer_ID & "'" &
                                   "   AND PO_No = '" & PO_No.Text & "'"

            GridView_AI_List.DataSource = GetDataTable(sqlStr)
            GridView_AI_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub Save_AIRenewal_Click(sender As Object, e As EventArgs) Handles btnSaveAIRenewal.Click
        Dim PO_No As TextBox = pnlAddAIRenewal.FindControl("TB_AI_PO_No")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")

        Dim GridView_AI_List As GridView = pnlAddAIRenewal.FindControl("GridView_AI_List")
        Dim UploadedRecordCount As Integer = GridView_AI_List.Rows.Count

        If UploadedRecordCount > 0 Then
            Try
                Dim sqlStr As String = "EXEC SP_CRUD_AI_Licence_Renewal N'" & EscapeChar(PO_No.Text) & "', '" & Customer_ID & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            AILicencelistboxerrormsg.Visible = True
            popupAIRenewal.Show()
        End If

        DeleteStaging()
        PopulateFormViewData()
        PopulateGridViewData()
    End Sub

    Protected Sub Delete_AIRenewal_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim UID As String = CType(sender, LinkButton).CommandArgument
        Try
            Dim sqlStr As String = "DELETE FROM LMS_AI_Licence_Renewal WHERE Renewal_UID = '" & UID & "' "
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
        Dim Notes_For As String = "Module Licence"
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
    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancelModuleLicenceOrder.Click, btnCancelModuleLicence.Click, btnCancelAIRenewal.Click
        DeleteStaging()
    End Sub


    '' DeleteStaging event shared for both modal, this event reset both stagig table
    Protected Sub DeleteStaging()
        Try
            '' In module licence page, it is using 2 staging table, reset both table after used
            Dim TableToReset() As String = {"LMS_Licence_Staging", "LMS_Module_Licence_Staging", "LMS_AI_Licence_Renewal_Staging"}
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


    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Form/Module_Licence.aspx")
    End Sub


End Class
