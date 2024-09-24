using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta
{
    public partial class EmployeesDataValuesExample : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtbEmployeeCode.Focus();
        }

        protected void btnSendDataEmployee_Click(object sender, EventArgs e)
        {
            /* MÉTODO NORMAL
            lblEployeeData.Visible = true;
            // HE AÑADIDO MÁS VARIABLES DE ID PARA PODER MODIFICAR MEJOR EL CSS EN CASO NECESARIO
            lblEployeeData.Text = 
            "<div id=lblFormData>" +
                "<h3>VALORES DEL FORMULARIO</h3>" +
                "<label id=employeeCodeData> Código Empleado: </label>" + "<label id=employeeCodeValue>" + txtbEmployeeCode.Text + "</label>" +
                "<label id=employeeNIFData> NIF: </label>" + "<label id=employeeNIFValue>" + txtbEmployeeNIF.Text + "</label>" +
                "<label id=employeeFullNameData> Nombre y Apellidos: </label>" + "<label id=employeeFullNameValue>" + txtbEmployeeFullName.Text + "</label>" +
                "<label id=employeeAdressData> Dirección: </label>" + "<label id=employeeAdressValue>" + txtbEmployeeAdress.Text + "</label>" +
                "<label id=employeeCityData> Ciudad: </label>" + "<label id=employeeCityValue>" + txtbEmployeeCity.Text + "</label>" +
                "<label id=employeePhonesData> Teléfonos: </label>" + "<label id=employeePhonesValue>" + txtbEmployeePhones.Text + "</label>" +
                "<label id=employeeBirthdateData> Fecha de Nacimiento: </label>" + "<label id=employeeBirthdateValue>" + txtbEmployeeBirthDate.Text + "</label>" +
                "<label id=employeeEntryDateData> Fecha de Ingreso: </label>" + "<label id=employeeEntryDateValue>" + txtbEmployeeEntryDate.Text + "</label>" +
                "<label id=employeeGenderData> Sexo: </label>" + "<label id=employeeGenderValue>" + rblEmployeeGender.SelectedItem.Text + "</label>" +
                "<label id=employeeDepartamentData> Departamento: </label> " + "<label id=employeeDepartamentValue>" + ddlEmployeeDepartment.SelectedItem.Text + "</label>" +
            "</div>";
            */


            //MÉTODO MEDIANTE STRINGBUILDER
            lblEployeeData.Visible = true;

            StringBuilder employeeData = new StringBuilder();
            employeeData.Append("<div id='lblFormData'>");
                employeeData.Append("<h3>VALORES DEL FORMULARIO</h3>");
                employeeData.Append("<label id='employeeCodeData'> Código Empleado: </label>");
                employeeData.Append("<label id='employeeCodeValue'>" + txtbEmployeeCode.Text + "</label>");
                employeeData.Append("<label id='employeeNIFData'> NIF: </label>");
                employeeData.Append("<label id='employeeNIFValue'>" + txtbEmployeeNIF.Text + "</label>");
                employeeData.Append("<label id='employeeFullNameData'> Nombre y Apellidos: </label>");
                employeeData.Append("<label id='employeeFullNameValue'>" + txtbEmployeeFullName.Text + "</label>");
                employeeData.Append("<label id='employeeAdressData'> Dirección: </label>");
                employeeData.Append("<label id='employeeAdressValue'>" + txtbEmployeeAdress.Text + "</label>");
                employeeData.Append("<label id='employeeCityData'> Ciudad: </label>");
                employeeData.Append("<label id='employeeCityValue'>" + txtbEmployeeCity.Text + "</label>");
                employeeData.Append("<label id='employeePhonesData'> Teléfonos: </label>");
                employeeData.Append("<label id='employeePhonesValue'>" + txtbEmployeePhones.Text + "</label>");
                employeeData.Append("<label id='employeeBirthdateData'> Fecha de Nacimiento: </label>");
                employeeData.Append("<label id='employeeBirthdateValue'>" + txtbEmployeeBirthDate.Text + "</label>");
                employeeData.Append("<label id='employeeEntryDateData'> Fecha de Ingreso: </label>");
                employeeData.Append("<label id='employeeEntryDateValue'>" + txtbEmployeeEntryDate.Text + "</label>");
                employeeData.Append("<label id='employeeGenderData'> Sexo: </label>");
                employeeData.Append("<label id='employeeGenderValue'>" + rblEmployeeGender.SelectedItem.Text + "</label>");
                employeeData.Append("<label id='employeeDepartamentData'> Departamento: </label>");
                employeeData.Append("<label id='employeeDepartamentValue'>" + ddlEmployeeDepartment.SelectedItem.Text + "</label>");
            employeeData.Append("</div>");
            lblEployeeData.Text = employeeData.ToString();  
        }
    }
}