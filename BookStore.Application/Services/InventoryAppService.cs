using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Domain;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Inventory.Entities;

namespace BookStore.Application.Services;

public class InventoryAppService:IInventoryAppService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IBookRepository _bookRepository;

    public InventoryAppService(IInventoryRepository inventoryRepository, IBookRepository bookRepository)
    {
        _inventoryRepository = inventoryRepository;
        _bookRepository = bookRepository;
    }
    
    public async Task<Result> AddInventoryItem(InventoryDto inventoryDto)
    {
        var book = await _bookRepository.GetBookById(inventoryDto.BookId);
        if (book is null)
        {
            return Result.Failed.AddError("Book not found", "Book not found");
        }

        if (book.Inventory is not null) return Result.Failed.AddError("Invalid operation", "Book is already in inventory");
        
        await _inventoryRepository.AddItem(new Inventory()
        {
            Book = new Book() { Id = inventoryDto.BookId },
            Quantity = inventoryDto.Quantity,
            MaximumQuantity = inventoryDto.MaximumQuantity,
            MinimumQuantity = inventoryDto.MinimumQuantity
        });

        return Result.Succeeded;
    }

    public async Task<Result> UpdateQuantity(InventoryQuantityUpdateDto inventoryQuantityUpdateDto)
    {
        var book = await _bookRepository.GetBookById(inventoryQuantityUpdateDto.BookId);
        if (book is null)
        {
            return Result.Failed.AddError("Book not found", "Book not found");
        }
        
        if (book.Inventory is null)
        {
          return Result.Failed.AddError("Invalid operation", "Book is not in inventory");
        }

        book.Inventory.SetNewQuantity(inventoryQuantityUpdateDto.NewQuantity);
        await _inventoryRepository.UpdateItemQuantity(book.Inventory);
        return Result.Succeeded;
    }
}