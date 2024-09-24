<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmployeesDataCalendarValues.aspx.cs" Inherits="GesPresta.EmployeesDataCalendarValues" %>
<%@ Register src="~/UserControllers/HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EMPLEADOS</title>
    <link href="~/CSS/EmployeeStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formEmployee" runat="server">

        <uc1:HeaderController ID="HeaderController1" runat="server" />
        <h2>DATOS DE LOS EMPLEADOS</h2>

        <div id="employeeDataDiv">

            <label>Código Empleado</label>
            <asp:TextBox ID="txtbEmployeeCode" runat="server"></asp:TextBox>

            <label>NIF</label>
            <asp:TextBox ID="txtbEmployeeNIF" runat="server"></asp:TextBox>

            <label>Apellidos y Nombre</label>
            <asp:TextBox ID="txtbEmployeeFullName" runat="server"></asp:TextBox>

            <label>Dirección</label>
            <asp:TextBox ID="txtbEmployeeAdress" runat="server"></asp:TextBox>

            <label>Ciudad</label>
            <asp:TextBox ID="txtbEmployeeCity" runat="server"></asp:TextBox>

            <label>Teléfonos</label>
            <asp:TextBox ID="txtbEmployeePhones" runat="server"></asp:TextBox>

            <label>Sexo</label>
            <asp:RadioButtonList ID="rblEmployeeGender" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Selected="True" Value="H">Hombre</asp:ListItem>
                <asp:ListItem Value="M">Mujer</asp:ListItem>
            </asp:RadioButtonList>

            <label>Departamento</label>
            <asp:DropDownList ID="ddlEmployeeDepartment" runat="server">
                <asp:ListItem Selected="True">Investigación</asp:ListItem>
                <asp:ListItem>Desarrollo</asp:ListItem>
                <asp:ListItem>Innovación</asp:ListItem>
                <asp:ListItem>Administración</asp:ListItem>
            </asp:DropDownList>

            <div id="divCalendarData">
                <div id="divBirthCalendar">
                    <label>Fecha de Nacimiento</label>
                    <asp:TextBox ID="txtbEmployeeBirthDate" runat="server"></asp:TextBox>
                    <asp:Calendar ID="calendarEmployeeBirthDate" runat="server" BackColor="White" BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black">
                        <DayHeaderStyle BackColor="#CCCCCC" BorderStyle="None" Font-Bold="True" Font-Size="7pt" Wrap="True" />
                        <NextPrevStyle VerticalAlign="Bottom" />
                        <OtherMonthDayStyle ForeColor="#808080" />
                        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <SelectorStyle BackColor="#CCCCCC" />
                        <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                        <TodayDayStyle ForeColor="Black" />
                        <WeekendDayStyle BackColor="#FFFFCC" />
                    </asp:Calendar>
                </div>

                <div id="divEntryCalendar">
                    <label>Fecha de Ingreso</label>
                    <asp:TextBox ID="txtbEmployeeEntryDate" runat="server"></asp:TextBox>
                    <asp:Calendar ID="calendarEmployeeEntryDate" runat="server" BackColor="White" BorderColor="#999999" CellPadding="4" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black">
                        <DayHeaderStyle BackColor="#CCCCCC" Font-Bold="True" Font-Size="7pt" />
                        <NextPrevStyle VerticalAlign="Bottom" />
                        <OtherMonthDayStyle ForeColor="#808080" />
                        <SelectedDayStyle BackColor="#666666" Font-Bold="True" ForeColor="White" />
                        <SelectorStyle BackColor="#CCCCCC" />
                        <TitleStyle BackColor="#999999" BorderColor="Black" Font-Bold="True" />
                        <TodayDayStyle ForeColor="Black" />
                        <WeekendDayStyle BackColor="#FFFFCC" />
                    </asp:Calendar>
                </div>

                <div id="divEmployeeAntiquity">
                    <label>Antiguedad</label>
                    <div id="employeeAntiquityValues">
                        <label>Años</label>
                        <asp:TextBox ID="txtbEmployeeYearsAntiquity" runat="server"></asp:TextBox>
                        <label>Meses</label>
                        <asp:TextBox ID="txtbEmployeeMonthsAntiquity" runat="server"></asp:TextBox>
                        <label>Días</label>
                        <asp:TextBox ID="txtbEmployeeDaysAntiquity" runat="server"></asp:TextBox>
                    </div>
                </div>

            </div>

        </div>


        <div class="employeeDataButtonDiv">
            <asp:Button ID="btnSendDataEmployee" runat="server" Text="Enviar" OnClick="btnSendDataEmployee_Click"/>
        </div>

        <!--Sin Width ni Backcolor, se la defino con CSS, sino el elemento quedaría así:
            <asp:Label ID="lblEmployeeDataFull" runat="server" Width="60%" BackColor="#666FFFF" Visible="false"></asp:Label> 
        -->
        <asp:Label ID="lblEmployeeData" runat="server" Visible="false"></asp:Label>

    </form>
</body>
</html>