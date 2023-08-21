using BookStore.Domain.Entities;

namespace BookStore.Application.Entities;

public class ApplicationUser : User
{
    public string PasswordHash { get; init; }
    public byte[] Salt { get; init; }
    public string Role { get; init; }
}