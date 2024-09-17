<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="GesPresta.Default" %>
<%@ Register src="HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ACME</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="HomeStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formHome" runat="server">
        <uc1:HeaderController ID="HeaderController2" runat="server" />
        <div id="InicioContainer">
            <p> La corporación ACME está comprometida con sus empleados. Para ello ha establecido una serie de prestaciones 
             que pueden utilizar sus empleados para obtener ayudas sociales asociados a diversos gastos de tipo familiar, médico, etc.</p>
            <p>Esta aplicación a través de Web permite realizar todas las tareas de gestión relacionadas con la prestación de ayudas a los empleados.</p>
            <p>Para cualquier duda o consulta puede contactar con el Departamento de Ayuda Social: 
            <asp:LinkButton ID="LinkButtonAyudaSocialMail" runat="server">ayuda.social@acme.com</asp:LinkButton></p>
        </div>
    </form>
</body>
</html>