using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tienda
{
    public partial class PedidosPorCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int InNumeroFilas;
            string StrResultado, StrError;
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string strClienteSeleccionado = grdClientes.SelectedRow.Cells[1].Text;
            string StrComandoSql = "SELECT PEDIDO.IdPedido,FecPed,SerPed,CobPed," +
                                   "SUM(CanDet*PreDet-CanDet*PreDet*DtoDet/100) AS Total " +
                                   "FROM PEDIDO INNER JOIN DETALLE ON PEDIDO.IdPedido = DETALLE.IdPedido " +
                                   "GROUP BY PEDIDO.IdPedido, PEDIDO.FecPed, PEDIDO.CobPed, PEDIDO.SerPed, PEDIDO.IdCliente " +
                                   "HAVING  (PEDIDO.IdCliente = '" + strClienteSeleccionado + "');";
            decimal DcTotal = 0;
            lblMensajes.Text = "";
            lblResultado.Visible = false;
            lblTotal.Visible = false;

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    conexion.Open();
                    SqlCommand comando = conexion.CreateCommand();
                    comando.CommandText = StrComandoSql;
                    SqlDataReader reader = comando.ExecuteReader();
                    if (reader.HasRows)
                    {
                        InNumeroFilas = 0;
                        lblResultado.Visible = true;
                        lblTotal.Visible = true;
                        StrResultado = "<h4>Detalle de pedidos</h4>";
                        StrResultado += "<table border='1' cellspacing='0' cellpadding='4' class='table-pedidos-cliente'>";
                        StrResultado += "<tr class='table-pedidos-cliente-header'>" +
                                            "<th>Núm. Pedido</th>" +
                                            "<th>Fecha</th>" +
                                            "<th>Servido</th>" +
                                            "<th>Cobrado</th>" +
                                            "<th>Total</th>" +
                                        "</tr>";
                        while (reader.Read())
                        {
                            StrResultado += "<tr class='table-pedidos-cliente-datos'>";
                                StrResultado += "<td>" + reader.GetValue(0) + "</td>";
                                StrResultado += "<td>" + string.Format("{0:d}", reader.GetValue(1)) + "</td>";
                                StrResultado += "<td>" + (reader.GetBoolean(2) ? "Sí" : "No") + "</td>";
                                StrResultado += "<td>" + (reader.GetBoolean(3) ? "Sí" : "No") + "</td>";
                                StrResultado += "<td>" + string.Format("{0:c}", reader.GetValue(4)) + "</td>";
                            StrResultado += "</tr>";
                            InNumeroFilas++;
                        }
                        StrResultado += "</table>";

                        lblResultado.Text = StrResultado;
                        reader.Close();

                        // Total de pedidos
                        string StrComandoSql1 = "SELECT SUM(CanDet*PreDet-CanDet*PreDet*DtoDet/100) AS Total " +
                                   "FROM CLIENTE INNER JOIN PEDIDO ON CLIENTE.IdCliente = PEDIDO.IdCliente " +
                                   "INNER JOIN DETALLE ON PEDIDO.IdPedido = DETALLE.IdPedido " +
                                   "GROUP BY CLIENTE.IdCliente " +
                                   "HAVING (CLIENTE.IdCliente = '" + strClienteSeleccionado + "');";
                        using (SqlConnection conexion1 = new SqlConnection(StrCadenaConexion))
                        {
                            conexion1.Open();
                            SqlCommand comando1 = conexion1.CreateCommand();
                            comando1.CommandText = StrComandoSql1;
                            DcTotal = Convert.ToDecimal(comando1.ExecuteScalar());
                        }
                        lblTotal.Text = "<div> Número pedidos realizados: " + InNumeroFilas + "</div>" +
                                        "<div>Importe total de los pedidos realizados por el cliente: " +
                                        string.Format("{0:c}", DcTotal) + "</div>";
                    }
                    else
                    {
                        lblMensajes.Text = "No existen registros resultantes de la consulta";
                        reader.Close();
                    }
                }
                catch (SqlException ex)
                {
                    StrError = "<p>Se han producido errores en el acceso a la base de datos.</p>";
                    StrError += "<div>Código: " + ex.Number + "</div>";
                    StrError += "<div>Descripción: " + ex.Message + "</div>";
                    lblMensajes.Text = StrError;
                    return;
                }
            }
        }

        protected void grdClientes_PageIndexChanged(object sender, EventArgs e)
        {
            grdClientes.SelectedIndex = -1;
            lblResultado.Text = "";
            lblTotal.Text = "";
            lblMensajes.Text = "";
            lblResultado.Visible = false;
            lblTotal.Visible = false;
        }
    }
}