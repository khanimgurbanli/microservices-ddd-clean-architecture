using MediatR;
using Order.API.Application.Interfaces;
using Order.API.Application.Mappers;
using Order.API.Application.Models.Orders;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>
{
    private readonly IOrderService _orderService;

    public UpdateOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateStatusAsync(OrderId.From(request.Id), (OrderStatus)request.OrderStatus);
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<OrderResponse, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }
}
