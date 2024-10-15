<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="GesPresta.Employees"%>
<%@ Register src="~/UserControllers/HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>EMPLEADOS</title>
    <link href="../../CSS/EmployeeStyleSheet2.css" rel="stylesheet" />
</head>
<body>
<form id="formEmployee" runat="server">
    <uc1:HeaderController ID="HeaderController1" runat="server" />
    <h2>DATOS DE LOS EMPLEADOS</h2>
    <div id="employeeDataDiv" style="
                                display: flex;
                                flex-direction: column;
                                gap: 20px;
                                width: 75em;
                                max-width: 75em;
                                margin: 20px auto;
                              ">

        <div class="employeeRow">
            <label for="txtbEmployeeCode">Código Empleado</label>
            <asp:TextBox ID="txtbEmployeeCode" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeCode" 
                runat="server"
                Text="*"
                ErrorMessage="Se requiere el código de empleado" 
                ControlToValidate="txtbEmployeeCode">
            </asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeCode" 
                runat="server" 
                ForeColor="green"
                ValidationExpression="\w\d{5}"
                ErrorMessage="El formato de codigo debe ser algo como: 'A12345'"
                ControlToValidate="txtbEmployeeCode">
            </asp:RegularExpressionValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeeNIF">NIF</label>
            <asp:TextBox ID="txtbEmployeeNIF" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeNIF" 
                runat="server" 
                Text="*"
                ErrorMessage="El NIF del empleado es requerido" 
                ControlToValidate="txtbEmployeeNIF">
            </asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeNIF" 
                runat="server"
                ForeColor="green"
                ValidationExpression="\d{8}-\w"
                ErrorMessage="El formato del NIF debe ser algo como: '12345678A'"
                ControlToValidate="txtbEmployeeNIF">
            </asp:RegularExpressionValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeeFullName">Apellidos y Nombre</label>
            <asp:TextBox ID="txtbEmployeeFullName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeFullName" 
                runat="server"
                Text="*"
                ErrorMessage="Los nombres y apellidos del empleado son necesarios" 
                ControlToValidate="txtbEmployeeFullName">
            </asp:RequiredFieldValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeeAdress">Dirección</label>
            <asp:TextBox ID="txtbEmployeeAdress" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeAdress"
                Text="*"
                runat="server" 
                ErrorMessage="Se requiere la direcciión del empleado" 
                ControlToValidate="txtbEmployeeAdress">
            </asp:RequiredFieldValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeeCity">Ciudad</label>
            <asp:TextBox ID="txtbEmployeeCity" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeCity" 
                Text="*"
                runat="server" 
                ErrorMessage="Se requiere la ciudad del empleado" 
                ControlToValidate="txtbEmployeeCity">
            </asp:RequiredFieldValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeePhones">Teléfonos</label>
            <asp:TextBox ID="txtbEmployeePhones" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeePhones" 
                Text="*"
                runat="server" 
                ErrorMessage="Se requiere el/los teléfono/s del empleado" 
                ControlToValidate="txtbEmployeePhones">
            </asp:RequiredFieldValidator>
        </div>

        <div class="employeeRowVarious">
            <label for="txtbEmployeeBirthMasterDate">Fecha de Nacimiento</label>
            <asp:TextBox ID="txtbEmployeeBirthMasterDate" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator 
                ForeColor="red"
                ID="rqdtxtbEmployeeBirthDate"
                runat="server"
                Text="*"
                ErrorMessage="Se requiere la fecha de nacimiento del empleado"
                ControlToValidate="txtbEmployeeBirthMasterDate">
            </asp:RequiredFieldValidator>
            <asp:CompareValidator
                Type="Date"
                ForeColor="red"
                ID="cmpttxtbEmployeeBirthDate" 
                runat="server"
                ErrorMessage="La fecha de nacimiento debe ser anterior a la fecha de ingreso."
                ControlToValidate="txtbEmployeeBirthMasterDate"
                ControlToCompare="txtbEmployeeEntryMasterDate"
                Operator="LessThan">
            </asp:CompareValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeBirthDate" 
                runat="server"
                ForeColor="green"
                ValidationExpression="\d\d\/\d\d\/\d\d\d\d"
                ErrorMessage="El formato de la fecha debe ser algo como: '03/10/2024'"
                ControlToValidate="txtbEmployeeBirthMasterDate">
            </asp:RegularExpressionValidator>
        </div>

        <div class="employeeRow">
            <label for="txtbEmployeeEntryMasterDate">Fecha de Ingreso</label>
            <asp:TextBox ID="txtbEmployeeEntryMasterDate" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeEntryDate" 
                runat="server"
                Text="*"
                ErrorMessage="Se requiere la fecha de entrada del empleado" 
                ControlToValidate="txtbEmployeeEntryMasterDate">
            </asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeEntryDate" 
                runat="server"
                ForeColor="green"
                ValidationExpression="\d\d\/\d\d\/\d\d\d\d"
                ErrorMessage="El formato de la fecha debe ser algo como: '03/10/2024'"
                ControlToValidate="txtbEmployeeEntryMasterDate">
            </asp:RegularExpressionValidator>
        </div>

        <div class="employeeRow">
            <label>Sexo</label>
            <asp:RadioButtonList ID="rblEmployeeGender" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Selected="True" Value="H">Hombre</asp:ListItem>
                <asp:ListItem Value="M">Mujer</asp:ListItem>
            </asp:RadioButtonList>
        </div>

        <div class="employeeRow">
            <label for="ddlEmployeeDepartment">Departamento</label>
            <asp:DropDownList ID="ddlEmployeeDepartment" runat="server">
                <asp:ListItem Selected="True">Investigación</asp:ListItem>
                <asp:ListItem>Desarrollo</asp:ListItem>
                <asp:ListItem>Innovación</asp:ListItem>
                <asp:ListItem>Administración</asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div class="employeeDataButtonDiv">
        <asp:Button ID="btnSendDataEmployee" CssClass="sendButton" runat="server" Text="Enviar" />
    </div>

    <asp:ValidationSummary 
        ID="employeValidationSummary" 
        runat="server" 
        ForeColor="red"
        Display="list" />
</form>
</body>
</html>