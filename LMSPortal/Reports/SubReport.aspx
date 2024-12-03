<%@ Page Title="MM" Language="VB" 
    AutoEventWireup="false" 
    CodeFile="SubReport.aspx.vb" 
    Inherits="Reports_SubReport" %>

<!DOCTYPE html>

  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">

  <!-- Style sheet and Script -->
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>  <%--This is required for tab effect--%>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
  <link rel="stylesheet" href="../Content/ContentPage.css" />

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="margin: 20px">
                <!-- Searchbox -->
                <div class="form-group form-inline" style="float: right; display: block;margin-top:50px">
                    <asp:TextBox ID="TB_Search" runat="server" CssClass="form-control pull-left" Width="400" placeholder="Type your text here..."></asp:TextBox>
                    <asp:Button ID="BT_Search" runat="server" CssClass="form-control btn btn-outline btn-info pull-left" Text="Search" />
                </div>
                <asp:GridView ID="GridView1" runat="server"></asp:GridView>
            </div>
            <div style="margin: 20px">
                <asp:LinkButton ID="btnClose" runat="server" Text="Close" CssClass="btn btn-default" OnClick="btnClose_Click"></asp:LinkButton>
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-default" />
            </div>
        </div>
    </form>
</body>
</html>
