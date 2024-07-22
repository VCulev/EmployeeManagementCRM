using MainLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace WebApplication1.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
{
    [HttpPost("register")]

    public async Task<IActionResult> CreateAsync(Register user)
    {
        if (user is null) return BadRequest("Model is empty!");
        var result = await accountInterface.CreateAsync(user);
        return Ok(result);

    }
}