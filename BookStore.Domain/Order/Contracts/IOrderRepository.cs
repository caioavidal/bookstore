namespace BookStore.Domain.Order.Contracts;

public interface IOrderRepository
{
    Task AddOrder(Entities.Order order);
    Task UpdateOrderStatus(Entities.Order order);
    Task<Entities.Order> GetById(Guid orderId);
    Task<IEnumerable<Entities.Order>> GetByUserId(Guid userId);
    Task<IEnumerable<Entities.Order>> GetByItemId(Guid itemId);
}