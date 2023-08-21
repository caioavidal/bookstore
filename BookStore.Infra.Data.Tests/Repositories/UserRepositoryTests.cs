using AutoFixture;
using BookStore.Application.Entities;
using BookStore.Domain.Entities;
using BookStore.Infra.Data.Repositories;
using BookStore.Infra.Data.Repositories.Contracts;
using FluentAssertions;
using Moq;

namespace BookStore.Infra.Data.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly Fixture _fixture;

    public UserRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetByEmail_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedUser = _fixture.Create<ApplicationUser>();

        dapperWrapperMock.Setup(d => d.QueryFirstOrDefaultAsync<ApplicationUser>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null,
            null,
            null
        )).ReturnsAsync(expectedUser);

        var repository = new UserRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetByEmail(email);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task AddUser_AddsUserSuccessfully()
    {
        // Arrange
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var userToAdd = _fixture.Create<ApplicationUser>();
        var repository = new UserRepository(dapperWrapperMock.Object);

        // Act
        await repository.AddUser(userToAdd);

        // Assert
        dapperWrapperMock.Verify(d => d.ExecuteAsync(
            It.IsAny<string>(),
            It.Is<ApplicationUser>(user =>
                user.Id == userToAdd.Id &&
                user.Name == userToAdd.Name &&
                user.BirthDate == userToAdd.BirthDate &&
                user.Email == userToAdd.Email &&
                user.PasswordHash == userToAdd.PasswordHash &&
                user.Role == userToAdd.Role &&
                user.Salt == userToAdd.Salt
            ),
            null,
            null,
            null
        ), Times.Once);
    }

    [Fact]
    public async Task GetById_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dapperWrapperMock = new Mock<IDapperWrapper>();
        var expectedUser = _fixture.Create<User>();

        dapperWrapperMock.Setup(d => d.QueryFirstOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null,
            null,
            null
        )).ReturnsAsync(expectedUser);

        var repository = new UserRepository(dapperWrapperMock.Object);

        // Act
        var result = await repository.GetById(userId);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }
}