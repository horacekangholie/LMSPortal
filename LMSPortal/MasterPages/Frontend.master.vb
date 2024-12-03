Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports System.Web.Configuration
Imports System.Web.Security

Partial Class MasterPages_Frontend
    Inherits System.Web.UI.MasterPage

    Dim Roles_Type As String
    Dim Username As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Roles_Type = Session("User_Roles")
        Username = Session("User_Name")

        If Not Username Is Nothing Then
            UserIcon.Text = "<div class='UserIcon'><i class='bx bx-user-circle'></i></div>"
            LoggedInUser.Text = Username
        Else
            LoggedInUser.Text = String.Empty
            Response.Redirect("~/Login.aspx")
        End If

        '' Connection to Database
        Dim Conn As New SqlConnection(WebConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString)
        Dim dbAdapter As New SqlDataAdapter()

        '' Define SQL Query
        Dim sqlQuery As New SqlCommand
        sqlQuery.CommandText = " SELECT Icon, Module_Name, Sub_Module, Path + '?Module=' + Replace(Sub_Module, ' ', '_') AS Path FROM DB_Access_Map "

        Select Case Roles_Type
            Case "Admin"
                sqlQuery.CommandText += "WHERE Admin = 1 "
            Case "Tech"
                sqlQuery.CommandText += "WHERE Tech = 1 "
            Case "Sales"
                sqlQuery.CommandText += "WHERE Sales = 1 "
            Case "CtrAdmin"
                sqlQuery.CommandText += "WHERE CtrAdmin = 1"
        End Select

        sqlQuery.CommandText += " ORDER BY Level_1, Level_2 "

        '' Execute SQL Query
        dbAdapter.SelectCommand = sqlQuery
        sqlQuery.Connection = Conn

        Dim ds As DataSet = New DataSet()
        Try
            Conn.Open()
            dbAdapter.Fill(ds)      '' Fill data to DataSet
            Conn.Close()            '' close the connection
        Catch ex As Exception
            Response.Write("Error:  " & ex.Message)
        End Try

        Dim CurrGroup As String = Nothing
        Dim GroupName As String

        '' Define Dashboard menu and logout menu link
        Dim DashboardLink As HyperLink = New HyperLink
        Dim DashboardBootstrapIcons As String = "<i class='bx bxs-grid-alt' style='padding-right:8px'></i>"
        DashboardLink.ID = "LN_Dashboard"
        DashboardLink.Text = DashboardBootstrapIcons & "Dashboard"
        DashboardLink.NavigateUrl = "../Default.aspx"

        Dim logoutLink As LinkButton = New LinkButton
        Dim LogoutBoostrapIcons As String = "<i class='bx bx-log-out' style='padding-right:8px'></i>"
        logoutLink.ID = "LN_Logout"
        logoutLink.Text = LogoutBoostrapIcons & "Logout"
        logoutLink.OnClientClick = "document.forms[0].target = '_self';"
        AddHandler logoutLink.Click, AddressOf logout_OnClick

        SideMenu.Controls.Add(New LiteralControl("<ul class='list-unstyled components mb-5'>"))   '' Open <ul>

        '' Dashboard link
        SideMenu.Controls.Add(New LiteralControl("<li>"))
        SideMenu.Controls.Add(DashboardLink)
        SideMenu.Controls.Add(New LiteralControl("</li>"))

        ' Loop through and extract data
        For Each row As DataRow In ds.Tables(0).Rows
            ' Define dropdown subheader as HyperLink
            Dim subHeader As HyperLink = New HyperLink
            subHeader.Text = row("Icon") & row("Module_Name").ToString()
            subHeader.ID = row("Module_Name").ToString()
            subHeader.Attributes.Add("data-toggle", "collapse")
            subHeader.Attributes.Add("aria-expanded", "false")
            subHeader.CssClass = "dropdown-toggle"

            ' Define sibling item as HyperLink
            Dim navLink As HyperLink = New HyperLink
            navLink.ID = row("Sub_Module").ToString()
            navLink.Text = row("Sub_Module").ToString()
            navLink.NavigateUrl = row("Path").ToString()
            navLink.Target = IIf(navLink.Text = "Notion", "blank", "_self")

            GroupName = row("Module_Name").ToString()

            If CurrGroup <> GroupName Then
                If Not CurrGroup Is Nothing Then
                    SideMenu.Controls.Add(New LiteralControl("</ul>"))
                    SideMenu.Controls.Add(New LiteralControl("</li>"))
                End If
                SideMenu.Controls.Add(New LiteralControl("<li style='cursor:pointer;'>"))
                SideMenu.Controls.Add(subHeader)
                SideMenu.Controls.Add(New LiteralControl("<ul class='collapse list-unstyled' id='pageSubmenu'>"))
                SideMenu.Controls.Add(New LiteralControl("<li style='padding-left:14px'>"))
                SideMenu.Controls.Add(navLink)
                SideMenu.Controls.Add(New LiteralControl("</li>"))
            Else
                SideMenu.Controls.Add(New LiteralControl("<li style='padding-left:14px'>"))
                SideMenu.Controls.Add(navLink)
                SideMenu.Controls.Add(New LiteralControl("</li>"))
            End If
            CurrGroup = row("Module_Name").ToString()
        Next
        SideMenu.Controls.Add(New LiteralControl("</ul>"))
        SideMenu.Controls.Add(New LiteralControl("</li>"))

        '' Logout link
        SideMenu.Controls.Add(New LiteralControl("<li>"))
        SideMenu.Controls.Add(logoutLink)
        SideMenu.Controls.Add(New LiteralControl("</li>"))

        SideMenu.Controls.Add(New LiteralControl("</ul>"))    '' close </ul>
    End Sub

    Public Sub logout_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles LB_Logout.Click
        Session("Login_Status") = "Logged out"
        Session("Curr_Status") = ""
        Session("Curr_Reporter") = ""
        Session("Tier") = ""
        Session("AppStatus") = ""
        Session("SalesRep") = ""

        ' Capture logout log
        AuditLog()

        Session("User_Name") = ""
        Session("User_Roles") = ""
        Session("User_Group") = ""
        Roles_Type = Nothing
        Username = Nothing

        ' User logout
        FormsAuthentication.SignOut()
        FormsAuthentication.RedirectToLoginPage()
    End Sub

    Protected Sub AuditLog()
        Dim success As Integer
        Session("User_Name") = IIf(Session("User_Name") = "" Or Session("User_Name") Is Nothing, Username, Session("User_Name"))
        Session("User_Roles") = IIf(Session("User_Roles") = "" Or Session("User_Roles") Is Nothing, Roles_Type, Session("User_Roles"))

        Dim Hostname As String = My.Computer.Name
        Dim sqlStr As String = "EXEC SP_AuditLog '" & Session("Login_Status") & "'" &
                                              ", '" & Date.Now().ToString("yyyy-MM-dd HH:mm:ss") & "'" &
                                              ", '" & Session("User_Name") & "'" &
                                              ", '" & Session("User_Roles") & "'" &
                                              ", '" & Hostname & "'"

        If Session("User_Name") <> "Administrator" Then
            success = RunSQL(sqlStr)
        End If
    End Sub

    Protected Function RunSQL(ByVal sqlStr As String) As Integer
        Dim Conn As SqlConnection
        Dim constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString
        Dim cmd As SqlCommand

        Conn = New SqlConnection(constr)
        cmd = New SqlCommand(sqlStr, Conn)
        Conn.Open()
        Return cmd.ExecuteNonQuery()
        Conn.Close()
    End Function


End Class

