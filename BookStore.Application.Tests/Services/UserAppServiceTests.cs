using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Entities;
using BookStore.Application.Services;
using Moq;

namespace BookStore.Application.Tests.Services;

public class UserAppServiceTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IApplicationUserRepository> _mockUserRepository;
    private readonly UserAppService _userAppService;

    public UserAppServiceTests()
    {
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockUserRepository = new Mock<IApplicationUserRepository>();
        _userAppService = new UserAppService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object
        );
    }

    [Fact]
    public async Task AddUser_CallsRepositoryWithCorrectParameters()
    {
        // Arrange
        var userDto = new UserInputDto
        {
            Email = "test@example.com",
            Name = "John Doe",
            Role = "User",
            BirthDate = new DateTime(1990, 1, 1),
            Password = "password123"
        };
        var passwordHash = "hashedPassword";
        var salt = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        _mockPasswordHasher.Setup(hasher =>
                hasher.Hash(userDto.Password, out salt))
            .Returns(passwordHash);

        // Act
        await _userAppService.AddUser(userDto);

        // Assert
        _mockUserRepository.Verify(repository => repository.AddUser(
                It.Is<ApplicationUser>(user =>
                    user.Email == userDto.Email &&
                    user.Name == userDto.Name &&
                    user.Role == userDto.Role &&
                    user.BirthDate == userDto.BirthDate &&
                    user.PasswordHash == passwordHash &&
                    user.Salt == salt)),
            Times.Once);
    }
}