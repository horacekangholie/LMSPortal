<%@ Page Title="LMSPortal - Hardkey Licence" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Hardkey_Licence.aspx.vb" 
    Inherits="Listings_Hardkey_Licence" %>

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
            BlockUI("<%=pnlUpdateHardkeyLicenceOrder.ClientID %>");
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

            <asp:Panel ID="pnlUpdateHardkeyLicenceOrder" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderHardkeyLicenceOrder" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 800px; padding: 0px 10px 20px 0px" colspan="2">
                            <label>PO No</label>
                            <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control disabled" Enabled="false"></asp:TextBox>
                            <asp:TextBox ID="TB_Customer_ID" runat="server" Visible="false"></asp:TextBox><!--Hiddenfield-->
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>SO No</label>
                            <asp:TextBox ID="TB_SO_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_SO_No" runat="server" ValidationGroup="HardkeyLicence"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_SO_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>SO Date</label>
                            <asp:TextBox ID="TB_SO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_SO_Date" runat="server" ValidationGroup="HardkeyLicence"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_SO_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>Invoice No</label>
                            <asp:TextBox ID="TB_Invoice_No" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_No" runat="server" ValidationGroup="HardkeyLicence"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_No">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 400px; padding: 0px 10px 20px 0px">
                            <label>Invoice Date</label>
                            <asp:TextBox ID="TB_Invoice_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_Date" runat="server" ValidationGroup="HardkeyLicence"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnUpdateHardkeyLicenceOrder" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="HardkeyLicence" />
                            <asp:Button ID="btnCancelHardkeyLicenceOrder" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:LinkButton ID="lnkFakeHardkeyLicenceOrder" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupHardkeyLicenceOrder" runat="server" DropShadow="false"
                PopupControlID="pnlUpdateHardkeyLicenceOrder" TargetControlID="lnkFakeHardkeyLicenceOrder"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnUpdateHardkeyLicenceOrder" />
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

</asp:Content>

