using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Entities;
using FluentAssertions;

namespace BookStore.Domain.Tests.Order.Entities;

public class OrderTests
{
    [Fact]
    public void CanBeCancelled_ReturnsTrue_WhenLatestStatusIsCreated()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        order.ChangeStatus(OrderState.Created);

        // Act
        var canBeCancelled = order.CanBeCancelled();

        // Assert
        canBeCancelled.Should().BeTrue();
    }

    [Fact]
    public void CanBeCancelled_ReturnsFalse_WhenLatestStatusIsDelivered()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        order.ChangeStatus(OrderState.Delivered);

        // Act
        var canBeCancelled = order.CanBeCancelled();

        // Assert
        canBeCancelled.Should().BeFalse();
    }

    [Fact]
    public void Cancel_ChangesStatusToCancelled_WhenCanBeCancelled()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        order.ChangeStatus(OrderState.Processing);

        // Act
        var result = order.Cancel();

        // Assert
        result.Success.Should().Be(true);
        order.LatestStatus.Should().Be(OrderState.Cancelled);
    }

    [Fact]
    public void Cancel_ReturnsFailedResult_WhenAlreadyCancelled()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        order.ChangeStatus(OrderState.Cancelled);

        // Act
        var result = order.Cancel();

        // Assert
        result.Success.Should().Be(false);
        order.LatestStatus.Should().Be(OrderState.Cancelled);
    }

    [Fact]
    public void TotalPrice_ReturnsZero_WhenNoOrderItems()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);

        // Act
        var totalPrice = order.TotalPrice();

        // Assert
        totalPrice.Should().Be(0);
    }

    [Fact]
    public void AddItem_ReturnsFailedResult_WhenValidationFails()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        var book = new Book();
        var quantity = 0;

        // Act
        var result = order.AddItem(book, quantity);

        // Assert
        result.Success.Should().Be(false);
        order.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_AddsOrderItem_WhenValidationSucceeds()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);
        var book = new Book();
        book.Inventory = new Domain.Inventory.Entities.Inventory { Quantity = 100 };
        var quantity = 2;

        // Act
        var result = order.AddItem(book, quantity);

        // Assert
        result.Success.Should().Be(true);
        order.OrderItems.Should().ContainSingle();
        order.OrderItems.First().Book.Should().Be(book);
        order.OrderItems.First().Quantity.Should().Be(quantity);
    }

    [Fact]
    public void ValidationErrors_ReturnsError_WhenNoOrderItems()
    {
        // Arrange
        var user = new User();
        var order = new Domain.Order.Entities.Order(user);

        // Act
        var validationErrors = order.ValidationErrors;

        // Assert
        validationErrors.Success.Should().BeFalse();
        validationErrors.Errors.Should().ContainKey("Item invalid");
        validationErrors.Errors["Item invalid"].Should().Be("Order must have at least one item");
    }
}