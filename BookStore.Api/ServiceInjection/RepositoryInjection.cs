using BookStore.Application.Contracts;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Inventory.Contracts;
using BookStore.Domain.Order.Contracts;
using BookStore.Infra.Data.Repositories;
using BookStore.Infra.Data.Repositories.Contracts;

namespace BookStore.Api.ServiceInjection;

public static class RepositoryInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBookRepository, BookRepository>();
        serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IApplicationUserRepository, UserRepository>();
        serviceCollection.AddScoped<IInventoryRepository, InventoryRepository>();
        serviceCollection.AddScoped<IDapperWrapper, DapperWrapper>();

        return serviceCollection;
    }
}