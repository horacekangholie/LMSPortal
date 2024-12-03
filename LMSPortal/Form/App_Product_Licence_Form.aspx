<%@ Page Title="LMSPortal - Register App / Product Licence" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="App_Product_Licence_Form.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Form_App_Product_Licence_Form" %>

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
            BlockUI("<%=pnlAddEditAppProductLicence.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddTermedLicenceRenewal.ClientID %>");
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

    <!-- App / Product Licence Content -->
    <div id="AppProductLicenceContent" runat="server" style="margin-top: 40px">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" class="nav-item" role="presentation"><a id="link1" class="nav-link active" data-toggle="tab" href="#licenceusage" aria-selected="true" role="tab"  style="font-size: 15px; width: 220px; text-align: center">Licence Usage Overview</a></li>
            <li id="tab_link2" class="nav-item" role="presentation"><a id="link2" class="nav-link" data-toggle="tab" href="#appproductlicenceorder" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 250px; text-align: center">App / Product Licence Order</a></li>
            <li id="tab_link3" class="nav-item" role="presentation"><a id="link3" class="nav-link" data-toggle="tab" href="#termedlicence" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 200px; text-align: center">Termed Licence</a></li>
            <li id="tab_link4" class="nav-item" role="presentation"><a id="link4" class="nav-link" data-toggle="tab" href="#termedlicencerenewal" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 220px; text-align: center">Termed Licence Renewal</a></li>
            <li id="tab_link5" class="nav-item" role="presentation"><a id="link5" class="nav-link" data-toggle="tab" href="#aigatewaytoken" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 220px; text-align: center">AI Gateway Token</a></li>
        </ul>
    </div>
    <div class="tab-content" id="myTabContent">
        <div class="tab-pane active" id="licenceusage" role="tabpanel">
            <!-- Licence Usage overview -->
            <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                <asp:GridView ID="GridView2" runat="server"></asp:GridView>

                <!--Add a refresh linkbutton to sync latest license status-->
                <div style="margin-top: 20px">
                    <asp:LinkButton ID="AILicenceRefresh" runat="server" CssClass="btn btn-default"></asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="tab-pane" id="appproductlicenceorder" role="tabpanel">
            <!-- App / Product Licence Details -->
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <!-- Searchbox for Licence -->
                        <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 0px">
                            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type to search..."></asp:TextBox>
                            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
                        </div>

                        <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddAppProductLicence" Text="Add New" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                    </div>

                    <!-- Modal body -->
                    <asp:Panel ID="pnlAddEditAppProductLicence" runat="server" CssClass="modalPopup">
                        <h3 style="padding: 10px 0px 20px 20px">
                            <asp:Label ID="ModalHeaderAppProductLicence" runat="server"></asp:Label></h3>
                        <table border="0" class="modalPopupTable" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="AppProductLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_PO_No">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="AppProductLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_PO_Date">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Application Type</label>
                                    <asp:DropDownList ID="DDL_Application_Type" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
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
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Email" runat="server" ValidationGroup="AppProductLicence"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Email">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_Email" runat="server" ValidationGroup="AppProductLicence"
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
                                    <asp:FileUpload ID="FileUpload1" runat="server" />
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
                            <tr id="licencelistboxerrormsg" runat="server" style="vertical-align: top">
                                <td style="width: 800px; padding: 0px 0px 0px 0px" colspan="3">
                                    <span style="font-size: 12px; color: #dc3545">Please upload licence file</span>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                                    <asp:Button ID="btnSaveAppProductLicence" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="AppProductLicence" />
                                    <asp:Button ID="btnCancelAppProductLicence" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                        <!-- Hidden field -->
                        <asp:HiddenField ID="hiddenModalVisible" runat="server" Value="true" />
                    </asp:Panel>

                    <asp:LinkButton ID="lnkFakeAppProductLicence" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popupAppProductLicence" runat="server" DropShadow="false"
                        PopupControlID="pnlAddEditAppProductLicence" TargetControlID="lnkFakeAppProductLicence"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>

                </ContentTemplate>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView1" />
                    <asp:AsyncPostBackTrigger ControlID="btnSaveAppProductLicence" />
                    <asp:PostBackTrigger ControlID="UploadLineItems" />
                </Triggers>

            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="termedlicence" role="tabpanel">
            <!-- Termed Licences  -->
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <asp:GridView ID="GridView3" runat="server"></asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="termedlicencerenewal" role="tabpanel">
            <!-- Termed Licenced Renewal -->
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                        <asp:GridView ID="GridView4" runat="server"></asp:GridView>
                        <asp:Button ID="btnAddTermedLicenceRenewal" Text="Add New" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" />
                        <div class="alert alert-dismissible alert-secondary" style="margin-top: 15px; margin-right: 10px">
                            <h5 class="alert-heading">Note:</h5>
                            <h6>
                                <p>List of Termed Licence Renewal record for the past 3 years</p>
                            </h6>
                        </div>
                    </div>

                    <!-- Modal body -->
                    <asp:Panel ID="pnlAddTermedLicenceRenewal" runat="server" CssClass="modalPopup">
                        <h3 style="padding: 10px 0px 20px 20px">
                            <asp:Label ID="ModalHeaderTermedLicenceRenewal" runat="server"></asp:Label></h3>
                        <table border="0" class="modalPopupTable" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_Termed_Licence_PO_No" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Termed_Licence_PO_No" runat="server" ValidationGroup="TermedLicenceRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Termed_Licence_PO_No">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_Termed_Licence_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Termed_Licence_PO_Date" runat="server" ValidationGroup="TermedLicenceRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Termed_Licence_PO_Date">
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Chargeable</label>
                                    <asp:DropDownList ID="DDL_Termed_Licence_Chargeable" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 45%; padding: 0px 10px 20px 0px">
                                    <label>Requestor</label>
                                    <asp:DropDownList ID="DDL_Termed_Licence_Sales_Representative" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 25%; padding: 0px 10px 20px 0px">
                                    <label>Currency</label>
                                    <asp:DropDownList ID="DDL_Termed_Licence_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                                </td>
                                <td style="width: 30%; padding: 0px 10px 20px 0px">
                                    <label>Fee</label>
                                    <asp:TextBox ID="TB_Termed_Licence_Fee" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Termed_Licence_Fee" runat="server" ValidationGroup="TermedLicenceRenewal"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Termed_Licence_Fee">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_Termed_Licence_Fee" runat="server" ValidationGroup="TermedLicenceRenewal"
                                        ErrorMessage="Fee must be numeric value" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Termed_Licence_Fee"
                                        ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 60%; padding: 0px 10px 10px 0px" colspan="2">
                                    <label>Remarks</label>
                                    <asp:TextBox ID="TB_Termed_Licence_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
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
                                    <label>Termed Licence Key</label>
                                    <asp:DropDownList ID="DDL_Termed_Licence" runat="server" AutoPostBack="true" CssClass="form-control" AppendDataBoundItems="true">
                                        <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareValidator_Termed_Licence" runat="server" ValidationGroup="TermedLicenceRenewal"
                                        ErrorMessage="Please select a license key" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        Operator="NotEqual" ValueToCompare="-1"
                                        ControlToValidate="DDL_Termed_Licence" Type="String">
                                    </asp:CompareValidator>
                                </td>
                                <td style="width: 30%; padding: 25px 10px 20px 0px">
                                    <asp:Button ID="btnAddTermedLicenceLineItems" runat="server" Text="Add" CssClass="btn btn-info" Height="40px" ValidationGroup="TermedLicenceRenewal" />
                                    <asp:Button ID="btnClearTermedLicenceLineItems" runat="server" Text="Clear" CssClass="btn btn-info" Height="40px" CausesValidation="false" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 0px 10px 0px 0px" colspan="3">
                                    <div class="GridList">
                                        <asp:GridView ID="GridView_Termed_Licence_List" runat="server"
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
                            <tr id="TermedLicencelistboxerrormsg" runat="server" style="vertical-align: top">
                                <td style="width: 800px; padding: 00px 0px 0px 0px" colspan="3">
                                    <span style="font-size: 12px; color: #dc3545">Please add Termed Licence for renewal</span>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 800px; padding: 20px 10px 10px 0px" colspan="3">
                                    <asp:Button ID="btnSaveTermedLicenceRenewal" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="TermedLicenceRenewal" />
                                    <asp:Button ID="btnCancelTermedLicenceRenewal" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <asp:LinkButton ID="lnkFakeTermedLicenceRenewal" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popupTermedLicenceRenewal" runat="server" DropShadow="false"
                        PopupControlID="pnlAddTermedLicenceRenewal" TargetControlID="lnkFakeTermedLicenceRenewal"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="tab-pane" id="aigatewaytoken" role="tabpanel">
            <!-- AI Gateway Token -->
            <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                <!-- Searchbox for Gateway Token -->
                <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 0px">
                    <asp:TextBox ID="TB_Search_Token" runat="server" CssClass="form-control" Width="400" placeholder="Type to search..."></asp:TextBox>
                    <asp:Button ID="BT_Search_Token" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
                </div>

                <asp:GridView ID="GridView6" runat="server"></asp:GridView>
            </div>
        </div>
    </div>

    <!-- Licence Notes -->
    <div id="LicenceNotes" runat="server" style="margin-top: 40px">
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <div class="card border-0 mb-3">
                    <div style="padding-left: 15px; padding-right: 12px">
                        <h4 class="card-title">Notes</h4>
                    </div>
                    <div class="card-body">
                        <asp:GridView ID="GridView5" runat="server"></asp:GridView>
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
                <asp:AsyncPostBackTrigger ControlID="GridView5" />
                <asp:AsyncPostBackTrigger ControlID="btnSaveNotes" />
            </Triggers>

        </asp:UpdatePanel>
    </div>


    <hr style="margin-top: 100px"/>

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>

    <script type="text/javascript">
        function changeMode(txt1) {
            document.getElementById("PageContent_TB_End_Date").setAttribute('type', 'date');
        }
    </script>

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

