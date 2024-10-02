<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LendingsDataValuesResponse.aspx.cs" Inherits="GesPresta.LendingsDataValuesResponse" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>VALORES FORMULARIO PRÉSTAMOS</title>
    <link href="../../../CSS/LendingStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formLending" runat="server">

         <!--Sin Width ni Backcolor, se la defino con CSS, sino el elemento quedaría así:
            <asp:Label ID="lblEployeeDataFull" runat="server" Width="60%" BackColor="#666FFFF" Visible="false"></asp:Label> 
         -->
         <asp:Label ID="lblLendingData" runat="server" Visible="false"></asp:Label>
    </form>
</body>
</html>
