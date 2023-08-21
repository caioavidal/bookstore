namespace BookStore.Domain.Order.Contracts;

public interface IOrderPlacementService
{
    Task<Result<Entities.Order>> PlaceOrder(Guid userId, List<(Guid BookId, int Quantity)> items);
}