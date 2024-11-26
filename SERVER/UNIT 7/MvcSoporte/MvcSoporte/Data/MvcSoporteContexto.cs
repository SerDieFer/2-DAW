using Microsoft.EntityFrameworkCore;
using MvcSoporte.Models;

namespace MvcSoporte.Data
{
    public class MvcSoporteContexto : DbContext
    {

        public MvcSoporteContexto(DbContextOptions<MvcSoporteContexto> options) : base(options) { }

        public DbSet<Aviso>? Avisos { get; set; }
        public DbSet<Empleado>? Empleados { get; set; }
        public DbSet<Equipo>? Equipos { get; set; }
        public DbSet<Localizacion>? Localizaciones { get; set; }
        public DbSet<TipoAveria>? TipoAverias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PONER EL NOMBRE DE LAS TABLAS EN SINGULAR 
            modelBuilder.Entity<Aviso>().ToTable("Aviso");
            modelBuilder.Entity<Empleado>().ToTable("Empleado");
            modelBuilder.Entity<Equipo>().ToTable("Equipo");
            modelBuilder.Entity<Localizacion>().ToTable("Localizacion");
            modelBuilder.Entity<TipoAveria>().ToTable("TipoAveria");

            // DESHABILITAR LA ELIMINACIÓN EN CASCADA EN TODAS LAS RELACIONES 
            base.OnModelCreating(modelBuilder);
            var modelBuilderRelationships = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys());
            foreach (var relationship in modelBuilderRelationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

    }
}
