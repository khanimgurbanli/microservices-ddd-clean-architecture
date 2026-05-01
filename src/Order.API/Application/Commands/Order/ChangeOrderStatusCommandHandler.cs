using MediatR;
using Order.API.Application.Interfaces;
using Order.API.Application.Mappers;
using Order.API.Application.Models.Orders;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>
{
    private readonly IOrderService _orderService;

    public ChangeOrderStatusCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var result = await _orderService.UpdateStatusAsync(OrderId.From(request.Id), (OrderStatus)request.Status);
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<OrderResponse, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }
}
