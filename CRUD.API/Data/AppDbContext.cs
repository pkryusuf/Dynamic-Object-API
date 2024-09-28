using CRUD.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderProduct>().ToTable("OrderProduct");

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)                     
                .WithMany(o => o.OrderProducts)             
                .HasForeignKey(op => op.OrderId)            
                .OnDelete(DeleteBehavior.Cascade);          //cascade delete

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)                    
                .WithMany(c => c.Orders)                    
                .HasForeignKey(o => o.CustomerId)           
                .OnDelete(DeleteBehavior.Restrict);         

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)                   
                .WithMany()                                 
                .HasForeignKey(op => op.ProductId)          
                .OnDelete(DeleteBehavior.Restrict);         
        }
    }
}
