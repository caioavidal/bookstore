using MediatR;

namespace BookStore.Domain.Inventory.Events;

public class ItemAvailabilityBelowMinimumEvent : INotification
{
    public Entities.Inventory Item { get; set; }
}