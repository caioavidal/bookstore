using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.Dtos.Input;

public class OrderCreationDto
{
    [Required] public Guid UserId { get; set; }

    [Required] public List<ItemDto> Items { get; init; } = new();

    public record ItemDto([Required] Guid BookId, [Required] int Quantity);
}