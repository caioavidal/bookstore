using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Authorize(Roles = "Admin")]
public class InventoryController : ApiController
{
    private readonly IInventoryAppService _inventoryAppService;

    public InventoryController(IInventoryAppService inventoryAppService)
    {
        _inventoryAppService = inventoryAppService;
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] InventoryDto inventoryDto)
    {
        var result = await _inventoryAppService.AddInventoryItem(inventoryDto);
        return HandleApplicationResult(result, Ok);
    }
    
    [HttpPatch]
    public async Task<ActionResult> UpdateQuantity([FromBody] InventoryQuantityUpdateDto inventoryQuantityUpdateDto)
    {
        var result = await _inventoryAppService.UpdateQuantity(inventoryQuantityUpdateDto);
        return HandleApplicationResult(result, Ok);
    }
}