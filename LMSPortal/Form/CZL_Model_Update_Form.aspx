<%@ Page Title="LMSPortal - Model Update Form" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Model_Update_Form.aspx.vb" 
    Inherits="Form_CZL_Model_Update_Form" %>

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
                <ItemTemplate>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Account ID</label>
                                <asp:TextBox ID="TB_CZL_Client_ID" Text='<%# Eval("Client_ID") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 45%; padding: 10px 10px 20px 0px">
                                <label>Account Name</label>
                                <asp:TextBox ID="TB_User_Group" Text='<%# Eval("User_Group") %>' runat="server" CssClass="form-control" Enabled="false" />
                            </td>
                            <td style="width: 15%; padding: 10px 10px 20px 0px">
                                <label>Account Model</label>
                                <asp:TextBox ID="TB_Model" Text='<%# Eval("Account_Model") %>' runat="server" CssClass="form-control" Enabled="false" />
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
                                <asp:DropDownList ID="DDL_By_Distributor" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="By_Distributor_SqlDataSource"
                                    DataTextField="Name"
                                    DataValueField="Customer_ID"
                                    OnDataBound="DDL_By_Distributor_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="By_Distributor_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT DISTINCT Customer_ID, Name FROM Master_Customer WHERE Type NOT IN ('Customer') AND Name NOT IN ('') ORDER BY Name"></asp:SqlDataSource>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 20px 0px">
                                <label>Country</label>
                                <asp:Label ID="LB_Country" Text='<%# Eval("Country") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Country" runat="server" CssClass="form-control" Enabled="false"
                                    DataSourceID="Country_SqlDataSource"
                                    DataTextField="Country"
                                    DataValueField="Country"
                                    OnDataBound="DDL_Country_DataBound"
                                    AppendDataBoundItems="True">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                                <asp:SqlDataSource ID="Country_SqlDataSource" runat="server"
                                    ConnectionString="<%$ ConnectionStrings:lmsConnectionString %>"
                                    SelectCommand="SELECT Value_1 AS Country FROM DB_Lookup WHERE Lookup_Name = 'Country' ORDER BY Value_1"></asp:SqlDataSource>
                            </td>
                            <td style="width: 20%; padding: 10px 10px 20px 0px">
                                <label>Gen Version</label>
                                <asp:Label ID="LB_Gen_Version" Text='<%# Eval("Gen_Version") %>' runat="server" Visible="false" />
                                <asp:DropDownList ID="DDL_Gen_Version" runat="server" CssClass="form-control" Enabled="false"
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
                </ItemTemplate>
            </asp:FormView>
        </div>
    </div>

    <hr />

    <!-- Add New Model Section -->
    <div class="card border-light mb-3" style="width: 34.6%; float: left">
        <div class="card-header">
            <h4 class="card-title">Upgrade New Model</h4>
        </div>
        <div class="card-body">
            <table border="0" style="width: 100%">
                <tr style="vertical-align: top">
                    <td style="width: 50%; padding: 20px 5px 0px 0px">
                        <label>New Model</label>
                        <asp:TextBox ID="TB_New_Model" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_New_Model" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_New_Model">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegEx_TB_New_Model" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Model must be numeric" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_New_Model"
                            ValidationExpression="^(0|[1-9]\d*)$">
                        </asp:RegularExpressionValidator>
                        <asp:Label ID="newmodelupdateerrormsg" runat="server" Text="Model version must be higher than current model" Visible="false" CssClass="invalid-feedback"></asp:Label>
                    </td>
                    <td style="width: 50%; padding: 20px 5px 0px 0px">
                        <label>Effective Date</label>
                        <asp:TextBox ID="TB_Effective_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_Effective_Date" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_Effective_Date">
                        </asp:RequiredFieldValidator>
                        <asp:Label ID="neweffectivedateerrormsg" runat="server" Text="Effective date must be equals or later date" Visible="false" CssClass="invalid-feedback"></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td style="width: 50%; padding: 20px 5px 0px 0px">
                        <label>PO No.</label>
                        <asp:TextBox ID="TB_PO_No" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_PO_No" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_PO_No">
                        </asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 50%; padding: 20px 5px 0px 0px">
                        <label>PO Date</label>
                        <asp:TextBox ID="TB_PO_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_PO_Date" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_PO_Date">
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                        <label>Bind Key</label>
                        <asp:TextBox ID="TB_Bind_Key" runat="server" CssClass="form-control" placeholder="XXXX-XXXX-XXXX-XXXX-XXXX"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredField_TB_Bind_Key" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_Bind_Key">
                        </asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegexValidator_Bind_Key" runat="server" ValidationGroup="CZLModelUpdate"
                            ErrorMessage="Invalid key format. Use XXXX-XXXX-XXXX-XXXX-XXXX format."
                            ValidationExpression="^[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}$"
                            CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_Bind_Key">
                        </asp:RegularExpressionValidator>
                        <asp:Label ID="bindkeyexisterrmsg" runat="server" Text="Bind key exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                        <label>Remarks</label>
                        <asp:TextBox ID="TB_Remarks" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td style="width: 100%; padding: 20px 5px 0px 0px" colspan="2">
                        <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="CZLModelUpdate" />
                    </td>
                </tr>
            </table>
        </div>
    </div>


    <!-- Model Trails Section -->
    <div class="card border-light mb-3" style="width: 65%; display: inline-block; margin-left: 5px">
        <div class="card-header">
            <h4 class="card-title">Model Upgrade Trails</h4>
        </div>
        <div class="card-body">
            <div style="width:99.3%">
                <table border="0" class="table table-bordered">
                    <tr class="table table-light">
                        <th style="width:50%;text-wrap:none"></th>
                        <th style="width:50%;text-wrap:none; text-align:center" colspan="2">Grace Period:&emsp;<asp:Label ID="LB_Grace_Period" runat="server"></asp:Label>
                            <asp:Label ID="LB_Grace_Period_End_Date" runat="server" Visible="false"></asp:Label>
                        </th>
                    </tr>
                    <tr class="table table-light">
                        <th style="width:50%;text-wrap:none">Current Model</th>
                        <th style="width:25%;text-wrap:none">Grace Period Model Upgrade <br /><i style="color:grey">(Max 4x)</i></th>
                        <th style="width:25%;text-wrap:none">Model Upgrade After Grace Period <br /><i style="color:grey">(Chargeable)</i></th>
                    </tr>
                    <tr>
                        <td><asp:Label ID="LB_Current_Model" runat="server"></asp:Label>
                            <asp:Label ID="LB_Current_Effective_Date" runat="server" Visible="false"></asp:Label>
                        </td>
                        <td><asp:Label ID="LB_Update_Within_Grace_Period" runat="server"></asp:Label></td>
                        <td><asp:Label ID="LB_Update_After_Grace_Period" runat="server"></asp:Label></td>
                    </tr>
                </table>
            </div>
            <asp:GridView ID="GridView1" runat="server">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="12%" HeaderText="From Model">
                        <ItemTemplate>
                            <asp:Label ID="LB_From_Model" runat="server" Text='<%# Eval("From_Model") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_From_Model" runat="server" Text='<%# Eval("From_Model") %>' CssClass="form-control" Enabled="false"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="12%" HeaderText="To Model">
                        <ItemTemplate>
                            <asp:Label ID="LB_To_Model" runat="server" Text='<%# Eval("To_Model") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_To_Model" runat="server" Text='<%# Eval("To_Model") %>' CssClass="form-control" Enabled="false"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%" HeaderText="Effective Date">
                        <ItemTemplate>
                            <asp:Label ID="LB_Effective_Date" runat="server" Text='<%# String.Format("{0:dd MMM yy}", Eval("Effective_Date")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Effective_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Effective_Date")) %>' CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="16%" HeaderText="Bind Key">
                        <ItemTemplate>
                            <asp:Label ID="LB_Bind_Key" runat="server" Text='<%# Eval("Bind_Key") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Bind_Key" runat="server" Text='<%# Eval("Bind_Key") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="40%" HeaderText="Remarks">
                        <ItemTemplate>
                            <asp:Label ID="LB_Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Remarks" runat="server" Text='<%# Eval("Remarks") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%" HeaderText="Last Updated">
                        <ItemTemplate>
                            <asp:Label ID="LB_Last_Update" runat="server" Text='<%# String.Format("{0:dd MMM yy}", Eval("Last_Update")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:Label ID="LB_Last_Update" runat="server" Text='<%# String.Format("{0:dd MMM yy}", Eval("Last_Update")) %>'></asp:Label>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="0%" ItemStyle-Wrap="false"
                        EditText="<span class='btn btn-xs btn-info'><i class='bi bi-pencil-fill'></i></span>"
                        DeleteText="<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>"
                        UpdateText="<span class='btn btn-xs btn-success'><i class='bi bi-save2'></i></span>"
                        CancelText="<span class='btn btn-xs btn-secondary'><i class='bi bi-x-lg'></i></span>" />
                </Columns>
            </asp:GridView>
        </div>
    </div>



    <!-- Control Button -->
    <div style="clear: both">
        <hr />
        <asp:LinkButton ID="BT_Close" Text="Close" CommandName="Close" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>
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

</asp:Content>

