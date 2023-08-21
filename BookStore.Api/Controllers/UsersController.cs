using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserAppService _userAppService;

    public UsersController(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    // POST: api/Users
    [HttpPost]
    public async Task<ActionResult> Post(UserInputDto userInput)
    {
        await _userAppService.AddUser(userInput);
        return Ok();
    }
}