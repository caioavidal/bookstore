using AutoFixture;
using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using BookStore.Domain.Order.Services;
using FluentAssertions;
using Moq;

namespace BookStore.Domain.Tests.Order.Services;

public class OrderPreProcessingUpdateServiceTests
{
    private Fixture _fixture = new();
    private readonly Mock<IOrderRepository> _mockOrderRepository = new();

    [Fact]
    public async Task Update_ChangesOrderStatusToCancelled_WhenOrderCannotProceed()
    {
        // Arrange
        var orderPreProcessingUpdateService = new OrderPreProcessingUpdateService(_mockOrderRepository.Object);
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);

        var book1 = new Book();
        var book2 = new Book();

        var inventoryItems = new List<Domain.Inventory.Entities.Inventory>
        {
            new() { Book = book1, Quantity = 10 },
            new() { Book = book2, Quantity = 30 }
        };

        book1.Inventory = inventoryItems[0];
        book2.Inventory = inventoryItems[1];

        order.AddItem(book1, 10);
        order.AddItem(book2, 10);

        inventoryItems[0].Quantity = 2;
        inventoryItems[1].Quantity = 2;

        // Act
        await orderPreProcessingUpdateService.Update(order, inventoryItems);

        // Assert
        order.LatestStatus.Should().Be(OrderState.Cancelled);
        _mockOrderRepository.Verify(repository => repository.UpdateOrderStatus(order), Times.Once);
    }

    [Fact]
    public async Task Update_ChangesOrderStatusToProcessing_WhenOrderCanProceed()
    {
        // Arrange
        var orderPreProcessingUpdateService = new OrderPreProcessingUpdateService(_mockOrderRepository.Object);
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);

        var book1 = new Book();
        var book2 = new Book();

        var inventoryItems = new List<Domain.Inventory.Entities.Inventory>
        {
            new() { Book = book1, Quantity = 10 },
            new() { Book = book2, Quantity = 30 }
        };

        book1.Inventory = inventoryItems[0];
        book2.Inventory = inventoryItems[1];

        order.AddItem(book1, 10);
        order.AddItem(book2, 10);

        // Act
        await orderPreProcessingUpdateService.Update(order, inventoryItems);

        // Assert
        order.LatestStatus.Should().Be(OrderState.Processing);
        _mockOrderRepository.Verify(repository => repository.UpdateOrderStatus(order), Times.Once);
    }
}