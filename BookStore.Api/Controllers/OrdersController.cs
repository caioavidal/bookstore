using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain.Order.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BookStore.Api.Controllers;

[Authorize]
public class OrdersController : ApiController
{
    private readonly IOrderAppService _orderAppService;

    public OrdersController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> Get()
    {
        var userId = GetUserId();

        var orders = await _orderAppService.GetUserOrders(userId);
        return Ok(orders);
    }

    // GET: api/Orders/{id}
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderAppService.GetById(id);
        if (order is null) return NotFound();

        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> Post(OrderCreationDto orderCreationDto)
    {
        var userId = GetUserId();

        orderCreationDto.UserId = userId;

        var result = await _orderAppService.PlaceOrder(orderCreationDto);

        return HandleApplicationResult(result,
            () => CreatedAtAction("GetById", new { id = result.Value.Id }, result.Value));
    }

    // DELETE: api/Orders/{id}/cancel
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var result = await _orderAppService.CancelOrder(userId, id);

        return HandleApplicationResult(result, NoContent);
    }

    private Guid GetUserId()
    {
        var sid = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value?.ToString();
        return Guid.TryParse(sid, out var userId) ? userId : Guid.Empty;
    }
}