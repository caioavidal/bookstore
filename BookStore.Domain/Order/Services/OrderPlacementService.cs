using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Events;

namespace BookStore.Domain.Order.Services;

public class OrderPlacementService : IOrderPlacementService
{
    private readonly IBookRepository _bookRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderPlacementService(IOrderRepository orderRepository, IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<Entities.Order>> PlaceOrder(Guid userId, List<(Guid BookId, int Quantity)> items)
    {
        var user = await _userRepository.GetById(userId);

        if (user is null) return new Result<Entities.Order>().AddError("Invalid user", "User not found");

        var order = new Entities.Order(user);

        foreach (var (bookId, quantity) in items)
        {
            var book = await _bookRepository.GetBookById(bookId);

            var result = order.AddItem(book, quantity);

            if (!result.Success) return Result<Entities.Order>.Failed.AddErrors(result.Errors);
        }

        var validationResult = order.ValidationErrors;
        if (!validationResult.Success) return Result<Entities.Order>.Failed.AddErrors(validationResult.Errors);

        await _orderRepository.AddOrder(order);

        order.AddDomainEvent(new OrderCreatedDomainEvent { Order = order });
        return Result<Entities.Order>.Succeeded.SetValue(order);
    }
}