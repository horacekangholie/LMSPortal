<%@ Page Title="LMSPortal - Subscription Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Subscription_Details.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Views_Subscription_Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- Style sheet and Script -->
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" />
    <link rel="stylesheet" href="/Content/ContentPage.css" />
    <link href="https://fonts.cdnfonts.com/css/source-sans-pro" rel="stylesheet">

    <!-- javascript to copy cell value to clipboard -->
    <script>
        function copyToClipboard(text) {
            var textarea = document.createElement("textarea");
            textarea.value = text;
            document.body.appendChild(textarea);
            textarea.select();
            document.execCommand("copy");
            document.body.removeChild(textarea);
        }
    </script>

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

    <style type="text/css">
        .sorted-column {
            background-color: #ffffe6;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Form Page Content -->
    <div class="card border-light mb-3">
        <div class="card-header"><h4 class="card-title">Customer Details</h4></div>
        <div class="card-body">
            <asp:FormView ID="FormView1" runat="server" Width="100%">
                <ItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Name</label>
                                <asp:TextBox ID="TB_Name" Text='<%# Eval("Name") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Code</label>
                                <asp:TextBox ID="TB_Code" Text='<%# Eval("Distributor_Code") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" rowspan="2">
                                <label style="display: block">Address</label>
                                <asp:TextBox ID="TB_Address" Text='<%# Eval("Address") %>' runat="server" TextMode="MultiLine" Enabled="false" BackColor="#eeeeee" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Type</label>
                                <asp:Label ID="LB_Type" Text='<%# Eval("Type") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Type" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Type_SqlDataSource"
                                    DataTextField="Type"
                                    DataValueField="Type"
                                    OnDataBound="DDL_Type_DataBound"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="DDL_Type_SelectedIndexChanged">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Type_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Type FROM DB_Lookup WHERE Lookup_Name = 'Customer Type'"></asp:SqlDataSource>
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Group</label>
                                <asp:Label ID="LB_Group_ID" Text='<%# Eval("Group_ID") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Group_ID" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Group_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Group_ID"
                                    OnDataBound="DDL_Group_ID_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Group_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Group_ID, Name FROM Master_Customer_Group"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: bottom">
                            <td style="width: 50%; padding: 0px 10px 10px 0px" colspan="2">
                                <label id="lbl_By_Distributor" runat="server">By Distributor</label>
                                <asp:Label ID="LB_By_Distributor" Text='<%# Eval("By_Distributor") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server"
                                    DataSourceID="By_Distributor_SqlDataSource" CssClass="form-control" Enabled="false"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer', 'DIGI') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 0px 10px 10px 0px">
                                <label>Contact Person</label>
                                <asp:TextBox ID="TB_Contact_Person" Text='<%# Eval("Contact_Person") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 20%; padding: 0px 10px 10px 0px">
                                <label>Phone</label>
                                <asp:TextBox ID="TB_Phone" Text='<%# Eval("Phone") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 30%; padding: 0px 10px 10px 0px">
                                <label>Email</label>
                                <asp:TextBox ID="TB_Email" Text='<%# Eval("Email") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
        </div>
    </div>


    <!-- DMC Account Content -->
    <div id="DMCAccountContent" runat="server" style="margin-top: 40px">
        <ul class='nav nav-tabs' id="myTab" role="tablist">
            <li id="tab_link1" class="nav-item" role="presentation"><a id="link1" class="nav-link" data-toggle="tab" href="#dmcheadquarter" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">DMC Headquarter</a></li>
            <li id="tab_link2" class="nav-item" role="presentation"><a id="link2" class="nav-link active" data-toggle="tab" href="#dmcstore" aria-selected="true" role="tab"  style="font-size: 15px; width: 180px; text-align: center">DMC Store</a></li>
            <li id="tab_link3" class="nav-item" role="presentation"><a id="link3" class="nav-link" data-toggle="tab" href="#dmcuser" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">DMC User</a></li>
            <li id="tab_link4" class="nav-item" role="presentation"><a id="link4" class="nav-link" data-toggle="tab" href="#dmcsubscription" aria-selected="false" tabindex="-1" role="tab" style="font-size: 15px; width: 180px; text-align: center">DMC Subscription</a></li>
        </ul>
        <div class="tab-content" id="myTabContent">
            <div class="tab-pane" id="dmcheadquarter" role="tabpanel">
                <!-- DMC Headquarter Details -->
                <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                    <asp:GridView ID="GridView1" runat="server"></asp:GridView>
                </div>
            </div>
            <div class="tab-pane active" id="dmcstore" role="tabpanel">
                <!-- DMC Store Details -->
                <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                    <asp:GridView ID="GridView2" runat="server"></asp:GridView>
                    <div class="alert alert-dismissible alert-secondary" style="margin-top: 15px; margin-right: 10px">
                        <h5 class="alert-heading">Store status:</h5>
                        <h6>
                            <p><b>Active</b> - Current active store.</p>
                            <p><b>Suspended</b> - Expired store, but still within 30-day period, charges applied for account re-instatement.</p>
                            <p><b>Closed</b> - Account permanently closed, no reinstatement allowed.</p>
                        </h6>
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="dmcuser" role="tabpanel">
                <!-- DMC User Details -->
                <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                    <asp:GridView ID="GridView3" runat="server"></asp:GridView>
                </div>
            </div>
            <div class="tab-pane" id="dmcsubscription" role="tabpanel">
                <!-- DMC Subscription Details -->
                <div style="padding: 20px 0px 40px 10px; border: solid 1px; border-color: #fff #dee2e6 #dee2e6">
                    <!-- Dropdownlist for Store -->
                    <div class="form-group form-inline" style="float: left; margin-left: 0px; margin-top: 0px">
                        <asp:Label ID="LB_Store_Contract_List" runat="server" Text="Filter By Store" CssClass="guided-text-dropdownlist"></asp:Label>
                        <asp:DropDownList ID="DDL_Store_Contract_List" runat="server" CssClass="form-control" Width="400px" AutoPostBack="true"
                            AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </div>
                    <asp:GridView ID="GridView4" runat="server"></asp:GridView>
                    <div class="alert alert-dismissible alert-secondary" style="margin-top: 15px; margin-right: 10px">
                        <h5 class="alert-heading">Note:</h5>
                        <h6>
                            <p>List of DMC Subscription record for the past 3 years</p>
                        </h6>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <!-- Account Notes -->
    <div id="DMCAccountNotes" runat="server" style="margin-top: 40px">
        <div class="card border-0 mb-3">
            <div style="padding-left: 15px; padding-right: 12px">
                <h4 class="card-title">Notes</h4>
            </div>
            <div class="card-body">
                <asp:GridView ID="GridView5" runat="server"></asp:GridView>
            </div>
        </div>
    </div>


    <hr style="margin-top: 100px"/>

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default"></asp:LinkButton>


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


    <!-- Keep where last tab selected -->
    <script>
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
                localStorage.setItem('activeTab', $(e.target).attr('href'));
                localStorage.setItem('activeLink', $(e.target).attr('id'));
            });

            var activeTab = localStorage.getItem('activeTab');
            //var activeLink = localStorage.getItem('activeLink');

            if (activeTab) {
                $('#myTab a[href="' + activeTab + '"]').tab('show');  // Show current tab
                //var element = document.getElementById("tab_" + activeLink);
                //element.classList.add("active");
            }
        });
    </script>

</asp:Content>

