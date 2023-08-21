using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Entities;

namespace BookStore.Application.Services;

public class UserAppService : IUserAppService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IApplicationUserRepository _userRepository;

    public UserAppService(IApplicationUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task AddUser(UserInputDto userInput)
    {
        var passwordHash = _passwordHasher.Hash(userInput.Password, out var salt);
        return _userRepository.AddUser(new ApplicationUser
        {
            Email = userInput.Email,
            Name = userInput.Name,
            Role = userInput.Role,
            BirthDate = userInput.BirthDate,
            PasswordHash = passwordHash,
            Salt = salt
        });
    }
}