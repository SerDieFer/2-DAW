﻿using System;
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
            txtbEmployeeCode.Focus(); // SITÚA EL FOCO EN EL ELEMENTO CÓDIGO EMPLEADO
        }
    }
}