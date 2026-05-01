using CSharpFunctionalExtensions;
using Order.API.Application.Commands.Order;
using Order.API.Application.Interfaces;
using Order.API.Application.Mappers;
using Order.API.Application.Models.Orders;
using Order.API.Application.Queries.Order;
using Order.API.Application.Transformations;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Abstractions.Dispatching;
using Shared.Errors;

namespace Order.API.Infrastructure.Dispatching;

public class DirectRequestDispatcher : IRequestDispatcher
{
    private readonly IOrderService _orderService;

    public DirectRequestDispatcher(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        object response = request switch
        {
            CreateOrderCommand cmd => await Handle(cmd),
            UpdateOrderCommand cmd => await Handle(cmd),
            ChangeOrderStatusCommand cmd => await Handle(cmd),
            DeleteOrderCommand cmd => await Handle(cmd),
            GetAllOrdersQuery query => await Handle(query),
            GetOrderByIdQuery query => await Handle(query),
            _ => throw new NotSupportedException($"Unsupported request type: {request.GetType().FullName}")
        };

        return (TResponse)response;
    }

    private async Task<Result<OrderResponse, DomainError>> Handle(CreateOrderCommand command)
    {
        var aggregate = OrderCommandMapper.ToAggregate(command);
        var result = await _orderService.CreateAsync(aggregate);
        if (result.IsFailure)
            return Result.Failure<OrderResponse, DomainError>(result.Error);

        return Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }

    private async Task<Result<OrderResponse, DomainError>> Handle(UpdateOrderCommand command)
    {
        var result = await _orderService.UpdateStatusAsync(OrderId.From(command.Id), (OrderStatus)command.OrderStatus);
        if (result.IsFailure)
            return Result.Failure<OrderResponse, DomainError>(result.Error);

        return Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }

    private async Task<Result<OrderResponse, DomainError>> Handle(ChangeOrderStatusCommand command)
    {
        var result = await _orderService.UpdateStatusAsync(OrderId.From(command.Id), (OrderStatus)command.Status);
        if (result.IsFailure)
            return Result.Failure<OrderResponse, DomainError>(result.Error);

        return Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }

    private async Task<Result<bool, DomainError>> Handle(DeleteOrderCommand command)
    {
        var result = await _orderService.DeleteAsync(OrderId.From(command.Id));
        if (result.IsFailure)
            return Result.Failure<bool, DomainError>(result.Error);

        return Result.Success<bool, DomainError>(true);
    }

    private async Task<Result<IEnumerable<OrderResponse>, DomainError>> Handle(GetAllOrdersQuery query)
    {
        var result = await _orderService.GetAllAsync();
        if (result.IsFailure)
            return Result.Failure<IEnumerable<OrderResponse>, DomainError>(result.Error);

        return Result.Success<IEnumerable<OrderResponse>, DomainError>(OrderMapper.MapToResponse(result.Value));
    }

    private async Task<Result<OrderResponse, DomainError>> Handle(GetOrderByIdQuery query)
    {
        var result = await _orderService.GetByIdAsync(OrderId.From(query.Id));
        if (result.IsFailure)
            return Result.Failure<OrderResponse, DomainError>(result.Error);

        return Result.Success<OrderResponse, DomainError>(OrderMapper.MapToResponse(result.Value));
    }
}
