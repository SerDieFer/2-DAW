<%@ Page Title="" Language="C#" MasterPageFile="~/WebForms/MasterPagesForms/MasterPage.Master" AutoEventWireup="true" CodeBehind="MasterPageHome.aspx.cs" Inherits="GesPresta.WebForms.MasterPageDefault" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>INICIO</title>
    <link href="../../CSS/HomeStyleSheet.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div id="homeDiv">
     <p> La corporación ACME está comprometida con sus empleados. Para ello ha establecido una serie de prestaciones 
      que pueden utilizar sus empleados para obtener ayudas sociales asociados a diversos gastos de tipo familiar, médico, etc.</p>
     <p>Esta aplicación a través de Web permite realizar todas las tareas de gestión relacionadas con la prestación de ayudas a los empleados.</p>
     <p>Para cualquier duda o consulta puede contactar con el Departamento de Ayuda Social: 
     <asp:LinkButton ID="LinkButtonSocialHelpMail" runat="server">ayuda.social@acme.com</asp:LinkButton></p>
 </div>
</asp:Content>
