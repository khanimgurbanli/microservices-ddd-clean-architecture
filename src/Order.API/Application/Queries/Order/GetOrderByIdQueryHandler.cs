using MediatR;
using Order.API.Application.Interfaces;
using Order.API.Application.Mappers;
using Order.API.Application.Models.Orders;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Application.Queries.Order;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, CSharpFunctionalExtensions.Result<OrderResponse, DomainError>>
{
    private readonly IOrderService _orderService;

    public GetOrderByIdQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<CSharpFunctionalExtensions.Result<OrderResponse, DomainError>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(OrderId.From(request.Id));
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<OrderResponse, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }
}
