<%@ Page Title="LMSPortal - Dashboard" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Default.aspx.vb" 
    Inherits="_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">

  <!-- Style sheet and Script -->
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>  <%--This is required for tab effect--%>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
  <link rel="stylesheet" href="Content/ContentPage.css" />

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
            BlockUI("<%=pnlAddReminder.ClientID %>");
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

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    <!-- Page Title -->
    <h2 class="mb-4">Dashboard</h2>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Reminder Section -->
    <div id="ReminderDashboard" runat="server">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <hr style='margin-top: 25px; margin-bottom: 25px' />
                <asp:GridView ID="GridViewReminder" runat="server">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:CheckBox ID="chk_Is_Done" runat="server" Checked='<%# Convert.ToBoolean(Eval("Is_Done")) %>' CssClass="form-check" AutoPostBack="true" OnCheckedChanged="chk_Is_Done_CheckedChanged" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Label ID="LB_Reminder" runat="server" Text='<%# Eval("Reminder") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Button ID="btnAddReminder" Text="Add Reminder" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" />

                <!-- Modal body -->
                <asp:Panel ID="pnlAddReminder" runat="server" CssClass="modalPopup">
                    <h3 style="padding: 10px 0px 20px 20px">
                        <asp:Label ID="ModalHeaderReminder" runat="server"></asp:Label></h3>
                    <table border="0" class="modalPopupTable" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 800px; padding: 0px 10px 20px 0px">
                                <label>Reminder</label>
                                <asp:TextBox ID="TB_Reminder" runat="server" CssClass="form-control" placeholder="Type reminder here.."></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Reminder" runat="server" ValidationGroup="Reminder"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Reminder">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 800px; padding: 10px 10px 10px 0px" colspan="4">
                                <asp:Button ID="btnSaveReminder" runat="server" CssClass="btn btn-sm btn-default" ValidationGroup="Reminder" />
                                <asp:Button ID="btnCancelReminder" runat="server" CssClass="btn btn-sm btn-default" CausesValidation="false" OnClick="btnCancelReminder_Click" OnClientClick="return Hidepopup()" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                <asp:LinkButton ID="lnkFakeReminder" runat="server"></asp:LinkButton>
                <cc1:ModalPopupExtender ID="popupReminder" runat="server" DropShadow="false"
                    PopupControlID="pnlAddReminder" TargetControlID="lnkFakeReminder"
                    BackgroundCssClass="modalBackground">
                </cc1:ModalPopupExtender>
            </ContentTemplate>

            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSaveReminder" EventName="Click" />
                <asp:PostBackTrigger ControlID="GridViewReminder" />
            </Triggers>

        </asp:UpdatePanel>
    </div>

    <!-- For Administrator -->
    <div id="AdminDashboard" runat="server">
        <asp:PlaceHolder ID="Dashboard" runat="server"></asp:PlaceHolder>
    </div>

    <!-- For Common -->
    <div id="CommonDashboard" runat="server">
        <asp:PlaceHolder ID="Dashboard1" runat="server"></asp:PlaceHolder>
    </div>

    <div ID="MaintenanceDashboard" runat="server">
        <asp:PlaceHolder ID="Dashboard2" runat="server"></asp:PlaceHolder>
    </div>


    <!-- javascript for Nested GridView effect -->
    <script src="Scripts/jquery-1.3.2.min.js"></script> <!-- need to use jquery-1.3.2.min.js -->
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "images/minus-square-solid-24.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "images/plus-square-solid-24.png");
            $(this).closest("tr").next().remove();
        });
    </script>

</asp:Content>

