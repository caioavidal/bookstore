namespace BookStore.Domain.Inventory.Contracts;

public interface IInventoryRepository
{
    Task<IEnumerable<Entities.Inventory>> GetOrderInventories(Guid orderId);
    Task UpdateItemQuantity(Entities.Inventory inventory);
    Task AddItem(Entities.Inventory inventory);
}