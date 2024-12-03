Imports System.Data

Partial Class _Default
    Inherits LMSPortalBaseCode

    Dim TotalHQs, TotalStores, TotalDemo, TotalBilled, TotalTrial, TotalSuspended, TotalClosed, TotalAndroid, TotaliOS, TotalWeb, TotalSM As Integer
    Dim TotalHardkeyDemo, TotalTier1, TotalTier2, TotalTier3, TotalTier4, TotalTier5, TotalCMSDemo, TotalCMS, TotalHardkey As Integer
    Dim TotalCurrYear, TotalPrevYear, TotalYearBef, Total3YearsAgo, Total4YearsAgo As Decimal


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not IsPostBack Then
            BindGridViewReminder()
        End If

        If Session("User_Roles") IsNot Nothing Then
            Select Case Session("User_Roles")
                Case "Admin"
                    'BindGridView()
                    'BindGridView1()
                    ReminderDashboard.Visible = (Session("User_Name") = "Administrator")
                    AdminDashboard.Visible = True
                    CommonDashboard.Visible = True
                    MaintenanceDashboard.Visible = False
                Case "CtrAdmin"
                    BindGridView2()
                    ReminderDashboard.Visible = False
                    AdminDashboard.Visible = False
                    CommonDashboard.Visible = False
                    MaintenanceDashboard.Visible = True
                Case Else
                    BindGridView1()
                    ReminderDashboard.Visible = False
                    AdminDashboard.Visible = False
                    CommonDashboard.Visible = True
                    MaintenanceDashboard.Visible = False
            End Select
        Else
            Response.Redirect("~/Login.aspx")
        End If
    End Sub


    '' Gridview Binder
    Protected Sub BindGridViewReminder()
        Try
            Dim sqlStr As String = "SELECT * FROM DB_Reminder WHERE Is_Done = 0 ORDER BY ID DESC "

            ''Response.Write(sqlStr)
            BuildGridViewReminder()
            GridViewReminder.DataSource = GetDataTable(sqlStr)
            GridViewReminder.DataBind()

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    Protected Sub BindGridView()
        Dim ControlObject() As Object = {New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView()}
        Dim ControlName() As String = {"HQStoreSummary", "DMCOutstandingInvoice", "HardkeySummary", "HarkeyOrderOutstandingInvoice", "AppLicenceSummry", "LicenceCodeOutstandingInvoice", "ModuleLicenceOutstandingInvoice", "TermedLicenceRenewal", "BilledItemSummary"}
        Dim TabName() As String = {"DMC Accounts", "DMC Subscription (Open)", "Hardkey Summary", "Harkey Order (Open)", "Product Licence Stats", "Licence Order (Open)", "Module Licence Order (Open)", "Termed Licence Renewal (Open)", "Billed Items Summary"}
        Dim sqlStr() As String = {"SELECT [Group Name], [HQ], [Store], [Demo], [Billed], [Trial], [Suspended], [Closed Store], [Closed Demo], [Closed Billed], [Closed Trial], REPLACE([Path], '~', '') AS [Path] FROM D_DMC_Summary A INNER JOIN DB_Access_Map B ON B.Sub_Module = A.[Group Name] ORDER BY CASE [Group Name] WHEN 'Singapore Outlets' THEN 1 WHEN 'Distributors' THEN 2 WHEN 'Overseas Subsidiaries' THEN 3 WHEN 'Overseas Customers' THEN 4 ELSE 5 END ",
                                  "SELECT * FROM D_DMC_Subscription_Outstanding_Invoice ORDER BY [Subscription ID] DESC ",
                                  "SELECT * FROM D_Hardkey_Licence_Summary",
                                  "SELECT * FROM D_LMS_Hardkey_Licence_Order_Outstanding_Invoice ORDER BY [Created Date] DESC",
                                  "SELECT * FROM D_LMS_Licence_Summary",
                                  "SELECT * FROM D_LMS_Licence_Order_Outstanding_Invoice ORDER BY [Days since created]",
                                  "SELECT * FROM D_LMS_Module_Licence_Order_Outstanding_Invoice ORDER BY [Created Date] DESC",
                                  "SELECT [UID], [Customer ID], [Customer], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], SUM(Fee) AS [Total Amount], [Renewal Date] FROM R_Termed_Licence_Renewal WHERE [PO No] != 'NA' AND ([Invoice No] = '' OR [Invoice No] IS NULL) GROUP BY [UID], [Customer ID], [Customer], [PO No], [PO Date], [Invoice No], [Invoice Date], [Currency], [Renewal Date] ORDER BY [UID] DESC ",
                                  "EXEC SP_D_Sales_Item_Summary"}
        Dim GridViewDataKeyNames() As String = {"Group Name", "Subscription ID", "Country", "Licence No", "Application Type", "Customer ID", "UID", "UID", "Item Code"}

        '' Tab header section
        Dashboard.Controls.Add(New LiteralControl("<hr style='margin-top: 25px; margin-bottom: 25px' />"))
        Dashboard.Controls.Add(New LiteralControl("<ul class='nav nav-tabs' id='adminTab' role='tablist'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard.Controls.Add(New LiteralControl("<li" & IIf(i = 0, " class='active'", "") & ">"))
            Dashboard.Controls.Add(New LiteralControl("<a data-toggle='tab' href=#" & ControlName(i) & ">" & TabName(i) & "</a></li>"))
        Next
        Dashboard.Controls.Add(New LiteralControl("</ul>"))

        '' Tab content section
        Dashboard.Controls.Add(New LiteralControl("<div class='tab-content' id='adminTabContent'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard.Controls.Add(New LiteralControl("<div id='" & ControlName(i) & "' class='" & IIf(i = 0, "tab-pane active", "tab-pane") & "' role='tabpanel'>"))
            If ControlObject(i).GetType().Name = "GridView" Then
                Dashboard.Controls.Add(ControlObject(i))
                BuildGridview(Me, ControlName(i), sqlStr(i), GridViewDataKeyNames(i))
            End If
            Dashboard.Controls.Add(New LiteralControl("</div>"))
        Next
        Dashboard.Controls.Add(New LiteralControl("</div>"))
    End Sub

    Protected Sub BindGridView1()
        Dim ControlObject() As Object = {New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView(), New GridView()}
        Dim ControlName() As String = {"BilledAccountExpiry", "TrialAccountExpiry", "HardkeyLicenceExpiry", "TermLicenceAppProduct", "TermLicenceAI", "AILicenceRenewStatus", "AILicenceExpiredStatus", "CZLAccountSetupFee", "SuspendedStores"}
        Dim TabName() As String = {"Billed - Expiry", "Trial - Expiry", "Hardkey - Expiry", "Termed Licence (App/Product) - Expiry", "AI Licence (Expiring)", "AI Licence (Renew)", "AI Licence (Expired)", "CZL Account Setup Fee (Open)", "Suspended Stores"}
        Dim sqlStr() As String = {"SELECT * FROM D_DMC_Billed_Account_Expired_In_2_Months ORDER BY [End Date], [Bill entity], [HQ Code], [Store Code]",
                                  "SELECT * FROM D_DMC_Trial_Account_Expired_In_2_Months ORDER BY [End Date], [Customer], [Store Code]",
                                  "SELECT * FROM D_Hardkey_Licence_Expired_In_2_Months",
                                  "SELECT * FROM D_Licence_With_Term WHERE [Expired Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) - 12, 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] NOT IN ('PC Scale (AI)') AND Chargeable NOT IN ('No') ORDER BY [Expired Date] DESC ",
                                  "SELECT * FROM D_Licence_With_Term WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] IN ('PC Scale (AI)') AND [Status] NOT IN ('Renew', 'Blocked', 'Expired') AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') ORDER BY [Expired Date], [Serial No] ",
                                  "SELECT * FROM R_LMS_Module_Licence WHERE [status] IN ('Renew') ORDER BY CAST([Expired Date] AS date) DESC ",
                                  "SELECT * FROM D_Licence_With_Term WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] IN ('PC Scale (AI)') AND [Status] IN ('Expired') AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') ORDER BY [Expired Date], [Serial No] ",
                                  "SELECT * FROM D_CZL_Account_Setup_Fee_Outstanding_Invoice ORDER BY [Created Date] DESC ",
                                  "SELECT * FROM R_Suspended_Stores WHERE [Suspended Date] BETWEEN DATEADD(month, DATEDIFF(MONTH, 0, DATEADD(MM, -6, GETDATE())), 0) AND GETDATE() ORDER BY [Suspended Date] DESC, [Headquarter ID] "}
        Dim GridViewDataKeyNames() As String = {"Bill Entity", "Customer", "Customer", "Licence Code", "Licence Code", "Licence Code", "Licence Code", "CZL Account ID", "Headquarter ID"}

        '' Tab header section
        Dashboard1.Controls.Add(New LiteralControl("<hr style='margin-top: 25px; margin-bottom: 25px' />"))
        Dashboard1.Controls.Add(New LiteralControl("<ul class='nav nav-tabs' id='commonTab' role='tablist'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard1.Controls.Add(New LiteralControl("<li" & IIf(i = 0, " class='active'", "") & ">"))
            Dashboard1.Controls.Add(New LiteralControl("<a data-toggle='tab' href=#" & ControlName(i) & ">" & TabName(i) & "</a></li>"))
        Next
        Dashboard1.Controls.Add(New LiteralControl("</ul>"))

        '' Tab content section
        Dashboard1.Controls.Add(New LiteralControl("<div class='tab-content' id='commonTabContent'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard1.Controls.Add(New LiteralControl("<div id='" & ControlName(i) & "' class='" & IIf(i = 0, "tab-pane active", "tab-pane") & "'>"))
            If ControlObject(i).GetType().Name = "GridView" Then
                Dashboard1.Controls.Add(ControlObject(i))
                BuildGridview(Me, ControlName(i), sqlStr(i), GridViewDataKeyNames(i))
            End If
            Dashboard1.Controls.Add(New LiteralControl("</div>"))
        Next
        Dashboard1.Controls.Add(New LiteralControl("</div>"))

    End Sub

    Protected Sub BindGridView2()
        Dim Services_Group As String = Left(Session("User_Group"), 1)
        Dim ControlObject() As Object = {New GridView(), New GridView(), New GridView()}
        Dim ControlName() As String = {"StoreDueForRenewal", "ContractUnderProcessing", "ESLTagsDeploymentOverview"}
        Dim TabName() As String = {"Due For Renewal", "Contract (Processing)", "ESL Tags Overview"}
        Dim sqlStr() As String = {"SELECT A.[Reference No], A.[Customer Name], A.[Store Name], MAX(A.[End Date]) AS [Expiry Date], A.[Unique ID] FROM R_Maintenance_Contract A INNER JOIN R_Maintenance_Contract B ON B.[Store ID] = A.[Store ID] WHERE A.[Services Group] = '" & Services_Group & "' AND A.[End Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 2, 0)) AND A.[Unique ID] = (SELECT TOP 1 [Unique ID] FROM R_Maintenance_Contract WHERE [Store ID] = A.[Store ID] ORDER BY [Unique ID] DESC) GROUP BY A.[Reference No], A.[Customer Name], A.[Store Name], A.[Services Group], A.[Store ID], A.[Unique ID] ORDER BY A.[Customer Name]    ",
                                  "SELECT [Reference No], [Customer Name], [Store Name], [Start Date], [End Date] AS [Expiry Date], [Currency], [Amount], [Status] FROM R_Maintenance_Contract WHERE [Services Group] = '" & Services_Group & "' AND [Status] NOT IN ('Invoiced') ORDER BY [Customer Name], [Start Date] ",
                                  "EXEC SP_ESL_Tags_Deployment_Overview "}

        Dim GridViewDataKeyNames() As String = {"Customer Name", "Reference No", "Customer Name"}

        '' Tab header section
        Dashboard2.Controls.Add(New LiteralControl("<hr style='margin-top: 25px; margin-bottom: 25px' />"))
        Dashboard2.Controls.Add(New LiteralControl("<ul class='nav nav-tabs' id='commonTab' role='tablist'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard2.Controls.Add(New LiteralControl("<li id='tablink" & i + 1 & "' " & IIf(i = 0, " class='active'", "") & ">"))
            Dashboard2.Controls.Add(New LiteralControl("<a data-toggle='tab' href=#" & ControlName(i) & ">" & TabName(i) & "</a></li>"))
        Next
        Dashboard2.Controls.Add(New LiteralControl("</ul>"))

        '' Tab content section
        Dashboard2.Controls.Add(New LiteralControl("<div class='tab-content' id='commonTabContent'>"))
        For i = 0 To ControlObject.Length - 1
            Dashboard2.Controls.Add(New LiteralControl("<div id='" & ControlName(i) & "' class='" & IIf(i = 0, "tab-pane active", "tab-pane") & "'>"))
            If ControlObject(i).GetType().Name = "GridView" Then
                Dashboard2.Controls.Add(ControlObject(i))
                BuildGridview(Me, ControlName(i), sqlStr(i), GridViewDataKeyNames(i))
            End If
            Dashboard2.Controls.Add(New LiteralControl("</div>"))
        Next
        Dashboard2.Controls.Add(New LiteralControl("</div>"))

    End Sub



    '' Format Gridview

    Protected Sub BuildGridViewReminder()
        '' GridView Properties
        GridViewReminder.AutoGenerateColumns = False
        GridViewReminder.AllowPaging = False
        GridViewReminder.PageSize = 15
        GridViewReminder.CellPadding = 4
        GridViewReminder.Font.Size = 10
        GridViewReminder.GridLines = GridLines.None
        GridViewReminder.ShowFooter = False
        GridViewReminder.ShowHeaderWhenEmpty = True
        GridViewReminder.DataKeyNames = New String() {"ID"}
        GridViewReminder.CssClass = "table table-bordered"

        '' Header Style
        GridViewReminder.HeaderStyle.CssClass = "table-warning"
        GridViewReminder.HeaderStyle.Font.Bold = True
        GridViewReminder.HeaderStyle.VerticalAlign = VerticalAlign.Top

        '' Row Style
        GridViewReminder.RowStyle.CssClass = "Default"
        GridViewReminder.RowStyle.VerticalAlign = VerticalAlign.Middle

        '' Footer Style
        GridViewReminder.FooterStyle.CssClass = "table-active"

        '' Pager Style
        GridViewReminder.PagerSettings.Mode = PagerButtons.NumericFirstLast
        GridViewReminder.PagerSettings.FirstPageText = "First"
        GridViewReminder.PagerSettings.LastPageText = "Last"
        GridViewReminder.PagerSettings.PageButtonCount = "10"
        GridViewReminder.PagerStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewReminder.PagerStyle.CssClass = "pagination-ys"

        '' Empty Data Template
        GridViewReminder.EmptyDataText = "No records found."

    End Sub

    Protected Sub BuildGridview(ByVal parent As Control, ByVal controlName As String, ByVal sqlStr As String, ByVal dataKeyNames As String)
        For Each WebControlItem As Control In parent.Controls
            If WebControlItem.GetType().Name = "GridView" AndAlso WebControlItem.ID Is Nothing Then
                Dim GridViewObj As GridView = CType(WebControlItem, GridView)

                '' GridView Properties
                GridViewObj.ID = controlName
                'GridViewObj.AutoGenerateColumns = False
                GridViewObj.AllowPaging = False
                GridViewObj.CellPadding = 4
                GridViewObj.Font.Size = 10
                GridViewObj.GridLines = GridLines.None
                GridViewObj.ShowFooter = True
                GridViewObj.ShowHeaderWhenEmpty = True
                GridViewObj.DataKeyNames = New String() {dataKeyNames}
                GridViewObj.CssClass = "table table-bordered table-hover"

                '' Header Style
                GridViewObj.HeaderStyle.CssClass = "table-primary"
                GridViewObj.HeaderStyle.Font.Bold = True
                GridViewObj.HeaderStyle.VerticalAlign = VerticalAlign.Top

                '' Row Style
                GridViewObj.RowStyle.CssClass = "Default"
                GridViewObj.RowStyle.VerticalAlign = VerticalAlign.Middle

                '' Footer Style
                GridViewObj.FooterStyle.CssClass = "table-active"

                '' Pager Style
                GridViewObj.PagerSettings.Mode = PagerButtons.NumericFirstLast
                GridViewObj.PagerSettings.PageButtonCount = "5"
                GridViewObj.PagerSettings.FirstPageText = "First"
                GridViewObj.PagerSettings.LastPageText = "Last"
                GridViewObj.PagerStyle.HorizontalAlign = HorizontalAlign.Center

                '' Empty Data Template
                GridViewObj.EmptyDataText = "No records found."

                '' Define each Gridview
                Select Case controlName
                    Case "HQStoreSummary"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Group Name", "HQ", "Store", "Demo", "Billed", "Trial", "Suspended", "Closed", "Path"}
                        Dim ColData() As String = {"Group Name", "HQ", "Store", "Demo", "Billed", "Trial", "Suspended", "Closed Store", "Path"}
                        Dim ColSize() As Integer = {300, 80, 120, 80, 80, 120, 80, 80, 100}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        '' Add event handler
                        AddHandler GridViewObj.RowCreated, AddressOf GridView1_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView1_RowDataBound

                    Case "BilledAccountExpiry"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Bill Entity", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "Start Date", "End Date", "Duration", "Currency", "Fee", "Status", "Account Type", "Sales Representative"}
                        Dim ColData() As String = {"Bill Entity", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "Start Date", "End Date", "Duration", "Currency", "Fee", "Status", "Account Type", "Sales Representative"}
                        Dim ColSize() As Integer = {250, 140, 50, 250, 50, 250, 90, 90, 90, 100, 60, 60, 60, 60, 140}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            If Bfield.HeaderText.Contains("Fee") Then
                                Bfield.DataFormatString = "{0:#,##0.00}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next

                        '' Add templatefield for Edit icon
                        'Dim TField As TemplateField = New TemplateField()
                        'TField.HeaderStyle.Width = 0
                        'TField.ItemStyle.Wrap = False
                        'TField.ItemTemplate = New GridViewItemTemplateControl()
                        'GridViewObj.Columns.Add(TField)

                        AddHandler GridViewObj.RowCreated, AddressOf GridView2_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView2_RowDataBound

                    Case "TrialAccountExpiry"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "End Date", "Status", "Account Type", "Sales Representative"}
                        Dim ColData() As String = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "End Date", "Status", "Account Type", "Sales Representative"}
                        Dim ColSize() As Integer = {250, 140, 50, 250, 50, 250, 90, 90, 90, 90, 100}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView3_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView3_RowDataBound

                    Case "DMCOutstandingInvoice"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Subscription ID", "Invoice Bill To", "Headquarter ID", "Headquarter Name", "Currency", "Total Amount", "Invoice No", "Status"}
                        Dim ColData() As String = {"Subscription ID", "Invoice Bill To", "Headquarter ID", "Headquarter Name", "Currency", "Total Amount", "Invoice No", "Status"}
                        Dim ColSize() As Integer = {100, 200, 100, 300, 50, 200, 100, 50}

                        '' Add a templatefield for the nexted gridview
                        Dim Tfield As TemplateField = New TemplateField()
                        Tfield.ItemTemplate = New SubscriptionNestedGridViewItemTemplate()
                        Tfield.HeaderStyle.Width = Unit.Percentage(1)
                        GridViewObj.Columns.Add(Tfield)

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            Bfield.HeaderStyle.Width = ColSize(i)
                            If Bfield.HeaderText.Contains("Amount") Then
                                Bfield.DataFormatString = "{0:#,##0.00}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView4_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView4_RowDataBound

                    Case "HardkeySummary"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Country", "Demo", "Tier 1", "Tier 2", "Tier 3", "Tier 4", "Tier 5", "CMS", "CMS Demo", "Total"}
                        Dim ColData() As String = {"Country", "Demo", "Tier 1", "Tier 2", "Tier 3", "Tier 4", "Tier 5", "CMS", "CMS Demo", "Total"}
                        Dim ColSize() As Integer = {220, 140, 140, 140, 140, 140, 140, 140, 140, 140}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView5_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView5_RowDataBound

                    Case "HardkeyLicenceExpiry"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Customer", "Country", "PO No", "PO Date", "SO No", "SO Date", "Invoice No", "Licence No", "PLU Code", "Description", "Invoice Date", "Prepared By", "Created Date", "Start Date", "End Date", "Requested By"}
                        Dim ColData() As String = {"Customer", "Country", "PO No", "PO Date", "SO No", "SO Date", "Invoice No", "Licence No", "PLU Code", "Description", "Invoice Date", "Prepared By", "Created Date", "Start Date", "End Date", "Requested By"}
                        Dim ColSize() As Integer = {250, 90, 140, 80, 50, 80, 100, 100, 50, 250, 90, 100, 80, 80, 80, 100}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                                Bfield.ItemStyle.Wrap = False
                            ElseIf Bfield.HeaderText.Contains("Licence") Then
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView6_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView6_RowDataBound

                    Case "HarkeyOrderOutstandingInvoice"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "PO No", "PO Date", "Licence No", "Created Date", "PLU Code", "Description", "Prepared By", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColData() As String = {"Licensee", "PO No", "PO Date", "Licence No", "Created Date", "PLU Code", "Description", "Prepared By", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColSize() As Integer = {250, 100, 50, 80, 50, 50, 100, 100, 100, 100, 50}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Wrap = False
                            Bfield.ItemStyle.Wrap = False
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView16_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView16_RowDataBound

                    Case "AppLicenceSummry"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Application Type", "Android", "iOS", "Web", "SM"}
                        Dim ColData() As String = {"Application Type", "Android", "iOS", "Web", "SM"}
                        Dim ColSize() As Integer = {250, 100, 100, 100, 100}
                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            Bfield.ItemStyle.Wrap = False
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView7_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView7_RowDataBound

                    Case "LicenceCodeOutstandingInvoice"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Invoice Bill To", "Licensee", "PO No", "PO Date", "Activated / No of Licence", "Requested By", "Payment Status", "Invoice No", "Days since created"}
                        Dim ColData() As String = {"Invoice Bill To", "Licensee", "PO No", "PO Date", "No of Licence", "Requested By", "Payment Status", "Invoice No", "Days since created"}
                        Dim ColSize() As Integer = {250, 250, 250, 80, 50, 250, 100, 100, 80}

                        '' Add a templatefield for the nexted gridview
                        Dim Tfield As TemplateField = New TemplateField()
                        Tfield.ItemTemplate = New LicenceNestedGridViewItemTemplate()
                        Tfield.HeaderStyle.Width = Unit.Percentage(1)
                        GridViewObj.Columns.Add(Tfield)

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                                Bfield.ItemStyle.Wrap = False
                            ElseIf Bfield.HeaderText.Contains("Licence") Then
                                Bfield.HeaderStyle.Wrap = False
                                Bfield.ItemStyle.Wrap = False
                                Bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView8_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView8_RowDataBound

                    Case "ModuleLicenceOutstandingInvoice"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "PO No", "PO Date", "Created Date", "Chargeable", "e.Sense", "BYOC", "AI", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColData() As String = {"Licensee", "PO No", "PO Date", "Created Date", "Chargeable", "e.Sense", "BYOC", "AI", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColSize() As Integer = {300, 100, 50, 50, 50, 60, 60, 60, 200, 100, 50}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView9_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView9_RowDataBound

                    Case "TermedLicenceRenewal"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"UID", "Customer", "PO No", "PO Date", "Currency", "Total Amount", "Renewal Date", "Invoice No", "Invoice Date"}
                        Dim ColData() As String = {"UID", "Customer", "PO No", "PO Date", "Currency", "Total Amount", "Renewal Date", "Invoice No", "Invoice Date"}
                        Dim ColSize() As Integer = {100, 300, 100, 100, 100, 100, 100, 100, 100}

                        Dim Expandfield As TemplateField = New TemplateField()
                        Expandfield.ItemTemplate = New TermedLicenceRenewalNestedGridViewItemTemplate()
                        Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                        GridViewObj.Columns.Add(Expandfield)

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            If Bfield.HeaderText.Contains("Amount") Then
                                Bfield.DataFormatString = "{0:#,##0.00}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView18_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView18_RowDataBound

                    'Case "AILicenceRenewal"
                    '    GridViewObj.AutoGenerateColumns = False
                    '    GridViewObj.Columns.Clear()
                    '    Dim ColName() As String = {"UID", "Customer", "PO No", "PO Date", "Currency", "Total Amount", "Renewal Date", "Invoice No", "Invoice Date"}
                    '    Dim ColData() As String = {"UID", "Customer", "PO No", "PO Date", "Currency", "Total Amount", "Renewal Date", "Invoice No", "Invoice Date"}
                    '    Dim ColSize() As Integer = {100, 300, 100, 100, 100, 100, 100, 100, 100}

                    '    Dim Expandfield As TemplateField = New TemplateField()
                    '    Expandfield.ItemTemplate = New AIRenewalNestedGridViewItemTemplate()
                    '    Expandfield.HeaderStyle.Width = Unit.Percentage(1)
                    '    GridViewObj.Columns.Add(Expandfield)

                    '    For i = 0 To ColName.Length - 1
                    '        Dim Bfield As BoundField = New BoundField()
                    '        Bfield.DataField = ColData(i)
                    '        Bfield.HeaderText = ColName(i)
                    '        If Bfield.HeaderText.Contains("Date") Then
                    '            Bfield.DataFormatString = "{0:dd MMM yy}"
                    '        End If
                    '        If Bfield.HeaderText.Contains("Amount") Then
                    '            Bfield.DataFormatString = "{0:#,##0.00}"
                    '            Bfield.ItemStyle.Wrap = False
                    '        End If
                    '        Bfield.HeaderStyle.Width = ColSize(i)
                    '        Bfield.ItemStyle.Wrap = False
                    '        GridViewObj.Columns.Add(Bfield)
                    '    Next
                    '    AddHandler GridViewObj.RowCreated, AddressOf GridView15_RowCreated
                    '    AddHandler GridViewObj.RowDataBound, AddressOf GridView15_RowDataBound

                    Case "BilledItemSummary"
                        GridViewObj.AutoGenerateColumns = True
                        GridViewObj.Columns.Clear()
                        AddHandler GridViewObj.RowCreated, AddressOf GridView10_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView10_RowDataBound

                    Case "TermLicenceAppProduct"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address / Server UUID / DRM", "Licence Term", "Max HQ", "Max Store", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Requested By"}
                        Dim ColData() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "Licence Term", "Max HQ", "Max Store", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Requested By"}
                        Dim ColSize() As Integer = {300, 100, 50, 100, 50, 50, 50, 80, 80, 80, 50, 150, 150}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView11_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView11_RowDataBound

                    Case "TermLicenceAI"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Sales Representative", "Remarks"}
                        Dim ColData() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Requested By", "Remarks"}
                        Dim ColSize() As Integer = {200, 100, 50, 100, 100, 50, 50, 80, 80, 80, 50, 100, 100, 200}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView12_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView12_RowDataBound

                    Case "AILicenceRenewStatus"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Sales Representative", "Remarks"}
                        Dim ColData() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Requested By", "Remarks"}
                        Dim ColSize() As Integer = {200, 100, 50, 100, 100, 50, 50, 80, 80, 80, 50, 100, 100, 200}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView19_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView19_RowDataBound

                    Case "AILicenceExpiredStatus"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Sales Representative", "Remarks"}
                        Dim ColData() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Application Type", "Requested By", "Remarks"}
                        Dim ColSize() As Integer = {200, 100, 50, 100, 100, 50, 50, 80, 80, 80, 50, 100, 100, 200}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView20_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView20_RowDataBound

                    Case "CZLAccountSetupFee"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"By Distributor / Owner", "Country", "Account ID", "Account Name", "Created Date", "PO No", "PO Date", "Currency", "Amount", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColData() As String = {"By Distributor", "Country", "Account ID", "Account Name", "Created Date", "PO No", "PO Date", "Currency", "Amount", "Requested By", "Invoice No", "Invoice Date"}
                        Dim ColSize() As Integer = {250, 80, 50, 150, 50, 100, 50, 50, 50, 100, 80, 50}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            If Bfield.HeaderText.Contains("Amount") Then
                                Bfield.DataFormatString = "{0:#,##0.00}"
                                Bfield.ItemStyle.Wrap = False
                            End If
                            Bfield.HeaderStyle.Wrap = False
                            Bfield.ItemStyle.Wrap = False
                            Bfield.HeaderStyle.Width = ColSize(i)
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView17_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView17_RowDataBound


                    Case "SuspendedStores"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Headquarter ID", "Headquarter Name", "Store No", "Store Name", "Account Type", "Created Date", "Expiry Date", "Suspended Date", "Status", "Requestor", "Reason of suspension"}
                        Dim ColData() As String = {"Headquarter ID", "Headquarter Name", "Store No", "Store Name", "Account Type", "Created Date", "Expiry Date", "Suspended Date", "Status", "Requestor", "Reason of suspension"}
                        Dim ColSize() As Integer = {50, 200, 50, 200, 50, 50, 50, 50, 50, 100, 300}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView13_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView13_RowDataBound

                    'Case "RequestForms"
                    '    GridViewObj.AutoGenerateColumns = False
                    '    GridViewObj.Columns.Clear()
                    '    Dim ColName() As String = {"File Name", "Uploaded Date", "Document Category"}
                    '    Dim ColData() As String = {"File_Name", "Uploaded_DateTime", "Doc_Category"}
                    '    Dim ColSize() As Integer = {300, 100, 100, 100}
                    '    For i = 0 To ColName.Length - 1
                    '        Dim Bfield As BoundField = New BoundField()
                    '        Bfield.DataField = ColData(i)
                    '        Bfield.HeaderText = ColName(i)
                    '        If Bfield.HeaderText.Contains("Date") Then
                    '            Bfield.DataFormatString = "{0:dd MMM yy}"
                    '            Bfield.ItemStyle.Wrap = False
                    '        End If
                    '        Bfield.HeaderStyle.Width = ColSize(i)
                    '        GridViewObj.Columns.Add(Bfield)
                    '    Next

                    '    '' Add templatefield for download icon
                    '    Dim TField As TemplateField = New TemplateField()
                    '    TField.HeaderStyle.Width = Unit.Percentage(2)
                    '    TField.ItemStyle.Wrap = False
                    '    TField.ItemTemplate = New GridViewItemTemplateControl()
                    '    GridViewObj.Columns.Add(TField)

                    '    AddHandler GridViewObj.RowCreated, AddressOf GridView14_RowCreated
                    '    AddHandler GridViewObj.RowDataBound, AddressOf GridView14_RowDataBound

                    Case "StoreDueForRenewal"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Reference No", "Customer Name", "Store Name", "Expiry Date"}
                        Dim ColData() As String = {"Reference No", "Customer Name", "Store Name", "Expiry Date"}
                        Dim ColSize() As Integer = {100, 200, 100, 50}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView21_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView21_RowDataBound

                    Case "ContractUnderProcessing"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Reference No", "Customer Name", "Store Name", "Start Date", "Expiry Date", "Currency", "Amount", "Status"}
                        Dim ColData() As String = {"Reference No", "Customer Name", "Store Name", "Start Date", "Expiry Date", "Currency", "Amount", "Status"}
                        Dim ColSize() As Integer = {100, 200, 100, 50, 50, 50, 50, 100}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:dd MMM yy}"
                            End If
                            Bfield.HeaderStyle.Width = ColSize(i)
                            Bfield.ItemStyle.Wrap = False
                            GridViewObj.Columns.Add(Bfield)
                        Next
                        AddHandler GridViewObj.RowCreated, AddressOf GridView23_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView23_RowDataBound

                    Case "ESLTagsDeploymentOverview"
                        GridViewObj.AutoGenerateColumns = True
                        GridViewObj.Columns.Clear()
                        AddHandler GridViewObj.RowCreated, AddressOf GridView22_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView22_RowDataBound

                End Select

                Try
                    '' GridView Databind
                    GridViewObj.DataSource = GetDataTable(sqlStr)
                    'GridViewObj.DataSource = GetDataSet(sqlStr)    '' Using dataset
                    GridViewObj.DataBind()
                Catch ex As Exception
                    Response.Write("Error:  " & ex.Message)
                End Try

            End If

            '' If Gridview is more then one, then repeat the loop (like i += 1)
            If WebControlItem.Controls.Count > 0 Then
                BuildGridview(WebControlItem, controlName, sqlStr, dataKeyNames)
            End If
        Next
    End Sub




    '' Gridview events

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='pointer';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Define navigation link to each listing page
            e.Row.ToolTip = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim Group As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()

            '' Get the page path
            If Not IsPostBack Then
                'e.Row.Attributes.Add("OnClick", String.Format("window.location='" & GetPagePath(Group) & "'"))  '' This create too much session will slow down the system, as it sends query in a loop
                e.Row.Attributes.Add("OnClick", String.Format("window.location='" & e.Row.Cells(GetColumnIndexByName(e.Row, "Path")).Text & "'"))  '' read the path from column is one time query only
            End If

            '' Total of each column
            TotalHQs += CInt(DataBinder.Eval(e.Row.DataItem, "HQ"))
            TotalStores += CInt(DataBinder.Eval(e.Row.DataItem, "Store"))
            TotalDemo += CInt(DataBinder.Eval(e.Row.DataItem, "Demo"))
            TotalBilled += CInt(DataBinder.Eval(e.Row.DataItem, "Billed"))
            TotalTrial += CInt(DataBinder.Eval(e.Row.DataItem, "Trial"))
            TotalSuspended += CInt(DataBinder.Eval(e.Row.DataItem, "Suspended"))
            TotalClosed += CInt(DataBinder.Eval(e.Row.DataItem, "Closed Store"))

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            '' Assign total value on footer
            e.Row.Cells(0).Text = "Total"
            e.Row.Cells(1).Text = TotalHQs
            e.Row.Cells(2).Text = TotalStores
            e.Row.Cells(3).Text = TotalDemo
            e.Row.Cells(4).Text = TotalBilled
            e.Row.Cells(5).Text = TotalTrial
            e.Row.Cells(6).Text = TotalSuspended
            e.Row.Cells(7).Text = TotalClosed
        End If

        e.Row.Cells(GetColumnIndexByName(e.Row, "Path")).Visible = False  '' Finally hide the Path column
    End Sub

    Private Sub GridView2_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        'If e.Row.RowType = DataControlRowType.DataRow Then
        '    '' Create Edit button at the last column
        '    Dim drv As System.Data.DataRowView = e.Row.DataItem
        '    Dim MarkctrlCellIndex As Integer = e.Row.Cells.Count - 1  '' The last column of gridview
        '    Dim MarkLinkButton As LinkButton = TryCast(e.Row.Cells(MarkctrlCellIndex).Controls(0), LinkButton)  ''convert the template control to linkbutton
        '    MarkLinkButton.Text = "<i class='bi bi-x large'></i>"
        '    MarkLinkButton.CssClass = "btn btn-xs btn-warning"
        '    MarkLinkButton.CausesValidation = False
        '    AddHandler MarkLinkButton.Click, AddressOf Mark_Subscription_Click
        'End If
    End Sub

    Private Sub GridView3_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView3_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
    End Sub

    Private Sub GridView4_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView4_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim Subscription_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim Subscription As GridView = TryCast(e.Row.FindControl("gvContractStore"), GridView)
            Dim query As String = " SELECT * FROM R_DMC_Subscription_Detail WHERE Subscription_ID ='" & Subscription_ID & "' ORDER BY Store_No"
            Try
                Subscription.DataSource = GetDataTable(query)
                Subscription.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub GridView5_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView5_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.Header Then
            Dim HeaderGrid As GridView = DirectCast(sender, GridView)
            Dim HeaderGridRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim HeaderCell = New TableCell()

            For i = 0 To e.Row.Cells.Count - 1
                HeaderCell = New TableCell()   ' Add new tablecell
                Select Case i
                    Case 0
                        HeaderGridRow.Cells.Add(HeaderCell)  '' add tablecell to griviewrow
                        'HeaderCell.Text = "Country"
                    Case 1
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "DMC Enterprise"
                        HeaderCell.ColumnSpan = 6
                        e.Row.Cells(i).Text = "Demo <br> (1 HQ - 2 Stores)"
                    Case 2
                        e.Row.Cells(i).Text = "Tier " & i - 1 & "<br> (1 HQ - 50 Stores)"
                    Case 3
                        e.Row.Cells(i).Text = "Tier " & i - 1 & "<br> (1 HQ - 100 Stores)"
                    Case 4
                        e.Row.Cells(i).Text = "Tier " & i - 1 & "<br> (1 HQ - 200 Stores)"
                    Case 5
                        e.Row.Cells(i).Text = "Tier " & i - 1 & "<br> (1 HQ - 201 Stores & above)"
                    Case 6
                        e.Row.Cells(i).Text = "Tier " & i - 1 & "<br> (Unlimited HQ and Stores)"
                    Case 7
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "T@POP CMS"
                        HeaderCell.ColumnSpan = 2
                        e.Row.Cells(i).Text = "Demo <br> (2 Stores)"
                    Case 8
                        e.Row.Cells(i).Text = "Standard <br> (Unlimited Stores)"
                    Case 9
                        HeaderGridRow.Cells.Add(HeaderCell)
                        HeaderCell.Text = "Total"
                        e.Row.Cells(i).Text = "By country"
                End Select
            Next
            GridViewObj.Controls(0).Controls.AddAt(0, HeaderGridRow)

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            TotalHardkeyDemo += CInt(DataBinder.Eval(e.Row.DataItem, "Demo"))
            TotalTier1 += CInt(DataBinder.Eval(e.Row.DataItem, "Tier 1"))
            TotalTier2 += CInt(DataBinder.Eval(e.Row.DataItem, "Tier 2"))
            TotalTier3 += CInt(DataBinder.Eval(e.Row.DataItem, "Tier 3"))
            TotalTier4 += CInt(DataBinder.Eval(e.Row.DataItem, "Tier 4"))
            TotalTier5 += CInt(DataBinder.Eval(e.Row.DataItem, "Tier 5"))
            TotalCMSDemo += CInt(DataBinder.Eval(e.Row.DataItem, "CMS Demo"))
            TotalCMS += CInt(DataBinder.Eval(e.Row.DataItem, "CMS"))
            TotalHardkey += CInt(DataBinder.Eval(e.Row.DataItem, "Total"))

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(0).Text = "Total"
            e.Row.Cells(1).Text = TotalHardkeyDemo
            e.Row.Cells(2).Text = TotalTier1
            e.Row.Cells(3).Text = TotalTier2
            e.Row.Cells(4).Text = TotalTier3
            e.Row.Cells(5).Text = TotalTier4
            e.Row.Cells(6).Text = TotalTier5
            e.Row.Cells(7).Text = TotalCMSDemo
            e.Row.Cells(8).Text = TotalCMS
            e.Row.Cells(9).Text = TotalHardkey
        Else
        End If

        For i = 1 To e.Row.Cells.Count - 1
            e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Left
        Next
    End Sub

    Private Sub GridView6_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView6_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False
    End Sub

    Private Sub GridView7_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView7_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            TotalAndroid += CInt(DataBinder.Eval(e.Row.DataItem, "Android"))
            TotaliOS += CInt(DataBinder.Eval(e.Row.DataItem, "iOS"))
            TotalWeb += CInt(DataBinder.Eval(e.Row.DataItem, "Web"))
            TotalSM += CInt(DataBinder.Eval(e.Row.DataItem, "SM"))
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(0).Text = "Total"
            e.Row.Cells(1).Text = TotalAndroid
            e.Row.Cells(2).Text = TotaliOS
            e.Row.Cells(3).Text = TotalWeb
            e.Row.Cells(4).Text = TotalSM
        ElseIf e.Row.RowType = DataControlRowType.Header Then
            e.Row.Height = 40
        End If
    End Sub

    Private Sub GridView8_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView8_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
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
            If Len(RemoveHTMLWhiteSpace(Requested_By)) > 0 Then
                query += " AND [Requested By] = '" & Requested_By & "'"
            End If
            query += " ORDER BY [Created Date] DESC "

            Try
                Licence_Code.DataSource = GetDataTable(query)
                Licence_Code.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' display the Child Gridview Requested By column when the PO No is NA
            Licence_Code.Columns(GetColumnIndexByColumnName(Licence_Code, "Requested By")).Visible = IIf(PO_No = "NA", True, False)

            e.Row.Cells(GetColumnIndexByName(e.Row, "Payment Status")).Text = "Pending"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "TBA"
        End If

        For i = 0 To e.Row.Cells.Count - 1
            If i = 5 Then
                e.Row.Cells(i).Style.Add("text-align", "right !important")
            End If
        Next
    End Sub

    Private Sub GridView9_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView9_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "Pending"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice Date")).Text = "TBA"
        End If
    End Sub

    Private Sub GridView10_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView10_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 2 Then
                    e.Row.Cells(i).Text = "Year of " & e.Row.Cells(i).Text & "<br />" & "(SGD)"
                End If
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                If i > 2 Then
                    e.Row.Cells(i).Text = String.Format("{0:c}", CDec(e.Row.Cells(i).Text))
                    e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                End If
            Next
            TotalCurrYear += CDec(DataBinder.Eval(e.Row.DataItem, Year(Now()).ToString()))
            TotalPrevYear += CDec(DataBinder.Eval(e.Row.DataItem, (Year(Now()) - 1).ToString()))
            TotalYearBef += CDec(DataBinder.Eval(e.Row.DataItem, (Year(Now()) - 2).ToString()))
            Total3YearsAgo += CDec(DataBinder.Eval(e.Row.DataItem, (Year(Now()) - 3).ToString))
            Total4YearsAgo += CDec(DataBinder.Eval(e.Row.DataItem, (Year(Now()) - 4).ToString))
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(2).Text = "Total"
            e.Row.Cells(2).HorizontalAlign = HorizontalAlign.Right
            e.Row.Cells(3).Text = String.Format("{0:c}", TotalCurrYear)
            e.Row.Cells(4).Text = String.Format("{0:c}", TotalPrevYear)
            e.Row.Cells(5).Text = String.Format("{0:c}", TotalYearBef)
            e.Row.Cells(6).Text = String.Format("{0:c}", Total3YearsAgo)
            e.Row.Cells(7).Text = String.Format("{0:c}", Total4YearsAgo)
        End If

        For i = 0 To e.Row.Cells.Count - 1
            If i > 2 Then
                e.Row.Cells(i).Width = 150
                e.Row.Cells(i).Style.Add("text-align", "right !important")
            End If
        Next
    End Sub

    Private Sub GridView11_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView11_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.Header Then

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem
            If IsDBNull(drv("Serial No")) Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Serial No")).Text = "(unknown)"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Serial No")).Style.Add("font-style", "italic")
                e.Row.Cells(GetColumnIndexByName(e.Row, "Serial No")).Style.Add("color", "#999999")
            End If
            '' if application type is not DMC Server Licence, then mark column maxhq and maxstore as "-"
            If Not drv("Application Type").ToString.Contains("DMC Server") Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Max HQ")).Text = "-"
                e.Row.Cells(GetColumnIndexByName(e.Row, "Max Store")).Text = "-"
            End If
        End If
    End Sub

    Private Sub GridView12_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView12_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.Header Then

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Set as unknown when the cell return dbnull value
            Dim AI_Tech_Details() As String = {"Serial No", "MAC Address", "AI Device ID", "AI Device Serial No"}
            For i = 0 To AI_Tech_Details.Length - 1
                If IsDBNull(drv(AI_Tech_Details(i))) Then
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Text = "(unknown)"
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("font-style", "italic")
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("color", "#999999")
                End If
            Next

        End If
    End Sub


    Private Sub GridView13_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView13_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
            Next
        End If
    End Sub

    'Private Sub GridView14_RowCreated(sender As Object, e As GridViewRowEventArgs)
    '    ' Call javascript function for GridView Row highlight effect
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
    '        e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
    '    End If
    'End Sub

    'Protected Sub GridView14_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
    '    Dim GridViewObj As GridView = CType(sender, GridView)
    '    GridViewObj.ShowFooter = False
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        Dim drv As System.Data.DataRowView = e.Row.DataItem
    '        Dim DownloadctrlCellIndex As Integer = e.Row.Cells.Count - 1
    '        Dim DownloadLinkButton As LinkButton = TryCast(e.Row.Cells(DownloadctrlCellIndex).Controls(0), LinkButton)
    '        DownloadLinkButton.Text = "<i class='bi bi-download'></i>"
    '        DownloadLinkButton.CommandArgument = drv("ID")
    '        DownloadLinkButton.CssClass = "btn btn-xs btn-info"
    '        DownloadLinkButton.OnClientClick = "document.forms[0].target = '_blank';"  ''open new tab
    '        AddHandler DownloadLinkButton.Click, AddressOf DownloadFile
    '    End If
    'End Sub

    'Private Sub GridView15_RowCreated(sender As Object, e As GridViewRowEventArgs)
    '    ' Call javascript function for GridView Row highlight effect
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
    '        e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
    '    End If
    'End Sub

    'Protected Sub GridView15_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
    '    Dim GridViewObj As GridView = CType(sender, GridView)
    '    GridViewObj.ShowFooter = False

    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        '' Expand for nested Gridview
    '        Dim UID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
    '        Dim AIRenewal As GridView = TryCast(e.Row.FindControl("gvAILicenceList"), GridView)
    '        Dim query As String = " SELECT * FROM R_AI_Licence_Renewal WHERE [UID] ='" & UID & "' "
    '        Try
    '            AIRenewal.DataSource = GetDataTable(query)
    '            AIRenewal.DataBind()
    '        Catch ex As Exception
    '            Response.Write("Error:  " & ex.Message)
    '        End Try

    '        e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "Pending"
    '        e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice Date")).Text = "TBA"

    '    End If

    'End Sub

    Private Sub GridView16_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView16_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "Pending"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice Date")).Text = "TBA"
        End If
    End Sub

    Private Sub GridView17_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView17_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "Pending"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice Date")).Text = "TBA"
        End If
    End Sub

    Private Sub GridView18_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView18_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim UID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim TermedLicence As GridView = TryCast(e.Row.FindControl("gvTermedLicenceList"), GridView)
            Dim query As String = " SELECT * FROM R_Termed_Licence_Renewal WHERE [UID] ='" & UID & "' "
            Try
                TermedLicence.DataSource = GetDataTable(query)
                TermedLicence.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice No")).Text = "Pending"
            e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice Date")).Text = "TBA"

        End If

    End Sub

    Private Sub GridView19_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView19_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.Header Then

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Set as unknown when the cell return dbnull value
            Dim AI_Tech_Details() As String = {"Serial No", "MAC Address", "AI Device ID", "AI Device Serial No"}
            For i = 0 To AI_Tech_Details.Length - 1
                If IsDBNull(drv(AI_Tech_Details(i))) Then
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Text = "(unknown)"
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("font-style", "italic")
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("color", "#999999")
                End If
            Next

        End If
    End Sub

    Private Sub GridView20_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView20_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.Header Then

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Set as unknown when the cell return dbnull value
            Dim AI_Tech_Details() As String = {"Serial No", "MAC Address", "AI Device ID", "AI Device Serial No"}
            For i = 0 To AI_Tech_Details.Length - 1
                If IsDBNull(drv(AI_Tech_Details(i))) Then
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Text = "(unknown)"
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("font-style", "italic")
                    e.Row.Cells(GetColumnIndexByName(e.Row, AI_Tech_Details(i))).Style.Add("color", "#999999")
                End If
            Next

        End If
    End Sub

    Private Sub GridView21_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView21_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

    End Sub

    Private Sub GridView22_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.Header Then
            Dim gridView As GridView = DirectCast(sender, GridView)
            Dim dataColumns As DataControlFieldCollection = gridView.Columns

            ' Create a new GridViewRow for the additional header row
            Dim headerRow As New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Insert)
            Dim headerRowCellArray As New List(Of TableCell)()

            Dim CurrentTagsGroup As String = Nothing
            Dim NextTagsGroup As String = Nothing
            Dim NoColumnSpan As Integer = 0
            Dim HeaderTextTypeCount As Integer = 0

            For i = 0 To e.Row.Cells.Count - 1
                NextTagsGroup = IIf(Not e.Row.Cells(i).Text.Contains("InfoTag") And Not e.Row.Cells(i).Text.Contains("RF Station"), "&nbsp;", e.Row.Cells(i).Text.Substring(0, e.Row.Cells(i).Text.IndexOf("_") + 1))
                If CurrentTagsGroup <> NextTagsGroup Then
                    '' Create new TableCell with proper columnspan before the currentgroup of header name change
                    If Not String.IsNullOrEmpty(CurrentTagsGroup) Then
                        headerRowCellArray(HeaderTextTypeCount - 1).ColumnSpan = NoColumnSpan
                        headerRowCellArray(HeaderTextTypeCount - 1).Text = Replace(CurrentTagsGroup, "_", "")
                        headerRowCellArray(HeaderTextTypeCount - 1).HorizontalAlign = HorizontalAlign.Left
                        headerRow.Cells.Add(headerRowCellArray(HeaderTextTypeCount - 1))
                    End If

                    headerRowCellArray.Add(New TableCell())
                    NoColumnSpan = 1
                    HeaderTextTypeCount += 1
                    CurrentTagsGroup = NextTagsGroup
                Else
                    NoColumnSpan += 1
                End If
            Next

            '' Create TableCell for the last group of header name
            If Not String.IsNullOrEmpty(CurrentTagsGroup) Then
                headerRowCellArray(HeaderTextTypeCount - 1).ColumnSpan = NoColumnSpan
                headerRowCellArray(HeaderTextTypeCount - 1).Text = Replace(CurrentTagsGroup, "_", "")
                headerRowCellArray(HeaderTextTypeCount - 1).HorizontalAlign = HorizontalAlign.Left
                headerRow.Cells.Add(headerRowCellArray(HeaderTextTypeCount - 1))
            End If

            gridView.Controls(0).Controls.AddAt(0, headerRow)   ' Add the header row to the GridView

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView22_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = True

        Dim dataSource As DataTable = CType(GridViewObj.DataSource, DataTable)
        Dim dataRowCount As Integer = dataSource.Rows.Count

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 2 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = e.Row.Cells(i).Text.Substring(e.Row.Cells(i).Text.IndexOf("_") + 1)  '' Get the string after the underscore symbol
            Next

            Dim totalColumns As Integer = e.Row.Cells.Count
            e.Row.Cells(0).Width = New Unit(15, UnitType.Percentage)
            e.Row.Cells(1).Width = New Unit(15, UnitType.Percentage)

            ' Distribute the remaining width evenly among the rest of the columns
            Dim remainingWidth As Double = 70.0 / (totalColumns - 2)
            For i = 2 To totalColumns - 1
                e.Row.Cells(i).Width = New Unit(remainingWidth, UnitType.Percentage)
            Next

        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowIndex = dataRowCount - 1 Then
                e.Row.Visible = False ' Hide the last row from data rows
            Else
                For i = 0 To e.Row.Cells.Count - 1
                    e.Row.Cells(i).Wrap = False
                    If i >= 2 Then
                        e.Row.Cells(i).Text = IIf(e.Row.Cells(i).Text <> "0", e.Row.Cells(i).Text, "-")
                    End If
                Next
            End If

        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            ' Display the last row of the datatable in the footer
            If dataRowCount > 0 Then
                Dim lastRow As DataRow = dataSource.Rows(dataRowCount - 1)
                For i = 0 To lastRow.ItemArray.Length - 1
                    e.Row.Cells(i).Text = lastRow(i).ToString()
                    If i = 1 Then
                        e.Row.Cells(i).HorizontalAlign = HorizontalAlign.Right
                        e.Row.Cells(i).Style.Add("padding-right", "50px")
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub GridView23_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView23_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

    End Sub




    '' Reminder section
    Protected Sub GridViewReminder_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewReminder.RowDataBound
        Dim ColName() As String = {"Mark Done", "Reminder"}
        Dim ColSize() As Integer = {0, 100}
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Width = Unit.Percentage(ColSize(i))
                e.Row.Cells(i).Wrap = True
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Wrap = False
            Next
        End If
    End Sub

    Protected Sub chk_Is_Done_CheckedChanged(sender As Object, e As EventArgs)
        Dim Row As GridViewRow = (CType((CType(sender, Control)).Parent.Parent, GridViewRow))
        Dim ID As String = GridViewReminder.DataKeys(Row.RowIndex).Value.ToString()
        Dim Status As String = IIf(CType(sender, CheckBox).Checked = "True", 1, 0)
        Dim sqlStr As String = " UPDATE DB_Reminder SET Is_Done = " & Status & ", Completed_Date = GETDATE() WHERE ID = " & ID
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        BindGridViewReminder()
    End Sub

    Protected Sub btnAddReminder_Click(sender As Object, e As EventArgs) Handles btnAddReminder.Click
        ModalHeaderReminder.Text = "Add Reminder"
        btnSaveReminder.Text = "Save"
        btnCancelReminder.Text = "Cancel"
        TB_Reminder.Text = String.Empty
        popupReminder.Show()
    End Sub

    Protected Sub btnSaveReminder_Click(sender As Object, e As EventArgs) Handles btnSaveReminder.Click
        Dim Reminder As TextBox = pnlAddReminder.FindControl("TB_Reminder")
        Dim sqlStr As String = " EXEC SP_CRUD_Reminder N'" & EscapeChar(Reminder.Text) & "' "
        Try
            RunSQL(sqlStr)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
        BindGridViewReminder()
    End Sub

    Protected Sub btnCancelReminder_Click(sender As Object, e As EventArgs)
        BindGridViewReminder()
    End Sub


    'Protected Sub Mark_Subscription_Click(ByVal sender As Object, ByVal e As EventArgs)
    '    'Enter code here..
    'End Sub


End Class


