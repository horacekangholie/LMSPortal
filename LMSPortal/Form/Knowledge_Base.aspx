<%@ Page Title="LMSPortal - Knowledge Base" 
    Language="VB" 
    MasterPageFile="~/MasterPages/Frontend.master" 
    AutoEventWireup="false" 
    CodeFile="Knowledge_Base.aspx.vb" 
    Inherits="Form_Knowledge_Base" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- Style sheet and Script -->
    <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>--%>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/ContentPage.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" Runat="Server">
    <!-- Page Title -->
    <h2 class="mb-4">
        <asp:Label ID="LB_PageTitle" runat="server"></asp:Label></h2>
    <hr />

    <!-- Searchbox -->
    <div class="form-group form-inline" style="float: left">
        <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control" Width="400" placeholder="Type your text here..."></asp:TextBox>
        <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info" Text="Search" />
    </div>


        <asp:Repeater ID="rptAccordian" runat="server">
            <ItemTemplate>
                <div class="accordion" id="knowledgeBase" style="float: left; clear: left; min-width: 100%; margin: 10px auto">
                    <div class="accordion-item">
                        <div id="H-<%# Eval("Article_No") %>" class="card-header">
                            <h4>
                                <button class="btn btn-link btn-block text-left" type="button" data-toggle="collapse" data-target="#C-<%# Eval("Article_No") %>" aria-expanded="true" aria-controls="C-<%# Eval("Article_No") %>">
                                    <%# Eval("Article_Title") %>
                                    <p style="float: right; font-style: italic; text-decoration: none; color: gray"><%# Eval("Article_Category") %></p>
                                </button>
                            </h4>
                        </div>
                        <div id="C-<%# Eval("Article_No") %>" class="collapse" aria-labelledby="H-<%# Eval("Article_No") %>" data-parent="#knowledgeBase">
                            <div class="card-body" style="padding: 20px 40px 200px 40px; border:1px solid #f7f7f7">
                                <p class="card-text"><%# Eval("Article_Content") %></p>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

</asp:Content>

