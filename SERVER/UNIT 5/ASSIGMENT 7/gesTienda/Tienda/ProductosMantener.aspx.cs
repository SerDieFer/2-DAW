using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tienda
{
    public partial class ProductosMantener : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void FnDeshabilitarControles()
        {
            txtbIdProducto.Enabled = false;
            txtbDescProducto.Enabled = false;
            txtbPreProducto.Enabled = false;
            ddlUnidadProducto.Enabled = false;
            ddlTipoProducto.Enabled = false;
        }

        protected void FnHabilitarControles()
        {
            txtbIdProducto.Enabled = true;
            txtbDescProducto.Enabled = true;
            txtbPreProducto.Enabled = true;
            ddlUnidadProducto.Enabled = true;
            ddlTipoProducto.Enabled = true;
        }

        protected string FnComaPorPunto(decimal Numero)
        {
            string StrNumero = Convert.ToString(Numero);
            string stNumeroConPunto = String.Format("{0}", StrNumero.Replace(',', '.'));
            return (stNumeroConPunto);
        }


        protected void grdProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensajes.Text = "";
            FnDeshabilitarControles();
            string StrIdProducto = grdProductos.SelectedRow.Cells[1].Text;
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA SELECCIONAR EL REGISTRO
            string StrComandoSql = "SELECT IdProducto, " +
                                   "       DesPro, " +
                                   "       PrePro, " +
                                   "       IdUnidad, " +
                                   "       PRODUCTO.IdTipo, " +
                                   "       DesTip " +
                                   "  FROM PRODUCTO INNER JOIN TIPO ON PRODUCTO.IdTipo = TIPO.IdTipo " +
                                   " WHERE PRODUCTO.IdProducto = @IdProducto;";

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    conexion.Open();
                    SqlCommand comando = conexion.CreateCommand();
                    comando.CommandText = StrComandoSql;

                    // AGREGAR EL PARÁMETRO
                    comando.Parameters.AddWithValue("@IdProducto", StrIdProducto);

                    SqlDataReader reader = comando.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        txtbIdProducto.Text = reader["IdProducto"].ToString();
                        txtbDescProducto.Text = reader["DesPro"].ToString();
                        txtbPreProducto.Text = string.Format("{0:c}", reader.GetDecimal(2));
                        ddlUnidadProducto.SelectedItem.Selected = false;
                        ddlUnidadProducto.SelectedItem.Text = reader["IdUnidad"].ToString();
                        ddlTipoProducto.SelectedItem.Selected = false;
                        ddlTipoProducto.SelectedItem.Text = reader["DesTip"].ToString();
                    }
                    else
                    {
                        lblMensajes.Text = "No existen registros resultantes de la consulta";
                    }
                    reader.Close();
                    btnNuevo.Visible = true;
                    btnEditar.Visible = true;
                    btnEliminar.Visible = true;
                    btnInsertar.Visible = false;
                    btnModificar.Visible = false;
                    btnBorrar.Visible = false;
                    btnCancelar.Visible = false;
                }
                catch (SqlException ex)
                {
                    string StrError = "<p>Se han producido errores en el acceso a la base de datos.</p>";
                    StrError = StrError + "<div>Código: " + ex.Number + "</div>";
                    StrError = StrError + "<div>Descripción: " + ex.Message + "</div>";
                    lblMensajes.Text = StrError;
                    return;
                }
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            lblMensajes.Text = "";
            btnNuevo.Visible = false;
            btnEditar.Visible = false;
            btnEliminar.Visible = false;
            btnInsertar.Visible = true;
            btnModificar.Visible = false;
            btnBorrar.Visible = false;
            btnCancelar.Visible = true;
            txtbIdProducto.Text = "";
            txtbDescProducto.Text = "";
            txtbPreProducto.Text = Convert.ToString(0);

            // VUELVE A ENLAZAR EL CONTROL PARA QUE SE ACTUALICEN LOS DATOS 
            ddlUnidadProducto.DataBind();

            ddlTipoProducto.DataBind();
            grdProductos.SelectedIndex = -1;
            FnHabilitarControles();
            txtbIdProducto.Focus();
        }

        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            lblMensajes.Text = "";
            String strIdProducto, strDescripcion, strIdUnidad, strIdTipo;
            Decimal dcPrecio;

            strIdProducto = txtbIdProducto.Text;
            strDescripcion = txtbDescProducto.Text;
            dcPrecio = Convert.ToDecimal(txtbPreProducto.Text);
            strIdUnidad = ddlUnidadProducto.SelectedItem.Text;
            strIdTipo = ddlTipoProducto.SelectedItem.Value;

            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA ACTUALIZAR EL REGISTRO
            string StrComandoSql = "INSERT INTO PRODUCTO " +
                                   "(IdProducto, DesPro, PrePro, IdUnidad, IdTipo) " +
                                   "VALUES (@IdProducto, @Descripcion, @Precio, @IdUnidad, @IdTipo)";

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    conexion.Open();
                    SqlCommand comando = conexion.CreateCommand();

                    // AGREGAR LOS PARÁMETROS PARA EVITAR INYECCIÓN SQL
                    comando.Parameters.AddWithValue("@IdProducto", strIdProducto);
                    comando.Parameters.AddWithValue("@Descripcion", strDescripcion);
                    comando.Parameters.AddWithValue("@Precio", dcPrecio);
                    comando.Parameters.AddWithValue("@Unidad", strIdUnidad);
                    comando.Parameters.AddWithValue("@Tipo", strIdTipo);

                    comando.CommandText = StrComandoSql;
                    int inRegistrosAfectados = comando.ExecuteNonQuery();

                    if (inRegistrosAfectados == 1)
                        lblMensajes.Text = "Registro insertado correctamente";
                    else
                        lblMensajes.Text = "Error al insertar el registro";
                        btnNuevo.Visible = true;
                        btnEditar.Visible = false;
                        btnEliminar.Visible = false;
                        btnInsertar.Visible = false;
                        btnModificar.Visible = false;
                        btnBorrar.Visible = false;
                        btnCancelar.Visible = false;
                }
                catch (SqlException ex)
                {
                    string StrError = "<p>Se han producido errores en el acceso a la base de datos.</p>";
                    StrError = StrError + "<div>Código: " + ex.Number + "</div>";
                    StrError = StrError + "<div>Descripción: " + ex.Message + "</div>";
                    lblMensajes.Text = StrError;
                    return;
                }       
            }
            // VUELVE A ENLAZAR EL GRIDVIEW PARA QUE SE ACTUALICEN LOS DATOS 
            grdProductos.DataBind();
            grdProductos.SelectedIndex = -1;
            FnDeshabilitarControles();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            lblMensajes.Text = "";
            btnNuevo.Visible = true;
            btnEditar.Visible = false;
            btnEliminar.Visible = false;
            btnInsertar.Visible = false;
            btnModificar.Visible = false;
            btnBorrar.Visible = false;
            btnCancelar.Visible = false;
            txtbIdProducto.Text = "";
            txtbDescProducto.Text = "";
            txtbPreProducto.Text = Convert.ToString(0);
            ddlUnidadProducto.DataBind();
            ddlTipoProducto.DataBind();
            grdProductos.SelectedIndex = -1;
            FnDeshabilitarControles();
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            lblMensajes.Text = "";
            txtbDescProducto.Enabled = true;
            txtbPreProducto.Enabled = true;
            ddlUnidadProducto.Enabled = true;
            ddlTipoProducto.Enabled = true;
            txtbIdProducto.Enabled = false;
            btnModificar.Visible = true;
            btnCancelar.Visible = true;
            btnNuevo.Visible = false;
            btnEditar.Visible = false;
            btnEliminar.Visible = false;
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            lblMensajes.Text = "";

            // OBTENER LOS VALORES DE LOS CONTROLES
            string strIdProducto = txtbIdProducto.Text;
            string strDescripcion = txtbDescProducto.Text;

            string precioTexto = txtbPreProducto.Text.Trim();
            if (precioTexto.EndsWith("€"))
            {
                precioTexto = precioTexto.Remove(precioTexto.Length - 1);
            }
            decimal dcPrecio = Convert.ToDecimal(precioTexto);


            string strIdUnidad = ddlUnidadProducto.SelectedItem.Text;
            string strIdTipo = ddlTipoProducto.SelectedItem.Value;

            // CADENA DE CONEXIÓN
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA ACTUALIZAR EL REGISTRO
            string StrComandoSql = "UPDATE PRODUCTO SET " +
                                   "DesPro = @Descripcion, " +
                                   "PrePro = @Precio, " +
                                   "IdUnidad = @IdUnidad, " +
                                   "IdTipo = @IdTipo " +
                                   "WHERE IdProducto = @IdProducto";

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand(StrComandoSql, conexion);

                    // AGREGAR LOS PARÁMETROS PARA EVITAR INYECCIÓN SQL
                    comando.Parameters.AddWithValue("@Descripcion", strDescripcion);
                    comando.Parameters.AddWithValue("@Precio", dcPrecio);
                    comando.Parameters.AddWithValue("@IdUnidad", strIdUnidad);
                    comando.Parameters.AddWithValue("@IdTipo", strIdTipo);
                    comando.Parameters.AddWithValue("@IdProducto", strIdProducto);

                    // EJECUTAR EL COMANDO
                    int registrosAfectados = comando.ExecuteNonQuery();

                    if (registrosAfectados == 1)
                        lblMensajes.Text = "Registro actualizado correctamente";
                    else
                        lblMensajes.Text = "Error al actualizar el registro";

                    // ACTUALIZAR LOS CONTROLES Y EL GRIDVIEW
                    btnNuevo.Visible = true;
                    btnEditar.Visible = false;
                    btnEliminar.Visible = false;
                    btnInsertar.Visible = false;
                    btnModificar.Visible = false;
                    btnCancelar.Visible = false;
                    FnDeshabilitarControles();
                    grdProductos.DataBind();
                }
                catch (SqlException ex)
                {
                    lblMensajes.Text = $"Error: {ex.Message}";
                }
            }
        }

        protected void btnBorrar_Click(object sender, EventArgs e)
        {
            // OBTENER EL ID DEL PRODUCTO DE LA FILA SELECCIONADA
            string strIdProducto = grdProductos.SelectedRow.Cells[1].Text;

            // CADENA DE CONEXIÓN
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA ACTUALIZAR EL REGISTRO
            string StrComandoSql = "DELETE FROM PRODUCTOS " +
                                   "WHERE IdProducto = @IdProducto";

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    conexion.Open();
                    SqlCommand comando = new SqlCommand(StrComandoSql, conexion);

                    // AGREGAR LOS PARÁMETROS PARA EVITAR INYECCIÓN SQL
                    comando.Parameters.AddWithValue("@IdProducto", strIdProducto);

                    // EJECUTAR EL COMANDO
                    int registrosAfectados = comando.ExecuteNonQuery();

                    if (registrosAfectados == 1)
                        lblMensajes.Text = "Borrado actualizado correctamente";
                    else
                        lblMensajes.Text = "No se encontró el registro o no se pudo borrar.";
                }
                catch (SqlException ex)
                {
                    lblMensajes.Text = $"Error: {ex.Message}";
                }

                // VUELVE A ENLAZAR EL GRIDVIEW PARA QUE SE ACTUALICEN LOS DATOS 
                grdProductos.DataBind();
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {

            lblMensajes.Text = "";

            btnCancelar.Visible = true;
            btnBorrar.Visible = true;
            btnEliminar.Visible = false;
            btnNuevo.Visible = false;
            btnEditar.Visible = false;
            btnInsertar.Visible = false;
            btnModificar.Visible = false;
        }
    }
}