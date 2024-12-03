
Partial Class Reports_AI_Licence_Expiry_Reminder
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGridview()
        End If
    End Sub

    Protected Sub BindGridview(Optional ByVal TB_Search As String = "")
        Dim ControlObject() As Object = {New GridView(), New GridView(), New GridView()}
        Dim ReportName() As String = {"AI Licence (Expiring)", "AI Licence (Expired)", "AI Licence (Renewed)"}
        Dim ControlName() As String = {"AI_Licence_Expiring", "AI_Licence_Expired", "AI_Licence_Renew"}

        Dim sqlStr() As String = {"SELECT * FROM D_Licence_With_Term WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] IN ('PC Scale (AI)') AND [Status] NOT IN ('Renew', 'Blocked', 'Expired') AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') ORDER BY [Expired Date], [Serial No] ",
                                  "SELECT * FROM D_Licence_With_Term WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] IN ('PC Scale (AI)') AND [Status] IN ('Expired') AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') ORDER BY [Expired Date], [Serial No] ",
                                  "SELECT * FROM D_Licence_With_Term WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) AND [Application Type] IN ('PC Scale (AI)') AND [Status] IN ('Renew') AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') ORDER BY [Expired Date], [Serial No] "}

        Dim GridViewDataKeyNames() As String = {"Licence Code", "Licence Code", "Licence Code"}

        Dim boxmessage() As String = {"<ul><li>Marketing sales representatives to observe expiry date of each AI License.</li><li>Liase with distributors/customers to send order and forward the order details to license admin.</li><ul> ",
                                      "<ul><li>AI Licenses which are not renewed will be marked as 'Expired' after the expiry date.</li><li>Marketing sales representatives to acknowledge above list of expired AI Licenses.</li><ul> ",
                                      "<ul><li>Marketing sales representatives be informed of above renewed AI Licenses.</li><li>Tech Support will liase and follow up with distributors/customers and provide assistances on AI License re-authentication. "}

        For i = 0 To ControlObject.Length - 1
            If ControlObject(i).GetType().Name = "GridView" Then
                Dashboard.Controls.Add(New LiteralControl("<div class='reportTitle'><h2 style='font-weight:600'>" & ReportName(i) & "</h2></div>"))
                If i < 1 Then
                    Dashboard.Controls.Add(New LiteralControl("<h4 class='licenceExpiryDateRange'>Licence expiry fall on date range: <b>" & DateSerial(Year(Now), Month(Now), 1).ToString("dd MMM yy") & " - " & DateSerial(Year(Now), Month(Now) + 3, 0).ToString("dd MMM yy") & "</b></h4>"))
                End If
                Dashboard.Controls.Add(New LiteralControl("<hr>"))
                Dashboard.Controls.Add(ControlObject(i))
                BuildGridview(Me, ControlName(i), sqlStr(i), GridViewDataKeyNames(i))
                Dashboard.Controls.Add(New LiteralControl("<div class='card bg-light mb-3' style='margin-top:20px'><div class='card-body'>"))
                Dashboard.Controls.Add(New LiteralControl("<b>Note:</b><div>" & boxmessage(i) & "</div>"))
                Dashboard.Controls.Add(New LiteralControl("</div></div>"))
            End If
        Next
    End Sub

    Protected Sub BuildGridview(ByVal parent As Control, ByVal controlName As String, ByVal sqlStr As String, ByVal dataKeyNames As String)
        For Each WebControlItem As Control In parent.Controls
            If WebControlItem.GetType().Name = "GridView" AndAlso WebControlItem.ID Is Nothing Then
                Dim GridViewObj As GridView = CType(WebControlItem, GridView)

                '' GridView Properties
                GridViewObj.ID = controlName
                GridViewObj.AllowPaging = False
                GridViewObj.CellPadding = 4
                GridViewObj.Font.Size = 10
                GridViewObj.GridLines = GridLines.None
                GridViewObj.ShowFooter = False
                GridViewObj.ShowHeaderWhenEmpty = False
                GridViewObj.DataKeyNames = New String() {dataKeyNames}
                GridViewObj.CssClass = "table table-bordered"
                GridViewObj.Width = Unit.Percentage(100)

                '' Header Style
                GridViewObj.HeaderStyle.CssClass = "table table-primary"
                GridViewObj.HeaderStyle.Font.Bold = True
                GridViewObj.HeaderStyle.VerticalAlign = VerticalAlign.Top

                '' Row Style
                GridViewObj.RowStyle.CssClass = "Default"
                GridViewObj.RowStyle.VerticalAlign = VerticalAlign.Middle

                '' Empty Data Template
                GridViewObj.EmptyDataText = "No records found."

                '' Define each Gridview
                Select Case controlName
                    Case "AI_Licence_Expiring", "AI_Licence_Renew", "AI_Licence_Expired"
                        GridViewObj.AutoGenerateColumns = False
                        GridViewObj.Columns.Clear()
                        Dim ColName() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Term (Month)", "Created Date", "Activated Date", "Expired Date", "Status", "Sales Representative", "Remarks"}
                        Dim ColData() As String = {"Licensee", "Licence Code", "Serial No", "MAC Address", "AI Device ID", "AI Device Serial No", "Licence Term", "Created Date", "Activated Date", "Expired Date", "Status", "Requested By", "Remarks"}
                        Dim ColSize() As Integer = {15, 10, 4, 7, 12, 7, 3, 4, 4, 4, 4, 5, 21}

                        For i = 0 To ColName.Length - 1
                            Dim Bfield As BoundField = New BoundField()
                            Bfield.DataField = ColData(i)
                            Bfield.HeaderText = ColName(i)
                            If Bfield.HeaderText.Contains("Date") Then
                                Bfield.DataFormatString = "{0:yyyy-MM-dd}"
                                Bfield.ItemStyle.Wrap = False
                            ElseIf Bfield.HeaderText.Contains("AI Device ID") Or Bfield.HeaderText.Contains("Licence Code") Then
                                Bfield.ItemStyle.Wrap = False
                            Else
                                Bfield.ItemStyle.Wrap = True
                            End If

                            Bfield.HeaderStyle.Width = Unit.Percentage(ColSize(i))
                            GridViewObj.Columns.Add(Bfield)
                        Next

                        AddHandler GridViewObj.RowCreated, AddressOf GridView_RowCreated
                        AddHandler GridViewObj.RowDataBound, AddressOf GridView_RowDataBound
                End Select

                Try
                    '' GridView Databind
                    GridViewObj.DataSource = GetDataTable(sqlStr)
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


    '' Gridview control
    Private Sub GridView_RowCreated(sender As Object, e As GridViewRowEventArgs)
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "this.style.cursor='default';")
            e.Row.Attributes.Add("OnMouseOut", "this.style.cursor='none';")
        End If
    End Sub

    Protected Sub GridView_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        Dim GridViewObj As GridView = CType(sender, GridView)
        If e.Row.RowType = DataControlRowType.DataRow Then
            'e.Row.Cells(GetColumnIndexByName(e.Row, "MAC Address")).Text = HidePartialFieldData(e.Row.Cells(GetColumnIndexByName(e.Row, "MAC Address")).Text)
        End If
    End Sub


    Protected Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Response.Write("<script>window.close()</script>")
    End Sub

End Class
