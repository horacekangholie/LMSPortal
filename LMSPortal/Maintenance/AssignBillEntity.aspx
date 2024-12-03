<%@ Page Title="LMSPortal - Re-assign Bill Entity" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="AssignBillEntity.aspx.vb" 
    Inherits="Maintenance_AssignBillEntity" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

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

    <div style="width: 100%; float: right">
        <div style="width: 30%; float: left">
            <div class="card border-light mb-3" style="width: 95%">
                <div class="card-header">
                    <h4 class="card-title">Reassign Bill To Entity</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>Unbilled Subscription</label>
                                <asp:DropDownList ID="DDL_Subscription_ID" runat="server"
                                    DataSourceID="SqlDataSource_DDL_Subscription_ID"
                                    DataTextField="Customer"
                                    DataValueField="Subscription ID"
                                    CssClass="form-control">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource_DDL_Subscription_ID" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT [Subscription ID] + ' - ' + [Customer] AS [Customer], [Subscription ID] FROM I_DMC_Subscription WHERE [Invoice No] = '' GROUP BY [Subscription ID] + ' - ' + [Customer], [Subscription ID] ORDER BY [Subscription ID] DESC"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <label>Assign New Bill Entity</label>
                                <asp:DropDownList ID="DDL_New_Bill_Entity" runat="server"
                                    DataSourceID="SqlDataSource_DDL_New_Bill_Entity"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    CssClass="form-control">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="SqlDataSource_DDL_New_Bill_Entity" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Customer_ID, Name FROM Master_Customer WHERE Is_Active = 1 ORDER BY Name"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 5px 0px 0px">
                                <asp:Button ID="BN_Assign" runat="server" Text="Assign" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="AssignBillEntity" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div style="margin-left: 30%">
            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
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

