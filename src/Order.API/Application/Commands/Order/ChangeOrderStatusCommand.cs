using MediatR;
using Order.API.Application.Models.Orders;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public record ChangeOrderStatusCommand(Guid Id, int Status) : IRequest<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>;
