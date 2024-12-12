<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ejercicio1.aspx.cs" Inherits="ServiciosWeb.Ejercicio1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style = "text-align: center">
            <h1>CONSUMIR UN SERVICIO WEB YA EXISTENTE</h1>
            <h2>Titulaciones Oficiales en la Universidad de Alicante</h2>
            <br />

            <span>Curso académico (formato aaaa-aa)&nbsp;</span> 
            <asp:TextBox ID="txtCurso" runat="server" placeholder="Introduzca el curso"></asp:TextBox>
            &nbsp;
            <asp:Button ID="btnObtenerInformacion" runat="server" Text="Obtener Información" OnClick="btnObtenerInformacion_Click" />
        </div>

            <br /><br />
            <asp:Label ID="lblResultado" runat="server" Text=""></asp:Label>

            <br /><br />
            <div style="display:table; width:70%; margin:auto">
                <asp:GridView ID="GridView1" runat="server"></asp:GridView>
            </div>
    </form>
</body>
</html>
