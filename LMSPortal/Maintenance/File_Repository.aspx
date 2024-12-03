<%@ Page Title="LMSPortal - File Repository" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="File_Repository.aspx.vb" 
    ValidateRequest="false"
    Inherits="Maintenance_File_Repository" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Use Panel and set default button to trigger validation -->
    <asp:Panel ID="pnlSearchBox" runat="server" DefaultButton="BT_Search">
        <!-- Searchbox -->
        <div class="form-group form-inline" style="float: right">
            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" CausesValidation="false" />
        </div>
    </asp:Panel>

    <!-- Upload Section -->
    <div style="width: 100%; float: right">
        <div style="width: 30%; float: left">
            <div class="card border-light mb-3" style="width: 95%">
                <div class="card-header">
                    <h4 class="card-title">File Upload</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <asp:FileUpload ID="FileUpload1" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredField_FileUpload1" runat="server" ValidationGroup="FileUpload"
                                    ErrorMessage="Please select a file to upload" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="FileUpload1">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 80%; padding: 20px 5px 20px 0px">
                                <asp:DropDownList ID="DDL_Doc_Category" runat="server"
                                    DataSourceID="SqlDataSource_DDL_Doc_Category"
                                    DataTextField="Doc_Category"
                                    DataValueField="Doc_Category"
                                    CssClass="form-control">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource_DDL_Doc_Category" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Doc_Category FROM DB_Lookup WHERE Lookup_Name = 'Document Type' ORDER BY Value_2"></asp:SqlDataSource>
                                <asp:CompareValidator ID="RequiredField_DDL_Doc_Category" runat="server" ValidationGroup="FileUpload"
                                    ErrorMessage="Please select a category" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    Operator="NotEqual" ValueToCompare="Please select"
                                    ControlToValidate="DDL_Doc_Category" Type="String">
                                </asp:CompareValidator>
                            </td>
                            <td style="width: 20%; padding: 20px 5px 20px 0px">
                                <asp:Button ID="BN_Upload" runat="server" Text="Upload" CssClass="form-control btn btn-default w-100" CausesValidation="true" ValidationGroup="FileUpload" />
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="padding: 50px 5px 10px 0px" align="center" colspan="2">
                                <asp:Button ID="BN_DownloadAll" runat="server" Text="Export All" CssClass="btn btn-default w-50" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div style="margin-left: 30%">
            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_File_Name" runat="server" Text='<%# Eval("File_Name") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Uploaded_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Uploaded_Date")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Content_Type" runat="server" Text='<%# Eval("Content_Type") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Category" runat="server" Text='<%# Eval("Category") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDownload" runat="server" Text="<span class='btn btn-xs btn-info'><i class='bi bi-cloud-download-fill'></i></span>" OnClick="DownloadFile" OnClientClick="document.forms[0].target='_blank';" CommandArgument='<%# Eval("ID") %>'></asp:LinkButton>
                            <asp:LinkButton ID="lnkDelete" runat="server" Text="<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>" OnClick="DeleteFile" CommandArgument='<%# Eval("ID") %>' OnClientClick="return confirm('Are you sure to delete record?')"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </div>
    </div>


    <!-- javascript for GridView Row highlight effect -->
    <script type="text/javascript">
        var OrigRowColor;
        function SetMouseOver(element) {
            OrigRowColor = element.style.backgroundColor;
            element.style.backgroundColor = '#f5f5f5';
            element.style.cursor = 'default';
        }
        function SetMouseOut(element) {
            element.style.backgroundColor = OrigRowColor;
        }
    </script>

</asp:Content>

