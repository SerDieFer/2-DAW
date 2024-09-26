using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta
{
    public partial class EmployeesDataCalendarValues : System.Web.UI.Page
    {
        DateTime todayDate = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtbEmployeeCode.Focus();
        }

        protected void calendarEmployeeBirthDate_SelectionChanged(object sender, EventArgs e)
        {
            txtbEmployeeBirthDate.Text = calendarEmployeeBirthDate.SelectedDate.ToShortDateString();
            HandleDateLogic();
        }

        protected void calendarEmployeeEntryDate_SelectionChanged(object sender, EventArgs e)
        {
            txtbEmployeeEntryDate.Text = calendarEmployeeEntryDate.SelectedDate.ToShortDateString();
            HandleDateLogic();
        }

        // MANEJO DE LA LÓGICA DE LA FECHA SELECCIONADA DE ENTRADA
        private void HandleDateLogic()
        {
            ShowAntiquitySection();
            ShowSendDataSection();

            // VERIFICAR ERRORES Y MOSTRAR MENSAJES DE ERROR, PERO REALIZAR EL CÁLCULO DE ANTIGÜEDAD SIEMPRE
            bool hasError = DateError();

            // SE REALIZA EL CÁLCULO DE ANTIGÜEDAD AUNQUE HAYA ERRORES
            if (!string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                CalculateAntiquity();
            }

            // SI HAY ERROR, SE MANTENDRÁN LOS VALORES DE ANTIGÜEDAD PERO SE MOSTRARÁN LOS ERRORES
            if (hasError)
            {
                // OCULTAMOS LA SECCIÓN DE ENVÍO SI HAY ERRORES
                HideSendDataSection(); 
            }
        }

        private bool DateError()
        {
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;
            DateTime birthDate = calendarEmployeeBirthDate.SelectedDate;
            bool errorFound = false;

            lblError1.Visible = false;
            lblError2.Visible = false;
            lblError3.Visible = false;

            // VERIFICO SI LA FECHA DE INGRESO ES MENOR QUE LA FECHA DE NACIMIENTO
            if (entryDate < birthDate && !string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                lblError1.Text = "ERROR: La fecha de ingreso no puede ser menor que la fecha de nacimiento.";
                lblError1.Visible = true;
                errorFound = true;
            }

            // VERIFICO SI LA FECHA DE INGRESO ES MAYOR QUE LA FECHA ACTUAL
            if (entryDate > todayDate && !string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                lblError2.Text = "ERROR: La fecha de ingreso no puede ser mayor que la fecha actual.";
                lblError2.Visible = true;
                errorFound = true;
                ResetAntiquityValues();
            }

            // VERIFICO SI LA FECHA DE NACIMIENTO ES MAYOR QUE LA FECHA ACTUAL
            if (birthDate > todayDate)
            {
                lblError3.Text = "ERROR: La fecha de nacimiento no puede ser mayor que la fecha actual.";
                lblError3.Visible = true;
                errorFound = true;
            }

            return errorFound;
        }

        private void CalculateAntiquity()
        {
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;

            if (entryDate <= todayDate)
            {
                TimeSpan dateDifference = todayDate - entryDate;
                DateTime antiquityBase = new DateTime(1, 1, 1);
                DateTime antiquity = antiquityBase.Add(dateDifference);

                txtbEmployeeYearsAntiquity.Text = (antiquity.Year - 1).ToString();
                txtbEmployeeMonthsAntiquity.Text = (antiquity.Month - 1).ToString();
                txtbEmployeeDaysAntiquity.Text = antiquity.Day.ToString();
            }
            else
            {
                ResetAntiquityValues();
            }
        }

        private void ResetAntiquityValues()
        {
            txtbEmployeeYearsAntiquity.Text = string.Empty;
            txtbEmployeeMonthsAntiquity.Text = string.Empty;
            txtbEmployeeDaysAntiquity.Text = string.Empty;
        }

        private void ShowAntiquitySection()
        {
            divEmployeeAntiquity.Visible = true;
        }

        private void ShowSendDataSection()
        {
            btnSendDataEmployee.Visible = true;
        }

        private void HideSendDataSection()
        {
            btnSendDataEmployee.Visible = false;
        }
    }
}