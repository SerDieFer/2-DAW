<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderController.ascx.cs" Inherits="GesPresta.UserFormController" %>

<link href="StyleSheet.css" rel="stylesheet" />

<div id="Header">
    <asp:LinkButton ID="LinkButtonInicio" runat="server" CssClass="inicioBtn">Inicio</asp:LinkButton>
    <asp:LinkButton ID="LinkButtonEmpleados" runat="server" CssClass="empleadosBtn">Empleados</asp:LinkButton>
    <asp:LinkButton ID="LinkButtonPrestaciones" runat="server" CssClass="prestacionesBtn">Prestaciones</asp:LinkButton>
    <br />
    <h1>ACME INNOVACIÓN, S. FIG.</h1>
    <asp:Label ID="LabelSubTitle" runat="server" Text="SubTitleLabel" CssClass="subtitleLbl">Gestión de Prestaciones Sociales</asp:Label>
    <hr />
</div>
