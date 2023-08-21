using System.Data;
using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using BookStore.Domain.Order.Events;
using MediatR;

namespace BookStore.Application.EventHandlers.Domain;

public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IDbConnection _dbConnection;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IItemAvailabilityService _itemAvailabilityService;
    private readonly IOrderPreProcessingUpdateService _orderPreProcessingUpdateService;

    public OrderCreatedDomainEventHandler(
        IInventoryRepository inventoryRepository,
        IOrderPreProcessingUpdateService orderPreProcessingUpdateService,
        IItemAvailabilityService itemAvailabilityService,
        IDbConnection dbConnection)
    {
        _inventoryRepository = inventoryRepository;
        _orderPreProcessingUpdateService = orderPreProcessingUpdateService;
        _itemAvailabilityService = itemAvailabilityService;
        _dbConnection = dbConnection;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = notification.Order;

        using var transaction = _dbConnection.BeginTransaction();

        var inventoryItems = (await _inventoryRepository.GetOrderInventories(order.Id)).ToList();

        await _orderPreProcessingUpdateService.Update(order, inventoryItems);

        if (order.LatestStatus == OrderState.Processing)
            _itemAvailabilityService.UpdateItemsQuantity(order, inventoryItems);

        foreach (var inventoryItem in inventoryItems) await _inventoryRepository.UpdateItemQuantity(inventoryItem);

        transaction.Commit();
    }
}