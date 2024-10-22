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

        // MÉTODO PARA CONFIGURAR LA VISIBILIDAD DE LOS BOTONES
        private void FnConfigureButtonsVisibility(bool cancelarVisible, bool borrarVisible, 
                                                  bool eliminarVisible, bool nuevoVisible, 
                                                  bool editarVisible, bool insertarVisible, 
                                                  bool modificarVisible)
        {
            btnCancelar.Visible = cancelarVisible;
            btnBorrar.Visible = borrarVisible;
            btnEliminar.Visible = eliminarVisible;
            btnNuevo.Visible = nuevoVisible;
            btnEditar.Visible = editarVisible;
            btnInsertar.Visible = insertarVisible;
            btnModificar.Visible = modificarVisible;
        }

        // MÉTODO PARA CONFIGURAR LA VISIBILIDAD DE LOS TEXTBOXES/DDL(INPUTS)
        private void FnConfigureInputStatus(bool? enabledID = null, bool? enabledDesc = null,
                                            bool? enabledPre = null, bool? enabledTipo = null,
                                            bool? enabledUnidad = null, bool? enabledAll = null)
        {
            // SI ENABLEDALL TIENE UN VALOR, ESE VALOR SE APLICARÁ A TODOS LOS INPUTS
            if (enabledAll.HasValue)
            {
                txtbIdProducto.Enabled = enabledAll.Value;
                txtbDescProducto.Enabled = enabledAll.Value;
                txtbPreProducto.Enabled = enabledAll.Value;
                ddlTipoProducto.Enabled = enabledAll.Value;
                ddlUnidadProducto.Enabled = enabledAll.Value;
                return;
            }

            // DE LO CONTRARIO, SE APLICAN LOS VALORES INDIVIDUALES SI ESTÁN DEFINIDOS
            if (enabledID.HasValue)
                txtbIdProducto.Enabled = enabledID.Value;

            if (enabledDesc.HasValue)
                txtbDescProducto.Enabled = enabledDesc.Value;

            if (enabledPre.HasValue)
                txtbPreProducto.Enabled = enabledPre.Value;

            if (enabledTipo.HasValue)
                ddlTipoProducto.Enabled = enabledTipo.Value;

            if (enabledUnidad.HasValue)
                ddlUnidadProducto.Enabled = enabledUnidad.Value;
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
            FnConfigureInputStatus(enabledAll:false);

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
                    SqlCommand comando = new SqlCommand(StrComandoSql, conexion);

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
                    FnConfigureButtonsVisibility(cancelarVisible: false, borrarVisible: false,
                                                 eliminarVisible: true, nuevoVisible: true,
                                                 editarVisible: true, insertarVisible: false,
                                                 modificarVisible: false);
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
            FnConfigureButtonsVisibility(cancelarVisible: false, borrarVisible: false,
                                         eliminarVisible: false, nuevoVisible: true,
                                         editarVisible: false, insertarVisible: false,
                                         modificarVisible: false);

            // DEJAMOS LOS TEXTOS Y VALORES DE ESTOS INPUTS COMO DEFAULT Y VACÍOS
            FnResetInputsTextAndValues();

            // RESETEO DEL INDEX SELECCIONADO A -1, PARA VOLVER ATRÁS
            grdProductos.SelectedIndex = -1;

            // DEJAMOS COMO NO EDITABLE TODOS LOS INPUT
            FnConfigureInputStatus(enabledAll: false);

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
            FnConfigureButtonsVisibility(cancelarVisible: true, borrarVisible: false,
                                         eliminarVisible: false, nuevoVisible: false,
                                         editarVisible: false, modificarVisible: false,
                                         insertarVisible: true);

            // RESET DEL TEXTO DE LOS INPUTS
            FnResetInputsTextAndValues();

            // RESETEO DEL INDEX SELECCIONADO A -1, PARA VOLVER ATRÁS
            grdProductos.SelectedIndex = -1;

            // PERMITE EDITAR TODOS LOS INPUTS PARA AÑADIR UN NUEVO ELEMENTO
            FnConfigureInputStatus(enabledAll:true);

            // INICIA SELECCIONANDO EL ID
            txtbIdProducto.Focus();
        }

        // TODO ESTO PARA FIXEAR LAS COMAS Y PUNTOS!!!!!!!!!!!
        protected string FnComaPorPunto(decimal Numero)
        {
            string StrNumero = Convert.ToString(Numero);
            string stNumeroConPunto = String.Format("{0}", StrNumero.Replace(',', '.'));
            return (stNumeroConPunto)
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

            // CONVERTIMOS Y LUEGO VERIFICAMOS QUE EL PRECIO SEA UN DECIMAL
            if (!decimal.TryParse(txtbPreProducto.Text.Replace(',', '.'), out dcPrecio))
            {
                lblMensajes.Text = "Formato de precio inválido. Por favor, usa un número válido.";
                return;
            }

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
                        comando.Parameters.AddWithValue("@Precio", FnComaPorPunto(dcPrecio));
                        comando.Parameters.AddWithValue("@IdUnidad", strIdUnidad);
                        comando.Parameters.AddWithValue("@IdTipo", strIdTipo);

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
                            FnConfigureButtonsVisibility(cancelarVisible: true, borrarVisible: false,
                                                         eliminarVisible: false, nuevoVisible: true,
                                                         editarVisible: false, modificarVisible: false,
                                                         insertarVisible: true);
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
            FnConfigureInputStatus(enabledAll: false);
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
            FnConfigureButtonsVisibility(cancelarVisible: true, borrarVisible: false,
                                         eliminarVisible: false, nuevoVisible: false,
                                         editarVisible: false, insertarVisible: false,
                                         modificarVisible: true);

            // PERMITIMOS USAR TODOS LOS INPUTS MENOS EL DE ID.
            FnConfigureInputStatus(enabledID: false, enabledDesc: true,
                                   enabledPre: true, enabledTipo: true,
                                   enabledUnidad: true);

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
            string precioTexto = txtbPreProducto.Text.Trim();

            // EN CASO DE QUE EL USUARIO HAY INTRODUCIDO YA EL VALOR DEL EURO
            if (precioTexto.EndsWith("€"))
            {
                // DECIDIMOS BORRARLE ESTE, PARA HACER LA CONVERSIÓN A DECIMAL SIN PROBLEMA POR ESTE ELEMENTO STRING
                precioTexto = precioTexto.Remove(precioTexto.Length - 1);
            }

            // RESULTADO DE ESTE PROCESO YA CONVERTIDO A DECIMAL
            decimal dcPrecio = Convert.ToDecimal(precioTexto);

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
                    SqlCommand comando = new SqlCommand(StrComandoSql, conexion);

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
                    FnConfigureButtonsVisibility(cancelarVisible: false, borrarVisible: false,
                                                 eliminarVisible: false, nuevoVisible: true,
                                                 editarVisible: false, insertarVisible: false,
                                                 modificarVisible: false);

                    // DESACTIVAMOS TODOS LOS INPUTS
                    FnConfigureInputStatus(enabledAll: false);

                    // ACTUALIZAMOS LOS DATOS DE LA TABLA
                    grdProductos.DataBind();
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
            FnConfigureButtonsVisibility(cancelarVisible: true, borrarVisible: true,
                                         eliminarVisible: false, nuevoVisible: false,
                                         editarVisible: false, insertarVisible: false,
                                         modificarVisible: false);
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
                    SqlCommand comando = new SqlCommand(StrComandoSql, conexion);

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