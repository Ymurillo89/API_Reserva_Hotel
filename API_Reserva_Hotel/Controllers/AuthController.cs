using API_Hotel.Infrastructure.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtAuthService _jwtAuthService;

    public AuthController(IJwtAuthService jwtAuthService)
    {
        _jwtAuthService = jwtAuthService;
    }

    public record LoginRequest(string Username, string Role);

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var role = request.Role.Equals("Agente", StringComparison.OrdinalIgnoreCase) ? "Agente" : "Viajero";
        var token = _jwtAuthService.GenerateToken(request.Username, role);

        return Ok(new
        {
            Token = token,
            TokenType = "Bearer",
            Username = request.Username,
            Role = role
        });
    }
}