<%@ Page Title="AI Licence Expiry Reminder" Language="VB" 
    AutoEventWireup="false" 
    CodeFile="AI_Licence_Expiry_Reminder.aspx.vb" 
    Inherits="Reports_AI_Licence_Expiry_Reminder" %>

<!DOCTYPE html>

  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">

  <!-- Style sheet and Script -->
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>  <%--This is required for tab effect--%>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
  <link rel="stylesheet" href="/Content/style.css" />

<style>
    .closeButton {
        -webkit-text-size-adjust: 100%;
        -webkit-tap-highlight-color: rgba(0,0,0,0);
        --blue: #007bff;
        --indigo: #6610f2;
        --purple: #6f42c1;
        --pink: #e83e8c;
        --red: #dc3545;
        --orange: #fd7e14;
        --yellow: #ffc107;
        --green: #28a745;
        --teal: #20c997;
        --cyan: #17a2b8;
        --white: #fff;
        --gray: #6c757d;
        --gray-dark: #343a40;
        --primary: #007bff;
        --secondary: #6c757d;
        --success: #28a745;
        --info: #17a2b8;
        --warning: #ffc107;
        --danger: #dc3545;
        --light: #f8f9fa;
        --dark: #343a40;
        --breakpoint-xs: 0;
        --breakpoint-sm: 576px;
        --breakpoint-md: 768px;
        --breakpoint-lg: 992px;
        --breakpoint-xl: 1200px;
        --font-family-sans-serif: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, "Noto Sans", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";
        --font-family-monospace: SFMono-Regular, Menlo, Monaco, Consolas, "Liberation Mono", "Courier New", monospace;
        font-family: "Helvetica Neue",Helvetica,Arial,sans-serif;
        box-sizing: border-box;
        text-decoration: none;
        transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out, -webkit-box-shadow 0.15s ease-in-out;
        display: inline-block;
        margin-bottom: 0;
        font-weight: 400;
        text-align: center;
        white-space: nowrap;
        vertical-align: middle;
        touch-action: manipulation;
        cursor: pointer;
        background-image: none;
        border: 1px solid transparent;
        padding: 6px 12px;
        font-size: 14px;
        line-height: 1.42857143;
        border-radius: 4px;
        user-select: none;
        color: #333;
        background-color: #fff;
        border-color: #ccc;
        margin-top: 20px;
    }

    .reportTitle {
        margin: 50px 0px 10px 5px;
        font-weight:700;
    }

    .reportViewAsOf {
        margin-left: 5px;
        margin-top: 50px;
        font-style:italic;
        color:#6f6f6f;
        display:inline-block;
        float:left;
    }

    .licenceExpiryDateRange{
        margin-left: 5px; 
        margin-bottom: 10px;
        color:#6f6f6f;
    }
</style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="margin: 20px">
                <asp:PlaceHolder ID="Dashboard" runat="server"></asp:PlaceHolder>
            </div>
            <div style="margin: 20px">
                <asp:Button ID="btnClose" runat="server" Text="Close" CssClass="closeButton" Visible="false"></asp:Button>
            </div>
            <div style="margin: 20px;margin-bottom:500px">
                <h4 class="reportViewAsOf">Report accurate as of <%= Now().ToString("F") %></h4>
            </div>
        </div>
    </form>

</body>
</html>
