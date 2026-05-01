using MediatR;
using Order.API.Domain.Events;
using Order.API.Domain.Interfaces;

namespace Order.API.Infrastructure.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task DispatchAsync(IEnumerable<DomainEvent> events)
    {
        foreach (var domainEvent in events)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}