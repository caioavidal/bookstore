using BookStore.Application.Contracts;
using BookStore.Application.Entities;
using BookStore.Domain.Contracts.Repositories;
using BookStore.Domain.Entities;
using BookStore.Infra.Data.Repositories.Contracts;

namespace BookStore.Infra.Data.Repositories;

public class UserRepository : IUserRepository, IApplicationUserRepository
{
    private readonly IDapperWrapper _dapperWrapper;

    public UserRepository(IDapperWrapper dapperWrapper)
    {
        _dapperWrapper = dapperWrapper;
    }

    public Task<ApplicationUser> GetByEmail(string email)
    {
        const string sql = """
                           SELECT id, name, birth_date BirthDate, Email, password_hash PasswordHash, role, salt from Users
                                               WHERE email = @Email
                           """;

        return _dapperWrapper.QueryFirstOrDefaultAsync<ApplicationUser>(sql, new { Email = email });
    }

    public Task AddUser(ApplicationUser user)
    {
        const string sql = """
                           INSERT INTO Users (id, name, birth_date, Email, password_hash, role, salt)
                            VALUES
                            (@Id, @Name, @BirthDate, @Email, @PasswordHash, @Role, @Salt)
                           """;
        return _dapperWrapper.ExecuteAsync(sql, user);
    }

    public Task<User> GetById(Guid id)
    {
        const string sql = """
                           SELECT id, name, birth_date BirthDate, Email, password_hash PasswordHash, role, salt from Users
                                               WHERE Id = @Id
                           """;

        return _dapperWrapper.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }
}