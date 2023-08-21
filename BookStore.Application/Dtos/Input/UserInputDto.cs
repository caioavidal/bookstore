using System.ComponentModel.DataAnnotations;

namespace BookStore.Application.Dtos.Input;

public class UserInputDto
{
    [Required] public string Name { get; set; }
    [Required] public DateTime BirthDate { get; set; }
    [Required] public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Role { get; set; }
}