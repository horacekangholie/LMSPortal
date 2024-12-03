<%@ Page Title="LMSPortal - New DMC Account" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="New_DMC_Account.aspx.vb" 
    Inherits="Reports_New_DMC_Account" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <script src="extensions/copy-rows/bootstrap-table-copy-rows.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <style>
        .ftpFont{
            font-family: Menlo,Monaco,Consolas,Liberation Mono,courier New,monospace;
            font-size: 15px;
            background-color: #fbfcfc;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- DMC Subscription Gridview -->
    <div id="DMCSubscriptionSection" runat="server" style="margin-top:30px;margin-bottom:50px">
        <h4>DMC Subscription</h4>
        <asp:GridView ID="GridView1" runat="server"></asp:GridView>
    </div>

    <!-- DMC Store FTP Info -->
    <div id="StoreFTPInfoSection" runat="server" style="margin-bottom:50px">
        <h4>DMC Store FTP Info</h4>
        <asp:GridView ID="GridView2" runat="server"></asp:GridView>
    </div>

    <!-- DMC User Account -->
    <div id="DMCUserAccountSection" runat="server" style="margin-bottom:50px">
        <h4>DMC User Account</h4>
        <asp:GridView ID="GridView3" runat="server"></asp:GridView>
        <asp:Label ID="UserAccountNotes" runat="server"></asp:Label>
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

