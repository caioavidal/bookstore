namespace BookStore.Application.Contracts;

public interface IPasswordHasher
{
    string Hash(string password, out byte[] salt);
    bool Verify(string password, string hash, byte[] salt);
}