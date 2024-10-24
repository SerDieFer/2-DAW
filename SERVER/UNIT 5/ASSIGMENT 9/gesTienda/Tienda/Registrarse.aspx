<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registrarse.aspx.cs" Inherits="Tienda.Registrarse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Registro</title>
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
            <div id="cuerpoRegistro">
                <h1>GesTienda</h1>
                <asp:Table ID="registro" runat="server" CssClass="registroUsuarioTabla">
                    <asp:TableRow>
                        <asp:TableCell align="center" colspan="2">
                            <h3>Registro de Usuario</h3>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">Correo Electrónico</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                            <%--<asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" ErrorMessage="El correo electrónico es obligatorio." ToolTip="El correo electrónico es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Contraseña</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                            <%--<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="La contraseña es obligatoria." ToolTip="La contraseña es obligatoria." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirmar Contraseña</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                          <%--  <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword" ErrorMessage="Confirmar contraseña es obligatorio." ToolTip="Confirmar contraseña es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="NifLabel" runat="server" AssociatedControlID="Nif">NIF/NIE/CIF</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                           <%-- <asp:RequiredFieldValidator ID="NifRequired" runat="server" ControlToValidate="Nif" ErrorMessage="El NIF/NIE/CIF es obligatorio." ToolTip="El NIF/NIE/CIF es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Nif" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="NombreRazonSocialLabel" runat="server" AssociatedControlID="NombreRazonSocial">Nombre/RazónSocial</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                          <%--  <asp:RequiredFieldValidator ID="NombreRazonSocialRequired" runat="server" ControlToValidate="NombreRazonSocial" ErrorMessage="El nombre/razón social es obligatorio." ToolTip="El nombre/razon social es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="NombreRazonSocial" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="DireccionLabel" runat="server" AssociatedControlID="Direccion">Dirección</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                         <%--   <asp:RequiredFieldValidator ID="DireccionRequired" runat="server" ControlToValidate="Direccion" ErrorMessage="La dirección es obligatoria." ToolTip="La dirección es obligatoria." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Direccion" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="PoblacionLabel" runat="server" AssociatedControlID="Poblacion">Población</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                        <%--    <asp:RequiredFieldValidator ID="PoblacionRequired" runat="server" ControlToValidate="Poblacion" ErrorMessage="La población es obligatoria." ToolTip="La población es obligatoria." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Poblacion" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="CodigoPostalLabel" runat="server" AssociatedControlID="CodigoPostal">Código Postal</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                          <%--  <asp:RequiredFieldValidator ID="CodigoPostalRequired" runat="server" ControlToValidate="CodigoPostal" ErrorMessage="El código postal es obligatorio." ToolTip="El código postal es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="CodigoPostal" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell align="left" class="registroTdLeft">
                            <asp:Label ID="TelefonoLabel" runat="server" AssociatedControlID="Telefono">Teléfono</asp:Label>
                        </asp:TableCell>
                        <asp:TableCell align="left" class="registroTdRight">
                           <%-- <asp:RequiredFieldValidator ID="TelefonoRequired" runat="server" ControlToValidate="Telefono" ErrorMessage="El teléfono es obligatorio." ToolTip="El teléfono es obligatorio." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>--%>
                            <asp:TextBox ID="Telefono" runat="server"></asp:TextBox>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell class="specialRegistry" align="left" colspan="2">
                            <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="Contraseña y Confirmar contraseña deben coincidir." ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell class="specialRegistry" align="left" colspan="2" style="color: Red;">
                            <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell class="specialRegistry" align="center" colspan="2" >
                            <asp:Button ID="RegisterButton" runat="server" CommandName="Registry" Text="INSERTAR" Width="100%" Height="2em" OnClick="RegisterButton_Click" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <br />
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
