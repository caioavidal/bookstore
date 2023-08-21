namespace BookStore.Domain.Order.Entities;

public class OrderStatus
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Order Order { get; init; }
    public OrderState State { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}