using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
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

        // MÉTODO PARA CONFIGURAR LA VISIBILIDAD DE LOS BOTONES
        private void FnConfigureButtonsVisibility(bool? all = null, bool cancelar = false, 
                                                  bool eliminar = false, bool nuevo = false, 
                                                  bool editar = false, bool insertar = false, 
                                                  bool modificar = false, bool borrar = false)
        {
            if (all == null)
            {
                btnCancelar.Visible = cancelar;
                btnBorrar.Visible = borrar;
                btnEliminar.Visible = eliminar;
                btnNuevo.Visible = nuevo;
                btnEditar.Visible = editar;
                btnInsertar.Visible = insertar;
                btnModificar.Visible = modificar;
            }
            else
            {
                btnCancelar.Visible = (bool)all;
                btnBorrar.Visible = (bool)all;
                btnEliminar.Visible = (bool)all;
                btnNuevo.Visible = (bool)all;
                btnEditar.Visible = (bool)all;
                btnInsertar.Visible = (bool)all;
                btnModificar.Visible = (bool)all;
            }
        }

        // MÉTODO PARA CONFIGURAR LA VISIBILIDAD DE LOS TEXTBOXES/DDL(INPUTS)
        private void FnConfigureInputStatus(bool? all = null, bool id = false, 
                                            bool pre = false, bool tipo = false,
                                            bool unidad = false, bool desc = false)
        {
            if (all == null)
            {
                txtbIdProducto.Enabled = id;
                txtbDescProducto.Enabled = desc;
                txtbPreProducto.Enabled = pre;
                ddlTipoProducto.Enabled = tipo;
                ddlUnidadProducto.Enabled = unidad;
            }
            else
            {
                txtbIdProducto.Enabled = (bool)all;
                txtbDescProducto.Enabled = (bool)all;
                txtbPreProducto.Enabled = (bool)all;
                ddlTipoProducto.Enabled = (bool)all;
                ddlUnidadProducto.Enabled = (bool)all;
            }
        }

        // MÉTODO PARA DEJAR LOS INPUTS DE TIPO TXTBOX CON VALORES POR DEFAULT VACÍOS
        private void FnResetInputsTextAndValues()
        {
            txtbIdProducto.Text = "";
            txtbDescProducto.Text = "";
            txtbPreProducto.Text = Convert.ToString(0);

            // VUELVE A ENLAZARLOS CONTROLES PARA QUE SE ACTUALICEN LOS DATOS 
            ddlUnidadProducto.DataBind();
            ddlTipoProducto.DataBind();
        }

        // EVENTO QUE OCURRIRÁ AL SELECCIONAR DISTINTO ELEMENTO (REGISTRO) EN LA TABLA
        protected void grdProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // DESACTIVAMOS LA EDICIÓN DE TODOS LOS INPUTS
            FnConfigureInputStatus(all: false);

            // SELECCIONAMOS EL ID RELACIONADO A LA FILA SELECCIONADA
            string StrIdProducto = grdProductos.SelectedRow.Cells[1].Text;

            // ASIGNAMOS LA CADENA DE CONEXIÓN A USAR A POSTERIORI
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA SELECCIONAR EL REGISTRO, CON PARAMETERS PARA EVITAR SQL-INJECTION
            string StrComandoSql = "SELECT IdProducto, " +
                                   "       DesPro, " +
                                   "       PrePro, " +
                                   "       IdUnidad, " +
                                   "       PRODUCTO.IdTipo, " +
                                   "       DesTip " +
                                   "  FROM PRODUCTO INNER JOIN TIPO ON PRODUCTO.IdTipo = TIPO.IdTipo " +
                                   " WHERE PRODUCTO.IdProducto = @IdProducto;";

            // COMANDO USING QUE CREARÁ UN NUEVO OBJETO DE CONEXION CON LA CADENA DE CONEXIÓN SELECCIONADA
            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    // ABRIMOS ESTA CONEXIÓN
                    conexion.Open();

                    // CREAMOS UN COMANDO (SCRIPT A EJECUTAR) PARA ESTA CONEXIÓN
                    using (SqlCommand comando = new SqlCommand(StrComandoSql, conexion))
                    {

                        // AGREGAMOS EL PARÁMETRO
                        comando.Parameters.AddWithValue("@IdProducto", StrIdProducto);

                        // INICIAMOS LA LECTURA CON EL READER
                        SqlDataReader reader = comando.ExecuteReader();

                        // SI EL READER ENCUENTRA FILAS
                        if (reader.HasRows)
                        {
                            // PROCEDE A LEER CELDA POR CELDA
                            reader.Read();

                            // ASIGNAMOS LOS VALORES A CADA TIPO
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
                            // LA LECTURA HA SIDO ERRONEA Y SIN RESULTADOS, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "No existen registros resultantes de la consulta";
                        }

                        // SE CIERRA EL LECTOR
                        reader.Close();

                        // PERMITIMOS VER LOS BOTONES DE NUEVO, EDITAR Y ELIMINAR, EL RESTO LOS OCULTAMOS
                        FnConfigureButtonsVisibility(eliminar: true, nuevo: true, editar: true);
                    }
                }
                catch (SqlException ex)
                {
                    // MANEJAMOS LOS ERRORES MEDIANTE UN CÓDIGO Y UNA EXCEPCIÓN, QUE SE APLICARÁ EN LOS MENSAJES A MOSTRAR
                    string StrError = "<p>Se han producido errores en el acceso a la base de datos.</p>";
                    StrError = StrError + "<div>Código: " + ex.Number + "</div>";
                    StrError = StrError + "<div>Descripción: " + ex.Message + "</div>";
                    lblMensajes.Text = StrError;
                    return;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------------- //

        // MÉTODO QUE RESETEA EL ESTADO AL ESTADO INICIAL AL QUERER CANCELAR EL USUARIO LA ACCIÓN
        private void FnCancelar()
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // PERMITIMOS VER SOLO EL BOTÓN DE NUEVO, EL RESTO LOS OCULTAMOS
            FnConfigureButtonsVisibility(nuevo: true);

            // DEJAMOS LOS TEXTOS Y VALORES DE ESTOS INPUTS COMO DEFAULT Y VACÍOS
            FnResetInputsTextAndValues();

            // RESETEO DEL INDEX SELECCIONADO A -1, PARA VOLVER ATRÁS
            grdProductos.SelectedIndex = -1;

            // DEJAMOS COMO NO EDITABLE TODOS LOS INPUT
            FnConfigureInputStatus();

        }

        // CLICK DE CANCELADO QUE EJECUTARÁ LA ACCIÓN DE CANCELAR
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FnCancelar();
        }

        // ---------------------------------------------------------------------------------------------------------------- //

        // ---------------------------------------------------------------------------------------------------------------- //

        // BOTON DE OPCIÓN QUE NOS PERMITE ACCEDER AL BOTÓN DE INSERTAR UN REGISTRO
        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // VISIBILIDAD DE LOS BOTONES DESACTIVADA, A EXCEPCION DE CANCELAR E INSERTAR
            FnConfigureButtonsVisibility(cancelar: true, insertar: true);

            // RESET DEL TEXTO DE LOS INPUTS
            FnResetInputsTextAndValues();

            // RESETEO DEL INDEX SELECCIONADO A -1, PARA VOLVER ATRÁS
            grdProductos.SelectedIndex = -1;

            // PERMITE EDITAR TODOS LOS INPUTS PARA AÑADIR UN NUEVO ELEMENTO
            FnConfigureInputStatus(true);

            // INICIA SELECCIONANDO EL ID
            txtbIdProducto.Focus();
        }

        // FUNCION QUE CONVIERTE UN VALOR DECIMAL A UNA CADENA STRING DETERMINADA
        // CAMBIA COMA POR PUNTO
        protected string FnCommaForPoint(decimal num)
        {
            string StrNumero = Convert.ToString(num);
            string stNumeroConPunto = String.Format("{0}", StrNumero.Replace(',', '.'));
            return (stNumeroConPunto);
        }

        // FUNCION QUE CONVIERTE UN VALOR STRING A OTRA CADENA STRING DETERMINADA
        // CAMBIA PUNTO POR COMA
        protected string FnPointForComma(string num)
        {
            string stNumeroConComa = String.Format("{0}", num.Replace('.', ','));
            return (stNumeroConComa);
        }

        // MÉTODO CRUD DE CREACIÓN, CON SQL PARAMETERS PARA EVITAR SQL-INJECTION
        private void FnInsertar()
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // VARIABLES CON LAS QUE TRABAJAREMOS PARA INSERTAR EN EL REGISTRO DATOS
            String strIdProducto, strDescripcion, strIdUnidad, strIdTipo;
            Decimal dcPrecio;

            strIdProducto = txtbIdProducto.Text;
            strDescripcion = txtbDescProducto.Text;
            strIdUnidad = ddlUnidadProducto.SelectedItem.Text;
            strIdTipo = ddlTipoProducto.SelectedItem.Value;

            //// CONVERTIMOS EL TEXTO EN UN FORMATO DETERMINADO Y LUEGO CONVERTIMOS ESTE PARA QUE EL PRECIO SEA UN DECIMAL
            dcPrecio = Convert.ToDecimal(FnPointForComma(txtbPreProducto.Text));

            // ASIGNAMOS LA CADENA DE CONEXIÓN A USAR A POSTERIORI
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA INSERTAR EN EL REGISTRO, USANDO SQL PARAMETERS
            string StrComandoSql = "INSERT INTO PRODUCTO " +
                                   "(IdProducto, DesPro, PrePro, IdUnidad, IdTipo) " +
                                   "VALUES (@IdProducto, @Descripcion, @Precio, @IdUnidad, @IdTipo)";

            // COMANDO USING QUE CREARÁ UN NUEVO OBJETO DE CONEXION CON LA CADENA DE CONEXIÓN SELECCIONADA
            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    // ABRIMOS ESTA CONEXIÓN
                    conexion.Open();

                    // CREAMOS UN COMANDO (SCRIPT A EJECUTAR) PARA ESTA CONEXIÓN
                    using (SqlCommand comando = new SqlCommand(StrComandoSql, conexion))
                    {

                        // AGREGAR LOS PARÁMETROS AL COMANDO PARA EVITAR INYECCIÓN SQL
                        comando.Parameters.AddWithValue("@IdProducto", strIdProducto);
                        comando.Parameters.AddWithValue("@Descripcion", strDescripcion);
                        comando.Parameters.AddWithValue("@IdUnidad", strIdUnidad);
                        comando.Parameters.AddWithValue("@IdTipo", strIdTipo);

                        // CON LA FUNCION FNCOMAPUNTO, HACEMOS LA RE-CONVERSION CONCRETA DEL DECIMAL PARA INTRODUCIRLO A SQL COMO STRING
                        comando.Parameters.AddWithValue("@Precio", FnCommaForPoint(dcPrecio));

                        // EJECUTAMOS EL PROCESO
                        int inRegistrosAfectados = comando.ExecuteNonQuery();

                        // SI EXISTE UN REGISTRO AFECTADO
                        if (inRegistrosAfectados == 1)
                        {
                            // EL REGISTRO HA SIDO EXITOSO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "Registro insertado correctamente";
                        }
                        else
                        {
                            // EL REGISTRO HA SIDO ERRONEO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "Error al insertar el registro";

                            // VISIBILIDAD DE LOS BOTONES DESACTIVADA, A EXCEPCION DE NUEVO
                            FnConfigureButtonsVisibility(nuevo: true);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // MANEJAMOS LOS ERRORES MEDIANTE UN CÓDIGO Y UNA EXCEPCIÓN, QUE SE APLICARÁ EN LOS MENSAJES A MOSTRAR
                    string StrError = "<p>Se han producido errores en el acceso a la base de datos.</p>";
                    StrError = StrError + "<div>Código: " + ex.Number + "</div>";
                    StrError = StrError + "<div>Descripción: " + ex.Message + "</div>";
                    lblMensajes.Text = StrError;
                    return;
                }
            }

            // VUELVE A ENLAZAR EL GRIDVIEW PARA QUE SE ACTUALICEN LOS DATOS 
            grdProductos.DataBind();

            // RESETEO DEL INDEX SELECCIONADO A -1, PARA VOLVER AL ESTADO INICIAL
            grdProductos.SelectedIndex = -1;

            // NO PERMITIMOS EDITAR NINGÚN INPUT
            FnConfigureInputStatus(all: false);

            // DESACTIVAMOS TODOS LOS INPUTS Y LOS RESETEAMOS
            FnConfigureInputStatus(all: false);
            FnResetInputsTextAndValues();

            // VISIBILIDAD DE LOS BOTONES DESACTIVADA, A EXCEPCION DE NUEVO
            FnConfigureButtonsVisibility(nuevo:true);
        }

        // CLICK DE INSERTADO QUE EJECUTARÁ LA ACCIÓN DE INSERTAR
        protected void btnInsertar_Click(object sender, EventArgs e)
        {
            FnInsertar();
        }

        // ---------------------------------------------------------------------------------------------------------------- //

        // ---------------------------------------------------------------------------------------------------------------- //

        // BOTON DE OPCIÓN QUE NOS PERMITE ACCEDER AL BOTÓN DE MODIFICAR UN REGISTRO
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // PERMITIMOS VER LOS BOTONES DE MODIFICADO Y CANCELADO, EL RESTO LOS OCULTAMOS
            FnConfigureButtonsVisibility(modificar: true, cancelar: true);

            // PERMITIMOS USAR TODOS LOS INPUTS MENOS EL DE ID.
            FnConfigureInputStatus(pre:true, tipo:true, unidad:true, desc:true);

        }

        // QUITA SÍMBOLO DE EURO
        protected string FnNoCurrencySymbol(string num)
        {
            string stNumEuro = String.Format("{0}", num.Replace("€", ""));
            stNumEuro = stNumEuro.Trim();
            return stNumEuro;
        }


        // MÉTODO CRUD DE MODIFICADO(UPDATE), CON SQL PARAMETERS PARA EVITAR SQL-INJECTION
        private void FnModificar()
        {
            // RESET DEL TEXTO DE LAS ACCIONES PREVIAS
            lblMensajes.Text = "";

            // OBTENER LOS VALORES DE LOS CONTROLES
            string strIdProducto = txtbIdProducto.Text;
            string strDescripcion = txtbDescProducto.Text;
            string strIdUnidad = ddlUnidadProducto.SelectedItem.Text;
            string strIdTipo = ddlTipoProducto.SelectedItem.Value;

            // CREAMOS UN STRING DEL TEXTO EN ESTE INPUT Y ELIMINAMOS POSIBLES VALORES NO NECESARIOS
            string precioTexto = FnNoCurrencySymbol(txtbPreProducto.Text);
            decimal dcPrecio;

            if(!decimal.TryParse(FnPointForComma(precioTexto), out decimal preComprobado))
            {
                lblMensajes.Text = "ERROR - Usar solamente formato numérico (1,5)";
                return;
            }

            dcPrecio = preComprobado;

            // ASIGNAMOS LA CADENA DE CONEXIÓN A USAR A POSTERIORI
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA ACTUALIZAR EL REGISTRO, USANDO SQL PARAMETERS
            string StrComandoSql = "UPDATE PRODUCTO SET " +
                                   "DesPro = @Descripcion, " +
                                   "PrePro = @Precio, " +
                                   "IdUnidad = @IdUnidad, " +
                                   "IdTipo = @IdTipo " +
                                   "WHERE IdProducto = @IdProducto";

            // COMANDO USING QUE CREARÁ UN NUEVO OBJETO DE CONEXION CON LA CADENA DE CONEXIÓN SELECCIONADA
            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    // ABRIMOS ESTA CONEXIÓN
                    conexion.Open();

                    // CREAMOS UN COMANDO (SCRIPT A EJECUTAR) PARA ESTA CONEXIÓN
                    using (SqlCommand comando = new SqlCommand(StrComandoSql, conexion))
                    { 

                        // AGREGAMOS LOS PARÁMETROS PARA EVITAR INYECCIÓN SQL
                        comando.Parameters.AddWithValue("@Descripcion", strDescripcion);
                        comando.Parameters.AddWithValue("@Precio", dcPrecio);
                        comando.Parameters.AddWithValue("@IdUnidad", strIdUnidad);
                        comando.Parameters.AddWithValue("@IdTipo", strIdTipo);
                        comando.Parameters.AddWithValue("@IdProducto", strIdProducto);

                        // EJECUTAMOS EL PROCESO
                        int registrosAfectados = comando.ExecuteNonQuery();

                        // SI SOLO AFECTA A UN REGISTRO
                        if (registrosAfectados == 1)
                        {
                            // EL REGISTRO HA SIDO EXITOSO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "Registro actualizado correctamente";
                        }
                        else
                        {
                            // EL REGISTRO HA SIDO ERRONEO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "Error al actualizar el registro";
                        }

                        // PERMITIMOS VER EL BOTON DE NUEVO, EL RESTO LOS OCULTAMOS
                        FnConfigureButtonsVisibility(nuevo:true);

                        // DESACTIVAMOS TODOS LOS INPUTS Y LOS RESETEAMOS
                        FnConfigureInputStatus(all: false);
                        FnResetInputsTextAndValues();

                        // ACTUALIZAMOS LOS DATOS DE LA TABLA
                        grdProductos.DataBind();
                    }
                }
                catch (SqlException ex)
                {
                    // MANEJAMOS LOS ERRORES MEDIANTE UNA EXCEPCIÓN, QUE SE APLICARÁ EN LOS MENSAJES A MOSTRAR
                    lblMensajes.Text = $"Error: {ex.Message}";
                }
            }
        }

        // CLICK DE MODIFICADO QUE EJECUTARÁ LA ACCIÓN DE ACTUALIZAR
        protected void btnModificar_Click(object sender, EventArgs e)
        {
            FnModificar();
        }

        // ---------------------------------------------------------------------------------------------------------------- //


        // ---------------------------------------------------------------------------------------------------------------- //

        // BOTON DE OPCIÓN QUE NOS PERMITE ACCEDER AL BOTÓN DE BORRAR UN REGISTRO
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            // RESETEAMOS EL MENSAJE PREVIO
            lblMensajes.Text = "";

            // PERMITIMOS VER LOS BOTONES DE BORRADO Y CANCELADO, EL RESTO LOS OCULTAMOS
            FnConfigureButtonsVisibility(borrar: true, cancelar: true);
        }

        // MÉTODO CRUD DE BORRADO, CON SQL PARAMETERS PARA EVITAR SQL-INJECTION
        private void FnBorrar()
        {
            // OBTENER EL ID DEL PRODUCTO DE LA FILA SELECCIONADA
            string strIdProducto = grdProductos.SelectedRow.Cells[1].Text;

            // CADENA DE CONEXIÓN
            string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // COMANDO SQL PARA ACTUALIZAR EL REGISTRO
            string StrComandoSql = "DELETE FROM PRODUCTO " +
                                   "WHERE IdProducto = @IdProducto";

            using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
            {
                try
                {
                    // ABRIMOS ESTA CONEXIÓN
                    conexion.Open();

                    // CREAMOS UN COMANDO (SCRIPT A EJECUTAR) PARA ESTA CONEXIÓN
                    using (SqlCommand comando = new SqlCommand(StrComandoSql, conexion))
                    { 
                        // AGREGAR LOS PARÁMETROS PARA EVITAR INYECCIÓN SQL
                        comando.Parameters.AddWithValue("@IdProducto", strIdProducto);

                        // EJECUTAMOS EL PROCESO
                        int registrosAfectados = comando.ExecuteNonQuery();

                        // SI SOLO AFECTA A UN REGISTRO
                        if (registrosAfectados == 1)
                        {
                            // ESTO EJECUTARÍA EL CÓDIGO DENTRO DE LA FUNCION CANCELAr,
                            // PARA UNA VEZ ACTUALIZADOS LOS DATOS DEJAR DATOS RESETEADOS UNA VEZ BORRADO
                            FnCancelar();

                            // EL BORRADO HA SIDO CORRECTO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "Borrado actualizado correctamente";
                        }
                        else
                        {
                            // EL BORRADO HA SIDO ERRONEO, POR LO QUE SE REFLEJA EN LOS MENSAJES
                            lblMensajes.Text = "No se encontró el registro o no se pudo borrar.";
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // MANEJAMOS LOS ERRORES MEDIANTE UNA EXCEPCIÓN, QUE SE APLICARÁ EN LOS MENSAJES A MOSTRAR
                    lblMensajes.Text = $"Error: {ex.Message}";
                }

                // VUELVE A ENLAZAR EL GRIDVIEW PARA QUE SE ACTUALICEN LOS DATOS 
                grdProductos.DataBind();
            }
        }

        // CLICK DE BORRADO QUE EJECUTARÁ LA ACCIÓN DE BORRAR
        protected void btnBorrar_Click(object sender, EventArgs e)
        {
            FnBorrar();
        }

        // ---------------------------------------------------------------------------------------------------------------- //
    }
}