namespace Order.API.Application.Models.Orders;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}
