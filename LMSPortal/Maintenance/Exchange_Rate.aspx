<%@ Page Title="LMSPortal - Exchange Rate" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Exchange_Rate.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Maintenance_Exchange_Rate" %>

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
            BlockUI("<%=pnlUpdateExchangeRate.ClientID %>");
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

    <!-- Use Panel and set default button to trigger validation -->
    <asp:Panel ID="pnlSearchBox" runat="server" DefaultButton="BT_Search">
        <!-- Searchbox -->
        <div class="form-group form-inline" style="float: right">
            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" CausesValidation="false" />
        </div>
    </asp:Panel>

    <!-- Upload Section -->
    <div style="width: 100%; float: right">
        <div style="width: 30%; float: left">
            <div class="card border-light mb-3" style="width: 95%">
                <div class="card-header">
                    <h4 class="card-title">Upload Exchange Rate</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                                <asp:FileUpload ID="FileUpload1" runat="server" />
                                <asp:RequiredFieldValidator ID="RequiredField_FileUpload1" runat="server" ValidationGroup="FileUpload"
                                    ErrorMessage="Please select a file to upload" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="FileUpload1">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_FileUpload1" runat="server" ValidationGroup="FileUpload"
                                    ErrorMessage="Only (.csv) file is allowed" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="FileUpload1"
                                    ValidationExpression="^([0-9a-zA-Z_\-~ :\\])+(.csv)$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 20%; padding: 20px 5px 20px 0px">
                                <asp:Button ID="BN_Upload" runat="server" Text="Upload" CssClass="form-control btn btn-default w-100" CausesValidation="true" ValidationGroup="FileUpload" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <!-- Script Manager -->
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <div style="margin-left: 30%">
            <!-- Modal Section -->
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <!-- GridView -->
                    <!-- GridView has to be inside panel to work -->
                    <asp:GridView ID="GridView1" runat="server">
                        <Columns>
                            <asp:BoundField DataField="Date" HeaderText="Date" SortExpression="Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="30%" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" ItemStyle-Width="30%" ItemStyle-Wrap="true" />
                            <asp:BoundField DataField="Rate" HeaderText="Rate" SortExpression="Rate" ItemStyle-Width="40%" ItemStyle-Wrap="false" />
                            <asp:TemplateField ItemStyle-Width="0%" ItemStyle-Wrap="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="EditLinkButton" runat="server"
                                        CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Date") & "|" & Eval("Currency") & "|" & Eval("Rate") %>'
                                        Text="<i class='bi bi-pencil-fill'></i>"
                                        CssClass="btn btn-xs btn-info"
                                        CausesValidation="false"
                                        OnClick="Edit_ExchangeRate_Click">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="DeleteLinkButton" runat="server"
                                        CommandArgument='<%# (CType(Container, GridViewRow)).RowIndex & "|" & Eval("ID") & "|" & Eval("Date") & "|" & Eval("Currency") & "|" & Eval("Rate") %>'
                                        Text="<i class='bi bi-trash'></i>"
                                        CssClass="btn btn-xs btn-danger"
                                        CausesValidation="false"
                                        OnClick="Delete_ExchangeRate_Click"
                                        OnClientClick="return confirm('Are you sure to delete record?')">
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                    <asp:Panel ID="pnlUpdateExchangeRate" runat="server" CssClass="modalPopup">
                        <h3 style="padding: 10px 0px 20px 20px"><asp:Label ID="ModalHeaderExchangeRate" runat="server"></asp:Label></h3>
                        <table border="0" class="modalPopupTable" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 300px; padding: 0px 10px 20px 0px">
                                    <label>Date</label>
                                    <asp:TextBox ID="TB_Date" runat="server" CssClass="form-control disabled" Enabled="false"></asp:TextBox>
                                </td>
                                <td style="width: 200px; padding: 0px 10px 20px 0px">
                                    <label>Currency</label>
                                    <asp:DropDownList ID="DDL_Currency" runat="server" CssClass="form-control disabled" Enabled="false"></asp:DropDownList>
                                </td>
                                <td style="width: 300px; padding: 0px 10px 20px 0px">
                                    <label>Rate</label>
                                    <asp:TextBox ID="TB_Rate" runat="server" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_Rate" runat="server" ValidationGroup="ExchangeRate"
                                        ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Rate">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_Rate" runat="server" ValidationGroup="ExchangeRate"
                                        ErrorMessage="Exchange rate must be numeric value" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_Rate"
                                        ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                    </asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr style="vertical-align: bottom">
                                <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="3">
                                    <asp:Button ID="btnUpdateExchangeRate" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="ExchangeRate" />
                                    <asp:Button ID="btnCancelExchangeRate" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClientClick="return Hidepopup()" />
                                </td>
                            </tr>
                        </table>
                        <!-- Hidden fields -->
                        <asp:TextBox ID="TB_Selected_ID" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_Selected_Date" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_Selected_Currency" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                        <asp:TextBox ID="TB_Selected_Rate" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                    </asp:Panel>
                    <asp:LinkButton ID="lnkFakeExchangeRate" runat="server"></asp:LinkButton>
                    <cc1:ModalPopupExtender ID="popupExchangeRate" runat="server" DropShadow="false"
                        PopupControlID="pnlUpdateExchangeRate" TargetControlID="lnkFakeExchangeRate"
                        BackgroundCssClass="modalBackground">
                    </cc1:ModalPopupExtender>

                    <!-- Display total number of records from the GridView -->
                    <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
                </ContentTemplate>

                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView1" />
                    <asp:AsyncPostBackTrigger ControlID="btnUpdateExchangeRate" />
                </Triggers>

            </asp:UpdatePanel>
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

</asp:Content>

