namespace Order.API.Infrastructure.Database.Entities;

public class OrderItemEntity
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
}