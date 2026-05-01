using Order.API.Domain.Enums;

namespace Order.API.Infrastructure.Database.Entities;

public class OrderEntity
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<OrderItemEntity> OrderItems { get; set; } = new();
}