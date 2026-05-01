namespace Order.API.Domain.Interfaces;

public interface IAggregateRoot
{
    IReadOnlyCollection<Domain.Events.DomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
