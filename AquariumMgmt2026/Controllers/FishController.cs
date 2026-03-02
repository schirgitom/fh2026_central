using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FishController : GenericController<Fish>
{
    private readonly IFishService _service;
    private readonly ILogger<FishController> _logger;

    public FishController(IFishService service, ILogger<FishController> logger) : base(service, logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("by-aquarium/{aquariumId}")]
    public async Task<IActionResult> GetByAquarium(string aquariumId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching fish for aquarium {AquariumId}", aquariumId);
        var fish = await _service.WhereAsync(f => f.AquariumId == aquariumId, cancellationToken);
        _logger.LogInformation("Fetched {Count} fish rows for aquarium {AquariumId}", fish.Count, aquariumId);
        return Ok(fish);
    }
}
