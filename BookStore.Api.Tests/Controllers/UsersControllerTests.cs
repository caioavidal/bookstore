using BookStore.Api.Controllers;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Api.Tests.Controllers;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly Mock<IUserAppService> _mockUserAppService;

    public UsersControllerTests()
    {
        _mockUserAppService = new Mock<IUserAppService>();
        _controller = new UsersController(_mockUserAppService.Object);
    }

    [Fact]
    public async Task Post_ReturnsOkResult_WhenUserIsAdded()
    {
        // Arrange
        var user = new UserInputDto();
        _mockUserAppService.Setup(service => service.AddUser(user)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(user);

        // Assert
        Assert.IsType<OkResult>(result);
    }
}