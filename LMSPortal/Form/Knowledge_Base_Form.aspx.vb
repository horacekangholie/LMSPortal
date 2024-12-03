Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.Configuration

Partial Class Form_Knowledge_Base_Form
    Inherits LMSPortalBaseCode

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        LB_PageTitle.Text = "Knowledge Notes"
        articlecontentemptyerrormsg.Visible = False
    End Sub

    Protected Sub DDL_Article_Category_Load(sender As Object, e As EventArgs) Handles DDL_Article_Category.Load
        If Not IsPostBack Then
            Try
                Dim sqlStr As String = " SELECT Value_1 As Article_Category " &
                                       " FROM DB_Lookup " &
                                       " WHERE Lookup_Name = 'Article Category' " &
                                       " ORDER BY Case When Value_1 = 'Please select' THEN 1 ELSE 2 END, Value_1 "

                ''Response.Write(sqlStr)
                DDL_Article_Category.DataSource = GetDataTable(sqlStr)
                DDL_Article_Category.DataTextField = "Article_Category"
                DDL_Article_Category.DataValueField = "Article_Category"
                DDL_Article_Category.DataBind()
            Catch ex As Exception
                Response.Write("Error: " & ex.Message)
            End Try
        End If
    End Sub

    Protected Sub Create(sender As Object, e As EventArgs) Handles BT_Create.Click
        Dim Article_Title As String = TB_Article_Title.Text
        Dim Article_Category As String = DDL_Article_Category.SelectedValue
        Dim Article_Content As String = TB_Article_Content.Text

        Dim sqlStr As String = " EXEC SP_Create_Article @Article_Title, @Article_Category, @Article_Content, @Submitted_By "
        Dim rowAffected As Integer = 0
        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString

        If Len(Article_Content) > 0 Then
            Using con As New SqlConnection(constr)
                Dim query As String = sqlStr
                Using cmd As New SqlCommand(query)
                    cmd.Connection = con
                    cmd.Parameters.Add("@Article_Title", SqlDbType.NVarChar).Value = Article_Title
                    cmd.Parameters.Add("@Article_Category", SqlDbType.NVarChar).Value = Article_Category
                    cmd.Parameters.Add("@Article_Content", SqlDbType.NVarChar).Value = HttpUtility.HtmlDecode(EscapeCharForArticle(Article_Content))
                    cmd.Parameters.Add("@Submitted_By", SqlDbType.NVarChar).Value = Session("User_Name")
                    con.Open()
                    rowAffected = cmd.ExecuteNonQuery()
                    con.Close()
                End Using
            End Using

            If rowAffected > 0 Then
                Response.Redirect("~\Form\Knowledge_Base.aspx")
            Else
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "msgBox1", "<script>alert('Record not saved.');</script>")
            End If
        Else
            articlecontentemptyerrormsg.Visible = True
        End If
    End Sub

    Protected Sub Cancel(sender As Object, e As EventArgs) Handles BT_Cancel.Click
        Response.Redirect("~\Form\Knowledge_Base.aspx")
    End Sub

End Class
