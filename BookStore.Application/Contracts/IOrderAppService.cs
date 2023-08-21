using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Order.Entities;

namespace BookStore.Application.Contracts;

public interface IOrderAppService
{
    Task<Result<Order>> PlaceOrder(OrderCreationDto orderCreationDto);
    Task<Result> CancelOrder(Guid userId, Guid orderId);
    Task<OrderDto> GetById(Guid orderId);
    Task<List<OrderDto>> GetUserOrders(Guid userId);
}