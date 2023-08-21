using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Dtos.Input;

public class InventoryDto
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public int MinimumQuantity { get; set; }
    public int MaximumQuantity { get; set; }
}