using BookStore.Application.Dtos.Input;
using BookStore.Domain;

namespace BookStore.Application.Contracts;

public interface IInventoryAppService
{
    Task<Result> AddInventoryItem(InventoryDto inventoryDto);
    Task<Result> UpdateQuantity(InventoryQuantityUpdateDto inventoryQuantityUpdateDto);
}