using System.ComponentModel.DataAnnotations;
using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Dtos.Input;

public class BookUpdatingDto
{
    [Required] public Guid Id { get; init; }
    [Required] public string Title { get; init; }
    [Required] public string Author { get; init; }
    [Required] public string ISBN { get; init; }
    [Required] public int PublicationYear { get; init; }
    [Required] public decimal Price { get; init; }

    public static implicit operator Book(BookUpdatingDto bookUpdatingDto)
    {
        return new Book
        {
            Id = bookUpdatingDto.Id,
            Price = bookUpdatingDto.Price,
            Author = bookUpdatingDto.Author,
            PublicationYear = bookUpdatingDto.PublicationYear,
            Title = bookUpdatingDto.Title,
            ISBN = bookUpdatingDto.ISBN
        };
    }
}