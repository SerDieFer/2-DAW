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
                Text="*"
                ErrorMessage="The employee code is required" 
                ControlToValidate="txtbEmployeeCode">
            </asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeCode" 
                runat="server" 
                ForeColor="green"
                ValidationExpression="\w\d{5}"
                ErrorMessage="The data format must be something like: 'A12345' - '_54321'"
                ControlToValidate="txtbEmployeeCode">
            </asp:RegularExpressionValidator>

            <br />

            <label>NIF</label>
            <asp:TextBox ID="txtbEmployeeNIF" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeNIF" 
                runat="server" 
                Text="*"
                ErrorMessage="The employee NIF is required" 
                ControlToValidate="txtbEmployeeNIF">
            </asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator 
                ID="regtxtbEmployeeNIF" 
                runat="server"
                ForeColor="green"
                ValidationExpression="\d{8}-\w"
                ErrorMessage="The data format must be something like: '12345678A'"
                ControlToValidate="txtbEmployeeNIF">
            </asp:RegularExpressionValidator>

            <br />

            <label>Apellidos y Nombre</label>
            <asp:TextBox ID="txtbEmployeeFullName" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeFullName" 
                runat="server"
                Text="*"
                ErrorMessage="The employee full name is required" 
                ControlToValidate="txtbEmployeeFullName">
            </asp:RequiredFieldValidator>

            <span></span>
            <br/>

            <label>Dirección</label>
            <asp:TextBox ID="txtbEmployeeAdress" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeAdress"
                Text="*"
                runat="server" 
                ErrorMessage="The employee adress is required" 
                ControlToValidate="txtbEmployeeAdress">
            </asp:RequiredFieldValidator>

            <span></span>
            <br/>

            <label>Ciudad</label>
            <asp:TextBox ID="txtbEmployeeCity" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeeCity" 
                Text="*"
                runat="server" 
                ErrorMessage="The employee city is required" 
                ControlToValidate="txtbEmployeeCity">
            </asp:RequiredFieldValidator>

            <span></span>
            <br/>

            <label>Teléfonos</label>
            <asp:TextBox ID="txtbEmployeePhones" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator
                ForeColor="red"
                ID="rqdtxtbEmployeePhones" 
                Text="*"
                runat="server" 
                ErrorMessage="The employee phone/s is/are required" 
                ControlToValidate="txtbEmployeePhones">
            </asp:RequiredFieldValidator>

            <span></span>
            <br/>

            <div id="employeeBirthDateDiv">
                <label>Fecha de Nacimiento</label>
                <asp:TextBox ID="txtbEmployeeBirthDate" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator 
                    ForeColor="red"
                    ID="rqdtxtbEmployeeBirthDate"
                    runat="server"
                    Text="*"
                    ErrorMessage="The employee birthdate is required" 
                    ControlToValidate="txtbEmployeeBirthDate">
                </asp:RequiredFieldValidator>
                <asp:CompareValidator
                    Type="Date"
                    ForeColor="red"
                    ID="cmpttxtbEmployeeBirthDate" 
                    runat="server"
                    ErrorMessage="The employee' birth date must be before the entry date of the employee "
                    ControlToValidate="txtbEmployeeBirthDate"
                    ControlToCompare="txtbEmployeeEntryDate"
                    Operator="LessThan">
                </asp:CompareValidator>
                <asp:RegularExpressionValidator 
                    ID="regtxtbEmployeeBirthDate" 
                    runat="server"
                    ForeColor="green"
                    ValidationExpression="\d\d\/\d\d\/\d\d\d\d"
                    ErrorMessage="The data format must be something like: '03/10/2024'"
                    ControlToValidate="txtbEmployeeBirthDate">
                </asp:RegularExpressionValidator>
            </div>

            <div id="employeeEntryDateDiv">
                <label>Fecha de Ingreso</label>
                <asp:TextBox ID="txtbEmployeeEntryDate" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator
                    ForeColor="red"
                    ID="rqdtxtbEmployeeEntryDate" 
                    runat="server"
                    Text="*"
                    ErrorMessage="The employee entry date is required" 
                    ControlToValidate="txtbEmployeeEntryDate">
                </asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator 
                    ID="regtxtbEmployeeEntryDate" 
                    runat="server"
                    ForeColor="green"
                    ValidationExpression="\d\d\/\d\d\/\d\d\d\d"
                    ErrorMessage="The data format must be something like: '03/10/2024'"
                    ControlToValidate="txtbEmployeeEntryDate">
                </asp:RegularExpressionValidator>
            </div>

            <br />

            <label>Sexo</label>
            <asp:RadioButtonList ID="rblEmployeeGender" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Selected="True" Value="H">Hombre</asp:ListItem>
                <asp:ListItem Value="M">Mujer</asp:ListItem>
            </asp:RadioButtonList>

            <span></span>
            <span></span>
            <br />

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

        <asp:ValidationSummary 
            ID="employeValidationSummary" 
            runat="server" 
            ForeColor="red"
            Display="list"
         />

    </form>
</body>
</html>