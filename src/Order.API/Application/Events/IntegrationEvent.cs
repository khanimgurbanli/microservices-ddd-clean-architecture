using MediatR;

namespace Order.API.Application.Events;

public interface IIntegrationEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

public abstract class IntegrationEvent : IIntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}