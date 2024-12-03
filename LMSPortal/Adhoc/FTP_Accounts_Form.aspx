<%@ Page Title="LMSPortal - FTP User Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="FTP_Accounts_Form.aspx.vb" 
    Inherits="Adhoc_FTP_Accounts_Form" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" />
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <script src="../Scripts/jquery.blockUI.js" type="text/javascript"></script>

    <script type="text/javascript">
        function BlockUI(elementID) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                $("#" + elementID).block({
                    message: '<table align = "center"><tr><td>' +
                        '<img src="../Images/loadingAnim.gif"/></td></tr></table>',
                    css: {},
                    overlayCSS: {
                        backgroundColor: '#000000', opacity: 0.6
                    }
                });
            });
            prm.add_endRequest(function () {
                $("#" + elementID).unblock();
            });
        }
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditFTPUser.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditAccessFolder.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        function Hidepopup() {
            $find("popup").hide();
            return false;
        }
    </script>

    <style>
        .modalBackground {
            display: block;
            background-color: Black;
            filter: blur(0px);
            opacity: 0.8;
        }

        .modalPopup {
            position: absolute;
            background-color: #FFFFFF;
            border-radius: 5px;
            padding: 0px;
            width:800px;
            height: auto;
        }

        .modalPopupTable {
            display: block;
            padding: 0px 10px 20px 20px;
            table-layout: fixed;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Form Page Content -->
    <div class="card border-light mb-3">
        <div class="card-header">
            <h4 class="card-title">Customer Details</h4>
        </div>
        <div class="card-body">
            <asp:FormView ID="FormView1" runat="server" Width="100%">
                <EditItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 20%; padding: 0px 10px 20px 0px">
                                <label>Code</label>
                                <asp:TextBox ID="TB_Code" Text='<%# Eval("Code") %>' runat="server" CssClass="form-control" MaxLength="4" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Code" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Code">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_E_TB_Code" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="Code should be numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Code"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 50%; padding: 0px 10px 20px 0px">
                                <label>Distributor</label>
                                <asp:TextBox ID="TB_Distributor" Text='<%# Eval("Distributor") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Distributor" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Distributor">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Country</label>
                                <asp:TextBox ID="TB_Country" Text='<%# Eval("Country") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Country" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Country">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Currency</label>
                                <asp:TextBox ID="TB_Currency" Text='<%# Eval("Currency") %>' runat="server" CssClass="form-control" MaxLength="3" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Currency" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Currency">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Company Abbreviation</label>
                                <asp:TextBox ID="TB_Company_Abbreviation" Text='<%# Eval("COY_ABBR") %>' runat="server" CssClass="form-control" MaxLength="3" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Company_Abbreviation" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Company_Abbreviation">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Region</label>
                                <asp:TextBox ID="TB_Region" Text='<%# Eval("Region") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Region" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Region">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Tech Support Representative</label>
                                <asp:TextBox ID="TB_TS_Rep" Text='<%# Eval("TS_Rep") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_TS_Rep" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_TS_Rep">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Marketing Representative</label>
                                <asp:TextBox ID="TB_MKT_Rep" Text='<%# Eval("MKT_Rep") %>' runat="server" CssClass="form-control" />
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Nature</label>
                                <asp:TextBox ID="TB_Nature" Text='<%# Eval("Nature") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Nature" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Nature">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 20%; padding: 0px 10px 20px 0px">
                                <label>Code</label>
                                <asp:TextBox ID="TB_Code" Text='<%# Eval("Code") %>' runat="server" CssClass="form-control" OnTextChanged="TB_Code_TextChanged" AutoPostBack="true" MaxLength="4" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Code" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Code">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_I_TB_Code" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="Code should be numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Code"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 50%; padding: 0px 10px 20px 0px">
                                <label>Distributor</label>
                                <asp:TextBox ID="TB_Distributor" Text='<%# Eval("Distributor") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Distributor" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Distributor">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Country</label>
                                <asp:TextBox ID="TB_Country" Text='<%# Eval("Country") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Country" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Country">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Currency</label>
                                <asp:TextBox ID="TB_Currency" Text='<%# Eval("Currency") %>' runat="server" CssClass="form-control" MaxLength="3" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Currency" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Currency">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Company Abbreviation</label>
                                <asp:TextBox ID="TB_Company_Abbreviation" Text='<%# Eval("COY_ABBR") %>' runat="server" CssClass="form-control" MaxLength="3" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Company_Abbreviation" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Company_Abbreviation">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Region</label>
                                <asp:TextBox ID="TB_Region" Text='<%# Eval("Region") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Region" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Region">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Tech Support Representative</label>
                                <asp:TextBox ID="TB_TS_Rep" Text='<%# Eval("TS_Rep") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_TS_Rep" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_TS_Rep">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 35%; padding: 0px 10px 20px 0px">
                                <label>Marketing Representative</label>
                                <asp:TextBox ID="TB_MKT_Rep" Text='<%# Eval("MKT_Rep") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_MKT_Rep" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_MKT_Rep">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 20px 0px">
                                <label>Nature</label>
                                <asp:TextBox ID="TB_Nature" Text='<%# Eval("Nature") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Nature" runat="server" ValidationGroup="Distributor_FTP_Account"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Nature">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </InsertItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <hr />

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- FTP Users -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="card border-light mb-3" style="width: 60%; float: left">
                <div class="card-header">
                    <h4 class="card-title">FTP Users</h4>
                </div>
                <div class="card-body">
                    <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                    <asp:Button ID="btnAddFTPUser" Text="Add FTP User" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                </div>
            </div>

            <!-- Modal body -->
            <asp:Panel ID="pnlAddEditFTPUser" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px">
                    <asp:Label ID="ModalHeaderFTPUser" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 30%; padding: 0px 10px 20px 0px">
                            <label>User Group</label>
                            <asp:DropDownList ID="DDL_User_Group" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                        </td>
                        <td style="width: 70%; padding: 0px 10px 20px 0px">
                            <label>Contact Person</label>
                            <asp:TextBox ID="TB_Contact_Person" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Contact_Person" runat="server" ValidationGroup="FTPUser"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Contact_Person">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 800px; padding: 0px 10px 20px 0px" colspan="2">
                            <label>Email</label>
                            <asp:TextBox ID="TB_Email" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Email" runat="server" ValidationGroup="FTPUser"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Email">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegEx_TB_Email" runat="server" ValidationGroup="FTPUser"
                                ErrorMessage="Invalid email address" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="true"
                                ControlToValidate="TB_Email"
                                ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" />
                        </td>
                    </tr>
                </table>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 35%; padding: 0px 10px 20px 0px">
                            <label>FTP User</label>
                            <asp:TextBox ID="TB_FTP_User" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_FTP_User" runat="server" ValidationGroup="FTPUser"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_FTP_User">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 35%; padding: 0px 10px 20px 0px">
                            <label>FTP Password</label>
                            <asp:TextBox ID="TB_FTP_Password" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_FTP_Password" runat="server" ValidationGroup="FTPUser"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_FTP_Password">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 30%; padding: 0px 10px 20px 0px">
                            <label>Status</label>
                            <asp:DropDownList ID="DDL_Status" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Active" Value="1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                            <asp:Button ID="btnSaveFTPUser" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="FTPUser" />
                            <asp:Button ID="btnCancelFTPUser" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeFTPUser" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupFTPUser" runat="server" DropShadow="false"
                PopupControlID="pnlAddEditFTPUser" TargetControlID="lnkFakeFTPUser"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveFTPUser" />
        </Triggers>

    </asp:UpdatePanel>


    <!-- Access Folder -->
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="card border-light mb-3" style="width: 39.6%; display: inline-block; margin-left: 5px">
                <div class="card-header">
                    <h4 class="card-title">Access Folder<asp:Label ID="FTP_User_Name" runat="server"></asp:Label></h4>
                </div>
                <div class="card-body">
                    <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                    <asp:Button ID="btnAddAccessFolder" Text="Add Folder" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                </div>
            </div>

            <!-- Modal body -->
            <asp:Panel ID="pnlAddEditAccessFolder" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px">
                    <asp:Label ID="ModalHeaderAccessFolder" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 100%; padding: 0px 10px 20px 0px" colspan="2">
                            <label>FTP User</label>
                            <asp:DropDownList ID="DDL_FTP_User" runat="server" CssClass="form-control" Enabled="false"></asp:DropDownList>
                            <%--<asp:DropDownList ID="DDL_FTP_User" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true">
                                <asp:ListItem Text="" Value="-1"></asp:ListItem>
                            </asp:DropDownList>--%>
                            <asp:CompareValidator ID="CompareValidator_DDL_FTP_User" runat="server" ValidationGroup="AccessFolder"
                                ErrorMessage="Please select an active FTP user" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="true"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_FTP_User" Type="String">
                            </asp:CompareValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 80%; padding: 0px 10px 20px 0px">
                            <label>Access Folder</label>
                            <asp:DropDownList ID="DDL_Access_Folder" runat="server" CssClass="form-control" OnLoad="DDL_Access_Folder_Load"></asp:DropDownList>
                        </td>
                        <td style="width: 30%; padding: 0px 10px 20px 0px">
                            <asp:Button ID="AddAccessFolder" runat="server" Text="Add" Height="40px" CssClass="btn btn-info" ValidationGroup="AccessFolder" />
                            <asp:Button ID="btnClearAccessFolder" runat="server" Text="Clear" Height="40px" CssClass="btn btn-info" OnClick="btnClearAccessFolder_Click" />
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 800px; padding: 0px 10px 0px 0px" colspan="2">
                            <div class="GridList">
                                <asp:GridView ID="GridView_Access_Folder" runat="server"
                                    AutoGenerateColumns="false"
                                    AllowPaging="false"
                                    CellPadding="4"
                                    Font-Size="10"
                                    GridLines="None"
                                    ShowHeaderWhenEmpty="true"
                                    CssClass="table table-bordered">
                                    <HeaderStyle CssClass="table-secondary" Font-Bold="true" VerticalAlign="Top" />
                                    <RowStyle CssClass="Default" VerticalAlign="Middle" />
                                    <Columns>
                                        <asp:BoundField DataField="Path" HeaderText="Path" SortExpression="Path" />
                                        <asp:BoundField DataField="Folder" HeaderText="Folder" SortExpression="Folder" />
                                        <asp:TemplateField ItemStyle-Width="0px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDeleteLineItem" runat="server"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <!-- Hidden fields -->
                            <asp:TextBox ID="TB_Selected_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="TB_Selected_FTP_User" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="TB_Selected_Access_List_UID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            <asp:TextBox ID="TB_Selected_Contact_Person" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnClosePanel" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeAccessFolder" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupAccessFolder" runat="server" DropShadow="false"
                PopupControlID="pnlAddEditAccessFolder" TargetControlID="lnkFakeAccessFolder"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView2" />
        </Triggers>

    </asp:UpdatePanel>


    <!-- Control Button -->
    <div style="clear:both">
        <hr />
        <asp:LinkButton ID="BT_Create" Text="Create" CommandName="Create" runat="server" CssClass="btn btn-default" ValidationGroup="Distributor_FTP_Account"></asp:LinkButton>
        <asp:LinkButton ID="BT_Cancel" Text="Cancel" CommandName="Cancel" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>
    </div>


    <!-- javascript for GridView Row highlight effect -->
    <script type="text/javascript">
        var OrigRowColor;
        function SetMouseOver(element) {
            OrigRowColor = element.style.backgroundColor;
            element.style.backgroundColor = '#f5f5f5';
            element.style.cursor = 'pointer';
        }
        function SetMouseOut(element) {
            element.style.backgroundColor = OrigRowColor;
        }
    </script>

</asp:Content>

