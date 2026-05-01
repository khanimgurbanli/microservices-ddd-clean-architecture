using MediatR;
using Stock.API.Application.Interfaces;
using Stock.API.Domain.ValueObjects;
using Shared.Errors;

namespace Stock.API.Application.Commands.Stock;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, CSharpFunctionalExtensions.Result<bool, DomainError>>
{
    private readonly IStockService _stockService;

    public ReserveStockCommandHandler(IStockService stockService)
    {
        _stockService = stockService;
    }

    public async Task<CSharpFunctionalExtensions.Result<bool, DomainError>> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var result = await _stockService.ReserveAsync(ProductId.From(request.ProductId), Quantity.From(request.Quantity));
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<bool, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<bool, DomainError>(true);
    }
}
