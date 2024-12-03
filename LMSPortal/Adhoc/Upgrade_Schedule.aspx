<%@ Page Title="LMSPortal - DMC Upgrade Schedule" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Upgrade_Schedule.aspx.vb" 
    Inherits="Adhoc_Upgrade_Schedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Use Panel and set default button to trigger validation -->
    <asp:Panel ID="pnlSearchBox" runat="server" DefaultButton="BT_Search">
        <!-- Searchbox -->
        <div class="form-group form-inline" style="float: right">
            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" CausesValidation="false" />
        </div>
    </asp:Panel>

    <div style="width: 100%; float: right">
        <div style="width: 30%; float: left">
            <div class="card border-light mb-3" style="width: 95%">
                <div class="card-header">
                    <h4 class="card-title">Add Maintenance Schedule</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 35%; padding: 20px 10px 0px 0px">
                                <label>Date</label>
                                <asp:TextBox ID="TB_Maintenance_Date" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Maintenance_Date" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Maintenance_Date">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 65%; padding: 20px 10px 0px 0px">
                                <label>Work Type</label>
                                <asp:TextBox ID="TB_Work_Type" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Work_Type" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Work_Type">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px" colspan="2">
                                <label>Description</label>
                                <asp:TextBox ID="TB_Description" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Description" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Description">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 20px 10px 0px 0px">
                                <label>Down Time From</label>
                                <asp:TextBox ID="TB_Downtime_From" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Downtime_From" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Downtime_From">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegexValidator_TB_Downtime_From" runat="server" ValidationGroup="MaintenanceSchedule" placeholder="00:00"
                                    ErrorMessage="Please enter time in 24-hour format (HH:mm)" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    ControlToValidate="TB_Downtime_From"
                                    ValidationExpression="^(?:[01]\d|2[0-3]):[0-5]\d$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 50%; padding: 20px 10px 0px 0px">
                                <label>Down Time To</label>
                                <asp:TextBox ID="TB_Downtime_To" runat="server" CssClass="form-control" AutoPostBack="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Downtime_To" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Downtime_To">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegexValidator_TB_Downtime_To" runat="server" ValidationGroup="MaintenanceSchedule" placeholder="00:00"
                                    ErrorMessage="Please enter time in 24-hour format (HH:mm)" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    ControlToValidate="TB_Downtime_To"
                                    ValidationExpression="^(?:[01]\d|2[0-3]):[0-5]\d$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 50%; padding: 20px 10px 0px 0px">
                                <label>Duration</label>
                                <asp:TextBox ID="TB_Duration" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Duration" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Duration">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 50%; padding: 20px 10px 0px 0px">
                                <label>Status</label>
                                <asp:DropDownList ID="DDL_Status" runat="server"
                                    AppendDataBoundItems="true" 
                                    AutoPostBack="true"
                                    CssClass="form-control">
                                    <asp:ListItem Text="Please select" Value="empty" />
                                </asp:DropDownList>
                                <asp:CompareValidator ID="CompareValidator_DDL_Status" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="Please select status" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    Operator="NotEqual" ValueToCompare="empty"
                                    ControlToValidate="DDL_Status" Type="String">
                                </asp:CompareValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px" colspan="2">
                                <label>Remarks</label>
                                <asp:TextBox ID="TB_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Remarks" runat="server" ValidationGroup="MaintenanceSchedule"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Remarks">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px" colspan="2">
                                <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="MaintenanceSchedule" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div style="margin-left: 30%">
            <!-- GridView -->
            <asp:GridView ID="GridView1" runat="server">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Maintenance_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Maintenance_Date")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Maintenance_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Maintenance_Date")) %>' TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Work_Type" runat="server" Text='<%# Eval("Work_Type") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Work_Type" runat="server" Text='<%# Eval("Work_Type") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Description" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Description" runat="server" Text='<%# Eval("Description") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Status" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Status" runat="server" Text='<%# Eval("Status") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Downtime_From" runat="server" Text='<%# Eval("Downtime_From") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Downtime_From" runat="server" Text='<%# Eval("Downtime_From") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Downtime_To" runat="server" Text='<%# Eval("Downtime_To") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Downtime_To" runat="server" Text='<%# Eval("Downtime_To") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Duration" runat="server" Text='<%# Eval("Duration") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Duration" runat="server" Text='<%# Eval("Duration") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="LB_Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Remarks" runat="server" Text='<%# Eval("Remarks") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:CommandField ShowEditButton="true" ShowDeleteButton="true"
                        EditText="<span class='btn btn-xs btn-info'><i class='bi bi-pencil-fill'></i></span>" 
                        DeleteText="<span class='btn btn-xs btn-danger'><i class='bi bi-trash'></i></span>"
                        UpdateText="<span class='btn btn-xs btn-success'><i class='bi bi-save2'></i></span>"
                        CancelText="<span class='btn btn-xs btn-secondary'><i class='bi bi-x-lg'></i></span>" />
                </Columns>
            </asp:GridView>

            <!-- Display total number of records from the GridView -->
            <asp:Label ID="FooterTotalCount" runat="server"></asp:Label>
        </div>
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

