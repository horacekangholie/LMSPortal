<%@ Page Title="LMSPortal - Suspended Stores" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Suspended_Stores.aspx.vb" 
    Inherits="Adhoc_Suspended_Stores" %>

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
            BlockUI("<%=pnlAddSuspendedStore.ClientID %>");
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

    <!-- Modal Section - Recover Invoice for Account Seupt Fee -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <!-- Create new CZL account setup charge record -->
            <asp:LinkButton ID="BT_Create" runat="server" CssClass="btn btn-default" Text="Add New"></asp:LinkButton>

            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>

            <asp:Panel ID="pnlAddSuspendedStore" runat="server" CssClass="modalPopup">
                <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderAddSuspendedStore" runat="server"></asp:Label></h3>
                <table border="0" class="modalPopupTable" style="width: 100%">
                    <tr style="vertical-align: top">
                        <td style="width: 100%; padding: 0px 10px 20px 0px" colspan="2">
                            <label>Headquarter</label>
                            <asp:DropDownList ID="DDL_Headquarter" runat="server" CssClass="form-control" Width="100%" AutoPostBack="true" AppendDataBoundItems="true">
                                <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Headquarter" runat="server" ValidationGroup="AddSuspendedStore"
                                ErrorMessage="Please select headquarter" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_Headquarter" Type="String">
                            </asp:CompareValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 100%; padding: 0px 10px 20px 0px" colspan="2">
                            <label>Store</label>
                            <asp:DropDownList ID="DDL_Store" runat="server" CssClass="form-control" Width="100%" AppendDataBoundItems="true" Enabled="false">
                                <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Store" runat="server" ValidationGroup="AddSuspendedStore" Enabled="false"
                                ErrorMessage="Please select store" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_Store" Type="String">
                            </asp:CompareValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: top">
                        <td style="width: 30%; padding: 0px 10px 20px 0px">
                            <label>Suspend from month</label>
                            <asp:DropDownList ID="DDL_Suspend_From_Month" runat="server" CssClass="form-control" Width="100%" AppendDataBoundItems="true" Enabled="false">
                                <asp:ListItem Value="-1" Text="Please select"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:CompareValidator ID="CompareValidator_Suspend_From_Month" runat="server" ValidationGroup="AddSuspendedStore" Enabled="false"
                                ErrorMessage="Please select suspension month" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                Operator="NotEqual" ValueToCompare="-1"
                                ControlToValidate="DDL_Suspend_From_Month" Type="String">
                            </asp:CompareValidator>
                        </td>
                        <td style="width: 70%; padding: 0px 10px 20px 0px">
                            <label>Reason</label>
                            <asp:TextBox ID="TB_Reason" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredField_TB_Reason" runat="server" ValidationGroup="AddSuspendedStore" Enabled="false"
                                ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                Display="Dynamic"
                                SetFocusOnError="True"
                                ControlToValidate="TB_Reason">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr style="vertical-align: bottom">
                        <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="2">
                            <asp:Button ID="btnAddSuspendedStore" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="AddSuspendedStore" />
                            <asp:Button ID="btnCancelAddSuspendedStore" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:LinkButton ID="lnkFakeAddSuspendedStore" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="popupAddSuspendedStore" runat="server" DropShadow="false"
                PopupControlID="pnlAddSuspendedStore" TargetControlID="lnkFakeAddSuspendedStore"
                BackgroundCssClass="modalBackground">
            </cc1:ModalPopupExtender>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="GridView1" />
            <asp:AsyncPostBackTrigger ControlID="btnAddSuspendedStore" />
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

