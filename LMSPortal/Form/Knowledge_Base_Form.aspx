<%@ Page Title="LMSPortal - Knowledge Base" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Knowledge_Base_Form.aspx.vb" 
    ValidateRequest="false"
    Inherits="Form_Knowledge_Base_Form" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />
    
    
    <script src="../Scripts/tinymce/tinymce.min.js"></script>
    <script type="text/javascript">
        tinymce.init({
            selector: 'textarea',
            plugins: "image, link, fullpage, imagetools, preview, pagebreak, table, save, insertdatetime, searchreplace, print, contextmenu, paste, directionality, fullscreen, noneditable, visualchars, nonbreaking, wordcount, advlist, autosave, autoresize",
            autoresize_bottom_margin: 50,
            min_height: 450
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    
    <hr />

    <!-- Main form -->
    <div class="card border-light mb-3">
        <div class="card-header">
            <h4 class="card-title">New Record</h4>
        </div>
        <div class="card-body">
            <table border="0" style="width: 100%">
                <tr style="vertical-align: top">
                    <td style="width: 65%; padding: 0px 10px 10px 0px">
                        <label>Title</label>
                        <asp:TextBox ID="TB_Article_Title" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredField_TB_Article_Title" runat="server" ValidationGroup="KnowledgeBase"
                            ErrorMessage="This field cannot be blank" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            ControlToValidate="TB_Article_Title">
                        </asp:RequiredFieldValidator>
                    </td>
                    <td style="width: 35%; padding: 0px 10px 10px 0px">
                        <label>Category</label>
                        <asp:TextBox ID="TB_Article_Category" runat="server" CssClass="form-control" Visible="false" />
                        <asp:DropDownList ID="DDL_Article_Category" runat="server" CssClass="form-control"></asp:DropDownList>
                        <asp:CompareValidator ID="CompareValidator_DDL_Article_Category" runat="server" ValidationGroup="KnowledgeBase"
                            ErrorMessage="Please select status" CssClass="invalid-feedback"
                            Display="Dynamic"
                            SetFocusOnError="True"
                            Operator="NotEqual" ValueToCompare="Please select"
                            ControlToValidate="DDL_Article_Category" Type="String">
                        </asp:CompareValidator>
                    </td>
                </tr>
                <tr style="vertical-align: top">
                    <td style="width: 100%; padding: 0px 10px 10px 0px" colspan="2">
                        <asp:TextBox ID="TB_Article_Content" runat="server" TextMode="MultiLine" />
                    </td>
                </tr>
                <tr id="articlecontentemptyerrormsg" runat="server" style="vertical-align: top" visible="false">
                    <td style="width: 800px; padding: 0px 0px 0px 0px" colspan="3">
                        <span style="font-size: 11px; color: #dc3545">Content cannot be blank</span>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <hr />

    <!-- Control Button -->
    <asp:LinkButton ID="BT_Create" Text="Create" CommandName="Create" runat="server" CssClass="btn btn-default" ValidationGroup="KnowledgeBase"></asp:LinkButton>
    <asp:LinkButton ID="BT_Cancel" Text="Cancel" CommandName="Cancel" runat="server" CssClass="btn btn-default" CausesValidation="false"></asp:LinkButton>

</asp:Content>

