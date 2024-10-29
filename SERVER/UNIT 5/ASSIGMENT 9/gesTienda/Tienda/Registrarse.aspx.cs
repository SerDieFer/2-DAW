using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tienda
{
    public partial class Registrarse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Lógica en el Page_Load si es necesario
        }

        // HASHEO DE CONTRASEÑAS
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private void FnRegistroUsuario()
        {
            ErrorMessage.Text = "";

            if (Password.Text == ConfirmPassword.Text)
            {
                string strLogin, strPassword, strRol, strFechaAlta;
                DateTime dtFechaAlta;
                string strIdCliente, strNomCli, strDirCli, strPobCli, strCpoCli, strTelCli, strCorCli;

                strLogin = Email.Text;
                strPassword = HashPassword(Password.Text);
                strRol = "U";

                // CARGA FECHA Y HORA DEL SISTEMA 
                dtFechaAlta = System.DateTime.Now;
                strFechaAlta = String.Format("{0:yyyy/MM/dd HH:mm:ss}", dtFechaAlta);

                strCorCli = Email.Text; // CORREO
                strIdCliente = Nif.Text; // DNI
                strNomCli = NombreRazonSocial.Text; // NOMBRE
                strDirCli = Direccion.Text; // DIRECCION
                strPobCli = Poblacion.Text; // POBLACION
                strCpoCli = CodigoPostal.Text; // CODIGO POSTAL
                strTelCli = Telefono.Text; // TELEFONO

                string StrCadenaConexion = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                string strComandoSql_1 = "INSERT INTO USUARIO(Login, Password, Rol, FechaAlta) " +
                                                     "VALUES (@Login, @Password, @Rol, @FechaAlta);";

                string strComandoSql_2 = "INSERT INTO CLIENTE (IdCliente, NomCli, DirCli, PobCli, CpoCli, TelCli, CorCli) " +
                                                      "VALUES (@IdCliente, @NomCli, @DirCli, @PobCli, @CpoCli, @TelCli, @CorCli);";

                using (SqlConnection conexion = new SqlConnection(StrCadenaConexion))
                {
                    conexion.Open();
                    using (SqlCommand comando = new SqlCommand())
                    {
                        comando.Connection = conexion;

                        SqlTransaction tran = conexion.BeginTransaction();
                        comando.Transaction = tran;
                        try
                        {
                            // PARÁMETROS DEL PRIMER COMANDO SQL
                            comando.CommandText = strComandoSql_1;
                            comando.Parameters.AddWithValue("@Login", strLogin);
                            comando.Parameters.AddWithValue("@Password", strPassword);
                            comando.Parameters.AddWithValue("@Rol", strRol);
                            comando.Parameters.AddWithValue("@FechaAlta", strFechaAlta);
                            comando.ExecuteNonQuery();

                            // LIMPIAR PARÁMETROS DEL COMANDO ANTERIOR
                            comando.Parameters.Clear();

                            // PARÁMETROS DEL SEGUNDO COMANDO SQL
                            comando.CommandText = strComandoSql_2;
                            comando.Parameters.AddWithValue("@IdCliente", strIdCliente);
                            comando.Parameters.AddWithValue("@NomCli", strNomCli);
                            comando.Parameters.AddWithValue("@DirCli", strDirCli);
                            comando.Parameters.AddWithValue("@PobCli", strPobCli);
                            comando.Parameters.AddWithValue("@CpoCli", strCpoCli);
                            comando.Parameters.AddWithValue("@TelCli", strTelCli);
                            comando.Parameters.AddWithValue("@CorCli", strCorCli);
                            comando.ExecuteNonQuery();

                            // CONFIRMAR TRANSACCIÓN
                            tran.Commit();
                            ErrorMessage.Text = "Usuario registrado correctamente";
                        }
                        catch (SqlException ex)
                        {
                            // REVERTIR TRANSACCIÓN EN CASO DE ERROR
                            tran.Rollback();
                            string StrError = "<p>Se han producido errores durante el registro</p>";
                            StrError += "<div>Código: " + ex.Number + "</div>";
                            StrError += "<div>Descripción: " + ex.Message + "</div>";
                            ErrorMessage.Text = StrError;
                        }
                        finally
                        {
                            conexion.Close();
                        }
                    }
                }
            }
            else
            {
                ErrorMessage.Text = "Se ha producido un error. Los valores de contraseña no coinciden.";
            }
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            // Verificar si la página es válida
            if (!Page.IsValid)
            {
                // Crear una lista para almacenar mensajes de error
                List<string> errorMessages = new List<string>();

                if (!EmailRequired.IsValid)
                    errorMessages.Add(EmailRequired.ErrorMessage);
                if (!emailRegex.IsValid)
                    errorMessages.Add(emailRegex.ErrorMessage);
                if (!passwordRegex.IsValid)
                    errorMessages.Add(passwordRegex.ErrorMessage);
                if (!PasswordRequired.IsValid)
                    errorMessages.Add(PasswordRequired.ErrorMessage);
                if (!PasswordCompare.IsValid)
                    errorMessages.Add(PasswordCompare.ErrorMessage);

                // Mostrar los mensajes en el control Literal
                ErrorMessage.Text = string.Join("<br/>", errorMessages);
                ErrorPanel.Style["display"] = "flex"; // Hacer visible el panel si hay errores
            }
            else
            {
                // Lógica para el registro de usuario
                // Aquí puedes llamar a tu método para registrar al usuario
                FnRegistroUsuario();
                ErrorPanel.Style["display"] = "none"; // Ocultar el panel si la operación es exitosa
            }
        }
    }
}