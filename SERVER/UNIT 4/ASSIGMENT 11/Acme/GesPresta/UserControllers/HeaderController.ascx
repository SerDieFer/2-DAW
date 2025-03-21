﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderController.ascx.cs" Inherits="GesPresta.UserFormController" %>

<link href="/CSS/HeaderStyleSheet.css" rel="stylesheet" />

<div id="headerDiv">
    <asp:LinkButton ID="linkButtonHome" runat="server" CssClass="homeBtn" CausesValidation="false" PostBackUrl="~/WebForms/Home.aspx">Inicio</asp:LinkButton>
    <asp:LinkButton ID="linkButtonEmployees" runat="server" CssClass="employeesBtn" CausesValidation="false" PostBackUrl="~/WebForms/EmployeesForms/Employees.aspx">Empleados</asp:LinkButton>
    <asp:LinkButton ID="linkButtonLendings" runat="server" CssClass="lendingsBtn" CausesValidation="false" PostBackUrl="~/WebForms/LendingsForms/Lendings.aspx">Prestaciones</asp:LinkButton>
    <br />
    <h1>ACME INNOVACIÓN, S. FIG.</h1>
    <asp:Label ID="labelSubTitleHeader" runat="server" CssClass="labelSubtitleHeader">Gestión de Prestaciones Sociales</asp:Label>
    <hr />
</div>
