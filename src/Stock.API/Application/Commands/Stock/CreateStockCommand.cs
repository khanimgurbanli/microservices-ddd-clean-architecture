using MediatR;
using Stock.API.Application.Models.Stock;
using Shared.Errors;

namespace Stock.API.Application.Commands.Stock;

public record CreateStockCommand(Guid ProductId, int Count) : IRequest<CSharpFunctionalExtensions.Result<StockResponse, DomainError>>;
