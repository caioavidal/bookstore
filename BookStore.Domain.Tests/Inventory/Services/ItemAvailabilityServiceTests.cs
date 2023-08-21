using AutoFixture;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Inventory.Services;
using Moq;

namespace BookStore.Domain.Tests.Inventory.Services;

public class ItemAvailabilityServiceTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void UpdateItemsQuantity_DecreasesInventoryQuantity_ByOrderItemQuantity()
    {
        // Arrange
        var itemAvailabilityService = new ItemAvailabilityService();
        var order = _fixture.Create<Domain.Order.Entities.Order>();
        var bookId = Guid.NewGuid();
        var book = new Book { Id = bookId };

        var inventory = new Domain.Inventory.Entities.Inventory
        {
            Book = book,
            Quantity = 20
        };

        book.Inventory = inventory;

        order.AddItem(book, 10);

        var inventoryItems = new List<Domain.Inventory.Entities.Inventory>
        {
            inventory
        };

        // Act
        itemAvailabilityService.UpdateItemsQuantity(order, inventoryItems);

        // Assert
        Assert.Equal(10, inventory.Quantity);
    }

    [Fact]
    public void UpdateItemsQuantity_DoesNotDecreaseInventoryQuantity_IfOrderItemNotFound()
    {
        // Arrange
        var itemAvailabilityService = new ItemAvailabilityService();
        var order = _fixture.Create<Domain.Order.Entities.Order>();
        var bookId = Guid.NewGuid();
        var book = new Book { Id = bookId };

        var inventory = new Domain.Inventory.Entities.Inventory
        {
            Book = book,
            Quantity = 20
        };

        book.Inventory = inventory;

        order.AddItem(new Book { Id = Guid.NewGuid() }, 10);

        var inventoryItems = new List<Domain.Inventory.Entities.Inventory>
        {
            inventory
        };

        // Act
        itemAvailabilityService.UpdateItemsQuantity(order, inventoryItems);

        // Assert
        Assert.Equal(20, inventory.Quantity);
    }
}