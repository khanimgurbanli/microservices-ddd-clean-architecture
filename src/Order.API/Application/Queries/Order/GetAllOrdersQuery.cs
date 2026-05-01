using MediatR;
using Order.API.Application.Models.Orders;
using Shared.Errors;
using System.Collections.Generic;

namespace Order.API.Application.Queries.Order;

public record GetAllOrdersQuery : IRequest<CSharpFunctionalExtensions.Result<IEnumerable<OrderResponse>, DomainError>>;
