using CSharpFunctionalExtensions;
using Order.API.Application.Interfaces;
using Order.API.Domain.Aggregates;
using Order.API.Domain.Enums;
using Order.API.Domain.Interfaces;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public OrderService(IOrderRepository orderRepository, IDomainEventDispatcher eventDispatcher)
    {
        _orderRepository = orderRepository;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Result<OrderAggregate, DomainError>> CreateAsync(OrderAggregate order)
    {
        await _orderRepository.AddAsync(order);
        await DispatchDomainEventsAsync(order);

        return Result.Success<OrderAggregate, DomainError>(order);
    }

    public async Task<Result<OrderAggregate, DomainError>> UpdateStatusAsync(OrderId id, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return Result.Failure<OrderAggregate, DomainError>(DomainError.NotFound($"Order with id {id.Value} not found"));

        order.UpdateStatus(newStatus);
        await _orderRepository.UpdateAsync(order);
        await DispatchDomainEventsAsync(order);

        return Result.Success<OrderAggregate, DomainError>(order);
    }

    public async Task<Result<OrderId, DomainError>> DeleteAsync(OrderId id)
    {
        await _orderRepository.DeleteAsync(id);
        return Result.Success<OrderId, DomainError>(id);
    }

    public async Task<Result<IEnumerable<OrderAggregate>, DomainError>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return Result.Success<IEnumerable<OrderAggregate>, DomainError>(orders);
    }

    public async Task<Result<OrderAggregate, DomainError>> GetByIdAsync(OrderId id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
            return Result.Failure<OrderAggregate, DomainError>(DomainError.NotFound($"Order with id {id.Value} not found"));

        return Result.Success<OrderAggregate, DomainError>(order);
    }

    private async Task DispatchDomainEventsAsync(OrderAggregate order)
    {
        var domainEvents = order.DomainEvents.ToList();
        order.ClearDomainEvents();

        if (domainEvents.Any())
        {
            await _eventDispatcher.DispatchAsync(domainEvents);
        }
    }
}
