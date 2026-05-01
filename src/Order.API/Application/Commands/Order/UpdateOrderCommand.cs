using MediatR;
using Order.API.Application.Models.Orders;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public record UpdateOrderCommand(Guid Id, int OrderStatus) : IRequest<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>;
