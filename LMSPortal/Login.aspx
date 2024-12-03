<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LMSPortal - Login</title>

    <!-- Style Sheet -->
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/Login.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.2/font/bootstrap-icons.css" />

</head>
<body>
    <div class="wrapper">
        <form class="form-signin" id="form1" runat="server">
            <h2 class="form-signin-logo"><i class="bi bi-house-fill" style="padding-right:8px"></i>LMSPortal</h2>
            <h4 class="form-signin-heading">Please login</h4>
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </form>
    </div>
</body>
</html>
