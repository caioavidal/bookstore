using AutoFixture;
using AutoMapper;
using BookStore.Application.Dtos.Output;
using BookStore.Application.Services;
using BookStore.Domain;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using FluentAssertions;
using Moq;

namespace BookStore.Application.Tests.Services;

public class BookAppServiceTests
{
    private readonly BookAppService _bookAppService;
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IOrderRepository> _mockOrderRepository;

    public BookAppServiceTests()
    {
        _mockBookRepository = new Mock<IBookRepository>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            // Configure AutoMapper mappings
            cfg.CreateMap<Book, BookDto>();
            cfg.CreateMap<Book, OutOfStockBookDto>();
        });
        _mapper = mapperConfig.CreateMapper();
        _bookAppService = new BookAppService(_mockBookRepository.Object, _mockOrderRepository.Object, _mapper);
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetBooks_ReturnsListOfBookDto()
    {
        // Arrange
        var books = _fixture.CreateMany<Book>().ToList();
        _mockBookRepository.Setup(repo => repo.GetBooks()).ReturnsAsync(books);

        // Act
        var result = await _bookAppService.GetBooks();

        // Assert
        var expected = _mapper.Map<IEnumerable<BookDto>>(books);
        expected.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetOutOfStockBooks_ReturnsListOfOutOfStockBookDto()
    {
        // Arrange
        var outOfStockBooks = _fixture.CreateMany<Book>().ToList();
        _mockBookRepository.Setup(repo => repo.GetOutOfStockBooks()).ReturnsAsync(outOfStockBooks);

        // Act
        var result = await _bookAppService.GetOutOfStockBooks();

        // Assert
        var expected = _mapper.Map<List<OutOfStockBookDto>>(outOfStockBooks);
        expected.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task GetBookById_ReturnsBookDto()
    {
        // Arrange
        var book = _fixture.Create<Book>();
        _mockBookRepository.Setup(repo => repo.GetBookById(book.Id)).ReturnsAsync(book);

        // Act
        var result = await _bookAppService.GetBookById(book.Id);

        // Assert
        var expected = _mapper.Map<BookDto>(book);
        expected.Should().BeEquivalentTo(result);
    }

    [Fact]
    public async Task AddBook_CallsAddBookOnRepository()
    {
        // Arrange
        var book = _fixture.Create<Book>();

        // Act
        await _bookAppService.AddBook(book);

        // Assert
        _mockBookRepository.Verify(repo => repo.AddBook(book), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_CallsUpdateBookOnRepository()
    {
        // Arrange
        var book = _fixture.Create<Book>();

        // Act
        await _bookAppService.UpdateBook(book);

        // Assert
        _mockBookRepository.Verify(repo => repo.UpdateBook(book), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_ReturnsFailedResult_WhenOrdersExist()
    {
        // Arrange
        var book = _fixture.Create<Book>();
        var orders = _fixture.CreateMany<Order>().ToList();
        _mockOrderRepository.Setup(repo => repo.GetByItemId(book.Id)).ReturnsAsync(orders);

        // Act
        var result = await _bookAppService.DeleteBook(book);

        // Assert
        Assert.Equal(Result.Failed.Success, result.Success);
    }

    [Fact]
    public async Task DeleteBook_CallsDeleteBookOnRepository_WhenNoOrdersExist()
    {
        // Arrange
        var book = _fixture.Create<Book>();
        _mockOrderRepository.Setup(repo => repo.GetByItemId(book.Id)).ReturnsAsync(new List<Order>());

        // Act
        var result = await _bookAppService.DeleteBook(book);

        // Assert
        _mockBookRepository.Verify(repo => repo.DeleteBook(book), Times.Once);
        Assert.Equal(Result.Succeeded.Success, result.Success);
    }
}