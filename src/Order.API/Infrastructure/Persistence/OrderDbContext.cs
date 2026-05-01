using Microsoft.EntityFrameworkCore;
using Order.API.Domain.Enums;
using Order.API.Infrastructure.Database.Entities;

namespace Order.API.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderItemEntity> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItemEntity>()
            .Property(o => o.Price)
            .HasColumnType("decimal(18,2)");

        base.OnModelCreating(modelBuilder);
    }
}

public static class OrderDbContextInitializer
{
    public static async Task InitializeAsync(OrderDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
    }
}