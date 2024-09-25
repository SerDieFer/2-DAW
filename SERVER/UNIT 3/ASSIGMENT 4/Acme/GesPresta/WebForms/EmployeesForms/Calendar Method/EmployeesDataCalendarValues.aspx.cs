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
        DateTime todayDate = System.DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            txtbEmployeeCode.Focus();
        }

        protected void calendarEmployeeBirthDate_SelectionChanged(object sender, EventArgs e)
        {
            if (!DateError())
            {
                ShowAntiquitySection();
                ShowSendDataSection();
            }
            else
            {
                HideAntiquitySection();
            }
            txtbEmployeeBirthDate.Text = calendarEmployeeBirthDate.SelectedDate.ToShortDateString();
        }

        protected void calendarEmployeeEntryDate_SelectionChanged(object sender, EventArgs e)
        {
            if (!DateError())
            {
                ShowAntiquitySection();
                ShowSendDataSection();
                CalculateAntiquity();
            }
            else
            {
                HideAntiquitySection();
            }
            txtbEmployeeEntryDate.Text = calendarEmployeeEntryDate.SelectedDate.ToShortDateString();
        }

        private void ShowAntiquitySection()
        {
            divEmployeeAntiquity.Visible = true;
        }

        private void ShowSendDataSection()
        {
            btnSendDataEmployee.Visible = true;
        }

        private void HideAntiquitySection()
        {
            divEmployeeAntiquity.Visible = false;
        }

        private void HideSendDataSection()
        {
            btnSendDataEmployee.Visible = false;
        }

        private void CalculateAntiquity()
        {
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;
            TimeSpan dateDifference = todayDate - entryDate;
            DateTime antiquityBase = new DateTime(1, 1, 1);

            DateTime antiquity = antiquityBase + dateDifference;

            txtbEmployeeYearsAntiquity.Text = (antiquity.Year - 1).ToString();
            txtbEmployeeMonthsAntiquity.Text = (antiquity.Month - 1).ToString();
            txtbEmployeeDaysAntiquity.Text = antiquity.Day.ToString();
        }

        private bool DateError()
        {
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;
            DateTime birthDate = calendarEmployeeBirthDate.SelectedDate;
            bool errorFound = false;

            // REINICIAR VISIBILIDAD DE LOS LABELS DE ERROR
            lblError1.Visible = false;
            lblError2.Visible = false;
            lblError3.Visible = false;

            // COMPROBAR SI LA FECHA DE INGRESO ES MENOR QUE LA FECHA DE NACIMIENTO
            if (txtbEmployeeEntryDate.Text != string.Empty)
            {
                if (entryDate < birthDate)
                {
                    lblError1.Text = "ERROR: La fecha de ingreso no puede ser menor que la fecha de nacimiento.";
                    lblError1.Visible = true;
                    errorFound = true;
                }

                // COMPROBAR SI LA FECHA DE INGRESO ES MAYOR QUE LA FECHA ACTUAL
                if (entryDate > todayDate)
                {
                    lblError2.Text = "ERROR: La fecha de ingreso no puede ser mayor que la fecha actual.";
                    lblError2.Visible = true;
                    errorFound = true;
                }
            }

            // COMPROBAR SI LA FECHA DE NACIMIENTO ES MAYOR QUE LA FECHA ACTUAL
            if (birthDate > todayDate)
            {
                lblError3.Text = "ERROR: La fecha de nacimiento no puede ser mayor que la fecha actual.";
                lblError3.Visible = true;
                errorFound = true;
            }

            // SI HAY ERRORES, OCULTAR LA SECCIÓN DE ENVÍO
            if (errorFound)
            {
                HideSendDataSection();
            }

            return errorFound;
        }
    }
}