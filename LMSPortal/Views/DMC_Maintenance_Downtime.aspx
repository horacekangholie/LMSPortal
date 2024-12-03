<%@ Page Title="DMC Maintenance Downtime Report" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="DMC_Maintenance_Downtime.aspx.vb" 
    Inherits="Views_DMC_Maintenance_Downtime" %>

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
        .right-stick-textbox {
            border-top-right-radius: 0px;
            border-bottom-right-radius: 0px;
        }
        .left-stick-button {
            border-top-left-radius: 0px;
            border-bottom-left-radius: 0px;
        }

        .default-button-new-product-type{
            width: 100px;
        }

        .guided-text-dropdownlist{
            margin-left: 10px;
            margin-right: 20px;
            font-weight:bold;
            color:#337ab7;
        }

        /*Style for label convert to asp:lable*/
        .label-asp{
            display: inline-block;
            max-width: 100%;
            margin-bottom: 5px;
            font-weight: 700;
            box-sizing: border-box;
        }

        .tab-box{
            padding: 10px 0px 100px 10px;
            border: solid 1px;
            border-color: #fff #dee2e6 #dee2e6;
        }

        .selectedRow {
            background-color: #eeeeee;
        }

        .unselectedRow{
            background-color:transparent;
        }

        /* CSS for the up and down arrows */
        .sort-arrow {
            font-size: 12px; /* You can adjust this value to change the icon size */
        }

        .sort-arrow.asc::after {
            content: '\25B2'; /* Up-arrow character */
        }

        .sort-arrow.desc::after {
            content: '\25BC'; /* Down-arrow character */
        }

        /* CSS to position the arrow next to the header text */
        th {
            position: relative;
            white-space: nowrap; /* Prevent header text from wrapping */
            cursor: pointer; /* Add a pointer cursor to indicate the header is sortable */
        }

        th span.sort-arrow {
            margin-left: 5px; /* Adjust this value as needed to create space between header text and arrow */
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- DMC Maintenance Downtime Report -->
    <div id="DMCMaintenanceDowntimeReport" runat="server" style="margin-top:30px;margin-bottom:50px">
        <!-- Dropdownlist for year -->
        <div class="form-group form-inline">
            <asp:Label ID="LB_Maintenance_Year" runat="server" Text="Filter By Year" CssClass="guided-text-dropdownlist"></asp:Label>
            <asp:DropDownList ID="DDL_Maintenance_Year" runat="server" CssClass="form-control" Width="250px" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div style="margin-top:30px">
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>
        </div>
    </div>

    <hr style="margin-top: 50px"/>

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

