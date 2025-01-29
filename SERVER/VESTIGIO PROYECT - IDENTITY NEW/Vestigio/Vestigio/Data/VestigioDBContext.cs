using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vestigio.Models;

namespace Vestigio.Data
{
    public class VestigioDbContext : IdentityDbContext<User>
    {
        public VestigioDbContext(DbContextOptions<VestigioDbContext> options) : base(options) { }

        // DBSETS (TABLES)
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<ChallengeResolution> ChallengeResolutions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Image> Images { get; set; }
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
            modelBuilder.Entity<Image>().ToTable("Images");
            modelBuilder.Entity<ProductSize>().ToTable("ProductSize");
            modelBuilder.Entity<ProductCategory>().ToTable("ProductCategory");

            // DISABLE CASCADING DELETION IN ALL RELATIONSHIPS 
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // SET DECIMAL PRECISION
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

            // RELATION: PRODUCT - PRODUCT CATEGORY (MANY-TO-MANY)
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                    .OnDelete(DeleteBehavior.Cascade); // CASCADE DELETES IMAGES WHEN A PRODUCT IS DELETED.

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);
                    

            // RELATION: PRODUCT - PRODUCT SIZE (1:N)
            modelBuilder.Entity<ProductSize>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.Sizes)
                .HasForeignKey(ps => ps.ProductId)
                  .OnDelete(DeleteBehavior.Cascade); // CASCADE DELETES IMAGES WHEN A PRODUCT IS DELETED.

            // Product-Size relationship
            modelBuilder.Entity<ProductSize>()
                .HasKey(ps => new { ps.ProductId, ps.Size });

            // RELATION: CHALLENGE - PRODUCT (1:1 OR 1:N)
            modelBuilder.Entity<Challenge>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Challenges)
                .HasForeignKey(c => c.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // RELATION: USER - ORDERS (1:N)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // RELATION: USER - CHALLENGE RESOLUTION (1:N)
            modelBuilder.Entity<ChallengeResolution>()
                .HasOne(cr => cr.User)
                .WithMany(u => u.ChallengesResolutions)
                .HasForeignKey(cr => cr.UserId);

            // RELATION: PRODUCT - IMAGE (1:N)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // CASCADE DELETES IMAGES WHEN A PRODUCT IS DELETED.

            // RELATION: CHALLENGE - IMAGE (1:N)
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Challenge)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade); // CASCADE DELETES IMAGES WHEN A CHALLENGE IS DELETED.


            base.OnModelCreating(modelBuilder);
        }
    }
}