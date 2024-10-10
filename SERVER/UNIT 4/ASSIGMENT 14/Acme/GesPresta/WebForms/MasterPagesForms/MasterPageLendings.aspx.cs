using GesPresta.UserControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta.WebForms
{
    public partial class MasterPageLendings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtbLendingCode.Focus();
        }

        protected void btnSeeLendings_Click(object sender, EventArgs e)
        {
            if (btnSelect.Visible == false)
            {
                btnSelect.Visible = true;
                LendingsSearchController1.Visible = true;
                btnSeeLendings.Text = "Ocultar prestaciones";
            }
            else
            {
                btnSelect.Visible = false;
                LendingsSearchController1.Visible = false;
                btnSeeLendings.Text = "Ver prestaciones";
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            txtbLendingCode.Text = LendingsSearchController1.Code;
            txtbLendingDescription.Text = LendingsSearchController1.Description;
        }
    }
}
