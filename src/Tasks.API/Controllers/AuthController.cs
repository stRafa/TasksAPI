using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Tasks.Application.DTOs.Identity;
using Tasks.Application.DTOs.User;
using Tasks.Application.Interfaces;

namespace Tasks.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(CreateUserDTO model)
    {
        var result = await _authService.Register(model);

        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var result = await _authService.Login(model);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}