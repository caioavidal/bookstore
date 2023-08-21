using BookStore.Api.Controllers;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Inventory.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Api.Tests.Controllers;

public class BooksControllerTests
{
    private readonly BooksController _controller;
    private readonly Mock<IBookAppService> _mockBookService;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookAppService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithBooks()
    {
        // Arrange
        var books = new List<BookDto> { new(), new() };
        _mockBookService.Setup(service => service.GetBooks()).ReturnsAsync(books);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<BookDto>>(okResult.Value);
        Assert.Equal(books, returnValue);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithNoBooks()
    {
        // Arrange
        _mockBookService.Setup(service => service.GetBooks()).ReturnsAsync(new List<BookDto>());

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<BookDto>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundResult_WhenBookDoesNotExist()
    {
        // Arrange
        _mockBookService.Setup(service => service.GetBookById(It.IsAny<Guid>())).ReturnsAsync((BookDto)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsBook_WhenBookExists()
    {
        // Arrange
        var book = new BookDto();
        _mockBookService.Setup(service => service.GetBookById(It.IsAny<Guid>())).ReturnsAsync(book);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.Equal(book, result.Value);
    }

    [Fact]
    public async Task Put_ReturnsBadRequestResult_WhenIdDoesNotMatch()
    {
        // Arrange
        var book = new BookUpdatingDto { Id = Guid.NewGuid() };

        // Act
        var result = await _controller.Put(Guid.NewGuid(), book);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Put_ReturnsNoContentResult_WhenIdMatches()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book = new BookUpdatingDto { Id = id };
        _mockBookService.Setup(service => service.UpdateBook(book)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Put(id, book);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtActionResult_WithBook()
    {
        // Arrange
        var bookDto = new BookCreationDto();
        var book = new Book();
        _mockBookService.Setup(service => service.AddBook(It.IsAny<Book>()));

        // Act
        var result = await _controller.Post(bookDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetById", createdAtActionResult.ActionName);
    }

    [Fact]
    public async Task Delete_ReturnsNotFoundResult_WhenBookDoesNotExist()
    {
        // Arrange
        _mockBookService.Setup(service => service.GetBookById(It.IsAny<Guid>())).ReturnsAsync((BookDto)null);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult_WhenBookExists()
    {
        // Arrange
        var book = new BookDto();
        _mockBookService.Setup(service => service.GetBookById(It.IsAny<Guid>())).ReturnsAsync(book);
        _mockBookService.Setup(service => service.DeleteBook(It.IsAny<Book>())).ReturnsAsync(Result.Succeeded);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsUnprocessableEntityResult_WhenDeleteFails()
    {
        // Arrange
        var book = new BookDto();
        _mockBookService.Setup(service => service.GetBookById(It.IsAny<Guid>())).ReturnsAsync(book);
        _mockBookService.Setup(service => service.DeleteBook(It.IsAny<Book>())).ReturnsAsync(Result.Failed);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }
}