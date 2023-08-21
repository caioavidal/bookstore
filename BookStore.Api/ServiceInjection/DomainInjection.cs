using BookStore.Application.EventHandlers.Domain;
using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Inventory.Services;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Events;
using BookStore.Domain.Order.Services;
using MediatR;

namespace BookStore.Api.ServiceInjection;

public static class DomainInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IOrderPlacementService, OrderPlacementService>();
        serviceCollection.AddScoped<IOrderPreProcessingUpdateService, OrderPreProcessingUpdateService>();
        serviceCollection.AddScoped<IItemAvailabilityService, ItemAvailabilityService>();
        return serviceCollection;
    }

    public static IServiceCollection AddDomainEvents(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<INotificationHandler<OrderCreatedDomainEvent>, OrderCreatedDomainEventHandler>();

        return serviceCollection;
    }
}