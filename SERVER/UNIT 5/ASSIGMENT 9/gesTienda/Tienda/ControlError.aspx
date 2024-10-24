<%@ OutputCache Duration="1" VaryByParam="None" %> 
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlError.aspx.cs" Inherits="Tienda.ControlError" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Error</title>
    <link href="Estilos/ControlError.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="controlError">
            <h1>Aplicación Web GesTienda</h1>
            <h2>Error en tiempo de ejecución</h2>
            <section id="errorSection">
                <label>Error ASP.NET:</label>
                <asp:label ID="lblErrorASP" runat="server" ForeColor="red"></asp:label>
                <p></p>
                <label>Error ADO.NET:</label>
                <asp:label ID="lblErrorADO" runat="server" ForeColor="red"></asp:label>
            </section>
        </div>
    </form>
</body>
</html>
