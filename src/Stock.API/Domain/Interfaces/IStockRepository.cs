using Stock.API.Domain.Aggregates;
using Stock.API.Domain.ValueObjects;

namespace Stock.API.Domain.Interfaces;

public interface IStockRepository
{
    Task<StockAggregate?> GetByIdAsync(StockId id);
    Task<StockAggregate?> GetByProductIdAsync(ProductId productId);
    Task<IEnumerable<StockAggregate>> GetAllAsync();
    Task AddAsync(StockAggregate stock);
    Task UpdateAsync(StockAggregate stock);
    Task DeleteAsync(StockId id);
}
