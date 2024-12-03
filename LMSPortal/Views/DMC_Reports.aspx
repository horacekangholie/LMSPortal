<%@ Page Title="LMSPortal - DMC Related Reports" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="DMC_Reports.aspx.vb" 
    Inherits="Views_DMC_Reports" %>

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

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float: right">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..." TextMode="Search"></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>


    <!-- GridView -->
    <asp:GridView ID="GridView1" runat="server"></asp:GridView>

    <!-- Display total number of records from the GridView -->
    <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>


    <!-- javascript for GridView Row highlight effect -->
    <script type="text/javascript">
        var OrigRowColor;
        function SetMouseOver(element) {
            OrigRowColor = element.style.backgroundColor;
            element.style.backgroundColor = '#f5f5f5';
            element.style.cursor = 'pointer';
        }
        function SetMouseOut(element) {
            element.style.backgroundColor = OrigRowColor;
        }
    </script>

</asp:Content>

