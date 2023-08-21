namespace BookStore.Domain.Inventory.Contracts;

public interface IItemAvailabilityService
{
    void UpdateItemsQuantity(Order.Entities.Order order, IEnumerable<Entities.Inventory> inventoryItems);
}