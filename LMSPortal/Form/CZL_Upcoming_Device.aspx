<%@ Page Title="LMSPortal - Upcoming AI Devices" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="CZL_Upcoming_Device.aspx.vb" 
    Inherits="Form_CZL_Upcoming_Device" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

    <style type="text/css">
        .CheckboxSelecttion input {
            width: 16px;
            height: 16px;
            display: inline-block;
            vertical-align: top;
        }
        .selected {
            background-color: #eeeeee;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- New AI Device Section -->
    <div style="width: 100%; float: right">
        <div style="width: 100%; float: left">
            <div class="card border-light mb-3" style="width: 100%">
                <div class="card-header">
                    <h4 class="card-title">Add New Device</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 20%; padding: 10px 10px 10px 0px">
                                <label>Device Serial</label>
                                <asp:TextBox ID="TB_Device_Serial" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Device_Serial" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Device_Serial">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 35%; padding: 10px 10px 10px 0px">
                                <label>Device ID</label>
                                <asp:TextBox ID="TB_Device_ID" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Device_ID" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Device_ID">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 10px 0px">
                                <label>Model</label>
                                <asp:TextBox ID="TB_Model" runat="server" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Model" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Model">
                                </asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 10px 0px">
                                <label>AI Software Version</label>
                                <asp:TextBox ID="TB_AI_Software_Version" runat="server" CssClass="form-control" MaxLength="10" placeholder="(optional)"></asp:TextBox>
                            </td>
                            <td style="width: 15%; padding: 10px 10px 10px 0px">
                                <label>R Version</label>
                                <asp:TextBox ID="TB_R_Version" runat="server" CssClass="form-control" MaxLength="10" placeholder="(optional)"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 20%; padding: 10px 10px 10px 0px">
                                <label>Scale Serial No</label>
                                <asp:TextBox ID="TB_Scale_Serial_No" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Scale_Serial_No" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Scale_Serial_No">
                                </asp:RequiredFieldValidator>
                                <!-- error message when duplicated scale serial no found -->
                                <asp:Label ID="czldeviceserialexisterrormsg" runat="server" Text="Serial number exists" Visible="false" CssClass="invalid-feedback"></asp:Label>
                            </td>
                            <td style="width: 20%; padding: 10px 10px 10px 0px">
                                <label>MAC Address</label>
                                <asp:TextBox ID="TB_MAC_Address" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegEx_TB_MAC_Address" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="MAC Address format incorrect" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_MAC_Address"
                                    ValidationExpression="^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})|([0-9a-fA-F]{4}\\.[0-9a-fA-F]{4}\\.[0-9a-fA-F]{4})$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 25%; padding: 10px 10px 10px 0px" colspan="2">
                                <label>Production Licence No</label>
                                <asp:TextBox ID="TB_Production_Licence_No" runat="server" CssClass="form-control" placeholder="(optional)"></asp:TextBox>
                            </td>
                            <td style="width: 35%; padding: 10px 10px 10px 0px">
                                <label>Location</label>
                                <asp:TextBox ID="TB_Location" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Location" runat="server" ValidationGroup="CZLDevice"
                                    ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Location">
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="padding: 10px 10px 10px 0px" colspan="5">
                                <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" ValidationGroup="CZLDevice" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <div style="width: 100%; float: left">
            <div class="card border-light mb-3" style="width: 100%">
                <div class="card-body">
                    <!-- Use Panel and set default button to trigger validation -->
                    <asp:Panel ID="pnlSearchBox" runat="server" DefaultButton="BT_Search">
                        <!-- Searchbox -->
                        <div class="form-group form-inline" style="float: right">
                            <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
                            <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" CausesValidation="false" />
                        </div>
                    </asp:Panel>

                    <!-- Assign CZL Client Account button -->
                    <div class="form-group form-inline" style="float: left">
                        <table border="0" style="width: 30%">
                            <tr style="vertical-align: top">
                                <td style="width: 80%; padding: 0px 5px 10px 0px">
                                    <asp:DropDownList ID="DDL_CZL_Client_ID" runat="server" CssClass="form-control dropdown" AppendDataBoundItems="true">
                                        <asp:ListItem Text="" Value="-1"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CompareValidator ID="CompareValidator_DDL_CZL_Client_ID" runat="server" ValidationGroup="AssignCZLAccount"
                                        ErrorMessage="Please select CZL client account" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="true"
                                        Operator="NotEqual" ValueToCompare="-1"
                                        ControlToValidate="DDL_CZL_Client_ID" Type="String">
                                    </asp:CompareValidator>
                                </td>
                                <td style="width: 20%; padding: 0px 5px 10px 0px">
                                    <asp:Button ID="BT_Assign" runat="server" Text="Assign" CssClass="form-control btn btn-default" CausesValidation="true" ValidationGroup="AssignCZLAccount"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <!-- Gridview section -->
                    <asp:GridView ID="GridView1" runat="server">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkHeader" runat="server" CssClass="CheckboxSelecttion" OnClick="javascript:SelectAllCheckboxesSpecific(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkRow" runat="server" CssClass="CheckboxSelecttion" OnClick="javascript:HighlightRow(this);" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="0%">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Unique_ID" runat="server" Text='<%# Eval("Unique_ID") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Unique_ID" runat="server" Text='<%# Eval("Unique_ID") %>' CssClass="form-control" Visible="false"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="10%" HeaderText="Device Serial">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Device_Serial" runat="server" Text='<%# Eval("Device_Serial") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Device_Serial" runat="server" Text='<%# Eval("Device_Serial") %>' CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Device_Serial" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Device_Serial">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="15%" HeaderText="Device ID">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Device_ID" runat="server" Text='<%# Eval("Device_ID") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Device_ID" runat="server" Text='<%# Eval("Device_ID") %>' CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Device_ID" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Device_ID">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="6%" HeaderText="Model">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Model" runat="server" Text='<%# Eval("Model") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Model" runat="server" Text='<%# Eval("Model") %>' CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Model" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Model">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="7%" HeaderText="AI Soft. Ver">
                                <ItemTemplate>
                                    <asp:Label ID="LB_AI_Software_Version" runat="server" Text='<%# Eval("AI_Software_Version") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_AI_Software_Version" runat="server" Text='<%# Eval("AI_Software_Version") %>' CssClass="form-control"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="6%" HeaderText="R Ver">
                                <ItemTemplate>
                                    <asp:Label ID="LB_R_Version" runat="server" Text='<%# Eval("R_Version") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_R_Version" runat="server" Text='<%# Eval("R_Version") %>' CssClass="form-control"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="7%" HeaderText="Scale SN">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Scale_SN" runat="server" Text='<%# Eval("Scale_SN") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Scale_SN" runat="server" Text='<%# Eval("Scale_SN") %>' CssClass="form-control" Enabled="false"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Scale_SN" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Scale_SN">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="10%" HeaderText="MAC Address">
                                <ItemTemplate>
                                    <asp:Label ID="LB_MAC_Addr" runat="server" Text='<%# Eval("MAC_Addr") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_MAC_Addr" runat="server" Text='<%# Eval("MAC_Addr") %>' CssClass="form-control"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegEx_TB_E_MAC_Address" runat="server"
                                        ErrorMessage="MAC Address format incorrect" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_MAC_Addr"
                                        ValidationExpression="^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})|([0-9a-fA-F]{4}\\.[0-9a-fA-F]{4}\\.[0-9a-fA-F]{4})$">
                                    </asp:RegularExpressionValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="14%" HeaderText="Prod. Licence No">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Production_Licence_No" runat="server" Text='<%# Eval("Production_Licence_No") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Production_Licence_No" runat="server" Text='<%# Eval("Production_Licence_No") %>' CssClass="form-control"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="16%" HeaderText="Location">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Location" runat="server" Text='<%# Eval("Location") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Location" runat="server" Text='<%# Eval("Location") %>' CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Location" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Location">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="9%" HeaderText="Effective Date">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Effective_Date" runat="server" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Effective_Date")) %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TB_E_Effective_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd}", Eval("Effective_Date")) %>' CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredField_TB_E_Effective_Date" runat="server"
                                        ErrorMessage="Field cannot be blank" CssClass="invalid-feedback"
                                        Display="Dynamic"
                                        SetFocusOnError="True"
                                        ControlToValidate="TB_E_Effective_Date">
                                    </asp:RequiredFieldValidator>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="0%"
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
        </div>
    </div>



    <!-- Javascript for checkbox all -->
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" language="javascript">
        $('[id$=chkHeader]').click(function (e) {
            $('[name$="chkRow"]').prop('checked', this.checked);
            if ($(this).is(':checked') == true) {
                $('#PageContent_GridView1 tr').addClass('selected');
            } else {
                $('#PageContent_GridView1 tr').removeClass('selected');
            }
        });

        $('[name$=chkRow]').click(function (e) {
            if ($('[name$="chkRow"]:checked').length == $('[name$="chkRow"]').length || !this.checked)
                $('[id$=chkHeader]').prop('checked', this.checked);
        });
        
        $('#PageContent_GridView1 tr').click(function (e) {
            $(this).toggleClass('selected');
        });
    </script>


</asp:Content>

