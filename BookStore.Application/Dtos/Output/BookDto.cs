using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Dtos.Output;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int PublicationYear { get; set; }
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
    public bool IsOutOfStock => QuantityAvailable <= 0;

    public static implicit operator Book(BookDto bookDto)
    {
        return new Book
        {
            Id = bookDto.Id,
            Title = bookDto.Title,
            ISBN = bookDto.ISBN,
            PublicationYear = bookDto.PublicationYear,
            Author = bookDto.Author,
            Price = bookDto.Price
        };
    }
}