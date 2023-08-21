using BookStore.Domain.Inventory.Entities;

namespace BookStore.Domain.Order.Entities;

public class OrderItem
{
    public OrderItem(Order order, Book book, decimal price, int quantity)
    {
        Order = order;
        Book = book;
        Price = price;
        Quantity = quantity;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Order Order { get; init; }
    public Book Book { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }

    public Result Validate()
    {
        if (Book is null) return Result.Failed.AddError("Item", "Item is invalid");
        if (Order is null) return Result.Failed.AddError("Order invalid", "Order is invalid");
        if (Quantity <= 0) return Result.Failed.AddError("Invalid quantity", "Quantity cannot be equal or less than 0");

        if (Book.QuantityAvailable < Quantity)
            return Result.Failed.AddError("Invalid items",
                $"There is {Book.QuantityAvailable} item(s) available for selected book {Book.Title}.");

        return Result.Succeeded;
    }
}