using System.Data;
using BookStore.Application.EventHandlers.Domain;
using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using BookStore.Domain.Order.Events;
using Moq;

namespace BookStore.Application.Tests.DomainEvents;

public class OrderCreatedDomainEventHandlerTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly Mock<IItemAvailabilityService> _mockItemAvailabilityService;
    private readonly Mock<IOrderPreProcessingUpdateService> _mockOrderPreProcessingUpdateService;
    private readonly OrderCreatedDomainEventHandler _orderCreatedDomainEventHandler;

    public OrderCreatedDomainEventHandlerTests()
    {
        _mockInventoryRepository = new Mock<IInventoryRepository>();
        _mockOrderPreProcessingUpdateService = new Mock<IOrderPreProcessingUpdateService>();
        _mockItemAvailabilityService = new Mock<IItemAvailabilityService>();
        _mockDbConnection = new Mock<IDbConnection>();
        _orderCreatedDomainEventHandler = new OrderCreatedDomainEventHandler(
            _mockInventoryRepository.Object,
            _mockOrderPreProcessingUpdateService.Object,
            _mockItemAvailabilityService.Object,
            _mockDbConnection.Object
        );
    }

    [Fact]
    public async Task Handle_UpdatesOrderPreProcessingAndInventoryItems()
    {
        // Arrange
        var user = new User();
        var order = new Order(user);
        var notification = new OrderCreatedDomainEvent { Order = order };
        var inventoryItems = new List<Inventory>();

        _mockInventoryRepository
            .Setup(repository => repository.GetOrderInventories(order.Id))
            .ReturnsAsync(inventoryItems);

        var transaction = new Mock<IDbTransaction>();
        _mockDbConnection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);

        // Act
        await _orderCreatedDomainEventHandler.Handle(notification, CancellationToken.None);

        // Assert
        _mockOrderPreProcessingUpdateService
            .Verify(service => service.Update(order, inventoryItems), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdatesItemAvailabilityForProcessingOrders()
    {
        // Arrange
        var user = new User();
        var order = new Order(user);
        order.ChangeStatus(OrderState.Processing);

        var notification = new OrderCreatedDomainEvent { Order = order };
        var inventoryItems = new List<Inventory>();

        _mockInventoryRepository
            .Setup(repository => repository.GetOrderInventories(order.Id))
            .ReturnsAsync(inventoryItems);

        var transaction = new Mock<IDbTransaction>();
        _mockDbConnection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);

        // Act
        await _orderCreatedDomainEventHandler.Handle(notification, CancellationToken.None);

        // Assert
        _mockItemAvailabilityService
            .Verify(service => service.UpdateItemsQuantity(order, inventoryItems), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdatesItemQuantityForEachInventoryItem()
    {
        // Arrange
        var user = new User();
        var order = new Order(user);
        var notification = new OrderCreatedDomainEvent { Order = order };
        var inventoryItems = new List<Inventory> { new() };

        _mockInventoryRepository
            .Setup(repository => repository.GetOrderInventories(order.Id))
            .ReturnsAsync(inventoryItems);

        var transaction = new Mock<IDbTransaction>();
        _mockDbConnection.Setup(x => x.BeginTransaction()).Returns(transaction.Object);

        // Act
        await _orderCreatedDomainEventHandler.Handle(notification, CancellationToken.None);

        // Assert
        _mockInventoryRepository
            .Verify(repository => repository.UpdateItemQuantity(inventoryItems[0]), Times.Once);
    }
}