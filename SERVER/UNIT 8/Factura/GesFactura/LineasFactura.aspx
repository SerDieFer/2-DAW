<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LineasFactura.aspx.cs" Inherits="GesFactura.LineasFactura" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server" style="align-content: center;
                                           justify-content: center;
                                           display: flex;
                                           flex-direction: column;
                                           width: 80vw;
                                           height: 80vh;
                                           margin: 0 auto;
                                           margin-top: calc((100vh - 80vh) / 2);">

      <h1 style="text-align: center;">Uso de Servicio Web - Cálculo de factura de un artículo</h1>
      <div style="margin: 1em auto; border-collapse: collapse; width: 100%; max-width: 600px; text-align: center">

        <div style="display: flex; flex-direction: column; align-items: center;">
            <div style="display: flex; flex-direction: row; justify-content: space-between; width: 300px; margin-bottom: 10px;">
                <span>Cantidad</span>
                <asp:TextBox runat="server" ID="txtCantidad" style="width: 100px;"></asp:TextBox>
            </div>

            <div style="display: flex; flex-direction: row; justify-content: space-between; width: 300px; margin-bottom: 10px;">
                <span>Precio</span>
                <asp:TextBox runat="server" ID="txtPrecio" style="width: 100px;"></asp:TextBox>
            </div>

            <div style="display: flex; flex-direction: row; justify-content: space-between; width: 300px; margin-bottom: 10px;">
                <span>Descuento (%)</span>
                <asp:TextBox runat="server" ID="txtDescuento" style="width: 100px;"></asp:TextBox>
            </div>

            <div style="display: flex; flex-direction: row; justify-content: space-between; width: 300px; margin-bottom: 10px;">
                <span>Tipo de IVA (%)</span>
                <asp:TextBox runat="server" ID="txtTipoIVA" style="width: 100px;"></asp:TextBox>
            </div>
        </div>

        <div style="margin: 20px 0;">
            <asp:Button runat="server" ID="btnEnviar" Text="Enviar" OnClick="btnEnviar_Click" style="padding: 0.5em 8em; font-size: 16px;"></asp:Button>
        </div>

        <p>Resultados de los cálculos:</p>
        <table style="margin: auto; border-collapse: collapse; width: 100%; max-width: 600px; text-align: center;">
            <thead>
                <tr>
                    <th style="border: 1px solid #ddd; padding: 1em;">Bruto</th>
                    <th style="border: 1px solid #ddd; padding: 1em;">Descuento</th>
                    <th style="border: 1px solid #ddd; padding: 1em;">Base Imponible</th>
                    <th style="border: 1px solid #ddd; padding: 1em;">IVA</th>
                    <th style="border: 1px solid #ddd; padding: 1em;">Total</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td style="padding: 1em;">
                        <asp:Label runat="server" ID="lblBruto"></asp:Label>
                    </td>
                    <td style="padding: 1em;">
                        <asp:Label runat="server" ID="lblDescuento"></asp:Label>
                    </td>
                    <td style="padding: 1em;">
                        <asp:Label runat="server" ID="lblBaseImponible"></asp:Label>
                    </td>
                    <td style="padding: 1em;">
                        <asp:Label runat="server" ID="lblIva"></asp:Label>
                    </td>
                    <td style="padding: 1em;">
                        <asp:Label runat="server" ID="lblTotal"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    </form>
</body>
</html>
