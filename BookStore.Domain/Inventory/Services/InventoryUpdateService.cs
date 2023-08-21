using BookStore.Domain.Inventory.Contracts;

namespace BookStore.Domain.Inventory.Services;

public class ItemAvailabilityService : IItemAvailabilityService
{
    public void UpdateItemsQuantity(Order.Entities.Order order, IEnumerable<Entities.Inventory> inventoryItems)
    {
        var mappedInventoryItems = inventoryItems.ToDictionary(x => x.Book.Id, x => x);

        foreach (var orderItem in order.OrderItems)
        {
            mappedInventoryItems.TryGetValue(orderItem.Book.Id, out var inventoryItem);

            inventoryItem?.SetNewQuantity(inventoryItem.Quantity - orderItem.Quantity);
        }
    }
}