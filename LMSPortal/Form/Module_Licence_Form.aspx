<%@ Page Title="LMSPortal - Register Module Licence" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Module_Licence_Form.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Form_Module_Licence_Form" %>

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
            BlockUI("<%=pnlAddEditModuleLicenceOrder.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditModuleLicence.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddNotes.ClientID %>");
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
        <div class="card-header"><h4 class="card-title">Customer Details</h4></div>
        <div class="card-body">
            <asp:FormView ID="FormView1" runat="server" Width="100%">
                <ItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Name</label>
                                <asp:TextBox ID="TB_Name" Text='<%# Eval("Name") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Code</label>
                                <asp:TextBox ID="TB_Code" Text='<%# Eval("Distributor_Code") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" rowspan="2">
                                <label style="display: block">Address</label>
                                <asp:TextBox ID="TB_Address" Text='<%# Eval("Address") %>' runat="server" TextMode="MultiLine" Enabled="false" BackColor="#eeeeee" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Type</label>
                                <asp:Label ID="LB_Type" Text='<%# Eval("Type") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Type" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Type_SqlDataSource"
                                    DataTextField="Type"
                                    DataValueField="Type"
                                    OnDataBound="DDL_Type_DataBound"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="DDL_Type_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Type_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Type FROM DB_Lookup WHERE Lookup_Name = 'Customer Type'"></asp:SqlDataSource>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Group</label>
                                <asp:Label ID="LB_Group_ID" Text='<%# Eval("Group_ID") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Group_ID" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Group_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Group_ID"
                                    OnDataBound="DDL_Group_ID_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Group_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Group_ID, Name FROM Master_Customer_Group"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" colspan="2">
                                <label id="lbl_By_Distributor" runat="server">By Distributor</label>
                                <asp:Label ID="LB_By_Distributor" Text='<%# Eval("By_Distributor") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server"
                                    DataSourceID="By_Distributor_SqlDataSource" CssClass="form-control" Enabled="false"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer', 'DIGI') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Contact Person</label>
                                <asp:TextBox ID="TB_Contact_Person" Text='<%# Eval("Contact_Person") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Phone</label>
                                <asp:TextBox ID="TB_Phone" Text='<%# Eval("Phone") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Email</label>
                                <asp:TextBox ID="TB_Email" Text='<%# Eval("Email") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Module Licence Content -->
    <div id="ModuleLicenceContent" runat="server" style="margin-top: 40px">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" class="nav-item" role="presentation"><a id="link1" class="nav-link active" data-toggle="tab" href="#licencepoolandorder" aria-selected="true" role="tab"  style="font-size: 15px; width: 200px; text-align: center">Licence Pool / Order</a></li>
            <li id="tab_link2" class="nav-item" role="presentation"><a id="link2" class="nav-link" data-toggle="tab" href="#activationkeybypo" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 200px; text-align: center">Activation Key by PO</a></li>
            <li id="tab_link3" class="nav-item" role="presentation"><a id="link3" class="nav-link" data-toggle="tab" href="#activatedailicence" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 200px; text-align: center">Activated AI Licences</a></li>
            <li id="tab_link4" class="nav-item" role="presentation"><a id="link4" class="nav-link" data-toggle="tab" href="#ailicencerenewal" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 200px; text-align: center">AI Licence Renewal</a></li>
        </ul>
    </div>
    <div class="tab-content" id="myTabContent">
        <div class="tab-pane active" id="licencepoolandorder" role="tabpanel">
            <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                <!-- Module Licence Order Details -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddModuleLicenceOrder" Text="Add New" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditModuleLicenceOrder" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px">
                                <asp:Label ID="ModalHeaderModuleLicenceOrder" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 45%; padding: 0px 10px 20px 0px">
                                        <label>PO No.</label>
                                        <asp:TextBox ID="TB_Order_PO_No" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Order_PO_No" runat="server" ValidationGroup="ModuleLicenceOrder"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Order_PO_No">
                                        </asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="Custom_Validator_TB_Order_PO_No" runat="server" ValidationGroup="ModuleLicenceOrder"
                                            ErrorMessage="PO No. exists, please try again." CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            ControlToValidate="TB_Order_PO_No"
                                            OnServerValidate="Custom_Validator_TB_Order_PO_No_ServerValidate">
                                        </asp:CustomValidator>
                                    </td>
                                    <td style="width: 25%; padding: 0px 10px 20px 0px">
                                        <label>PO Date</label>
                                        <asp:TextBox ID="TB_Order_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Order_PO_Date" runat="server" ValidationGroup="ModuleLicenceOrder"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Order_PO_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 30%; padding: 0px 10px 20px 0px">
                                        <label>Chargeable</label>
                                        <asp:DropDownList ID="DDL_Order_Chargeable" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                                <tr style="vertical-align: top">
                                    <td style="width: 45%; padding: 0px 10px 20px 0px">
                                        <label>Requestor</label>
                                        <asp:DropDownList ID="DDL_Order_Sales_Representative" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 55%; padding: 0px 10px 20px 0px" colspan="2">
                                        <label>Remarks</label>
                                        <asp:TextBox ID="TB_Order_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align: top">
                                    <td style="width: 100%; padding: 0px 10px 10px 0px" colspan="3">
                                        <hr />
                                        <h4>Order Items</h4>
                                    </td>
                                </tr>
                                <tr style="vertical-align: top">
                                    <td style="width: 45%; padding: 0px 10px 20px 0px">
                                        <label>Module Type</label>
                                        <asp:DropDownList ID="DDL_Order_Module_Type" runat="server" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 25%; padding: 0px 10px 20px 0px">
                                        <label>Quantity</label>
                                        <asp:TextBox ID="TB_Order_Quantity" runat="server" CssClass="form-control" placeholder="0"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Order_Quantity" runat="server" ValidationGroup="ModuleLicenceOrder"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Order_Quantity">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Order_Quantity" runat="server" ValidationGroup="ModuleLicenceOrder"
                                            ErrorMessage="Quantity must be an integer" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Order_Quantity"
                                            ValidationExpression="^(0|[1-9]\d*)$">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                    <td style="width: 30%; padding: 25px 10px 20px 0px">
                                        <asp:Button ID="AddOrderLineItems" runat="server" Text="Add" Height="40px" CssClass="btn btn-info" ValidationGroup="ModuleLicenceOrder" />
                                        <asp:Button ID="btnClearOrderLineItems" runat="server" Text="Clear" Height="40px" CssClass="btn btn-info" />
                                    </td>
                                </tr>
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 0px 0px" colspan="3">
                                        <div class="GridList">
                                            <asp:GridView ID="GridView_Order_List" runat="server"
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
                                                    <asp:BoundField DataField="Module_Type" HeaderText="Module Type" SortExpression="Module_Type" />
                                                    <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr id="licenceorderlistboxerrormsg" runat="server" style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 0px 0px 0px" colspan="3">
                                        <span style="font-size: 12px; color: #dc3545">Please add module type and quantity</span>
                                    </td>
                                </tr>
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveModuleLicenceOrder" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="ModuleLicenceOrder" />
                                        <asp:Button ID="btnCancelModuleLicenceOrder" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeModuleLicenceOrder" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupModuleLicenceOrder" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditModuleLicenceOrder" TargetControlID="lnkFakeModuleLicenceOrder"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView1" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveModuleLicenceOrder" />
                    </Triggers>

                </asp:UpdatePanel>

                <!-- Module Licence Pool -->
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="GridView3" runat="server"></asp:GridView>

                        <!-- Guide to add Licence Pool for customer under local DIGI Singapore -->
                        <div id="AddLicencePoolGuide" runat="server" visible="false" class="alert alert-dismissible alert-secondary" style="background-color: #f2f2f3; border: solid 1px #f3f3f3; width: 99%">
                            <h4>Notes:</h4>
                            <p>If Licence Pool not appear in above list, do the following:</p>
                            <p>Go to [Settings] > [Lookup Table] > Search <b>Module Licence Bind</b> to add Licence Pool.</p>
                        </div>

                        <!--Add a refresh linkbutton to sync latest license status-->
                        <div style="margin-top: 20px">
                            <asp:LinkButton ID="AILicenceRefresh" runat="server" CssClass="btn btn-default"></asp:LinkButton>
                        </div>
                    </ContentTemplate>

                    <Triggers>
                        <asp:PostBackTrigger ControlID="AILicenceRefresh" />
                    </Triggers>
                </asp:UpdatePanel>

                <div style="clear: both"></div>
            </div>
        </div>
        <div class="tab-pane" id="activationkeybypo" role="tabpanel">
            <!-- Module Licence Activation Key Details -->
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddModuleLicence" Text="Add New" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                    </div>

                    <!-- Modal body -->
                    <asp:Panel ID="pnlAddEditModuleLicence" runat="server" CssClass="modalPopup">
                        <h3 style="padding: 10px 0px 20px 20px">
                            <asp:Label ID="ModalHeaderModuleLicence" runat="server"></asp:Label></h3>
                        <table border="0" class="modalPopupTable" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="ModuleLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_PO_No">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="ModuleLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_PO_Date">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Application Type</label>
                                    <asp:DropDownList ID="DDL_Application_Type" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>Requestor</label>
                                    <asp:DropDownList ID="DDL_Sales_Representative" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>Chargeable</label>
                                    <asp:DropDownList ID="DDL_Chargeable" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>OS Type</label>
                                    <asp:DropDownList ID="DDL_OS_Type" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>Licensee's Email</label>
                                    <asp:TextBox ID="TB_Email" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Email" runat="server" ValidationGroup="ModuleLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Email">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_Email" runat="server" ValidationGroup="ModuleLicence"
                                        ErrorMessage="Invalid email address" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="true"
                                        ControlToValidate="TB_Email"
                                        ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" />
                                </td>
                                <td style="width: 55%; padding: 0px 10px 20px 0px" colspan="2">
                                    <label>Remarks</label>
                                    <asp:TextBox ID="TB_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 100%; padding: 0px 10px 0px 0px" colspan="3">
                                    <label>Upload Licence file</label>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 0px 10px 20px 0px" colspan="2">
                                    <asp:FileUpload ID="FileUpload1" runat="server" ToolTip="Click to browse" />
                                    <asp:RequiredFieldValidator ID="RequiredField_FileUpload1" runat="server" ValidationGroup="FileUpload"
                                        ErrorMessage="Please select a file to upload" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="FileUpload1">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_FileUpload1" runat="server" ValidationGroup="FileUpload"
                                        ErrorMessage="Only (.txt) file is allowed" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="FileUpload1"
                                        ValidationExpression="^([0-9a-zA-Z_\-~ :\\])+(.txt)$">
                                    </asp:RegularExpressionValidator>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <asp:Button ID="UploadLineItems" runat="server" Text="Upload" Height="40px" CssClass="btn btn-info" ValidationGroup="FileUpload" />
                                    <asp:Button ID="btnClearLineItems" runat="server" Text="Clear" Height="40px" CssClass="btn btn-info" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 800px; padding: 0px 10px 0px 0px" colspan="3">
                                    <div class="GridList">
                                        <asp:GridView ID="GridView_Licence_List" runat="server"
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
                                                <asp:BoundField DataField="Application_Type" HeaderText="Application Type" SortExpression="Application_Type" />
                                                <asp:BoundField DataField="OS_Type" HeaderText="OS Type" SortExpression="OS_Type" />
                                                <asp:BoundField DataField="Licence_Code" HeaderText="Licence Code" SortExpression="Licence_Code" />
                                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                            <tr id="licencekeylistboxerrormsg" runat="server" style="vertical-align: top">
                                <td style="width: 800px; padding: 0px 0px 0px 0px" colspan="3">
                                    <span style="font-size: 12px; color: #dc3545">Please upload licence file</span>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                                    <asp:Button ID="btnSaveModuleLicence" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="ModuleLicence" />
                                    <asp:Button ID="btnCancelModuleLicence" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                        <!-- Hidden field -->
                        <asp:HiddenField ID="hiddenModalVisible" runat="server" Value="true" />
                    </asp:Panel>

                    <asp:LinkButton ID="lnkFakeModuleLicence" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popupModuleLicence" runat="server" DropShadow="false"
                        PopupControlID="pnlAddEditModuleLicence" TargetControlID="lnkFakeModuleLicence"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>

                </ContentTemplate>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView2" />
                    <asp:AsyncPostBackTrigger ControlID="btnSaveModuleLicence" />
                    <asp:PostBackTrigger ControlID="UploadLineItems" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="activatedailicence" role="tabpanel">
            <!-- AI Licences list  -->
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <asp:GridView ID="GridView4" runat="server"></asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="ailicencerenewal" role="tabpanel">
            <!-- AI Licences Renewal  -->
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <asp:GridView ID="GridView5" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddAIRenewal" Text="Add New" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" />
                        <div class="alert alert-dismissible alert-secondary" style="margin-top: 15px; margin-right: 15px">
                            <h5 class="alert-heading">Note:</h5>
                            <h6>
                                <p>List of AI Licence Renewal record for the past 3 years</p>
                            </h6>
                        </div>
                    </div>


                    <!-- Modal body -->
                    <asp:Panel ID="pnlAddAIRenewal" runat="server" CssClass="modalPopup">
                        <h3 style="padding: 10px 0px 20px 20px">
                            <asp:Label ID="ModalHeaderAIRenewal" runat="server"></asp:Label></h3>
                        <table border="0" class="modalPopupTable" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_AI_PO_No" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_AI_PO_No" runat="server" ValidationGroup="AIRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_AI_PO_No">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_AI_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_AI_PO_Date" runat="server" ValidationGroup="AIRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_AI_PO_Date">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Chargeable</label>
                                    <asp:DropDownList ID="DDL_AI_Chargeable" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>Requestor</label>
                                    <asp:DropDownList ID="DDL_AI_Sales_Representative" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>Currency</label>
                                    <asp:DropDownList ID="DDL_AI_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Fee</label>
                                    <asp:TextBox ID="TB_AI_Fee" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_AI_Fee" runat="server" ValidationGroup="AIRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_AI_Fee">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_AI_Fee" runat="server" ValidationGroup="AIRenewal"
                                        ErrorMessage="Fee must be numeric value" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_AI_Fee"
                                        ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 60%; padding: 0px 10px 10px 0px" colspan="2">
                                    <label>Remarks</label>
                                    <asp:TextBox ID="TB_AI_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                                </td>
                                <td style="width: 40%; padding: 0px 10px 0px 0px"></td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 0px 10px 10px 0px" colspan="3">
                                    <hr />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 70%; padding: 0px 10px 20px 0px" colspan="2">
                                    <label>AI Licence Key</label>
                                    <asp:DropDownList ID="DDL_AI_Licence" runat="server" AutoPostBack="true" CssClass="form-control" AppendDataBoundItems="true">
                                        <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareValidator_AI_Licence" runat="server" ValidationGroup="AIRenewal"
                                        ErrorMessage="Please select a license key" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        Operator="NotEqual" ValueToCompare="-1"
                                        ControlToValidate="DDL_AI_Licence" Type="String">
                                    </asp:CompareValidator>
                                </td>
                                <td style="width: 30%; padding: 25px 10px 20px 0px">
                                    <asp:Button ID="btnAddAILineItems" runat="server" Text="Add" CssClass="btn btn-info" Height="40px" ValidationGroup="AIRenewal" />
                                    <asp:Button ID="btnClearAILineItems" runat="server" Text="Clear" CssClass="btn btn-info" Height="40px" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 0px 10px 0px 0px" colspan="3">
                                    <div class="GridList">
                                        <asp:GridView ID="GridView_AI_List" runat="server"
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
                                                <asp:BoundField DataField="Licence_Code" HeaderText="Licence Code" SortExpression="Licence_Code" />
                                                <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" />
                                                <asp:BoundField DataField="Fee" HeaderText="Fee" SortExpression="Fee" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                            <tr id="AILicencelistboxerrormsg" runat="server" style="vertical-align: top">
                                <td style="width: 800px; padding: 00px 0px 0px 0px" colspan="3">
                                    <span style="font-size: 12px; color: #dc3545">Please add AI Licence for renewal</span>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                                    <asp:Button ID="btnSaveAIRenewal" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="AIRenewal" />
                                    <asp:Button ID="btnCancelAIRenewal" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <asp:LinkButton ID="lnkFakeAIRenewal" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popupAIRenewal" runat="server" DropShadow="false"
                        PopupControlID="pnlAddAIRenewal" TargetControlID="lnkFakeAIRenewal"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>
                </ContentTemplate>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView5" />
                    <asp:AsyncPostBackTrigger ControlID="btnSaveAIRenewal" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Licence Notes -->
    <div id="LicenceNotes" runat="server" style="margin-top: 40px">
        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
            <ContentTemplate>
                <div class="card border-0 mb-3">
                    <div style="padding-left: 15px; padding-right: 12px">
                        <h4 class="card-title">Notes</h4>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="GridView6" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddNotes" Text="Add New" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" />
                    </div>
                </div>

                <!-- Modal body -->
                <asp:Panel ID="pnlAddNotes" runat="server" CssClass="modalPopup">
                    <h3 style="padding: 10px 0px 20px 20px">
                        <asp:Label ID="ModalHeaderNotes" runat="server"></asp:Label></h3>
                    <table border="0" class="modalPopupTable" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 800px; padding: 0px 10px 20px 0px">
                                <label>Notes</label>
                                <asp:TextBox ID="TB_Notes" runat="server" TextMode="MultiLine" placeholder="Type note here.."></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Notes" runat="server" ValidationGroup="Notes"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Notes">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="4">
                                <asp:Button ID="btnSaveNotes" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="Notes" CommandArgument="Create" />
                                <asp:Button ID="btnCancelNotes" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:LinkButton ID="lnkFakeNotes" runat="server"></asp:LinkButton>
                <cc1:ModalPopupExtender ID="popupNotes" runat="server" DropShadow="false"
                    PopupControlID="pnlAddNotes" TargetControlID="lnkFakeNotes"
                    BackgroundCssClass="modalBackground">
                </cc1:ModalPopupExtender>
            </ContentTemplate>

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="GridView6" />
                <asp:AsyncPostBackTrigger ControlID="btnSaveNotes" />
            </Triggers>

        </asp:UpdatePanel>
    </div>

    <hr style="margin-top: 100px"/>

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>


    <!-- javascript for Nested GridView effect -->
    <script src="/Scripts/jquery-1.3.2.min.js"></script> <!-- need to use jquery-1.3.2.min.js -->
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "/Images/minus-square-solid-24.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "/Images/plus-square-solid-24.png");
            $(this).closest("tr").next().remove();
        });
    </script>


    <!-- Keep where last tab selected -->
    <script>
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
                localStorage.setItem('activeTab', $(e.target).attr('href'));
                localStorage.setItem('activeLink', $(e.target).attr('id'));
            });

            var activeTab = localStorage.getItem('activeTab');
            //var activeLink = localStorage.getItem('activeLink');

            if (activeTab) {
                $('#myTab a[href="' + activeTab + '"]').tab('show');  // Show current tab
                //var element = document.getElementById("tab_" + activeLink);
                //element.classList.add("active");
            }
        });
    </script>

</asp:Content>

