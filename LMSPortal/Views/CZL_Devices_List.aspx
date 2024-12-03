<%@ Page Title="LMSPortal - Device Listing" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Devices_List.aspx.vb" 
    EnableEventValidation="false"
    Inherits="Views_CZL_Devices_List" %>

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
            BlockUI("<%=pnlTransferLDevice.ClientID %>");  // For the popup modal
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
        .sort-arrow {
            margin-left: 5px; /* Add some spacing between text and arrow */
        }

        .asc::after {
            content: '\25B2'; /* Unicode character for upward arrow */
        }

        .desc::after {
            content: '\25BC'; /* Unicode character for downward arrow */
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <div class="card border-light mb-3">
        <div class="card-body">
            <!-- Country dropdownlist -->
            <div  style="float: left; width: 25%">
                <label>Country</label>
                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
            </div>

            <!-- By Distributor / Owner dropdownlist -->
            <!-- The list is populated based on Country dropdownlist-->
            <div style="float: left; width: 25%; margin-left: 10px">
                <label>Distributor / Owner</label>
                <asp:DropDownList ID="DDL_By_Distributor" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
            </div>

            <!-- CZL Account -->
            <!-- The list is populated based on DDL_By_Distributor / Owner dropdownlist-->
            <div style="float: left; width: 25%; margin-left: 10px">
                <label>CZL Account</label>
                <asp:DropDownList ID="DDL_CZL_Client_ID" runat="server" CssClass="form-control" AutoPostBack="true"></asp:DropDownList>
            </div>

            <div style="margin-top:100px">
                <!-- Searchbox -->
                <div class="form-group form-inline" style="float: right; margin-right: 15px">
                    <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="500" placeholder="Device Serial, Device ID, Scale SN, Location" TextMode="Search"></asp:TextBox>
                    <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
                </div>

                <!-- Download button -->
                <div class="form-group form-inline">
                    <asp:LinkButton ID="BT_Download_Excel" runat="server" CssClass="btn btn-default" Text="Download excel" OnClientClick="setDownloadTarget(); return true;" Visible="false"></asp:LinkButton>
                    <asp:Label ID="ReportSQL" runat="server" Visible="false"></asp:Label>
                </div>

                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                
                <!-- Registered device list -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <!-- Gridview -->
                        <asp:GridView ID="GridView1" runat="server">
                            <Columns>
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" ItemStyle-Width="5%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Expiry_Date" HeaderText="Expiry Date" SortExpression="Expiry_Date" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Licence_Key" HeaderText="Binding Key" SortExpression="Licence_Key" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Device_Serial" HeaderText="Device Serial" SortExpression="Device_Serial" ItemStyle-Width="10%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Device_ID" HeaderText="Device ID" SortExpression="Device_ID" ItemStyle-Width="15%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Model" HeaderText="Model" SortExpression="Model" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Created_Date" HeaderText="Created On" SortExpression="Created_Date" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" DataFormatString="{0:yyyy-MM-dd}" />
                                <asp:BoundField DataField="AI_Software_Version" HeaderText="AI Soft. Ver." SortExpression="AI_Software_Version" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="R_Version" HeaderText="R Vers" SortExpression="R_Version" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Scale_SN" HeaderText="Scale SN" SortExpression="Scale_SN" ItemStyle-Width="5%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Location" HeaderText="Location" SortExpression="Location" ItemStyle-Width="25%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="MAC_Addr" HeaderText="MAC Address" SortExpression="MAC_Addr" ItemStyle-Width="10%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:BoundField DataField="Production_Licence_No" HeaderText="Prod. Licence No" SortExpression="Production_Licence_No" ItemStyle-Width="10%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                                <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="TransferLinkButton" runat="server"
                                            Text="<i class='bi bi-arrow-down-up'></i>"
                                            CssClass="btn btn-xs btn-info"
                                            ToolTip="Transfer CZL Account"
                                            CausesValidation="false"
                                            CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("Unique_ID") & "|" & Eval("Distributor_Code") & "|" & Eval("Account_ID") & "|" & Eval("Account_Name") %>'
                                            OnClick="TransferLinkButton_Click">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                        <!-- Modal body -->
                        <asp:Panel ID="pnlTransferLDevice" runat="server" CssClass="modalPopup">
                            <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderTransferLDevice" runat="server"></asp:Label></h3>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: top">
                                    <td style="width: 800px; padding: 0px 10px 10px 0px" colspan="3">
                                        <div style="padding: 0px 0px 20px 0px">
                                            <asp:Label ID="TransferGuidedMessage" runat="server"></asp:Label>
                                        </div>
                                        <asp:DropDownList ID="DDL_Transfer_CZL_Client_ID" runat="server" CssClass="form-control"
                                            AppendDataBoundItems="True">
                                            <asp:ListItem Text="Please select" Value="-1" />
                                        </asp:DropDownList>
                                        <asp:CompareValidator ID="CompareValidator_DDL_Transfer_CZL_Client_ID" runat="server" ValidationGroup="TranferDevice"
                                            ErrorMessage="Please select account to move" CssClass="invalid-feedback"
                                            Display="Dynamic"
                                            SetFocusOnError="true"
                                            Operator="NotEqual" ValueToCompare="-1"
                                            ControlToValidate="DDL_Transfer_CZL_Client_ID" Type="String">
                                        </asp:CompareValidator>
                                    </td>
                                </tr>
                            </table> 
                            <!-- hidden field -->
                            <div>
                                <asp:TextBox ID="TB_Hidden_Selected_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Hidden_Selected_Unique_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Hidden_Selected_Distributor_Code" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Hidden_Selected_Existing_Client_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:TextBox ID="TB_Hidden_Selected_Existing_Client_Name" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            </div>
                            <table border="0" class="modalPopupTable" style="width: 100%">
                                <tr style="vertical-align: bottom">
                                    <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                        <asp:Button ID="btnTransferDevice" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="TranferDevice" />
                                        <asp:Button ID="btnCancelTransferDevice" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>

                        <asp:LinkButton ID="lnkFakeTransferDevice" runat="server"></asp:LinkButton>
                        <cc1:ModalPopupExtender ID="popupTransferDevice" runat="server" DropShadow="false"
                            PopupControlID="pnlTransferLDevice" TargetControlID="lnkFakeTransferDevice"
                            BackgroundCssClass="modalBackground">
                        </cc1:ModalPopupExtender>

                        <!-- Display total number of records from the GridView -->
                        <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
                    </ContentTemplate>

                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView1" />
                        <asp:AsyncPostBackTrigger ControlID="btnTransferDevice" />
                    </Triggers>

                </asp:UpdatePanel>
            </div>
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

    <script>
        function setDownloadTarget() {
            // Set target to open a new tab for download
            document.forms[0].target = '_blank';

            // Reset target to null after a delay (e.g., 100ms)
            setTimeout(function () {
                document.forms[0].target = '';
            }, 100);
        }
    </script>

</asp:Content>

