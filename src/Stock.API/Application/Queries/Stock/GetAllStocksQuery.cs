using MediatR;
using Stock.API.Application.Models.Stock;
using Shared.Errors;
using System.Collections.Generic;

namespace Stock.API.Application.Queries.Stock;

public record GetAllStocksQuery : IRequest<CSharpFunctionalExtensions.Result<IEnumerable<StockResponse>, DomainError>>;
