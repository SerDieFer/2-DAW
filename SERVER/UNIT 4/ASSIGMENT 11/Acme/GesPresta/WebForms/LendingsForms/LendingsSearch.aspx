<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LendingsSearch.aspx.cs" Inherits="GesPresta.LendingsSearch" %>

<%@ Register Src="~/UserControllers/HeaderController.ascx" TagName="HeaderController" TagPrefix="uc1" %>
<%@ Register Src="~/UserControllers/LendingsSearchController.ascx" TagName="LendingsSearchController" TagPrefix="uc2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>PRESTACIONES</title>
    <link href="../../CSS/LendingStyleSheet.css" rel="stylesheet" />
</head>
<body>
    <form id="formLending" runat="server">

        <uc1:HeaderController ID="HeaderController1" runat="server" />
        <h2>DATOS DE LAS PRESTACIONES</h2>

        <div class="lendingContainer">

            <div id="lendingDataDiv">

                <!-- Fila 1 - Código de la Prestación -->
                <div id="lendingLendCodeDiv" class="lendingRow">
                    <label class="item1">Código Prestación</label>
                    <asp:TextBox ID="txtbLendingCode" runat="server" CssClass="item2"></asp:TextBox>
                    <asp:RequiredFieldValidator
                        ForeColor="red"
                        ID="rqdtxtbLendingCode"
                        Text="*"
                        runat="server"
                        ErrorMessage="The lending code is required"
                        ControlToValidate="txtbLendingCode"
                        CssClass="item3">
                </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
                        ID="regtxtbLendingCode"
                        runat="server"
                        ForeColor="green"
                        ValidationExpression="\d{3}-\d{3}-\d{3}"
                        ErrorMessage="The data format must be something like: '123-456-789'"
                        ControlToValidate="txtbLendingCode"
                        CssClass="item4">
                </asp:RegularExpressionValidator>
                </div>

                <!-- Fila 2 - Descripción -->
                <div id="lendingLendDescriptionDiv" class="lendingRow">
                    <label class="item1">Descripción</label>
                    <asp:TextBox ID="txtbLendingDescription" runat="server" CssClass="item2"></asp:TextBox>
                    <asp:RequiredFieldValidator
                        ForeColor="red"
                        ID="rqdtxtbLendingDescription"
                        runat="server"
                        Text="*"
                        ErrorMessage="The lending description is required"
                        ControlToValidate="txtbLendingDescription"
                        CssClass="item3">
                </asp:RequiredFieldValidator>
                    <span class="item4"></span>
                </div>

                <!-- Fila 3 - Importe Fijo -->
                <div id="lendingLendValueDiv" class="lendingRow">
                    <label class="item1">Importe Fijo</label>
                    <asp:TextBox ID="txtbLendingFixedValue" runat="server" CssClass="item2"></asp:TextBox>
                    <asp:RequiredFieldValidator
                        ForeColor="red"
                        ID="rqdtxtbLendingFixedValue"
                        runat="server"
                        Text="*"
                        ErrorMessage="The lending import is required"
                        ControlToValidate="txtbLendingFixedValue"
                        CssClass="item3">
                </asp:RequiredFieldValidator>
                    <asp:RangeValidator
                        ForeColor="red"
                        ID="rngtxtbLendingFixedValue"
                        runat="server"
                        Type="Double"
                        MaximumValue="500,00"
                        MinimumValue="0,00"
                        ErrorMessage="Values must be between 0.00 and 500.00"
                        ControlToValidate="txtbLendingFixedValue"
                        CssClass="item4">
                </asp:RangeValidator>
                </div>

                <!-- Fila 4 - Porcentaje del Importe -->
                <div id="lendingLendPercentageDiv" class="lendingRow">
                    <label class="item1">Porcentaje del Importe</label>
                    <asp:TextBox ID="txtbLendingPercentage" runat="server" CssClass="item2"></asp:TextBox>
                    <asp:RequiredFieldValidator
                        ForeColor="red"
                        ID="rqdtxtbLendingPercentage"
                        runat="server"
                        Text="*"
                        ErrorMessage="The lending percentage is required"
                        ControlToValidate="txtbLendingPercentage"
                        CssClass="item3">
                </asp:RequiredFieldValidator>
                    <asp:RangeValidator
                        ForeColor="red"
                        ID="rngtxtbLendingPercentage"
                        runat="server"
                        Type="Double"
                        MaximumValue="15,00"
                        MinimumValue="0,00"
                        ErrorMessage="Values must be between 0.00% and 15.00%"
                        ControlToValidate="txtbLendingPercentage"
                        CssClass="item4">
                </asp:RangeValidator>
                </div>

                <!-- Fila 5 - Tipo de Prestación -->
                <div id="lendingLendTypeDiv" class="lendingRow">
                    <label class="item1">Tipo de Prestación</label>
                    <asp:DropDownList ID="ddlLendingType" runat="server" CssClass="item2">
                        <asp:ListItem Selected="True">Dentaria</asp:ListItem>
                        <asp:ListItem>Familiar</asp:ListItem>
                        <asp:ListItem>Ocular</asp:ListItem>
                        <asp:ListItem>Otras</asp:ListItem>
                    </asp:DropDownList>
                    <span class="item3"></span>
                    <span class="item4"></span>
                </div>

            </div>

            <!-- Columna 2 -->
            <div id="extra">
                <uc2:LendingsSearchController ID="LendingsSearchController1" Visible="false" runat="server" />
                <div id="lendingDescriptionButtonsDiv" class="lendingButtonDiv">
                    <asp:Button ID="btnSeeLendings" runat="server" Text="Ver Prestaciones" CausesValidation="false" OnClick="btnSeeLendings_Click" CssClass="button" />
                    <asp:Button ID="btnSelect" runat="server" Text="Seleccionar" CausesValidation="false" Visible="false" OnClick="btnSelect_Click" CssClass="button" />
                </div>
            </div>

        </div>

        <!-- Botón Envío -->
        <div class="lendingDataButtonDiv">
            <asp:Button ID="btnSendDataLending" runat="server" Text="Enviar" CssClass="sendButton" />
        </div>

        <asp:ValidationSummary ID="lendinbgValidationSummary" runat="server" ForeColor="red" Display="list" />

    </form>
</body>
</html>
