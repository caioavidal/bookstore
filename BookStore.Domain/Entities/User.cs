namespace BookStore.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
}