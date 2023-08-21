namespace BookStore.Application.Dtos.Input;

public class InventoryQuantityUpdateDto
{
    public Guid BookId { get; set; }
    public int NewQuantity { get; set; }
}