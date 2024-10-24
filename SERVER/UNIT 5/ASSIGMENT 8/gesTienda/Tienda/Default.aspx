<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Tienda._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Inicio Sesión</title>
    <link href="~/Estilos/HojaEstilo.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            <div id="cabecera">
                <div id="cabecera1">
                    <br />
                    comerciodaw.com &nbsp;
                </div>
                <div id="cabecera2">
                    <br />
                    &nbsp;&nbsp;TIENDA ONLINE - SHOPPING DAW<br />
                    <br />
                </div>
            </div>
            <div id="cuerpoLogin">
                <h1>GesTienda</h1>
                <br />
                <asp:Login ID="Login1" runat="server" OnAuthenticate="Login1_Authenticate">
                    <LayoutTemplate>
                        <table cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
                            <tr>
                                <td>
                                    <table cellpadding="0" class="auto-style1">
                                        <tr>
                                            <td align="center" colspan="2"><h3>Iniciar sesión</h3></td>
                                        </tr>
                                        <tr>
                                            <td align="left" class="auto-style2">
                                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><b>Nombre de usuario:</b></asp:Label>
                                            </td>
                                            <td class="auto-style2" align="right">
                                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="El nombre de usuario es obligatorio." ToolTip="El nombre de usuario es obligatorio." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><b>Contraseña:</b></asp:Label>
                                            </td>
                                            <td align="right">
                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="La contraseña es obligatoria." ToolTip="La contraseña es obligatoria." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2" style="color: Red; height: 2em">
                                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" colspan="2">
                                                <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Inicio de sesión" ValidationGroup="Login1" Width="100%" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </LayoutTemplate>
                </asp:Login>
            </div>
            <div id="pie">
                <br />
                <br />
                © Desarrollo de Aplicaciones Web interactivas con Acceso a Datos
                <br />
                IES Mare Nostrum (Alicante)
            </div>
        </div>
    </form>
</body>
</html>
