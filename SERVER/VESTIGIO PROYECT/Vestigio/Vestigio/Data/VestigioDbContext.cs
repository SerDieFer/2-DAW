﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Importante para IdentityDbContext

            // SINGULAR NAME TABLE
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Challenge>().ToTable("Challenge");
            modelBuilder.Entity<ChallengeResolution>().ToTable("ChallengeResolution");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetail");
            modelBuilder.Entity<OrderStatus>().ToTable("OrderStatus");
            modelBuilder.Entity<Image>().ToTable("Image");
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
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            // COMPOUND KEY FOR CHALLENGE RESOLUTION (A CHALLENGE CAN ONLY BE SOLVED ONCE PER USER)
            modelBuilder.Entity<ChallengeResolution>()
                .HasIndex(cr => new { cr.UserId, cr.ChallengeId })
                .IsUnique();

            // RELATION: CHALLENGE - IMAGE (1:N)
            modelBuilder.Entity<Challenge>()
                .HasMany(c => c.Images)
                .WithOne(i => i.Challenge)
                .HasForeignKey(i => i.ChallengeId)
                .OnDelete(DeleteBehavior.Cascade); // Habilitar eliminación en cascada

            // RELATION: PRODUCT - ORDER DETAILS (1:N)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
             .HasMany(p => p.ProductSizes)
             .WithOne(ps => ps.Product)
             .HasForeignKey(ps => ps.ProductId)
             .OnDelete(DeleteBehavior.Cascade); // Eliminar tallas al borrar el producto

            // Configurar relación Product ↔ ProductCategory (N:N)
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar relaciones al borrar el producto

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar categorías al borrar relaciones

            // Configurar relación ProductSize ↔ OrderDetail (1:N)
            modelBuilder.Entity<ProductSize>()
                .HasMany(ps => ps.OrderDetails)
                .WithOne(od => od.ProductSize)
                .HasForeignKey(od => od.ProductSizeId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar tallas si hay pedidos asociados

            // RELATION: PRODUCT - PRODUCT CATEGORY (MANY-TO-MANY)
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<ProductSize>()
                .HasIndex(ps => new { ps.ProductId, ps.Size })
                .IsUnique();

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductSize)
                .WithMany(ps => ps.OrderDetails)
                .HasForeignKey(od => od.ProductSizeId)
                .OnDelete(DeleteBehavior.Cascade);

            // RELATION: ORDER - ORDER STATUS (1:N)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey(o => o.OrderStatusId);

            // RELATION: ORDER - USER (1:N)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // RELATION: ORDER - ORDER DETAILS (1:N)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
