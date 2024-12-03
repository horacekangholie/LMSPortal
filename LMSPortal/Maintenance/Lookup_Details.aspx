<%@ Page Title="LMSPortal - Lookup Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Lookup_Details.aspx.vb" 
    ValidateRequest="false"
    Inherits="Maintenance_Lookup_Details" %>

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
                    <h4 class="card-title">Add Lookup Details</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: bottom">
                            <td style="width: 50%; padding: 20px 5px 0px 0px" colspan="2">
                                <label>Lookup Name</label>
                                <asp:TextBox ID="TB_Lookup_Name" runat="server" CssClass="form-control disabled" Enabled="false"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <label>Value 1</label>
                                <asp:TextBox ID="TB_Value_1" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Value_1" runat="server" ValidationGroup="DBLookup"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Value_1">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <label>Value 2</label>
                                <asp:TextBox ID="TB_Value_2" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <label>Value 3</label>
                                <asp:TextBox ID="TB_Value_3" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <label>Value 4</label>
                                <asp:TextBox ID="TB_Value_4" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="DBLookup" />
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
                            <asp:Label ID="LB_Lookup_Name" runat="server" Text='<%# Eval("Lookup_Name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Lookup_Name" runat="server" Text='<%# Eval("Lookup_Name") %>' ReadOnly="true" Enabled="false" CssClass="form-control disabled"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Value_1" runat="server" Text='<%# Eval("Value_1") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Value_1" runat="server" Text='<%# Eval("Value_1") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Value_2" runat="server" Text='<%# Eval("Value_2") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Value_2" runat="server" Text='<%# Eval("Value_2") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Value_3" runat="server" Text='<%# Eval("Value_3") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Value_3" runat="server" Text='<%# Eval("Value_3") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Value_4" runat="server" Text='<%# Eval("Value_4") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Value_4" runat="server" Text='<%# Eval("Value_4") %>' CssClass="form-control"></asp:TextBox>
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

    <div style="position: fixed; bottom: 0; margin-bottom: 30px; width: 100%; padding-right: 14%">
        <!-- Control Button -->
        <hr />
        <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>
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

