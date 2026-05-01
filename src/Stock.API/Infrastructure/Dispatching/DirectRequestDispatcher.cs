using CSharpFunctionalExtensions;
using Shared.Abstractions.Dispatching;
using Shared.Errors;
using Stock.API.Application.Commands.Stock;
using Stock.API.Application.Interfaces;
using Stock.API.Application.Mappers;
using Stock.API.Application.Models.Stock;
using Stock.API.Application.Queries.Stock;
using Stock.API.Domain.Aggregates;
using Stock.API.Domain.ValueObjects;

namespace Stock.API.Infrastructure.Dispatching;

public class DirectRequestDispatcher : IRequestDispatcher
{
    private readonly IStockService _stockService;

    public DirectRequestDispatcher(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        object response = request switch
        {
            CreateStockCommand cmd => await Handle(cmd),
            ReserveStockCommand cmd => await Handle(cmd),
            GetAllStocksQuery query => await Handle(query),
            GetStockByProductIdQuery query => await Handle(query),
            _ => throw new NotSupportedException($"Unsupported request type: {request.GetType().FullName}")
        };

        return (TResponse)response;
    }

    private async Task<Result<StockResponse, DomainError>> Handle(CreateStockCommand command)
    {
        var aggregate = StockAggregate.Create(command.ProductId, command.Count);
        var result = await _stockService.CreateAsync(aggregate);
        if (result.IsFailure)
            return Result.Failure<StockResponse, DomainError>(result.Error);

        return Result.Success<StockResponse, DomainError>(StockMapper.MapToResponse(result.Value));
    }

    private async Task<Result<bool, DomainError>> Handle(ReserveStockCommand command)
    {
        var result = await _stockService.ReserveAsync(ProductId.From(command.ProductId), Quantity.From(command.Quantity));
        if (result.IsFailure)
            return Result.Failure<bool, DomainError>(result.Error);

        return Result.Success<bool, DomainError>(true);
    }

    private async Task<Result<IEnumerable<StockResponse>, DomainError>> Handle(GetAllStocksQuery query)
    {
        var result = await _stockService.GetAllAsync();
        if (result.IsFailure)
            return Result.Failure<IEnumerable<StockResponse>, DomainError>(result.Error);

        return Result.Success<IEnumerable<StockResponse>, DomainError>(StockMapper.MapToResponse(result.Value));
    }

    private async Task<Result<StockResponse, DomainError>> Handle(GetStockByProductIdQuery query)
    {
        var result = await _stockService.GetByProductIdAsync(ProductId.From(query.ProductId));
        if (result.IsFailure)
            return Result.Failure<StockResponse, DomainError>(result.Error);

        return Result.Success<StockResponse, DomainError>(StockMapper.MapToResponse(result.Value));
    }
}
