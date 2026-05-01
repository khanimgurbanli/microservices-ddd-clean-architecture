using MediatR;
using Stock.API.Application.Models.Stock;
using Shared.Errors;

namespace Stock.API.Application.Queries.Stock;

public record GetStockByProductIdQuery(Guid ProductId) : IRequest<CSharpFunctionalExtensions.Result<StockResponse, DomainError>>;
