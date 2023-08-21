using AutoFixture;
using BookStore.Api.Controllers;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Entities;
using BookStore.CrossCutting.Security.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookStore.Api.Tests.Controllers;

public class TokenControllerTests
{
    private readonly TokenController _controller;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IApplicationUserRepository> _mockUserRepository;

    public TokenControllerTests()
    {
        _mockUserRepository = new Mock<IApplicationUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        var jwtConfiguration = new JwtConfiguration(
            Key: "83a7e17f0a761af9d5ff1af756caf4750389fa69a50d0fa30c00cbcb5455eb59",
            Issuer: "test",
            Audience: "test"
        );

        _controller =
            new TokenController(jwtConfiguration, _mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Fact]
    public async Task CreateToken_ReturnsUnauthorizedResult_WhenUserDoesNotExist()
    {
        // Arrange
        var login = new LoginDto();
        _mockUserRepository.Setup(repo => repo.GetByEmail(login.Email)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _controller.CreateToken(login);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreateToken_ReturnsUnauthorizedResult_WhenPasswordIsInvalid()
    {
        // Arrange
        var login = new LoginDto();
        var user = new ApplicationUser();
        _mockUserRepository.Setup(repo => repo.GetByEmail(login.Email)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.Verify(login.Password, user.PasswordHash, user.Salt))
            .Returns(false);

        // Act
        var result = await _controller.CreateToken(login);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task CreateToken_ReturnsOkResult_WithToken_WhenUserExistsAndPasswordIsValid()
    {
        // Arrange
        var fixture = new Fixture();
        var login = fixture.Create<LoginDto>();
        var user = fixture.Create<ApplicationUser>();
        _mockUserRepository.Setup(repo => repo.GetByEmail(login.Email)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.Verify(login.Password, user.PasswordHash, user.Salt))
            .Returns(true);

        // Act
        var result = await _controller.CreateToken(login);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}