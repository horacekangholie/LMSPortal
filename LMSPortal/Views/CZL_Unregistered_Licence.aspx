<%@ Page Title="LMSPortal - Unregistered Licences" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Unregistered_Licence.aspx.vb" 
    EnableEventValidation="false"
    Inherits="Views_CZL_Unregistered_Licence" %>

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
            BlockUI("<%=pnlAssignUnregisteredLDevice.ClientID %>");  // For the popup modal
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

    <!-- Alert Message for unregistered device -->
    <div class="alert alert-info" style="float: left; width: 50%">
        <b>Note: </b>These AI Licence are activated but have not been registered in CZL accounts
    </div>

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float: right; margin-top: 15px">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>


    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Unregistered device list -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server">
                <Columns>
                    <asp:BoundField DataField="Licensee" HeaderText="Licensee" SortExpression="Licensee" ItemStyle-Width="12%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="Licence Code" HeaderText="Licence Code" SortExpression="Licence Code" ItemStyle-Width="13%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="Created Date" HeaderText="Created Date" SortExpression="Created Date" ItemStyle-Width="4%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Serial No" HeaderText="Serial No" SortExpression="Serial No" ItemStyle-Width="6%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="MAC Address" HeaderText="MAC Address" SortExpression="MAC Address" ItemStyle-Width="7%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="AI Device ID" HeaderText="AI Device ID" SortExpression="AI Device ID" ItemStyle-Width="13%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="AI Device Serial No" HeaderText="AI Device Serial No" SortExpression="AI Device Serial No" ItemStyle-Width="7%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="Activated Date" HeaderText="Activated Date" SortExpression="Activated Date" ItemStyle-Width="5%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Expired Date" HeaderText="Expired Date" SortExpression="Expired Date" ItemStyle-Width="5%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Requested By" HeaderText="Requested By" SortExpression="Requested By" ItemStyle-Width="8%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:BoundField DataField="Remarks" HeaderText="Remarks" SortExpression="Remarks" ItemStyle-Width="20%" ItemStyle-Wrap="false" HeaderStyle-Wrap="false" />
                    <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton ID="AssignLinkButton" runat="server"
                                Text="<i class='bi bi-send-fill'></i>"
                                CssClass="btn btn-xs btn-info"
                                ToolTip="Assign to CZL Account"
                                CausesValidation="false"
                                CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("AI Device Serial No") & "|" & Eval("AI Device ID") & "|" & Eval("Serial No") & "|" & Eval("MAC Address") & "|" & Eval("Customer ID") & "|" & Eval("Licensee") %>'
                                OnClick="Assign_Unregistered_Device_Click">
                            </asp:LinkButton>
                        </ItemTemplate>    
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <!-- Modal body -->
            <asp:Panel ID="pnlAssignUnregisteredLDevice" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderUnregisteredLDevice" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 800px; padding: 0px 10px 10px 0px" colspan="3">
                            <div style="padding: 0px 0px 20px 0px">
                                <asp:Label ID="AssignGuidedMessage" runat="server"></asp:Label>
                            </div>
                            <asp:DropDownList ID="DDL_CZL_Client_ID" runat="server" CssClass="form-control"
                                AppendDataBoundItems="True">
                                <asp:ListItem Text="Please select" Value="-1" />
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_DDL_CZL_Client_ID" runat="server" ValidationGroup="UnregisteredDevice"
                                ErrorMessage="Please select account to assign" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="true"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_CZL_Client_ID" Type="String">
                            </asp:CompareValidator>
                        </td>
                    </tr>
                </table> 
                <!-- hidden field -->
                <div>
                    <asp:TextBox ID="TB_Hidden_Selected_Row_Index" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_AI_Device_Serial_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_AI_Device_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_Serial_No" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_MAC_Address" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_Customer_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    <asp:TextBox ID="TB_Hidden_Selected_Licensee" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                </div>                 
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                            <asp:Button ID="btnAssignUnregisteredDevice" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="UnregisteredDevice" />
                            <asp:Button ID="btnCancelUnregisteredDevice" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeUnregisteredLDevice" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupAssignUnregisteredLDevice" runat="server" DropShadow="false"
                PopupControlID="pnlAssignUnregisteredLDevice" TargetControlID="lnkFakeUnregisteredLDevice"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnAssignUnregisteredDevice" />
        </Triggers>

    </asp:UpdatePanel>



    <!-- Display total number of records from the GridView -->
    <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>


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

