<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="GesPresta.Employees"%>
<%@ Register src="~/UserControllers/HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EMPLEADOS</title>
    <link href="../../CSS/EmployeeStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formEmployee" runat="server">

        <uc1:HeaderController ID="HeaderController1" runat="server" />
        <h2>DATOS DE LOS EMPLEADOS</h2>

        <div id="employeeDataDiv">

            <label>Código Empleado</label>
            <asp:TextBox ID="txtbEmployeeCode" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeCode" 
                runat="server" 
                ErrorMessage="The employee code is required" 
                ControlToValidate="txtbEmployeeCode">
            </asp:RequiredFieldValidator>

            <label>NIF</label>
            <asp:TextBox ID="txtbEmployeeNIF" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeNIF" 
                runat="server" 
                ErrorMessage="The employee NIF is required" 
                ControlToValidate="txtbEmployeeNIF">
            </asp:RequiredFieldValidator>

            <label>Apellidos y Nombre</label>
            <asp:TextBox ID="txtbEmployeeFullName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeFullName" 
                runat="server" 
                ErrorMessage="The employee full name is required" 
                ControlToValidate="txtbEmployeeFullName">
            </asp:RequiredFieldValidator>

            <label>Dirección</label>
            <asp:TextBox ID="txtbEmployeeAdress" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeAdress"
                runat="server" 
                ErrorMessage="The employee adress is required" 
                ControlToValidate="txtbEmployeeAdress">
            </asp:RequiredFieldValidator>

            <label>Ciudad</label>
            <asp:TextBox ID="txtbEmployeeCity" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeCity" 
                runat="server" 
                ErrorMessage="The employee city is required" 
                ControlToValidate="txtbEmployeeCity">
            </asp:RequiredFieldValidator>

            <label>Teléfonos</label>
            <asp:TextBox ID="txtbEmployeePhones" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                 ForeColor="red"
                ID="rqdtxtbEmployeePhones" 
                runat="server" 
                ErrorMessage="The employee phone/s is/are required" 
                ControlToValidate="txtbEmployeePhones">
            </asp:RequiredFieldValidator>

            <label>Fecha de Nacimiento</label>
            <asp:TextBox ID="txtbEmployeeBirthDate" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator 
                ForeColor="red"
                ID="rqdtxtbEmployeeBirthDate"
                runat="server" 
                ErrorMessage="The employee birthdate is required" 
                ControlToValidate="txtbEmployeeBirthDate">
            </asp:RequiredFieldValidator>
            <asp:CompareValidator
                Type="Date"
                ForeColor="red"
                ID="cmpttxtbEmployeeBirthDate" 
                runat="server"
                ErrorMessage="CompareValidator"
                ControlToValidate="txtbEmployeeBirthDate"
                ControlToCompare="txtbEmployeeEntryDate"
                Operator="LessThan">
            </asp:CompareValidator>

            <label>Fecha de Ingreso</label>
            <asp:TextBox ID="txtbEmployeeEntryDate" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator 
                ForeColor="red"
                ID="rqdtxtbEmployeeEntryDate" 
                runat="server" 
                ErrorMessage="The employee entry date is required" 
                ControlToValidate="txtbEmployeeEntryDate">
            </asp:RequiredFieldValidator>

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

        </div>

        <div class="employeeDataButtonDiv">
            <asp:Button ID="btnSendDataEmployee" runat="server" Text="Enviar" />
        </div>

    </form>
</body>
</html>