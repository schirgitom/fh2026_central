using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoralController : GenericController<Coral>
{
    private readonly ICoralService _service;
    private readonly ILogger<CoralController> _logger;

    public CoralController(ICoralService service, ILogger<CoralController> logger) : base(service, logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("by-aquarium/{aquariumId}")]
    public async Task<IActionResult> GetByAquarium(string aquariumId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching corals for aquarium {AquariumId}", aquariumId);
        var corals = await _service.WhereAsync(c => c.AquariumId == aquariumId, cancellationToken);
        _logger.LogInformation("Fetched {Count} corals for aquarium {AquariumId}", corals.Count, aquariumId);
        return Ok(corals);
    }
}
