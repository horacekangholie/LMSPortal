<%@ Page Title="LMSPortal - CZL Licensed Devices" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Licensed_Device_Form.aspx.vb" 
    MaintainScrollPositionOnPostback="true"
    EnableEventValidation="false"
    Inherits="Form_CZL_Licensed_Device_Form" %>

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
            BlockUI("<%=pnlAddEditCZLDevice.ClientID %>");  // For the popup modal
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditCZLChangeLog.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlAddEditNotes.ClientID %>");
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
            <h4 class="card-title">Account Details</h4>
        </div>
        <div class="card-body">
            <asp:FormView ID="FormView1" runat="server" Width="100%">
                <ItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Account ID</label>
                                <asp:TextBox ID="TB_CZL_Client_ID" Text='<%# Eval("Client_ID") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 45%; padding: 10px 10px 20px 0px">
                                <label>Account Name</label>
                                <asp:TextBox ID="TB_User_Group" Text='<%# Eval("User_Group") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Account Model</label>
                                <asp:TextBox ID="TB_Model" Text='<%# Eval("Account_Model") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Created Date</label>
                                <asp:TextBox ID="TB_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created_Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 40%; padding: 10px 10px 20px 0px">
                                <label>By Distributor / Owner</label>
                                <asp:Label ID="LB_By_Distributor" Text='<%# Eval("By_Distributor") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="By_Distributor_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                            <td style="width: 20%; padding: 10px 10px 20px 0px">
                                <label>Gen Version</label>
                                <asp:Label ID="LB_Gen_Version" Text='<%# Eval("Gen_Version") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Gen_Version" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Gen_Version_SqlDataSource"
                                    DataTextField="Gen_Version"
                                    DataValueField="Gen_Version"
                                    OnDataBound="DDL_Gen_Version_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Gen_Version_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Gen_Version FROM DB_Lookup WHERE Lookup_Name = 'CZL Gen Version' ORDER BY Value_2 DESC"></asp:SqlDataSource>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Effective Date</label>
                                <asp:TextBox ID="TB_Effective_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Effective_Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <hr />


    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- CZL Account Content -->
    <div id="CZLAccountContent" runat="server" style="margin-top: 40px">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" class="nav-item" role="presentation"><a id="link1" class="nav-link active" data-toggle="tab" href="#czldevicelist" aria-selected="true" role="tab"  style="font-size: 15px; width: 180px; text-align: center">CZL Device Listing</a></li>
            <li id="tab_link2" class="nav-item" role="presentation"><a id="link2" class="nav-link" data-toggle="tab" href="#czlaccountnotes" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">Account Notes</a></li>
        </ul>
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane active" id="czldevicelist" role="tabpanel">
                <!-- CZL Licenced Device List -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div id="DeviceListingSection" runat="server" style="padding: 10px 0px 40px 10px">
                            <!-- Searchbox for CZL Licenced Devices -->
                            <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 20px">
                                <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Device Serial, Device ID, Scale SN, MAC Address"></asp:TextBox>
                                <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView1" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Device_Serial" HeaderText="Device Serial" SortExpression="Device_Serial" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Unique_ID" HeaderText="Unique_ID" SortExpression="Unique_ID" ItemStyle-Width="0%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Device_ID" HeaderText="Device ID" SortExpression="Device_ID" ItemStyle-Width="10%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Licence_Key" HeaderText="Activation Key(s)" SortExpression="Licence_Key" ItemStyle-Width="12%" ItemStyle-Wrap="true" />
                                    <asp:BoundField DataField="Model" HeaderText="Model" SortExpression="Model" ItemStyle-Width="0%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Created_Date" HeaderText="Created On" SortExpression="Created_Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Effective_Date" HeaderText="CZL Eff. Date" SortExpression="Effective_Date" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="8%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="AI_Software_Version" HeaderText="AI Soft. Ver." SortExpression="AI_Software_Version" ItemStyle-Width="0%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="R_Version" HeaderText="R Vers" SortExpression="R_Version" ItemStyle-Width="0%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Scale_SN" HeaderText="Scale SN" SortExpression="Scale_SN" ItemStyle-Width="8%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" ItemStyle-Width="25%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="MAC_Addr" HeaderText="MAC Address" SortExpression="MAC_Addr" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Production_Licence_No" HeaderText="Prod. Licence No" SortExpression="Production_Licence_No" ItemStyle-Width="12%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Last_Updated" HeaderText="Last Updated" SortExpression="Last_Updated" DataFormatString="{0:dd MMM yy}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique_ID") & "|" & Eval("Device_Serial") & "|" & Eval("Device_ID") & "|" & Eval("Model") & "|" & Eval("AI_Software_Version") & "|" & Eval("R_Version") & "|" & Eval("Location") & "|" & Eval("Scale_SN") & "|" & Eval("MAC_Addr") & "|" & Eval("Production_Licence_No") & "|" & Eval("Effective_Date") & "|" & Eval("Client_ID") & "|" & Eval("CZL_Account_Unique_ID") & "|" & Eval("Licence_Key") %>'
                                                OnClick="Edit_CZL_Device_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique_ID") & "|" & Eval("Device_Serial") & "|" & Eval("Device_ID") & "|" & Eval("Model") & "|" & Eval("AI_Software_Version") & "|" & Eval("R_Version") & "|" & Eval("Location") & "|" & Eval("Scale_SN") & "|" & Eval("MAC_Addr") & "|" & Eval("Production_Licence_No") & "|" & Eval("Effective_Date") & "|" & Eval("Client_ID") & "|" & Eval("CZL_Account_Unique_ID") & "|" & Eval("Licence_Key") %>'
                                                OnClick="Delete_CZL_Device_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddCZLDevice" Text="Add Device" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCZLDevice" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px">
                                <asp:Label ID="ModalHeaderCZLDevice" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 250px; padding: 0px 10px 10px 0px">
                                        <label>Device Serial</label>
                                        <asp:TextBox ID="TB_Device_Serial" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Device_Serial" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Device_Serial">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 400px; padding: 0px 10px 10px 0px">
                                        <label>Device ID</label>
                                        <asp:TextBox ID="TB_Device_ID" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Device_ID" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Device_ID">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 150px; padding: 0px 10px 10px 0px">
                                        <label>Model</label>
                                        <asp:TextBox ID="TB_Model" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Model" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Model">
                                        </asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_Model" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="Model must be numeric" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Model"
                                            ValidationExpression="^(0|[1-9]\d*)$">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 200px; padding: 0px 10px 10px 0px">
                                        <label>AI Software Version</label>
                                        <asp:TextBox ID="TB_AI_Software_Version" runat="server" CssClass="form-control" MaxLength="10" placeholder="(optional)"></asp:TextBox>
                                    </td>
                                    <td style="width: 150px; padding: 0px 10px 10px 0px">
                                        <label>R Version</label>
                                        <asp:TextBox ID="TB_R_Version" runat="server" CssClass="form-control" MaxLength="10" placeholder="(optional)"></asp:TextBox>
                                    </td>
                                    <td style="width: 450px; padding: 0px 10px 10px 0px">
                                        <label>Location</label>
                                        <asp:TextBox ID="TB_Location" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Location" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Location">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 240px; padding: 0px 10px 10px 0px">
                                        <label>Scale Serial No</label>
                                        <asp:TextBox ID="TB_Scale_Serial_No" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Scale_Serial_No" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Scale_Serial_No">
                                        </asp:RequiredFieldValidator>
                                        <asp:Label ID="czldeviceserialexisterrormsg" runat="server" Text="Scale serial number exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                                    </td>
                                    <td style="width: 240px; padding: 0px 10px 10px 0px">
                                        <label>MAC Address</label>
                                        <asp:TextBox ID="TB_MAC_Address" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegEx_TB_MAC_Address" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="MAC Address format incorrect" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_MAC_Address"
                                            ValidationExpression="^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})|([0-9a-fA-F]{4}\\.[0-9a-fA-F]{4}\\.[0-9a-fA-F]{4})$">
                                        </asp:RegularExpressionValidator>
                                    </td>
                                    <td style="width: 320px; padding: 0px 10px 10px 0px">
                                        <label>Production Licence No</label>
                                        <asp:TextBox ID="TB_Production_Licence_No" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 300px; padding: 0px 10px 10px 0px">
                                        <label>Device Effective Date</label><p class="text-success" style="font-family: 'Franklin Gothic Medium', 'Arial Narrow', 'Arial', 'sans-serif'; float: left">&nbsp;</p>
                                        <asp:TextBox ID="TB_Effective_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Effective_Date" runat="server" ValidationGroup="CZLDevice"
                                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Effective_Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td style="width: 500px; padding: 0px 10px 10px 0px" colspan="2">
                                        <label id="lbl_client_id" runat="server" style="float: left">CZL Client ID</label>
                                        <p id="p_client_id_instruction" runat="server" class="text-success" style="font-family: 'Franklin Gothic Medium', 'Arial Narrow', 'Arial', 'sans-serif'; float: left; margin-left: 10px">Change Client ID to transfer account</p>
                                        <asp:DropDownList ID="DDL_CZL_Client_ID" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Selected_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Device_Serial" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Device_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Model" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_AI_Software_Version" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_R_Version" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Location" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Scale_Serial_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_MAC_Address" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Production_Licence_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Effective_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Client_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_CZL_Account_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Selected_Bind_Licence_Key" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnSaveCZLDevice" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="CZLDevice" />
                                        <asp:Button ID="btnCancelCZLDevice" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeCZLDevice" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupCZLDevice" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCZLDevice" TargetControlID="lnkFakeCZLDevice"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView1" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveCZLDevice" />
                    </Triggers>

                </asp:UpdatePanel>

                <!-- CZL Change Log -->
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div id="DeviceLogSection" runat="server" style="padding: 10px 0px 40px 10px">
                            <!-- Separator line -->
                            <hr style="margin: 0px 15px 20px 0px"/>
                            <div id="ChangeLog_Header" runat="server" style="text-align: left">
                                <div style="margin-left: 10px">
                                    <h5 style="float: left; vertical-align: baseline;margin-right: 10px">Serial No: </h5>
                                    <h5 style="float: left; margin-bottom: 0px"><asp:Label ID="LB_Header_Serial_No" runat="server" Visible="false" Font-Bold="true"></asp:Label></h5>
                                    <h5 style="float: left; vertical-align: baseline;margin-right: 10px; clear:both">Activation Key(s): </h5>
                                    <h5 style="float: left; margin-bottom: 50px">
                                        <table border="0" class="modalPopupTable" style="width: 100%">
                                            <tr style="vertical-align: top">
                                                <td style="width: 200px">
                                                    <asp:Label ID="LB_Header_Licence_Key" runat="server" Visible="false" Font-Bold="true"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </h5>
                                </div>
                                <div style="margin-left:10px; clear:both"><h5>Comments:</h5></div>
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView2" runat="server">
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
                                                OnClick="Edit_Change_Log_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Unique_ID") %>'
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                OnClick="Delete_Change_Log_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <!-- Control button -->
                            <asp:Button ID="btnAddCZLChangeLog" Text="Add Comment" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" CommandName="Single_Log" Visible="false" />
                            <asp:Button ID="btnLogBatchUpdate" Text="Add Comments (Batch)" runat="server" CssClass="btn btn-sm btn-info" CommandName="Multiple_Log" CausesValidation="false" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditCZLChangeLog" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderCZLChangeLog" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 20px 0px">
                                        <label>Comment</label>
                                        <asp:TextBox ID="TB_CZLChangeLog" runat="server" CssClass="form-control" placeholder="Type note here.."></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_CZLChangeLog" runat="server" ValidationGroup="CZLChangeLog"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_CZLChangeLog">
                                        </asp:RequiredFieldValidator>
                                        <!-- Hidden fields -->
                                        <asp:TextBox ID="TB_Selected_CZLChangeLog_Parent_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:TextBox ID="TB_Selected_CZLChangeLog_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:TextBox ID="TB_Selected_CZLChangeLog_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:TextBox ID="TB_Selected_Button_Command_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="4">
                                        <asp:Button ID="btnSaveCZLChangeLog" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="CZLChangeLog" />
                                        <asp:Button ID="btnCancelCZLChangeLog" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeCZLChangeLog" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupCZLChangeLog" runat="server" DropShadow="false"
                            PopupControlID="pnlAddEditCZLChangeLog" TargetControlID="lnkFakeCZLChangeLog"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>

                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView2" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveCZLChangeLog" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
            <div class="tab-pane" id="czlaccountnotes" role="tabpanel">
                <!-- CZL Account Log -->
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div id="AccountNotesSection" runat="server" style="padding: 10px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                            <div id="Div1" runat="server" style="text-align: left">
                                <div style="margin-top: 40px; margin-left:10px"><h5>Notes:</h5></div>
                            </div>

                            <!-- Gridview -->
                            <asp:GridView ID="GridView3" runat="server">
                                <Columns>
                                    <asp:BoundField DataField="Added_Date" HeaderText="Date" SortExpression="Added_Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="5%" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Notes" HeaderText="Notes" SortExpression="Notes" ItemStyle-Width="95%" ItemStyle-Wrap="false" />
                                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="EditLinkButton" runat="server"
                                                CommandArgument='<%# Eval("ID") %>'
                                                Text="<i class='bi bi-pencil-fill'></i>"
                                                CssClass="btn btn-xs btn-info"
                                                CausesValidation="false"
                                                OnClick="Edit_Notes_Click">
                                            </asp:LinkButton>
                                            <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                                CommandArgument='<%# Eval("ID") %>'
                                                Text="<i class='bi bi-trash'></i>"
                                                CssClass="btn btn-xs btn-danger"
                                                CausesValidation="false"
                                                OnClick="Delete_Notes_Click"
                                                OnClientClick="return confirm('Are you sure to delete record?')">
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:Button ID="btnAddNotes" Text="Add Note" runat="server" CausesValidation="false" CssClass="btn btn-sm btn-info" />
                        </div>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlAddEditNotes" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px">
                                <asp:Label ID="ModalHeaderNotes" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 20px 0px">
                                        <label>Notes</label>
                                        <asp:TextBox ID="TB_Notes" runat="server" CssClass="form-control" placeholder="Type note here.."></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredField_TB_Notes" runat="server" ValidationGroup="Notes"
                                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="True"
                                            ControlToValidate="TB_Notes">
                                        </asp:RequiredFieldValidator>
                                        <!-- Hidden fields -->
                                        <asp:TextBox ID="TB_Selected_Notes_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
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
                            PopupControlID="pnlAddEditNotes" TargetControlID="lnkFakeNotes"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView3" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveNotes" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
        </div>
    </div>


    <!-- Control Button -->
    <div style="clear: both">
        <br /><hr style="margin-top: 50px"/>
        <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>
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


    <!-- Keep where last tab selected -->
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

