Imports System.Data.SqlClient

Partial Class Form_Knowledge_Base
    Inherits LMSPortalBaseCode


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Me.Page.User.Identity.IsAuthenticated AndAlso Session("Login_Status") <> "Logged in" Then
            FormsAuthentication.RedirectToLoginPage()
        End If

        If Not Me.IsPostBack Then
            Me.BindRepeater()
        End If

        LB_PageTitle.Text = "Search articles"
    End Sub

    Private Sub BindRepeater(Optional ByVal TB_Search As String = "")
        Dim keyword As String = EscapeChar(TB_Search)
        Dim sqlStr As String = " SELECT DISTINCT * FROM DB_Knowledge_Base "

        If TB_Search <> "" Then
            sqlStr += " WHERE Article_Title Like '%" & keyword & "%' Or Article_Category Like '%" & keyword & "%' Or Article_Content Like '%" & keyword & "%'"
        Else
            sqlStr += " WHERE 1=0 "
        End If

        sqlStr += " ORDER BY Article_Category, Article_Title"

        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString

        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand(sqlStr)
                cmd.CommandText = sqlStr
                cmd.Connection = con
                con.Open()
                rptAccordian.DataSource = cmd.ExecuteReader()
                rptAccordian.DataBind()
                con.Close()
            End Using
        End Using

    End Sub

    Protected Sub BT_Search_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BT_Search.Click
        BindRepeater(TB_Search.Text)
        TB_Search.Text = String.Empty
    End Sub

End Class
