using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain.Inventory.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Authorize]
public class BooksController : ApiController
{
    private readonly IBookAppService _bookService;

    public BooksController(IBookAppService bookService)
    {
        _bookService = bookService;
    }

    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> Get()
    {
        return Ok(await _bookService.GetBooks());
    }

    // GET: api/Books/out-of-stock
    [HttpGet("/out-of-stock")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<OutOfStockBookDto>>> GetOutOfStockBooks()
    {
        return Ok(await _bookService.GetOutOfStockBooks());
    }

    // GET: api/Books/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    {
        var book = await _bookService.GetBookById(id);

        if (book is null) return NotFound();

        return book;
    }

    // PUT: api/Books/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Put(Guid id, BookUpdatingDto book)
    {
        if (id != book.Id) return BadRequest();

        await _bookService.UpdateBook(book);
        return NoContent();
    }

    // POST: api/Books
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Book>> Post(BookCreationDto bookDto)
    {
        Book book = bookDto;
        await _bookService.AddBook(book);

        return CreatedAtAction("GetById", new { id = book.Id }, book);
    }

    // DELETE: api/Books/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null) return NotFound();

        var result = await _bookService.DeleteBook(book);

        return HandleApplicationResult(result, NoContent);
    }
}