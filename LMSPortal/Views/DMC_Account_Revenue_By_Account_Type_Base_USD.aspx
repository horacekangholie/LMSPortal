<%@ Page Title="LMSPortal - DMC Revenue By Account Type (USD)" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="DMC_Account_Revenue_By_Account_Type_Base_USD.aspx.vb" 
    MaintainScrollPositionOnPostback="true"
    EnableEventValidation="false"
    Inherits="Views_DMC_Account_Revenue_By_Account_Type_Base_USD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" />
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <style>
        .selectedTab {
            margin-top:20px;
        }

        .tab-box {
            padding: 10px 10px 40px 10px !important;
            border: solid 1px !important;
            border-color: #fff #dee2e6 #dee2e6 !important;
            border-top: none;
        }

        .tab-box-multiple-table {
            padding: 10px 10px 40px 10px !important;
            border: solid 1px !important;
            border-color: #fff #dee2e6 #dee2e6 !important;
            border-top: none;
            display: flex;
            gap: 10px;       /* 10px space between each flex item */
        }

        .tab-box-multiple-table .gridview-wrapper {
            flex: 1;         /* each grid takes equal width */
            padding: 10px;   /* 10px padding around each grid */
        }

        .dropdownlist-guided-text{
            margin-bottom: 10px;
            margin-left: 5px;
        }

        .dropdownlist-gap{
            margin-bottom: 5px;
        }

        .header-break {
            white-space: normal;
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

    <!-- Content -->
    <div ID="RevenueReport" runat="server" class="selectedTab">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" runat="server" visible="true" class="nav-item" role="presentation"><a id="link1" class="nav-link active" data-toggle="tab" href="#RevenueMonthly" aria-selected="true" role="tab"  style="font-size: 15px; width: 150px; text-align: center">Monthly Details</a></li>
            <li id="tab_link2" runat="server" visible="true" class="nav-item" role="presentation"><a id="link2" class="nav-link" data-toggle="tab" href="#RevenueOverview" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 150px; text-align: center">Yearly Overview</a></li>
            <%--<li id="tab_link3" runat="server" visible="true" class="nav-item" role="presentation"><a id="link3" class="nav-link" data-toggle="tab" href="#Statistics" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 150px; text-align: center">Statistics</a></li>--%>
        </ul>
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane active" id="RevenueMonthly" role="tabpanel">
                <div class="tab-box">
                    <div class="alert alert-success">
                        <div class="dropdownlist-guided-text"><b>Note: </b>View report of past 15 months.</div>
                        <asp:DropDownList ID="DDL_ReportMonth" runat="server" CssClass="form-control w-25" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <div class="row">
                        <div class="col-md-8">
                            <asp:GridView ID="GridView1" runat="server" AllowSorting="false">
                        <Columns>
                            <asp:BoundField DataField="Device_Type" HeaderText="Account Type" SortExpression="Device_Type" ItemStyle-Width="100" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" ItemStyle-Width="50" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Headquarter_ID" HeaderText="Headquarter ID" SortExpression="Headquarter_ID" ItemStyle-Width="50" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Headquarter_Name" HeaderText="Headquarter Name" SortExpression="Headquarter_Name" ItemStyle-Width="500" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Owned_Store" HeaderText="Owned Store" SortExpression="Owned_Store" DataFormatString="{0:#}" ItemStyle-Width="50" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Currency" HeaderText="Currency" SortExpression="Currency" ItemStyle-Width="0" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Total_Amount_Per_Month" HeaderText="Total Amount (Monthly)" HeaderStyle-CssClass="header-break" SortExpression="Total_Amount_Per_Month" ItemStyle-Width="0" ItemStyle-Wrap="false" />
                            <asp:BoundField DataField="Average" HeaderText="Average" SortExpression="Average" DataFormatString="{0:#,##0.00}" ItemStyle-Width="10" ItemStyle-Wrap="false" />
                        </Columns>
                    </asp:GridView>
                        </div>
                        <div class="col-md-4">
                            <div class="mb-4">
                                <asp:GridView ID="GridView3" runat="server" CssClass="table table-striped"></asp:GridView>
                            </div>
                            <div class="mb-4">
                                <asp:GridView ID="GridView4" runat="server" CssClass="table table-striped"></asp:GridView>
                            </div>
                            <div>
                                <asp:GridView ID="GridView5" runat="server" CssClass="table table-striped"></asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="RevenueOverview" role="tabpanel">
                <div class="tab-box">
                    <div class="alert alert-info">
                        <div class="dropdownlist-guided-text"><b>Note: </b>View report by country and account type</div>
                        <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control w-25 dropdownlist-gap" AutoPostBack="true" AppendDataBoundItems="true">
                            <asp:ListItem Value="ALL" Text="ALL"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:DropDownList ID="DDL_Account_Type" runat="server" CssClass="form-control w-25" AutoPostBack="true" AppendDataBoundItems="true">
                            <asp:ListItem Value="ALLOFALL" Text="ALL (ALL, POS and RETAIL)"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div style="margin-top: 40px; margin-bottom: 20px">
                        <h2><asp:Label ID="LB_Country" runat="server"></asp:Label></h2>
                    </div>
                    <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                </div>
            </div>
            <%--<div class="tab-pane" id="Statistics" role="tabpanel">
                <div class="tab-box-multiple-table">
                    <div class="gridview-wrapper">
                        <asp:GridView ID="GridView3" runat="server" CssClass="table table-striped">
                        </asp:GridView>
                    </div>
                    <div class="gridview-wrapper">
                        <asp:GridView ID="GridView4" runat="server" CssClass="table table-striped">
                        </asp:GridView>
                    </div>
                    <div class="gridview-wrapper">
                        <asp:GridView ID="GridView5" runat="server" CssClass="table table-striped">
                        </asp:GridView>
                    </div>
                </div>
            </div>--%>

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


    <!-- Keep where last tab selected -->
    <script>
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
                localStorage.setItem('activeTab', $(e.target).attr('href'));
                localStorage.setItem('activeLink', $(e.target).attr('id'));
            });

            var activeTab = localStorage.getItem('activeTab');

            if (activeTab) {
                $('#myTab a[href="' + activeTab + '"]').tab('show');  // Show current tab
            }
        });
    </script>

    <script>
        // Get all the <a>tags with class "nav-link"
        var links = document.getElementsByClassName('nav-link');

        // Add event listeners to each <a>tag
        for (var i = 0; i < links.length; i++) {
            links[i].addEventListener('click', function () {
                // Reset the font weight of all the links
                for (var j = 0; j < links.length; j++) {
                    //links[j].style.fontWeight = 'normal';
                    //links[j].style.color = '#337ab7';
                    links[j].style.backgroundColor = 'transparent';
                }

                // Set the font weight of the clicked link to bold
                //this.style.fontWeight = 'bold';
                //this.style.color = '#337ab7';
                this.style.backgroundColor = 'transparent';
            });

            // Add the bold effect for the default selected link
            if (links[i].classList.contains('active')) {
                //links[i].style.fontWeight = 'bold';
                //links[i].style.color = '#337ab7';
                links[i].style.backgroundColor = 'transparent';
            }
        }
    </script>

</asp:Content>

