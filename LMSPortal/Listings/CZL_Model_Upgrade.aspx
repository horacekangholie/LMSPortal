<%@ Page Title="LMSPortal - Recover CZL Model Upgrade Fee Invoice" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Model_Upgrade.aspx.vb" 
    Inherits="Listings_CZL_Model_Upgrade" %>

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
            BlockUI("<%=pnlRecoverCZLModelUpgradeFee.ClientID %>");
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


    <!-- Modal Section -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- GridView -->
            <!-- GridView has to be inside panel to work -->
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>

            <asp:Panel ID="pnlRecoverCZLModelUpgradeFee" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderRecoverCZLModelUpgradeFee" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>PO No</label>
                            <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>PO Date</label>
                            <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_PO_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 400px; padding: 0px 10px 10px 0px">
                            <label>Invoice No</label>
                            <asp:TextBox ID="TB_Invoice_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_No" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 10px 0px">
                            <label>Invoice Date</label>
                            <asp:TextBox ID="TB_Invoice_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_Date" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>   
                </table>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>Chargeable</label>
                            <asp:DropDownList ID="DDL_Chargeable" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>Currency</label>
                            <asp:DropDownList ID="DDL_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>Fee</label>
                            <asp:TextBox ID="TB_Fee" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Fee" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Fee">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegEx_TB_Fee" runat="server" ValidationGroup="RecoverCZLModelUpgradeFee"
                                ErrorMessage="Fee must be numeric value" CssClass="invalid-feedback" 
                                Display="Dynamic" 
                                SetFocusOnError="True"
                                ControlToValidate="TB_Fee"
                                ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                </table>           
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px">
                            <asp:Button ID="btnUpdateRecoverCZLModelUpgradeFee" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="RecoverCZLModelUpgradeFee" />
                            <asp:Button ID="btnCancelRecoverCZLModelUpgradeFee" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                        <!-- Hidden field -->
                        <asp:TextBox ID="TB_CZL_Account_Unique_ID" runat="server" CssClass="form-control disabled" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_UID" runat="server" CssClass="form-control disabled" Visible="false"></asp:TextBox>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeRecoverCZLModelUpgradeFee" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupRecoverCZLModelUpgradeFee" runat="server" DropShadow="false"
                PopupControlID="pnlRecoverCZLModelUpgradeFee" TargetControlID="lnkFakeRecoverCZLModelUpgradeFee"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnUpdateRecoverCZLModelUpgradeFee" />
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

