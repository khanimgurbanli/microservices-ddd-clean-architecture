using Order.API.Domain.Events;

namespace Order.API.Domain.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<DomainEvent> events);
}