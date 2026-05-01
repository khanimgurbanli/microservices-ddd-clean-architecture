using MediatR;
using Order.API.Application.Commands.Order.Models;
using Order.API.Application.Models.Orders;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public class CreateOrderCommand : IRequest<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>
{
    public Guid BuyerId { get; set; }
    public List<CreateOrderItemRequest> OrderItems { get; set; } = new();
}
