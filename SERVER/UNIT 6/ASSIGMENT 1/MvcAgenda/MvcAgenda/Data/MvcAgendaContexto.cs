using Microsoft.EntityFrameworkCore;
using MvcAgenda.Models;

namespace MvcAgenda.Data
{
    public class MvcAgendaContexto : DbContext
    {
        // CONSTRUCTOR QUE RECIBE OPCIONES DE DbContext Y LAS PASA A LA CLASE BASE (PADRE)
        public MvcAgendaContexto(DbContextOptions<MvcAgendaContexto> options) : base(options) { }

        // ---------------------------------------------------------------------------- //

        // CONJUNTO DE DATOS QUE REPRESENTA LA TABLA DEPARTAMENTOS EN LA BASE DE DATOS
        public DbSet<Departamento>? Departamentos { get; set; }

        // CONJUNTO DE DATOS QUE REPRESENTA LA TABLA EMPLEADOS EN LA BASE DE DATOS
        public DbSet<Empleado>? Empleados { get; set; }

        // CONJUNTO DE DATOS QUE REPRESENTA LA TABLA TAREAS EN LA BASE DE DATOS
        public DbSet<Tarea>? Tareas { get; set; }

        // ---------------------------------------------------------------------------- //

        // MÉTODO OVERRIDE QUE CONFIGURA EL MODELO DE LA BASE DE DATOS CUANDO SE CREA
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            // DESACTIVAR ELIMINACIÓN EN CASCADA Y ASIGNAR UN COMPORTAMIENTO RESTRICTIVO PARA LAS RELACIONES

            // LLAMA AL MÉTODO BASE
            base.OnModelCreating(modelBuilder);

            // ITERA SOBRE TODAS LAS RELACIONES DE CLAVE FORÁNEA
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                // ASIGNA UN COMPORTAMIENTO RESTRICTIVO AL ELIMINAR UNA ENTIDAD RELACIONADA
                relationship.DeleteBehavior = DeleteBehavior.Restrict; 
            }
        }
    }
}