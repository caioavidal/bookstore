using BookStore.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class ApiController : ControllerBase
{
    protected ActionResult HandleApplicationResult(Result result, Func<ActionResult> responseFunction)
    {
        if (result.Success) return responseFunction?.Invoke();
        return Handle(result.Errors);
    }

    protected ActionResult HandleApplicationResult<T>(Result<T> result, Func<ActionResult> responseFunction)
    {
        if (result.Success) return responseFunction?.Invoke();
        return Handle(result.Errors);
    }

    private ObjectResult Handle(Dictionary<string, string> errors)
    {
        foreach (var error in errors) ModelState.TryAddModelError(error.Key, error.Value);

        return UnprocessableEntity(ModelState);
    }
}