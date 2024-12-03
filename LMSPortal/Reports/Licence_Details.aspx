<%@ Page Title="LMSPortal - Licence Details" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Licence_Details.aspx.vb" 
    Inherits="Reports_Licence_Details" %>

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

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float:left">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control me-sm-2" Width="600" placeholder="e.g. Licence Code, MAC Address, Serial No., AI Device ID, Licensee"></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>

    <!-- Form Page Content -->
    <asp:FormView ID="FormView1" runat="server" Width="100%" AllowPaging="true" 
        OnModeChanging="FormView1_ModeChanging"
        OnPageIndexChanging="FormView1_PageIndexChanging">
        <ItemTemplate>
            <div>
                <div class="card border-light mb-3" style="width:50%;float:left;height:contain">
                    <div class="card-header">
                        <h4 class="card-title">Licence Details</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 20px 10px 10px 0px" colspan="3">
                                     <label>Licence Code</label>
                                     <asp:TextBox ID="TB_Licence_Code" Text='<%# Eval("Licence Code") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Application Type</label>
                                    <asp:TextBox ID="TB_Application_Type" Text='<%# Eval("Application Type") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>OS Type</label>
                                    <asp:TextBox ID="TB_OS_Type" Text='<%# Eval("OS Type") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Created Date</label>
                                    <asp:TextBox ID="TB_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 30%; padding: 20px 10px 10px 0px">
                                    <label>Activated On</label>
                                    <asp:TextBox ID="TB_Activated_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Activated Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Status</label>
                                    <asp:TextBox ID="TB_Status" Text='<%# Eval("Status") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Term</label>
                                    <asp:TextBox ID="TB_Licence_Term" Text='<%# IIf(CInt(Eval("Licence Term")) < 9999, Eval("Licence Term") & " month(s)", "No Expiry") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Expiry Date</label>
                                    <asp:TextBox ID="TB_Expired_Date" Text='<%# Eval("Expired Date") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 75%; padding: 20px 10px 10px 0px">
                                    <label>MAC Address / UUID</label>
                                    <asp:TextBox ID="TB_MAC_Address" Text='<%# Eval("MAC Address") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Serial No.</label>
                                    <asp:TextBox ID="TB_Serial_No" Text='<%# Eval("Serial No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr id="AI_Device_ID_Row" runat="server" style="vertical-align: top">
                                <td style="width: 60%; padding: 20px 10px 10px 0px">
                                    <label>AI Device ID</label>
                                    <asp:TextBox ID="TB_AI_Device_ID" Text='<%# Eval("AI Device ID") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 40%; padding: 20px 10px 10px 0px">
                                    <label>AI Device Serial No.</label>
                                    <asp:TextBox ID="TB_AI_Device_Serial_No" Text='<%# Eval("AI Device Serial No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr id="DMC_Server_Licence_Row" runat="server" style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Max HQ</label>
                                    <asp:TextBox ID="TB_Max_HQ" Text='<%# Eval("Max HQ") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Max Store</label>
                                    <asp:TextBox ID="TB_Max_Store" Text='<%# Eval("Max Store") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="card border-light mb-3" style="width:49%;display:inline-block;margin-left:10px">
                    <div class="card-header">
                        <h4 class="card-title">Owner Details</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Licensee</label>
                                    <asp:TextBox ID="TB_Licensee" Text='<%# Eval("Licensee") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Email</label>
                                    <asp:TextBox ID="TB_Email" Text='<%# Eval("Email") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 20px 10px 10px 0px" colspan="2">
                                    <label>Remarks</label>
                                    <asp:TextBox ID="TB_Remarks" Text='<%# Eval("Remarks") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="card border-light mb-3" style="width:49%;display:inline-block;margin-left:10px">
                    <div class="card-header">
                        <h4 class="card-title">Order Information</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 60%; padding: 20px 10px 10px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_PO_No" Text='<%# Eval("PO No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_PO_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("PO Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Chargeable</label>
                                    <asp:TextBox ID="TB_Chargeable" Text='<%# Eval("Chargeable") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 30%; padding: 20px 10px 10px 0px">
                                    <label>Invoice No.</label>
                                    <asp:TextBox ID="TB_Invoice_No" Text='<%# Eval("Invoice No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Invoice Date</label>
                                    <asp:TextBox ID="TB_Invoice_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Invoice Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Requested By</label>
                                    <asp:TextBox ID="TB_Requested_By" Text='<%# Eval("Requested By") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div style="position:fixed;margin-top:20px">
                <asp:LinkButton ID="btnEdit" runat="server" Text="Edit" CommandName="Edit" CssClass="btn btn-default" OnClick="btnEdit_Click"></asp:LinkButton>
            </div>  
        </ItemTemplate>
        <EditItemTemplate>
            <div>
                <div class="card border-light mb-3" style="width: 50%; float: left; height: contain">
                    <div class="card-header">
                        <h4 class="card-title">Licence Details</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 20px 10px 10px 0px" colspan="3">
                                    <label>Licence Code</label>
                                    <asp:TextBox ID="TB_E_Licence_Code" Text='<%# Eval("Licence Code") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Application Type</label>
                                    <asp:TextBox ID="TB_E_Application_Type" Text='<%# Eval("Application Type") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>OS Type</label>
                                    <asp:TextBox ID="TB_E_OS_Type" Text='<%# Eval("OS Type") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Created Date</label>
                                    <asp:TextBox ID="TB_E_Created_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Created Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 30%; padding: 20px 10px 10px 0px">
                                    <label>Activated On</label>
                                    <asp:TextBox ID="TB_E_Activated_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Activated Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Status</label>
                                    <asp:TextBox ID="TB_E_Status" Text='<%# Eval("Status") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Term</label>
                                    <asp:TextBox ID="TB_E_Licence_Term" Text='<%# IIf(CInt(Eval("Licence Term")) < 9999, Eval("Licence Term") & " month(s)", "No Expiry") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Expiry Date</label>
                                    <asp:TextBox ID="TB_E_Expired_Date" Text='<%# Eval("Expired Date") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 75%; padding: 20px 10px 10px 0px">
                                    <label>MAC Address / UUID / DRM Key</label>
                                    <asp:TextBox ID="TB_E_MAC_Address" Text='<%# Eval("MAC Address") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 25%; padding: 20px 10px 10px 0px">
                                    <label>Serial No.</label>
                                    <asp:TextBox ID="TB_E_Serial_No" Text='<%# Eval("Serial No") %>' runat="server" CssClass="form-control" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr id="AI_E_Device_ID_Row" runat="server" style="vertical-align: top">
                                <td style="width: 60%; padding: 20px 10px 10px 0px">
                                    <label>AI Device ID</label>
                                    <asp:TextBox ID="TB_E_AI_Device_ID" Text='<%# Eval("AI Device ID") %>' runat="server" CssClass="form-control" />
                                </td>
                                <td style="width: 40%; padding: 20px 10px 10px 0px">
                                    <label>AI Device Serial No.</label>
                                    <asp:TextBox ID="TB_E_AI_Device_Serial_No" Text='<%# Eval("AI Device Serial No") %>' runat="server" CssClass="form-control" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr id="DMC_E_Server_Licence_Row" runat="server" style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Max HQ</label>
                                    <asp:TextBox ID="TB_E_Max_HQ" Text='<%# Eval("Max HQ") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Max Store</label>
                                    <asp:TextBox ID="TB_E_Max_Store" Text='<%# Eval("Max Store") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="card border-light mb-3" style="width: 49%; display: inline-block; margin-left: 10px">
                    <div class="card-header">
                        <h4 class="card-title">Owner Details</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Licensee</label>
                                    <asp:TextBox ID="TB_E_Licensee" Text='<%# Eval("Licensee") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Email</label>
                                    <asp:TextBox ID="TB_E_Email" Text='<%# Eval("Email") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                            <tr style="vertical-align: top">
                                <td style="width: 100%; padding: 20px 10px 10px 0px" colspan="2">
                                    <label>Remarks</label>
                                    <asp:TextBox ID="TB_E_Remarks" Text='<%# Eval("Remarks") %>' runat="server" CssClass="form-control" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="card border-light mb-3" style="width: 49%; display: inline-block; margin-left: 10px">
                    <div class="card-header">
                        <h4 class="card-title">Order Information</h4>
                    </div>
                    <div class="card-body">
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 60%; padding: 20px 10px 10px 0px">
                                    <label>PO No.</label>
                                    <asp:TextBox ID="TB_E_PO_No" Text='<%# Eval("PO No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>PO Date</label>
                                    <asp:TextBox ID="TB_E_PO_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("PO Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Chargeable</label>
                                    <asp:TextBox ID="TB_E_Chargeable" Text='<%# Eval("Chargeable") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                        <table border="0" style="width: 100%">
                            <tr style="vertical-align: top">
                                <td style="width: 30%; padding: 20px 10px 10px 0px">
                                    <label>Invoice No.</label>
                                    <asp:TextBox ID="TB_E_Invoice_No" Text='<%# Eval("Invoice No") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 20%; padding: 20px 10px 10px 0px">
                                    <label>Invoice Date</label>
                                    <asp:TextBox ID="TB_E_Invoice_Date" Text='<%# String.Format("{0:dd MMM yyyy}", Eval("Invoice Date")) %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                                <td style="width: 50%; padding: 20px 10px 10px 0px">
                                    <label>Requested By</label>
                                    <asp:TextBox ID="TB_E_Requested_By" Text='<%# Eval("Requested By") %>' runat="server" CssClass="form-control" ReadOnly="true" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div style="position:fixed;margin-top:20px">
                <asp:LinkButton ID="btnUpdate" runat="server" Text="Update" CommandName="Update" CssClass="btn btn-default" OnClick="btnUpdate_Click"></asp:LinkButton>
                <asp:LinkButton ID="btnCancel" runat="server" Text="Cancel" CommandName="Cancel" CssClass="btn btn-default" OnClick="btnCancel_Click"></asp:LinkButton>
            </div>
        </EditItemTemplate>
    </asp:FormView>

    <div id="NoRecordAlert" runat="server" class="alert alert-dismissible alert-warning">
        <span class="mb-0">No record found, please try again.</span>
    </div>

</asp:Content>

