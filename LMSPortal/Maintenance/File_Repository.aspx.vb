Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Ionic.Zip

Partial Class Maintenance_File_Repository
    Inherits LMSPortalBaseCode

    Dim PageTitle As String = "File Repository"

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
            Dim sqlStr As String = "SELECT * FROM R_File_Repository"
            sqlStr += " WHERE [File_Name] LIKE '%" & keyword & "%' "
            sqlStr += " ORDER BY Uploaded_Date DESC, File_Name DESC "

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

    End Sub


    '' Gridview controls
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        PopulateGridViewData(TB_Search.Text)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim ColName() As String = {"File_Name", "Uploaded_Date", "Content_Type", "Category", ""}
        Dim ColSize() As Integer = {300, 300, 300, 300, 800, 0}

        If e.Row.RowType = DataControlRowType.Header Then
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Text = Replace(ColName(i), "_", " ")
                e.Row.Cells(i).Wrap = False
            Next
        ElseIf e.Row.RowType = DataControlRowType.DataRow Then
            Dim CtrlCellIndex As Integer = e.Row.Cells.Count - 1
            For i = 0 To e.Row.Cells.Count - 1
                e.Row.Cells(i).Width = ColSize(i)  '' set the column width at DataRow when headerrow is smaller than HeaderRow
                e.Row.Cells(i).Wrap = False
                If i = CtrlCellIndex Then
                    e.Row.Cells(CtrlCellIndex).Width = 0
                    e.Row.Cells(CtrlCellIndex).Wrap = False
                    e.Row.Cells(CtrlCellIndex).HorizontalAlign = HorizontalAlign.Center
                End If
            Next
        End If
    End Sub

    Private Sub GridView1_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowCreated
        ' Call javascript function for GridView Row highlight effect
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("OnMouseOver", "javascript:SetMouseOver(this);")
            e.Row.Attributes.Add("OnMouseOut", "javascript:SetMouseOut(this);")
        End If
    End Sub



    Protected Sub DeleteFile(ByVal sender As Object, ByVal e As EventArgs)
        Dim ID As Integer = Integer.Parse(TryCast(sender, LinkButton).CommandArgument)
        Dim query As String = " DELETE FROM DB_FileUpload WHERE ID = @ID "
        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString
        Using con As SqlConnection = New SqlConnection(constr)
            Using cmd As SqlCommand = New SqlCommand(query)
                cmd.Parameters.AddWithValue("@ID", ID)
                cmd.Connection = con
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Search button
    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        PopulateGridViewData(TB_Search.Text)
    End Sub



    '' Upload File section
    Protected Sub UploadFile(ByVal sender As Object, ByVal e As EventArgs) Handles BN_Upload.Click
        Dim Doc_Category As String = DDL_Doc_Category.SelectedValue
        Dim filename As String = Path.GetFileName(FileUpload1.PostedFile.FileName)
        Dim contentType As String = FileUpload1.PostedFile.ContentType

        Dim FileExists As Integer = Get_Value("SELECT Count(*) AS FileExists FROM DB_FileUpload WHERE File_Name = '" & filename & "'", "FileExists")
        If FileExists < 1 Then
            Try
                Using fs As Stream = FileUpload1.PostedFile.InputStream
                    Using br As New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(fs.Length)
                        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString

                        Using con As New SqlConnection(constr)
                            Dim query As String = " INSERT INTO DB_FileUpload (File_Name, Uploaded_DateTime, Content_Type, File_Data, Doc_Category) VALUES(@File_Name, @Uploaded_DateTime, @Content_Type, @File_Data, @Doc_Category) "
                            Using cmd As New SqlCommand(query)
                                cmd.Connection = con
                                cmd.Parameters.Add("@File_Name", SqlDbType.NVarChar).Value = filename
                                cmd.Parameters.Add("@Uploaded_DateTime", SqlDbType.DateTime).Value = System.DateTime.Now()
                                cmd.Parameters.Add("@Content_type", SqlDbType.NVarChar).Value = contentType
                                cmd.Parameters.Add("@File_Data", SqlDbType.VarBinary).Value = bytes
                                cmd.Parameters.Add("@Doc_Category", SqlDbType.NVarChar).Value = Doc_Category
                                con.Open()
                                cmd.ExecuteNonQuery()
                                con.Close()
                            End Using
                        End Using
                    End Using
                End Using

                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('File successfully upload.');</script>")
            Catch ex As Exception
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('File upload failed.');</script>")
            End Try
        Else
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('File exists.');</script>")
        End If

        DDL_Doc_Category.SelectedIndex = 0
        TB_Search.Text = String.Empty
        PopulateGridViewData(TB_Search.Text)
    End Sub


    '' Download and zip all binary file in DB_FileUpload table
    Protected Sub DownloadMultipleFile(sender As Object, e As EventArgs) Handles BN_DownloadAll.Click
        Dim zip As ZipFile = New ZipFile()
        zip.AlternateEncodingUsage = ZipOption.AsNecessary

        Dim bytes As Byte() = Nothing
        Dim dr = RunSQLExecuteReader("SELECT ID, File_Name FROM DB_FileUpload")
        While dr.Read()
            bytes = GetBinaryData(dr("ID"))
            zip.AddEntry(dr("File_Name"), bytes)
        End While

        Response.Clear()
        Response.BufferOutput = False
        Dim zipName As String = String.Format("Archived_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"))
        Response.ContentType = "application/zip"
        Response.AddHeader("content-disposition", "attachment; filename=" & zipName)
        zip.Save(Response.OutputStream)
        Response.[End]()
    End Sub

    Protected Function GetBinaryData(ByVal id As String) As Byte()
        Dim bytes As Byte()
        Dim query As String = " SELECT File_Data FROM DB_FileUpload WHERE ID = @Id "
        Dim cmd As SqlCommand = New SqlCommand(query)
        Using con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString)
            cmd.Parameters.AddWithValue("@Id", id)
            cmd.Connection = con
            con.Open()
            bytes = CType(cmd.ExecuteScalar(), Byte())
            con.Close()
        End Using
        Return bytes
    End Function

End Class
