using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GesPresta.WebForms
{
    public partial class MasterPageEmployees : System.Web.UI.Page
    {
        DateTime todayDate = DateTime.Now;
        protected void Page_Load(object sender, EventArgs e)
        {
            txtbEmployeeCode.Focus();
        }

        protected void txtbEmployeeDaysAntiquity_TextChanged(object sender, EventArgs e)
        {
            int dayAntiquity = GetPositiveNumberFromText(txtbEmployeeDaysAntiquity.Text.Trim());

            if (dayAntiquity == 0)
            {
                ResetAntiquityValues();
                txtbEmployeeEntryDate.Text = string.Empty;
                calendarEmployeeEntryDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeEntryDate.VisibleDate = DateTime.MinValue;
            }
            else
            {
                CalculateAntiquityDataToCalendarSelection();
            }

            HandleDateLogic();
        }

        protected void txtbEmployeeMonthsAntiquity_TextChanged(object sender, EventArgs e)
        {
            int monthAntiquity = GetPositiveNumberFromText(txtbEmployeeMonthsAntiquity.Text.Trim());

            if (monthAntiquity == 0)
            {
                ResetAntiquityValues();
                txtbEmployeeEntryDate.Text = string.Empty;
                calendarEmployeeEntryDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeEntryDate.VisibleDate = DateTime.MinValue;
            }
            else
            {
                CalculateAntiquityDataToCalendarSelection();
            }

            HandleDateLogic();
        }

        protected void txtbEmployeeYearsAntiquity_TextChanged(object sender, EventArgs e)
        {
            int yearAntiquity = GetPositiveNumberFromText(txtbEmployeeYearsAntiquity.Text.Trim());

            if (yearAntiquity == 0)
            {
                ResetAntiquityValues();
                txtbEmployeeEntryDate.Text = string.Empty;
                calendarEmployeeEntryDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeEntryDate.VisibleDate = DateTime.MinValue;
            }
            else
            {
                CalculateAntiquityDataToCalendarSelection();
            }

            HandleDateLogic();
        }

        protected void txtbEmployeeBirthDate_TextChanged(object sender, EventArgs e)
        {
            lblError4.Visible = false;
            DateTime birthDate;

            if (DateTime.TryParse(txtbEmployeeBirthDate.Text, out birthDate))
            {
                calendarEmployeeBirthDate.SelectedDate = birthDate;
                calendarEmployeeBirthDate.VisibleDate = birthDate;
                HandleDateLogic();
            }
            else
            {
                txtbEmployeeBirthDate.Text = string.Empty;
                calendarEmployeeBirthDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeBirthDate.VisibleDate = DateTime.MinValue;

                HideSendDataSection();
                lblError4.Text = "ERROR: Formato de fecha de nacimiento inválido";
                lblError4.Visible = true;
                lblError3.Visible = false;
                lblError5.Visible = false;
            }
        }

        protected void txtbEmployeeEntryDate_TextChanged(object sender, EventArgs e)
        {
            lblError5.Visible = false;
            DateTime entryDate;

            if (DateTime.TryParse(txtbEmployeeEntryDate.Text, out entryDate))
            {
                calendarEmployeeEntryDate.SelectedDate = entryDate;
                calendarEmployeeEntryDate.VisibleDate = entryDate;
                HandleDateLogic();
            }
            else
            {
                txtbEmployeeEntryDate.Text = string.Empty;
                calendarEmployeeEntryDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeEntryDate.VisibleDate = DateTime.MinValue;

                HideSendDataSection();
                ResetAntiquityValues();
                lblError5.Text = "ERROR: Formato de fecha de entrada a la empresa inválido";
                lblError5.Visible = true;
                lblError2.Visible = false;
                lblError4.Visible = false;
            }
        }

        protected void calendarEmployeeBirthDate_SelectionChanged(object sender, EventArgs e)
        {
            lblError4.Visible = false;
            txtbEmployeeBirthDate.Text = calendarEmployeeBirthDate.SelectedDate.ToShortDateString();
            HandleDateLogic();
        }

        protected void calendarEmployeeEntryDate_SelectionChanged(object sender, EventArgs e)
        {
            lblError5.Visible = false;
            txtbEmployeeEntryDate.Text = calendarEmployeeEntryDate.SelectedDate.ToShortDateString();
            HandleDateLogic();
        }

        // MANEJO DE LA LÓGICA DE LA FECHA SELECCIONADA DE ENTRADA
        private void HandleDateLogic()
        {
            // MOSTRAR LA SECCIÓN DE ANTIGÜEDAD SI HAY UNA FECHA DE INGRESO
            if (!string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                ShowAntiquitySection();
                CalculateAntiquity();
            }

            // CHEQUEO DE POSIBLES ERRORES
            bool hasError = DateError();

            if (!hasError)
            {
                ShowSendDataSection();

                // EN CASO DE FALTAR SELECCIÓN DE FECHA DE ENTRADA OCULTARÁ LA SELECCIÓN DE ANTIGÜEDAD
                if (calendarEmployeeEntryDate.SelectedDate == DateTime.MinValue || string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
                {
                    HideAntiquitySection();
                }
            }
            else
            {
                HideSendDataSection();
            }
        }

        // BOOLEANO PARA LOS ERRORES DE FECHA
        private bool DateError()
        {
            // OBTENCIÓN DE LA FECHA DE INGRESO Y LA FECHA DE NACIMIENTO
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;
            DateTime birthDate = calendarEmployeeBirthDate.SelectedDate;
            bool errorFound = false;

            // OCULTAMOS LA SECCIÓN DE ERRORES
            HideErrorsSection();

            // VALIDACIÓN DE SI LA FECHA DE INGRESO ES MENOR QUE LA DE NACIMIENTO
            if (entryDate < birthDate && !string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                lblError1.Text = "ERROR: La fecha de ingreso no puede ser menor que la fecha de nacimiento.";
                lblError1.Visible = true;
                validationItems.Visible = true;
                errorFound = true;
            }

            // VALIDACIÓN DE SI LA FECHA DE INGRESO ES MAYOR QUE LA FECHA ACTUAL
            if (entryDate > todayDate && !string.IsNullOrEmpty(txtbEmployeeEntryDate.Text))
            {
                lblError2.Text = "ERROR: La fecha de ingreso no puede ser mayor que la fecha actual.";
                lblError2.Visible = true;
                validationItems.Visible = true;
                errorFound = true;
                ResetAntiquityValues();
            }

            // VALIDACIÓN DE SI LA FECHA DE NACIMIENTO ES MAYOR QUE LA FECHA ACTUAL
            if (birthDate > todayDate)
            {
                lblError3.Text = "ERROR: La fecha de nacimiento no puede ser mayor que la fecha actual.";
                lblError3.Visible = true;
                validationItems.Visible = true;
                errorFound = true;

            }

            return errorFound;
        }

        // COLOCA LA FECHA DE INGRESO DEL TRABAJADOR DEPENDIENDO LOS DÍAS-MESES-AÑOS SELECCIONADOS EN LAS CASILLAS DE LA ANTIGÜEDAD
        private void CalculateAntiquityDataToCalendarSelection()
        {
            DateTime actualDate = DateTime.Today;

            // OBTENER LOS AÑOS, MESES Y DÍAS DE ANTIGÜEDAD, ASEGURÁNDOSE DE QUE SEAN POSITIVOS
            int yearAntiquity = GetPositiveNumberFromText(txtbEmployeeYearsAntiquity.Text.Trim());
            int monthAntiquity = GetPositiveNumberFromText(txtbEmployeeMonthsAntiquity.Text.Trim());
            int dayAntiquity = GetPositiveNumberFromText(txtbEmployeeDaysAntiquity.Text.Trim());

            // SI ALGUNO DE LOS VALORES ES 0, SE REINICIAN LOS CAMPOS
            if (yearAntiquity == 0 && monthAntiquity == 0 && dayAntiquity == 0)
            {
                ResetAntiquityValues();
                txtbEmployeeEntryDate.Text = string.Empty;
                calendarEmployeeEntryDate.SelectedDate = DateTime.MinValue;
                calendarEmployeeEntryDate.VisibleDate = DateTime.MinValue;
                return;
            }

            // CALCULO DE LA FECHA DE ENTRADA EN FUNCIÓN DE LOS AÑOS, MESES Y DÍAS DE ANTIGÜEDAD
            DateTime calculatedAntiquityDate = actualDate
                                                 .AddYears(-yearAntiquity)
                                                 .AddMonths(-monthAntiquity)
                                                 .AddDays(-dayAntiquity);

            // ACTUALIZAMOS EL TEXTBOX Y EL CALENDARIO
            txtbEmployeeEntryDate.Text = calculatedAntiquityDate.ToShortDateString();
            calendarEmployeeEntryDate.SelectedDate = calculatedAntiquityDate;
            calendarEmployeeEntryDate.VisibleDate = calculatedAntiquityDate;
        }

        // CALCULO ANTIGÜEDAD
        private void CalculateAntiquity()
        {
            // OBTIENE LA FECHA DE INGRESO
            DateTime entryDate = calendarEmployeeEntryDate.SelectedDate;

            if (entryDate <= todayDate)
            {
                // CALCULO DE LA DIFERENCIA ENTRE LAS FECHAS
                TimeSpan dateDifference = todayDate - entryDate;

                // CREACIÓN DE UNA BASE DE FECHA PARA CALCULAR LA ANTIGÜEDAD
                DateTime antiquityBase = new DateTime(1, 1, 1);

                // AÑADE LA DIFERENCIA DE FECHA A LA FECHA BASE PARA OBTENER LA ANTIGÜEDAD
                DateTime antiquity = antiquityBase.Add(dateDifference);

                // ASIGNACIÓN DE LOS VALORES A LOS CAMPOS DE ANTIGÜEDAD
                txtbEmployeeYearsAntiquity.Text = (antiquity.Year - 1).ToString();
                txtbEmployeeMonthsAntiquity.Text = (antiquity.Month - 1).ToString();
                txtbEmployeeDaysAntiquity.Text = antiquity.Day.ToString();
            }
            else
            {
                // SI LA FECHA DE INGRESO ES MAYOR QUE LA FECHA ACTUAL, RESETEA LOS VALORES DE ANTIGÜEDAD
                ResetAntiquityValues();
            }
        }

        // CONVERSOR REGEX PARA OBTENER UN NÚMERO POSITIVO [>1] DEL TEXTO
        private int GetPositiveNumberFromText(string text)
        {
            // USO UNA EXPRESIÓN REGULAR PARA ENCONTRAR TODOS LOS DÍGITOS EN EL TEXTO
            Match numRegex = Regex.Match(text, @"\d+");

            if (numRegex.Success)
            {
                // SI SE ENCUENTRA UN NÚMERO, LO DEVUELVE COMO ENTERO POSITIVO
                return Math.Abs(int.Parse(numRegex.Value));
            }
            return 0;
        }

        // RESETEA LOS CAMPOS DE ANTIGÜEDAD A 0
        private void ResetAntiquityValues()
        {
            txtbEmployeeYearsAntiquity.Text = "0";
            txtbEmployeeMonthsAntiquity.Text = "0";
            txtbEmployeeDaysAntiquity.Text = "0";
        }

        // MUESTRA LA SECCIÓN DE ANTIGÜEDAD
        private void ShowAntiquitySection()
        {
            divEmployeeAntiquity.Visible = true;
        }

        // OCULTA LA SECCIÓN DE ANTIGÜEDAD
        private void HideAntiquitySection()
        {
            divEmployeeAntiquity.Visible = false;
        }

        // MUESTRA LA SECCIÓN DE ENVÍO DE DATOS
        private void ShowSendDataSection()
        {
            btnSendDataEmployee.Visible = true;
        }

        // OCULTA LA SECCIÓN DE ENVÍO DE DATOS
        private void HideSendDataSection()
        {
            btnSendDataEmployee.Visible = false;
        }

        // OCULTA TODAS LAS SECCIONES DE ERROR
        private void HideErrorsSection()
        {
            lblError1.Visible = false;
            lblError2.Visible = false;
            lblError3.Visible = false;
            lblError4.Visible = false;
            lblError5.Visible = false;
            validationItems.Visible = false;
        }
    }
}