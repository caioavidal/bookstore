using BookStore.Application.Dtos.Input;

namespace BookStore.Application.Contracts;

public interface IUserAppService
{
    Task AddUser(UserInputDto userInput);
}