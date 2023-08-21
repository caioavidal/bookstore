using BookStore.Domain.Entities;

namespace BookStore.Domain.Contracts.Repositories;

public interface IUserRepository
{
    Task<User> GetById(Guid id);
}