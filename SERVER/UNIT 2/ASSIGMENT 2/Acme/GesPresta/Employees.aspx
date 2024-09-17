<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="GesPresta.Employees" %>
<%@ Register src="HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>DATOS DE LOS EMPLEADOS</title>
    <link href="EmployeeStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formEmployee" runat="server">

        <uc1:HeaderController ID="HeaderController1" runat="server" />
        <h2>DATOS DE LOS EMPLEADOS</h2>

        <div id="employeeDataDiv">

            <asp:Label ID="lblEmployeeCode" runat="server">Código Empleado</asp:Label>
            <asp:TextBox ID="txtEmployeeCode" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeNIF" runat="server">NIF</asp:Label>
            <asp:TextBox ID="txtEmployeeNIF" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeFullName" runat="server">Apellidos y Nombre</asp:Label>
            <asp:TextBox ID="txtEmployeeFullName" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeAdress" runat="server">Dirección</asp:Label>
            <asp:TextBox ID="txtEmployeeAdress" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeCity" runat="server">Ciudad</asp:Label>
            <asp:TextBox ID="txtEmployeeCity" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeePhones" runat="server">Teléfonos</asp:Label>
            <asp:TextBox ID="txtEmployeePhones" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeBirthDate" runat="server">Fecha de Nacimiento</asp:Label>
            <asp:TextBox ID="txtEmployeeBirthDate" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeEntryDate" runat="server">Fecha de Ingreso</asp:Label>
            <asp:TextBox ID="txtEmployeeEntryDate" runat="server"></asp:TextBox>

            <asp:Label ID="lblEmployeeGender" runat="server">Sexo</asp:Label>
            <asp:RadioButtonList ID="rblEmployeeGender" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Selected="True" Value="H">Hombre</asp:ListItem>
                <asp:ListItem Value="M">Mujer</asp:ListItem>
            </asp:RadioButtonList>

            <asp:Label ID="lblEmployeeDepartment" runat="server">Departamento</asp:Label>
            <asp:DropDownList ID="ddlEmployeeDepartment" runat="server">
                <asp:ListItem Selected="True">Investigación</asp:ListItem>
                <asp:ListItem>Desarrollo</asp:ListItem>
                <asp:ListItem>Innovación</asp:ListItem>
                <asp:ListItem>Administración</asp:ListItem>
            </asp:DropDownList>

        </div>

        <div class="button-group">
            <asp:Button ID="btnSendDataEmployee" runat="server" Text="Enviar" />
        </div>

    </form>
</body>
</html>