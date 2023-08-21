using System.Security.Claims;
using AutoFixture;
using BookStore.Api.Controllers;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Dtos.Output;
using BookStore.Domain;
using BookStore.Domain.Order.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Api.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly OrdersController _controller;
    private readonly Fixture _fixture;
    private readonly Mock<IOrderAppService> _mockOrderAppService;

    public OrdersControllerTests()
    {
        _mockOrderAppService = new Mock<IOrderAppService>();
        _controller = new OrdersController(_mockOrderAppService.Object);
        _fixture = new Fixture();

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithOrders()
    {
        // Arrange
        var orders = _fixture.Create<List<OrderDto>>();
        _mockOrderAppService.Setup(service => service.GetUserOrders(It.IsAny<Guid>())).ReturnsAsync(orders);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<OrderDto>>(okResult.Value);
        Assert.Equal(orders, returnValue);
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundResult_WhenOrderDoesNotExist()
    {
        // Arrange
        _mockOrderAppService.Setup(service => service.GetById(It.IsAny<Guid>())).ReturnsAsync((OrderDto)null);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOrder_WhenOrderExists()
    {
        // Arrange
        var order = _fixture.Create<OrderDto>();
        _mockOrderAppService.Setup(service => service.GetById(It.IsAny<Guid>())).ReturnsAsync(order);

        // Act
        var result = await _controller.GetById(Guid.NewGuid());

        // Assert
        Assert.Equal(order, ((OkObjectResult)result.Result)?.Value);
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtActionResult_WithOrder()
    {
        // Arrange
        var orderDto = _fixture.Create<OrderCreationDto>();
        var order = _fixture.Create<Order>();
        _mockOrderAppService.Setup(service => service.PlaceOrder(orderDto))
            .ReturnsAsync(Result<Order>.Succeeded.SetValue(order));

        // Act
        var result = await _controller.Post(orderDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal("GetById", createdAtActionResult.ActionName);
        Assert.Equal(order, createdAtActionResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult_WhenOrderIsCancelled()
    {
        // Arrange
        _mockOrderAppService.Setup(service => service.CancelOrder(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(Result.Succeeded);

        // Act
        var result = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}