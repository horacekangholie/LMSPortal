Imports System.Data.SqlClient
Imports System.Web.Security

Partial Class Login
    Inherits LMSPortalBaseCode

    Dim ControlObject() As Object = {New TextBox(), New TextBox(), New Button()}
    Dim ControlName() As String = {"Username", "Password", "Login"}

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Request.QueryString("status") Is Nothing Then
            Response.Write(AlertMessage("The server is busy. \nPlease try again few minutes later."))
        End If

        For i = 0 To ControlObject.Length - 1
            If ControlObject(i).GetType().Name <> "Button" Then
                PlaceHolder1.Controls.Add(ControlObject(i))
                BuildLoginPage(Me, ControlName(i))
            Else
                Dim Loginbtn As Button = ControlObject(i)
                PlaceHolder1.Controls.Add(ControlObject(i))
                BuildLoginPage(Me, ControlName(i))
                AddHandler Loginbtn.Click, AddressOf Loginbtn_Click
            End If
        Next
    End Sub

    Protected Sub BuildLoginPage(ByVal parent As Control, ByVal controlname As String)
        For Each WebControlItem As Control In parent.Controls
            '' Define web control attributes
            If WebControlItem.GetType().Name = "TextBox" AndAlso WebControlItem.ID Is Nothing Then
                CType(WebControlItem, TextBox).ID = controlname
                CType(WebControlItem, TextBox).CssClass = "form-control"
                CType(WebControlItem, TextBox).Attributes.Add("required", "")
                If controlname = "Password" Then
                    CType(WebControlItem, TextBox).TextMode = TextBoxMode.Password   '' Changed to password mode for password field
                    CType(WebControlItem, TextBox).Attributes.Add("placeholder", "Password")
                Else
                    CType(WebControlItem, TextBox).Attributes.Add("placeholder", "Username")
                End If
            End If

            If WebControlItem.GetType().Name = "Button" AndAlso WebControlItem.ID Is Nothing Then
                CType(WebControlItem, Button).ID = controlname
                CType(WebControlItem, Button).Text = controlname
                CType(WebControlItem, Button).CssClass = "btn btn-primary btn-block col-12"
            End If

            If WebControlItem.Controls.Count > 0 Then
                BuildLoginPage(WebControlItem, controlname)
            End If
        Next
    End Sub

    Protected Sub Loginbtn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim Username As TextBox = Page.FindControl("Username")
        Dim Password As TextBox = Page.FindControl("Password")

        Dim sqlString As String = "SELECT Username, Password, Roles, Display_Name, User_Group " &
                                  "FROM DB_Users " &
                                  "WHERE Username = '" & Username.Text.ToLower() & "' " &
                                  "  AND Password = '" & Encrypt(Password.Text) & "' "

        Try
            Dim dr As SqlDataReader = RunSQLExecuteReader(sqlString)

            '' Check SQL Injection
            If Check_SQLInjection(Username.Text, Password.Text) > 0 Then
                Response.Write(AlertMessage("Login credential contain SQL Injection code"))
            Else
                If dr.HasRows Then
                    dr.Read()
                    Session("User_Name") = dr.Item("Display_Name").ToString()
                    Session("Login_Name") = dr.Item("Username").ToString()
                    Session("User_Roles") = dr.Item("Roles").ToString()
                    Session("User_Group") = dr.Item("User_Group").ToString()
                    Session("Login_Status") = "Logged in"

                    AuditLog(Session("Login_Status"))  ' Capture login log
                    FormsAuthentication.RedirectFromLoginPage(Session("User_Name"), False)
                    Response.Redirect("~/Default.aspx")
                Else
                    Response.Write(AlertMessage("Username or password is incorrect\nPlease check and try again."))
                    Exit Sub
                End If
            End If

            dr.Close()
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try

    End Sub

End Class
