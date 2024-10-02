<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Lendings.aspx.cs" Inherits="GesPresta.Lendings" %>
<%@ Register src="~/UserControllers/HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>PRESTACIONES</title>
    <link href="../../CSS/LendingStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formLending" runat="server">

         <uc1:HeaderController ID="HeaderController1" runat="server" />
         <h2>DATOS DE LAS PRESTACIONES</h2>

         <div id="lendingDataDiv">

             <label>Código Prestación</label>
             <asp:TextBox ID="txtbLendingCode" runat="server"></asp:TextBox>

             <label>Descripción</label>
             <asp:TextBox ID="txtbLendingDescription" runat="server"></asp:TextBox>

             <label>Importe Fijo</label>
             <asp:TextBox ID="txtbLendingFixedValue" runat="server"></asp:TextBox>

             <label>Porcentaje del Importe</label>
             <asp:TextBox ID="txtbLendingPercentage" runat="server"></asp:TextBox>

             <label>Tipo de Prestación</label>
             <asp:DropDownList ID="ddlLendingType" runat="server">
                 <asp:ListItem Selected="True">Dentaria</asp:ListItem>
                 <asp:ListItem>Familiar</asp:ListItem>
                 <asp:ListItem>Ocular</asp:ListItem>
                 <asp:ListItem>Otras</asp:ListItem>
             </asp:DropDownList>

         </div>

         <div class="lendingDataButtonDiv">
             <asp:Button ID="btnSendDataLending" runat="server" Text="Enviar" />
         </div>
    </form>
</body>
</html>
