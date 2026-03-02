using AquariumMgmt2026.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for {Email}", request.Email);
        var user = await _userService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Unauthorized login attempt for {Email}", request.Email);
            return Unauthorized();
        }

        _logger.LogInformation("Successful login for {Email}", request.Email);
        return Ok(user);
    }
}
