<%@ Page Title="LMSPortal - DMC Account Revenue" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="DMC_Account_Revenue.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Views_DMC_Account_Revenue" %>

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
        }

        /* Add this style to make the bottom border of the active tab invisible */
        .nav-link.active {
            border-bottom: none;
        }

        .dropdownlist-guided-text{
            margin-bottom: 10px;
            margin-left: 5px;
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
        </ul>
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane active" id="RevenueMonthly" role="tabpanel">
                <div class="tab-box">
                    <div class="alert alert-success">
                        <div class="dropdownlist-guided-text"><b>Note: </b>View report of past 15 months.</div>
                        <asp:DropDownList ID="DDL_ReportMonth" runat="server" CssClass="form-control w-25" AutoPostBack="true"></asp:DropDownList>
                    </div>
                    <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                </div>
            </div>
            <div class="tab-pane" id="RevenueOverview" role="tabpanel">
                <div class="tab-box">
                    <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                </div>
            </div>
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

