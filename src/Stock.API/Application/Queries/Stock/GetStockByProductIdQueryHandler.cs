using MediatR;
using Stock.API.Application.Interfaces;
using Stock.API.Application.Mappers;
using Stock.API.Application.Models.Stock;
using Stock.API.Domain.ValueObjects;
using Shared.Errors;

namespace Stock.API.Application.Queries.Stock;

public class GetStockByProductIdQueryHandler : IRequestHandler<GetStockByProductIdQuery, CSharpFunctionalExtensions.Result<StockResponse, DomainError>>
{
    private readonly IStockService _stockService;

    public GetStockByProductIdQueryHandler(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<CSharpFunctionalExtensions.Result<StockResponse, DomainError>> Handle(GetStockByProductIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _stockService.GetByProductIdAsync(ProductId.From(request.ProductId));
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<StockResponse, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<StockResponse, DomainError>(StockMapper.MapToResponse(result.Value));
    }
}
