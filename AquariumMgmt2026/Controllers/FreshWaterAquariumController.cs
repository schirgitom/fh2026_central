using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreshWaterAquariumController : GenericController<FreshWaterAquarium>
{
    private readonly IFreshWaterAquariumService _service;
    private readonly ILogger<FreshWaterAquariumController> _logger;

    public FreshWaterAquariumController(IFreshWaterAquariumService service,
        ILogger<FreshWaterAquariumController> logger) : base(service, logger)
    {
        _service = service;
        _logger = logger;
    }

    [Authorize(Policy = "UserOrService")]
    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching freshwater aquariums for user {UserId}", userId);
        var aquariums = await _service.WhereAsync(a => a.OwnerId == userId, cancellationToken);
        _logger.LogInformation("Fetched {Count} freshwater aquariums for user {UserId}", aquariums.Count, userId);
        return Ok(aquariums);
    }
}
