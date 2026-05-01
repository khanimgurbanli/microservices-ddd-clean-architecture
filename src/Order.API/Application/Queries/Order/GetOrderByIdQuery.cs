using MediatR;
using Order.API.Application.Models.Orders;
using Shared.Errors;

namespace Order.API.Application.Queries.Order;

public record GetOrderByIdQuery(Guid Id) : IRequest<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>;
