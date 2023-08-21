using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Events;

namespace BookStore.Domain.Inventory.Entities;

public class Inventory : Entity
{
    public Book Book { get; set; }
    public int Quantity { get; set; }
    public int MinimumQuantity { get; init; }
    public int MaximumQuantity { get; init; }

    public void SetNewQuantity(int quantity)
    {
        Quantity = Math.Max(quantity, 0);

        if (Quantity < MinimumQuantity) AddDomainEvent(new ItemAvailabilityBelowMinimumEvent { Item = this });
    }
}