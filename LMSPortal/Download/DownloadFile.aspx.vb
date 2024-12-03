Imports System.Data
Imports System.Data.SqlClient

Partial Class Download_DownloadFile
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim Inv_Ref_No As String = Request.QueryString("Inv_Ref_No")
        Dim bytes As Byte() = Nothing
        Dim filename As String = Nothing
        Dim contentType As String = Nothing
        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString
        Dim HasFile As Boolean = True

        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand()
                cmd.CommandText = " SELECT File_Name, Content_Type, File_Data " &
                                  " FROM DB_FileUpload " &
                                  " WHERE Content_Type LIKE '%pdf%' " &
                                  "   AND REPLACE(File_Name, '.pdf', '') = SUBSTRING(@Inv_Ref_No, 5, LEN(@Inv_Ref_No) - 4) "

                cmd.Parameters.AddWithValue("@Inv_Ref_No", Inv_Ref_No)
                cmd.Connection = con
                con.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    If sdr.HasRows Then
                        sdr.Read()
                        bytes = DirectCast(sdr("File_Data"), Byte())
                        contentType = sdr("Content_Type").ToString()
                        filename = sdr("File_Name").ToString()
                    Else
                        HasFile = False
                    End If
                End Using
                con.Close()
            End Using
        End Using

        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType

        If HasFile Then
            Response.AppendHeader("Content-Disposition", "inline; filename=""" + filename + """")
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.End()
        Else
            Select Case Session("User_Roles")
                Case "Admin"
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('Please upload the invoice at [Maintenance] > [Upload File] \nTo name the file based on Invoice no. \n\n E.g. TWS/29001234 \n Name the invoice file as 29001234.pdf');</script>")
                Case Else
                    Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('The Invoice document is not available.\n\nPlease contact Administrator to upload the invoice.');</script>")
            End Select
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "Close", "window.close()", True)
        End If
    End Sub

End Class
