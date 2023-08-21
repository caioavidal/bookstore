using System.Collections.Immutable;
using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;

namespace BookStore.Domain.Order.Entities;

public class Order : Entity
{
    private const int ItemsLimit = 20;

    private readonly List<OrderItem> _orderItems;

    private readonly List<OrderStatus> _statuses;

    public Order(User user)
    {
        CreatedAt = DateTime.UtcNow;

        User = user;
        _statuses = new List<OrderStatus>();
        _orderItems = new List<OrderItem>();

        ChangeStatus(OrderState.Created);
    }

    public Order(User user, List<OrderStatus> statuses, List<OrderItem> items)
    {
        User = user;
        _statuses = statuses ?? new List<OrderStatus>();
        _orderItems = items ?? new List<OrderItem>();
    }

    public User User { get; }
    public ImmutableList<OrderItem> OrderItems => _orderItems.ToImmutableList();
    public ImmutableList<OrderStatus> Statuses => _statuses.ToImmutableList();

    public OrderState LatestStatus => _statuses?.MaxBy(x => x.CreatedAt)?.State ?? OrderState.Created;

    public DateTime CreatedAt { get; init; }

    public Result ValidationErrors
    {
        get
        {
            var errors = Result.Succeeded;

            if (!_orderItems.Any()) errors.AddError("Item invalid", "Order must have at least one item");

            return errors;
        }
    }

    public void ChangeStatus(OrderState status)
    {
        if (_statuses.Any(x => x.State == status)) return;

        _statuses.Add(new OrderStatus
        {
            State = status,
            Order = this
        });
    }

    public Result AddItem(Book book, int quantity)
    {
        var orderItem = new OrderItem(this, book, book.Price, quantity);

        var validationResult = orderItem.Validate();

        if (!validationResult.Success) return validationResult;

        _orderItems.Add(orderItem);

        return Result.Succeeded;
    }

    public bool CanBeCancelled()
    {
        return LatestStatus is not (OrderState.Delivered or OrderState.Shipped);
    }

    public bool IsCancelled()
    {
        return LatestStatus == OrderState.Cancelled;
    }

    public Result Cancel()
    {
        if (!CanBeCancelled()) return Result.Failed.AddError("Cannot be cancelled", "Order cannot be cancelled");
        if (IsCancelled()) return Result.Failed.AddError("Already cancelled", "Order is already cancelled");

        ChangeStatus(OrderState.Cancelled);
        return Result.Succeeded;
    }

    public decimal TotalPrice()
    {
        return OrderItems?.Sum(x => x.Quantity * x.Price) ?? 0;
    }
}