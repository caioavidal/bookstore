using System.Data;
using AutoFixture;
using BookStore.Domain.Inventory.Entities;
using BookStore.Infra.Data.Repositories;
using BookStore.Infra.Data.Repositories.Contracts;
using FluentAssertions;
using Moq;

namespace BookStore.Infra.Data.Tests.Repositories;

public class BookRepositoryTests
{
    private readonly Fixture _fixture;

    public BookRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetBooks_ReturnsListOfBooks()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedBooks = _fixture.CreateMany<Book>().ToList();

        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<Book, Inventory, Book>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>())
        ).ReturnsAsync(expectedBooks);

        var repository = new BookRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetBooks();

        // Assert
        result.Should().BeEquivalentTo(expectedBooks);
    }

    [Fact]
    public async Task GetOutOfStockBooks_ReturnsListOfOutOfStockBooks()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedBooks = _fixture.CreateMany<Book>().ToList();
        dapperWrapperMock.Setup(d => d.QueryAsync<Book>(It.IsAny<string>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>())
        ).ReturnsAsync(expectedBooks);

        var repository = new BookRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetOutOfStockBooks();

        // Assert
        result.Should().BeEquivalentTo(expectedBooks);
    }

    [Fact]
    public async Task GetBookById_ReturnsBookWithInventory()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedBook = new Book();
        dapperWrapperMock.Setup(d => d.QueryAsync(
            It.IsAny<string>(),
            It.IsAny<Func<Book, Inventory, Book>>(),
            It.IsAny<object>(),
            It.IsAny<IDbTransaction>(),
            It.IsAny<bool>(),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CommandType?>())
        ).ReturnsAsync(new List<Book> { expectedBook });

        var repository = new BookRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetBookById(expectedBook.Id);

        // Assert
        result.Should().BeEquivalentTo(expectedBook);
    }

    [Fact]
    public async Task AddBook_AddsBookSuccessfully()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var bookToAdd = new Book();
        var repository = new BookRepository(dapperWrapperMock.Object);

        // Act
        await repository.AddBook(bookToAdd);

        // Assert
        dapperWrapperMock.Verify(d => d.ExecuteAsync(
                It.IsAny<string>(),
                It.Is<Book>(b => b.Id == bookToAdd.Id && b.Title == bookToAdd.Title),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()),
            Times.Once);
    }
}