Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Ionic.Zip

Partial Class Services_Maintenance_Service_Records_Details
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = Nothing
    Dim TotalAmount As Decimal
    Dim currentSortedColumnIndex As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        Dim Services_Group As String = Left(Session("User_Group"), 1)

        If Not IsPostBack Then
            If Request.QueryString("Customer_ID") <> "" Then
                FormView1.ChangeMode(FormViewMode.Edit)
                PageTitle = "Update Customer Record"
                BT_Create.Text = "Update"
                BT_Create.CommandName = "Update"
            Else
                FormView1.ChangeMode(FormViewMode.Insert)
                PageTitle = "New Customer Record"
                BT_Create.Text = "Create"
                BT_Create.CommandName = "Create"
            End If

            BT_Cancel.Text = "Cancel"
            LB_PageTitle.Text = PageTitle

            '' Populate all data on first load
            PopulateFormViewData()
            PopulateGridViewDataBanner()
            PopulateGridViewDataStore(TB_Search_Store.Text)
            PopulateGridViewDataProductType(TB_Search_Product_Type.Text)
            PopulateGridViewDataProduct(TB_Search_Product.Text)
            PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text)
            PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
            PopulateGridViewDataComment()


            '' Get the latest FY based on store contract record and populate the gridview based on latest financial year
            Dim sqlStr = "SELECT TOP 1 [Year] AS [FY] FROM ( " &
                         "   SELECT [Year] FROM dbo.Get_Financial_Year_List() " &
                         "   UNION " &
                         "   SELECT [FY] FROM R_Maintenance_Contract WHERE [Services Group] = '" & Services_Group & "' " & " AND [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                         "   ) TBL " &
                         "ORDER BY TBL.[Year] DESC "

            Dim Lasted_FY As String = Get_Value(sqlStr, "FY")
            PopulateGridViewDataContract(Lasted_FY)
        End If

        '' Gridview for Tags Deployment will have to be call every postback for the edit and delete linkbutton to work
        PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)


        '' Control to hide/display tab
        tab_link3.Visible = True   '' POS / Retail Product Type Tab
        tab_link5.Visible = IIf(Services_Group <> "H", True, False)   '' Retail Product Tab
        tab_link4.Visible = IIf(Services_Group = "H", True, False)    '' POS Product Tab

        '' Hide tab based on services group login
        tab_link6.Visible = IIf(Services_Group <> "H", True, False)   '' Tag Type Tab
        tab_link7.Visible = IIf(Services_Group <> "H", True, False)   '' Tag Distributions Tab


        '' Hide frequency column when Services_Group is "H"
        GridView1.Columns(GetColumnIndexByColumnName(GridView1, "Frequency")).Visible = IIf(Services_Group <> "H", True, False)

    End Sub


    '' Populate gridviews
    Protected Sub PopulateFormViewData()
        Try
            FormView1.DataSource = GetDataTable(GetSQL(FormView1, Nothing, Nothing, Nothing))
            FormView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataBanner(Optional ByVal TB_Search_Banner As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search_Banner)
        Try
            BuildGridView(GridView1, "GridView1", "Banner ID")
            GridView1.DataSource = GetDataTable(GetSQL(GridView1, Nothing, keyword, Nothing))
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataStore(Optional ByVal TB_Search_Store As String = Nothing, Optional currentSortingGridview As String = Nothing, Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search_Store)
        Try
            Dim ControlName As String = "GridView2"
            Dim DataKeyName As String = "Store ID"
            BuildGridView(GridView2, ControlName, DataKeyName)
            Dim dt As DataTable = GetDataTable(GetSQL(GridView2, Nothing, keyword, Nothing))
            Dim dataView As New DataView(dt)
            dataView.Sort = IIf(ControlName = currentSortingGridview, currentSortedExpressionDirection, Nothing)

            GridView2.DataSource = dataView
            GridView2.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataProductType(Optional ByVal TB_Search_Product_Type As String = Nothing, Optional currentSortingGridview As String = Nothing, Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search_Product_Type)
        Try
            Dim ControlName As String = "GridView3"
            Dim DataKeyName As String = "Code"
            BuildGridView(GridView3, ControlName, DataKeyName)
            Dim Services_Group As String = Left(Session("User_Group"), 1)
            Dim dt As DataTable = GetDataTable(GetSQL(GridView3, Services_Group, keyword, Nothing))
            Dim dataView As New DataView(dt)
            dataView.Sort = IIf(ControlName = currentSortingGridview, currentSortedExpressionDirection, Nothing)

            GridView3.DataSource = dataView
            GridView3.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataProduct(Optional ByVal TB_Search_Product As String = Nothing, Optional currentSortingGridview As String = Nothing, Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search_Product)
        Try
            Dim ControlName As String = "GridView4"
            Dim DataKeyName As String = "Serial No"
            BuildGridView(GridView4, ControlName, DataKeyName)
            Dim dt As DataTable = GetDataTable(GetSQL(GridView4, Nothing, keyword, Nothing))
            Dim dataView As New DataView(dt)
            dataView.Sort = IIf(ControlName = currentSortingGridview, currentSortedExpressionDirection, Nothing)

            GridView4.DataSource = dataView
            GridView4.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataRetailProduct(Optional ByVal TB_Search_Retail_Product As String = Nothing, Optional currentSortingGridview As String = Nothing, Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(TB_Search_Retail_Product)
        Try
            Dim ControlName As String = "GridView5"
            Dim DataKeyName As String = "Serial No"
            BuildGridView(GridView5, ControlName, DataKeyName)
            Dim dt As DataTable = GetDataTable(GetSQL(GridView5, Nothing, keyword, Nothing))
            Dim dataView As New DataView(dt)
            dataView.Sort = IIf(ControlName = currentSortingGridview, currentSortedExpressionDirection, Nothing)

            GridView5.DataSource = dataView
            GridView5.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataTagsType(Optional ByVal Tags_Group As String = Nothing)
        Dim keyword As String = EscapeChar(Tags_Group)
        Dim firstTagsGroup As String = IIf(keyword = "-1", Nothing, DDL_Tags_Group.SelectedValue)

        Try
            BuildGridView(GridView6, "GridView6", "ID")
            GridView6.DataSource = GetDataTable(GetSQL(GridView6, Nothing, firstTagsGroup, Nothing))
            GridView6.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' set the selected in financial year dropdownlist
        DDL_Tags_Group.SelectedIndex = DDL_Tags_Group.Items.IndexOf(DDL_Tags_Group.Items.FindByValue(firstTagsGroup))
    End Sub

    Protected Sub PopulateGridViewDataTagsDeployment(Optional ByVal StoreName As String = Nothing)
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Store_Name As String = EscapeChar(StoreName)

        Try
            Dim sqlStr = "EXEC SP_ESL_Tags_Deployment '" & Customer_ID & "', '" & Store_Name & "' "
            BuildGridView(GridView7, "GridView7", "Unique ID")  '' Format Gridview

            ' Clear any existing columns
            GridView7.Columns.Clear()

            '' Get the datatable
            Dim dt As DataTable = GetDataTable(sqlStr)

            '' Columns to skip
            Dim columnsToSkip As List(Of String) = New List(Of String) From {"Unique ID", "Customer ID", "Customer Name", "Store ID", "Services Group", "Editable"}

            ' Loop through each column in the DataTable
            For Each column As DataColumn In dt.Columns
                If Not columnsToSkip.Contains(column.ColumnName) Then
                    ' Create a new BoundField
                    Dim Bfield As New BoundField()

                    ' Set the properties of the BoundField
                    Bfield.DataField = column.ColumnName
                    Bfield.HeaderText = IIf(column.ColumnName = "Installation Date", "Reading Date", column.ColumnName)
                    Bfield.ItemStyle.Wrap = False

                    ' Add the BoundField to the GridView
                    GridView7.Columns.Add(Bfield)
                End If
            Next

            '' Add templatefield for Edit icon
            Dim TField As TemplateField = New TemplateField()
            TField.HeaderStyle.Width = Unit.Percentage(0)
            TField.ItemStyle.Wrap = False
            TField.ItemStyle.HorizontalAlign = HorizontalAlign.Center
            TField.ItemTemplate = New GridViewItemTemplateControl()
            GridView7.Columns.Add(TField)


            '' Bind the datatable to Gridview
            GridView7.DataSource = dt
            GridView7.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
    End Sub

    Protected Sub PopulateGridViewDataContract(Optional ByVal post_Selected_Year As String = Nothing, Optional currentSortingGridview As String = Nothing, Optional currentSortedExpressionDirection As String = Nothing)
        Dim keyword As String = EscapeChar(post_Selected_Year)
        Try
            Dim ControlName As String = "GridView8"
            Dim DataKeyName As String = "Unique ID"
            BuildGridView(GridView8, ControlName, DataKeyName)
            Dim dt As DataTable = GetDataTable(GetSQL(GridView8, Nothing, keyword, Nothing))
            Dim dataView As New DataView(dt)
            dataView.Sort = IIf(ControlName = currentSortingGridview, currentSortedExpressionDirection, Nothing)

            GridView8.DataSource = dataView
            GridView8.DataBind()
        Catch ex As Exception
            'Response.Write("Error:  " & ex.Message)
        End Try

        '' set the selected in financial year dropdownlist
        DDL_Financial_Year.SelectedIndex = DDL_Financial_Year.Items.IndexOf(DDL_Financial_Year.Items.FindByValue(keyword))
    End Sub

    Protected Sub PopulateGridViewDataComment(Optional ByVal Unique_ID As String = Nothing)
        Try
            BuildGridView(GridView9, "GridView9", "Unique_ID")
            GridView9.DataSource = GetDataTable(GetSQL(GridView9, Nothing, Unique_ID, Nothing))
            GridView9.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' hide /show contract status log section controls
        MaintenanceContractLogSection.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        MaintenanceContractStatus_Header.Visible = IIf(Not Unique_ID Is Nothing, True, False)
        GridView9.Visible = IIf(Not Unique_ID Is Nothing, True, False)

        btnAddMaintenanceContractStatusLog.Visible = IIf(Not Unique_ID Is Nothing, True, False)

        '' Change the behavior of border line
        MaintenanceContractListingSection.Style.Add("border", "solid 1px")
        MaintenanceContractListingSection.Style.Add("border-color", IIf(Not Unique_ID Is Nothing, "#fff #dee2e6 #fff", "#fff #dee2e6 #dee2e6"))
        MaintenanceContractLogSection.Style.Add("border", IIf(Not Unique_ID Is Nothing, "solid 1px", "solid 0px"))
        MaintenanceContractLogSection.Style.Add("border-color", "#fff #dee2e6 #dee2e6")

        '' Draw last line if page count less than 1
        GridView9.Style.Add("border-bottom", "1px solid " & IIf(GridView9.PageCount < 2, "#ddd", "#fff"))

    End Sub




    '' Format Gridview
    Protected Sub BuildGridView(ByVal ControlObj As Object, ByVal ControlName As String, ByVal DataKeyName As String)
        Dim GridViewObj As GridView = CType(ControlObj, GridView)

        '' GridView Properties
        GridViewObj.ID = ControlName
        GridViewObj.AutoGenerateColumns = False
        GridViewObj.CellPadding = 4
        GridViewObj.Font.Size = 10
        GridViewObj.GridLines = GridLines.None
        GridViewObj.ShowHeader = True
        GridViewObj.ShowFooter = False
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
            Case "GridView1", "GridView6"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10

            Case "GridView2", "GridView3", "GridView4", "GridView5"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.AllowSorting = True

            Case "GridView7"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.ShowHeaderWhenEmpty = False

            Case "GridView8"
                GridViewObj.AllowPaging = False
                GridViewObj.AllowSorting = True
                GridViewObj.ShowFooter = True

            Case "GridView9"
                GridViewObj.AllowPaging = True
                GridViewObj.PageSize = 10
                GridViewObj.CssClass = "table"
                GridViewObj.ShowHeader = False
                GridViewObj.ShowFooter = False
                GridViewObj.GridLines = GridLines.None
                GridViewObj.Style.Add("border-top", "1px solid #ddd")
                GridViewObj.Style.Add("border-bottom", "1px solid #fff")

        End Select
    End Sub





    '' Formview controls
    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As FormViewModeEventArgs) Handles FormView1.ModeChanged
        FormView1.ChangeMode(e.NewMode)
        PopulateFormViewData()
    End Sub

    Protected Sub FormView1_DataBound(sender As Object, e As EventArgs) Handles FormView1.DataBound
        '' Loop through formview and controls visible based on formview mode
        Dim FormViewControlsName As String() = {"lbl_Created_Date", "lbl_Last_Updated", "lbl_Account_Status", "TB_Created_Date", "TB_Last_Updated", "DDL_Status"}
        For Each controlID As String In FormViewControlsName
            Dim control As Control = FormView1.FindControl(controlID)
            If Not control Is Nothing Then
                control.Visible = (FormView1.CurrentMode <> FormViewMode.Insert)
            End If
        Next

        '' Hide / Display the customer profile contenct based on formview mode
        CustomerProfileContent.Visible = (FormView1.CurrentMode <> FormViewMode.Insert)

        '' Pass existing name to a hidden field
        Dim Hidden_Old_Name As Label = FormView1.FindControl("LB_Hidden_Name")
        Dim Name As TextBox = FormView1.FindControl("TB_Name")
        If Not Name Is Nothing Then
            Hidden_Old_Name.Text = Name.Text
        Else
            Hidden_Old_Name.Text = ""
        End If
    End Sub

    Protected Sub DDL_Status_DataBound(sender As Object, e As EventArgs)
        Dim Status As Label = FormView1.FindControl("LB_Status")
        Dim DDL_Status As DropDownList = FormView1.FindControl("DDL_Status")
        Dim i = DDL_Status.Items.IndexOf(DDL_Status.Items.FindByText(Status.Text))
        DDL_Status.SelectedIndex = i
    End Sub




    '' Gridview control
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If


            '' Change the color for sorted column
            If currentSortedColumnIndex >= 0 AndAlso currentSortedColumnIndex < e.Row.Cells.Count Then
                e.Row.Cells(currentSortedColumnIndex).CssClass = "sorted-column"
            End If
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If

            '' Change the color for sorted column
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).CssClass = "sorted-column"
            Next
        End If
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If

            '' Set column background color for currency, amoung and warranty
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency")).Style.Add("background-color", "#ebf5fb")       '' Currency
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency Value")).Style.Add("background-color", "#ebf5fb") '' Amount
            e.Row.Cells(GetColumnIndexByName(e.Row, "Under Warranty")).Style.Add("background-color", IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "Under Warranty")).Text <> "No", "#e8f8f5", "#fdedec"))   '' Warranty status

            '' Change the color for sorted column
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).CssClass = "sorted-column"
            Next
        End If
    End Sub

    Protected Sub GridView4_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView4.RowCreated
        If e.Row.RowType = DataControlRowType.Header Then
            Dim gridView As GridView = DirectCast(sender, GridView)

            ' Create a new GridViewRow for the additional header row
            Dim headerRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim headerCellArray As Object() = {New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell()}

            For i = 0 To headerCellArray.Length - 1
                ' Create a new TableCell to hold the content of the additional header row
                Select Case i
                    Case 0
                        headerCellArray(i).columnspan = 3
                        headerCellArray(i).Text = "Product Info"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))   ' Add the TableCell to the header row
                    Case 1
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).Text = "Product Value"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 2
                        headerCellArray(i).columnspan = 3
                        headerCellArray(i).Text = "Installation Location / Date"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 3
                        headerCellArray(i).columnspan = 3
                        headerCellArray(i).Text = "Warranty"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 4
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                End Select

                ' Add the header row to the GridView
                gridView.Controls(0).Controls.AddAt(0, headerRow)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
            Else
                DeleteLinkButton.Enabled = True
            End If

            '' Set column background color for currency, amoung and warranty
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency")).Style.Add("background-color", "#ebf5fb")       '' Currency
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency Value")).Style.Add("background-color", "#ebf5fb") '' Amount
            e.Row.Cells(GetColumnIndexByName(e.Row, "Under Warranty")).Style.Add("background-color", IIf(e.Row.Cells(GetColumnIndexByName(e.Row, "Under Warranty")).Text <> "No", "#e8f8f5", "#fdedec"))   '' Warranty status

            '' Change the color for sorted column
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(currentSortedColumnIndex).CssClass = "sorted-column"
            Next
        End If
    End Sub

    Protected Sub GridView5_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView5.RowCreated
        If e.Row.RowType = DataControlRowType.Header Then
            Dim gridView As GridView = DirectCast(sender, GridView)

            ' Create a new GridViewRow for the additional header row
            Dim headerRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim headerCellArray As Object() = {New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell()}

            For i = 0 To headerCellArray.Length - 1
                ' Create a new TableCell to hold the content of the additional header row
                Select Case i
                    Case 0
                        headerCellArray(i).columnspan = 4
                        headerCellArray(i).Text = "Product Info"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))   ' Add the TableCell to the header row
                    Case 1
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).Text = "Product Value"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 2
                        headerCellArray(i).columnspan = 3
                        headerCellArray(i).Text = "Installation Location / Date"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 3
                        headerCellArray(i).columnspan = 3
                        headerCellArray(i).Text = "Warranty"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 4
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                End Select

                ' Add the header row to the GridView
                gridView.Controls(0).Controls.AddAt(0, headerRow)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView6_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView6.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object

            '' If the banner is in used, then can only edit it but cannot delete
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
                DeleteLinkButton.Visible = True
            End If
            EditLinkButton.Enabled = True   '' Edit button remain visible
        End If
    End Sub

    Protected Sub GridView7_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView7.RowDataBound
        Dim maxColumnWidth As Integer = 0
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 2 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(e.Row.Cells(i).Text.IndexOf("_") + 1)  '' Get the string after the underscore symbol
                e.Row.Cells(i).Wrap = False
            Next

            Dim totalColumns As Integer = e.Row.Cells.Count

            ' First two columns with 20% width each
            e.Row.Cells(0).Width = New Unit(10, UnitType.Percentage)
            e.Row.Cells(1).Width = New Unit(30, UnitType.Percentage)

            ' Distribute the remaining width evenly among the rest of the columns
            Dim remainingWidth As Double = 60.0 / (totalColumns - 3)
            For i = 2 To totalColumns - 1
                e.Row.Cells(i).Width = New Unit(remainingWidth, UnitType.Percentage)
            Next

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigate link to Customer details page
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Click on edit icon to view edit page
            Dim ActionctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The column where the edit and delete button are
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(ActionctrlCellIndex).Controls(0), LinkButton)
            EditLinkButton.Text = IIf(drv("Editable") <> 1, "<i class='bi bi-lock'></i>", "<i class='bi bi-pencil-fill'></i>")
            EditLinkButton.CssClass = IIf(drv("Editable") <> 1, "btn btn-xs btn-light disabled", "btn btn-xs btn-info")
            EditLinkButton.CommandArgument = (e.Row.RowIndex + 1) & "|" & drv("Unique ID") & "|" & drv("Customer ID") & "|" & drv("Store ID") & "|" & drv("Installation Date")
            AddHandler EditLinkButton.Command, AddressOf Edit_Tags_Deployment_Click

            EditLinkButton.Style.Add("margin-right", "5px")   '' add separator between button

            Dim DeleteLinkButton As LinkButton = TryCast(e.Row.Cells(ActionctrlCellIndex).Controls(1), LinkButton)
            DeleteLinkButton.Visible = IIf(drv("Editable") <> 1, False, True)
            DeleteLinkButton.Text = "<i class='bi bi-trash'></i>"
            DeleteLinkButton.CssClass = "btn btn-xs btn-danger"
            DeleteLinkButton.CommandArgument = drv("Unique ID")
            DeleteLinkButton.OnClientClick = "return confirm('Are you sure to delete record?')"
            AddHandler DeleteLinkButton.Command, AddressOf Delete_Tags_Deployment_Click
        End If
    End Sub

    'Protected Sub GridView7_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView7.RowCreated
    '    If e.Row.RowType = DataControlRowType.Header Then
    '        Dim gridView As GridView = DirectCast(sender, GridView)
    '        Dim dataColumns As DataControlFieldCollection = gridView.Columns

    '        ' Create a new GridViewRow for the additional header row
    '        Dim headerRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
    '        Dim headerRowCellArray As New List(Of TableCell)()

    '        Dim CurrentTagsGroup As String = Nothing
    '        Dim NextTagsGroup As String = Nothing
    '        Dim NoColumnSpan As Integer = 0
    '        Dim HeaderTextTypeCount As Integer = 0

    '        For i = 0 To e.Row.Cells.Count - 1
    '            NextTagsGroup = IIf(Not e.Row.Cells(i).Text.Contains("InfoTag") And Not e.Row.Cells(i).Text.Contains("RF Station"), "&nbsp;", e.Row.Cells(i).Text.Substring(0, e.Row.Cells(i).Text.IndexOf("_") + 1))
    '            If CurrentTagsGroup <> NextTagsGroup Then
    '                '' Create new TableCell with proper columnspan before the currentgroup of header name change
    '                If Not String.IsNullOrEmpty(CurrentTagsGroup) Then
    '                    headerRowCellArray(HeaderTextTypeCount - 1).ColumnSpan = NoColumnSpan
    '                    headerRowCellArray(HeaderTextTypeCount - 1).Text = Replace(CurrentTagsGroup, "_", "")
    '                    headerRowCellArray(HeaderTextTypeCount - 1).HorizontalAlign = HorizontalAlign.Left
    '                    headerRow.Cells.Add(headerRowCellArray(HeaderTextTypeCount - 1))
    '                End If

    '                headerRowCellArray.Add(New TableCell())
    '                NoColumnSpan = 1
    '                HeaderTextTypeCount += 1
    '                CurrentTagsGroup = NextTagsGroup
    '            Else
    '                NoColumnSpan += 1
    '            End If
    '        Next

    '        '' Create TableCell for the last group of header name
    '        If Not String.IsNullOrEmpty(CurrentTagsGroup) Then
    '            headerRowCellArray(HeaderTextTypeCount - 1).ColumnSpan = NoColumnSpan
    '            headerRowCellArray(HeaderTextTypeCount - 1).Text = Replace(CurrentTagsGroup, "_", "")
    '            headerRowCellArray(HeaderTextTypeCount - 1).HorizontalAlign = HorizontalAlign.Left
    '            headerRow.Cells.Add(headerRowCellArray(HeaderTextTypeCount - 1))
    '        End If

    '        gridView.Controls(0).Controls.AddAt(0, headerRow)   ' Add the header row to the GridView

    '    ElseIf e.Row.RowType = DataControlRowType.DataRow Then
    '        e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
    '        e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
    '    End If
    'End Sub

    Protected Sub GridView7_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView7.RowCreated
        If e.Row.RowType = DataControlRowType.Header Then
            ' Assume the first row of headers are already the subtypes created by default or by RowDataBound.
            ' Now, create an additional header row for main types.

            ' Create a new header row for main types.
            Dim newHeaderRow As New GridViewRow(-1, -1, DataControlRowType.Header, DataControlRowState.Normal)
            Dim mainType As String = String.Empty
            Dim columnSpanCounter As Integer = 1

            ' Add empty cells for fixed columns up to last fixed column.
            For i = 0 To 1
                Dim cell As New TableCell()
                cell.Text = "&nbsp;" ' Non-breaking space for layout purposes.
                newHeaderRow.Cells.Add(cell)
            Next

            ' Process dynamic columns starting from the 3th cell.
            For i As Integer = 2 To e.Row.Cells.Count - 1
                Dim currentHeader As String = e.Row.Cells(i).Text
                Dim currentMainType As String = currentHeader.Split("_"c)(0)

                If mainType <> currentMainType Then
                    If i > 2 Then ' Add the previous main type cell if not the first iteration.
                        Dim headerCell As New TableCell()
                        headerCell.ColumnSpan = columnSpanCounter
                        headerCell.Text = mainType
                        headerCell.HorizontalAlign = HorizontalAlign.Left ' left-align the main type header.
                        newHeaderRow.Cells.Add(headerCell)
                        columnSpanCounter = 1 ' Reset counter.
                    End If
                    mainType = currentMainType
                Else
                    columnSpanCounter += 1 ' Increment span for repeated main types.
                End If

                ' For the last group of columns, add the header cell outside the loop.
                If i = e.Row.Cells.Count - 1 Then
                    Dim headerCell As New TableCell()
                    headerCell.ColumnSpan = columnSpanCounter
                    headerCell.Text = mainType
                    headerCell.HorizontalAlign = HorizontalAlign.Left
                    newHeaderRow.Cells.Add(headerCell)
                End If
            Next

            ' Add the new header row to the GridView at the first position.
            CType(sender, GridView).Controls(0).Controls.AddAt(0, newHeaderRow)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            ' Adding mouseover and mouseout attributes for highlighting the row
            'e.Row.Attributes.Add("OnMouseOver", "this.style.backgroundColor='#f5f5f5'; this.style.cursor='default';")
            'e.Row.Attributes.Add("OnMouseOut", "this.style.backgroundColor='';")

            ' Alternatively, if you prefer not to use inline styles for the cursor,
            ' you could set a CSS class on the row and define the cursor style in your CSS.
            ' Example:
            e.Row.Attributes.Add("class", "data-row-cursor-pointer")
        End If
    End Sub


    Protected Sub GridView8_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView8.RowDataBound
        If e.Row.RowType = DataControlRowType.Header Then
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), e.Row)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            '' click on a row to hightlight and postback to populate comment section
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridView8, "Select$" & e.Row.RowIndex)  ''Indicate a row index selected then trigger PopulateGridViewDataComment(Unique_ID)
            e.Row.Attributes("style") = "cursor:pointer"

            '' Get the gridview row detaisl
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim EditLinkButton As LinkButton = CType((e.Row.FindControl("EditLinkButton")), LinkButton)       '' Edit Link button object
            Dim DeleteLinkButton As LinkButton = CType((e.Row.FindControl("DeleteLinkButton")), LinkButton)   '' Delete Link button object
            Dim UploadLinkButton As LinkButton = CType((e.Row.FindControl("UploadLinkButton")), LinkButton)   '' Delete Link button object

            '' If currenct date is more than 3 months from the start date, the record will locked for editing / deletion to mark as finalized
            If CInt(drv("In Used")) > 0 Then
                DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                DeleteLinkButton.ToolTip = "Item Locked"
                DeleteLinkButton.Enabled = False
                EditLinkButton.Visible = False
                UploadLinkButton.Visible = Len(drv("Invoice No")) < 1
            Else
                DeleteLinkButton.Enabled = Len(drv("Invoice No")) < 1
                EditLinkButton.Visible = True
                UploadLinkButton.Visible = True

                If Len(drv("Invoice No")) > 0 Then
                    DeleteLinkButton.Text = "<i class='bi bi-lock'></i>"
                    DeleteLinkButton.CssClass = "btn btn-xs btn-light disabled"
                    DeleteLinkButton.ToolTip = "Item Locked"
                    DeleteLinkButton.Enabled = False
                    EditLinkButton.Visible = False
                    UploadLinkButton.Visible = False
                End If
            End If


            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If drv("Invoice No") <> "" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If


            '' Total Amount
            TotalAmount += CDec(DataBinder.Eval(e.Row.DataItem, "Base Currency Amount"))


            '' Change the color for sorted column
            If currentSortedColumnIndex >= 0 AndAlso currentSortedColumnIndex < e.Row.Cells.Count Then
                e.Row.Cells(currentSortedColumnIndex).CssClass = "sorted-column"
            End If


            '' Format the content of Reference No column
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Text = drv("Reference No")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Style.Add("font-style", "italic")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Reference No")).Style.Add("color", "#808080")

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency")).Text = "SGD"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency Amount")).Text = TotalAmount.ToString("#,##0.00")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency Amount")).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, "Base Currency Amount")).Style.Add("padding-right", "15px !important")

            e.Row.Cells(GetColumnIndexByName(e.Row, "End Date")).Text = "TOTAL"
            e.Row.Cells(GetColumnIndexByName(e.Row, "End Date")).HorizontalAlign = HorizontalAlign.Right
            e.Row.Cells(GetColumnIndexByName(e.Row, "End Date")).Style.Add("padding-right", "30px !important")
        End If

        '' Format the Base Currency and amount column
        Dim formatColumn As String() = {"Base Currency", "Base Currency Amount"}
        For i = 0 To formatColumn.Length - 1
            e.Row.Cells(GetColumnIndexByName(e.Row, formatColumn(i))).Style.Add("text-align", "right !important")
            e.Row.Cells(GetColumnIndexByName(e.Row, formatColumn(i))).Style.Add("padding-right", "15px !important")
        Next
    End Sub

    Protected Sub GridView8_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView8.RowCommand
        If e.CommandName = "Select" Then
            Dim eCommandArgument As Array = Split(e.CommandArgument, "|")  '' pass eCommandArgument to array
            Dim Selected_Row_Of_Contract As Integer = Convert.ToInt32(eCommandArgument(0))

            '' Format the sorted header row with arrow symbol
            CustomizeSortedHeaderRow(DirectCast(sender, GridView), GridView8.HeaderRow)

            '' Change the cell color of the selected row for the View (select command)
            Dim rowIndex As Integer = Convert.ToInt32(Selected_Row_Of_Contract)
            Dim selectedRow As GridViewRow = GridView8.Rows(rowIndex)

            '' Highlight the row with gray color
            For i = 0 To selectedRow.Cells.Count - 1
                selectedRow.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
            Next
        End If
    End Sub

    Protected Sub GridView8_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView8.RowCreated
        If e.Row.RowType = DataControlRowType.Header Then
            Dim gridView As GridView = DirectCast(sender, GridView)

            ' Create a new GridViewRow for the additional header row
            Dim headerRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim headerCellArray As Object() = {New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell(), New TableCell()}

            For i = 0 To headerCellArray.Length - 1
                ' Create a new TableCell to hold the content of the additional header row
                Select Case i
                    Case 0
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))   ' Add the TableCell to the header row
                    Case 1
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 2
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 3
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).Text = "Contract Period"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 4
                        headerCellArray(i).columnspan = 2
                        headerCellArray(i).Text = "Value (SGD)"
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 5
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 6
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                    Case 7
                        headerCellArray(i).columnspan = 1
                        headerCellArray(i).horizontalalign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerCellArray(i))
                End Select

                ' Add the header row to the GridView
                gridView.Controls(0).Controls.AddAt(0, headerRow)
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Protected Sub GridView8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView8.SelectedIndexChanged
        Dim Selected_Row_Index As Integer = GridView8.SelectedRow.RowIndex
        '' To get the Contract unique id
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim FY As String = DDL_Financial_Year.SelectedValue
        Dim RowNo As Integer = GridView8.SelectedRow.Cells(0).Text
        Dim Selected_Contract_Unique_ID As String = Get_Value("SELECT [Unique ID] FROM (SELECT ROW_NUMBER() OVER(ORDER BY [Store Name] ASC) AS No, * FROM R_Maintenance_Contract WHERE [Customer ID] ='" & Customer_ID & "' AND [FY] LIKE '%" & FY & "%') TBL WHERE No = " & RowNo & " ", "Unique ID")

        '' Repopulate gridviews
        PopulateFormViewData()
        PopulateGridViewDataContract(DDL_Financial_Year.SelectedValue, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
        PopulateGridViewDataComment(Selected_Contract_Unique_ID)


        '' Stay highligthed the row when click on update device record
        Dim selectedRow As GridViewRow = GridView8.Rows(Selected_Row_Index)
        For i = 0 To selectedRow.Cells.Count - 1
            selectedRow.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
        Next

        '' display the device id as header
        MaintenanceContractStatus_Header.Visible = True
        btnAddMaintenanceContractStatusLog.Visible = True


        '' pass value hidden field in maintenance contract log panel
        TB_Selected_TB_MaintenanceContractStatusLog_Parent_Row_Index.Text = Selected_Row_Index  '' row index of device selected in gridview 1
        TB_Selected_TB_MaintenanceContractStatusLog_Unique_ID.Text = Selected_Contract_Unique_ID  '' unique id of the device selected in gridview 1

        '' Draw last/bottom line if page count less than 1
        GridView9.Style.Add("border-bottom", If(GridView9.PageCount < 2, "1px solid #ddd", "1px solid #fff"))

    End Sub






    '' Common gridview events
    Protected Sub GridView_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging, GridView2.PageIndexChanging, GridView3.PageIndexChanging, GridView4.PageIndexChanging, GridView5.PageIndexChanging, GridView6.PageIndexChanging, GridView7.PageIndexChanging, GridView8.PageIndexChanging, GridView9.PageIndexChanging
        Dim CurrActiveGV As GridView = CType(sender, GridView)
        CurrActiveGV.PageIndex = e.NewPageIndex

        PopulateFormViewData()

        Select Case CurrActiveGV.ID
            Case "GridView1"
                PopulateGridViewDataBanner()
            Case "GridView2"
                PopulateGridViewDataStore(TB_Search_Store.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
            Case "GridView3"
                PopulateGridViewDataProductType(TB_Search_Product_Type.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
            Case "GridView4"
                PopulateGridViewDataProduct(TB_Search_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
            Case "GridView5"
                PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
            Case "GridView6"
                PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
            Case "GridView7"
                PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)           '' PageIndexchanging for Gridview7 not working, have to create manually
            Case "GridView8"
                PopulateGridViewDataContract(DDL_Financial_Year.SelectedValue, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
            Case "GridView9"
                CustomizeSortedHeaderRow(GridView8, GridView8.HeaderRow)
                PopulateGridViewDataComment(TB_Selected_TB_MaintenanceContractStatusLog_Unique_ID.Text)
        End Select
    End Sub

    Protected Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated, GridView2.RowCreated, GridView3.RowCreated, GridView6.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub

    Private Sub GridView_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs) Handles GridView2.Sorting, GridView3.Sorting, GridView4.Sorting, GridView5.Sorting, GridView8.Sorting
        Dim gridView As GridView = DirectCast(sender, GridView)
        Dim dt As DataTable = Nothing

        '' Populate based on gridview
        Select Case gridView.ID
            Case "GridView2"
                Dim keyword = EscapeChar(TB_Search_Store.Text)
                BuildGridView(gridView, gridView.ID, "Store ID")
                dt = GetDataTable(GetSQL(gridView, Nothing, keyword, Nothing))
                ViewState("currentSortingGridview") = gridView.ID

            Case "GridView3"
                Dim Services_Group As String = Left(Session("User_Group"), 1)
                Dim keyword = EscapeChar(TB_Search_Product_Type.Text)
                BuildGridView(gridView, gridView.ID, "Code")
                dt = GetDataTable(GetSQL(gridView, Services_Group, keyword, Nothing))
                ViewState("currentSortingGridview") = gridView.ID

            Case "GridView4"
                Dim keyword As String = EscapeChar(TB_Search_Product.Text)
                BuildGridView(gridView, gridView.ID, "Serial No")
                dt = GetDataTable(GetSQL(gridView, Nothing, keyword, Nothing))
                ViewState("currentSortingGridview") = gridView.ID

            Case "GridView5"
                Dim keyword As String = EscapeChar(TB_Search_Retail_Product.Text)
                BuildGridView(gridView, gridView.ID, "Serial No")
                dt = GetDataTable(GetSQL(gridView, Nothing, keyword, Nothing))
                ViewState("currentSortingGridview") = gridView.ID

            Case "GridView8"
                Dim FY As String = DDL_Financial_Year.SelectedValue
                BuildGridView(gridView, gridView.ID, "Unique ID")
                dt = GetDataTable(GetSQL(gridView, Nothing, FY, Nothing))
                ViewState("currentSortingGridview") = gridView.ID

        End Select

        Dim dataView As New DataView(dt)
        Dim sortExpression As String = e.SortExpression

        ' Common sorting logic
        If ViewState("SortDirection") IsNot Nothing AndAlso ViewState("SortExpression") IsNot Nothing Then
            Dim previousSortExpression As String = ViewState("SortExpression").ToString()
            Dim previousSortDirection As String = ViewState("SortDirection").ToString()

            ViewState("SortDirection") = If(previousSortExpression = sortExpression, If(previousSortDirection = "ASC", "DESC", "ASC"), "DESC")
        Else
            ViewState("SortDirection") = "DESC"
            ViewState("SortExpression") = gridView.Columns(0).SortExpression
        End If

        ViewState("SortExpression") = sortExpression
        Dim sortDirection As String = If(ViewState("SortDirection").ToString() = "ASC", " ASC", " DESC")
        sortExpression += sortDirection

        dataView.Sort = sortExpression

        gridView.DataSource = dataView
        gridView.DataBind()

    End Sub

    Private Sub CustomizeSortedHeaderRow(ByVal gridView As GridView, ByVal headerRow As GridViewRow)
        Dim sortExpression As String = ViewState("SortExpression")?.ToString()
        Dim sortDirection As String = ViewState("SortDirection")?.ToString()

        ' If sortExpression is empty, set it to the first column's SortExpression
        If String.IsNullOrEmpty(sortExpression) Then
            sortExpression = gridView.Columns(0).SortExpression
            sortDirection = "ASC"
            currentSortedColumnIndex = 0
        End If

        ' Loop through the headerrow control field to find which is the current selected column
        For Each field As DataControlField In gridView.Columns
            If field.SortExpression = sortExpression Then
                Dim cellIndex As Integer = gridView.Columns.IndexOf(field)
                Dim sortArrow As New Label()
                sortArrow.CssClass = "sort-arrow " & If(sortDirection = "ASC", "asc", "desc")

                ' Add the sorting arrow inside a <span> element
                Dim span As New HtmlGenericControl("span")
                span.Controls.Add(sortArrow)

                ' Append the <span> to the header cell
                headerRow.Cells(cellIndex).Controls.Add(span)

                ' Get current sorted column index
                currentSortedColumnIndex = cellIndex
            End If

            ' Explicitly set right alignment for the "Base Currency Value" column header
            If TypeOf field Is BoundField Then
                Dim boundField As BoundField = CType(field, BoundField)

                If boundField.HeaderText = "Amount" Then
                    Dim cellIndex As Integer = gridView.Columns.IndexOf(field)
                    headerRow.Cells(cellIndex).Style.Add("text-align", "right !important")
                End If
            End If
        Next
    End Sub





    '' Gridview of listbox
    Protected Sub GridView_Tags_Deployment_List_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView_Tags_Deployment_List.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' click on a row to hightlight and postback to populate value
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridView_Tags_Deployment_List, "Select$" & e.Row.RowIndex)
            e.Row.Attributes("style") = "cursor:default"
        End If
    End Sub

    Protected Sub GridView_Tags_Deployment_List_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView_Tags_Deployment_List.RowCommand
        If e.CommandName = "Select" Then
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            Dim selectedRow As GridViewRow = GridView_Tags_Deployment_List.Rows(rowIndex)
            If selectedRow.CssClass <> "selectedRow" Then
                For Each row As GridViewRow In GridView_Tags_Deployment_List.Rows
                    row.CssClass = "unselectedRow"      ' Deselect all rows
                Next
                selectedRow.CssClass = "selectedRow"    ' Select the clicked row

                For i = 0 To selectedRow.Cells.Count - 2
                    '' populate tags group and tags type value to both dropdownlist and tags quantity value
                    Dim Services_Group As String = Left(Session("User_Group"), 1)
                    Dim filter As String = DDL_Tags_Group_Deployment.SelectedValue
                    Shared_Dropdownlist_Bind(DDL_Tags_Type_Deployment, GetSQL(DDL_Tags_Type_Deployment, Services_Group, Nothing, filter), "Tags Type", "Tags Type", "Please select", True)
                    DDL_Tags_Group_Deployment.SelectedIndex = DDL_Tags_Group_Deployment.Items.IndexOf(DDL_Tags_Group_Deployment.Items.FindByValue(selectedRow.Cells(0).Text))
                    DDL_Tags_Group_Deployment.Enabled = False
                    DDL_Tags_Type_Deployment.SelectedIndex = DDL_Tags_Type_Deployment.Items.IndexOf(DDL_Tags_Type_Deployment.Items.FindByValue(selectedRow.Cells(1).Text))
                    DDL_Tags_Type_Deployment.Enabled = False
                    TB_Tags_Deployment_Quantity.Text = selectedRow.Cells(2).Text
                Next
                AddTagsDeploymentLineItems.Text = "Update"
            Else
                selectedRow.CssClass = "unselectedRow"   ' Deselect the clicked row
                For i = 0 To selectedRow.Cells.Count - 2
                    '' reset tags group and tags type value to both dropdownlist and clear quantity value
                    Dim Services_Group As String = Left(Session("User_Group"), 1)
                    Dim filter As String = DDL_Tags_Group_Deployment.SelectedValue
                    Shared_Dropdownlist_Bind(DDL_Tags_Type_Deployment, GetSQL(DDL_Tags_Type_Deployment, Services_Group, Nothing, filter), "Tags Type", "Tags Type", "Please select", True)
                    DDL_Tags_Group_Deployment.SelectedIndex = 0
                    DDL_Tags_Group_Deployment.Enabled = True
                    DDL_Tags_Type_Deployment.SelectedIndex = 0
                    DDL_Tags_Type_Deployment.Enabled = True
                    TB_Tags_Deployment_Quantity.Text = String.Empty
                Next
                AddTagsDeploymentLineItems.Text = "Add"
            End If
        End If
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub GridView_Tags_Deployment_List_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView_Tags_Deployment_List.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Call javascript function for GridView Row highlight effect
            If e.Row.RowType = DataControlRowType.DataRow Then
                e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
                e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
            End If

            Dim drv As System.Data.DataRowView = e.Row.DataItem
            Dim DeleteLineItem As LinkButton = e.Row.FindControl("lnkDeleteLineItem")
            DeleteLineItem.Text = "<i class='bi bi-trash'></i>"
            DeleteLineItem.CssClass = "btn btn-xs btn-danger"
            DeleteLineItem.CausesValidation = False
            DeleteLineItem.CommandName = "Delete"
            AddHandler DeleteLineItem.Click, AddressOf DeleteLineItem_Click
        End If
    End Sub





    '' Modal control - Banner
    Protected Sub Add_Banner_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddBanner.Click
        ModalHeaderBanner.Text = "Add New Banner"
        btnSaveBanner.Text = "Save"
        btnCancelBanner.Text = "Cancel"

        Dim Services_Group As String = Left(Session("User_Group"), 1)

        '' Initialize the fields
        TB_Banner.Text = String.Empty
        TB_Banner_Frequency.Text = IIf(Services_Group = "R", "00000000", String.Empty)

        '' Initialize the Unique ID hidden field for new created
        Dim HiddenFields As Array = {TB_Selected_Banner_Row_Index, TB_Selected_Banner_ID, TB_Selected_Banner_Name, TB_Selected_Banner_Customer_ID, TB_Selected_Banner_Frequency}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        bannerfrequencyrow.Visible = IIf(Services_Group = "R", True, False)
        RequiredField_TB_Banner_Frequency.Enabled = IIf(Services_Group = "R", True, False)
        RegExValidator_TB_Banner_Frequency.Enabled = IIf(Services_Group = "R", True, False)
        TB_Banner_Frequency.Visible = IIf(Services_Group = "R", True, False)


        bannernameexisterrormsg.Visible = False   '' hide Banner field error message
        popupBanner.Show()
    End Sub

    Protected Sub Edit_Banner_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderBanner.Text = "Edit Banner"
        btnSaveBanner.Text = "Update"
        btnCancelBanner.Text = "Cancel"

        '' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Banner_Row_Index, TB_Selected_Banner_ID, TB_Selected_Banner_Name, TB_Selected_Banner_Customer_ID, TB_Selected_Banner_Frequency}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).text = EditLinkButtonCommandArgument(i)
        Next

        Dim Services_Group As String = Left(Session("User_Group"), 1)
        bannerfrequencyrow.Visible = IIf(Services_Group = "R", True, False)
        RequiredField_TB_Banner_Frequency.Enabled = IIf(Services_Group = "R", True, False)
        RegExValidator_TB_Banner_Frequency.Enabled = IIf(Services_Group = "R", True, False)
        TB_Banner_Frequency.Visible = IIf(Services_Group = "R", True, False)

        '' Populate cell value with value from hidden fields to edit mode
        TB_Banner.Text = TB_Selected_Banner_Name.Text
        TB_Banner_Frequency.Text = IIf(Services_Group = "R", TB_Selected_Banner_Frequency.Text, String.Empty)

        bannernameexisterrormsg.Visible = False   '' hide Banner field error message
        popupBanner.Show()
    End Sub

    Protected Sub Save_Banner_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveBanner.Click
        Dim Banner_ID As TextBox = pnlAddEditCustomerBanner.FindControl("TB_Selected_Banner_ID")
        Dim Banner_Name As TextBox = pnlAddEditCustomerBanner.FindControl("TB_Banner")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim Banner_Frequency As TextBox = pnlAddEditCustomerBanner.FindControl("TB_Banner_Frequency")

        '' Define SQL
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Banner N'" & Banner_ID.Text & "', N'" & EscapeChar(Trim(Banner_Name.Text)) & "', N'" & Customer_ID & "', N'" & Banner_Frequency.Text & "' "

        '' Check database banner name exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Banner WHERE [Services Group] = '" & Services_Group & "' AND [Banner Name] = N'" & EscapeChar(Trim(Banner_Name.Text)) & "' AND [Customer ID] = N'" & Customer_ID & "' ", "NoOfRecord")) > 0, True, False)

        '' return the existing banner name's banner id, this value used for comparison if user is editing an existing account
        Dim Existing_Banner_Name_ID As String = Get_Value("SELECT [Banner ID] FROM R_Maintenance_Banner WHERE [Services Group] = '" & Services_Group & "' AND [Banner Name] = N'" & EscapeChar(Trim(Banner_Name.Text)) & "' AND [Customer ID] = N'" & Customer_ID & "' ", "Banner ID")

        Try
            If btnSaveBanner.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(bannernameexisterrormsg, popupBanner, TB_Banner, sqlStr, True)
                Else
                    Validate_Name(bannernameexisterrormsg, popupBanner, TB_Banner, sqlStr, False)
                End If
            ElseIf btnSaveBanner.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(bannernameexisterrormsg, popupBanner, TB_Banner, sqlStr, True)
                Else
                    If Banner_ID.Text = Existing_Banner_Name_ID Then
                        Validate_Name(bannernameexisterrormsg, popupBanner, TB_Banner, sqlStr, True)
                    Else
                        Validate_Name(bannernameexisterrormsg, popupBanner, TB_Banner, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate gridview
        PopulateGridViewDataBanner()
    End Sub

    Protected Sub Delete_Banner_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Banner WHERE Banner_ID = '" & DeleteLinkButtonCommandArgument(1) & "' AND Customer_ID = '" & DeleteLinkButtonCommandArgument(3) & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-Populate banner gridview
        PopulateGridViewDataBanner()
    End Sub





    '' Modal control - Store
    Protected Sub Add_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddStore.Click
        ModalHeaderStore.Text = "Add New Store"
        btnSaveStore.Text = "Save"
        btnCancelStore.Text = "Cancel"

        '' Re-bind Banner Dropdownlist when adding new store
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Banner, GetSQL(DDL_Banner, Services_Group, Nothing, Nothing), "Banner Name", "Banner ID", "Please select", True)

        '' Initialize the fields
        DDL_Banner.Enabled = True
        DDL_Banner.SelectedIndex = 0   '' always reset the banner dropdownlist selection
        TB_Store.Text = String.Empty
        storenameexisterrormsg.Visible = False

        status_row.Visible = False
        lbl_Store_Status.Visible = False
        DDL_Store_Status.Visible = False
        DDL_Store_Status.Width = Unit.Percentage(50)
        CompareValidator_DDL_Store_Status.Enabled = False


        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Store_Row_Index, TB_Selected_Store_ID, TB_Selected_Store_Name, TB_Selected_Store_Status, TB_Selected_Store_Banner_ID, TB_Selected_Store_Customer_ID}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupStore.Show()
    End Sub

    Protected Sub Edit_Store_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderStore.Text = "Update Store"
        btnSaveStore.Text = "Update"
        btnCancelStore.Text = "Cancel"

        '' Re-bind Banner Dropdownlist when adding new store
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Banner, GetSQL(DDL_Banner, Services_Group, Nothing, Nothing), "Banner Name", "Banner ID", "Please select", True)

        '' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Store_Row_Index, TB_Selected_Store_ID, TB_Selected_Store_Name, TB_Selected_Store_Status, TB_Selected_Store_Banner_ID, TB_Selected_Store_Customer_ID}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' bind the value of banner dropdownlist and disable it
        DDL_Banner.SelectedIndex = DDL_Banner.Items.IndexOf(DDL_Banner.Items.FindByValue(TB_Selected_Store_Banner_ID.Text))
        DDL_Banner.Enabled = False

        '' Populate cell value with value from hidden fields to edit mode
        TB_Store.Text = TB_Selected_Store_Name.Text

        status_row.Visible = True
        lbl_Store_Status.Visible = True
        DDL_Store_Status.Visible = True
        DDL_Store_Status.Width = Unit.Percentage(50)
        DDL_Store_Status.SelectedIndex = DDL_Store_Status.Items.IndexOf(DDL_Store_Status.Items.FindByText(TB_Selected_Store_Status.Text))
        CompareValidator_DDL_Store_Status.Enabled = True

        storenameexisterrormsg.Visible = False '' hide Banner field error message
        popupStore.Show()
    End Sub

    Protected Sub Save_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveStore.Click
        Dim Store_ID As TextBox = pnlAddEditCustomerStore.FindControl("TB_Selected_Store_ID")
        Dim Store_Name As TextBox = pnlAddEditCustomerStore.FindControl("TB_Store")
        Dim Status As DropDownList = pnlAddEditCustomerStore.FindControl("DDL_Store_Status")
        Dim Banner_ID As DropDownList = pnlAddEditCustomerStore.FindControl("DDL_Banner")
        Dim Banner_ID_Selected_Edit As TextBox = pnlAddEditCustomerStore.FindControl("TB_Selected_Store_Banner_ID")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Services_Group As String = Left(Session("User_Group"), 1)

        '' Define SQL
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Store N'" & Store_ID.Text & "', N'" & EscapeChar(Trim(Store_Name.Text)) & "', N'" & Status.SelectedValue & "', N'" & IIf(btnSaveStore.Text = "Save", Banner_ID.SelectedValue, Banner_ID_Selected_Edit.Text) & "', N'" & Customer_ID & "' "

        '' Check database store name exists under a banner, same store can be of differenct banner
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Store WHERE [Services Group] = '" & Services_Group & "' AND [Store Name] = N'" & EscapeChar(Trim(Store_Name.Text)) & "' AND [Banner ID] = N'" & Banner_ID.Text & "' AND [Customer ID] = N'" & Customer_ID & "' ", "NoOfRecord")) > 0, True, False)

        '' return the existing store name's store id,  this value used for comparison if user is editing an existing account
        Dim Existing_Store_Name_ID As String = Get_Value("SELECT [Store ID] FROM R_Maintenance_Store WHERE [Services Group] = '" & Services_Group & "' AND [Store Name] = N'" & EscapeChar(Trim(Store_Name.Text)) & "' AND [Banner ID] = N'" & Banner_ID.Text & "' AND [Customer ID] = N'" & Customer_ID & "' ", "Store ID")

        Try
            If btnSaveStore.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(storenameexisterrormsg, popupStore, TB_Store, sqlStr, True)
                Else
                    Validate_Name(storenameexisterrormsg, popupStore, TB_Store, sqlStr, False)
                End If
            ElseIf btnSaveStore.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(storenameexisterrormsg, popupStore, TB_Store, sqlStr, True)
                Else
                    If Store_ID.Text = Existing_Store_Name_ID Then
                        Validate_Name(storenameexisterrormsg, popupStore, TB_Store, sqlStr, True)
                    Else
                        Validate_Name(storenameexisterrormsg, popupStore, TB_Store, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate Formview and gridview
        PopulateGridViewDataBanner()
        PopulateGridViewDataStore(TB_Search_Store.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Delete_Store_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Store WHERE Store_ID = '" & DeleteLinkButtonCommandArgument(1) & "' AND Customer_ID = '" & DeleteLinkButtonCommandArgument(5) & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-Populate store gridview
        PopulateGridViewDataBanner()
        PopulateGridViewDataStore(TB_Search_Store.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub






    '' Modal control - contract
    Protected Sub DDL_Financial_Year_Load(sender As Object, e As EventArgs) Handles DDL_Financial_Year.Load
        '' first load for financial year dropdownlist
        If Not IsPostBack Then
            Try
                Dim Services_Group As String = Left(Session("User_Group"), 1)
                Shared_Dropdownlist_Bind(DDL_Financial_Year, GetSQL(DDL_Financial_Year, Services_Group, Nothing, Nothing), "FY", "FY", "Please select", True)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Financial_Year_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Financial_Year.SelectedIndexChanged
        PopulateGridViewDataContract(DDL_Financial_Year.SelectedValue, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
        PopulateGridViewDataComment()
    End Sub

    Protected Sub Add_Contract_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddContract.Click
        ModalHeaderContract.Text = "Add New Contract"
        btnSaveContract.Text = "Save"
        btnCancelContract.Text = "Cancel"

        '' Re-bind Store when creating new contract
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Store ID"

        '' Bind the store list
        'Shared_Dropdownlist_Bind(DDL_Contract_Store, GetSQL(DDL_Contract_Store, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Contract_Store, GetSQL(DDL_Contract_Store, Services_Group, Nothing, Nothing), "Store Name", "Store ID", "Please select", True)

        '' Rebind with all available store for new financial year if all store has contract for financial year
        'If DDL_Contract_Store.Items.Count <= 1 And DDL_Contract_Store.SelectedValue = "-1" Then
        '    Shared_Dropdownlist_Bind(DDL_Contract_Store, GetSQL(DDL_Contract_Store, Services_Group, Nothing, Nothing), "Store Name", "Store ID", "Please select", True)
        'Else
        '    Shared_Dropdownlist_Bind(DDL_Contract_Store, GetSQL(DDL_Contract_Store, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        'End If

        '' Bind currency
        Shared_Dropdownlist_Bind(DDL_Contract_Currency, GetSQL(DDL_Contract_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", False)

        '' Bind Contract Process Status
        Shared_Dropdownlist_Bind(DDL_Contract_Process_Status, GetSQL(DDL_Contract_Process_Status, Nothing, Nothing, Nothing), "Status_Description", "Status_Code", "Please select", False)


        '' Initialize the fields
        DDL_Contract_Store.SelectedIndex = 0
        DDL_Contract_Store.Enabled = True
        TB_Contract_Start_Date.Text = String.Empty
        TB_Contract_Start_Date.Enabled = True
        TB_Contract_End_Date.Text = String.Empty
        TB_Contract_End_Date.Enabled = True
        DDL_Contract_Currency.SelectedIndex = DDL_Contract_Currency.Items.IndexOf(DDL_Contract_Currency.Items.FindByText("SGD"))
        DDL_Contract_Process_Status.SelectedIndex = 0
        TB_Contract_Amount.Text = String.Empty
        TB_Reference_No.Text = String.Empty


        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Contract_Row_Index, TB_Selected_Contract_Unique_ID, TB_Selected_Contract_Store_ID, TB_Selected_Contract_Start_Date, TB_Selected_Contract_End_Date, TB_Selected_Contract_Currency, TB_Selected_Contract_Amount, TB_Selected_Contract_Reference_No, TB_Selected_Contract_Process_Status}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupContract.Show()
    End Sub

    Protected Sub Edit_Contract_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderContract.Text = "Update Product"
        btnSaveContract.Text = "Update"
        btnCancelContract.Text = "Cancel"

        '' Re-bind Store when creating new contract
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Contract_Store, GetSQL(DDL_Contract_Store, Services_Group, Nothing, Nothing), "Store Name", "Store ID", "Please select", True)

        '' Bind currency
        Shared_Dropdownlist_Bind(DDL_Contract_Currency, GetSQL(DDL_Contract_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", False)

        '' Bind Contract Process Status
        Shared_Dropdownlist_Bind(DDL_Contract_Process_Status, GetSQL(DDL_Contract_Process_Status, Nothing, Nothing, Nothing), "Status_Description", "Status_Code", "Please select", False)


        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Contract_Row_Index, TB_Selected_Contract_Unique_ID, TB_Selected_Contract_Store_ID, TB_Selected_Contract_Start_Date, TB_Selected_Contract_End_Date, TB_Selected_Contract_Currency, TB_Selected_Contract_Amount, TB_Selected_Contract_Reference_No, TB_Selected_Contract_Process_Status}

        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' populate value for all modal fields in edit mode
        DDL_Contract_Store.SelectedIndex = DDL_Contract_Store.Items.IndexOf(DDL_Contract_Store.Items.FindByValue(TB_Selected_Contract_Store_ID.Text))
        DDL_Contract_Store.Enabled = False
        TB_Contract_Start_Date.Text = CDate(TB_Selected_Contract_Start_Date.Text).ToString("yyyy-MM-dd")
        TB_Contract_Start_Date.Enabled = False
        TB_Contract_End_Date.Text = CDate(TB_Selected_Contract_End_Date.Text).ToString("yyyy-MM-dd")
        DDL_Contract_Currency.SelectedIndex = DDL_Contract_Currency.Items.IndexOf(DDL_Contract_Currency.Items.FindByValue(TB_Selected_Contract_Currency.Text))
        TB_Contract_Amount.Text = CDbl(TB_Selected_Contract_Amount.Text).ToString("#,##0.00")
        TB_Reference_No.Text = TB_Selected_Contract_Reference_No.Text
        DDL_Contract_Process_Status.SelectedIndex = DDL_Contract_Process_Status.Items.IndexOf(DDL_Contract_Process_Status.Items.FindByValue(TB_Selected_Contract_Process_Status.Text))

        popupContract.Show()
    End Sub

    Protected Sub Save_Contract_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveContract.Click
        Dim Unique_ID As TextBox = pnlAddEditCustomerContract.FindControl("TB_Selected_Contract_Unique_ID")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Contract_Store As DropDownList = pnlAddEditCustomerContract.FindControl("DDL_Contract_Store")
        Dim Start_Date As TextBox = pnlAddEditCustomerContract.FindControl("TB_Contract_Start_Date")
        Dim End_Date As TextBox = pnlAddEditCustomerContract.FindControl("TB_Contract_End_Date")
        Dim Currency As DropDownList = pnlAddEditCustomerContract.FindControl("DDL_Contract_Currency")
        Dim Amount As TextBox = pnlAddEditCustomerContract.FindControl("TB_Contract_Amount")
        Dim Reference_No As TextBox = pnlAddEditCustomerContract.FindControl("TB_Reference_No")
        Dim Status_Code As DropDownList = pnlAddEditCustomerContract.FindControl("DDL_Contract_Process_Status")

        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Contract N'" & Unique_ID.Text &
                                                              "', N'" & Customer_ID &
                                                              "', N'" & Contract_Store.SelectedValue &
                                                              "', N'" & Start_Date.Text &
                                                              "', N'" & End_Date.Text &
                                                              "', N'" & Currency.SelectedValue &
                                                              "', N'" & Amount.Text &
                                                              "', N'" & EscapeChar(Reference_No.Text) &
                                                              "', N'" & Status_Code.SelectedValue & "' "
        Dim post_Selected_Year As String = Nothing

        ' Proceed with parsing and comparison since we now know the dates are not empty
        Dim startDate As DateTime = DateTime.Parse(Start_Date.Text)
        Dim endDate As DateTime = DateTime.Parse(End_Date.Text)


        If endDate <= startDate Then
            ' End date is the same as or before the start date
            AlertMessageMsgBox("The End date must be later than the Start date.")
            popupContract.Show()
        Else
            Try
                '' In Edit Mode
                If btnSaveContract.Text = "Update" Then
                    If CDate(End_Date.Text) > CDate(Start_Date.Text) Then
                        Validate_Contract_End_Date(sqlStr, True)
                    Else
                        Validate_Contract_End_Date(sqlStr, False)
                    End If
                    post_Selected_Year = Get_Value("SELECT TOP 1 [FY] FROM R_Maintenance_Contract WHERE [Unique ID] = '" & Unique_ID.Text & "' ", "FY")
                Else
                    '' In New Mode
                    'Dim RecordExists As Boolean = IIf(Get_Value("SELECT COUNT([Store ID]) AS NoOfRecord FROM R_Maintenance_Contract WHERE [Customer ID] = '" & Customer_ID & "' AND [Store ID] = '" & Contract_Store.SelectedValue & "' AND [FY] = '" & CDate(End_Date.Text).Year & "' ", "NoOfRecord") > 0, True, False)
                    'If Not RecordExists Then
                    Validate_Contract_End_Date(sqlStr, True)
                    post_Selected_Year = CDate(End_Date.Text).Year
                    'Else
                    '    AlertMessageMsgBox("Store " & Contract_Store.SelectedItem.Text & " is already included in contract for financial year " & DDL_Financial_Year.SelectedValue)
                    '    popupContract.Show()
                    'End If
                End If
            Catch ex As Exception
                Response.Write("Error:   " & ex.Message)
            End Try
        End If


        '' Rebind the financial dropdownlist
        DDL_Financial_Year_Rebind(post_Selected_Year)


        '' repoopulate gridview
        PopulateGridViewDataContract(post_Selected_Year, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))   '' Populate the gridview of current working on financial year
        PopulateGridViewDataComment(TB_Selected_Contract_Unique_ID.Text)


        '' Selected row stay highlighted in update mode
        If Len(TB_Selected_Contract_Row_Index.Text) > 0 Then
            Dim selectedRow As GridViewRow = GridView8.Rows(TB_Selected_Contract_Row_Index.Text)
            For i = 0 To selectedRow.Cells.Count - 1
                selectedRow.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
            Next
        End If
    End Sub

    Protected Sub Validate_Contract_End_Date(ByVal sqlStr As String, ByVal isPassedValidate As Boolean)
        If isPassedValidate Then
            enddatelessererrormsg.Visible = False
            enddatelessererrormsg.Text = String.Empty
            RunSQL(sqlStr)
        Else
            enddatelessererrormsg.Visible = True
            enddatelessererrormsg.Text = "Contract End Date must be greater than Start Date"
            enddatelessererrormsg.Style.Add("display", "inline")
            popupContract.Show()
            TB_Contract_End_Date.Focus()
        End If
    End Sub

    Protected Sub Delete_Contract_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Contract_Status_Log WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; " &
                                   "DELETE FROM DB_FileUpload WHERE Replace(File_Name, '.pdf', '') IN (SELECT Replace(Invoice_No, 'TWS/', '') FROM Maintenance_Contract WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "'); " &
                                   "DELETE FROM Maintenance_Contract WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "'; "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        Dim Max_Financial_Year As String = Get_Value("SELECT MAX([FY]) AS FY FROM R_Maintenance_Contract where [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ", "FY")
        Dim Current_Selected_Financial_Year As String = DDL_Financial_Year.SelectedValue
        Dim post_Selected_Year As String = IIf(CInt(Max_Financial_Year) > CInt(Current_Selected_Financial_Year), Current_Selected_Financial_Year, Max_Financial_Year)


        '' Rebind the financial dropdownlist and re-populate maintenance_contract gridview
        DDL_Financial_Year_Rebind(post_Selected_Year)

        '' repoopulate gridview
        PopulateGridViewDataContract(post_Selected_Year, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
        PopulateGridViewDataComment()
    End Sub

    Protected Sub btnCancelContract_Click(sender As Object, e As EventArgs) Handles btnCancelContract.Click
        PopulateGridViewDataContract(DDL_Financial_Year.SelectedValue, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
        PopulateGridViewDataComment(TB_Selected_Contract_Unique_ID.Text)
    End Sub

    Protected Sub DDL_Financial_Year_Rebind(ByVal selectedFY As String)
        '' rebind dropdownlist
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Financial_Year, GetSQL(DDL_Financial_Year, Services_Group, Nothing, Nothing), "FY", "FY", "Please select", True)
        DDL_Financial_Year.SelectedIndex = DDL_Financial_Year.Items.IndexOf(DDL_Financial_Year.Items.FindByText(selectedFY))
    End Sub

    Protected Sub RegEx_TB_Contract_Amount_ServerValidate(source As Object, args As ServerValidateEventArgs)
        Dim contractValue As String = args.Value
        If contractValue.StartsWith("$") Then
            contractValue = contractValue.Substring(1) ' Remove the leading dollar sign
        End If

        ' Parse the value as decimal and round it to 2 decimal places
        Dim value As Decimal
        If Decimal.TryParse(contractValue, value) Then
            value = Math.Round(value, 2)
            args.IsValid = True
        Else
            args.IsValid = False
        End If
    End Sub



    Protected Sub Upload_Invoice_Modal_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderUploadInvoice.Text = "Upload Invoice"
        btnUploadInvoice.Text = "Upload"
        btnCancelUploadInvoice.Text = "Cancel"

        Chk_UploadSameInvoice.Checked = False   '' Clear the checkbox wheneve click to upload file

        '' Get row command argument
        Dim UploadLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim UploadLinkButtonCommandArgument As Array = Split(UploadLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Contract_To_Upload_Invoice_Row_Index, TB_Selected_Contract_To_Upload_Invoice_Unique_ID, TB_Selected_Contract_To_Upload_Invoice_Store_ID, TB_Selected_Contract_To_Upload_Invoice_Reference_No}

        For i = 0 To UploadLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = UploadLinkButtonCommandArgument(i)
        Next


        Dim Max_Financial_Year As String = Get_Value("SELECT MAX([FY]) AS FY FROM R_Maintenance_Contract where [Customer ID] = '" & Request.QueryString("Customer_ID") & "' ", "FY")
        Dim Current_Selected_Financial_Year As String = DDL_Financial_Year.SelectedValue
        Dim post_Selected_Year As String = IIf(CInt(Max_Financial_Year) > CInt(Current_Selected_Financial_Year), Current_Selected_Financial_Year, Max_Financial_Year)


        '' Rebind the financial dropdownlist and re-populate maintenance_contract gridview
        DDL_Financial_Year_Rebind(post_Selected_Year)

        '' repoopulate gridview
        PopulateGridViewDataContract(post_Selected_Year, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
        PopulateGridViewDataComment()

        popupUploadInvoice.Show()
    End Sub

    Protected Sub UploadFile(ByVal sender As Object, ByVal e As EventArgs) Handles btnUploadInvoice.Click
        ' Find the FileUpload control inside the pnlUploadInvoice panel
        Dim Invoice_Upload As FileUpload = pnlUploadInvoice.FindControl("FileUpload1")
        Dim unique_ID As String = TB_Selected_Contract_To_Upload_Invoice_Unique_ID.Text

        ' Check if the control is found
        If Invoice_Upload IsNot Nothing Then
            If Invoice_Upload.HasFile Then
                Dim filename As String = Path.GetFileName(Invoice_Upload.PostedFile.FileName)
                Dim contentType As String = Invoice_Upload.PostedFile.ContentType
                Dim Doc_Category As String = "Invoice"

                If Not Chk_UploadSameInvoice.Checked Then
                    '' Check if file exists
                    Dim FileExists As Integer = Get_Value("SELECT Count(*) AS FileExists FROM DB_FileUpload WHERE File_Name = '" & filename & "'", "FileExists")
                    If FileExists < 1 Then
                        If Len(Replace(filename, ".pdf", "")) > 8 Then
                            AlertMessageMsgBox("Invoice file name incorrect.\nExample: 40123456.pdf")
                            Exit Sub
                        End If

                        Try
                            Using fs As Stream = FileUpload1.PostedFile.InputStream
                                Using br As New BinaryReader(fs)
                                    Dim bytes As Byte() = br.ReadBytes(fs.Length)
                                    Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString

                                    Using con As New SqlConnection(constr)
                                        Dim query As String = "INSERT INTO DB_FileUpload (File_Name, Uploaded_DateTime, Content_Type, File_Data, Doc_Category) VALUES(@File_Name, @Uploaded_DateTime, @Content_Type, @File_Data, @Doc_Category) " &
                                                    "UPDATE Maintenance_Contract SET Status_Code = (SELECT Value_2 AS Status_Code FROM DB_Lookup WHERE Lookup_Name = 'Contract Process Status' AND Value_1 = 'Invoiced'), Invoice_No = 'TWS/' + Replace(@File_Name, '.pdf', '') WHERE Unique_ID = @Unique_ID "

                                        Using cmd As New SqlCommand(query)
                                            cmd.Connection = con
                                            cmd.Parameters.Add("@File_Name", SqlDbType.NVarChar).Value = filename
                                            cmd.Parameters.Add("@Uploaded_DateTime", SqlDbType.DateTime).Value = System.DateTime.Now()
                                            cmd.Parameters.Add("@Content_type", SqlDbType.NVarChar).Value = contentType
                                            cmd.Parameters.Add("@File_Data", SqlDbType.VarBinary).Value = bytes
                                            cmd.Parameters.Add("@Doc_Category", SqlDbType.NVarChar).Value = Doc_Category
                                            cmd.Parameters.Add("@Unique_ID", SqlDbType.NVarChar).Value = unique_ID
                                            con.Open()
                                            cmd.ExecuteNonQuery()
                                            con.Close()
                                        End Using
                                    End Using
                                End Using
                            End Using

                            AlertMessageMsgBox(IIf(Chk_UploadSameInvoice.Checked, "Record successfully updated.", "File successfully upload."))
                        Catch ex As Exception
                            AlertMessageMsgBox("File upload failed.")
                        End Try
                    Else
                        AlertMessageMsgBox("File exists.")
                    End If
                Else
                    Try
                        ' update contract record without upload file.
                        Dim Query As String = "UPDATE Maintenance_Contract SET Status_Code = (SELECT Value_2 AS Status_Code FROM DB_Lookup WHERE Lookup_Name = 'Contract Process Status' AND Value_1 = 'Invoiced'), Invoice_No = 'TWS/' + Replace('" & filename & "', '.pdf', '') WHERE Unique_ID = '" & unique_ID & "' "
                        RunSQL(Query)
                    Catch ex As Exception
                        Response.Write("File Upload - Error: " & ex.Message)
                    End Try
                End If
            End If
        End If

        '' Rebind the financial dropdownlist
        Dim post_Selected_Year = DDL_Financial_Year.SelectedValue
        DDL_Financial_Year_Rebind(post_Selected_Year)


        '' repoopulate gridview
        PopulateGridViewDataContract(post_Selected_Year, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))   '' Populate the gridview of current working on financial year
        PopulateGridViewDataComment(unique_ID)


        '' Selected row stay highlighted in update mode
        Dim selectedRowIndex As Integer = TB_Selected_Contract_To_Upload_Invoice_Row_Index.Text
        If Len(selectedRowIndex) > 0 Then
            Dim selectedRow As GridViewRow = GridView8.Rows(selectedRowIndex)
            For i = 0 To selectedRow.Cells.Count - 1
                selectedRow.Cells(i).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")
            Next
        End If
    End Sub



    '' Model control - contract status log
    Protected Sub Add_Maintenance_Status_Log_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddMaintenanceContractStatusLog.Click
        ModalHeaderMaintenanceContractStatusLog.Text = "Add Comment"
        btnSaveMaintenanceContractStatusLog.Text = "Save"
        btnCancelMaintenanceContractStatusLog.Text = "Cancel"

        TB_MaintenanceContractStatusLog.Text = String.Empty
        TB_Selected_TB_MaintenanceContractStatusLog_ID.Text = String.Empty  '' clear this hidden field when adding new
        popupMaintenanceContractStatusLog.Show()
    End Sub

    Protected Sub Edit_Maintenance_Status_Log_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderMaintenanceContractStatusLog.Text = "Update Comment"
        btnSaveMaintenanceContractStatusLog.Text = "Update"
        btnCancelMaintenanceContractStatusLog.Text = "Cancel"

        '' Get the gridview row details
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        TB_MaintenanceContractStatusLog.Text = HttpUtility.HtmlDecode(RemoveHTMLWhiteSpace(row.Cells(1).Text))

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")

        '' Pass value to hidden field when row command button is clicked
        TB_Selected_TB_MaintenanceContractStatusLog_ID.Text = EditLinkButtonCommandArgument(1)  '' Get log ID from EditLinkButtonCommandArgument array index 1

        popupMaintenanceContractStatusLog.Show()
    End Sub

    Protected Sub Save_Maintenance_Status_Log_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveMaintenanceContractStatusLog.Click
        Dim Maintenance_Contract_Status_Log_ID As String = TB_Selected_TB_MaintenanceContractStatusLog_ID.Text
        Dim Maintenance_Contract_Status_Log As String = TryCast(pnlAddEditMaintenanceContractStatusLog.FindControl("TB_MaintenanceContractStatusLog"), TextBox).Text
        Dim Maintenance_Contract_Unique_ID As String = TB_Selected_TB_MaintenanceContractStatusLog_Unique_ID.Text
        Dim By_Who As String = Session("User_Name")

        Try
            Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Contract_Status_Log N'" & Maintenance_Contract_Status_Log_ID & "', N'" & Maintenance_Contract_Status_Log & "', N'" & Maintenance_Contract_Unique_ID & "', N'USR', N'" & By_Who & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Save contract status log - Error: " & ex.Message)
        End Try

        popupMaintenanceContractStatusLog.Hide()

        '' Populate contract log (comment) section
        PopulateGridViewDataComment(Maintenance_Contract_Unique_ID)


        '' Format the sorted header row with arrow symbol
        CustomizeSortedHeaderRow(GridView8, GridView8.HeaderRow)

        '' Selected row stay highlighted
        Dim Selected_Row_Index As TextBox = pnlAddEditMaintenanceContractStatusLog.FindControl("TB_Selected_TB_MaintenanceContractStatusLog_Parent_Row_Index")
        GridView8.Rows(Selected_Row_Index.Text).BackColor = Drawing.ColorTranslator.FromHtml("#eeeeee")

    End Sub

    Protected Sub Delete_Maintenance_Status_Log_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)  '' Command button from Gridview2
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Dim sqlStr As String = "DELETE FROM Maintenance_Contract_Status_Log WHERE ID = " & DeleteLinkButtonCommandArgument(1)
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        '' Populate contract log (comment) section
        Dim Maintenance_Contract_Unique_ID As String = DeleteLinkButtonCommandArgument(2)
        PopulateGridViewDataComment(Maintenance_Contract_Unique_ID)


        '' Format the sorted header row with arrow symbol
        CustomizeSortedHeaderRow(GridView8, GridView8.HeaderRow)

    End Sub

    Protected Sub btnCancelMaintenanceContractStatusLog_Click(sender As Object, e As EventArgs) Handles btnCancelMaintenanceContractStatusLog.Click
        PopulateGridViewDataComment(TB_Selected_TB_MaintenanceContractStatusLog_Unique_ID.Text)
        '' Format the sorted header row with arrow symbol
        CustomizeSortedHeaderRow(GridView8, GridView8.HeaderRow)
    End Sub




    '' MOdal control - Product Type
    Protected Sub Add_Product_Type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddProductType.Click
        ModalHeaderProductType.Text = "Add New Product Type"
        btnSaveProductType.Text = "Save"
        btnCancelProductType.Text = "Cancel"

        '' Bind the product category
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Product_Category, GetSQL(DDL_Product_Category, Services_Group, Nothing, Nothing), "Category", "Category", "Please select", True)

        '' Initialize the fields
        TB_Code.Text = String.Empty
        codeexisterrormsg.Visible = False
        TB_Code.Enabled = True
        TB_Product_Name.Text = String.Empty

        '' Hide / show the category dropdownlist based the login group
        'Dim Services_Group As String = Left(Session("User_Group"), 1)
        DDL_Product_Category.SelectedIndex = 0
        category_row.Visible = (Services_Group <> "H")
        lbl_category.Visible = (Services_Group <> "H")
        DDL_Product_Category.Visible = (Services_Group <> "H")
        CompareValidator_DDL_Product_Category.Enabled = (Services_Group <> "H")

        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Product_Type_Row_Index, TB_Selected_UID, TB_Selected_Code, TB_Selected_Product_Name, TB_Selected_Category}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupProductType.Show()
    End Sub

    Protected Sub Edit_Product_Type_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderProductType.Text = "Update Store"
        btnSaveProductType.Text = "Update"
        btnCancelProductType.Text = "Cancel"

        '' Bind the product category
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Shared_Dropdownlist_Bind(DDL_Product_Category, GetSQL(DDL_Product_Category, Services_Group, Nothing, Nothing), "Category", "Category", "Please select", True)

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Product_Type_Row_Index, TB_Selected_UID, TB_Selected_Code, TB_Selected_Product_Name, TB_Selected_Category}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' Populate cell value with value from hidden fields to edit mode
        TB_Code.Text = TB_Selected_Code.Text
        TB_Code.Enabled = False
        TB_Product_Name.Text = TB_Selected_Product_Name.Text

        '' Hide / show the category dropdownlist based the login group
        DDL_Product_Category.SelectedIndex = DDL_Product_Category.Items.IndexOf(DDL_Product_Category.Items.FindByText(TB_Selected_Category.Text))
        category_row.Visible = (Services_Group <> "H")
        lbl_category.Visible = (Services_Group <> "H")
        DDL_Product_Category.Visible = (Services_Group <> "H")
        CompareValidator_DDL_Product_Category.Enabled = (Services_Group <> "H")


        codeexisterrormsg.Visible = False '' hide code field error message
        popupProductType.Show()
    End Sub

    Protected Sub Save_Product_Type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveProductType.Click
        Dim UID As TextBox = pnlAddEditProductType.FindControl("TB_Selected_UID")
        Dim Code As TextBox = pnlAddEditProductType.FindControl("TB_Code")
        Dim Product_Name As TextBox = pnlAddEditProductType.FindControl("TB_Product_Name")
        Dim Category As DropDownList = pnlAddEditProductType.FindControl("DDL_Product_Category")
        Dim Services_Group As String = Left(Session("User_Group"), 1)

        '' Define SQL
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Product_Type N'" & UID.Text & "', N'" & EscapeChar(Code.Text) & "', N'" & EscapeChar(Trim(Product_Name.Text)) & "', N'" & DDL_Product_Category.SelectedValue & "', N'" & Services_Group & "' "

        '' Check if product type code is exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Product_Type WHERE [Services Group] = '" & Services_Group & "' AND [Code] = N'" & EscapeChar(Code.Text) & "' ", "NoOfRecord")) > 0, True, False)

        '' return the existing store name's store id,  this value used for comparison if user is editing an existing account
        Dim Existing_UID As String = Get_Value("SELECT UID FROM R_Maintenance_Product_Type WHERE [Services Group] = '" & Services_Group & "' AND [Code] = N'" & EscapeChar(Trim(Code.Text)) & "' ", "UID")

        Try
            If btnSaveProductType.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(codeexisterrormsg, popupProductType, TB_Code, sqlStr, True)
                Else
                    Validate_Name(codeexisterrormsg, popupProductType, TB_Code, sqlStr, False)
                End If
            ElseIf btnSaveProductType.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(codeexisterrormsg, popupProductType, TB_Code, sqlStr, True)
                Else
                    If UID.Text = Existing_UID Then  '' Check if is editing the same record
                        Validate_Name(codeexisterrormsg, popupProductType, TB_Code, sqlStr, True)
                    Else
                        Validate_Name(codeexisterrormsg, popupProductType, TB_Code, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate Formview and gridview
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Delete_Product_Type_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Product_Type WHERE UID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-populate gridview
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub




    '' Modal control - Product
    Protected Sub Add_Product_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddProduct.Click
        ModalHeaderProduct.Text = "Add New Product"
        btnSaveProduct.Text = "Save"
        btnCancelProduct.Text = "Cancel"

        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Product, GetSQL(DDL_Product, Services_Group, Nothing, Nothing), "Product Name", "Code", "Please select", True)                '' Re-bind product dropdownlist
        Shared_Dropdownlist_Bind(DDL_Location, GetSQL(DDL_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)             '' Re-bind store dropdownlist (only active store)
        Shared_Dropdownlist_Bind(DDL_Warranty_Cover, GetSQL(DDL_Warranty_Cover, Nothing, Nothing, Nothing), "Value_1", "Value_2", "Please select", True)           '' Bind warranty cover period dropdownlist
        Shared_Dropdownlist_Bind(DDL_Value_Currency, GetSQL(DDL_Value_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", True)         '' Bind value currency dropdownlist

        '' Initialize the fields
        TB_Serial_No.Text = String.Empty
        serialnoexisterrormsg.Visible = False   '' hide serial no field error message
        DDL_Product.SelectedIndex = 0
        DDL_Location.SelectedIndex = 0
        TB_Install_Date.Text = String.Empty
        TB_Install_Date.Enabled = True
        TB_Start_Date.Text = String.Empty
        TB_Start_Date.Enabled = True
        DDL_Warranty_Cover.SelectedIndex = 1
        DDL_Warranty_Cover.Enabled = False      '' Default is disabled, as it only allow user to select 1 year warranty


        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Product_Row_Index, TB_Selected_Product_Unique_ID, TB_Selected_Product_Serial_No, TB_Selected_Product_Code, TB_Selected_Product_Store_ID, TB_Selected_Product_Start_Date, TB_Selected_Product_Warranty_Cover_Period, TB_Selected_Value_Currency, TB_Selected_Product_Value, TB_Selected_Product_Install_Date}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        DDL_Value_Currency.SelectedValue = DDL_Value_Currency.Items.FindByText("SGD").Value  '' Default as SGD
        TB_Product_Value.Text = String.Empty

        popupProduct.Show()
    End Sub

    Protected Sub Edit_Product_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderProduct.Text = "Update Product"
        btnSaveProduct.Text = "Update"
        btnCancelProduct.Text = "Cancel"

        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Product, GetSQL(DDL_Product, Services_Group, Nothing, Nothing), "Product Name", "Code", "Please select", True)                '' Re-bind product dropdownlist
        Shared_Dropdownlist_Bind(DDL_Location, GetSQL(DDL_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)             '' Re-bind store dropdownlist (only active store)
        Shared_Dropdownlist_Bind(DDL_Warranty_Cover, GetSQL(DDL_Warranty_Cover, Nothing, Nothing, Nothing), "Value_1", "Value_2", "Please select", True)           '' Bind warranty cover period dropdownlist
        Shared_Dropdownlist_Bind(DDL_Value_Currency, GetSQL(DDL_Value_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", True)         '' Bind value currency dropdownlist

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Product_Row_Index, TB_Selected_Product_Unique_ID, TB_Selected_Product_Serial_No, TB_Selected_Product_Code, TB_Selected_Product_Store_ID, TB_Selected_Product_Start_Date, TB_Selected_Product_Warranty_Cover_Period, TB_Selected_Value_Currency, TB_Selected_Product_Value, TB_Selected_Product_Install_Date}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' Populate cell value to edit mode
        TB_Serial_No.Text = TB_Selected_Product_Serial_No.Text
        TB_Install_Date.Text = CDate(TB_Selected_Product_Install_Date.Text).ToString("yyyy-MM-dd")
        TB_Install_Date.Enabled = True
        TB_Start_Date.Text = CDate(TB_Selected_Product_Start_Date.Text).ToString("yyyy-MM-dd")
        TB_Start_Date.Enabled = False
        TB_Product_Value.Text = CDbl(TB_Selected_Product_Value.Text).ToString("#,##0.00")

        '' bind the value back to dropdownlist in edit mode
        DDL_Product.SelectedIndex = DDL_Product.Items.IndexOf(DDL_Product.Items.FindByValue(TB_Selected_Product_Code.Text))
        DDL_Location.SelectedIndex = DDL_Location.Items.IndexOf(DDL_Location.Items.FindByValue(TB_Selected_Product_Store_ID.Text))
        DDL_Warranty_Cover.SelectedIndex = DDL_Warranty_Cover.Items.IndexOf(DDL_Warranty_Cover.Items.FindByValue(TB_Selected_Product_Warranty_Cover_Period.Text))
        DDL_Warranty_Cover.Enabled = False
        DDL_Value_Currency.SelectedIndex = DDL_Value_Currency.Items.IndexOf(DDL_Value_Currency.Items.FindByValue(TB_Selected_Value_Currency.Text))

        serialnoexisterrormsg.Visible = False   '' hide serial no field error message
        popupProduct.Show()
    End Sub

    Protected Sub Save_Product_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveProduct.Click
        Dim Unique_ID As TextBox = pnlAddEditCustomerProduct.FindControl("TB_Selected_Product_Unique_ID")
        Dim Serial_No As TextBox = pnlAddEditCustomerProduct.FindControl("TB_Serial_No")
        Dim Warranty_Start_Date As TextBox = pnlAddEditCustomerProduct.FindControl("TB_Start_Date")
        Dim Warranty_Cover_Period As DropDownList = pnlAddEditCustomerProduct.FindControl("DDL_Warranty_Cover")
        Dim Value_Currency As DropDownList = pnlAddEditCustomerProduct.FindControl("DDL_Value_Currency")
        Dim Product_Value As TextBox = pnlAddEditCustomerProduct.FindControl("TB_Product_Value")
        Dim Product_Code As DropDownList = pnlAddEditCustomerProduct.FindControl("DDL_Product")
        Dim Location As DropDownList = pnlAddEditCustomerProduct.FindControl("DDL_Location")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim Install_Date As TextBox = pnlAddEditCustomerProduct.FindControl("TB_Install_Date")

        '' Define SQL
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Product N'" & Unique_ID.Text & "', N'" & EscapeChar(Trim(Serial_No.Text)) & "', N'" & Install_Date.Text & "', N'" & Warranty_Start_Date.Text & "', N'" & Warranty_Cover_Period.SelectedValue & "', N'" & Value_Currency.SelectedValue & "', N'" & Product_Value.Text & "', N'" & Product_Code.SelectedValue & "', N'" & Location.SelectedValue & "', N'" & Customer_ID & "' "

        '' Check database store name exists under a banner, same store can be of differenct banner
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Product WHERE [Services Group] = '" & Services_Group & "' AND [Serial No] = N'" & EscapeChar(Trim(Serial_No.Text)) & "' ", "NoOfRecord")) > 0, True, False)

        '' return the existing store name's store id,  this value used for comparison if user is editing an existing account
        Dim Existing_Unique_ID As String = Get_Value("SELECT [Unique ID] FROM R_Maintenance_Product WHERE [Services Group] = '" & Services_Group & "' AND [Serial No] = N'" & EscapeChar(Trim(Serial_No.Text)) & "' ", "Unique ID")

        Try
            If btnSaveProduct.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(serialnoexisterrormsg, popupProduct, TB_Serial_No, sqlStr, True)
                Else
                    Validate_Name(serialnoexisterrormsg, popupProduct, TB_Serial_No, sqlStr, False)
                End If
            ElseIf btnSaveProduct.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(serialnoexisterrormsg, popupProduct, TB_Serial_No, sqlStr, True)
                Else
                    If Unique_ID.Text = Existing_Unique_ID Then  '' Check if is editing the same record
                        Validate_Name(serialnoexisterrormsg, popupProduct, TB_Serial_No, sqlStr, True)
                    Else
                        Validate_Name(serialnoexisterrormsg, popupProduct, TB_Serial_No, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate gridview
        PopulateGridViewDataStore(TB_Search_Store.Text)
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text)
        PopulateGridViewDataProduct(TB_Search_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Delete_Product_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Product WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-populate gridview
        PopulateGridViewDataStore(TB_Search_Store.Text)
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text)
        PopulateGridViewDataProduct(TB_Search_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Cancel_Product_Click(sender As Object, e As EventArgs) Handles btnCancelProduct.Click
        PopulateGridViewDataProduct(TB_Search_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub




    '' Modal control - Retail product
    Protected Sub Add_RetailProduct_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddRetailProduct.Click
        ModalHeaderRetailProduct.Text = "Add New Product"
        btnSaveRetailProduct.Text = "Save"
        btnCancelRetailProduct.Text = "Cancel"


        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Retail_Product, GetSQL(DDL_Retail_Product, Services_Group, Nothing, Nothing), "Product Name", "Code", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Location, GetSQL(DDL_Retail_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Product_Warranty_Cover, GetSQL(DDL_Retail_Product_Warranty_Cover, Nothing, Nothing, Nothing), "Value_1", "Value_2", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Product_Value_Currency, GetSQL(DDL_Retail_Product_Value_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", True)

        '' Initialize the fields
        TB_Retail_Product_Serial_No.Text = String.Empty
        DDL_Retail_Product.SelectedIndex = 0
        DDL_Retail_Location.SelectedIndex = 0
        TB_Retail_Product_Install_Date.Text = String.Empty
        TB_Retail_Product_Install_Date.Enabled = True
        TB_Retail_Product_Start_Date.Text = String.Empty
        TB_Retail_Product_Start_Date.Enabled = True
        DDL_Retail_Product_Warranty_Cover.SelectedIndex = 1
        DDL_Retail_Product_Warranty_Cover.Enabled = False   '' Default is disabled, as it only allow user to select 1 year warranty
        retailproductserialnoexisterrormsg.Visible = False

        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Retail_Product_Row_Index, TB_Selected_Retail_Product_Unique_ID, TB_Selected_Retail_Product_Serial_No, TB_Selected_Retail_Product_Code, TB_Selected_Retail_Product_Store_ID, TB_Selected_Retail_Product_Start_Date, TB_Selected_Retail_Product_Warranty_Cover_Period, TB_Selected_Retail_Product_Value_Currency, TB_Selected_Retail_Product_Value, TB_Selected_Retail_Product_Install_Date}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        Dim DDL_Value_Currency As DropDownList = pnlAddEditCustomerRetailProduct.FindControl("DDL_Retail_Product_Value_Currency")
        DDL_Value_Currency.SelectedValue = DDL_Value_Currency.Items.FindByText("SGD").Value  '' Default as SGD
        TB_Retail_Product_Value.Text = String.Empty

        popupRetailProduct.Show()
    End Sub

    Protected Sub Edit_Retail_Product_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderRetailProduct.Text = "Update Product"
        btnSaveRetailProduct.Text = "Update"
        btnCancelRetailProduct.Text = "Cancel"


        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Retail_Product, GetSQL(DDL_Retail_Product, Services_Group, Nothing, Nothing), "Product Name", "Code", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Location, GetSQL(DDL_Retail_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Product_Warranty_Cover, GetSQL(DDL_Retail_Product_Warranty_Cover, Nothing, Nothing, Nothing), "Value_1", "Value_2", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Retail_Product_Value_Currency, GetSQL(DDL_Retail_Product_Value_Currency, Nothing, Nothing, Nothing), "Currency", "Currency", "Please select", True)

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Retail_Product_Row_Index, TB_Selected_Retail_Product_Unique_ID, TB_Selected_Retail_Product_Serial_No, TB_Selected_Retail_Product_Code, TB_Selected_Retail_Product_Store_ID, TB_Selected_Retail_Product_Start_Date, TB_Selected_Retail_Product_Warranty_Cover_Period, TB_Selected_Retail_Product_Value_Currency, TB_Selected_Retail_Product_Value, TB_Selected_Retail_Product_Install_Date}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next


        '' Populate cell value to edit mode
        TB_Retail_Product_Serial_No.Text = TB_Selected_Retail_Product_Serial_No.Text
        TB_Retail_Product_Install_Date.Text = CDate(TB_Selected_Retail_Product_Install_Date.Text).ToString("yyyy-MM-dd")
        TB_Retail_Product_Install_Date.Enabled = False
        TB_Retail_Product_Start_Date.Text = CDate(TB_Selected_Retail_Product_Start_Date.Text).ToString("yyyy-MM-dd")
        TB_Retail_Product_Start_Date.Enabled = False
        TB_Retail_Product_Value.Text = CDbl(TB_Selected_Retail_Product_Value.Text).ToString("#,##0.00")

        '' bind the value back to dropdownlist in edit mode
        DDL_Retail_Product.SelectedIndex = DDL_Retail_Product.Items.IndexOf(DDL_Retail_Product.Items.FindByValue(TB_Selected_Retail_Product_Code.Text))
        DDL_Retail_Location.SelectedIndex = DDL_Retail_Location.Items.IndexOf(DDL_Retail_Location.Items.FindByValue(TB_Selected_Retail_Product_Store_ID.Text))
        DDL_Retail_Product_Warranty_Cover.SelectedIndex = DDL_Retail_Product_Warranty_Cover.Items.IndexOf(DDL_Retail_Product_Warranty_Cover.Items.FindByValue(TB_Selected_Retail_Product_Warranty_Cover_Period.Text))
        DDL_Retail_Product_Warranty_Cover.Enabled = False
        DDL_Retail_Product_Value_Currency.SelectedIndex = DDL_Retail_Product_Value_Currency.Items.IndexOf(DDL_Retail_Product_Value_Currency.Items.FindByValue(TB_Selected_Retail_Product_Value_Currency.Text))

        retailproductserialnoexisterrormsg.Visible = False   '' hide serial no field error message

        popupRetailProduct.Show()
    End Sub

    Protected Sub Save_RetailProduct_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveRetailProduct.Click
        Dim Unique_ID As TextBox = pnlAddEditCustomerRetailProduct.FindControl("TB_Selected_Retail_Product_Unique_ID")
        Dim Serial_No As TextBox = pnlAddEditCustomerRetailProduct.FindControl("TB_Retail_Product_Serial_No")
        Dim Warranty_Start_Date As TextBox = pnlAddEditCustomerRetailProduct.FindControl("TB_Retail_Product_Start_Date")
        Dim Warranty_Cover_Period As DropDownList = pnlAddEditCustomerRetailProduct.FindControl("DDL_Retail_Product_Warranty_Cover")
        Dim Value_Currency As DropDownList = pnlAddEditCustomerRetailProduct.FindControl("DDL_Retail_Product_Value_Currency")
        Dim Product_Value As TextBox = pnlAddEditCustomerRetailProduct.FindControl("TB_Retail_Product_Value")
        Dim Product_Code As DropDownList = pnlAddEditCustomerRetailProduct.FindControl("DDL_Retail_Product")
        Dim Location As DropDownList = pnlAddEditCustomerRetailProduct.FindControl("DDL_Retail_Location")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim Install_Date As TextBox = pnlAddEditCustomerRetailProduct.FindControl("TB_Retail_Product_Install_Date")

        '' Define SQL
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Product N'" & Unique_ID.Text & "', N'" & EscapeChar(Trim(Serial_No.Text)) & "', N'" & Install_Date.Text & "', N'" & Warranty_Start_Date.Text & "', N'" & Warranty_Cover_Period.SelectedValue & "', N'" & Value_Currency.SelectedValue & "', N'" & Product_Value.Text & "', N'" & Product_Code.SelectedValue & "', N'" & Location.SelectedValue & "', N'" & Customer_ID & "' "

        '' Check database store name exists under a banner, same store can be of differenct banner
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Product WHERE [Services Group] = '" & Services_Group & "' AND [Serial No] = N'" & EscapeChar(Trim(Serial_No.Text)) & "' ", "NoOfRecord")) > 0, True, False)

        '' return the existing store name's store id,  this value used for comparison if user is editing an existing account
        Dim Existing_Unique_ID As String = Get_Value("SELECT [Unique ID] FROM R_Maintenance_Product WHERE [Services Group] = '" & Services_Group & "' AND [Serial No] = N'" & EscapeChar(Trim(Serial_No.Text)) & "' ", "Unique ID")

        Try
            If btnSaveRetailProduct.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(retailproductserialnoexisterrormsg, popupRetailProduct, TB_Retail_Product_Serial_No, sqlStr, True)
                Else
                    Validate_Name(retailproductserialnoexisterrormsg, popupRetailProduct, TB_Retail_Product_Serial_No, sqlStr, False)
                End If
            ElseIf btnSaveRetailProduct.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(retailproductserialnoexisterrormsg, popupRetailProduct, TB_Retail_Product_Serial_No, sqlStr, True)
                Else
                    If Unique_ID.Text = Existing_Unique_ID Then  '' Check if is editing the same record
                        Validate_Name(retailproductserialnoexisterrormsg, popupRetailProduct, TB_Retail_Product_Serial_No, sqlStr, True)
                    Else
                        Validate_Name(retailproductserialnoexisterrormsg, popupRetailProduct, TB_Retail_Product_Serial_No, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate gridview
        PopulateGridViewDataStore(TB_Search_Store.Text)
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text)
        PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Delete_Retail_Product_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_Product WHERE Unique_ID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)

        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-populate gridview
        PopulateGridViewDataStore(TB_Search_Store.Text)
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text)
        PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub Cancel_Retail_Product_Click(sender As Object, e As EventArgs) Handles btnCancelRetailProduct.Click
        PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub





    '' Modal control - Tags Type
    Protected Sub DDL_Tags_Group_Load(sender As Object, e As EventArgs) Handles DDL_Tags_Group.Load
        If Not IsPostBack Then
            Try
                Shared_Dropdownlist_Bind(DDL_Tags_Group, GetSQL(DDL_Tags_Group, Nothing, Nothing, Nothing), "Tags Group", "Tags Group", "Please select", False)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub DDL_Tags_Group_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Tags_Group.SelectedIndexChanged
        PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
    End Sub

    Protected Sub Add_ESL_Tags_Type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddESLTagsType.Click
        ModalHeaderESLTagsType.Text = "Add New Tags Type"
        btnSaveESLTagsType.Text = "Save"
        btnCancelESLTagsType.Text = "Cancel"

        '' Initialiaze fiels
        TB_Tags_Group.Text = String.Empty
        TB_Tags_Type.Text = String.Empty
        esltagstypeexisterrormsg.Visible = False

        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_ESL_Tags_Type_Row_Index, TB_Selected_ESL_Tags_ID, TB_Selected_ESL_Tags_Type_Tags_Group, TB_Selected_ESL_Tags_Type_Tags_Type}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        popupESLTagsType.Show()
    End Sub

    Protected Sub Edit_ESL_Tags_Type_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderESLTagsType.Text = "Update Tags Type"
        btnSaveESLTagsType.Text = "Update"
        btnCancelESLTagsType.Text = "Cancel"

        '' Get row command argument
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_ESL_Tags_Type_Row_Index, TB_Selected_ESL_Tags_ID, TB_Selected_ESL_Tags_Type_Tags_Group, TB_Selected_ESL_Tags_Type_Tags_Type}

        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).Text = EditLinkButtonCommandArgument(i)
        Next

        '' Populate the fields
        TB_Tags_Group.Text = TB_Selected_ESL_Tags_Type_Tags_Group.Text
        TB_Tags_Type.Text = TB_Selected_ESL_Tags_Type_Tags_Type.Text

        esltagstypeexisterrormsg.Visible = False

        popupESLTagsType.Show()
    End Sub

    Protected Sub Save_ESL_Tags_Type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveESLTagsType.Click
        Dim ID As TextBox = pnlAddEditESLTagsType.FindControl("TB_Selected_ESL_Tags_ID")
        Dim Tags_Group As TextBox = pnlAddEditESLTagsType.FindControl("TB_Tags_Group")
        Dim Tags_Type As TextBox = pnlAddEditESLTagsType.FindControl("TB_Tags_Type")

        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_ESL_Tags_Type N'" & ID.Text & "', N'" & EscapeChar(Trim(Tags_Group.Text)) & "', '" & EscapeChar(Trim(Tags_Type.Text)) & "' "
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_ESL_Tags_Type WHERE [Tags Group] = '" & EscapeChar(Trim(Tags_Group.Text)) & "' AND [Tags Type] = N'" & EscapeChar(Trim(Tags_Type.Text)) & "' ", "NoOfRecord")) > 0, True, False)
        Dim Existing_Unique_ID As String = Get_Value("SELECT [ID] FROM R_Maintenance_ESL_Tags_Type WHERE [Tags Group] = '" & EscapeChar(Trim(Tags_Group.Text)) & "' AND [Tags Type] = N'" & EscapeChar(Trim(Tags_Type.Text)) & "' ", "ID")

        Try
            If btnSaveESLTagsType.Text = "Save" Then
                If Not RecordExists Then
                    Validate_Name(esltagstypeexisterrormsg, popupESLTagsType, TB_Tags_Type, sqlStr, True)
                Else
                    Validate_Name(esltagstypeexisterrormsg, popupESLTagsType, TB_Tags_Type, sqlStr, False)
                End If
            ElseIf btnSaveESLTagsType.Text = "Update" Then
                If Not RecordExists Then
                    Validate_Name(esltagstypeexisterrormsg, popupESLTagsType, TB_Tags_Type, sqlStr, True)
                Else
                    If ID.Text = Existing_Unique_ID Then  '' Check if is editing the same record
                        Validate_Name(esltagstypeexisterrormsg, popupESLTagsType, TB_Tags_Type, sqlStr, True)
                    Else
                        Validate_Name(esltagstypeexisterrormsg, popupESLTagsType, TB_Tags_Type, sqlStr, False)
                    End If
                End If
            End If
        Catch ex As Exception
            Response.Write("Error:   " & ex.Message)
        End Try

        '' Populate gridview
        PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
        PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)
    End Sub

    Protected Sub Delete_ESL_Tags_Type_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim DeleteLinkButtonCommandArgument As Array = Split(DeleteLinkButton.CommandArgument, "|")

        Try
            Dim sqlStr As String = "DELETE FROM Maintenance_ESL_Tags_Type WHERE ID = '" & DeleteLinkButtonCommandArgument(1) & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        '' Re-populate gridview
        PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
    End Sub





    '' Modal control - Tags Deployment
    Protected Sub DDL_Tags_Group_Deployment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Tags_Group_Deployment.SelectedIndexChanged
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = DDL_Tags_Group_Deployment.SelectedValue
        Shared_Dropdownlist_Bind(DDL_Tags_Type_Deployment, GetSQL(DDL_Tags_Type_Deployment, Services_Group, Nothing, filter), "Tags Type", "Tags Type", "Please select", True)
        DDL_Tags_Type_Deployment.Enabled = IIf(DDL_Tags_Group_Deployment.SelectedValue <> "-1", True, False)
        TB_Tags_Deployment_Quantity.Text = String.Empty

        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub DDL_Tags_Type_Deployment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDL_Tags_Type_Deployment.SelectedIndexChanged
        TB_Tags_Deployment_Quantity.Text = String.Empty
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub Add_Tags_Deployment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddTagsDeployment.Click
        ModalHeaderESLTagsDeployment.Text = "Add New Tags Deployment"
        btnSaveESLTagsDeployment.Text = "Save"
        btnCancelESLTagsDeployment.Text = "Cancel"
        AddTagsDeploymentLineItems.Text = "Add"

        '' Initialize fields
        DDL_Tags_Deployment_Location.SelectedIndex = 0
        DDL_Tags_Deployment_Location.Enabled = True
        TB_Tags_Install_Date.Text = String.Empty
        TB_Tags_Install_Date.Enabled = True
        DDL_Tags_Group_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.Enabled = False
        TB_Tags_Deployment_Quantity.Text = String.Empty
        tagdeploymentlistboxerrormsg.Visible = False

        '' Bind dropdwnlist
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Tags_Deployment_Location, GetSQL(DDL_Tags_Deployment_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Tags_Group_Deployment, GetSQL(DDL_Tags_Group_Deployment, Services_Group, Nothing, Nothing), "Tags Group", "Tags Group", "Please select", True)

        '' Disable Tags type dropdownlis then Tags group dropdownlist not selected
        DDL_Tags_Type_Deployment.Enabled = IIf(DDL_Tags_Group_Deployment.SelectedValue <> "-1", True, False)


        '' Initialiaze the hidden fields
        Dim HiddenFields As Array = {TB_Selected_Tags_Deployment_Row_Index, TB_Selected_Tags_Deployment_Unique_ID, TB_Selected_Tags_Deployment_Customer_ID, TB_Selected_Tags_Deployment_Store_ID, TB_Selected_Tags_Deployment_Installation_Date}
        For i = 0 To HiddenFields.Length - 1
            HiddenFields(i).Text = Nothing
        Next

        '' Populate listbox in new mode
        DeleteStaging()   '' clear the staging when adding new
        PopulateTagsDeploymentListbox()
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub Edit_Tags_Deployment_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderESLTagsDeployment.Text = "Update Tags Deployment"
        btnSaveESLTagsDeployment.Text = "Update"
        btnCancelESLTagsDeployment.Text = "Cancel"
        AddTagsDeploymentLineItems.Text = "Add"

        '' Bind dropdwnlist
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim filter As String = "Status"
        Shared_Dropdownlist_Bind(DDL_Tags_Deployment_Location, GetSQL(DDL_Tags_Deployment_Location, Services_Group, Nothing, filter), "Store Name", "Store ID", "Please select", True)
        Shared_Dropdownlist_Bind(DDL_Tags_Group_Deployment, GetSQL(DDL_Tags_Group_Deployment, Services_Group, Nothing, Nothing), "Tags Group", "Tags Group", "Please select", True)


        '' Get row command argument, get the value and pass them to hidden fields
        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim EditLinkButtonCommandArgument As Array = Split(EditLinkButton.CommandArgument, "|")
        Dim HiddenFields As Array = {TB_Selected_Tags_Deployment_Row_Index, TB_Selected_Tags_Deployment_Unique_ID, TB_Selected_Tags_Deployment_Customer_ID, TB_Selected_Tags_Deployment_Store_ID, TB_Selected_Tags_Deployment_Installation_Date}

        '' Loop through to assign value to hidden fields
        For i = 0 To EditLinkButtonCommandArgument.Length - 1
            HiddenFields(i).text = EditLinkButtonCommandArgument(i)
        Next

        ' Re-populate the fields in edit mode
        DDL_Tags_Deployment_Location.SelectedIndex = DDL_Tags_Deployment_Location.Items.IndexOf(DDL_Tags_Deployment_Location.Items.FindByValue(TB_Selected_Tags_Deployment_Store_ID.Text))
        DDL_Tags_Deployment_Location.Enabled = False
        TB_Tags_Install_Date.Text = CDate(TB_Selected_Tags_Deployment_Installation_Date.Text).ToString("yyyy-MM-dd")
        TB_Tags_Install_Date.Enabled = True

        DDL_Tags_Group_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.Enabled = True
        TB_Tags_Deployment_Quantity.Text = String.Empty
        tagdeploymentlistboxerrormsg.Visible = False

        '' clear staging table right before the edit
        DeleteStaging()

        '' Get record from existing table and insert to staging table
        Try
            Dim sqlStr = "INSERT INTO Maintenance_ESL_Tags_Deployment_Staging(Customer_ID, Store_ID, Tags_Group, Tags_Type, Quantity) " &
                         "SELECT A.Customer_ID, A.Store_ID, B.Tags_Group, B.Tags_Type, B.Quantity FROM Maintenance_ESL_Tags A " &
                         "LEFT JOIN Maintenance_ESL_Tags_Deployment B On B.Unique_ID = A.Unique_ID " &
                         "WHERE A.Unique_ID = '" & TB_Selected_Tags_Deployment_Unique_ID.Text & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message & "\nEdit_Tags_Deployment_Click")
        End Try

        '' Populate listbox in edit mode
        PopulateTagsDeploymentListbox()
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub Save_Tags_Deployment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveESLTagsDeployment.Click
        Dim Unique_ID As TextBox = pnlAddEditESLTagsDeployment.FindControl("TB_Selected_Tags_Deployment_Unique_ID")
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Store_ID As DropDownList = pnlAddEditESLTagsDeployment.FindControl("DDL_Tags_Deployment_Location")
        Dim Installation_Date As TextBox = pnlAddEditESLTagsDeployment.FindControl("TB_Tags_Install_Date")

        Dim GridView_Tags_Deployment_List As GridView = pnlAddEditESLTagsDeployment.FindControl("GridView_Tags_Deployment_List")
        Dim UploadedRecordCount As Integer = GridView_Tags_Deployment_List.Rows.Count

        If UploadedRecordCount > 0 Then
            Try
                Dim sqlStr As String = " EXEC SP_CRUD_Maintenance_ESL_Tags_Deployment N'" & Unique_ID.Text &
                                                                                  "', N'" & Customer_ID &
                                                                                  "', N'" & Store_ID.SelectedValue &
                                                                                  "', N'" & Installation_Date.Text & "' "
                RunSQL(sqlStr)
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        Else
            tagdeploymentlistboxerrormsg.Visible = True
            popupESLTagsDeployment.Show()
        End If

        DeleteStaging()
        PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
        PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)
    End Sub

    Protected Sub Delete_Tags_Deployment_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim DeleteLinkButton As LinkButton = TryCast(sender, LinkButton)
        Dim Unique_ID As String = DeleteLinkButton.CommandArgument

        Try
            Dim sqlStr = "BEGIN TRANSACTION; " &
                         "DELETE FROM Maintenance_ESL_Tags_Deployment " &
                         "WHERE Unique_ID In ( Select Unique_ID FROM Maintenance_ESL_Tags WHERE Unique_ID = '" & Unique_ID & "'); " &
                         "DELETE FROM Maintenance_ESL_Tags WHERE Unique_ID = '" & Unique_ID & "'; " &
                         "COMMIT;"

            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        PopulateGridViewDataTagsType(DDL_Tags_Group.SelectedValue)
        PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)
    End Sub

    Protected Sub Cancel_Tags_Deployment_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelESLTagsDeployment.Click
        DeleteStaging()
    End Sub

    Protected Sub RegEx_TB_Tags_Deployment_Quantity_ServerValidate(source As Object, args As ServerValidateEventArgs)
        Dim quantity As Integer
        If Integer.TryParse(TB_Tags_Deployment_Quantity.Text, quantity) Then
            args.IsValid = (quantity <> 0)
        Else
            args.IsValid = False
        End If
    End Sub



    Protected Sub AddTagsDeploymentLineItems_Click(sender As Object, e As EventArgs) Handles AddTagsDeploymentLineItems.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Store_ID As DropDownList = pnlAddEditESLTagsDeployment.FindControl("DDL_Tags_Deployment_Location")
        Dim Tags_Group As DropDownList = pnlAddEditESLTagsDeployment.FindControl("DDL_Tags_Group_Deployment")
        Dim Tags_Type As DropDownList = pnlAddEditESLTagsDeployment.FindControl("DDL_Tags_Type_Deployment")
        Dim Quantity As TextBox = pnlAddEditESLTagsDeployment.FindControl("TB_Tags_Deployment_Quantity")

        Try
            Dim sqlStr = " EXEC SP_Insert_Tags_Deployment_Staging N'" & Customer_ID &
                                                                  "', N'" & Store_ID.SelectedValue &
                                                                  "', N'" & Tags_Group.SelectedValue &
                                                                  "', N'" & Tags_Type.SelectedValue &
                                                                  "', N'" & Quantity.Text & "' "
            RunSQL(sqlStr)

            '' Reset field when line item is added
            DDL_Tags_Group_Deployment.SelectedIndex = 0
            DDL_Tags_Type_Deployment.SelectedIndex = 0
            DDL_Tags_Type_Deployment.Enabled = False
            TB_Tags_Deployment_Quantity.Text = String.Empty
            AddTagsDeploymentLineItems.Text = "Add"

        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try

        tagdeploymentlistboxerrormsg.Visible = False

        PopulateTagsDeploymentListbox()
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub ClearTagsDeploymentLineItems_Click(sender As Object, e As EventArgs) Handles btnClearTagsDeploymentLineItems.Click
        '' Reset tags related field
        DDL_Tags_Group_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.SelectedIndex = 0
        DDL_Tags_Type_Deployment.Enabled = IIf(DDL_Tags_Type_Deployment.SelectedIndex <= 0, False, True)
        TB_Tags_Deployment_Quantity.Text = String.Empty

        DeleteStaging()
        tagdeploymentlistboxerrormsg.Visible = False
        PopulateTagsDeploymentListbox()
        popupESLTagsDeployment.Show()
    End Sub

    Protected Sub PopulateTagsDeploymentListbox()
        Dim Store_ID As DropDownList = pnlAddEditESLTagsDeployment.FindControl("DDL_Tags_Deployment_Location")
        Try
            GridView_Tags_Deployment_List.DataSource = GetDataTable(GetSQL(GridView_Tags_Deployment_List, Nothing, Nothing, Store_ID.SelectedValue))
            GridView_Tags_Deployment_List.DataBind()
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub

    Protected Sub DeleteLineItem_Click(sender As Object, e As EventArgs)
        Dim row As GridViewRow = CType(CType(sender, LinkButton).Parent.Parent, GridViewRow)
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Tags_Group = HttpUtility.HtmlDecode(row.Cells(0).Text)
        Dim Tags_Type = HttpUtility.HtmlDecode(row.Cells(1).Text)
        Try
            Dim sqlStr As String = " DELETE FROM Maintenance_ESL_Tags_Deployment_Staging " &
                                   " WHERE Tags_Group = '" & Trim(Tags_Group) & "' " &
                                   "   AND Tags_Type = '" & Trim(Tags_Type) & "' " &
                                   "   AND Customer_ID ='" & Customer_ID & "' "
            RunSQL(sqlStr)
        Catch ex As Exception
            ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('Record fail to delete.');", True)
        End Try

        PopulateTagsDeploymentListbox()
        popupESLTagsDeployment.Show()
    End Sub






    '' Common procedure for Product value with decimal rounding
    Protected Sub RegEx_TB_Product_Value_ServerValidate(source As Object, args As ServerValidateEventArgs)
        Dim productValue As String = args.Value
        If productValue.StartsWith("$") Then
            productValue = productValue.Substring(1) ' Remove the leading dollar sign
        End If

        ' Parse the value as decimal and round it to 2 decimal places
        Dim value As Decimal
        If Decimal.TryParse(productValue, value) Then
            value = Math.Round(value, 2)
            args.IsValid = True
        Else
            args.IsValid = False
        End If
    End Sub

    Protected Sub Validate_Name(ByVal errorMsg As Object, ByVal popupModal As Object, ByVal textboxField As Object, ByVal sqlStr As String, ByVal isPassedValidate As Boolean)
        If isPassedValidate Then
            errorMsg.Visible = False
            RunSQL(sqlStr)
        Else
            errorMsg.Visible = True
            errorMsg.Style.Add("display", "inline")
            popupModal.Show()
            textboxField.Focus()
        End If
    End Sub

    Protected Function GetSQL(ByVal controlObj As Object, Optional ByVal Services_Group As String = Nothing, Optional ByVal keyword As String = Nothing, Optional ByVal filter As String = Nothing) As String
        Dim sqlStr As String = Nothing
        Dim objID As String = String.Empty
        Dim Search_Keyword As String = EscapeChar(keyword)

        ' Check if the object has an ID property or field
        Dim idProperty = controlObj.GetType().GetProperty("ID")
        If idProperty IsNot Nothing AndAlso idProperty.PropertyType Is GetType(String) Then
            objID = idProperty.GetValue(controlObj)?.ToString()
        End If

        ' Form SQL String based on control object ID
        Select Case objID
            Case "FormView1"
                sqlStr = "SELECT * FROM R_Maintenance_Customer " &
                         "WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                         "ORDER BY [Name] "

            Case "GridView1"
                sqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY [Banner Name] ASC) AS No, * FROM R_Maintenance_Banner " &
                         "WHERE [Customer ID] ='" & Request.QueryString("Customer_ID") & "' "

            Case "GridView2"
                sqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY [Banner Name] ASC) AS No, * " &
                         "FROM R_Maintenance_Store " &
                         "WHERE [Customer ID] ='" & Request.QueryString("Customer_ID") & "' " &
                         "  AND ([Store Name] LIKE '%" & keyword & "%' OR [Banner Name] LIKE '%" & keyword & "%') " &
                         "ORDER BY [Banner Name], [Store Name] "

            Case "GridView3"
                sqlStr = "SELECT * FROM R_Maintenance_Product_Type " &
                         "WHERE [Services Group] ='" & Services_Group & "' AND ([Code] LIKE '%" & keyword & "%' OR [Product Name] LIKE '%" & keyword & "%') " &
                         "ORDER BY [Code] "

            Case "GridView4"
                sqlStr = "SELECT * FROM R_Maintenance_Product " &
                         "WHERE [Customer ID] ='" & Request.QueryString("Customer_ID") & "' " &
                         "  AND ([Serial No] LIKE '%" & keyword & "%' OR [Product Name] LIKE '%" & keyword & "%' OR [Banner] LIKE '%" & keyword & "%' OR [Location] LIKE '%" & keyword & "%') " &
                         "ORDER BY [Serial No] "

            Case "GridView5"
                sqlStr = "SELECT * FROM R_Maintenance_Product " &
                         "WHERE [Customer ID] ='" & Request.QueryString("Customer_ID") & "' " &
                         "  AND ([Serial No] LIKE '%" & keyword & "%' OR [Product Name] LIKE '%" & keyword & "%' OR [Banner] LIKE '%" & keyword & "%' OR [Location] LIKE '%" & keyword & "%') " &
                         "ORDER BY [Serial No] "

            Case "GridView6"
                sqlStr = "SELECT * FROM R_Maintenance_ESL_Tags_Type WHERE [Tags Group] LIKE '%" & keyword & "%' ORDER BY [Tags Group], [Tags Type] "

            Case "GridView7"
                sqlStr = "EXEC SP_ESL_Tags_Deployment '" & Request.QueryString("Customer_ID") & "', '" & keyword & "' "

            Case "GridView8"
                sqlStr = "SELECT ROW_NUMBER() OVER(ORDER BY [Store Name] ASC) AS No, * FROM R_Maintenance_Contract " &
                         "WHERE [Customer ID] ='" & Request.QueryString("Customer_ID") & "' AND [FY] LIKE '%" & keyword & "%' " &
                         "ORDER BY [FY] DESC, [Store Name] "

            Case "GridView9"
                sqlStr = "SELECT * FROM R_Maintenance_Contract_Status_Log WHERE Unique_ID = '" & keyword & "' ORDER BY Created_Date DESC, ID DESC "

            Case "GridView_Tags_Deployment_List"
                sqlStr = "Select * FROM Maintenance_ESL_Tags_Deployment_Staging " &
                         " WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "'" &
                         "   AND Store_ID = '" & filter & "' "

            Case "DDL_Banner"
                sqlStr = "SELECT [Banner ID], [Banner Name] FROM R_Maintenance_Banner " &
                         "WHERE [Services Group] = '" & Services_Group & "' " &
                         "  AND [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                         "ORDER BY [Banner Name] "

            Case "DDL_Product_Category"
                sqlStr = "SELECT Value_1 AS Category FROM DB_Lookup " &
                         "WHERE Lookup_Name = 'Retail Product Category' AND Value_2 = '" & Services_Group & "' "

            Case "DDL_Product", "DDL_Retail_Product"
                sqlStr = "SELECT [Code], [Product Name] FROM R_Maintenance_Product_Type " &
                         "WHERE [Services Group] = '" & Services_Group & "' ORDER BY [Product Name] "

            Case "DDL_Location", "DDL_Retail_Location", "DDL_Tags_Deployment_Location"
                sqlStr = "SELECT [Store ID], [Banner Name] + ' ' + [Store Name] AS [Store Name] FROM R_Maintenance_Store " &
                         "WHERE [Services Group] = '" & Services_Group & "' " &
                         "  AND [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " & IIf(Not filter Is Nothing And filter = "Status", "AND [Status] = 'Active' ", Nothing) & "ORDER BY [Banner Name], [Store Name] "

            Case "DDL_Warranty_Cover", "DDL_Retail_Product_Warranty_Cover"
                sqlStr = "SELECT Value_1, Value_2 FROM DB_Lookup " &
                         "WHERE Lookup_Name = 'Warranty Period' "

            Case "DDL_Value_Currency", "DDL_Retail_Product_Value_Currency", "DDL_Contract_Currency"
                sqlStr = "SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup " &
                         "WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR') "

            Case "DDL_Contract_Process_Status"
                sqlStr = "SELECT Value_1 AS Status_Description, Value_2 AS Status_Code FROM DB_Lookup WHERE Lookup_Name = 'Contract Process Status' "

            Case "DDL_Financial_Year"
                sqlStr = "SELECT [Year] AS [FY] FROM ( " &
                         "   SELECT [Year] FROM dbo.Get_Financial_Year_List() " &
                         "   UNION " &
                         "   SELECT [FY] FROM R_Maintenance_Contract WHERE [Services Group] = '" & Services_Group & "' AND [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " &
                         "   ) TBL " &
                         "ORDER BY TBL.[Year] DESC "

            Case "DDL_Contract_Store"
                sqlStr = "SELECT [Store ID], [Banner Name] + ' - ' + [Store Name] AS [Store Name] FROM R_Maintenance_Store " &
                         "WHERE [Services Group] = '" & Services_Group & "' " &
                         "  AND [Customer ID] = '" & Request.QueryString("Customer_ID") & "' " & IIf(Not filter Is Nothing And filter = "Store ID", "AND [Status] = 'Active' ", Nothing) & IIf(Not filter Is Nothing And filter = "Store ID", "AND [Store ID] NOT IN (SELECT [Store ID] FROM R_Maintenance_Contract WHERE [Customer ID] = '" & Request.QueryString("Customer_ID") & "' AND [FY] = '" & DDL_Financial_Year.SelectedValue & "') ", Nothing) & "ORDER BY [Store Name] "

            Case "DDL_Tags_Group", "DDL_Tags_Group_Deployment"
                sqlStr = "SELECT DISTINCT [Tags Group] FROM R_Maintenance_ESL_Tags_Type ORDER BY [Tags Group] "

            Case "DDL_Tags_Type_Deployment"
                sqlStr = "SELECT Distinct [Tags Type] FROM R_Maintenance_ESL_Tags_Type WHERE [Tags Group] = '" & filter & "' "

        End Select

        Return sqlStr
    End Function

    Protected Sub DeleteStaging()
        Try
            '' In module licence page, it is using 2 staging table, reset both table after used
            Dim TableToReset() As String = {"Maintenance_ESL_Tags_Deployment_Staging"}
            For i = 0 To TableToReset.Length - 1
                RunSQL("EXEC SP_Reset_Staging_Table '" & TableToReset(i) & "'")
            Next
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub




    '' Searchbox
    Protected Sub BT_Search_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Store.Click
        PopulateGridViewDataStore(TB_Search_Store.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub BT_Search_Product_Type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Product_Type.Click
        PopulateGridViewDataProductType(TB_Search_Product_Type.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub BT_Search_Product_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Product.Click
        PopulateGridViewDataProduct(TB_Search_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub BT_Search_Retail_Product_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Retail_Product.Click
        PopulateGridViewDataRetailProduct(TB_Search_Retail_Product.Text, ViewState("currentSortingGridview"), ViewState("SortExpression") & " " & ViewState("SortDirection"))
    End Sub

    Protected Sub BT_Search_Tags_Store_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search_Tags_Store.Click
        PopulateGridViewDataTagsDeployment(TB_Search_Tags_Store.Text)
    End Sub




    '' Command buttons
    Protected Sub BT_Create_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Create.Click
        Dim Customer_ID As String = Request.QueryString("Customer_ID")
        Dim Name As TextBox = FormView1.FindControl("TB_Name")
        Dim Address As TextBox = FormView1.FindControl("TB_Address")
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim Contact_Person As TextBox = FormView1.FindControl("TB_Contact_Person")
        Dim Phone As TextBox = FormView1.FindControl("TB_Phone")
        Dim Email As TextBox = FormView1.FindControl("TB_Email")
        Dim Status As DropDownList = FormView1.FindControl("DDL_Status")
        Dim BtnCommand As LinkButton = TryCast(sender, LinkButton)

        Dim Hidden_Old_Name As Label = FormView1.FindControl("LB_Hidden_Name")   '' store value of existing customer name for comparison

        '' Define the sqlstr
        Dim sqlStr As String = "EXEC SP_CRUD_Maintenance_Customer N'" & Customer_ID & "' " &
                                                               ", N'" & EscapeChar(Trim(Name.Text)) & "' " &
                                                               ", N'" & EscapeChar(Address.Text) & "' " &
                                                               ", N'" & Services_Group & "' " &
                                                               ", N'" & EscapeChar(Contact_Person.Text) & "' " &
                                                               ", N'" & EscapeChar(Phone.Text) & "' " &
                                                               ", N'" & EscapeChar(Email.Text) & "' " &
                                                               ", N'" & EscapeChar(Status.SelectedValue) & "' " &
                                                               ", N'" & BtnCommand.CommandName & "' "

        '' Check database if the Client ID exists
        Dim RecordExists As Boolean = IIf(CInt(Get_Value("SELECT COUNT(*) AS NoOfRecord FROM R_Maintenance_Customer WHERE [Services Group] = '" & Services_Group & "' AND [Name] ='" & EscapeChar(Name.Text) & "'", "NoOfRecord")) > 0, True, False)

        If FormView1.CurrentMode = FormViewMode.Insert Then
            If Not RecordExists Then
                Try
                    RunSQL(sqlStr)
                    Response.Write(AlertMessageRedirect("Customer " & Replace(EscapeChar(Name.Text), "''", "\'") & "\'s profile successfully " & BtnCommand.Text.ToLower & "d.", "/Services/Maintenance_Service_Records.aspx"))
                Catch ex As Exception
                    Response.Write("ERROR: " & ex.Message)
                End Try
            Else
                Response.Write(AlertMessageRedirect("Customer " & Replace(EscapeChar(Name.Text), "''", "\'") & " exists.\nYou\'re not allowed to create profile with the same name. ", "/Services/Maintenance_Service_Records.aspx"))
            End If
        ElseIf FormView1.CurrentMode = FormViewMode.Edit Then
            If Not RecordExists Or Trim(Name.Text) = Trim(Hidden_Old_Name.Text) Then
                Try
                    RunSQL(sqlStr)
                    Response.Write(AlertMessageRedirect("Customer " & Replace(EscapeChar(Name.Text), "''", "\'") & "\'s profile successfully " & BtnCommand.Text.ToLower & "d.", "/Services/Maintenance_Service_Records.aspx"))
                Catch ex As Exception
                    Response.Write("ERROR: " & ex.Message)
                End Try
            Else
                Response.Write(AlertMessageRedirect("Customer name " & Replace(EscapeChar(Name.Text), "''", "\'") & " exists.\nYou\'re not allowed to create profile with the same name. ", "/Services/Maintenance_Service_Records_Details.aspx?Customer_ID=" & Customer_ID))
                Name.Focus()
            End If
        End If
    End Sub

    Protected Sub BT_Cancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Cancel.Click
        Response.Redirect("~/Services/Maintenance_Service_Records.aspx")
    End Sub



End Class




