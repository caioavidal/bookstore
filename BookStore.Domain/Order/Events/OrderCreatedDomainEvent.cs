using MediatR;

namespace BookStore.Domain.Order.Events;

public class OrderCreatedDomainEvent : INotification
{
    public Entities.Order Order { get; init; }
}