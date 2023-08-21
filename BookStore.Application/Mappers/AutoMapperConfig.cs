using AutoMapper;
using BookStore.Application.Dtos.Output;
using BookStore.Domain.Entities;
using BookStore.Domain.Inventory.Entities;
using BookStore.Domain.Order.Entities;

namespace BookStore.Application.Mappers;

public static class AutoMapperConfig
{
    public static IMapper Setup()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, OutOfStockBookDto>();
            cfg.CreateMap<Book, BookDto>();

            cfg.CreateMap<OrderStatus, OrderDto.OrderStatusDto>();
            cfg.CreateMap<User, OrderDto.UserDto>();
            cfg.CreateMap<OrderItem, OrderDto.OrderItemDto>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(x => x.Book.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(x => x.Book.Title));

            cfg.CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(x => x.OrderItems));
        });

        configuration.AssertConfigurationIsValid();

        return configuration.CreateMapper();
    }
}