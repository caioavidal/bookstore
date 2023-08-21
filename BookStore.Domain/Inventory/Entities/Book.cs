using BookStore.Domain.Entities;

namespace BookStore.Domain.Inventory.Entities;

public class Book : Entity
{
    public string Title { get; init; }
    public string Author { get; init; }
    public string ISBN { get; init; }
    public int PublicationYear { get; init; }
    public decimal Price { get; init; }

    public Inventory Inventory { get; set; }
    public int QuantityAvailable => Inventory?.Quantity ?? 0;

    public bool IsOutOfStock => QuantityAvailable <= 0;
}