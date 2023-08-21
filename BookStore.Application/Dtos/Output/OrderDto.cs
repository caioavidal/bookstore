using BookStore.Domain.Order.Entities;

namespace BookStore.Application.Dtos.Output;

public class OrderDto
{
    public Guid Id { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public List<OrderStatusDto> Statuses { get; set; }
    public OrderState LatestStatus => Statuses?.MaxBy(x => x.CreatedAt)?.State ?? OrderState.Created;
    public UserDto User { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsCancelled => LatestStatus is OrderState.Cancelled;

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderStatusDto
    {
        public Guid Id { get; set; }
        public OrderState State { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}