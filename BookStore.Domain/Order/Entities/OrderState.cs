namespace BookStore.Domain.Order.Entities;

public enum OrderState
{
    Created,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}