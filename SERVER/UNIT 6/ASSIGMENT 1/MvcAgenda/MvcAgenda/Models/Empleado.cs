using System.ComponentModel.DataAnnotations;

namespace MvcAgenda.Models
{
    public class Empleado
    {
        // PROPIEDAD QUE ACTÚA COMO CLAVE PRIMARIA PARA IDENTIFICAR UN EMPLEADO
        public int Id { get; set; }

        // ---------------------------------------------------------------------------- //

        // VALIDACIÓN QUE INDICA QUE ESTE CAMPO ES OBLIGATORIO
        [Required(ErrorMessage = "El nombre del empleado es un campo requerido.")]

            // PROPIEDAD OPCIONAL PARA ALMACENAR EL NOMBRE DEL EMPLEADO
            public string? Nombre { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA LA FECHA DE NACIMIENTO EN LA INTERFAZ
        [Display(Name = "Fecha de nacimiento")]
        // FORMATEA LA PROPIEDAD COMO UNA FECHA
        [DataType(DataType.Date)]

            // PROPIEDAD QUE ALMACENA LA FECHA DE NACIMIENTO DEL EMPLEADO
            public DateTime? FechaNacimiento { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA EL TELÉFONO EN LA INTERFAZ
        [Display(Name = "Teléfono")]

            // PROPIEDAD QUE ALMACENA EL TELÉFONO DEL EMPLEADO
            public string? Teléfono { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA EL EMAIL EN LA INTERFAZ
        [Display(Name = "Correo electrónico")]
        // VALIDACIÓN QUE COMPRUEBA EL FORMATO DE CORREO, Y ESPECIFICA SU MENSAJE DE ERROR
        [EmailAddress(ErrorMessage = "Dirección de correo electrónico invalida")]

            // PROPIEDAD QUE ALMACENA EL EMAIL DEL EMPLEADO
            public string? Email { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA EL DEPARTAMENTO EN LA INTERFAZ
        [Display(Name = "Departamento")]

            // PROPIEDAD QUE ACTÚA COMO CLAVE FORÁNEA PARA RELACIONAR UN EMPLEADO CON UN DEPARTAMENTO
            public int DepartamentoId { get; set; }

        // ---------------------------------------------------------------------------- //

        // PROPIEDAD DE NAVEGACIÓN QUE PERMITE ACCEDER AL OBJETO DEPARTAMENTO RELACIONADO
        public Departamento? Departamento { get; set; }

        // ---------------------------------------------------------------------------- //

        // COLECCIÓN QUE PERMITE RELACIONAR EL EMPLEADO CON MÚLTIPLES TAREAS
        public ICollection<Tarea>? Tareas { get; set; }
    }
}
