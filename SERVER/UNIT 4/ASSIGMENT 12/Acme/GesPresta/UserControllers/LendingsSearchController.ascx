<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LendingsSearchController.ascx.cs" Inherits="GesPresta.UserControllers.LendingsSearchController" %>

    <asp:ListBox ID="lstLendings" runat="server">
        <asp:ListItem Selected="True" Value="123-456-789">Gafas</asp:ListItem>
        <asp:ListItem Value="123-557-209">Gafas Progresivas</asp:ListItem>
        <asp:ListItem Value="123-787-001">Lentes de Contacto</asp:ListItem>
        <asp:ListItem Value="123-674-650">Substitución de Cristales</asp:ListItem>
        <asp:ListItem Value="157-010-338">Ortodoncia</asp:ListItem>
        <asp:ListItem Value="157-778-023">Obturación o Empaste</asp:ListItem>
        <asp:ListItem Value="157-114-995">Endodoncia</asp:ListItem>
        <asp:ListItem Value="012-783-146">Ayuda de Estudios</asp:ListItem>
    </asp:ListBox>