using System.ComponentModel.DataAnnotations;

namespace MvcAgenda.Models
{
    public class Departamento
    {
        // PROPIEDAD QUE ACTÚA COMO CLAVE PRIMARIA PARA IDENTIFICAR UN DEPARTAMENTO
        public int Id { get; set; }

        // ---------------------------------------------------------------------------- //

        // VALIDACIÓN QUE INDICA QUE ESTE CAMPO ES OBLIGATORIO
        [Required(ErrorMessage = "El nombre del departamento es un campo requerido.")]

            // PROPIEDAD OPCIONAL PARA ALMACENAR EL NOMBRE DEL DEPARTAMENTO
            public string? Nombre { get; set; }

        // ---------------------------------------------------------------------------- //

        // COLECCIÓN QUE PERMITE RELACIONAR EL DEPARTAMENTO CON MÚLTIPLES EMPLEADOS
        public ICollection<Empleado>? Empleados { get; set; }
    }
}
    