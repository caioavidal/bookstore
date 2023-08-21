using MediatR;

namespace BookStore.Domain.Entities;

public abstract class Entity
{
    private List<INotification> _domainEvents;
    public Guid Id { get; init; } = Guid.NewGuid();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}