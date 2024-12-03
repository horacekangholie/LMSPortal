
Partial Class Views_Hardkey_Details
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
            Dim sqlStr() As String = {"SELECT  Customer_ID, [PO No], [PO Date], [SO No], [SO Date], [Invoice No], [Invoice Date], [Prepared By] " &
                                      "FROM R_Hardkey_Licence WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "' " &
                                      "GROUP BY Customer_ID, [PO No], [PO Date], [SO No], [SO Date], [Invoice No], [Invoice Date], [Prepared By] "}

            BuildGridView(GridView1, "GridView1", "Customer_ID")
            GridView1.DataSource = GetDataTable(sqlStr(0))
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try
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
                GridViewObj.AllowPaging = False
                GridViewObj.Columns.Clear()
                Dim ColData() As String = {"PO No", "PO Date", "SO No", "SO Date", "Invoice No", "Invoice Date", "Prepared By"}
                Dim ColSize() As Integer = {100, 50, 100, 50, 100, 50, 200}

                '' add template field for the nested gridview
                Dim Expandfield As TemplateField = New TemplateField()
                Expandfield.ItemTemplate = New HardkeyNestedGridViewItemTemplate()
                Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                GridView1.Columns.Add(Expandfield)

                For i = 0 To ColData.Length - 1
                    Dim Bfield As BoundField = New BoundField()
                    Bfield.DataField = ColData(i)
                    Bfield.HeaderText = Replace(ColData(i), "_", " ")
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
            '' Expand for nested Gridview
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO Date")).Text
            Dim HarkeyLicence As GridView = TryCast(e.Row.FindControl("gvHardkeyLicence"), GridView)

            Dim query As String = " SELECT * FROM I_LMS_Hardkey_Licence " &
                                  " WHERE Customer_ID = '" & Customer_ID & "'  AND PO_No = '" & PO_No & "' AND PO_Date = '" & CDate(PO_Date).ToString("yyyy-MM-dd") & "' ORDER BY Licence_No "
            Try
                HarkeyLicence.DataSource = GetDataTable(query)
                HarkeyLicence.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"
            InvoiceDownloadLink.Text = drv("Invoice No")
            If Len(drv("Invoice No").ToString) > 0 Then
                e.Row.Cells(5).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

        End If
    End Sub


    '' Bottom control button
    Protected Sub BT_Close_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Close.Click
        Response.Redirect("~/Views/Hardkey_Licence_List.aspx")
    End Sub

End Class
