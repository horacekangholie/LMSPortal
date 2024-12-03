<%@ Page Title="LMSPortal - Request SO No" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Request_SO_No.aspx.vb" 
    EnableEventValidation="false"
    Inherits="Maintenance_Request_SO_No" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <style>
        .CheckboxSelection {
            float: left;
            display: inline-block;
            padding-top: 1em;
        }

        .CheckboxSelection > input {
            width: 16px;
            height: 16px;
            margin-left: 5px;
        }

        .CheckboxSelection > span {
            display:inline-block;
            font-size: 16px;
            vertical-align: central;
            align-self:center;
            padding-left: 3px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Use Panel and set default button to trigger validation -->
    <asp:Panel ID="pnlSearchBox" runat="server" DefaultButton="BT_Search">
        <!-- Checkbox for display account with no device -->
        <div class="CheckboxSelection">
            <asp:CheckBox ID="chk_empty_so_no" runat="server" AutoPostBack="true" OnCheckedChanged="chk_empty_so_no_CheckedChanged" />
            <asp:Label ID="lb_empty_so_no" runat="server" Text="Orders SO No have not been recovered"></asp:Label>
        </div>

        <!-- Searchbox -->
        <div class="form-group form-inline" style="float: right">
            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div style="width: 100%; float: left">
        <!-- GridView -->
        <asp:GridView ID="GridView1" runat="server">
            <Columns>
                <asp:TemplateField HeaderText="Name" ItemStyle-Width="40%">
                    <ItemTemplate>
                        <asp:Label ID="LB_Name" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_E_Name" runat="server" Text='<%# Eval("Name") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Requested By" ItemStyle-Width="20%">
                    <ItemTemplate>
                        <asp:Label ID="LB_Requested_By" runat="server" Text='<%# Eval("Requested By") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_E_Requested_By" runat="server" Text='<%# Eval("Requested By") %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PO No" ItemStyle-Width="15%">
                    <ItemTemplate>
                        <asp:Label ID="LB_PO_No" runat="server" Text='<%# Eval("PO No") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_E_PO_No" runat="server" Text='<%# Eval("PO No") %>' CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_E_PO_No" runat="server"
                            ErrorMessage="PO No caanot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_E_PO_No">
                        </asp:RequiredFieldValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="PO Date" ItemStyle-Width="15%">
                    <ItemTemplate>
                        <asp:Label ID="LB_PO_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("PO Date")) %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_E_PO_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("PO Date")) %>' ReadOnly="true" Enabled="false" CssClass="form-control"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="SO No" ItemStyle-Width="10%">
                    <ItemTemplate>
                        <asp:Label ID="LB_SO_No" runat="server" Text='<%# Eval("SO No") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_E_SO_No" runat="server" Text='<%# Eval("SO No") %>' CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_E_SO_No" runat="server"
                            ErrorMessage="SO No caanot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_E_SO_No">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegEx_TB_E_SO_No" runat="server"
                            ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_E_SO_No"
                            ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                        </asp:RegularExpressionValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ItemStyle-Width="0%" ItemStyle-Wrap="false" ShowEditButton="true" ShowDeleteButton="false"
                    EditText="<span class='btn btn-xs btn-info'><i class='bi bi-pencil-fill'></i></span>"
                    DeleteText="<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>"
                    UpdateText="<span class='btn btn-xs btn-success'><i class='bi bi-save2'></i></span>"
                    CancelText="<span class='btn btn-xs btn-secondary'><i class='bi bi-x-lg'></i></span>" />
            </Columns>
        </asp:GridView>

        <!-- Display total number of records from the GridView -->
        <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>

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

