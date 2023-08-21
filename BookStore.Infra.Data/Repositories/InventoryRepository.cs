using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Inventory.Entities;
using BookStore.Infra.Data.Repositories.Contracts;

namespace BookStore.Infra.Data.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly IDapperWrapper _dapperWrapper;

    public InventoryRepository(IDapperWrapper dapperWrapper)
    {
        _dapperWrapper = dapperWrapper;
    }

    public Task<IEnumerable<Inventory>> GetOrderInventories(Guid orderId)
    {
        const string sql = """
                           SELECT i.id, i.quantity, i.Minimum_Quantity as MinimumQuantity, i.maximum_quantity as MaximumQuantity,
                                                      b.id as InventoryId, b.id, b.title, b.author, b.isbn, b.publication_year as PublicationYear, b.price
                                                   FROM inventory i
                                                   INNER JOIN books b ON i.book_id = b.id
                                                   INNER JOIN order_items oi ON oi.book_id = b.id
                                                       WHERE oi.order_id = @Id
                           """;

        return _dapperWrapper.QueryAsync<Inventory, Book, Inventory>(sql,
            (inventory, book) =>
            {
                inventory.Book = book;
                return inventory;
            },
            new { Id = orderId },
            splitOn: "InventoryId"
        );
    }

    public async Task UpdateItemQuantity(Inventory inventory)
    {
        const string sql = """
                           UPDATE inventory set quantity = @NewQuantity
                           WHERE id = @InventoryId
                           """;

        await _dapperWrapper.ExecuteAsync(sql, new { InventoryId = inventory.Id, NewQuantity = inventory.Quantity });
    }
}