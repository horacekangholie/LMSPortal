<%@ Page Title="LMSPortal - Activated AI Licence" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Activated_AI_Licence.aspx.vb" 
    Inherits="Reports_Activated_AI_Licence" %>

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
    <div class="form-group form-inline" style="float: right; display:block">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control pull-left" Width="400" placeholder="Type your text here..."></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info pull-left" Text="Search" />
    </div>

    <!-- Content -->
    <div ID="AILicenceReport" runat="server" style="margin-top:40px" role="tabpanel">
        <ul class='nav nav-tabs' id="myTab">
            <li id="tab_link1"><a data-toggle="tab" id="link1" href="#ActivatedAILicence" class="nav-tabs active">Activated AI Licence</a></li>
            <li id="tab_link2"><a data-toggle="tab" id="link2" href="#AILicenceWithoutDeviceID" class="nav-tabs">AI Licence without Serial No</a></li>
            <li id="tab_link3"><a data-toggle="tab" id="link3" href="#AILicenceRenew" class="nav-tabs">AI Licence with renew status</a></li>
            <li id="tab_link4"><a data-toggle="tab" id="link4" href="#AILicenceExpired" class="nav-tabs">Expired AI Licence</a></li>
        </ul>
        <div class="tab-content">
            <div id="ActivatedAILicence" class="tab-pane active">
                <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                <asp:Label ID="FooterTotalCount1" runat="server"></asp:Label>
            </div>
            <div id="AILicenceWithoutDeviceID" class="tab-pane">
                <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                <asp:Label ID="FooterTotalCount2" runat="server"></asp:Label>
            </div>
            <div id="AILicenceRenew" class="tab-pane">
                <asp:GridView ID="GridView3" runat="server"></asp:GridView>
                <asp:Label ID="FooterTotalCount3" runat="server"></asp:Label>
            </div>
            <div id="AILicenceExpired" class="tab-pane">
                <asp:GridView ID="GridView4" runat="server"></asp:GridView>
                <asp:Label ID="FooterTotalCount4" runat="server"></asp:Label>
            </div>
            
            <!--Add a refresh linkbutton to sync latest license status-->
            <div style="margin-top: 20px">
                <asp:LinkButton ID="AILicenceRefresh" runat="server" CssClass="btn btn-default"></asp:LinkButton>
            </div>
        </div>
    </div>

    <script>
        // Retrieve the last selected tab from localStorage
        var lastTab = localStorage.getItem("AILicencelastTab");

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
                    localStorage.setItem("AILicencelastTab", selectedTabId);
                });
            });
        });
    </script>

</asp:Content>

