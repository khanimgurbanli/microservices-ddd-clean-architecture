using CSharpFunctionalExtensions;
using Stock.API.Domain.Aggregates;
using Stock.API.Domain.ValueObjects;
using Shared.Errors;

namespace Stock.API.Application.Interfaces;

public interface IStockService
{
    Task<Result<StockAggregate, DomainError>> CreateAsync(StockAggregate stock);
    Task<Result<StockAggregate, DomainError>> GetByProductIdAsync(ProductId productId);
    Task<Result<IEnumerable<StockAggregate>, DomainError>> GetAllAsync();
    Task<Result<StockAggregate, DomainError>> ReserveAsync(ProductId productId, Quantity quantity);
    Task<Result<IEnumerable<StockAggregate>, DomainError>> ReserveForOrderAsync(IEnumerable<StockReservationItem> items);
}
