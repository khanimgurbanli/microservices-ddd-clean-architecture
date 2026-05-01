using MediatR;
using Stock.API.Application.Interfaces;
using Stock.API.Application.Mappers;
using Stock.API.Application.Models.Stock;
using Stock.API.Domain.Aggregates;
using Shared.Errors;

namespace Stock.API.Application.Commands.Stock;

public class CreateStockCommandHandler : IRequestHandler<CreateStockCommand, CSharpFunctionalExtensions.Result<StockResponse, DomainError>>
{
    private readonly IStockService _stockService;

    public CreateStockCommandHandler(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<CSharpFunctionalExtensions.Result<StockResponse, DomainError>> Handle(CreateStockCommand request, CancellationToken cancellationToken)
    {
        var aggregate = StockAggregate.Create(request.ProductId, request.Count);
        var result = await _stockService.CreateAsync(aggregate);
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<StockResponse, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<StockResponse, DomainError>(StockMapper.MapToResponse(result.Value));
    }
}
