using MediatR;
using Shared.Errors;

namespace Stock.API.Application.Commands.Stock;

public record ReserveStockCommand(Guid ProductId, int Quantity) : IRequest<CSharpFunctionalExtensions.Result<bool, DomainError>>;
