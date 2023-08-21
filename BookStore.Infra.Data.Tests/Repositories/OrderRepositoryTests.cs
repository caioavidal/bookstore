using System.Data;
using AutoFixture;
using BookStore.Domain.Order.Entities;
using BookStore.Infra.Data.QueryModels;
using BookStore.Infra.Data.Repositories;
using BookStore.Infra.Data.Repositories.Contracts;
using FluentAssertions;
using Moq;

namespace BookStore.Infra.Data.Tests.Repositories;

public class OrderRepositoryTests
{
    private readonly Fixture _fixture;

    public OrderRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetByUserId_ReturnsListOfOrdersForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var orderQueryModels = _fixture.CreateMany<OrderQueryModel>().ToList();
        var expectedOrders = orderQueryModels.Select(x => (Order)x);

        var dbConnection = new Mock<IDbConnection>();

        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<OrderQueryModel, OrderQueryModel.UserModel, OrderQueryModel.OrderItemModel,
                OrderQueryModel.BookModel, OrderQueryModel.OrderStatusModel, OrderQueryModel>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        )).ReturnsAsync(orderQueryModels);

        var repository = new OrderRepository(dbConnection.Object, dapperWrapperMock.Object);

        // Act
        var result = await repository.GetByUserId(userId);

        // Assert
        result.Should().BeEquivalentTo(expectedOrders);
    }

    [Fact]
    public async Task GetByItemId_ReturnsListOfOrdersForItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var dbConnection = new Mock<IDbConnection>();
        var orderQueryModels = _fixture.CreateMany<OrderQueryModel>().ToList();
        var expectedOrders = orderQueryModels.Select(x => (Order)x);

        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<OrderQueryModel, OrderQueryModel.UserModel, OrderQueryModel.OrderItemModel,
                OrderQueryModel.BookModel, OrderQueryModel.OrderStatusModel, OrderQueryModel>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        )).ReturnsAsync(orderQueryModels);

        var repository = new OrderRepository(dbConnection.Object, dapperWrapperMock.Object);

        // Act
        var result = await repository.GetByItemId(itemId);

        // Assert
        result.Should().BeEquivalentTo(expectedOrders);
    }

    [Fact]
    public async Task GetById_ReturnsOrderWithDetails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedOrder = _fixture.Create<OrderQueryModel>();
        var dbConnection = new Mock<IDbConnection>();

        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<OrderQueryModel, OrderQueryModel.UserModel, OrderQueryModel.OrderItemModel,
                OrderQueryModel.BookModel, OrderQueryModel.OrderStatusModel, OrderQueryModel>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        )).ReturnsAsync(new List<OrderQueryModel> { expectedOrder });

        var repository = new OrderRepository(dbConnection.Object, dapperWrapperMock.Object);

        // Act
        var result = await repository.GetById(orderId);

        // Assert
        result.Should().BeEquivalentTo(expectedOrder, options =>
            options.Excluding(o => o.User)
                .Excluding(o => o.OrderItems)
                .Excluding(o => o.OrderStatuses));

        result.User.Should().BeEquivalentTo(expectedOrder.User, opt => opt.Excluding(o => o.UserId));
    }

    [Fact]
    public async Task AddOrder_AddsOrderWithItemsAndStatuses()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var orderToAdd = _fixture.Create<Order>();
        var dbConnection = new Mock<IDbConnection>();

        dbConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);
        var repository = new OrderRepository(dbConnection.Object, dapperWrapperMock.Object);

        // Act
        await repository.AddOrder(orderToAdd);

        // Assert
        dapperWrapperMock.Verify(d => d.ExecuteAsync(
            It.IsAny<string>(),
            It.Is<object>(param =>
                param.GetType().GetProperty("Id").GetValue(param).Equals(orderToAdd.Id) &&
                param.GetType().GetProperty("UserId").GetValue(param).Equals(orderToAdd.User.Id) &&
                param.GetType().GetProperty("CreatedAt").GetValue(param).Equals(orderToAdd.CreatedAt)
            ),
            It.IsAny<IDbTransaction>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        ), Times.Once);

        foreach (var orderItem in orderToAdd.OrderItems)
            dapperWrapperMock.Verify(d => d.ExecuteAsync(
                It.IsAny<string>(),
                It.Is<object>(param =>
                    param.GetType().GetProperty("Id").GetValue(param).Equals(orderItem.Id) &&
                    param.GetType().GetProperty("OrderId").GetValue(param).Equals(orderToAdd.Id) &&
                    param.GetType().GetProperty("BookId").GetValue(param).Equals(orderItem.Book.Id) &&
                    param.GetType().GetProperty("Price").GetValue(param).Equals(orderItem.Price) &&
                    param.GetType().GetProperty("Quantity").GetValue(param).Equals(orderItem.Quantity)
                ),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()
            ), Times.Once);

        foreach (var orderStatus in orderToAdd.Statuses)
            dapperWrapperMock.Verify(d => d.ExecuteAsync(
                It.IsAny<string>(),
                It.Is<object>(param =>
                    param.GetType().GetProperty("Id").GetValue(param).Equals(orderStatus.Id) &&
                    param.GetType().GetProperty("OrderId").GetValue(param).Equals(orderToAdd.Id) &&
                    param.GetType().GetProperty("State").GetValue(param).Equals(orderStatus.State) &&
                    param.GetType().GetProperty("CreatedAt").GetValue(param).Equals(orderStatus.CreatedAt)
                ),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()
            ), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_UpdatesStatuses()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var orderToUpdate = _fixture.Create<Order>();

        var dbConnection = new Mock<IDbConnection>();
        var repository = new OrderRepository(dbConnection.Object, dapperWrapperMock.Object);

        var existingStatuses = new List<OrderStatus>
        {
            _fixture.Create<OrderStatus>(),
            _fixture.Create<OrderStatus>()
        };

        dbConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);

        dapperWrapperMock.Setup(d => d.QueryAsync<OrderStatus>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        )).ReturnsAsync(existingStatuses);

        // Act
        await repository.UpdateOrderStatus(orderToUpdate);

        // Assert
        dapperWrapperMock.Verify(d => d.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        ), Times.Once);
    }
}