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
                        StrResultado += "<div class='grid-container'>";
                        StrResultado += "<div class='grid-header'>Núm.Pedido</div>";
                        StrResultado += "<div class='grid-header'>Fecha</div>";
                        StrResultado += "<div class='grid-header'>Servido</div>";
                        StrResultado += "<div class='grid-header'>Cobrado</div>";
                        StrResultado += "<div class='grid-header'>Total</div>";
                        while (reader.Read())
                        {
                            StrResultado += "<div class='grid-cell'>" + reader.GetValue(0) + "</div>";
                            StrResultado += "<div class='grid-cell'>" + string.Format("{0:d}", reader.GetValue(1)) + "</div>";
                            StrResultado += "<div class='grid-cell'>" + (reader.GetBoolean(2) ? "Sí" : "No") + "</div>";
                            StrResultado += "<div class='grid-cell'>" + (reader.GetBoolean(3) ? "Sí" : "No") + "</div>";
                            StrResultado += "<div class='grid-cell'>" + string.Format("{0:c}", reader.GetValue(4)) + "</div>";
                            InNumeroFilas++;
                        }
                        StrResultado += "</div>";

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