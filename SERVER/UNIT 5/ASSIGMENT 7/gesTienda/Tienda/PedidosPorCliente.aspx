<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageAdm.Master" AutoEventWireup="true" CodeBehind="PedidosPorCliente.aspx.cs" Inherits="Tienda.PedidosPorCliente" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="InfoContenido" runat="server">

    <asp:SqlDataSource 
        ID="SqlDataSource1" 
        runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        SelectCommand="SELECT * FROM [CLIENTE]">
    </asp:SqlDataSource>

    <div class="contenidotitulo">Pedidos realizados por los clientes</div>
    <br /> 

    <asp:GridView
        ID="grdClientes" 
        runat="server" 
        AllowPaging="True" 
        AutoGenerateColumns="False" 
        CellPadding="4" 
        DataKeyNames="IdCliente" 
        DataSourceID="SqlDataSource1" 
        ForeColor="#333333" 
        GridLines="None" 
        OnSelectedIndexChanged="grdClientes_SelectedIndexChanged" 
        HorizontalAlign="Center" 
        CssClass="gridview-row"
        Width="600px">

        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
            <asp:CommandField ShowSelectButton="True" ButtonType="Button" />
            <asp:BoundField DataField="IdCliente" HeaderText="Id Cliente" ReadOnly="True" SortExpression="IdCliente" />
            <asp:BoundField DataField="NomCli" HeaderText="Nombre" SortExpression="NomCli" />
            <asp:BoundField DataField="PobCli" HeaderText="Población" SortExpression="PobCli" />
            <asp:BoundField DataField="CorCli" HeaderText="Correo Electrónico" SortExpression="CorCli" />
        </Columns>

        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerSettings FirstPageText="Primera" LastPageText="Última" Mode="NextPreviousFirstLast" NextPageText="Siguiente" PreviousPageText="Anterior" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <SelectedRowStyle CssClass="gridview-selected-row" BackColor="#E2DED6" ForeColor="#333333" />
        <SortedAscendingCellStyle BackColor="#E9E7E2" />
        <SortedAscendingHeaderStyle BackColor="#506C8C" />
        <SortedDescendingCellStyle BackColor="#FFFDF8" />
        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
    <br /> 

    <asp:Label ID="lblMensajes" ForeColor="red" runat="server" CssClass="mensajes"></asp:Label> 

    <div id="resultados"> 
        <asp:Label ID="lblResultado" runat="server"></asp:Label> 
        <br />     
    </div>
    <div id="total"> 
        <asp:Label ID="lblTotal" runat="server" Visible="false"></asp:Label> 
        <br /> 
        <br />
    </div>


</asp:Content>
