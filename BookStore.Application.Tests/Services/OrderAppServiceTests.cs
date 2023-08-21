using AutoFixture;
using AutoMapper;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Application.Mappers;
using BookStore.Application.Services;
using BookStore.Domain;
using BookStore.Domain.Entities;
using BookStore.Domain.Order.Contracts;
using BookStore.Domain.Order.Entities;
using FluentAssertions;
using MediatR;
using Moq;

namespace BookStore.Application.Tests.Services;

public class OrderAppServiceTests
{
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;
    private readonly Mock<IOrderPlacementService> _mockOrderPlacementService;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly OrderAppService _orderAppService;

    public OrderAppServiceTests()
    {
        _mockOrderPlacementService = new Mock<IOrderPlacementService>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            // Configure AutoMapper mappings
            cfg.CreateMap<OrderCreationDto, Order>();
            cfg.CreateMap<Order, OrderDto>();
        });
        _mapper = AutoMapperConfig.Setup();
        _orderAppService = new OrderAppService(
            _mockOrderPlacementService.Object,
            Mock.Of<IMediator>(),
            _mockOrderRepository.Object,
            _mapper
        );
        _fixture = new Fixture();
    }

    [Fact]
    public async Task PlaceOrder_ReturnsSuccessResult_WhenOrderPlacementSucceeds()
    {
        // Arrange
        var orderCreationDto = _fixture.Create<OrderCreationDto>();
        var items = orderCreationDto.Items.Select(x => (x.BookId, x.Quantity)).ToList();
        var createdOrder = _fixture.Create<Order>();
        var successResult = Result<Order>.Succeeded.SetValue(createdOrder);

        _mockOrderPlacementService.Setup(service =>
            service.PlaceOrder(orderCreationDto.UserId, items)
        ).ReturnsAsync(successResult);

        // Act
        var result = await _orderAppService.PlaceOrder(orderCreationDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(createdOrder);
        _mockOrderPlacementService.Verify(service =>
                service.PlaceOrder(orderCreationDto.UserId, items),
            Times.Once
        );
    }

    [Fact]
    public async Task PlaceOrder_ReturnsFailedResult_WhenOrderPlacementFails()
    {
        // Arrange
        var orderCreationDto = _fixture.Create<OrderCreationDto>();
        var items = orderCreationDto.Items.Select(x => (x.BookId, x.Quantity)).ToList();
        var failedResult = Result<Order>.Failed.AddError("Order placement", "Order placement failed");

        _mockOrderPlacementService.Setup(service =>
            service.PlaceOrder(orderCreationDto.UserId, items)
        ).ReturnsAsync(failedResult);

        // Act
        var result = await _orderAppService.PlaceOrder(orderCreationDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Values.Should().Contain("Order placement failed");
        _mockOrderPlacementService.Verify(service =>
                service.PlaceOrder(orderCreationDto.UserId, items),
            Times.Once
        );
    }

    [Fact]
    public async Task CancelOrder_ReturnsFailedResult_WhenUserIsNotAuthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var order = new Order(new User { Id = Guid.NewGuid() });
        var unauthorizedResult = Result.Failed.AddError("Invalid user", "You cannot cancel this order");
        _mockOrderRepository.Setup(repo => repo.GetById(orderId)).ReturnsAsync(order);

        // Act
        var result = await _orderAppService.CancelOrder(userId, orderId);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Keys.Should().Contain("Invalid user");
        result.Errors.Values.Should().Contain("You cannot cancel this order");
        _mockOrderRepository.Verify(repo => repo.GetById(orderId), Times.Once);
    }

    [Fact]
    public async Task CancelOrder_ReturnsSuccessResult_WhenOrderIsCanceled()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var order = new Order(new User { Id = userId });

        _mockOrderRepository.Setup(repo => repo.GetById(orderId)).ReturnsAsync(order);
        _mockOrderRepository.Setup(repo => repo.UpdateOrderStatus(order)).Returns(Task.CompletedTask);

        // Act
        var result = await _orderAppService.CancelOrder(userId, orderId);

        // Assert
        result.Success.Should().BeTrue();
        _mockOrderRepository.Verify(repo => repo.GetById(orderId), Times.Once);
        _mockOrderRepository.Verify(repo => repo.UpdateOrderStatus(order), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsOrderDto()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = _fixture.Create<Order>();
        _mockOrderRepository.Setup(repo => repo.GetById(orderId)).ReturnsAsync(order);

        // Act
        var result = await _orderAppService.GetById(orderId);

        // Assert
        var expected = _mapper.Map<OrderDto>(order);
        result.Should().BeEquivalentTo(expected);
        _mockOrderRepository.Verify(repo => repo.GetById(orderId), Times.Once);
    }

    [Fact]
    public async Task GetUserOrders_ReturnsListOfOrderDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orders = _fixture.CreateMany<Order>().ToList();
        _mockOrderRepository.Setup(repo => repo.GetByUserId(userId)).ReturnsAsync(orders);

        // Act
        var result = await _orderAppService.GetUserOrders(userId);

        // Assert
        var expected = _mapper.Map<List<OrderDto>>(orders);
        result.Should().BeEquivalentTo(expected);
        _mockOrderRepository.Verify(repo => repo.GetByUserId(userId), Times.Once);
    }
}