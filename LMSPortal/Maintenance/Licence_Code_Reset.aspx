<%@ Page Title="LMSPortal - Licence Reset" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Licence_Code_Reset.aspx.vb" 
    EnableEventValidation="false"
    Inherits="Maintenance_Licence_Code_Reset" %>

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
                    <h4 class="card-title">Add Licence Reset Record</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Licence Code</label>
                                <asp:TextBox ID="TB_Licence_Code" runat="server" CssClass="form-control" placeholder="e.g. XXXX-XXXX-XXXX-XXXX-XXXX"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Licence_Code" runat="server" ValidationGroup="ValidateLicenceCodeReset"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Licence_Code">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegExValidator_TB_Licence_Code" runat="server" ValidationGroup="ValidateLicenceCodeReset"
                                    ErrorMessage="Invalid licence. Please enter a valid licence code." CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Licence_Code"
                                    ValidationExpression="^([A-Fa-f0-9]{4}-){4}[A-Fa-f0-9]{4}$|^[A-Fa-f0-9]{1,24}$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">MAC Address / UUID</label>
                                <asp:TextBox ID="TB_MAC_Address" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_MAC_Address" runat="server" ValidationGroup="ValidateLicenceCodeReset"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_MAC_Address">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegExValidator_TB_MAC_Address" runat="server" ValidationGroup="ValidateLicenceCodeReset"
                                    ErrorMessage="Invalid MAC address format. Please enter a valid MAC address." CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_MAC_Address"
                                    ValidationExpression="^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: text-top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <asp:Button ID="BN_Check" runat="server" Text="Check" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="ValidateLicenceCodeReset" />
                                <asp:Button ID="BN_Clear" runat="server" Text="Clear" CssClass="btn btn-default" CausesValidation="false" />
                                <hr />
                            </td>
                        </tr>
                        <tr id="LicenceValidityTr" style="vertical-align: text-top" runat="server" visible="false">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <div id="LicenceValidityDiv" runat="server">
                                    <asp:Label ID="LicenceValidity" runat="server"></asp:Label>
                                </div>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Licensee</label>
                                <asp:TextBox ID="TB_Customer_ID" runat="server" CssClass="form-control" ReadOnly="true" Enabled="false" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Customer_Name" runat="server" CssClass="form-control" ReadOnly="true" Enabled="false" Visible="true"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Remarks</label>
                                <asp:TextBox ID="TB_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Remarks" runat="server" ValidationGroup="LicenceCodeReset"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback" Enabled="false"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Remarks">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px">
                                <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="LicenceCodeReset" />
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
                            <asp:Label ID="LB_Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Name" runat="server" Text='<%# Eval("Name") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Licence_Code" runat="server" Text='<%# Eval("Licence_Code") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Licence_Code" runat="server" Text='<%# Eval("Licence_Code") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_MAC_Address" runat="server" Text='<%# Eval("MAC_Address") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_MAC_Address" runat="server" Text='<%# Eval("MAC_Address") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Reset_On" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd - hh:mm tt}", Eval("Reset_On")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Reset_On" runat="server" Text='<%# Eval("Reset_On") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Remarks" runat="server" Text='<%# Eval("Remarks") %>' CssClass="form-control"></asp:TextBox>
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

