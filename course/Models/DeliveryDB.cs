using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace course.Models
{
    public class DeliveryDBContext : DbContext
    {
        public DeliveryDBContext(DbContextOptions<DeliveryDBContext> options) : base(options)
        {
        }
        public DbSet<Order> orders { get; set; }
        public DbSet<Products> product { get; set; }
        public DbSet<ProductsHistory> producthistory { get; set; }
        public DbSet<Cart> cart { get; set; }
        public DbSet<History> history { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsHistory>()
                .HasMany(e => e.Cart)
                .WithOne(e => e.ProductsHistory)
                .HasForeignKey(e => e.ProductsHistoryID)
                .IsRequired();
        }
    }
}