<%@ Page Title="LMSPortal - CZL Licensed Devices" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Licensed_Device.aspx.vb" 
    Inherits="Form_CZL_Licensed_Device" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <style>
        dl dt {
            display: inline-block;
            width: 30px;
            height: 30px;
            vertical-align: middle;
            border:1px solid #f0a8a8;
            margin-left:10px;
        }

        dl dd {
            display: inline-block;
            margin: 0px 10px;
            padding-bottom: 0;
            vertical-align: middle;
            font-family:'Franklin Gothic Medium', 'Arial Narrow', 'Arial', 'sans-serif';
        }

        dl dt.red {
            background: #f8d7da;
        }

        tr:hover td { background: #f5f5f5 !important; }   /*override mouseover color*/


        .CheckboxSelection {
            float: left;
            display: inline-block;
            padding-top: 1em;
        }

        .CheckboxSelection > input {
            width: 16px;
            height: 16px;
            margin-left: 5px;
        }

        .CheckboxSelection > span {
            display:inline-block;
            font-size: 16px;
            vertical-align: central;
            align-self:center;
            padding-left: 3px;
        }

    </style>

    <style>
        .sort-arrow {
            margin-left: 5px; /* Add some spacing between text and arrow */
        }

        .asc::after {
            content: '\25B2'; /* Unicode character for upward arrow */
        }

        .desc::after {
            content: '\25BC'; /* Unicode character for downward arrow */
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Checkbox for display account with no device -->
    <div class="CheckboxSelection">
        <asp:CheckBox ID="chk_empty_account" runat="server" AutoPostBack="true" OnCheckedChanged="chk_empty_account_CheckedChanged" />
        <asp:Label ID="lb_empty_account" runat="server" Text="Account with no device"></asp:Label>
    </div>

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float: right">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="600" placeholder="Act. ID, Act. Name, By Distributor, Device ID, Scale SN, MAC, Gen Vers, Country"></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>


    <!-- GridView -->
    <asp:GridView ID="GridView1" runat="server"></asp:GridView>

    <!-- Display total number of records from the GridView -->
    <div style="margin:0px 0px 20px 10px">
        <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
    </div>

    <!-- Legend -->
    <div style="display:inline-block">
        <dl style="display: inline-block; float: left">
            <dt class="red"></dt>
            <dd class="text-info">Account has no registered licensed device</dd>
        </dl>
    </div>

    <hr />

    <!-- Display total no of registered devices -->
    <div style="display: inline-block;float: left; width: 25%">
        <ul class="list-group">
            <li class="list-group-item d-flex justify-content-between align-items-center">Number of CZL account
            <span class="badge bg-secondary rounded-pill">
                <asp:Label ID="LB_No_Of_Registered_Account" runat="server" Visible="true"></asp:Label></span>
            </li>
            <li class="list-group-item d-flex justify-content-between align-items-center">Number of registered devices
            <span class="badge bg-secondary rounded-pill">
                <asp:Label ID="LB_No_Of_Registered_Devices" runat="server" Visible="true"></asp:Label></span>
            </li>
        </ul>
    </div>



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

