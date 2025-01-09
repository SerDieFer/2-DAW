using Microsoft.EntityFrameworkCore;
using Vestigio.Models;

namespace Vestigio.Data
{
    public class VestigioDBContext : DbContext
    {
        public VestigioDBContext(DbContextOptions<VestigioDBContext> options): base(options) { }

        // DBSETS (TABLES)
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeResolution> ChallengeResolutions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // SINGULAR NAME TABLE
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Challenge>().ToTable("Challenge");
            modelBuilder.Entity<ChallengeResolution>().ToTable("ChallengeResolution");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");

            // DISABLE CASCADING DELETION IN ALL RELATIONSHIPS 
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // SET DECIMAL PRECISION (TO AVOID TRUNCATION ISSUES)
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(9, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(9, 2);

            // SET UP COMPOUND KEY FOR CHALLENGE RESOLUTION (A CHALLENGE CAN ONLY BE SOLVED ONCE PER USER)
            modelBuilder.Entity<ChallengeResolution>()
                .HasIndex(cr => new { cr.UserId, cr.ChallengeId })
                .IsUnique();

            // RELATION: ORDER - ORDER DETAILS (1:N)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId);

            // RELATION: PRODUCT - ORDER DETAILS (1:N)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId);

            // RELATION: PRODUCT - CATEGORY (N:1)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // RELATION: CHALLENGE - PRODUCT (1:1 O 1:N)
            modelBuilder.Entity<Challenge>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .IsRequired(false); // A CHALLENGE MAY OR MAY NOT BE RELATED TO A SPECIFIC PRODUCT.

            // RELATION: USER - ORDERS (1:N)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // RELACIÓN: USER - CHALLENGE RESOLUTION (1:N)
            modelBuilder.Entity<ChallengeResolution>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.ChallengesResolutions)
                .HasForeignKey(cr => cr.UserId);

            // CREATE THE MODEL
            base.OnModelCreating(modelBuilder);
        }
    }
}
