<%@ Page Title="Maintenance Service Details" 
    Language="VB"
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Maintenance_Service_Records_Details.aspx.vb" 
    MaintainScrollPositionOnPostback="true"
    EnableEventValidation="false"
    Inherits="Services_Maintenance_Service_Records_Details" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
            BlockUI("<%=pnlAddEditCustomerBanner.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditCustomerStore.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditProductType.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditCustomerProduct.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditCustomerRetailProduct.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditESLTagsType.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditESLTagsDeployment.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditCustomerContract.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlUploadInvoice.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditMaintenanceContractStatusLog.ClientID %>");
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

    <style>
        .right-stick-textbox {
            border-top-right-radius: 0px;
            border-bottom-right-radius: 0px;
        }
        .left-stick-button {
            border-top-left-radius: 0px;
            border-bottom-left-radius: 0px;
        }

        .default-button-new-product-type{
            width: 100px;
        }

        .guided-text-dropdownlist{
            margin-left: 10px;
            margin-right: 20px;
            font-weight:bold;
            color:#337ab7;
        }

        /*Style for label convert to asp:lable*/
        .label-asp{
            display: inline-block;
            max-width: 100%;
            margin-bottom: 5px;
            font-weight: 700;
            box-sizing: border-box;
        }

        .tab-box{
            padding: 10px 0px 100px 10px;
            border: solid 1px;
            border-color: #fff #dee2e6 #dee2e6;
        }

        .selectedRow {
            background-color: #eeeeee;
        }

        .unselectedRow{
            background-color:transparent;
        }

        /* CSS for the up and down arrows */
        .sort-arrow {
            font-size: 12px; /* You can adjust this value to change the icon size */
        }

        .sort-arrow.asc::after {
            content: '\25B2'; /* Up-arrow character */
        }

        .sort-arrow.desc::after {
            content: '\25BC'; /* Down-arrow character */
        }

        /* CSS to position the arrow next to the header text */
        th {
            position: relative;
            white-space: nowrap; /* Prevent header text from wrapping */
            cursor: pointer; /* Add a pointer cursor to indicate the header is sortable */
        }

        th span.sort-arrow {
            margin-left: 5px; /* Adjust this value as needed to create space between header text and arrow */
        }
    </style>

    <style type="text/css">
        .sorted-column {
            background-color: #ffffe6;
        }
    </style>

    <style>
        .data-row-cursor-pointer {
            cursor: pointer;
        }

        /* If using the class approach, don't forget to include the highlighting as well */
        .data-row-cursor-pointer:hover {
            background-color: #f5f5f5;
        }
    </style>

    <style type="text/css">
        .CheckboxSelection {
            margin-left: 10px;
        }

        .CheckboxSelection input {
            width: 16px;
            height: 16px;
        }
        .CheckboxSelection label {
            font-weight:normal;
            font-size: 14px;
            margin-left: 10px;
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
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Name</label>
                                <asp:Label ID="LB_Hidden_Name" runat="server" Visible="false" />
                                <asp:TextBox ID="TB_Name" Text='<%# Eval("Name") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Name" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Name">
                                </asp:RequiredFieldValidator>
                                <asp:Label ID="nameexisterrormsg" runat="server" Text="Customer profile with same name exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                            </td>
                            <td style="width: 25%; padding: 0px 10px 10px 0px">
                                <label>Contact Person</label>
                                <asp:TextBox ID="TB_Contact_Person" Text='<%# Eval("Contact Person") %>' runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_E_Contact_Person" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Contact_Person">
                                </asp:RequiredFieldValidator>--%>
                            </td>
                            <td style="width: 25%; padding: 0px 10px 10px 0px">
                                <label>Phone</label>
                                <asp:TextBox ID="TB_Phone" Text='<%# Eval("Phone") %>' runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_E_Phone" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Phone">
                                </asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator ID="RegEx_TB_E_TB_Phone" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="Phone number must be numeric" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Phone"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" rowspan="2">
                                <label style="display: block">Address</label>
                                <asp:TextBox ID="TB_Address" Text='<%# Eval("Address") %>' runat="server" TextMode="MultiLine" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Address" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Address">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 50%; padding: 0px 10px 10px 0px" colspan="3">
                                <label>Email</label>
                                <asp:TextBox ID="TB_Email" Text='<%# Eval("Email") %>' runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_E_Email" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Email">
                                </asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator ID="RegEx_TB_E_Email" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="Invalid email address" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    ControlToValidate="TB_Email"
                                    ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" />
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 17%; padding: 0px 10px 10px 0px">
                                <asp:Label id="lbl_Created_Date" runat="server">Created Date</asp:Label>
                                <asp:TextBox ID="TB_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 17%; padding: 0px 10px 10px 0px">
                                <asp:Label id="lbl_Last_Updated" runat="server">Last Updated</asp:Label>
                                <asp:TextBox ID="TB_Last_Updated" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Last Updated")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 16%; padding: 0px 10px 10px 0px">
                                <asp:Label ID="LB_Status" Text='<%# Eval("Status") %>' runat="server" Visible="false" />
                                <asp:Label id="lbl_Account_status" runat="server">Account Status</asp:Label>
                                <asp:DropDownList ID="DDL_Status" runat="server" CssClass="form-control" OnDataBound="DDL_Status_DataBound">
                                    <asp:ListItem Text="Active" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Name</label>
                                <asp:Label ID="LB_Hidden_Name" runat="server" Visible="false" />
                                <asp:TextBox ID="TB_Name" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Name" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Name">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 25%; padding: 0px 10px 10px 0px">
                                <label>Contact Person</label>
                                <asp:TextBox ID="TB_Contact_Person" runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_I_Contact_Person" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Contact_Person">
                                </asp:RequiredFieldValidator>--%>
                            </td>
                            <td style="width: 25%; padding: 0px 10px 10px 0px">
                                <label>Phone</label>
                                <asp:TextBox ID="TB_Phone" runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_I_Phone" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Phone">
                                </asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator ID="RegEx_TB_I_TB_Phone" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="Phone number must be numeric" CssClass="invalid-feedback" 
                                    Display="Dynamic" 
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Phone"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" rowspan="2">
                                <label style="display: block">Address</label>
                                <asp:TextBox ID="TB_Address" runat="server" TextMode="MultiLine" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Address" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Address">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 50%; padding: 0px 10px 10px 0px" colspan="3">
                                <label>Email</label>
                                <asp:TextBox ID="TB_Email" runat="server" CssClass="form-control" />
                                <%--<asp:RequiredFieldValidator ID="RequiredField_I_Email" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Email">
                                </asp:RequiredFieldValidator>--%>
                                <asp:RegularExpressionValidator ID="RegEx_TB_I_Email" runat="server" ValidationGroup="MaintenanceAccount"
                                    ErrorMessage="Invalid email address" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    ControlToValidate="TB_Email"
                                    ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" />
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 17%; padding: 0px 10px 10px 0px">
                                <asp:Label id="lbl_Created_Date" runat="server">Created Date</asp:Label>
                                <asp:TextBox ID="TB_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 17%; padding: 0px 10px 10px 0px">
                                <asp:Label id="lbl_Last_Updated" runat="server">Last Updated</asp:Label>
                                <asp:TextBox ID="TB_Last_Updated" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Last Updated")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 16%; padding: 0px 10px 10px 0px">
                                <asp:Label ID="LB_Status" Text='<%# Eval("Status") %>' runat="server" Visible="false" />
                                <asp:Label id="lbl_Account_status" runat="server">Account Status</asp:Label>
                                <asp:DropDownList ID="DDL_Status" runat="server" CssClass="form-control" Enabled="false" OnDataBound="DDL_Status_DataBound">
                                    <asp:ListItem Text="Active" Value="1" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </InsertItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <hr />
    

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Customer Profile Content -->
    <div id="CustomerProfileContent" runat="server" style="margin-top: 40px">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" runat="server" visible="true" class="nav-item" role="presentation"><a id="link1" class="nav-link" data-toggle="tab" href="#customerBannerList" aria-selected="true" role="tab"  style="font-size: 15px; width: 150px; text-align: center">Banners</a></li>
            <li id="tab_link2" runat="server" visible="true" class="nav-item" role="presentation"><a id="link2" class="nav-link" data-toggle="tab" href="#customerStoreList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 150px; text-align: center">Stores</a></li>
            <li id="tab_link3" runat="server" visible="true" class="nav-item" role="presentation"><a id="link3" class="nav-link" data-toggle="tab" href="#ProductTypeList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">Product Types</a></li>
            <li id="tab_link4" runat="server" visible="true" class="nav-item" role="presentation"><a id="link4" class="nav-link" data-toggle="tab" href="#customerProductList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 150px; text-align: center">Products</a></li>
            <li id="tab_link5" runat="server" visible="true" class="nav-item" role="presentation"><a id="link5" class="nav-link" data-toggle="tab" href="#customerRetailProductList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">Retail Products</a></li>
            <li id="tab_link8" runat="server" visible="true" class="nav-item" role="presentation"><a id="link8" class="nav-link active" data-toggle="tab" href="#customerContractList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 190px; text-align: center">Service Contracts</a></li>
            <li id="tab_link6" runat="server" visible="true" class="nav-item" role="presentation"><a id="link6" class="nav-link" data-toggle="tab" href="#TagsTypeList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 190px; text-align: center">Tags Types</a></li>            
            <li id="tab_link7" runat="server" visible="true" class="nav-item" role="presentation"><a id="link7" class="nav-link" data-toggle="tab" href="#TagsDeploymentList" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 190px; text-align: center">Tags Deployment</a></li>
        </ul>
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane" id="customerBannerList" role="tabpanel">
                <!-- Customer Banner List -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Gridview -->
                            <asp:GridView ID="GridView1" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="No" HeaderText="No" SortExpression="No" ItemStyle-Width="0%" ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Banner Name" HeaderText="Banner Name" SortExpression="Banner Name" ItemStyle-Width="70%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Frequency" HeaderText="Frequency" SortExpression="Frequency" ItemStyle-Width="70%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Created Date" HeaderText="Created Date" SortExpression="Created Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Last Updated" HeaderText="Last Updated" SortExpression="Last Updated" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Banner ID") & "|" & Eval("Banner Name") & "|" & Eval("Customer ID") & "|" & Eval("Frequency") %>'
                                                OnClick="Edit_Banner_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Banner ID") & "|" & Eval("Banner Name") & "|" & Eval("Customer ID") & "|" & Eval("Frequency") %>'
                                                OnClick="Delete_Banner_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddBanner" Text="Add Banner" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCustomerBanner" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderBanner" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Banner Name</label>
                                        <asp:TextBox ID="TB_Banner" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Banner" runat="server" ValidationGroup="MaintenanceBanner"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Banner">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="bannernameexisterrormsg" runat="server" Text="Banner name exists, please choose a different name" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                </tr>
                                <tr style="vertical-align: top" id="bannerfrequencyrow" runat="server">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Frequency</label>
                                        <asp:TextBox ID="TB_Banner_Frequency" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Banner_Frequency" runat="server" ValidationGroup="MaintenanceBanner"
                                            ErrorMessage="This field cannot be blank. Input 00000000 as default" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Banner_Frequency">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegExValidator_TB_Banner_Frequency" runat="server" ValidationGroup="MaintenanceBanner"
                                            ErrorMessage="The value must be numeric and at least 8 characters long, and may include commas and spaces" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Banner_Frequency"
                                            ValidationExpression="^[\d\s,]*$">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Banner_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Banner_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Banner_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Banner_Customer_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Banner_Frequency" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveBanner" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceBanner" />
                                        <asp:Button ID="btnCancelBanner" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeBanner" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupBanner" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCustomerBanner" TargetControlID="lnkFakeBanner"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView1" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveBanner" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="customerStoreList" role="tabpanel">
                <!-- Customer Store List -->
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Use Panel and set default button to trigger validation -->
                            <asp:Panel ID="pnlSearchBox_Store" runat="server" DefaultButton="BT_Search_Store">
                                <!-- Searchbox for Maintenance Store -->
                                <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                    <asp:TextBox ID="TB_Search_Store" runat="server" CssClass="form-control right-stick-textbox" Width="400" placeholder="Type to search..."></asp:TextBox>
                                    <asp:Button ID="BT_Search_Store" runat="server" CssClass="form-control btn btn-outline btn-info left-stick-button" Text="Search" />
                                </div>
                            </asp:Panel>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView2" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="No" HeaderText="No" SortExpression="No" ItemStyle-Width="0%" ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Store Name" HeaderText="Store Name" SortExpression="Store Name" ItemStyle-Width="35%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Banner Name" HeaderText="Banner Name" SortExpression="Banner Name" ItemStyle-Width="30%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Created Date" HeaderText="Created Date" SortExpression="Created Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Last Updated" HeaderText="Last Updated" SortExpression="Last Updated" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Store ID") & "|" & Eval("Store Name") & "|" & Eval("Status") & "|" & Eval("Banner ID") & "|" & Eval("Customer ID") %>'
                                                OnClick="Edit_Store_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Store ID") & "|" & Eval("Store Name") & "|" & Eval("Status") & "|" & Eval("Banner ID") & "|" & Eval("Customer ID") %>'
                                                OnClick="Delete_Store_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddStore" Text="Add Store" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCustomerStore" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderStore" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Banner Name</label>
                                        <asp:DropDownList ID="DDL_Banner" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Banner" runat="server" ValidationGroup="MaintenanceStore"
                                            ErrorMessage="You have to select a banner" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Banner" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Store Name</label>
                                        <asp:TextBox ID="TB_Store" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Store" runat="server" ValidationGroup="MaintenanceStore"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Store">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="storenameexisterrormsg" runat="server" Text="Store name exists, please choose a different name" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <table id="status_row" runat="server" border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label id="lbl_Store_Status" runat="server" cssclass="label-asp-float-left">Status</label>
                                        <asp:DropDownList ID="DDL_Store_Status" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Active" Value="1" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Inactive" Value="0"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Store_Status" runat="server" ValidationGroup="MaintenanceStore"
                                            ErrorMessage="Please select" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Store_Status" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Store_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Store_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Store_Status" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Store_Banner_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Store_Customer_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveStore" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceStore" />
                                        <asp:Button ID="btnCancelStore" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeStore" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupStore" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCustomerStore" TargetControlID="lnkFakeStore"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView2" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveStore" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="ProductTypeList" role="tabpanel">
                <!-- Product Type List -->
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Use Panel and set default button to trigger validation -->
                            <asp:Panel ID="pnlSearchBox_Product_Type" runat="server" DefaultButton="BT_Search_Product_Type">
                                <!-- Searchbox for Product Type -->
                                <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                    <asp:TextBox ID="TB_Search_Product_Type" runat="server" CssClass="form-control right-stick-textbox" Width="400" placeholder="Type to search..."></asp:TextBox>
                                    <asp:Button ID="BT_Search_Product_Type" runat="server" CssClass="form-control btn btn-outline btn-info left-stick-button" Text="Search" />
                                </div>
                            </asp:Panel>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView3" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Product Name" HeaderText="Product Name" SortExpression="Product Name" ItemStyle-Width="80%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("UID") & "|" & Eval("Code") & "|" & Eval("Product Name") & "|" & Eval("Category") %>'
                                                OnClick="Edit_Product_Type_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("UID") & "|" & Eval("Code") & "|" & Eval("Product Name") & "|" & Eval("Category") %>'
                                                OnClick="Delete_Product_Type_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddProductType" Text="Add Product Type" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditProductType" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderProductType" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 200px; padding: 0px 10px 10px 0px">
                                        <label>Code</label>
                                        <asp:TextBox ID="TB_Code" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Code" runat="server" ValidationGroup="MaintenanceProductType"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Code">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="codeexisterrormsg" runat="server" Text="Product code exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                    <td style="width: 600px; padding: 0px 10px 10px 0px">
                                        <label>Product Name</label>
                                        <asp:TextBox ID="TB_Product_Name" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Product_Name" runat="server" ValidationGroup="MaintenanceProductType"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Product_Name">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                            <table id="category_row" runat="server" border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label id="lbl_category" runat="server">Category</label>
                                        <asp:DropDownList ID="DDL_Product_Category" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Product_Category" runat="server" ValidationGroup="MaintenanceProductType"
                                            ErrorMessage="Please select category" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Product_Category" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Product_Type_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_UID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Code" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Category" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveProductType" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceProductType" />
                                        <asp:Button ID="btnCancelProductType" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeProductType" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupProductType" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditProductType" TargetControlID="lnkFakeProductType"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView3" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddProductType" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="customerProductList" role="tabpanel">
                <!-- Customer POS Product List -->
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Use Panel and set default button to trigger validation -->
                            <asp:Panel ID="pnlSearchBox_Product" runat="server" DefaultButton="BT_Search_Product">
                                <!-- Searchbox for Maintenance Product -->
                                <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                    <asp:TextBox ID="TB_Search_Product" runat="server" CssClass="form-control right-stick-textbox" Width="400" placeholder="Type to search..."></asp:TextBox>
                                    <asp:Button ID="BT_Search_Product" runat="server" CssClass="form-control btn btn-outline btn-info left-stick-button" Text="Search" />
                                </div>
                            </asp:Panel>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView4" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Serial No" HeaderText="Serial No" SortExpression="Serial No" ItemStyle-Width="8%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Product Name" HeaderText="Product Name" SortExpression="Product Name" ItemStyle-Width="15%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Created Date" HeaderText="Created Date" SortExpression="Created Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Base Currency" HeaderText="Currency" SortExpression="Base Currency" ItemStyle-Width="0%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Base Currency Value" HeaderText="Amount" SortExpression="Base Currency Value" DataFormatString="{0:#,##0.00}" ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Banner" HeaderText="Banner" SortExpression="Banner" ItemStyle-Width="10%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" ItemStyle-Width="15%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Installation Date" HeaderText="Install Date" SortExpression="Installation Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />                                                                        
                                    <asp:BoundField DataField="Usage Start Date" HeaderText="Start Date" SortExpression="Usage Start Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Warranty Expiration" HeaderText="Expiry Date" SortExpression="Warranty Expiration" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Under Warranty" HeaderText="Under Warranty" SortExpression="Under Warranty" ItemStyle-Width="8%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Last Updated" HeaderText="Last Updated" SortExpression="Last Updated" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="10%" ItemStyle-Wrap="false" /> 
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Serial No") & "|" & Eval("Product Code") & "|" & Eval("Store ID") & "|" & Eval("Usage Start Date") & "|" & Eval("Warranty Cover Period") & "|" & Eval("Currency") & "|" & Eval("Product Value") & "|" & Eval("Installation Date")  %>'
                                                OnClick="Edit_Product_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Serial No") & "|" & Eval("Product Code") & "|" & Eval("Store ID") & "|" & Eval("Usage Start Date") & "|" & Eval("Warranty Cover Period") & "|" & Eval("Currency") & "|" & Eval("Product Value") & "|" & Eval("Installation Date")  %>'
                                                OnClick="Delete_Product_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                                               
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddProduct" Text="Add Product" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCustomerProduct" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderProduct" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 300px; padding: 0px 10px 10px 0px">
                                        <label>Serial No</label>
                                        <asp:TextBox ID="TB_Serial_No" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Serial_No" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Serial_No">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="serialnoexisterrormsg" runat="server" Text="Serial no exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                    <td style="width: 500px; padding: 0px 10px 10px 0px">
                                        <label>Product Name</label>
                                        <asp:DropDownList ID="DDL_Product" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Product" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="You have to select a product name" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Product" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 500px; padding: 0px 10px 10px 0px">
                                        <label>Location</label>
                                        <asp:DropDownList ID="DDL_Location" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Location" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="You have to select location" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Location" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                    <td style="width: 300px; padding: 0px 10px 10px 0px">
                                        <label>Install Date</label>
                                        <asp:TextBox ID="TB_Install_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Install_Date" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Install_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Warranty Start Date</label>
                                        <asp:TextBox ID="TB_Start_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Start_Date" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Start_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Warranty Cover</label>
                                        <asp:DropDownList ID="DDL_Warranty_Cover" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Warranty_Cover" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="You have to select warranty cover period" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Warranty_Cover" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Currency</label>
                                        <asp:DropDownList ID="DDL_Value_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Product Value</label>
                                        <asp:TextBox ID="TB_Product_Value" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="Required_TB_Product_Value" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Product_Value">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Product_Value" runat="server" ValidationGroup="MaintenanceProduct"
                                            ErrorMessage="Product value must be numeric" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Product_Value"
                                            ValidationExpression="^\$?\d+(,\d{3})*(\.\d+)?$"
                                            OnServerValidate="RegEx_TB_Product_Value_ServerValidate">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Product_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Serial_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Code" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Start_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Warranty_Cover_Period" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Value_Currency" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Value" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Product_Install_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                                        <asp:Button ID="btnSaveProduct" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceProduct" />
                                        <asp:Button ID="btnCancelProduct" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeProduct" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupProduct" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCustomerProduct" TargetControlID="lnkFakeProduct"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView4" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveProduct" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="customerRetailProductList" role="tabpanel">
                <!-- Customer Retail Product List -->
                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                         <div class="tab-box">
                            <!-- Use Panel and set default button to trigger validation -->
                             <asp:Panel ID="pnlSearchBox_Scale" runat="server" DefaultButton="BT_Search_Retail_Product">
                                 <!-- Searchbox for Maintenance Product -->
                                 <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                     <asp:TextBox ID="TB_Search_Retail_Product" runat="server" CssClass="form-control right-stick-textbox" Width="400" placeholder="Type to search..."></asp:TextBox>
                                     <asp:Button ID="BT_Search_Retail_Product" runat="server" CssClass="form-control btn btn-outline btn-info left-stick-button" Text="Search" />
                                 </div>
                             </asp:Panel>

                             <!-- Gridview -->
                             <asp:GridView ID="GridView5" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Serial No" HeaderText="Serial No" SortExpression="Serial No" ItemStyle-Width="5%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Product Name" HeaderText="Product Name" SortExpression="Product Name" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" ItemStyle-Width="10%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Created Date" HeaderText="Created Date" SortExpression="Created Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Base Currency" HeaderText="Currency" SortExpression="Base Currency" ItemStyle-Width="0%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Base Currency Value" HeaderText="Amount" SortExpression="Base Currency Value" DataFormatString="{0:#,##0.00}" ItemStyle-Width="5%"  HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Banner" HeaderText="Banner" SortExpression="Banner" ItemStyle-Width="15%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" ItemStyle-Width="20%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Installation Date" HeaderText="Install Date" SortExpression="Installation Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />                                    
                                    <asp:BoundField DataField="Usage Start Date" HeaderText="Start Date" SortExpression="Usage Start Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Warranty Expiration" HeaderText="Expiry Date" SortExpression="Warranty Expiration" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Under Warranty" HeaderText="Under Warranty" SortExpression="Under Warranty" ItemStyle-Width="5%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Last Updated" HeaderText="Last Updated" SortExpression="Last Updated" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" /> 
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Serial No") & "|" & Eval("Product Code") & "|" & Eval("Store ID") & "|" & Eval("Usage Start Date") & "|" & Eval("Warranty Cover Period") & "|" & Eval("Currency") & "|" & Eval("Product Value") & "|" & Eval("Installation Date") %>'
                                                OnClick="Edit_Retail_Product_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Serial No") & "|" & Eval("Product Code") & "|" & Eval("Store ID") & "|" & Eval("Usage Start Date") & "|" & Eval("Warranty Cover Period") & "|" & Eval("Currency") & "|" & Eval("Product Value") & "|" & Eval("Installation Date") %>'
                                                OnClick="Delete_Retail_Product_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>      
                                </Columns>
                             </asp:GridView>
                             <asp:Button ID="btnAddRetailProduct" Text="Add Product" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                         </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCustomerRetailProduct" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderRetailProduct" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 300px; padding: 0px 10px 10px 0px">
                                        <label>Serial No</label>
                                        <asp:TextBox ID="TB_Retail_Product_Serial_No" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Retail_Product_Serial_No" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Retail_Product_Serial_No">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="retailproductserialnoexisterrormsg" runat="server" Text="Serial no exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                    <td style="width: 500px; padding: 0px 10px 10px 0px">
                                        <label>Product Name</label>
                                        <asp:DropDownList ID="DDL_Retail_Product" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Retail_Product" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="You have to select a product name" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Retail_Product" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 500px; padding: 0px 10px 10px 0px">
                                        <label>Location</label>
                                        <asp:DropDownList ID="DDL_Retail_Location" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Retail_Location" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="You have to select location" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Retail_Location" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                    <td style="width: 300px; padding: 0px 10px 10px 0px">
                                        <label>Install Date</label>
                                        <asp:TextBox ID="TB_Retail_Product_Install_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Retall_Product_Install_Date" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Retail_Product_Install_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Warranty Start Date</label>
                                        <asp:TextBox ID="TB_Retail_Product_Start_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Retail_Product_Start_Date" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Retail_Product_Start_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Warranty Cover</label>
                                        <asp:DropDownList ID="DDL_Retail_Product_Warranty_Cover" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Retail_Product_Warranty_Cover" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="You have to select warranty cover period" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Retail_Product_Warranty_Cover" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Currency</label>
                                        <asp:DropDownList ID="DDL_Retail_Product_Value_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Product Value</label>
                                        <asp:TextBox ID="TB_Retail_Product_Value" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="Required_TB_Retail_Product_Value" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Retail_Product_Value">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Retail_Product_Value" runat="server" ValidationGroup="MaintenanceRetailProduct"
                                            ErrorMessage="Product value must be numeric" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Retail_Product_Value"
                                            ValidationExpression="^\$?\d+(,\d{3})*(\.\d+)?$"
                                            OnServerValidate="RegEx_TB_Product_Value_ServerValidate">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Serial_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Code" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Start_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Warranty_Cover_Period" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Value_Currency" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Value" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Retail_Product_Install_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                                        <asp:Button ID="btnSaveRetailProduct" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceRetailProduct" />
                                        <asp:Button ID="btnCancelRetailProduct" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeRetailProduct" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupRetailProduct" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCustomerRetailProduct" TargetControlID="lnkFakeRetailProduct"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView5" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveRetailProduct" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane active" id="customerContractList" role="tabpanel">
                <!-- Customer Service Contract -->
                <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                    <ContentTemplate>
                        <div id="MaintenanceContractListingSection" runat="server" class="tab-box">
                            <!-- Dropdownlist for financial year -->
                            <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                <asp:Label ID="LB_Financial_Year" runat="server" Text="Financial Year" CssClass="guided-text-dropdownlist"></asp:Label>
                                <asp:DropDownList ID="DDL_Financial_Year" runat="server" CssClass="form-control" Width="250px" AutoPostBack="true"></asp:DropDownList>
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView8" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="No" HeaderText="No" SortExpression="No" ItemStyle-Width="0%" ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Reference No" HeaderText="Contract Reference No" SortExpression="Reference No" ItemStyle-Width="10%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Store Name" HeaderText="Maintenance Service Cover To" SortExpression="Store Name" ItemStyle-Width="45%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Start Date" HeaderText="Start Date" SortExpression="Start Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="End Date" HeaderText="End Date" SortExpression="End Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Base Currency" HeaderText="Currency" SortExpression="Base Currency" ItemStyle-Width="0%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Base Currency Amount" HeaderText="Amount" SortExpression="Base Currency Amount" DataFormatString="{0:#,##0.00}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-Width="10%" ItemStyle-Wrap="false" /> 
                                    <asp:BoundField DataField="Invoice No" HeaderText="Invoice No" SortExpression="Invoice No" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Store ID") & "|" & Eval("Start Date") & "|" & Eval("End Date") & "|" & Eval("Currency") & "|" & Eval("Amount") & "|" & Eval("Reference No") & "|" & Eval("Status Code") %>'
                                                OnClick="Edit_Contract_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Store ID") & "|" & Eval("Start Date") & "|" & Eval("End Date") & "|" & Eval("Currency") & "|" & Eval("Amount") & "|" & Eval("Reference No") & "|" & Eval("Status Code") %>'
                                                OnClick="Delete_Contract_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="UploadLinkButton" runat="server"
                                                Text="<i class='bi bi-cloud-arrow-up-fill'></i>"
                                                CssClass="btn btn-xs btn-success"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique ID") & "|" & Eval("Store ID") & "|" & Eval("Reference No") %>'
                                                OnClick="Upload_Invoice_Modal_Click">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddContract" Text="Add Contract" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />

                            <div class="alert alert-info" style="float:right; text-align:left; margin-right: 20px; width: 36%; text-align:right">
                                <div style="text-align: left"><b>Notes:</b></div>
                                <div style="text-align: left">1. Contract record with <b>Start Date</b> of last year will be locked for editing/deletion to mark as finalized.</div>
                                <div style="text-align: left">2. Contract Amount stored in foreign currency will be auto calculated to Base Currency (SGD).</div>
                            </div>

                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCustomerContract" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderContract" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Contract Reference No</label>
                                        <asp:TextBox ID="TB_Reference_No" runat="server" CssClass="form-control" Placeholder="Unique Identification for a Contract"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="Required_TB_Reference_No" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Please type contract reference no" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Reference_No">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Start Date</label>
                                        <asp:TextBox ID="TB_Contract_Start_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Contract_Start_Date" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Contract_Start_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>End Date</label>
                                        <asp:TextBox ID="TB_Contract_End_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Contract_End_Date" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Contract_End_Date">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="enddatelessererrormsg" runat="server" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 200px; padding: 0px 10px 10px 0px">
                                        <label>Currency</label>
                                        <asp:DropDownList ID="DDL_Contract_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 200px; padding: 0px 10px 10px 0px">
                                        <label>Amount</label>
                                        <asp:TextBox ID="TB_Contract_Amount" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="Required_TB_Contract_Amount" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Contract_Amount">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Contract_Amount" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Amount must be numeric value" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Contract_Amount"
                                            ValidationExpression="^\$?\d+(,\d{3})*(\.\d+)?$"
                                            OnServerValidate="RegEx_TB_Contract_Amount_ServerValidate">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                    <td style="width: 380px; padding: 0px 10px 10px 0px" colspan="2">
                                        <label>Status</label>
                                        <asp:DropDownList ID="DDL_Contract_Process_Status" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <label>Maintenance Service Cover To</label>
                                        <asp:DropDownList ID="DDL_Contract_Store" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Contract_Store" runat="server" ValidationGroup="MaintenanceContract"
                                            ErrorMessage="Please select store" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Contract_Store" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Contract_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Start_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_End_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Currency" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Amount" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Reference_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_Process_Status" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveContract" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceContract" />
                                        <asp:Button ID="btnCancelContract" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <!-- Model body - Upload Invoice -->
                        <asp:Panel ID="pnlUploadInvoice" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderUploadInvoice" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
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
                                    <td style="width: 800px; padding: 0px 10px 10px 0px">
                                        <div>
                                            <asp:CheckBox ID="Chk_UploadSameInvoice" runat="server" CssClass="CheckboxSelection" Text="Update invoice of different contract" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Contract_To_Upload_Invoice_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_To_Upload_Invoice_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_To_Upload_Invoice_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Contract_To_Upload_Invoice_Reference_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnUploadInvoice" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="true" ValidationGroup="FileUpload" />
                                        <asp:Button ID="btnCancelUploadInvoice" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeContract" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupContract" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCustomerContract" TargetControlID="lnkFakeContract"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>

                        <asp:LinkButton ID="lnkFakeUploadInvoice" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupUploadInvoice" runat="server" DropShadow="false"
                            PopupControlID="pnlUploadInvoice" TargetControlID="lnkFakeUploadInvoice"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView8" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveContract" />
                        <asp:PostBackTrigger ControlID="btnUploadInvoice" />   <%--Must use PostBackTrigger, otherwise FileUpload return null value--%>
                    </Triggers>

                </asp:UpdatePanel>

                <!-- Service Contract Log -->
                <asp:UpdatePanel ID="UpdatePanel9" runat="server">
                    <ContentTemplate>
                        <div id="MaintenanceContractLogSection" runat="server" style="padding: 10px 0px 40px 10px">
                            <!-- Separator line -->
                            <div id="MaintenanceContractStatus_Header" runat="server" style="text-align: left">
                                <div style="margin-left:10px; clear:both"><h5>Comments:</h5></div>
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView9" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Created_Date" HeaderText="Date" SortExpression="Created_Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Log_Description" HeaderText="Log Description" SortExpression="Log_Description" ItemStyle-Width="80%" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="Edited_By" HeaderText="Edited By" SortExpression="Edited_By" ItemStyle-Width="15%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Unique_ID") %>'
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                OnClick="Edit_Maintenance_Status_Log_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Unique_ID") %>'
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                OnClick="Delete_Maintenance_Status_Log_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <!-- Control button -->
                            <asp:Button ID="btnAddMaintenanceContractStatusLog" Text="Add Comment" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" Visible="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditMaintenanceContractStatusLog" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderMaintenanceContractStatusLog" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 20px 0px">
                                        <label>Comment</label>
                                        <asp:TextBox ID="TB_MaintenanceContractStatusLog" runat="server" CssClass="form-control" placeholder="Type note here.."></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_MaintenanceContractStatusLog" runat="server" ValidationGroup="MaintenanceContractStatusLog"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_MaintenanceContractStatusLog">
                                        </asp:RequiredFieldValidator>
                                        <!-- Hidden fields -->
                                        <asp:TextBox ID="TB_Selected_TB_MaintenanceContractStatusLog_Parent_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:TextBox ID="TB_Selected_TB_MaintenanceContractStatusLog_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:TextBox ID="TB_Selected_TB_MaintenanceContractStatusLog_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="4">
                                        <asp:Button ID="btnSaveMaintenanceContractStatusLog" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceContractStatusLog" />
                                        <asp:Button ID="btnCancelMaintenanceContractStatusLog" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()"  />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeMaintenanceContractStatusLog" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupMaintenanceContractStatusLog" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditMaintenanceContractStatusLog" TargetControlID="lnkFakeMaintenanceContractStatusLog"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>

                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView9" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveMaintenanceContractStatusLog" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="TagsTypeList" role="tabpanel">
                <!-- ESL Tags Type List -->
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Dropdownlist for financial year -->
                            <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                <asp:Label ID="LB_Tags_Group_Selection" runat="server" Text="Tags Group" CssClass="guided-text-dropdownlist"></asp:Label>
                                <asp:DropDownList ID="DDL_Tags_Group" runat="server" CssClass="form-control" Width="250px" AutoPostBack="true"
                                    AppendDataBoundItems="true">
                                    <asp:ListItem Text="(All)" Value="-1" />
                                </asp:DropDownList>
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView6" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Tags Group" HeaderText="Tags Group" SortExpression="Tags Group" ItemStyle-Width="50%" ItemStyle-Wrap="false" />   
                                    <asp:BoundField DataField="Tags Type" HeaderText="Tags Type" SortExpression="Tags Type" ItemStyle-Width="50%" ItemStyle-Wrap="false" />   
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Tags Group") & "|" & Eval("Tags Type") %>'
                                                OnClick="Edit_ESL_Tags_Type_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Tags Group") & "|" & Eval("Tags Type") %>'
                                                OnClick="Delete_ESL_Tags_Type_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddESLTagsType" Text="Add Tag Type" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditESLTagsType" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderESLTagsType" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Tag Group</label>
                                        <asp:TextBox ID="TB_Tags_Group" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Tags_Group" runat="server" ValidationGroup="MaintenanceESLTagsType"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Tags_Group">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Tag Type</label>
                                        <asp:TextBox ID="TB_Tags_Type" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Tags_Type" runat="server" ValidationGroup="MaintenanceESLTagsType"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Tags_Type">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="esltagstypeexisterrormsg" runat="server" Text="Tags Type exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_ESL_Tags_Type_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_ESL_Tags_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>								
                                <asp:TextBox ID="TB_Selected_ESL_Tags_Type_Tags_Group" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
							    <asp:TextBox ID="TB_Selected_ESL_Tags_Type_Tags_Type" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>							
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px">
                                        <asp:Button ID="btnSaveESLTagsType" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceESLTagsType" />
                                        <asp:Button ID="btnCancelESLTagsType" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeESLTagsType" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupESLTagsType" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditESLTagsType" TargetControlID="lnkFakeESLTagsType"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView6" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveESLTagsType" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="TagsDeploymentList" role="tabpanel">
                <!-- ESL Tags Deployment List -->
                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                    <ContentTemplate>
                        <div class="tab-box">
                            <!-- Use Panel and set default button to trigger validation -->
                             <asp:Panel ID="pnlSearchBox_Tags_Store_Deployment" runat="server" DefaultButton="BT_Search_Tags_Store">
                                 <!-- Searchbox for Maintenance Product -->
                                 <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 5px">
                                     <asp:TextBox ID="TB_Search_Tags_Store" runat="server" CssClass="form-control right-stick-textbox" Width="400" placeholder="Type to search..."></asp:TextBox>
                                     <asp:Button ID="BT_Search_Tags_Store" runat="server" CssClass="form-control btn btn-outline btn-info left-stick-button" Text="Search" />
                                 </div>
                             </asp:Panel>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView7" runat="server"></asp:GridView>
                            <asp:Button ID="btnAddTagsDeployment" Text="Add Tag Deployment" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditESLTagsDeployment" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderESLTagsDeployment" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 480px; padding: 0px 10px 10px 0px" colspan="2">
                                        <label>Location</label>
                                        <asp:DropDownList ID="DDL_Tags_Deployment_Location" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Tags_Deployment_Location" runat="server" ValidationGroup="MaintenanceESLTagsDeployment"
                                            ErrorMessage="Please select location" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Tags_Deployment_Location" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                    <td style="width: 320px; padding: 0px 10px 10px 0px" colspan="2">
                                        <label>Install Date</label>
                                        <asp:TextBox ID="TB_Tags_Install_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Tags_Install_Date" runat="server" ValidationGroup="MaintenanceESLTagsDeployment"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Tags_Install_Date">
                                        </asp:RequiredFieldValidator>		
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 220px; padding: 0px 10px 10px 0px">
                                        <label>Tags Group</label>
                                        <asp:DropDownList ID="DDL_Tags_Group_Deployment" runat="server" CssClass="form-control" AutoPostBack="true"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Tags_Group_Deployment" runat="server" ValidationGroup="MaintenanceESLTagsDeploymentAddLineItems"
                                            ErrorMessage="Please select Tags Group" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Tags_Group_Deployment" Type="String">
                                        </asp:CompareValidator>	
                                    </td>
                                    <td style="width: 220px; padding: 0px 10px 10px 0px">
                                        <label>Model</label>
                                        <asp:DropDownList ID="DDL_Tags_Type_Deployment" runat="server" CssClass="form-control" AutoPostBack="true"
                                            AppendDataBoundItems="true">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Tags_Type_Deployment" runat="server" ValidationGroup="MaintenanceESLTagsDeploymentAddLineItems"
                                            ErrorMessage="Please select Model" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Tags_Type_Deployment" Type="String">
                                        </asp:CompareValidator>	
                                    </td>
                                    <td style="width: 150px; padding: 0px 10px 10px 0px">
                                        <label>Quantity</label>
                                        <asp:TextBox ID="TB_Tags_Deployment_Quantity" runat="server" CssClass="form-control" placeholder="0"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Tags_Deployment_Quantity" runat="server" ValidationGroup="MaintenanceESLTagsDeploymentAddLineItems"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Tags_Deployment_Quantity">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Tags_Deployment_Quantity" runat="server" ValidationGroup="MaintenanceESLTagsDeploymentAddLineItems"
                                            ErrorMessage="Value must be numeric and non-zero" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Tags_Deployment_Quantity"
                                            ValidationExpression="^(-?[1-9]\d*)$"
                                            OnServerValidate="RegEx_TB_Tags_Deployment_Quantity_ServerValidate">
                                        </asp:RegularExpressionValidator>	
                                    </td>
                                    <td style="width: 150px; padding: 25px 10px 20px 0px">
                                        <asp:Button ID="AddTagsDeploymentLineItems" runat="server" Text="Add" Height="40px" Width="70px" CssClass="btn btn-info" ValidationGroup="MaintenanceESLTagsDeploymentAddLineItems" />
                                        <asp:Button ID="btnClearTagsDeploymentLineItems" runat="server" Text="Clear" Height="40px" Width="70px" CssClass="btn btn-info" />
                                    </td>
                                </tr>
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 0px 0px" colspan="4">
                                        <div class="GridList">
                                            <asp:GridView ID="GridView_Tags_Deployment_List" runat="server"
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
                                                    <asp:BoundField DataField="Tags_Group" HeaderText="Tags Group" SortExpression="Tags_Group" ItemStyle-Width="30%" />
                                                    <asp:BoundField DataField="Tags_Type" HeaderText="Model" SortExpression="Tags_Type" ItemStyle-Width="30%" />
                                                    <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" ItemStyle-Width="40%" />
                                                    <asp:TemplateField ItemStyle-Width="0px">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkDeleteLineItem" runat="server"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="tagdeploymentlistboxerrormsg" runat="server" style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 0px 0px 0px" colspan="4">
                                        <span style="font-size: 12px; color: #dc3545">Tags deployment record cannot be blank</span>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Tags_Deployment_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Tags_Deployment_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Tags_Deployment_Customer_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Tags_Deployment_Store_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Tags_Deployment_Installation_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveESLTagsDeployment" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="MaintenanceESLTagsDeployment" />
                                        <asp:Button ID="btnCancelESLTagsDeployment" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeESLTagsDeployment" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupESLTagsDeployment" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditESLTagsDeployment" TargetControlID="lnkFakeESLTagsDeployment"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView7" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveESLTagsDeployment" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
        </div>
    </div>


    <!-- Control Button -->
    <div style="clear: both">
        <br />
        <hr style="margin-top: 50px" />
        <asp:LinkButton ID="BT_Create" runat="server" CssClass="btn btn-default" ValidationGroup="MaintenanceAccount"></asp:LinkButton>
        <asp:LinkButton ID="BT_Cancel" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>
    </div>


    <!-- javascript for GridView Row highlight effect -->
    <script type="text/javascript">
        var OrigRowColor;
        var OrigColumnColor;

        function SetMouseOver(element) {
            OrigRowColor = element.style.backgroundColor;

            // Check if the cell has the sorted-column class
            var sortedColumnCell = element.querySelector('.sorted-column');
            if (sortedColumnCell) {
                OrigColumnColor = sortedColumnCell.style.backgroundColor;
                sortedColumnCell.style.backgroundColor = '#f5f5f5';
            }

            element.style.backgroundColor = '#f5f5f5';
            element.style.cursor = 'pointer';
        }

        function SetMouseOut(element) {
            element.style.backgroundColor = OrigRowColor;

            // Check if the cell has the sorted-column class and restore the original color
            var sortedColumnCell = element.querySelector('.sorted-column');
            if (sortedColumnCell) {
                sortedColumnCell.style.backgroundColor = OrigColumnColor;
            }
        }
    </script>



    <!-- Keep where last tab selected -->
    <script src="/Scripts/jquery-1.3.2.min.js"></script> <!-- need to use jquery-1.3.2.min.js -->
    <script>
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
                localStorage.setItem('activeTab', $(e.target).attr('href'));
                localStorage.setItem('activeLink', $(e.target).attr('id'));
            });

            var activeTab = localStorage.getItem('activeTab');

            if (activeTab) {
                $('#myTab a[href="' + activeTab + '"]').tab('show');  // Show current tab
            }
        });
    </script>

</asp:Content>

