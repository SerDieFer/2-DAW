<%@ Page Title="" Language="C#" MasterPageFile="~/WebForms/MasterPagesForms/MasterPage.Master" AutoEventWireup="true" CodeBehind="MasterPageEmployees.aspx.cs" Inherits="GesPresta.WebForms.MasterPageEmployees" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>EMPLEADOS</title>
    <link href="../../CSS/EmployeeStyleSheet2.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>DATOS DE LOS EMPLEADOS</h2>
    <div class="employeeContainer">

        <div id="employeeDataDiv">

            <!-- Fila 1 - Código del Empleado -->
            <div id="employeeCodeDiv" class="employeeRow">
                <label>Código Empleado</label>
                <asp:TextBox ID="txtbEmployeeCode" runat="server"></asp:TextBox>
            </div>

            <!-- Fila 2 - NIF del Empleado -->
            <div id="employeeNIFDiv" class="employeeRow">
                <label>NIF</label>
                <asp:TextBox ID="txtbEmployeeNIF" runat="server"></asp:TextBox>
            </div>

            <!-- Fila 3 - Nombre Completo Empleado -->
            <div id="employeeFullNameDiv" class="employeeRow">
                <label>Apellidos y Nombre</label>
                <asp:TextBox ID="txtbEmployeeFullName" runat="server"></asp:TextBox>
            </div>

            <!-- Fila 4 - Dirección Empleado -->
            <div id="employeeAdressDiv" class="employeeRow">
                <label>Dirección</label>
                <asp:TextBox ID="txtbEmployeeAdress" runat="server"></asp:TextBox>
            </div>

            <!-- Fila 5 - Ciudad Empleado -->
            <div id="employeeCityDiv" class="employeeRow">
                <label>Ciudad</label>
                <asp:TextBox ID="txtbEmployeeCity" runat="server"></asp:TextBox>
            </div>
        
            <!-- Fila 6 - Teléfono Empleado -->
            <div id="employeePhoneDiv" class="employeeRow">
                <label>Teléfonos</label>
                <asp:TextBox ID="txtbEmployeePhones" runat="server"></asp:TextBox>
            </div>

            <!-- Fila 7 - Género Empleado -->
            <div id="employeeGenderDiv" class="employeeRow">
                <label>Sexo</label>
                <asp:RadioButtonList ID="rblEmployeeGender" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Value="H">Hombre</asp:ListItem>
                    <asp:ListItem Value="M">Mujer</asp:ListItem>
                </asp:RadioButtonList>
            </div>

            <!-- Fila 7 - Departamento Empleado -->
            <div id="employeeDepartmentDiv" class="employeeRow">
                <label>Departamento</label>
                <asp:DropDownList ID="ddlEmployeeDepartment" runat="server">
                    <asp:ListItem Selected="True">Investigación</asp:ListItem>
                    <asp:ListItem>Desarrollo</asp:ListItem>
                    <asp:ListItem>Innovación</asp:ListItem>
                    <asp:ListItem>Administración</asp:ListItem>
                </asp:DropDownList>
            </div>

        </div>

        <!-- Columna 2 -->
         <div id="extra">
            <div id="divCalendarData">
                <div id="divBirthCalendar">
                    <label>Fecha de Nacimiento</label>
                    <asp:TextBox ID="txtbEmployeeBirthDate" runat="server" AutoPostBack="True" OnTextChanged="txtbEmployeeBirthDate_TextChanged"></asp:TextBox>
                    <asp:Calendar ID="calendarEmployeeBirthDate" runat="server" BackColor="White" BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" OnSelectionChanged="calendarEmployeeBirthDate_SelectionChanged">
                        <DayHeaderStyle BackColor="#CCCCCC" BorderStyle="None" Font-Bold="True" Font-Size="7pt" Wrap="True" Height="2em" />
                        <DayStyle Height="2em" />
                        <NextPrevStyle VerticalAlign="Bottom" />
                        <OtherMonthDayStyle ForeColor="#808080" />
                        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <SelectorStyle BackColor="#CCCCCC" />
                        <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" Height="3em" />
                        <TodayDayStyle ForeColor="Black" BackColor="#FFCC99" />
                        <WeekendDayStyle BackColor="#FFFFCC" />
                    </asp:Calendar>
                </div>

                <div id="divEntryCalendar">
                    <label>Fecha de Ingreso</label>
                    <asp:TextBox ID="txtbEmployeeEntryDate" runat="server" AutoPostBack="True" OnTextChanged="txtbEmployeeEntryDate_TextChanged"></asp:TextBox>
                    <asp:Calendar ID="calendarEmployeeEntryDate" runat="server" BackColor="White" BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" OnSelectionChanged="calendarEmployeeEntryDate_SelectionChanged">
                        <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" Height="2em" />
                        <DayStyle Height="2em" />
                        <NextPrevStyle VerticalAlign="Bottom" />
                        <OtherMonthDayStyle ForeColor="#808080" />
                        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <SelectorStyle BackColor="#CCCCCC" />
                        <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" Height="3em" />
                        <TodayDayStyle ForeColor="Black" BackColor="#FFCC99" />
                        <WeekendDayStyle BackColor="#FFFFCC" />
                    </asp:Calendar>
                </div>

                <div id="divEmployeeAntiquity" runat="server" visible="false">
                    <label id="lblAntiquity">Antiguedad</label>
                    <div id="employeeAntiquityValues">
                        <label>Años</label>
                        <asp:TextBox ID="txtbEmployeeYearsAntiquity" runat="server" AutoPostBack="True" OnTextChanged="txtbEmployeeYearsAntiquity_TextChanged"></asp:TextBox>
                        <label>Meses</label>
                        <asp:TextBox ID="txtbEmployeeMonthsAntiquity" runat="server" AutoPostBack="True" OnTextChanged="txtbEmployeeMonthsAntiquity_TextChanged"></asp:TextBox>
                        <label>Días</label>
                        <asp:TextBox ID="txtbEmployeeDaysAntiquity" runat="server" AutoPostBack="True" OnTextChanged="txtbEmployeeDaysAntiquity_TextChanged"></asp:TextBox>
                    </div>
                </div>

            </div>

        </div>

    </div>

    <div class="employeeDataButtonDiv">
        <asp:Button ID="btnSendDataEmployee" CssClass="btnSendDataButton" runat="server" Text="Enviar" />
        <div class="employeeErrorsDiv">
            <!-- FECHA INGRESO COMPAÑÍA MENOR A LA FECHA DE NACIMIENTO-->
            <asp:Label ID="lblError1" runat="server" Visible="false"></asp:Label>
            <br />
            <!-- FECHA INGRESO COMPAÑÍA MAYOR A LA FECHA ACTUAL-->
            <asp:Label ID="lblError2" runat="server" Visible="false"></asp:Label>
            <br />
            <!-- FECHA NACIMIENTO MAYOR A LA FECHA ACTUAL-->
            <asp:Label ID="lblError3" runat="server" Visible="false"></asp:Label>
            <br />
            <!-- FORMATO NACIMIENTO ESCRITO INVÁLIDO-->
            <asp:Label ID="lblError4" runat="server" Visible="false"></asp:Label>
            <br />
            <!-- FORMATO ENTRADA EMPRESA ESCRITO INVÁLIDO-->
            <asp:Label ID="lblError5" runat="server" Visible="false"></asp:Label>
            <br />
        </div>
    </div>
</asp:Content>
