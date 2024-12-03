<%@ Page Title="LMSPortal - Store Name Change" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Store_Name_Change.aspx.vb" 
    ValidateRequest="false"
    Inherits="Maintenance_Store_Name_Change" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
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

    <div style="width: 100%; float: right">
        <div style="width: 30%; float: left">
            <div class="card border-light mb-3" style="width: 95%">
                <div class="card-header">
                    <h4 class="card-title">Store Name Change</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>Headquarter</label>
                                <asp:DropDownList ID="DDL_Headquarter" runat="server"
                                    AppendDataBoundItems="true"
                                    AutoPostBack="true"
                                    CssClass="form-control">
                                    <asp:ListItem Text="Please select" Value="empty" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_DDL_Headquarter" runat="server" ValidationGroup="StoreNameChange"
                                    ErrorMessage="Please select a headquarter" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    Operator="NotEqual" ValueToCompare="empty"
                                    ControlToValidate="DDL_Headquarter" Type="String">
                                </asp:CompareValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>Store</label>
                                <asp:DropDownList ID="DDL_Store" runat="server"
                                    AppendDataBoundItems="true" 
                                    AutoPostBack="true"
                                    CssClass="form-control">
                                    <asp:ListItem Text="Please select" Value="empty" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_DDL_Store" runat="server" ValidationGroup="StoreNameChange"
                                    ErrorMessage="Please select a store" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    Operator="NotEqual" ValueToCompare="empty"
                                    ControlToValidate="DDL_Store" Type="String">
                                </asp:CompareValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <asp:TextBox ID="TB_Store_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <label>Banner</label>
                                <asp:TextBox ID="TB_Banner" runat="server" CssClass="form-control" ReadOnly="true" Enabled="false"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 0px 0px 0px 0px">
                                <hr />
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 5px 5px 0px 0px">
                                <label>New Store Name</label>
                                <asp:TextBox ID="TB_New_Store_Name" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_New_Store_Name" runat="server" ValidationGroup="StoreNameChange"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_New_Store_Name">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>New Banner Name</label>
                                <asp:TextBox ID="TB_New_Banner_Name" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_New_Banner_Name" runat="server" ValidationGroup="StoreNameChange"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_New_Banner_Name">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>Effective Date</label>
                                <asp:TextBox ID="TB_Effective_Date" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Effective_Date" runat="server" ValidationGroup="StoreNameChange"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Effective_Date">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <asp:Button ID="BN_AddNewChange" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="StoreNameChange" />
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
                            <asp:Label ID="LB_Store_ID" runat="server" Text='<%# Eval("Store_ID") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Store_ID" runat="server" Text='<%# Eval("Store_ID") %>' ReadOnly="true" Enabled="false" CssClass="form-control disabled"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Old_Store_Name" runat="server" Text='<%# Eval("Old_Store_Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Old_Store_Name" runat="server" Text='<%# Eval("Old_Store_Name") %>' ReadOnly="true" Enabled="false" CssClass="form-control disabled"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Old_Banner_Name" runat="server" Text='<%# Eval("Old_Banner_Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Old_Banner_Name" runat="server" Text='<%# Eval("Old_Banner_Name") %>' ReadOnly="true" Enabled="false" CssClass="form-control disabled"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_New_Store_Name" runat="server" Text='<%# Eval("New_Store_Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_New_Store_Name" runat="server" Text='<%# Eval("New_Store_Name") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_New_Banner_Name" runat="server" Text='<%# Eval("New_Banner_Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_New_Banner_Name" runat="server" Text='<%# Eval("New_Banner_Name") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Effective_Date" runat="server" Text='<%# Eval("Effective_Date") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Effective_Date" runat="server" Text='<%# Eval("Effective_Date") %>' TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true"
                        EditText="<span class='btn btn-xs btn-info'><i class='bi bi-pencil-fill'></i></span>" 
                        DeleteText="<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>"
                        UpdateText="<span class='btn btn-xs btn-success'><i class='bi bi-save2'></i></span>"
                        CancelText="<span class='btn btn-xs btn-secondary'><i class='bi bi-x-lg'></i></span>" />
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

