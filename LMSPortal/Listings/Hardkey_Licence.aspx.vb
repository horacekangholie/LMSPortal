
Partial Class Listings_Hardkey_Licence
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "Hardkey Licence"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LB_PageTitle.Text = PageTitle

        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub PopulateGridViewData(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Try
            Dim sqlStr = "SELECT Customer_ID, Name, PO_No, PO_Date, SO_No, SO_Date, Invoice_No, Invoice_Date, Prepared_By FROM I_LMS_Hardkey_Licence "
            sqlStr += "WHERE Name LIKE '%" & keyword & "%' OR PO_No LIKE '%" & keyword & "%' OR SO_No LIKE '%" & keyword & "%' OR Invoice_No LIKE '%" & keyword & "%' OR Licence_No LIKE '%" & keyword & "%' OR Requested_By LIKE '%" & keyword & "%' "
            sqlStr += "GROUP BY Customer_ID, Name, PO_No, PO_Date, SO_No, SO_Date, Invoice_No, Invoice_Date, Prepared_By "
            sqlStr += "ORDER BY PO_Date DESC"

            BuildGridView()

            ''Response.Write(sqlStr & "<br>")
            GridView1.DataSource = GetDataTable(sqlStr)
            GridView1.DataBind()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        '' Get the total record
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
        GridView1.DataKeyNames = New String() {"Customer_ID"}
        GridView1.CssClass = "table table-bordered"
        GridView1.Width = Unit.Percentage(100)

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

        GridView1.Columns.Clear()

        Dim ColData() As String = {"Name", "PO_No", "PO_Date", "SO_No", "SO_Date", "Invoice_No", "Invoice_Date", "Prepared_By"}
        Dim ColSize() As Integer = {40, 10, 10, 10, 10, 10, 10, 10, 10}

        '' add template field for the nested gridview
        Dim Expandfield As TemplateField = New TemplateField()
        Expandfield.ItemTemplate = New HardkeyNestedGridViewItemTemplate()
        Expandfield.HeaderStyle.Width = Unit.Percentage(1)
        GridView1.Columns.Add(Expandfield)

        For i = 0 To ColData.Length - 1
            Dim Bfield As BoundField = New BoundField()
            Bfield.DataField = ColData(i)
            Bfield.HeaderText = Replace(ColData(i), "_", " ")
            Bfield.HeaderStyle.Width = Unit.Percentage(ColSize(i))
            If Bfield.HeaderText.Contains("Date") Then
                Bfield.DataFormatString = "{0:dd MMM yy}"
            End If
            Bfield.HeaderStyle.Wrap = False
            Bfield.ItemStyle.Wrap = False
            GridView1.Columns.Add(Bfield)
        Next

        '' Add template field for the edit button
        Dim TField As TemplateField = New TemplateField()
        TField.HeaderStyle.Width = Unit.Percentage(2)
        TField.ItemStyle.Wrap = False
        TField.ItemTemplate = New GridViewItemTemplateControl()
        GridView1.Columns.Add(TField)

    End Sub


    '' Gridview control
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim GridViewObj As GridView = CType(sender, GridView)
        GridViewObj.ShowFooter = False

        If e.Row.RowType = DataControlRowType.DataRow Then
            '' Expand for nested Gridview
            Dim Customer_ID As String = GridViewObj.DataKeys(e.Row.RowIndex).Value.ToString()
            Dim PO_No As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO_No")).Text
            Dim PO_Date As String = e.Row.Cells(GetColumnIndexByName(e.Row, "PO_Date")).Text
            Dim HarkeyLicence As GridView = TryCast(e.Row.FindControl("gvHardkeyLicence"), GridView)
            Dim query As String = " SELECT * FROM I_LMS_Hardkey_Licence " &
                                  " WHERE Customer_ID = '" & Customer_ID & "'  AND PO_No = '" & PO_No & "' AND PO_Date = '" & CDate(PO_Date).ToString("yyyy-MM-dd") & "' ORDER BY Licence_No "
            Try
                HarkeyLicence.DataSource = GetDataTable(query)
                HarkeyLicence.DataBind()
            Catch ex As Exception
                Response.Write("Error:  " & ex.Message)
            End Try

            '' Get Data row details
            Dim drv As System.Data.DataRowView = e.Row.DataItem

            '' Invoice Download Link
            Dim InvoiceDownloadLink As HyperLink = New HyperLink()
            InvoiceDownloadLink.ID = "lnkDownload"

            If drv("Invoice_No") <> "" And drv("Invoice_No") <> "NA" Then
                e.Row.Cells(GetColumnIndexByName(e.Row, "Invoice_No")).Controls.Add(InvoiceDownloadLink)
                InvoiceDownloadLink.Text = drv("Invoice_No")
                InvoiceDownloadLink.NavigateUrl = String.Format("/Download/DownloadFile.aspx?Inv_Ref_No={0}", drv("Invoice_No"))
                InvoiceDownloadLink.Target = "_blank"
            End If

            '' Edit Button
            Dim EditctrlCellIndex As Integer = e.Row.Cells.Count - 1
            Dim EditLinkButton As LinkButton = TryCast(e.Row.Cells(EditctrlCellIndex).Controls(0), LinkButton)
            EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
            EditLinkButton.CssClass = "btn btn-xs btn-info"
            EditLinkButton.CommandName = drv("Customer_ID")
            EditLinkButton.CommandArgument = drv("PO_No")
            EditLinkButton.CausesValidation = False
            AddHandler EditLinkButton.Click, AddressOf Edit_HardkeyLicenceOrder_Click

            '' Lock record if invoice has been recovered
            If drv("Invoice_No") = "" Then
                EditLinkButton.Text = "<i class='bi bi-pencil-fill'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-info"
                EditLinkButton.Enabled = True
            Else
                EditLinkButton.Text = "<i class='bi bi-lock'></i>"
                EditLinkButton.CssClass = "btn btn-xs btn-light disabled"
                EditLinkButton.ToolTip = "Item Locked"
                EditLinkButton.Enabled = False
            End If

        End If

    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub


    '' record edit control
    Protected Sub Edit_HardkeyLicenceOrder_Click(ByVal sender As Object, ByVal e As EventArgs)
        ModalHeaderHardkeyLicenceOrder.Text = "Recover Invoice"
        btnUpdateHardkeyLicenceOrder.Text = "Update"
        btnCancelHardkeyLicenceOrder.Text = "Cancel"

        Dim EditLinkButton As LinkButton = TryCast(sender, LinkButton)
        TB_PO_No.Text = EditLinkButton.CommandArgument   '' pass the PO No from link button command argument
        TB_Customer_ID.Text = EditLinkButton.CommandName

        TB_SO_No.Text = String.Empty
        TB_SO_Date.Text = String.Empty
        TB_Invoice_No.Text = String.Empty
        TB_Invoice_Date.Text = String.Empty

        RequiredField_TB_SO_No.Enabled = True
        RequiredField_TB_SO_Date.Enabled = True
        RequiredField_TB_Invoice_No.Enabled = True
        RequiredField_TB_Invoice_Date.Enabled = True

        popupHardkeyLicenceOrder.Show()
    End Sub

    Protected Sub Update_HardkeyLicenceOrder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateHardkeyLicenceOrder.Click
        Dim Customer_ID As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_Customer_ID")
        Dim PO_No As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_PO_No")

        Dim SO_No As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_SO_No")
        Dim SO_Date As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_SO_Date")
        Dim Invoice_No As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_Invoice_No")
        Dim Invoice_Date As TextBox = pnlUpdateHardkeyLicenceOrder.FindControl("TB_Invoice_Date")

        Try
            Dim sqlStr As String = " UPDATE LMS_Hardkey_Licence SET SO_No = '" & SO_No.Text &
                                                            "', SO_Date = '" & SO_Date.Text &
                                                            "', Invoice_No = '" & Invoice_No.Text &
                                                            "', Invoice_Date = '" & Invoice_Date.Text & "' " &
                                   " WHERE Customer_ID = '" & Customer_ID.Text & "' AND PO_No = '" & PO_No.Text & "' "

            Dim sqlStr1 As String = " EXEC SP_Insert_Hardkey_Order_Recovered_Invoice_Items '" & Customer_ID.Text & "', '" & PO_No.Text & "' "

            RunSQL(sqlStr)
            RunSQL(sqlStr1)

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub Cancel_HardkeyLicenceOrder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancelHardkeyLicenceOrder.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub

End Class
