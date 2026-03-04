using AquariumMgmt2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [AllowAnonymous]
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
        var token = GenerateToken(user);
        var expiresInMinutes = ParseExpiresInMinutes();

        return Ok(new LoginResponse
        {
            Token = token,
            TokenType = "Bearer",
            ExpiresInMinutes = expiresInMinutes,
            UserId = user.ID,
            Email = user.Email
        });
    }

    private string GenerateToken(DAL.Entities.User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var key = jwtSection["Key"]!;
        var expiresInMinutes = ParseExpiresInMinutes();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.ID),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int ParseExpiresInMinutes()
    {
        var value = _configuration["Jwt:ExpiresInMinutes"];
        return int.TryParse(value, out var minutes) && minutes > 0 ? minutes : 60;
    }
}
