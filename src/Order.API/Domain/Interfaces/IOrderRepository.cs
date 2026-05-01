using Order.API.Domain.Aggregates;
using Order.API.Domain.ValueObjects;

namespace Order.API.Domain.Interfaces;

public interface IOrderRepository
{
    Task<OrderAggregate?> GetByIdAsync(OrderId id);
    Task<IEnumerable<OrderAggregate>> GetAllAsync();
    Task AddAsync(OrderAggregate order);
    Task UpdateAsync(OrderAggregate order);
    Task DeleteAsync(OrderId id);
}
