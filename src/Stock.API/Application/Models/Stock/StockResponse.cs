namespace Stock.API.Application.Models.Stock;

public class StockResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Count { get; set; }
}
