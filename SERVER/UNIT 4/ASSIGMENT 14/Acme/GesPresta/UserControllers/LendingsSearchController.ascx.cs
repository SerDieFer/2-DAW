using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta.UserControllers
{
    public partial class LendingsSearchController : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public String Code
        {
            get
            {
                return lstLendings.SelectedValue;
            }

        }
        public String Description
        {
            get
            {
                return lstLendings.SelectedItem.Text;
            }

        }
    }
}