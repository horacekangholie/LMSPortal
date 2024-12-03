<%@ Page Title="LMSPortal - Register Hardkey Licence" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Hardkey_Licence_Form.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Form_Hardkey_Licence_Form" %>

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
            BlockUI("<%=pnlAddEditHardkey.ClientID %>");
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

    <hr />

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Hardkey Licence Details -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="card border-light mb-3">
                <div class="card-header">
                    <h4 class="card-title">Hardkey Licence</h4>
                </div>
                <div class="card-body">
                    <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                    <asp:Button ID="btnAddHardkey" Text="Add New" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                </div>
            </div>

            <!-- Modal body -->
            <asp:Panel ID="pnlAddEditHardkey" runat="server" CssClass="modalPopup">
                <h3 style="padding:10px 0px 20px 20px"><asp:Label ID="ModalHeaderHardkey" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width:100%">
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>PO No.</label>
                            <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%; padding: 0px 10px 20px 0px">
                            <label>PO Date</label>
                            <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%; padding: 0px 10px 20px 0px"></td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Licence No.</label>
                            <asp:TextBox ID="TB_Licence_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Licence_No" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Licence_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%; padding: 0px 10px 20px 0px">
                            <label>Start Date</label>
                            <asp:TextBox ID="TB_Start_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Start_Date" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Start_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 25%; padding: 0px 10px 20px 0px">
                            <label>End Date</label>
                            <asp:TextBox ID="TB_End_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_End_Date" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_End_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Licence Type</label>
                            <asp:DropDownList ID="DDL_Licence_Type" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
                        </td>
                        <td style="width: 50%; padding: 0px 10px 20px 0px" colspan="2">
                            <label>Sales Representative</label>
                            <asp:DropDownList ID="DDL_Sales_Representative" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Prepared By</label>
                            <asp:TextBox ID="TB_Prepared_By" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Prepared_By" runat="server" ValidationGroup="Hardkey"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Prepared_By">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 50%; padding: 0px 10px 20px 0px" colspan="2"></td>
                    </tr>
                    <tr style="vertical-align:bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnSaveHardkey" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="Hardkey" />
                            <asp:Button ID="btnCancelHardkey" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeHardkey" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupHardkey" runat="server" DropShadow="false" 
                PopupControlID="pnlAddEditHardkey" TargetControlID="lnkFakeHardkey"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveHardkey" />
        </Triggers>

    </asp:UpdatePanel>

    <hr />

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>

    <script type="text/javascript">
        function changeMode(txt1) {
            document.getElementById("PageContent_TB_End_Date").setAttribute('type', 'date');
        }
    </script>

</asp:Content>

