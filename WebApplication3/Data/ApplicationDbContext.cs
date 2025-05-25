using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important: This configures the Identity models

            // Configure decimal precision
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            // Seed initial product data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Espresso",
                    Description = "Strong Italian coffee",
                    Price = 3.50m,
                    ImageUrl = "/images/espresso.jpg",
                    Category = "Coffee",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Cappuccino",
                    Description = "Espresso with steamed milk and foam",
                    Price = 4.20m,
                    ImageUrl = "/images/cappuccino.jpg",
                    Category = "Coffee",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 3,
                    Name = "Latte",
                    Description = "Espresso with steamed milk",
                    Price = 4.50m,
                    ImageUrl = "/images/latte.jpg",
                    Category = "Coffee",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 4,
                    Name = "Croissant",
                    Description = "Buttery and flaky pastry",
                    Price = 2.50m,
                    ImageUrl = "/images/croissant.jpg",
                    Category = "Pastry",
                    IsAvailable = true
                },
                new Product
                {
                    Id = 5,
                    Name = "Muffin",
                    Description = "Chocolate chip muffin",
                    Price = 3.00m,
                    ImageUrl = "/images/muffin.jpg",
                    Category = "Pastry",
                    IsAvailable = true
                }
            );
        }
    }
} 