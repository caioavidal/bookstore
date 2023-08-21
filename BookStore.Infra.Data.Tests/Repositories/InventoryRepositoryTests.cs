using System.Data;
using AutoFixture;
using BookStore.Domain.Inventory.Entities;
using BookStore.Infra.Data.Repositories;
using BookStore.Infra.Data.Repositories.Contracts;
using FluentAssertions;
using Moq;

namespace BookStore.Infra.Data.Tests.Repositories;

public class InventoryRepositoryTests
{
    private readonly Fixture _fixture;

    public InventoryRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetOrderInventories_ReturnsListOfInventoriesForOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedInventories = _fixture.CreateMany<Inventory>().ToList();

        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<Inventory, Book, Inventory>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>()
        )).ReturnsAsync(expectedInventories);

        var repository = new InventoryRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetOrderInventories(orderId);

        // Assert
        result.Should().BeEquivalentTo(expectedInventories);
    }

    [Fact]
    public async Task UpdateItemQuantity_UpdatesQuantitySuccessfully()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var inventoryToUpdate = _fixture.Create<Inventory>();
        var repository = new InventoryRepository(dapperWrapperMock.Object);

        // Act
        await repository.UpdateItemQuantity(inventoryToUpdate);

        // Assert
        dapperWrapperMock.Verify(d => d.ExecuteAsync(
            It.IsAny<string>(),
            It.Is<object>(param =>
                param.GetType().GetProperty("InventoryId").GetValue(param).Equals(inventoryToUpdate.Id) &&
                param.GetType().GetProperty("NewQuantity").GetValue(param).Equals(inventoryToUpdate.Quantity)
            ),
            null,
            null,
            null
        ), Times.Once);
    }
}