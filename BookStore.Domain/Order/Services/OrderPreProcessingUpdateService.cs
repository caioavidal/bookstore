using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;

namespace BookStore.Domain.Order.Services;

public class OrderPreProcessingUpdateService : IOrderPreProcessingUpdateService
{
    private readonly IOrderRepository _orderRepository;

    public OrderPreProcessingUpdateService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Update(Entities.Order order, IEnumerable<Inventory.Entities.Inventory> inventoryItems)
    {
        var orderCanProceed = OrderCanProceed(order, inventoryItems);

        ChangeOrderStatus(orderCanProceed, order);

        await _orderRepository.UpdateOrderStatus(order);
    }

    private static bool OrderCanProceed(Entities.Order order, IEnumerable<Inventory.Entities.Inventory> inventoryItems)
    {
        var mappedInventoryItems = inventoryItems.ToDictionary(x => x.Book.Id, x => x);

        var orderCanProceed = true;

        foreach (var orderItem in order.OrderItems)
        {
            mappedInventoryItems.TryGetValue(orderItem.Book.Id, out var inventoryItem);

            if (inventoryItem is null)
            {
                orderCanProceed = false;
                break;
            }

            if (orderItem.Quantity > inventoryItem.Quantity)
            {
                orderCanProceed = false;
                break;
            }
        }

        return orderCanProceed;
    }

    private static void ChangeOrderStatus(bool orderCanProceed, Entities.Order order)
    {
        if (!orderCanProceed)
        {
            order.ChangeStatus(OrderState.Cancelled);
            return;
        }

        order.ChangeStatus(OrderState.Processing);
    }
}