using System.ComponentModel.DataAnnotations;
using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Dtos.Input;

public class BookCreationDto
{
    [Required] public string Title { get; init; }
    [Required] public string Author { get; init; }
    [Required] public string ISBN { get; init; }
    [Required] public int PublicationYear { get; init; }
    [Required] public decimal Price { get; init; }

    public static implicit operator Book(BookCreationDto bookCreationDto)
    {
        return new Book
        {
            Id = Guid.NewGuid(),
            Price = bookCreationDto.Price,
            Author = bookCreationDto.Author,
            PublicationYear = bookCreationDto.PublicationYear,
            Title = bookCreationDto.Title,
            ISBN = bookCreationDto.ISBN
        };
    }
}