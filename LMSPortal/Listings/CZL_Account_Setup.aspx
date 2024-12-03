<%@ Page Title="LMSPortal - Recover CZL Account Setup Fee Invoice" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Account_Setup.aspx.vb" 
    Inherits="Listings_CZL_Account_Setup" %>

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
            BlockUI("<%=pnlAddNewCZLAccountSetupFee.ClientID %>");
            $.blockUI.defaults.css = {};
        });
        $(document).ready(function () {
            BlockUI("<%=pnlRecoverCZLAccountSetupFee.ClientID %>");
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

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float: right">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>

    <!-- Script Manager -->
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>


    <!-- Modal Section  - Add New Account Setup Fee -->
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <!-- Create new CZL account setup charge record -->
            <asp:LinkButton ID="BT_Create" runat="server" CssClass="btn btn-default" Text="Add New"></asp:LinkButton>

            <asp:Panel ID="pnlAddNewCZLAccountSetupFee" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderAddNewCZLAccountSetupFee" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>CZL Client ID</label>
                            <asp:DropDownList ID="DDL_CZL_Client_ID" runat="server" CssClass="form-control" Width="100%" AppendDataBoundItems="true">
                                <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_CZL_Client_ID" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="Please select Client ID" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_CZL_Client_ID" Type="String">
                            </asp:CompareValidator>
                        </td>
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Requested By</label>
                            <asp:DropDownList ID="DDL_Sales_Representative" runat="server" CssClass="form-control" Width="100%" AppendDataBoundItems="true">
                                <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Sales_Representative" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="Please select Requestor" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_Sales_Representative" Type="String">
                            </asp:CompareValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>PO No</label>
                            <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>PO Date</label>
                            <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Currency</label>
                            <asp:DropDownList ID="DDL_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td style="width: 50%; padding: 0px 10px 20px 0px">
                            <label>Fee</label>
                            <asp:TextBox ID="TB_Fee" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Fee" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Fee">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegEx_TB_Fee" runat="server" ValidationGroup="AddNewCZLAccountSetupFee"
                                ErrorMessage="Fee must be numeric value" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Fee"
                                ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnAddNewCZLAccountSetupFee" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="AddNewCZLAccountSetupFee" />
                            <asp:Button ID="btnCancelAddNewCZLAccountSetupFee" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeAddNewCZLAccountSetupFee" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupAddNewCZLAccountSetupFee" runat="server" DropShadow="false"
                PopupControlID="pnlAddNewCZLAccountSetupFee" TargetControlID="lnkFakeAddNewCZLAccountSetupFee"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="BT_Create" />
        </Triggers>

    </asp:UpdatePanel>


    <!-- Modal Section - Recover Invoice for Account Seupt Fee -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- GridView -->
            <!-- GridView has to be inside panel to work -->
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>

            <asp:Panel ID="pnlRecoverCZLAccountSetupFee" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderRecoverCZLAccountSetupFee" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 35%; padding: 0px 10px 20px 0px">
                            <label>Invoice No</label>
                            <asp:TextBox ID="TB_Invoice_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_No" runat="server" ValidationGroup="RecoverCZLAccountSetupFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 35%; padding: 0px 10px 20px 0px">
                            <label>Invoice Date</label>
                            <asp:TextBox ID="TB_Invoice_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_Date" runat="server" ValidationGroup="RecoverCZLAccountSetupFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 30%; padding: 0px 10px 20px 0px">
                            <label>SO No</label>
                            <asp:TextBox ID="TB_SO_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_SO_No" runat="server" ValidationGroup="RecoverCZLAccountSetupFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_SO_No">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                            <asp:Button ID="btnUpdateRecoverCZLAccountSetupFee" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="RecoverCZLAccountSetupFee" />
                            <asp:Button ID="btnCancelRecoverCZLAccountSetupFee" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                        <!-- Hidden field -->
                        <asp:TextBox ID="TB_CZL_Account_Unique_ID" runat="server" CssClass="form-control disabled" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_Distributor_ID" runat="server" CssClass="form-control disabled" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_Hidden_PO_No" runat="server" CssClass="form-control disabled" Visible="false"></asp:TextBox>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeRecoverCZLAccountSetupFee" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupRecoverCZLAccountSetupFee" runat="server" DropShadow="false"
                PopupControlID="pnlRecoverCZLAccountSetupFee" TargetControlID="lnkFakeRecoverCZLAccountSetupFee"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnUpdateRecoverCZLAccountSetupFee" />
        </Triggers>

    </asp:UpdatePanel>


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

