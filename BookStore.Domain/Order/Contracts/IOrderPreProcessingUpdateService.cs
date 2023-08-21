namespace BookStore.Domain.Order.Contracts;

public interface IOrderPreProcessingUpdateService
{
    Task Update(Entities.Order order, IEnumerable<Inventory.Entities.Inventory> inventoryItems);
}