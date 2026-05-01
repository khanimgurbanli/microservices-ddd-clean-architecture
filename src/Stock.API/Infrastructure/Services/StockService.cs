using CSharpFunctionalExtensions;
using Stock.API.Application.Interfaces;
using Stock.API.Domain.Aggregates;
using Stock.API.Domain.Interfaces;
using Stock.API.Domain.ValueObjects;
using Shared.Errors;

namespace Stock.API.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Result<StockAggregate, DomainError>> CreateAsync(StockAggregate stock)
    {
        await _stockRepository.AddAsync(stock);

        return Result.Success<StockAggregate, DomainError>(stock);
    }

    public async Task<Result<StockAggregate, DomainError>> GetByProductIdAsync(ProductId productId)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            return Result.Failure<StockAggregate, DomainError>(DomainError.NotFound($"Stock for product {productId.Value} not found"));

        return Result.Success<StockAggregate, DomainError>(stock);
    }

    public async Task<Result<IEnumerable<StockAggregate>, DomainError>> GetAllAsync()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return Result.Success<IEnumerable<StockAggregate>, DomainError>(stocks);
    }

    public async Task<Result<StockAggregate, DomainError>> ReserveAsync(ProductId productId, Quantity quantity)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            return Result.Failure<StockAggregate, DomainError>(DomainError.NotFound($"Stock for product {productId.Value} not found"));

        var success = stock.ReserveStock(quantity.Value);
        if (!success)
            return Result.Failure<StockAggregate, DomainError>(DomainError.BadRequest("Insufficient stock"));

        await _stockRepository.UpdateAsync(stock);
        return Result.Success<StockAggregate, DomainError>(stock);
    }

    public async Task<Result<IEnumerable<StockAggregate>, DomainError>> ReserveForOrderAsync(IEnumerable<StockReservationItem> items)
    {
        var materialized = items.ToList();
        if (!materialized.Any())
            return Result.Success<IEnumerable<StockAggregate>, DomainError>(Array.Empty<StockAggregate>());

        var stocks = new Dictionary<Guid, StockAggregate>();

        foreach (var item in materialized)
        {
            var stock = await _stockRepository.GetByProductIdAsync(item.ProductId);
            if (stock == null || stock.Count < item.Quantity.Value)
                return Result.Failure<IEnumerable<StockAggregate>, DomainError>(DomainError.BadRequest("Insufficient stock"));

            stocks[item.ProductId.Value] = stock;
        }

        foreach (var item in materialized)
        {
            var stock = stocks[item.ProductId.Value];
            var success = stock.ReserveStock(item.Quantity.Value);
            if (!success)
                return Result.Failure<IEnumerable<StockAggregate>, DomainError>(DomainError.BadRequest("Insufficient stock"));
        }

        foreach (var stock in stocks.Values)
        {
            await _stockRepository.UpdateAsync(stock);
        }

        return Result.Success<IEnumerable<StockAggregate>, DomainError>(stocks.Values.ToList());
    }
}
