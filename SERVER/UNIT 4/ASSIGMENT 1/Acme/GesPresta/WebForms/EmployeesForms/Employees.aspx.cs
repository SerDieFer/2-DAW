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
            txtbEmployeeCode.Focus();
            //txtbEmployeeNIF.Text = 11111111.ToString();
            //int a = 3; // Produce un error en tiempo de ejecución
            //int b = 0;
            //int c = a / b;
        }
    }
}