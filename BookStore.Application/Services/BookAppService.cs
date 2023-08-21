using AutoMapper;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Contracts;

namespace BookStore.Application.Services;

public class BookAppService : IBookAppService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public BookAppService(IBookRepository bookRepository, IOrderRepository orderRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookDto>> GetBooks()
    {
        var records = await _bookRepository.GetBooks();
        return _mapper.Map<IEnumerable<BookDto>>(records);
    }

    public async Task<IEnumerable<OutOfStockBookDto>> GetOutOfStockBooks()
    {
        var records = await _bookRepository.GetOutOfStockBooks();
        var outOfStockBooks = _mapper.Map<List<OutOfStockBookDto>>(records);

        return outOfStockBooks;
    }

    public async Task<BookDto> GetBookById(Guid id)
    {
        var record = await _bookRepository.GetBookById(id);
        return _mapper.Map<BookDto>(record);
    }

    public async Task AddBook(Book book)
    {
        await _bookRepository.AddBook(book);
    }

    public async Task UpdateBook(Book book)
    {
        await _bookRepository.UpdateBook(book);
    }

    public async Task<Result> DeleteBook(Book book)
    {
        var orders = await _orderRepository.GetByItemId(book.Id);

        if (orders.Any())
            return Result.Failed.AddError("Cannot delete book",
                "The book cannot be deleted because there is a order for it");

        await _bookRepository.DeleteBook(book);
        return Result.Succeeded;
    }
}