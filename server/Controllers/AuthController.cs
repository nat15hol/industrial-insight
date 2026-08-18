using Microsoft.AspNetCore.Mvc;
using server.DTOs;
using server.Services;

namespace server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request);

        if (user == null)
        {
            return Conflict("A user with that email already exists, or the Technician role is missing.");
        }

        return Created("", new
        {
            user.UserId,
            user.Name,
            user.Email,
            user.RoleId
        });
    }
}
