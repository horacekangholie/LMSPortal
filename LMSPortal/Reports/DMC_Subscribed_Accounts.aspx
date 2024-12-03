<%@ Page Title="LMSPortal - DMC Account Report" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="DMC_Subscribed_Accounts.aspx.vb" 
    Inherits="Reports_DMC_Subscribed_Accounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">

  <!-- Style sheet and Script -->
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>  <%--This is required for tab effect--%>
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
    <div class="form-group form-inline" style="float: right; display: block">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control pull-left" Width="400" placeholder="Type your text here..."></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info pull-left" Text="Search" />
    </div>

    <!-- Content -->
    <div ID="DMCAccountReport" runat="server" style="margin-top:40px" role="tabpanel">
        <ul class='nav nav-tabs' id="myTab">
            <li id="tab_link1"><a data-toggle="tab" id="link1" href="#BilledAccounts" class="nav-tabs active">Billed Accounts</a></li>
            <li id="tab_link2"><a data-toggle="tab" id="link2" href="#DemoAccounts" class="nav-tabs">Demo Accounts</a></li>
            <li id="tab_link3"><a data-toggle="tab" id="link3" href="#TrialAccounts" class="nav-tabs">Trial Accounts</a></li>
            <li id="tab_link4"><a data-toggle="tab" id="link4" href="#TestAccounts" class="nav-tabs">Test Accounts</a></li>
        </ul>
        <div class="tab-content">
            <div id="BilledAccounts" class="tab-pane active">
                <asp:GridView ID="GridView1" runat="server"></asp:GridView>
            </div>
            <div id="DemoAccounts" class="tab-pane">
                <asp:GridView ID="GridView2" runat="server"></asp:GridView>
            </div>
            <div id="TrialAccounts" class="tab-pane">
                <asp:GridView ID="GridView3" runat="server"></asp:GridView>
            </div>
            <div id="TestAccounts" class="tab-pane">
                <asp:GridView ID="GridView4" runat="server"></asp:GridView>
            </div>
        </div>
    </div>

    <!-- Download button -->
    <div class="form-group form-inline">
        <asp:LinkButton ID="BT_Download_Excel" runat="server" CssClass="btn btn-default" Text="Download" Visible="false"></asp:LinkButton>
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


    <script>
        // Retrieve the last selected tab from localStorage
        var lastTab = localStorage.getItem("DMCAccountlastTab");

        // If a last selected tab exists, activate it
        if (lastTab) {
            var tabLink = document.getElementById(lastTab);
            if (tabLink) {
                tabLink.click();
            }
        }

        // Store the ID of the currently selected tab in localStorage
        document.addEventListener("DOMContentLoaded", function () {
            var tabLinks = document.querySelectorAll(".nav-tabs a[data-toggle='tab']");
            tabLinks.forEach(function (tabLink) {
                tabLink.addEventListener("click", function () {
                    var selectedTabId = this.getAttribute("id");
                    localStorage.setItem("DMCAccountlastTab", selectedTabId);
                });
            });
        });
    </script>

</asp:Content>

