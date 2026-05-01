namespace Order.API.Application.Commands.Order.Models;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}
