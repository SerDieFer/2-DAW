<%@ OutputCache Duration="1" VaryByParam="None" %> 
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProductosVer.aspx.cs" Inherits="Tienda.ProductosVer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="InfoContenido" runat="server">
    <div id="contenido3">
        <div class="contenidotitulo">Productos</div>
            <br /> 
            <div style="width: 94.5vw;"> 
            <asp:Label ID="lblResultado" runat="server"></asp:Label> 
        </div>
        <br /> 
        <asp:Label ID="lblMensajes" ForeColor="red" runat="server"></asp:Label> 
        <br /> 
        <br /> 
    </div>
</asp:Content>
