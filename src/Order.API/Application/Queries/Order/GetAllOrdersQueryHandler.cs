using MediatR;
using Order.API.Application.Interfaces;
using Order.API.Application.Mappers;
using Order.API.Application.Models.Orders;
using Shared.Errors;

namespace Order.API.Application.Queries.Order;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, CSharpFunctionalExtensions.Result<IEnumerable<OrderResponse>, DomainError>>
{
    private readonly IOrderService _orderService;

    public GetAllOrdersQueryHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<CSharpFunctionalExtensions.Result<IEnumerable<OrderResponse>, DomainError>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync();
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<IEnumerable<OrderResponse>, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<IEnumerable<OrderResponse>, DomainError>(
            OrderMapper.MapToResponse(result.Value));
    }
}
