using BookStore.Domain.Inventory.Entities;

namespace BookStore.Domain.Contracts.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetBooks();
    Task<Book> GetBookById(Guid id);
    Task AddBook(Book book);
    Task UpdateBook(Book book);
    Task DeleteBook(Book book);
    Task<IEnumerable<Book>> GetOutOfStockBooks();
}