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
                <table cellpadding="0" class="auto-style1">
                    <tr>
                        <td align="center" colspan="2">
                            <h3>Registro de Usuario</h3>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email"><b>Correo Electrónico:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="Email" runat="server" placeholder="tuemail@ejemplo.com"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="NifLabel" runat="server" AssociatedControlID="Nif"><b>NIF/NIE/CIF:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="Nif" runat="server" placeholder="Introduce tu NIF/NIE/CIF"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><b>Contraseña:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" placeholder="Introduce tu contraseña"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"><b>Confirmar Contraseña:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" placeholder="Confirma tu contraseña"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="NombreRazonSocialLabel" runat="server" AssociatedControlID="NombreRazonSocial"><b>Nombre/Razón Social:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="NombreRazonSocial" runat="server" placeholder="Introduce tu nombre o razón social"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="DireccionLabel" runat="server" AssociatedControlID="Direccion"><b>Dirección:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="Direccion" runat="server" placeholder="Introduce tu dirección"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="PoblacionLabel" runat="server" AssociatedControlID="Poblacion"><b>Población:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="Poblacion" runat="server" placeholder="Introduce tu población"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="CodigoPostalLabel" runat="server" AssociatedControlID="CodigoPostal"><b>Código Postal:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">
                            <asp:TextBox ID="CodigoPostal" runat="server" placeholder="Introduce tu código postal"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" class="registroTdLeft">
                            <asp:Label ID="TelefonoLabel" runat="server" AssociatedControlID="Telefono"><b>Teléfono:</b></asp:Label>
                        </td>
                        <td align="left" class="registroTdRight">

                            <asp:TextBox ID="Telefono" runat="server" placeholder="Introduce tu teléfono"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="specialRegistry" align="center" colspan="2" style="height: 4em">
                            <asp:Button ID="RegisterButton" runat="server" CommandName="Registry" Text="INSERTAR" Width="100%" Height="2em" OnClick="RegisterButton_Click" ValidationGroup="validar" />
                        </td>
                    </tr>
                </table>

                <div class="filaEspecial" style="color: red">

                    <%--DE MOMENTO NO LOS PONEMOS OBLIGATORIOS           
                  <asp:RequiredFieldValidator ID="NombreRazonSocialRequired" runat="server" ControlToValidate="NombreRazonSocial" ErrorMessage="El nombre/razón social es obligatorio." ToolTip="El nombre/razón social es obligatorio." ValidationGroup="validar"></asp:RequiredFieldValidator>
                  <asp:RequiredFieldValidator ID="DireccionRequired" runat="server" ControlToValidate="Direccion" ErrorMessage="La dirección es obligatoria." ToolTip="La dirección es obligatoria." ValidationGroup="validar"></asp:RequiredFieldValidator>
                  <asp:RequiredFieldValidator ID="PoblacionRequired" runat="server" ControlToValidate="Poblacion" ErrorMessage="La población es obligatoria." ToolTip="La población es obligatoria." ValidationGroup="validar"></asp:RequiredFieldValidator>
                  <asp:RequiredFieldValidator ID="TelefonoRequired" runat="server" ControlToValidate="Telefono" ErrorMessage="El teléfono es obligatorio." ToolTip="El teléfono es obligatorio." ValidationGroup="validar"></asp:RequiredFieldValidator>
                  <asp:RequiredFieldValidator ID="CodigoPostalRequired" runat="server" ControlToValidate="CodigoPostal" ErrorMessage="El código postal es obligatorio." ToolTip="El código postal es obligatorio." ValidationGroup="validar"></asp:RequiredFieldValidator>--%>

                    <asp:RequiredFieldValidator
                        EnableViewState="False"
                        ID="EmailRequired"
                        runat="server"
                        ControlToValidate="Email"
                        ErrorMessage="El correo electrónico es obligatorio."
                        ValidationGroup="validar"
                        Display="Dynamic"
                        Style="display: none;"></asp:RequiredFieldValidator>

                    <%--<asp:RegularExpressionValidator
                        EnableViewState="False"
                        ID="emailRegex"
                        runat="server"
                        ControlToValidate="Email"
                        ValidationExpression="^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$"
                        ErrorMessage="El formato del correo electrónico es incorrecto."
                        ValidationGroup="validar"
                        Display="Dynamic"
                        Style="display: none;"></asp:RegularExpressionValidator>--%>

                    <asp:RequiredFieldValidator
                        ID="NifRequired" runat="server"
                        ControlToValidate="Nif"
                        ErrorMessage="El NIF/NIE/CIF es obligatorio."
                        ToolTip="El NIF/NIE/CIF es obligatorio."
                        ValidationGroup="validar"></asp:RequiredFieldValidator>

                    <%--<asp:RegularExpressionValidator
                        EnableViewState="False"
                        ID="passwordRegex"
                        runat="server"
                        ControlToValidate="Password"
                        ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"
                        ErrorMessage="La contraseña debe tener al menos 8 caracteres, una letra mayúscula, una letra minúscula, un número y un carácter especial."
                        ValidationGroup="validar"
                        Display="Dynamic"
                        Style="display: none;"></asp:RegularExpressionValidator>--%>

                    <asp:RequiredFieldValidator
                        EnableViewState="False"
                        ID="PasswordRequired"
                        runat="server"
                        ControlToValidate="Password"
                        ErrorMessage="La contraseña es obligatoria."
                        ValidationGroup="validar"
                        Display="Dynamic"
                        Style="display: none;"></asp:RequiredFieldValidator>

                    <asp:CompareValidator
                        EnableViewState="False"
                        ID="PasswordCompare"
                        runat="server"
                        ControlToCompare="Password"
                        ControlToValidate="ConfirmPassword"
                        ErrorMessage="Contraseña y Confirmar contraseña deben coincidir."
                        ValidationGroup="validar"
                        Display="Dynamic"
                        Style="display: none;"></asp:CompareValidator>

                    <%--<asp:RequiredFieldValidator
                    ID="ConfirmPasswordRequired"
                    runat="server"
                    ControlToValidate="ConfirmPassword"
                    ErrorMessage="Confirmar contraseña es obligatorio."
                    ToolTip="Confirmar contraseña es obligatorio."
                    ValidationGroup="validar"></asp:RequiredFieldValidator>--%>

                    <asp:Panel ID="ErrorPanel" runat="server" CssClass="errorPanel">
                        <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="True"></asp:Literal>
                        <asp:LinkButton PostBackUrl="~/Default.aspx" runat="server">Inicio de sesión</asp:LinkButton>
                    </asp:Panel>
                </div>
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
