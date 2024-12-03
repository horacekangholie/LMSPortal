<%@ Page Title="LMSPortal - Server Diskspace Reading" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Diskspace_Reading.aspx.vb" 
    Inherits="Adhoc_Diskspace_Reading" %>

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
                    <h4 class="card-title">Add Diskspace Reading</h4>
                </div>
                <div class="card-body">
                    <table border="0" style="width: 100%">
                        <tr style="vertical-align: top">
                            <td style="width: 25%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Total Space</label>
                                <asp:TextBox ID="TB_Total_Space" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Total_Space" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Total_Space">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_Total_Space" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Total_Space"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 25%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Used</label>
                                <asp:TextBox ID="TB_Used" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Used" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Used">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_Used" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Used"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 25%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">Avail</label>
                                <asp:TextBox ID="TB_Avail" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Avail" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Avail">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_Avail" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Avail"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 25%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">% Used</label>
                                <asp:TextBox ID="TB_Percentage_Used" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_Percentage_Used" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Percentage_Used">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_Percentage_Used" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_Percentage_Used"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 25%; padding: 20px 10px 0px 0px">
                                <label style="white-space:nowrap">DB Size</label>
                                <asp:TextBox ID="TB_DB_Size" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredField_TB_DB_Size" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_DB_Size">
                                </asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegEx_TB_DB_Size" runat="server" ValidationGroup="ServerSpaceReading"
                                    ErrorMessage="Only numeric value" CssClass="invalid-feedback"
                                    Display="Dynamic"
                                    SetFocusOnError="True"
                                    ControlToValidate="TB_DB_Size"
                                    ValidationExpression="^[+-]?(?:\d+\.?\d*|\d*\.?\d+)[\r\n]*$">
                                </asp:RegularExpressionValidator>
                            </td>
                            <td style="width: 75%; padding: 20px 10px 0px 0px" colspan="3">
                                <label>Remarks</label>
                                <asp:TextBox ID="TB_Remarks" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                        </tr>
                        <tr style="vertical-align: top">
                            <td style="width: 100%; padding: 20px 10px 0px 0px" colspan="2">
                                <asp:Button ID="BN_AddNew" runat="server" Text="Add New" CssClass="btn btn-default" CausesValidation="true" ValidationGroup="ServerSpaceReading" />
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
                    <asp:TemplateField ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Reading_Date" runat="server" Text='<%# String.Format("{0:yyyy-MM-dd - hh:mm tt}", Eval("Reading_Date")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Reading_Date" runat="server" Text='<%# Eval("Reading_Date") %>' TextMode="DateTime" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Size" runat="server" Text='<%# String.Format("{0:# GB}", Eval("Size")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Size" runat="server" Text='<%# Eval("Size") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Used" runat="server" Text='<%# String.Format("{0:# GB}", Eval("Used")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Used" runat="server" Text='<%# Eval("Used") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Avail" runat="server" Text='<%# String.Format("{0:# GB}", Eval("Avail")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Avail" runat="server" Text='<%# Eval("Avail") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Used_Percentage" runat="server" Text='<%# String.Format("{0:P0}", (Eval("Used_Percentage")) / 100) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Used_Percentage" runat="server" Text='<%# Eval("Used_Percentage") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:Label ID="LB_DB_Size" runat="server" Text='<%# String.Format("{0:# GB}", Eval("DB_Size")) %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_DB_Size" runat="server" Text='<%# Eval("DB_Size") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="30%">
                        <ItemTemplate>
                            <asp:Label ID="LB_Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TB_E_Remarks" runat="server" Text='<%# Eval("Remarks") %>' CssClass="form-control"></asp:TextBox>
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

