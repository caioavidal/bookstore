using BookStore.Application.Entities;

namespace BookStore.Application.Contracts;

public interface IApplicationUserRepository
{
    Task<ApplicationUser> GetByEmail(string email);
    Task AddUser(ApplicationUser user);
}