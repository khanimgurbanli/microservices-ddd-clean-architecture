using MediatR;
using Stock.API.Application.Interfaces;
using Stock.API.Application.Mappers;
using Stock.API.Application.Models.Stock;
using Shared.Errors;
using System.Collections.Generic;

namespace Stock.API.Application.Queries.Stock;

public class GetAllStocksQueryHandler : IRequestHandler<GetAllStocksQuery, CSharpFunctionalExtensions.Result<IEnumerable<StockResponse>, DomainError>>
{
    private readonly IStockService _stockService;

    public GetAllStocksQueryHandler(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<CSharpFunctionalExtensions.Result<IEnumerable<StockResponse>, DomainError>> Handle(GetAllStocksQuery request, CancellationToken cancellationToken)
    {
        var result = await _stockService.GetAllAsync();
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<IEnumerable<StockResponse>, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<IEnumerable<StockResponse>, DomainError>(StockMapper.MapToResponse(result.Value));
    }
}
