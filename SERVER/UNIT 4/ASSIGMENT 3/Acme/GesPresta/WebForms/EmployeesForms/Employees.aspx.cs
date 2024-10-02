using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta
{
    public partial class Employees : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("Evento", "Entrando en Page_Load");
            txtbEmployeeCode.Focus(); // SITÚA EL FOCO EN EL ELEMENTO CÓDIGO EMPLEADO
            txtbEmployeeNIF.Text = "11111111X"; // ESTABLECE UN VALOR POR DEFECTO PARA EL CAMPO
            Trace.Warn("Asignación", "Cambiado el valor de txtbEmployeeNIF a: " + txtbEmployeeNIF.Text);
            Trace.Write("Evento", "Saliendo de Page_Load");
        }
    }
}