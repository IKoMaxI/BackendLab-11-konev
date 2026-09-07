using Microsoft.EntityFrameworkCore;
using StoreApiLR11.Models;

namespace StoreApiLR11.Data;

public class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(order => order.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.Order)
            .WithMany(order => order.OrderItems)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.Product)
            .WithMany(product => product.OrderItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>().HasQueryFilter(entity => !entity.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(entity => !entity.IsDeleted);
        modelBuilder.Entity<Customer>().HasQueryFilter(entity => !entity.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(entity => !entity.IsDeleted);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(entity => !entity.IsDeleted);

        modelBuilder.Entity<Product>().Property(product => product.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(order => order.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(item => item.Price).HasPrecision(18, 2);
    }
}
