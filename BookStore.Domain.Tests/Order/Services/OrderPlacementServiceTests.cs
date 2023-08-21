using AutoFixture;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Events;
using BookStore.Domain.Order.Services;
using FluentAssertions;
using Moq;

namespace BookStore.Domain.Tests.Order.Services;

public class OrderPlacementServiceTests
{
    private Fixture _fixture = new();
    private readonly Mock<IBookRepository> _mockBookRepository = new();
    private readonly Mock<IOrderRepository> _mockOrderRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    [Fact]
    public async Task PlaceOrder_ReturnsSuccessfulResult_WhenOrderIsPlacedSuccessfully()
    {
        // Arrange
        var orderPlacementService = new OrderPlacementService(
            _mockOrderRepository.Object,
            _mockBookRepository.Object,
            _mockUserRepository.Object
        );

        var book1 = new Book();
        var book2 = new Book();

        book1.Inventory = new Domain.Inventory.Entities.Inventory { Quantity = 10 };
        book2.Inventory = new Domain.Inventory.Entities.Inventory { Quantity = 10 };

        var userId = Guid.NewGuid();
        var items = new List<(Guid BookId, int Quantity)>
        {
            (book1.Id, 2),
            (book2.Id, 3)
        };

        var user = new User();
        var order = new Domain.Order.Entities.Order(user);

        _mockUserRepository.Setup(repository => repository.GetById(userId)).ReturnsAsync(user);

        _mockBookRepository.Setup(repository => repository.GetBookById(book1.Id)).ReturnsAsync(book1);
        _mockBookRepository.Setup(repository => repository.GetBookById(book2.Id)).ReturnsAsync(book2);

        _mockOrderRepository.Setup(repository => repository.AddOrder(order)).Returns(Task.CompletedTask);

        // Act
        var result = await orderPlacementService.PlaceOrder(userId, items);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle().And.Subject.First().Should()
            .BeOfType<OrderCreatedDomainEvent>();
    }

    [Fact]
    public async Task PlaceOrder_ReturnsFailedResult_WhenUserNotFound()
    {
        // Arrange
        var orderPlacementService = new OrderPlacementService(
            _mockOrderRepository.Object,
            _mockBookRepository.Object,
            _mockUserRepository.Object
        );
        var userId = Guid.NewGuid();
        var items = new List<(Guid BookId, int Quantity)>
        {
            (Guid.NewGuid(), 2),
            (Guid.NewGuid(), 3)
        };

        _mockUserRepository.Setup(repository => repository.GetById(userId)).ReturnsAsync((User)null);

        // Act
        var result = await orderPlacementService.PlaceOrder(userId, items);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Errors.Should().ContainKey("Invalid user").And.Subject["Invalid user"].Should()
            .Contain("User not found");
    }
}