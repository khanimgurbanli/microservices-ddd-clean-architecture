using CSharpFunctionalExtensions;
using Order.API.Domain.Aggregates;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderAggregate, DomainError>> CreateAsync(OrderAggregate order);
    Task<Result<OrderAggregate, DomainError>> UpdateStatusAsync(OrderId id, OrderStatus newStatus);
    Task<Result<OrderId, DomainError>> DeleteAsync(OrderId id);
    Task<Result<IEnumerable<OrderAggregate>, DomainError>> GetAllAsync();
    Task<Result<OrderAggregate, DomainError>> GetByIdAsync(OrderId id);
}
