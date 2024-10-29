<%@ OutputCache Duration="1" VaryByParam="None" %> 
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPageAdm.Master" AutoEventWireup="true" CodeBehind="ProductosMantener.aspx.cs" Inherits="Tienda.ProductosMantener" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="InfoContenido" runat="server">
    <div id="contenido2">
    <%-- DATOS TABLA PRODUCTOS --%>
    <asp:SqlDataSource 
        ID="SqlDataSource1" 
        runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        SelectCommand="SELECT [IdProducto], [DesPro], [PrePro] FROM [PRODUCTO]">
    </asp:SqlDataSource>

    <%-- DATOS TABLA UNIDAD --%>
    <asp:SqlDataSource 
        ID="SqlDataSource2" 
        runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        SelectCommand="SELECT * FROM [UNIDAD]">
    </asp:SqlDataSource>

    
    <%-- DATOS TABLA TIPO --%>
    <asp:SqlDataSource 
        ID="SqlDataSource3" 
        runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        SelectCommand="SELECT * FROM [TIPO]">
    </asp:SqlDataSource>


    <div class="contenidotitulo">Mantenimiento productos</div>
    <br /> 
    <br /> 
    <br /> 

    <div id="contenedor">
        <div id="gridTable">
            <asp:GridView ID="grdProductos" runat="server" AutoGenerateColumns="False" DataSourceID="SqlDataSource1" AllowPaging="True" CellPadding="4" DataKeyNames="IdProducto" ForeColor="#333333" GridLines="None" HorizontalAlign="Center" OnSelectedIndexChanged="grdProductos_SelectedIndexChanged">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:CommandField ButtonType="Button" ShowSelectButton="True" />
                    <asp:BoundField DataField="IdProducto" HeaderText="Id Producto" ReadOnly="True" SortExpression="IdProducto" />
                    <asp:BoundField DataField="DesPro" HeaderText="Descripción" SortExpression="DesPro" />
                    <asp:BoundField DataField="PrePro" DataFormatString="{0:c}" HeaderText="Precio" SortExpression="PrePro">
                    <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                <PagerSettings FirstPageText="Primera" LastPageText="Última" Mode="NextPreviousFirstLast" NextPageText="Siguiente" PreviousPageText="Anterior" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#E9E7E2" />
                <SortedAscendingHeaderStyle BackColor="#506C8C" />
                <SortedDescendingCellStyle BackColor="#FFFDF8" />
                <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
            </asp:GridView>
            <br /> 
        </div>

        <div id="valoresProductos">
            <asp:Label ID="lblIdProducto" runat="server" Text="ID Producto"></asp:Label>
            <asp:TextBox ID="txtbIdProducto" runat="server" Enabled="false"></asp:TextBox>

            <asp:Label ID="lblDescProducto" runat="server" Text="Descripción"></asp:Label>
            <asp:TextBox ID="txtbDescProducto" runat="server" Enabled="false"></asp:TextBox>

            <asp:Label ID="lblPreProducto" runat="server" Text="Precio"></asp:Label>
            <asp:TextBox ID="txtbPreProducto" runat="server" Text ="0" Enabled="false"></asp:TextBox>

            <asp:Label ID="lblUnidadProducto" runat="server" Text="Unidad"></asp:Label>
            <asp:dropdownlist ID="ddlUnidadProducto" runat="server" Enabled="False" DataSourceID="SqlDataSource2" DataTextField="IdUnidad" DataValueField="IdUnidad"></asp:dropdownlist>

            <asp:Label ID="lblTipoProducto" runat="server" Text="Tipo Producto"></asp:Label>
            <asp:dropdownlist ID="ddlTipoProducto" runat="server" Enabled="False" DataSourceID="SqlDataSource3" DataTextField="DesTip" DataValueField="IdTipo"></asp:dropdownlist>

            <div id="botonesValoresProducto">
                <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" Visible="true" OnClick="btnNuevo_Click"/>
                <asp:Button ID="btnEditar" runat="server" Text="Editar" Visible="false" OnClick="btnEditar_Click"/>
                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" Visible="false" OnClick="btnEliminar_Click"/>
                <asp:Button ID="btnInsertar" runat="server" Text="Insertar" Visible="false" OnClick="btnInsertar_Click"/>
                <asp:Button ID="btnModificar" runat="server" Text="Modificar" Visible="false" OnClick="btnModificar_Click"/>
                <asp:Button ID="btnBorrar" runat="server" Text="Borrar" Visible="false" OnClick="btnBorrar_Click"/>
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" Visible="false" OnClick="btnCancelar_Click"/>
            </div>
        </div>
    </div>

    <div id="resultadosMantener"> 
        <asp:Label ID="lblMensajes" CssClass="mensajesResultado" ForeColor="red" runat="server"></asp:Label> 
        <asp:Label ID="lblResultado" runat="server"></asp:Label> 
        <br />     
    </div>
        </div>
</asp:Content>
