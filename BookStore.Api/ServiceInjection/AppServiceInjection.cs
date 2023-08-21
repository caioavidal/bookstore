using BookStore.Application.Contracts;
using BookStore.Application.Services;

namespace BookStore.Api.ServiceInjection;

public static class AppServiceInjection
{
    public static IServiceCollection AddAppServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBookAppService, BookAppService>();
        serviceCollection.AddScoped<IOrderAppService, OrderAppService>();
        serviceCollection.AddScoped<IUserAppService, UserAppService>();
        serviceCollection.AddScoped<IInventoryAppService, InventoryAppService>();

        return serviceCollection;
    }
}