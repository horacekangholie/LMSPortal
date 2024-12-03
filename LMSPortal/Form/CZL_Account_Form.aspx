<%@ Page Title="LMSPortal - Account Info" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Account_Form.aspx.vb" 
    MaintainScrollPositionOnPostback ="true"
    Inherits="Form_CZL_Account_Form" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
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
        <div class="card-header">
            <h4 class="card-title">Account Details</h4>
        </div>
        <div class="card-body">
            <asp:FormView ID="FormView1" runat="server" Width="100%">
                <EditItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Account ID</label>
                                <asp:TextBox ID="TB_CZL_Client_ID" Text='<%# Eval("Client_ID") %>' runat="server" CssClass="form-control" Enabled="false" OnTextChanged="TB_CZL_Client_ID_TextChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_CZL_Client_ID" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback" 
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_CZL_Client_ID">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_E_CZL_Client_ID" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Client ID must be numeric" CssClass="invalid-feedback" 
                                    Display="Dynamic" 
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_CZL_Client_ID"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                                <asp:Label ID="czlclientidexisterrormsg" runat="server" Text="CZL Client ID exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                            </td>
                            <td style="width: 45%; padding: 10px 10px 20px 0px">
                                <label>Account Name</label>
                                <asp:TextBox ID="TB_User_Group" Text='<%# Eval("User_Group") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_User_Group" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback" 
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_User_Group">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Model</label>
                                <asp:TextBox ID="TB_Account_Model" Text='<%# Eval("Account_Model") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_E_Account_Model" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Account_Model">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_E_Account_Model" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Model must be numeric" CssClass="invalid-feedback" 
                                    Display="Dynamic" 
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Account_Model"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Created Date</label>
                                <asp:TextBox ID="TB_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created_Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 40%; padding: 10px 10px 20px 0px">
                                <label>By Distributor / Owner</label>
                                <asp:Label ID="LB_By_Distributor" Text='<%# Eval("By_Distributor") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server" CssClass="form-control"
                                    DataSourceID="By_Distributor_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="-1" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_E_DDL_By_Distributor" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please select a Distributor / Owner" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_By_Distributor" Type="String">
                                </asp:CompareValidator>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="-1" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_E_DDL_Country" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please select a country" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_Country" Type="String">
                                </asp:CompareValidator>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                            <td style="width: 20%; padding: 10px 10px 20px 0px">
                                <label>Gen Version</label>
                                <asp:Label ID="LB_Gen_Version" Text='<%# Eval("Gen_Version") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Gen_Version" runat="server" CssClass="form-control"
                                    DataSourceID="Gen_Version_SqlDataSource"
                                    DataTextField="Gen_Version"
                                    DataValueField="Gen_Version"
                                    OnDataBound="DDL_Gen_Version_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Gen_Version_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Gen_Version FROM DB_Lookup WHERE Lookup_Name = 'CZL Gen Version' ORDER BY Value_2 DESC"></asp:SqlDataSource>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Effective Date</label>
                                <asp:TextBox ID="TB_Effective_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Effective_Date")) %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 30%; padding: 10px 10px 20px 0px">
                                <label>Account ID</label>
                                <asp:TextBox ID="TB_CZL_Client_ID" Text='<%# Eval("Client_ID") %>' runat="server" CssClass="form-control" Enabled="true" OnTextChanged="TB_CZL_Client_ID_TextChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_CZL_Client_ID" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_CZL_Client_ID">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_I_CZL_Client_ID" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="CZL Client ID must be numeric" CssClass="invalid-feedback" 
                                    Display="Dynamic" 
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_CZL_Client_ID"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                                <asp:Label ID="czlclientidexisterrormsg" runat="server" Text="CZL Client ID exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                            </td>
                            <td style="width: 55%; padding: 10px 10px 20px 0px">
                                <label>Account Name</label>
                                <asp:TextBox ID="TB_User_Group" Text='<%# Eval("User_Group") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_User_Group" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_User_Group">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Model</label>
                                <asp:TextBox ID="TB_Account_Model" Text='<%# Eval("Account_Model") %>' runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Account_Model" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Account_Model">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_I_Account_Model" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Model must be numeric" CssClass="invalid-feedback" 
                                    Display="Dynamic" 
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Account_Model"
                                    ValidationExpression="^(0|[1-9]\d*)$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 40%; padding: 10px 10px 20px 0px">
                                <label>By Distributor / Owner</label>
                                <asp:Label ID="LB_By_Distributor" Text='<%# Eval("By_Distributor") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server" CssClass="form-control"
                                    DataSourceID="By_Distributor_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="-1" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_I_DDL_By_Distributor" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please select a Distributor / Owner" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_By_Distributor" Type="String">
                                </asp:CompareValidator>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="-1" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_I_DDL_Country" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please select a country" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_Country" Type="String">
                                </asp:CompareValidator>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                            <td style="width: 20%; padding: 10px 10px 20px 0px">
                                <label>Gen Version</label>
                                <asp:Label ID="LB_Gen_Version" Text='<%# Eval("Gen_Version") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Gen_Version" runat="server" CssClass="form-control"
                                    DataSourceID="Gen_Version_SqlDataSource"
                                    DataTextField="Gen_Version"
                                    DataValueField="Gen_Version"
                                    OnDataBound="DDL_Gen_Version_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Gen_Version_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Gen_Version FROM DB_Lookup WHERE Lookup_Name = 'CZL Gen Version' ORDER BY Value_2 DESC"></asp:SqlDataSource>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Account Effective Date</label>
                                <asp:TextBox ID="TB_Effective_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Effective_Date")) %>' runat="server" CssClass="form-control" TextMode="Date" />
                                <asp:RequiredFieldValidator ID="RequiredField_I_Effective_Date" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Effective_Date">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <!-- Alert Message for unregistered device -->
                    <div class="alert alert-warning" style="float: left; width: 100%">
                        <b>Note: </b><br />If this CZL account setup is non-chargeable, please select Account setup billable? as <b>No</b><br />Otherwise fill up the PO Details
                    </div>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 20%; padding: 10px 10px 20px 0px">
                                <label>Account setup billable?</label>
                                <asp:DropDownList ID="DDL_Account_Setup_Billable" runat="server" CssClass="form-control" 
                                    AppendDataBoundItems="true" 
                                    AutoPostBack="true" 
                                    OnDataBound="DDL_Account_Setup_Billable_DataBound"
                                    OnSelectedIndexChanged="DDL_Account_Setup_Billable_SelectedIndexChanged">
                                   <%-- <asp:ListItem Text="" Value="-1"></asp:ListItem>--%>
                                    <asp:ListItem Text="No" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                                <%--<asp:CompareValidator ID="CompareValidator_I_DDL_Account_Setup_Billable" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please select to confirm if account billable" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_Account_Setup_Billable" Type="String">
                                </asp:CompareValidator>--%>
                            </td>
                            <td style="width: 23%; padding: 10px 10px 20px 0px">
                                <label id="lbl_po_no" runat="server" visible="true">PO No</label>
                                <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control" Visible="true" />
                                <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback" Enabled="true"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_PO_No">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 12%; padding: 10px 10px 20px 0px">
                                <label id="lbl_po_date" runat="server" visible="true">PO Date</label>
                                <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date" Visible="true" />
                                <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback" Enabled="true"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_PO_Date">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 10%; padding: 10px 10px 20px 0px">
                                <label id="lbl_currency" runat="server" visible="true">Currency</label>
                                <asp:DropDownList ID="DDL_Currency" runat="server" CssClass="form-control" Visible="true"
                                    DataSourceID="Currency_sqlDataSource"
                                    DataTextField="Currency"
                                    DataValueField="Currency" 
                                    OnDataBound="DDL_Currency_DataBound">
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Currency_sqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT(Value_3) AS Currency FROM DB_Lookup WHERE Lookup_Name = 'Country' AND Value_3 in ('SGD', 'USD', 'EUR')"></asp:SqlDataSource>
                            </td>
                            <td style="width: 10%; padding: 10px 10px 20px 0px">
                                <label id="lbl_fee" runat="server" visible="true">Fee</label>
                                <asp:TextBox ID="TB_Fee" runat="server" CssClass="form-control" placeholder="0.00" Visible="true"/>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Fee" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback" Enabled="true"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Fee">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_Fee" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Fee must be numeric value" CssClass="invalid-feedback" Enabled="true"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Fee"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label id="lbl_sales_representative" runat="server" visible="true">Sales Representative</label>
                                <asp:DropDownList ID="DDL_Sales_Representative" runat="server" CssClass="form-control" AppendDataBoundItems="true" Visible="true"
                                    DataSourceID="Sales_Representative_sqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Sales_Representative_ID" 
                                    OnDataBound="DDL_Sales_Representative_DataBound">
                                    <asp:ListItem Text="" Value="-1" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_I_DDL_Sales_Representative" runat="server" ValidationGroup="CZLAccount"
                                    ErrorMessage="Please sale representative" CssClass="invalid-feedback" Enabled="true"
                                    Display="Dynamic"
                                    SetFocusOnError="true"
                                    Operator="NotEqual" ValueToCompare="-1"
                                    ControlToValidate="DDL_Sales_Representative" Type="String">
                                </asp:CompareValidator>
                                <asp:SqlDataSource ID="Sales_Representative_sqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Sales_Representative_ID, Name FROM Master_Sales_Representative ORDER BY Name"></asp:SqlDataSource>
                            </td>
                        </tr>
                    </table>
                </InsertItemTemplate>
            </asp:FormView>
        </div>
    </div>


    <!-- Control Button -->
    <asp:LinkButton ID="BT_Create" Text="Create" CommandName="Create" runat="server" CssClass="btn btn-default" ValidationGroup="CZLAccount"></asp:LinkButton>
    <asp:LinkButton ID="BT_Cancel" Text="Cancel" CommandName="Cancel" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>

</asp:Content>

