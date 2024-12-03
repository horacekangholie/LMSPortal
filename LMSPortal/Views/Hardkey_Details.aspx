<%@ Page Title="LMSPortal - Harkey Licence Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Hardkey_Details.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    ValidateRequest="false"
    Inherits="Views_Hardkey_Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" />
    <link rel="stylesheet" href="/Content/ContentPage.css" />

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

    <hr />

    <!-- Hardkey Licence Details -->
    <div class="card border-light mb-3">
        <div class="card-header">
            <h4 class="card-title">Hardkey Licence</h4>
        </div>
        <div class="card-body">
            <asp:GridView ID="GridView1" runat="server"></asp:GridView>
        </div>
    </div>

    <hr />

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

</asp:Content>

