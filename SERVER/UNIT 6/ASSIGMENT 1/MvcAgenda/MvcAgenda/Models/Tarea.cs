using System.ComponentModel.DataAnnotations;

namespace MvcAgenda.Models
{
    public class Tarea
    {
        // PROPIEDAD QUE ACTÚA COMO CLAVE PRIMARIA PARA IDENTIFICAR UNA TAREA
        public int Id { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE QUE SE MOSTRARÁ EN LA INTERFAZ DE USUARIO
        [Display(Name = "Descripción")]

            // PROPIEDAD OPCIONAL PARA ALMACENAR UNA DESCRIPCIÓN DE LA TAREA
            public string? Descripcion { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA LA FECHA DE INICIO EN LA INTERFAZ
        [Display(Name = "Fecha de inicio")]
        // VALIDACIÓN QUE INDICA QUE ESTE CAMPO ES OBLIGATORIO
        [Required(ErrorMessage = "La fecha de inicio es un campo requerido.")]
        // FORMATEA LA PROPIEDAD COMO UNA FECHA
        [DataType(DataType.Date)]

            // PROPIEDAD QUE ALMACENA LA FECHA DE INICIO DE LA TAREA
            public DateTime FechaInicio { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA LA FECHA DE FIN EN LA INTERFAZ
        [Display(Name = "Fecha de fin")]
        // FORMATEA LA PROPIEDAD COMO UNA FECHA
        [DataType(DataType.Date)]

            // PROPIEDAD OPCIONAL PARA ALMACENAR LA FECHA DE FIN DE LA TAREA
            public DateTime? FechaFin { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA LA HORA DE INICIO EN LA INTERFAZ
        [Display(Name = "Hora de inicio")]
        // VALIDACIÓN QUE INDICA QUE ESTE CAMPO ES OBLIGATORIO
        [Required(ErrorMessage = "La hora de inicio es un campo requerido.")]
        // FORMATEA LA PROPIEDAD COMO UNA HORA
        [DataType(DataType.Time)]

            // PROPIEDAD QUE ALMACENA LA HORA DE INICIO DE LA TAREA
            public DateTime HoraInicio { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA LA HORA DE FIN EN LA INTERFAZ
        [Display(Name = "Hora de fin")]
        // FORMATEA LA PROPIEDAD COMO UNA HORA
        [DataType(DataType.Time)]

            // PROPIEDAD OPCIONAL PARA ALMACENAR LA HORA DE FIN DE LA TAREA
            public DateTime? HoraFin { get; set; }

        // ---------------------------------------------------------------------------- //

        // ESPECIFICA EL NOMBRE PARA EL EMPLEADO EN LA INTERFAZ
        [Display(Name = "Empleado")]

            // PROPIEDAD QUE ACTÚA COMO CLAVE FORÁNEA PARA RELACIONAR UNA TAREA CON UN EMPLEADO
            public int EmpleadoId { get; set; }

        // ---------------------------------------------------------------------------- //

        // PROPIEDAD DE NAVEGACIÓN QUE PERMITE ACCEDER AL OBJETO EMPLEADO RELACIONADO
        public Empleado? Empleado { get; set; } 
    }
}