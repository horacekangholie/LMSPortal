Imports Microsoft.VisualBasic
Imports System
Imports System.Web.Configuration
Imports System.Collections.Generic
Imports System.Text
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Configuration
Imports System.Web.UI
Imports System.Security.Cryptography

'' import for NPOI
Imports NPOI.HSSF.UserModel
Imports NPOI.XSSF.UserModel   ' XSSF for Excel 2007 File (.xlsx)
Imports NPOI.SS.UserModel     ' Added since v.1.2.4
Imports NPOI.SS.Util


Public Class LMSPortalBaseCode
    Inherits System.Web.UI.Page

    ' Declarations
    Dim Conn As SqlConnection
    Dim Cmd As SqlCommand
    Dim Constr As String = ConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString

    Dim dr As SqlDataReader


    '' Page Render Events
    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If String.IsNullOrEmpty(Me.Title) OrElse Me.Title.Equals("Untitled Page", StringComparison.CurrentCultureIgnoreCase) Then
            Throw New Exception("Page title cannot be ""Untitled Page"" or an empty string.")
        End If

        ' Capture visited page and save to auditlog
        If Not IsPostBack Then
            If Request.QueryString("Module") <> "" Then
                AuditLog(Request.QueryString("Module"))
            ElseIf Request.QueryString("Customer_ID") <> "" Then
                Dim queryTable As String = IIf(Len(Session("User_Group")) > 0, "Maintenance_Customer", "Master_Customer")
                Dim customer_name As String = Get_Value("SELECT Name FROM " & queryTable & " WHERE Customer_ID = '" & Request.QueryString("Customer_ID") & "'", "Name")
                If (Not (Request.UrlReferrer) Is Nothing) Then
                    Dim previousPageName As String = Request.UrlReferrer.Segments((Request.UrlReferrer.Segments.Length - 1))
                    AuditLog(previousPageName & " - " & EscapeChar(customer_name))
                Else
                    AuditLog(EscapeChar(customer_name))
                End If
            End If
        End If
    End Sub


    '' Common function
    Protected Function SetFileName(ByVal Name As String, ByVal FileType As String) As String
        Dim FileName As String = Replace(Name, " ", "_")
        Select Case FileType
            Case "Excel"
                Return FileName & "_" & AppendDatetime() & ".xlsx"
            Case Else
                Return ""
        End Select
    End Function

    Protected Function AppendDatetime() As String
        Return Now.Year &
               Now.Month.ToString().PadLeft(2, "0") &
               Now.Day.ToString().PadLeft(2, "0") &
               "_" &
               Now.Hour.ToString().PadLeft(2, "0") &
               Now.Minute.ToString().PadLeft(2, "0") &
               Now.Second.ToString().PadLeft(2, "0")
    End Function

    ' Insert dashes into plain licence code
    Protected Function FormatLicenceCode(ByVal PreLicenceCode As String) As String
        PreLicenceCode = CleanSymbolInString(PreLicenceCode)
        Dim PostLicenceCode As String = Nothing
        Dim InsertPosition As Integer = 1
        For i = 0 To Len(PreLicenceCode) - 1
            PostLicenceCode = PostLicenceCode & CStr(PreLicenceCode(i)) & IIf(InsertPosition Mod 4 = 0 And InsertPosition <= 16, "-", "")
            InsertPosition = InsertPosition + 1
        Next
        Return PostLicenceCode
    End Function

    Protected Function FormatMACAddress(ByVal PreMAC As String) As String
        PreMAC = CleanSymbolInString(PreMAC)
        Dim PostMAC As String = Nothing
        Dim InsertPosition As Integer = 1
        For i = 0 To Len(PreMAC) - 1
            PostMAC = PostMAC & CStr(PreMAC(i)) & IIf(InsertPosition Mod 2 = 0 And InsertPosition <= 10, ":", "")
            InsertPosition = InsertPosition + 1
        Next
        Return PostMAC
    End Function

    Protected Function CleanSymbolInString(ByVal PreString As String) As String
        Dim symbolSeries As String() = {"-", ":"}
        Dim PostString As String = PreString
        For i = 0 To symbolSeries.Length - 1
            PostString = Replace(UCase(PostString), symbolSeries(i), "")
        Next
        Return PostString
    End Function

    Protected Sub AuditLog(ByVal eventName As String)
        Dim eventToLog As String = Replace(eventName, "_", " ")
        eventToLog = Replace(eventToLog, ".aspx", "")
        Dim success As Integer

        Try
            Dim sqlStr As String = "EXEC SP_AuditLog '" & eventToLog & "'" &
                                                  ", '" & Date.Now().ToString("yyyy-MM-dd HH:mm:ss") & "'" &
                                                  ", '" & Session("User_Name") & "'" &
                                                  ", '" & Session("User_Roles") & "'" &
                                                  ", '" & System.Net.Dns.GetHostName & "'"

            '' Do not capture admin's log
            If eventToLog <> "" Then
                If Session("User_Name") <> "Administrator" Then
                    success = RunSQL(sqlStr)
                End If
            End If

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
        End Try
    End Sub

    ' Execute SQL Command and return number of rows affected
    Protected Function RunSQL(ByVal sqlStr As String) As Integer
        Conn = New SqlConnection(Constr)
        Cmd = New SqlCommand(sqlStr, Conn)
        Conn.Open()
        Return Cmd.ExecuteNonQuery()
        If Conn.State = ConnectionState.Open Then
            Conn.Close()
            Conn.Dispose()    '' add dispose function
        End If
    End Function

    ' Execute SQL Command and return sqlDataReader
    Protected Function RunSQLExecuteReader(ByVal sqlStr As String) As SqlDataReader
        Conn = New SqlConnection(Constr)
        Cmd = New SqlCommand(sqlStr, Conn)
        Conn.Open()
        Return Cmd.ExecuteReader()
        If Conn.State = ConnectionState.Open Then   '' Add in
            Conn.Close()
            Conn.Dispose()   '' add dispose function
        End If
    End Function

    ' Excecute SQL and return DataSet
    Protected Function GetDataSet(ByVal sqlStr As String) As DataSet
        Using Conn As New SqlConnection(Constr)
            Using cmd As New SqlCommand(sqlStr)
                Using dbAdapter As New SqlDataAdapter()
                    cmd.Connection = Conn
                    dbAdapter.SelectCommand = cmd
                    Using ds As New DataSet()
                        dbAdapter.Fill(ds)
                        Return ds
                    End Using
                    cmd.Connection.Close()
                End Using
            End Using
            Conn.Close()
            Conn.Dispose()
        End Using
    End Function

    ' Excecute SQL and return DataTable
    Protected Function GetDataTable(ByVal sqlStr As String) As DataTable
        Try
            Using Conn As New SqlConnection(Constr)
                Using cmd As New SqlCommand(sqlStr)
                    Using dbAdapter As New SqlDataAdapter()
                        cmd.Connection = Conn
                        dbAdapter.SelectCommand = cmd
                        Using dt As New DataTable()
                            dbAdapter.Fill(dt)
                            Return dt
                        End Using
                        cmd.Connection.Close()
                    End Using
                End Using
                Conn.Close()
                Conn.Dispose()
            End Using
        Catch ex As Exception
            ' Log or rethrow the exception for debugging purposes
            Response.Write("Error in GetDataTable: " & ex.Message)
            Throw
        End Try
    End Function

    ' Common for Dropdownlist binding
    Protected Sub BindDropDownList(ByVal ddl As DropDownList, query As String, text As String, value As String, defaultText As String)
        Using Conn As New SqlConnection(Constr)
            Using cmd As New SqlCommand(query)
                Using dbAdapter As New SqlDataAdapter()
                    cmd.Connection = Conn
                    Conn.Open()
                    ddl.DataSource = cmd.ExecuteReader()
                    ddl.DataTextField = text
                    ddl.DataValueField = value
                    ddl.DataBind()
                    Conn.Close()
                End Using
            End Using
        End Using
        ddl.Items.Insert(0, New ListItem(defaultText, "0"))
    End Sub

    Protected Sub BindDropDownList_Custom_Default_Value(ByVal ddl As DropDownList, query As String, text As String, value As String, defaultText As String, defaultValue As String, isRebind As Boolean)
        Using Conn As New SqlConnection(Constr)
            Using cmd As New SqlCommand(query)
                Using dbAdapter As New SqlDataAdapter()
                    cmd.Connection = Conn
                    Conn.Open()
                    ddl.DataSource = cmd.ExecuteReader()
                    ddl.DataTextField = text
                    ddl.DataValueField = value
                    If isRebind Then
                        ddl.Items.Clear()
                        ddl.Items.Insert(0, New ListItem(defaultText, defaultValue))
                    End If
                    ddl.DataBind()
                    Conn.Close()
                End Using
            End Using
        End Using
    End Sub

    '' common procedure
    Protected Sub Shared_Dropdownlist_Bind(ByVal ddl As DropDownList, ByVal query As String, ByVal dataText As String, ByVal dataValue As String, ByVal defaultText As String, ByVal isRebind As Boolean)
        Using Conn As New SqlConnection(Constr)
            Using cmd As New SqlCommand(query)
                Using dbAdapter As New SqlDataAdapter()
                    cmd.Connection = Conn
                    Conn.Open()
                    ddl.DataSource = cmd.ExecuteReader()
                    ddl.DataTextField = dataText
                    ddl.DataValueField = dataValue
                    If isRebind Then
                        ddl.Items.Clear()
                        ddl.Items.Insert(0, New ListItem(defaultText, "-1"))
                    End If
                    ddl.DataBind()
                    Conn.Close()
                End Using
            End Using
        End Using
    End Sub


    ' Execute SQL Command to get a single value
    Protected Function Get_Value(ByVal query As String, ByVal ColName As String) As String
        Dim dr = RunSQLExecuteReader(query)
        Dim Value As String = ""
        While dr.Read()
            Value = dr(ColName)
        End While
        Cmd.Cancel()   '' Add in cancel function
        dr.Close()
        Conn.Dispose()
        Return Value
    End Function

    Protected Function AlertMessage(ByVal message As String) As String
        Dim sb As New StringBuilder()
        sb.Append("<SCRIPT LANGUAGE=""JavaScript"">alert('")
        sb.Append(message)
        sb.Append("');</SCRIPT>")
        Return sb.ToString()
    End Function

    Protected Function AlertMessageRedirect(ByVal message As String, ByVal posturl As String) As String
        Dim sb As New StringBuilder()
        sb.Append("<SCRIPT LANGUAGE='JavaScript'>")
        sb.Append("alert('" & message & "');")
        sb.Append("location.href='" & posturl & "';")
        sb.Append("</SCRIPT>")
        Return sb.ToString()
    End Function

    Protected Sub AlertMessageMsgBox(ByVal msg As String)
        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.Page.GetType(), "alert", "alert('" & msg & "');", True)
    End Sub


    'Protected Function GetPagePath(ByVal SubModuleName As String) As String
    '    Dim PagePath As String
    '    PagePath = Get_Value("SELECT * FROM DB_Access_Map WHERE Sub_Module = '" & SubModuleName & "'", "Path")   '' this part being called multiple time at once will slow down the system
    '    Return Replace(PagePath, "~", "")
    'End Function

    Protected Function Check_SQLInjection(ByVal Username As String, ByVal Password As String) As Integer
        Dim i As Integer = 1
        If (InStr(1, Username, "--") <> 0) Then Return 1
        If (InStr(1, Username, "1=1") <> 0) Then Return 1
        If (InStr(1, Password, "--") <> 0) Then Return 1
        If (InStr(1, Password, "1=1") <> 0) Then Return 1
        Return 0
    End Function

    Protected Function Encrypt(clearText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim clearBytes As Byte() = Encoding.Unicode.GetBytes(clearText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)
                    cs.Write(clearBytes, 0, clearBytes.Length)
                    cs.Close()
                End Using
                clearText = Convert.ToBase64String(ms.ToArray())
            End Using
        End Using
        Return clearText
    End Function

    Protected Function Decrypt(cipherText As String) As String
        Dim EncryptionKey As String = "MAKV2SPBNI99212"
        Dim cipherBytes As Byte() = Convert.FromBase64String(cipherText)
        Using encryptor As Aes = Aes.Create()
            Dim pdb As New Rfc2898DeriveBytes(EncryptionKey, New Byte() {&H49, &H76, &H61, &H6E, &H20, &H4D, &H65, &H64, &H76, &H65, &H64, &H65, &H76})
            encryptor.Key = pdb.GetBytes(32)
            encryptor.IV = pdb.GetBytes(16)
            Using ms As New MemoryStream()
                Using cs As New CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)
                    cs.Write(cipherBytes, 0, cipherBytes.Length)
                    cs.Close()
                End Using
                cipherText = Encoding.Unicode.GetString(ms.ToArray())
            End Using
        End Using
        Return cipherText
    End Function

    Protected Function DisplayPasswordMode(ByVal pwd As String) As String
        Dim PasswordMode As String = Nothing
        Dim StrArry As Char() = pwd.ToCharArray()
        For i = 0 To StrArry.Length - 1
            PasswordMode += "*"
        Next
        Return PasswordMode
    End Function

    Protected Function HidePartialFieldData(ByVal dataString As String) As String
        Dim fielddata As String = Nothing
        Dim StrArry As Char() = dataString.ToCharArray()
        For i = 0 To StrArry.Length - 1
            If i < StrArry.Length - 5 Then
                fielddata += "*"
            Else
                fielddata += StrArry(i)
            End If
        Next
        Return fielddata
    End Function

    Protected Function GenerateFTPPassword() As String
        'Dim xCharArray() As Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray
        Dim xCharArray() As Char = "ABCDEFGHJKMNOPQRSTUVWXYZ".ToCharArray   '' without I and L
        Dim xNoArray() As Char = "0123456789".ToCharArray
        Dim xGenerator As System.Random = New System.Random()
        Dim xStr As String = String.Empty

        While xStr.Length < 6
            If xGenerator.Next(0, 2) = 0 Then
                xStr &= xCharArray(xGenerator.Next(0, xCharArray.Length))
            Else
                xStr &= xNoArray(xGenerator.Next(0, xNoArray.Length))
            End If
        End While

        Return xStr.ToLower()
    End Function

    Protected Function EscapeChar(ByVal content As String) As String
        Dim EscChar As String() = {"'", "<", ">"}  ' add character to here
        For i = 0 To EscChar.Length - 1
            Select Case EscChar(i)
                Case "'"
                    content = Replace(content, EscChar(i), EscChar(i) & EscChar(i))  'escape by double the character
                Case "<", ">"
                    content = Replace(content, EscChar(i), "")
                Case Else
                    content = content
            End Select
        Next
        Return content
    End Function

    Protected Function EscapeCharForArticle(ByVal content As String) As String
        Dim EscChar As String() = {"'"}  ' add character to here
        For i = 0 To EscChar.Length - 1
            Select Case EscChar(i)
                Case "'"
                    content = Replace(content, EscChar(i), EscChar(i) & EscChar(i))  'escape by double the character
                Case Else
                    content = content
            End Select
        Next
        Return content
    End Function

    Protected Function RemoveHTMLWhiteSpace(ByVal content As String) As String
        Return content.Replace("&nbsp;", "")
    End Function

    Protected Function GetColumnIndexByName(ByVal row As GridViewRow, ByVal columnName As String) As Integer
        Dim columnIndex As Integer = 0
        For Each cell As DataControlFieldCell In row.Cells
            If TypeOf cell.ContainingField Is BoundField Then
                If (CType(cell.ContainingField, BoundField)).DataField.Equals(columnName) Then Exit For
            End If
            columnIndex += 1
        Next
        Return columnIndex
    End Function

    ' Function to return the column index by column name
    Public Shared Function GetColumnIndexByColumnName(gridView As GridView, columnName As String) As Integer
        For i As Integer = 0 To gridView.Columns.Count - 1
            ' Assuming you are using BoundField or similar for the column
            Dim column As DataControlField = gridView.Columns(i)
            If TypeOf column Is BoundField Then
                Dim boundField As BoundField = CType(column, BoundField)
                If String.Equals(boundField.HeaderText, columnName, StringComparison.OrdinalIgnoreCase) Then
                    Return i ' Column found
                End If
            End If
            ' Extend with additional checks here if you use different column types that might have headers
        Next
        Return -1 ' Column not found
    End Function

    Protected Function GetDataFieldNameByHeaderText(ByVal gv As GridView, ByVal headerText As String) As String
        Dim datafieldname As String = Nothing
        For Each column As DataControlField In gv.Columns
            If column.HeaderText = headerText Then
                If TypeOf column Is BoundField Then
                    datafieldname = DirectCast(column, BoundField).DataField
                End If
            End If
        Next
        Return datafieldname
    End Function

    Protected Function GetColumnHeader(ByVal ReportName As String) As String()
        ' Define the column name
        Dim ColName As String() = Nothing
        Select Case ReportName
            Case "DMC All Account"
                ColName = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Account Type", "Date Created", "End Date", "Status"}
            Case "DMC Billed Account"
                ColName = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "Period From", "Period To", "Subscription", "Currency", "Fee", "Status", "Account Type"}
            Case "DMC Trial Account"
                ColName = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "End Date", "Status", "Account Type"}
            Case "DMC Demo Account"
                ColName = {"Customer", "Group", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "Terminated On", "Status", "Account Type", "Last Used Date"}
            Case "DMC All Stores"
                ColName = {"Name", "HQ Code", "HQ Name", "Store Code", "Store Name", "Created Date", "End Date", "Status", "Account Type", "Last Used Date"}
            Case "DMC User Account"
                ColName = {"No", "Username", "Password", "Email", "Created Date", "Inactive Date", "Status", "Device Type", "Customer"}
            Case "DMC Store FTP Info"
                ColName = {"No", "Headquarter ID", "Name", "Store No", "Store Name", "Public IP", "FTP Host", "FTP User", "FTP Password", "Status", "Customer"}
            Case "Hardkey Licence"
                ColName = {"Customer", "Country", "PO No", "Date of PO", "Sales Order", "Date of SO", "Invoice", "Invoiced Date", "Prepared By", "Created Date", "Licence No", "Sales Representative", "Licence Tier"}
            Case "Term Licences"
                ColName = {"Licensee", "Licence Code", "Term", "Max HQ", "Max Store", "Created Date", "Activated Date", "Serial No", "MAC / UUID", "Status", "Expired Date", "Licence Type"}
            Case "Billed Items By Ref. Invoice"
                ColName = {"Ref. No", "Invoiced Date", "Invoice Bill To", "Customer", "Contract ID / Category", "Item Code", "Description", "Currency", "Amount"}
            Case "App and Product Licence"
                ColName = {"Licensee", "PO No", "Date of PO", "Invoice No", "Invoice Date", "Application Type", "OS Type", "Chargeable", "Licence Code", "MAC Address", "Created Date", "Activated Date", "Expired Date", "Status", "Requested By", "Remarks"}
            Case "DMC Monthly Subscription"
                ColName = {"Customer", "Headquarter", "Store", "Period Start", "Period End", "No of cycle", "Remaining cycle", "Base currency", "Monthly", "Cycle status"}
            Case "Licence Order"
                ColName = {"Licensee", "PO No", "PO Date", "Application Type", "OS Type", "Chargeable", "Licence Code", "Created Date", "Status", "Requested By", "Remarks"}
            Case "Sales Data Upload Report"
                ColName = {"Customer", "HQ Code", "HQ Name", "Store Code", "Store Name", "Banner", "Account Type", "Latest Sales Data"}
            Case "Module Licence Activation Keys"
                ColName = {"Licensee", "PO No", "Date of PO", "Invoice No", "Invoice Date", "Activation Key", "Created Date", "Activated Date", "MAC Address", "Status", "e.Sense", "BYOC", "AI"}
            Case "AI Device List"
                ColName = {"Status", "Expiry Date", "Device Serial", "Device ID", "Model", "Created On", "AI Soft. Ver.", "R Vers", "Scale SN", "Location", "MAC Address", "Prod. Licence No"}
        End Select
        Return ColName
    End Function

    Protected Function GetEndOfMonthDate(ByVal inputDate As Date) As Date
        Dim year As Integer = inputDate.Year
        Dim month As Integer = inputDate.Month
        Dim daysInMonth As Integer = Date.DaysInMonth(year, month)
        Dim endOfMonthDate As Date = New Date(year, month, daysInMonth)
        Return endOfMonthDate
    End Function

    Protected Function GetTimeDifference(ByVal FromTime As String, ByVal ToTime As String) As Integer
        Dim format As String = "HH:mm" ' 24-hour format
        Dim provider As System.Globalization.CultureInfo = System.Globalization.CultureInfo.InvariantCulture

        Try
            Dim downtimeFrom As DateTime = DateTime.ParseExact(FromTime, format, provider)
            Dim downtimeTo As DateTime = DateTime.ParseExact(ToTime, format, provider)

            If downtimeTo < downtimeFrom Then
                downtimeTo = downtimeTo.AddDays(1)
            End If

            Return CType((downtimeTo - downtimeFrom).TotalMinutes, Integer)
        Catch ex As Exception
            ' Optionally log the exception or handle it as deemed appropriate
            ' Return a default value indicating an error or uncomputable difference
            Return 0
        End Try
    End Function

    Protected Function ConvertMinutesToHHmm(totalMinutes As Integer) As String
        Dim hours As Integer = totalMinutes \ 60 ' Integer division to get hours
        Dim minutes As Integer = totalMinutes Mod 60 ' Modulus to get remaining minutes
        ' Format the hours and minutes to two digits and concatenate
        Return hours.ToString("D2") & ":" & minutes.ToString("D2")
    End Function

    Protected Function ConverDateFormat(ByVal dateStr As String) As String
        ' Normalize "Sept" to "Sep" to match the standard three-letter month format
        dateStr = dateStr.Replace("Sept ", "Sep ")

        Dim parsedDate As DateTime
        If DateTime.TryParseExact(dateStr, "dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, parsedDate) Then
            Return parsedDate.ToString("yyyy-MM-dd")
        Else
            Return dateStr ' Return original string if parsing fails
        End If
    End Function




    '' Class to create ItemTemplateField in Gridview
    Public Class SubscriptionNestedGridViewItemTemplate
        Implements ITemplate

        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            Dim expandImage As Image = New Image()
            expandImage.ID = "expandImage"
            expandImage.AlternateText = "Click to view subscribed store under this order"
            expandImage.Style.Add("cursor", "pointer")
            expandImage.Style.Add("width", "16px")
            expandImage.Style.Add("vertical-align", "top")
            expandImage.ImageUrl = "/Images/plus-square-solid-24.png"
            container.Controls.Add(expandImage)

            Dim pnChild As Panel = New Panel()
            pnChild.ID = "pnlContractStore"
            pnChild.Style.Add("display", "none")
            container.Controls.Add(pnChild)

            Dim gvChild As GridView = New GridView()
            gvChild.ID = "gvContractStore"
            gvChild.AutoGenerateColumns = False
            gvChild.ShowHeaderWhenEmpty = True
            gvChild.EmptyDataText = "No records found"
            gvChild.HeaderStyle.CssClass = "table-secondary"
            gvChild.Attributes.Add("width", "100%")
            gvChild.Style.Add("margin-bottom", "25px")

            Dim ColName() As String = {"Store_No", "Store_Name", "Banner", "Created_Date", "Start_Date", "End_Date", "Duration", "Currency", "Fee", "Type"}
            Dim ColData() As String = {"Store_No", "Store_Name", "Banner", "Created_Date", "Start_Date", "End_Date", "Duration", "Currency", "Fee", "Type"}
            Dim ColSize() As Integer = {100, 300, 200, 100, 100, 100, 100, 100, 100, 200}

            For i = 0 To ColName.Length - 1
                Dim Bfield As BoundField = New BoundField()
                Bfield.DataField = ColData(i)
                Bfield.HeaderText = Replace(ColName(i), "_", " ")
                If Bfield.HeaderText.Contains("Date") Then
                    Bfield.DataFormatString = "{0:dd MMM yy}"
                    Bfield.ItemStyle.Wrap = False
                End If
                Bfield.HeaderStyle.Width = ColSize(i)
                gvChild.Columns.Add(Bfield)
            Next
            pnChild.Controls.Add(gvChild)
        End Sub
    End Class

    Public Class HardkeyNestedGridViewItemTemplate
        Implements ITemplate

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim expandImage As Image = New Image()
            expandImage.ID = "expandImage"
            expandImage.AlternateText = "Click to view licence code under this order"
            expandImage.Style.Add("cursor", "pointer")
            expandImage.Style.Add("width", "16px")
            expandImage.Style.Add("vertical-align", "top")
            expandImage.ImageUrl = "/Images/plus-square-solid-24.png"
            container.Controls.Add(expandImage)

            Dim pnChild As Panel = New Panel()
            pnChild.ID = "pnlHardkeyLicence"
            pnChild.Style.Add("display", "none")
            container.Controls.Add(pnChild)

            Dim gvChild As GridView = New GridView()
            gvChild.ID = "gvHardkeyLicence"
            gvChild.AutoGenerateColumns = False
            gvChild.ShowHeaderWhenEmpty = True
            gvChild.EmptyDataText = "No records found"
            gvChild.HeaderStyle.CssClass = "table-secondary"
            gvChild.Attributes.Add("width", "100%")
            gvChild.Style.Add("margin-bottom", "25px")

            Dim ColName() As String = {"Licence No", "Created Date", "Licence Tier"}
            Dim ColData() As String = {"Licence_No", "Created_Date", "Licence_Tier"}
            Dim ColSize() As Integer = {30, 10, 30}

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
                Bfield.HeaderStyle.Wrap = False
                gvChild.Columns.Add(Bfield)
            Next
            pnChild.Controls.Add(gvChild)

        End Sub
    End Class

    Public Class LicenceNestedGridViewItemTemplate
        Implements ITemplate

        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            Dim expandImage As Image = New Image()
            expandImage.ID = "expandImage"
            expandImage.AlternateText = "Click to view licence code under this order"
            expandImage.Style.Add("cursor", "pointer")
            expandImage.Style.Add("width", "16px")
            expandImage.Style.Add("vertical-align", "top")
            expandImage.ImageUrl = "/Images/plus-square-solid-24.png"
            container.Controls.Add(expandImage)

            Dim pnChild As Panel = New Panel()
            pnChild.ID = "pnlLicenceList"
            pnChild.Style.Add("display", "none")
            container.Controls.Add(pnChild)

            Dim gvChild As GridView = New GridView()
            gvChild.ID = "gvLicenceList"
            gvChild.AutoGenerateColumns = False
            gvChild.ShowHeaderWhenEmpty = True
            gvChild.EmptyDataText = "No records found"
            gvChild.HeaderStyle.CssClass = "table-secondary"
            gvChild.Attributes.Add("width", "100%")
            gvChild.Style.Add("margin-bottom", "25px")

            Dim ColName() As String = {"Application Type", "OS Type", "Chargeable", "Created Date", "Licence Code", "MAC Address", "Email", "Status", "Activated Date", "Expired Date", "Requested By", "Remarks"}
            Dim ColData() As String = {"Application Type", "OS Type", "Chargeable", "Created Date", "Licence Code", "MAC Address", "Email", "Status", "Activated Date", "Expired Date", "Requested By", "Remarks"}
            'Dim ColSize() As Integer = {300, 100, 80, 100, 250, 120, 80, 120, 80, 200, 500}

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
                'Bfield.HeaderStyle.Width = ColSize(i)
                Bfield.HeaderStyle.Wrap = False
                gvChild.Columns.Add(Bfield)
            Next
            pnChild.Controls.Add(gvChild)
        End Sub
    End Class

    Public Class AIRenewalNestedGridViewItemTemplate
        Implements ITemplate

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim expandImage As Image = New Image()
            expandImage.ID = "expandImage"
            expandImage.AlternateText = "Click to view licence code under this order"
            expandImage.Style.Add("cursor", "pointer")
            expandImage.Style.Add("width", "16px")
            expandImage.Style.Add("vertical-align", "top")
            expandImage.ImageUrl = "/Images/plus-square-solid-24.png"
            container.Controls.Add(expandImage)

            Dim pnChild As Panel = New Panel()
            pnChild.ID = "pnlAILicenceList"
            pnChild.Style.Add("display", "none")
            container.Controls.Add(pnChild)

            Dim gvChild As GridView = New GridView()
            gvChild.ID = "gvAILicenceList"
            gvChild.AutoGenerateColumns = False
            gvChild.ShowHeaderWhenEmpty = True
            gvChild.EmptyDataText = "No records found"
            gvChild.HeaderStyle.CssClass = "table-secondary"
            gvChild.Attributes.Add("width", "100%")
            gvChild.Style.Add("margin-bottom", "25px")

            Dim ColName() As String = {"Licence Code", "Currency", "Fee", "Requested By", "Remarks"}
            Dim ColData() As String = {"Licence Code", "Currency", "Fee", "Requested By", "Remarks"}
            Dim ColSize() As Integer = {100, 50, 100, 100, 300}

            For i = 0 To ColName.Length - 1
                Dim Bfield As BoundField = New BoundField()
                Bfield.DataField = ColData(i)
                Bfield.HeaderText = ColName(i)
                If Bfield.HeaderText.Contains("Licence") Or Bfield.HeaderText.Contains("Remarks") Then
                    Bfield.ItemStyle.Wrap = False
                End If
                If Bfield.HeaderText.Contains("Date") Then
                    Bfield.DataFormatString = "{0:dd MMM yy}"
                End If
                If Bfield.HeaderText.Contains("Fee") Then
                    Bfield.DataFormatString = "{0:#,##0.00}"
                End If
                Bfield.HeaderStyle.Width = ColSize(i)
                Bfield.HeaderStyle.Wrap = False
                gvChild.Columns.Add(Bfield)
            Next
            pnChild.Controls.Add(gvChild)

        End Sub
    End Class

    Public Class TermedLicenceRenewalNestedGridViewItemTemplate
        Implements ITemplate

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Dim expandImage As Image = New Image()
            expandImage.ID = "expandImage"
            expandImage.AlternateText = "Click to view licence code under this order"
            expandImage.Style.Add("cursor", "pointer")
            expandImage.Style.Add("width", "16px")
            expandImage.Style.Add("vertical-align", "top")
            expandImage.ImageUrl = "/Images/plus-square-solid-24.png"
            container.Controls.Add(expandImage)

            Dim pnChild As Panel = New Panel()
            pnChild.ID = "pnlTermedLicenceList"
            pnChild.Style.Add("display", "none")
            container.Controls.Add(pnChild)

            Dim gvChild As GridView = New GridView()
            gvChild.ID = "gvTermedLicenceList"
            gvChild.AutoGenerateColumns = False
            gvChild.ShowHeaderWhenEmpty = True
            gvChild.EmptyDataText = "No records found"
            gvChild.HeaderStyle.CssClass = "table-secondary"
            gvChild.Attributes.Add("width", "100%")
            gvChild.Style.Add("margin-bottom", "25px")

            Dim ColName() As String = {"Licence Code", "Currency", "Fee", "Requested By", "Remarks"}
            Dim ColData() As String = {"Licence Code", "Currency", "Fee", "Requested By", "Remarks"}
            Dim ColSize() As Integer = {100, 50, 100, 100, 300}

            For i = 0 To ColName.Length - 1
                Dim Bfield As BoundField = New BoundField()
                Bfield.DataField = ColData(i)
                Bfield.HeaderText = ColName(i)
                If Bfield.HeaderText.Contains("Licence") Or Bfield.HeaderText.Contains("Remarks") Then
                    Bfield.ItemStyle.Wrap = False
                End If
                If Bfield.HeaderText.Contains("Date") Then
                    Bfield.DataFormatString = "{0:dd MMM yy}"
                End If
                If Bfield.HeaderText.Contains("Fee") Then
                    Bfield.DataFormatString = "{0:#,##0.00}"
                End If
                Bfield.HeaderStyle.Width = ColSize(i)
                Bfield.HeaderStyle.Wrap = False
                gvChild.Columns.Add(Bfield)
            Next
            pnChild.Controls.Add(gvChild)

        End Sub
    End Class


    '' For edit / delete control in last column of gridview
    Public Class GridViewItemTemplateControl
        Implements ITemplate

        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            '' First Control(0) in this template
            Dim FirstLinkButton As LinkButton = New LinkButton()
            FirstLinkButton.CausesValidation = False
            container.Controls.Add(FirstLinkButton)

            '' Additional control add here
            Dim SecondLinkButton As LinkButton = New LinkButton()
            SecondLinkButton.CausesValidation = False
            container.Controls.Add(SecondLinkButton)

            Dim ThirdLinkButton As LinkButton = New LinkButton()
            ThirdLinkButton.CausesValidation = False
            container.Controls.Add(ThirdLinkButton)
        End Sub
    End Class

    Public Class GridViewItemTemplateHyperlinkControl
        Implements ITemplate

        Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
            '' First Control(0) in this template
            Dim the_url As HyperLink = New HyperLink()
            container.Controls.Add(the_url)
        End Sub
    End Class

    Public Class CreateTemplateFieldContent
        Implements ITemplate

        Dim ItemType As ListItemType
        Dim FieldName As String

        Public Sub New(ByVal item_type As ListItemType, ByVal field_name As String)
            ItemType = item_type
            FieldName = field_name
        End Sub

        Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
            Select Case ItemType
                Case ListItemType.Item
                    Dim LB_Field As Label = New Label
                    LB_Field.ID = "Item_" & FieldName
                    LB_Field.Text = String.Empty
                    AddHandler LB_Field.DataBinding, AddressOf OnDataBinding
                    container.Controls.Add(LB_Field)
                Case ListItemType.EditItem
                    Dim TB_Field As TextBox = New TextBox()
                    TB_Field.ID = "EditItem_" & FieldName
                    TB_Field.Text = String.Empty
                    TB_Field.CssClass = "form-control"
                    AddHandler TB_Field.DataBinding, AddressOf OnDataBinding
                    container.Controls.Add(TB_Field)
            End Select
        End Sub

        Private Sub OnDataBinding(ByVal sender As Object, ByVal e As EventArgs)
            Dim bound_value_obj As Object = Nothing
            Dim ctrl As Control = CType(sender, Control)
            Dim data_item_container As IDataItemContainer = CType(ctrl.NamingContainer, IDataItemContainer)
            bound_value_obj = DataBinder.Eval(data_item_container.DataItem, FieldName)

            Select Case ItemType
                Case ListItemType.Item
                    Dim LB_Field As Label = CType(sender, Label)
                    LB_Field.Text = bound_value_obj.ToString()
                Case ListItemType.EditItem
                    Dim TB_Field As TextBox = CType(sender, TextBox)
                    TB_Field.Text = bound_value_obj.ToString()
            End Select
        End Sub

    End Class   '' Un-used



    '' File download related
    Protected Sub DownloadFile(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As Integer = Integer.Parse(TryCast(sender, LinkButton).CommandArgument)
        Dim bytes As Byte()
        Dim filename As String
        Dim contentType As String

        Using con As New SqlConnection(Constr)
            Using cmd As New SqlCommand()
                cmd.CommandText = " SELECT File_Name, Content_Type, File_Data FROM DB_FileUpload WHERE ID = @Id "
                cmd.Parameters.AddWithValue("@Id", id)
                cmd.Connection = con
                con.Open()
                Using sdr As SqlDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = DirectCast(sdr("File_Data"), Byte())
                    contentType = sdr("Content_Type").ToString()
                    filename = sdr("File_Name").ToString()
                End Using
                con.Close()
                con.Dispose()
            End Using
        End Using

        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=""" + filename + """")
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.End()
    End Sub

    Protected Function DownloadExcel(ByVal ReportSQL As String, ByVal ReportName As String, ByVal FileType As String, ByVal Format As String) As Boolean
        Dim success As Boolean = False

        ' Determine the format to generate the excel
        Select Case Format
            Case "General"
                success = GeneralFormat(ReportSQL, ReportName, FileType)
            Case "Excel Reports"
                success = ExcelReport(ReportSQL, ReportName, FileType)
        End Select

        Return success
    End Function



    ' Generate Excel Reports
    Protected Function GeneralFormat(ByVal ReportSQL As String, ByVal ReportName As String, ByVal FileType As String) As Boolean
        Response.Clear()
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

        Dim Generated As Boolean
        Dim oWorkBook As XSSFWorkbook = New XSSFWorkbook()
        Dim oSheet As ISheet = oWorkBook.CreateSheet(ReportName)  ' Create SpreadSheet

        oSheet.ProtectSheet("123123")  ' Protect the Sheet with password

        '  ADO.NET Starts
        Conn = New SqlConnection(Constr)
        dr = Nothing
        Cmd = New SqlCommand(ReportSQL, Conn)

        Try
            Conn.Open()   ' Connect and open database
            dr = Cmd.ExecuteReader()   ' Retrieve data

            Dim k As Integer = 1
            While dr.Read()
                Dim u_Row As IRow = oSheet.CreateRow(k)
                For i = 0 To (dr.FieldCount - 1)   ' FieldCount refer to total number of column of each row
                    If ReportName = "Module Licence Activation Keys" And i >= 10 Then
                        '' For report to insert tick symbol
                        If CInt(dr.GetValue(i).ToString()) > 0 Then
                            Dim tick As String = Nothing
                            For j = 1 To CInt(dr.GetValue(i).ToString())
                                tick += "✓"
                            Next
                            u_Row.CreateCell(i).SetCellValue(tick)
                        Else
                            u_Row.CreateCell(i).SetCellValue("")
                        End If
                    Else
                        u_Row.CreateCell(i).SetCellValue(dr.GetValue(i).ToString())
                        oSheet.AutoSizeColumn(i)   ' Set each column autosizeremo
                    End If
                Next
                k = k + 1
            End While

            ' Create column header on first row
            Dim ColHeader As IRow = oSheet.CreateRow(0)
            Dim ColName() As String = GetColumnHeader(ReportName)

            ' Write data to cells
            For i = 0 To ColName.Length - 1
                ColHeader.CreateCell(i).SetCellValue(ColName(i))
                oSheet.AutoSizeColumn(i)
            Next

            Generated = True
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message)
            Generated = False
        Finally
            If Not (dr Is Nothing) Then
                Cmd.Cancel()
                dr.Close()
            End If
            If (Conn.State = ConnectionState.Open) Then
                Conn.Close()
                Conn.Dispose()
            End If
        End Try
        ' ADO.NET Ends

        ' Export Excel File
        Dim MS As MemoryStream = New MemoryStream()   ' This required System.IO namespace
        oWorkBook.Write(MS)

        ' Name the excel file
        If ReportName <> "Licence Order" Then
            Response.AddHeader("Content-Disposition", "attachment; filename=" & SetFileName(ReportName, FileType))
        Else
            Dim Query As String = Replace(ReportSQL, " ORDER BY [PO Date] DESC, [Application Type] ", "")  '' Redefine the SQL
            Dim LatestPO As String = Get_Value(Query, "PO No")
            Dim LatestLicensee As String = Get_Value(Query, "Licensee")
            Dim LatestCreatedDate As String = CDate(Get_Value(Query, "Created Date")).ToString("yyyy-MM-dd")
            Response.AddHeader("Content-Disposition", "attachment; filename=" & EscapeChar(LatestLicensee) & "_" & LatestPO & "_" & LatestCreatedDate)
        End If
        Response.BinaryWrite(MS.ToArray())

        ' Release object
        oWorkBook = Nothing
        MS.Close()
        MS.Dispose()

        ' Without the following 2 lines, output excel file will corrupt
        Response.Flush()
        Response.End()

        Return Generated
    End Function

    Protected Function ExcelReport(ByVal ReportSQL As String, ByVal ReportName As String, ByVal FileType As String) As Boolean
        Response.Clear()
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

        Dim Generated As Boolean
        Dim oWorkbook As XSSFWorkbook = New XSSFWorkbook()

        '' Report Title Cell Style
        Dim ReportTitleFont As XSSFFont = CType(oWorkbook.CreateFont(), XSSFFont)
        Dim ReportTitleCellStyle As XSSFCellStyle = CType(oWorkbook.CreateCellStyle(), XSSFCellStyle)
        ReportTitleFont.FontHeightInPoints = 14
        ReportTitleFont.IsBold = True
        ReportTitleCellStyle.SetFont(ReportTitleFont)


        '' Report Column Header Cell Style
        Dim ReportHeaderRowFont As XSSFFont = CType(oWorkbook.CreateFont(), XSSFFont)
        Dim ReportHeaderCellStyle As XSSFCellStyle = CType(oWorkbook.CreateCellStyle(), XSSFCellStyle)
        Dim HeaderBackgroundColor As XSSFColor = New XSSFColor()
        Dim BackGroundColorRGB As Byte() = {CByte(184), CByte(218), CByte(255)}

        HeaderBackgroundColor.SetRgb(BackGroundColorRGB)
        ReportHeaderCellStyle.FillForegroundColorColor = HeaderBackgroundColor
        ReportHeaderCellStyle.FillPattern = FillPattern.SolidForeground
        ReportHeaderRowFont.IsBold = True
        ReportHeaderRowFont.Color = IndexedColors.Black.Index
        ReportHeaderCellStyle.BorderTop = BorderStyle.Thin
        ReportHeaderCellStyle.BorderRight = BorderStyle.Thin
        ReportHeaderCellStyle.BorderBottom = BorderStyle.Thin
        ReportHeaderCellStyle.BorderLeft = BorderStyle.Thin
        ReportHeaderCellStyle.SetFont(ReportHeaderRowFont)


        '' Report Data Row Cell Style
        Dim DataRowFont As XSSFFont = CType(oWorkbook.CreateFont(), XSSFFont)
        Dim DataRowCellStyle As XSSFCellStyle = CType(oWorkbook.CreateCellStyle(), XSSFCellStyle)
        DataRowCellStyle.SetFont(DataRowFont)
        DataRowCellStyle.Alignment = HorizontalAlignment.Left

        DataRowCellStyle.BorderTop = BorderStyle.Thin
        DataRowCellStyle.BorderRight = BorderStyle.Thin
        DataRowCellStyle.BorderBottom = BorderStyle.Thin
        DataRowCellStyle.BorderLeft = BorderStyle.Thin

        '' Report Footer Cell Style
        Dim ReportFooterFont As XSSFFont = CType(oWorkbook.CreateFont(), XSSFFont)
        Dim ReportFooterCellStyle As XSSFCellStyle = CType(oWorkbook.CreateCellStyle(), XSSFCellStyle)
        ReportFooterFont.IsItalic = True
        ReportFooterCellStyle.SetFont(ReportFooterFont)

        Dim oSheet As XSSFSheet

        Dim ReportSheetName As String = Get_Value("SELECT SheetName FROM Excel_Reports_List WHERE Report_Name = '" & ReportName & "'", "SheetName")
        Dim Sheet_Name() As String = Nothing
        Sheet_Name = Split(ReportSheetName, ";")

        Dim sqlStr() As String = Nothing
        sqlStr = Split(ReportSQL, ";")

        For idx = 0 To (Sheet_Name.Length - 1)
            Dim Conn As SqlConnection = New SqlConnection(WebConfigurationManager.ConnectionStrings("lmsConnectionString").ConnectionString)
            Dim dr As SqlDataReader = Nothing
            Dim cmd As SqlCommand

            If Trim(Sheet_Name(idx)) = "Current Year" Then
                oSheet = oWorkbook.CreateSheet("Year " & Now.Year.ToString)
            ElseIf Trim(Sheet_Name(idx)) = "Previous Year" Then
                oSheet = oWorkbook.CreateSheet("Year " & (Now.Year - 1).ToString)
            Else
                oSheet = oWorkbook.CreateSheet(Sheet_Name(idx))
            End If

            cmd = New SqlCommand(sqlStr(idx), Conn)

            Try
                Conn.Open()
                dr = cmd.ExecuteReader()

                ' Insert value to cell
                Dim HeaderRowIndex As Integer = 2
                Dim DataRowIndex As Integer = 3
                Dim ReportWidth As Integer

                While dr.Read()

                    ' Write Column Header Row
                    Dim HeaderRow As XSSFRow = oSheet.CreateRow(HeaderRowIndex)
                    For i = 0 To dr.FieldCount - 1
                        CreateCell(HeaderRow, i, dr.GetName(i).GetType().ToString(), dr.GetName(i), ReportHeaderCellStyle)
                        oSheet.AutoSizeColumn(i)
                    Next

                    ' Write Data Row
                    Dim DataRow As IRow = oSheet.CreateRow(DataRowIndex)
                    For i = 0 To (dr.FieldCount - 1)
                        '' Insert data based on data type
                        CreateCell(DataRow, i, dr.GetValue(i).GetType().ToString(), dr.GetValue(i).ToString(), DataRowCellStyle)
                        If dr.GetValue(i).GetType().ToString() = "System.Int32" Then
                            oSheet.SetColumnWidth(i, 4500)
                        Else
                            oSheet.AutoSizeColumn(i)
                        End If
                    Next
                    DataRowIndex = DataRowIndex + 1
                    ReportWidth = dr.FieldCount - 1   '' Determine the colspan for Report title

                End While

                '' At least 1 column width in excel
                ReportWidth = IIf(ReportWidth <= 0, 1, ReportWidth)

                ' Report Title
                Dim ReportTitle As IRow
                Dim TitleRowIndex As Integer = 0
                Dim ReportTitleMergedRange As CellRangeAddress = New CellRangeAddress(0, 0, 0, ReportWidth)   '' Merge cell based on the report width
                ReportTitle = oSheet.CreateRow(0)
                oSheet.AddMergedRegion(ReportTitleMergedRange)
                CreateCell(ReportTitle, 0, Sheet_Name(idx).GetType().ToString(), Trim(Sheet_Name(idx)), ReportTitleCellStyle)

                ' Report Footer
                Dim ReportFooter As IRow
                Dim FooterRowIndex As Integer = DataRowIndex + 3   '' Give 3 rows gap before footer started
                Dim ReportFooterMergedRange As CellRangeAddress = New CellRangeAddress(FooterRowIndex, FooterRowIndex, 0, ReportWidth)
                Dim FooterNote As String = "Generated on " & Now.Day & " " & MonthName(Now.Month, True) & " " & Now.Year & " " & Now.Hour & ":" & Now.Minute & ":" & Now.Second
                ReportFooter = oSheet.CreateRow(FooterRowIndex)
                oSheet.AddMergedRegion(ReportFooterMergedRange)
                CreateCell(ReportFooter, 0, FooterNote.GetType().ToString(), FooterNote, ReportFooterCellStyle)

                Generated = True

            Catch ex As Exception
                Response.Write("ERROR: " & ex.Message)
                Generated = False

            Finally
                If Not (dr Is Nothing) Then
                    cmd.Cancel()
                    dr.Close()
                End If
                If (Conn.State = ConnectionState.Open) Then
                    Conn.Close()
                    Conn.Dispose()
                End If
            End Try

        Next

        ' Export Excel File
        Dim MS As MemoryStream = New MemoryStream()   ' This required System.IO namespace
        oWorkbook.Write(MS)

        ' Name the excel file
        Response.AddHeader("Content-Disposition", "attachment; filename=" & SetFileName(ReportName, FileType))
        Response.BinaryWrite(MS.ToArray())

        ' Release object
        oWorkbook = Nothing
        MS.Close()
        MS.Dispose()

        ' Without the following 2 lines, output excel file will corrupt
        Response.Flush()
        Response.End()

        Return Generated
    End Function



    '' Function to create cell
    Private Sub CreateCell(ByVal CurrentRow As IRow, ByVal CellIndex As Integer, ByVal ValueDataType As String, ByVal Value As String, ByVal Style As XSSFCellStyle)
        Dim Cell As ICell = CurrentRow.CreateCell(CellIndex)
        Value = IIf(Value Is Nothing, "", Value)
        Select Case ValueDataType
            Case "System.Int32"
                Cell.SetCellValue(Integer.Parse(Value))
            Case "System.Decimal"
                Cell.SetCellValue(Decimal.Parse(Value).ToString("#,0.00"))
            Case Else
                Cell.SetCellValue(Value)
        End Select
        Cell.CellStyle = Style
    End Sub

End Class
