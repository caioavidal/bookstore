using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Contracts;

public interface IBookAppService
{
    Task<IEnumerable<BookDto>> GetBooks();
    Task<BookDto> GetBookById(Guid id);
    Task AddBook(Book book);
    Task UpdateBook(Book book);
    Task<Result> DeleteBook(Book book);
    Task<IEnumerable<OutOfStockBookDto>> GetOutOfStockBooks();
}