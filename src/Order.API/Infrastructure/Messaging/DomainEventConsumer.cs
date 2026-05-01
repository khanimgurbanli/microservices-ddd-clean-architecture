using MassTransit;
using MediatR;
using Order.API.Domain.Events;
using Order.API.Domain.Interfaces;

namespace Order.API.Infrastructure.Messaging;

public class DomainEventConsumer :
    IConsumer<OrderCreatedEvent>,
    IConsumer<OrderStatusChangedEvent>
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventConsumer(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await _dispatcher.DispatchAsync(new[] { context.Message });
    }

    public async Task Consume(ConsumeContext<OrderStatusChangedEvent> context)
    {
        await _dispatcher.DispatchAsync(new[] { context.Message });
    }
}