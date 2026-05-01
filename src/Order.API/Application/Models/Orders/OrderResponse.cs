namespace Order.API.Application.Models.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public int OrderStatus { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; } = new();
}
