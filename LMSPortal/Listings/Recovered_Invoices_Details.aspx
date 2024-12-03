<%@ Page Title="LMSPortal - Invoice Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Recovered_Invoices_Details.aspx.vb" 
    Inherits="Listings_Recovered_Invoices_Details" %>

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
            BlockUI("<%=pnlAddEditBilledItem.ClientID %>");
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

    <!-- Script Manager -->
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Modal Section -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="card border-light mb-3">
                <div class="card-header">
                    <h4 class="card-title">Reference: <asp:LinkButton ID="RefLink" runat="server"><%=Request.QueryString("Invoice_No") %></asp:LinkButton></h4>
                </div>
                <div class="card-body">
                    <!-- GridView -->
                    <!-- GridView has to be inside panel to work -->
                    <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                    <asp:Button ID="btnAddBilledItem" Text="Add New" runat="server" CssClass="btn btn-sm btn-info" CausesValidation="false" />
                </div>
            </div>

            <asp:Panel ID="pnlAddEditBilledItem" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderBilledItem" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 800px; padding: 0px 10px 20px 0px" colspan="3">
                            <label>Item Code</label>
                            <asp:TextBox ID="TB_Old_Item_Code" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                            <asp:DropDownList ID="DDL_Item_Code" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 200px; padding: 0px 10px 20px 0px">
                            <label>Invoice Date</label>
                            <asp:TextBox ID="TB_Invoice_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Invoice_Date" runat="server" ValidationGroup="BilledItem"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Invoice_Date">
                            </asp:RequiredFieldValidator>
                        </td>
                        <td style="width: 100px; padding: 0px 10px 20px 0px">
                            <label>Currency</label>
                            <asp:DropDownList ID="DDL_Currency" runat="server" CssClass="form-control"></asp:DropDownList>
                        </td>
                        <td style="width: 500px; padding: 0px 10px 20px 0px">
                            <label>Amount</label>
                            <asp:TextBox ID="TB_Amount" runat="server" CssClass="form-control" placeholder="0.00"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Amount" runat="server" ValidationGroup="BilledItem"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Amount">
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="RegEx_TB_Amount" runat="server" ValidationGroup="BilledItem"
                                ErrorMessage="Amount must be numeric value" CssClass="invalid-feedback" 
                                Display="Dynamic" 
                                SetFocusOnError="True"
                                ControlToValidate="TB_Amount"
                                ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnSaveBilledItem" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="BilledItem" />
                            <asp:Button ID="btnCancelBilledItem" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:LinkButton ID="lnkFakeBilledItem" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupBilledItem" runat="server" DropShadow="false"
                PopupControlID="pnlAddEditBilledItem" TargetControlID="lnkFakeBilledItem"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnSaveBilledItem" />
        </Triggers>
    </asp:UpdatePanel>

    <hr />

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>


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

