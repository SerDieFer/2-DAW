using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta
{
    public partial class LendingsDataValuesResponse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Acceso directo por ID a los valores de la colección Form del objeto Request
            string lendingString = "";
            lendingString += "<h2>VALORES RECIBIDOS DESDE EL FORMULARIO LENDINGSDATAVALUESEXAMPLE</h2>";
            lendingString += "<div id=lblFormData>";
            lendingString += "<label id=lendingCodeData>Código: </label>" + "<label id=lendingCodeValue>" + Request.Form["txtbLendingCode"] + "</label>";
            lendingString += "<label id=lendingDescriptionData>Descripción: </label>" + "<label id=lendingDescriptionValue>" + Request.Form["txtbLendingDescription"] + "</label>";
            lendingString += "<label id=lendingCostData>Importe: </label>" + "<label id=lendingCostValue>" + Request.Form["txtbLendingFixedValue"] + "</label>";
            lendingString += "<label id=lendingPercentageData>Porcentaje: </label>" + "<label id=lendingPercentageValue>" + Request.Form["txtbLendingPercentage"] + "</label>";
            lendingString += "<label id=lendingTypeData>Tipo de Prestación: </label>" + "<label id=lendingTypeValue>" + Request.Form["ddlLendingType"] + "</label>";
            lendingString += "</div>";
            lblLendingData.Text = lendingString;
            lblLendingData.Visible = true;
        }

    }
}