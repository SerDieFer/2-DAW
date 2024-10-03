<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LendingsSearch.aspx.cs" Inherits="GesPresta.LendingsSearch" %>
<%@ Register src="~/UserControllers/HeaderController.ascx" tagname="HeaderController" tagprefix="uc1" %>
<%@ Register src="~/UserControllers/LendingsSearchController.ascx" tagname="LendingsSearchController" tagprefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>PRESTACIONES</title>
    <link href="../../CSS/LendingStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formLending" runat="server">

         <uc1:HeaderController ID="HeaderController1" runat="server" />
         <h2>DATOS DE LAS PRESTACIONES</h2>

         <div id="lendingDataDiv">

                 <label>Código Prestación</label>
                 <asp:TextBox ID="txtbLendingCode" runat="server"></asp:TextBox>
                 <asp:RequiredFieldValidator 
                    ForeColor="red"
                    ID="rqdtxtbLendingCode"
                    Text="*"
                    runat="server" 
                    ErrorMessage="The lending code is required" 
                    ControlToValidate="txtbLendingCode">
                 </asp:RequiredFieldValidator>
                 <asp:RegularExpressionValidator 
                    ID="regtxtbLendingCode" 
                    runat="server" 
                    ForeColor="green"
                    ValidationExpression="\d{3}-\d{3}-\d{3}"
                    ErrorMessage="The data format must be something like: '123-456-789'"
                    ControlToValidate="txtbLendingCode">
                </asp:RegularExpressionValidator>

                 <uc2:LendingsSearchController ID="LendingsSearchController1" Visible="false" runat="server" />
                 <br />
                 <label>Descripción</label>
                 <asp:TextBox ID="txtbLendingDescription" runat="server"></asp:TextBox>
                 <asp:RequiredFieldValidator 
                    ForeColor="red"
                    ID="rqdtxtbLendingDescription"
                    runat="server"
                    Text="*"
                    ErrorMessage="The lending description is required" 
                    ControlToValidate="txtbLendingDescription">
                 </asp:RequiredFieldValidator>
                
                 <asp:Button ID="btnSelect" runat="server" Text="Seleccionar" CausesValidation="false" Visible="false" OnClick="btnSelect_Click" />
                 <asp:Button ID="btnSeeLendings" runat="server" Text="Ver Prestaciones" CausesValidation="false" OnClick="btnSeeLendings_Click" />
                 <br />

             <label>Importe Fijo</label>
             <asp:TextBox ID="txtbLendingFixedValue" runat="server"></asp:TextBox>
             <asp:RequiredFieldValidator 
                ForeColor="red"
                ID="rqdtxtbLendingFixedValue"
                runat="server" 
                Text="*"
                ErrorMessage="The lending import is required" 
                ControlToValidate="txtbLendingFixedValue">
             </asp:RequiredFieldValidator>
             <asp:RangeValidator
                 ForeColor="red"
                 ID="rngtxtbLendingFixedValue" 
                 runat="server"
                 Type="Double"
                 MaximumValue="500,00" 
                 MinimumValue="0,00"
                 ErrorMessage="Values must be between 0.00 and 500.00"
                 ControlToValidate="txtbLendingFixedValue">
             </asp:RangeValidator>

             <br />

             <label>Porcentaje del Importe</label>
             <asp:TextBox ID="txtbLendingPercentage" runat="server"></asp:TextBox>
             <asp:RequiredFieldValidator 
                ForeColor="red"
                ID="rqdtxtbLendingPercentage"
                runat="server" 
                Text="*"
                ErrorMessage="The lending percentage is required" 
                ControlToValidate="txtbLendingPercentage">
             </asp:RequiredFieldValidator>
             <asp:RangeValidator 
                 ForeColor="red"
                 ID="rngtxtbLendingPercentage" 
                 runat="server" 
                 Type="Double"
                 MaximumValue="15,00" 
                 MinimumValue="0,00"
                 ErrorMessage="Values must be between 0.00% and 15.00%"
                 ControlToValidate="txtbLendingPercentage">
             </asp:RangeValidator>

             <br />

             <label>Tipo de Prestación</label>
             <asp:DropDownList ID="ddlLendingType" runat="server">
                 <asp:ListItem Selected="True">Dentaria</asp:ListItem>
                 <asp:ListItem>Familiar</asp:ListItem>
                 <asp:ListItem>Ocular</asp:ListItem>
                 <asp:ListItem>Otras</asp:ListItem>
             </asp:DropDownList>

         </div>

         <div class="lendingDataButtonDiv">
             <asp:Button ID="btnSendDataLending" runat="server" Text="Enviar" />
         </div>

        <asp:ValidationSummary 
            ID="lendinbgValidationSummary" 
            runat="server" 
            ForeColor="red"
            Display="list"
         />
    </form>
</body>
</html>
