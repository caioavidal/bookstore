using AutoMapper;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using MediatR;

namespace BookStore.Application.Services;

public class OrderAppService : AppService, IOrderAppService
{
    private readonly IMapper _mapper;
    private readonly IOrderPlacementService _orderPlacementService;
    private readonly IOrderRepository _orderRepository;

    public OrderAppService(IOrderPlacementService orderPlacementService,
        IMediator mediator,
        IOrderRepository orderRepository,
        IMapper mapper) : base(mediator)
    {
        _orderPlacementService = orderPlacementService;
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<Result<Order>> PlaceOrder(OrderCreationDto orderCreationDto)
    {
        var items = orderCreationDto.Items.Select(x => (x.BookId, x.Quantity)).ToList();

        var result = await _orderPlacementService.PlaceOrder(orderCreationDto.UserId, items);

        if (!result.Success) return result;

        var createdOrder = result.Value;
        await RaiseDomainEvents(createdOrder);

        return result;
    }

    public async Task<Result> CancelOrder(Guid userId, Guid orderId)
    {
        var order = await _orderRepository.GetById(orderId);

        if (order.User.Id != userId) return Result.Failed.AddError("Invalid user", "You cannot cancel this order");

        var result = order.Cancel();

        if (!result.Success) return result;

        await RaiseDomainEvents(order);
        await _orderRepository.UpdateOrderStatus(order);

        return result;
    }

    public async Task<OrderDto> GetById(Guid orderId)
    {
        var order = await _orderRepository.GetById(orderId);
        return _mapper.Map<OrderDto>(order);
    }

    public async Task<List<OrderDto>> GetUserOrders(Guid userId)
    {
        var orders = await _orderRepository.GetByUserId(userId);
        return _mapper.Map<List<OrderDto>>(orders);
    }
}