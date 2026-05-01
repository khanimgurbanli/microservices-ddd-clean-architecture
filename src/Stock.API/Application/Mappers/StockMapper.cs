using Stock.API.Application.Models.Stock;
using Stock.API.Domain.Aggregates;

namespace Stock.API.Application.Mappers;

public static class StockMapper
{
    public static StockResponse MapToResponse(StockAggregate stock)
    {
        return new StockResponse
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            Count = stock.Count
        };
    }

    public static IEnumerable<StockResponse> MapToResponse(IEnumerable<StockAggregate> stocks)
    {
        return stocks.Select(MapToResponse);
    }
}
