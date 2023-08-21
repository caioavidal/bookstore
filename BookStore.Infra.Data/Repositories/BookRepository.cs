using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Inventory.Entities;
using BookStore.Infra.Data.Repositories.Contracts;

namespace BookStore.Infra.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IDapperWrapper _dapperWrapper;

    public BookRepository(IDapperWrapper dapperWrapper)
    {
        _dapperWrapper = dapperWrapper;
    }

    public async Task<IEnumerable<Book>> GetBooks()
    {
        var sql = "SELECT b.publication_year PublicationYear, * FROM Books b left join Inventory i on i.Book_Id = b.Id";
        return await _dapperWrapper.QueryAsync<Book, Inventory, Book>(
            sql,
            (book, inventory) =>
            {
                book.Inventory = inventory;
                return book;
            },
            splitOn: "Quantity"
        );
    }

    public async Task<IEnumerable<Book>> GetOutOfStockBooks()
    {
        var sql = @"SELECT b.Id,
                           b.title,
                           b.author,
                           b.isbn,
                           b.publication_year PublicationYear,
                           b.price FROM Books b
                    inner join Inventory i on i.Book_Id = b.Id   
                    where i.Quantity <= 0";

        return await _dapperWrapper.QueryAsync<Book>(sql);
    }

    public async Task<Book> GetBookById(Guid id)
    {
        var sql = @"SELECT b.Id,
       b.title,
       b.author,
       b.isbn,
                                  b.publication_year PublicationYear,
                                  b.price,
                                  i.book_id as BookId,
                                  i.Id,
                                  i.quantity,
                                  i.minimum_quantity as MinimumQuantity,
                                  i.maximum_quantity as MaximumQuantity
                         FROM Books b
                    left join Inventory i on i.Book_Id = b.Id   
                    WHERE b.Id = @Id";

        var result = await _dapperWrapper.QueryAsync<Book, Inventory, Book>(
            sql,
            (book, inventory) =>
            {
                book.Inventory = inventory;
                return book;
            },
            new { id },
            splitOn: "BookId"
        );

        return result.FirstOrDefault();
    }

    public async Task AddBook(Book book)
    {
        var sql =
            "INSERT INTO Books (Id, Title, Author, ISBN, Publication_Year, Price) VALUES (@Id, @Title, @Author, @ISBN, @PublicationYear, @Price)";
        await _dapperWrapper.ExecuteAsync(sql, book);
    }

    public async Task UpdateBook(Book book)
    {
        var sql = "UPDATE Books SET Title = @Title, Author = @Author, ISBN = @ISBN, " +
                  "Publication_Year = @PublicationYear, Price = @Price " +
                  "WHERE Id = @Id";
        await _dapperWrapper.ExecuteAsync(sql, book);
    }

    public async Task DeleteBook(Book book)
    {
        var sql = "DELETE FROM Books WHERE Id = @Id";
        await _dapperWrapper.ExecuteAsync(sql, new { book.Id });
    }
}