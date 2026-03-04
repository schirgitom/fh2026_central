using AquariumMgmt2026.Models;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AquariumImageController : ControllerBase
{
    private readonly IAquariumImageService _imageService;
    private readonly ILogger<AquariumImageController> _logger;

    public AquariumImageController(IAquariumImageService imageService, ILogger<AquariumImageController> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadAquariumImageRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Image.Length == 0)
        {
            return BadRequest("Die Datei ist leer.");
        }

        var aquariumExists = await _imageService.AquariumExistsAsync(request.AquariumId, cancellationToken);
        if (!aquariumExists)
        {
            return NotFound($"Aquarium mit ID '{request.AquariumId}' wurde nicht gefunden.");
        }

        await using var memoryStream = new MemoryStream();
        await request.Image.CopyToAsync(memoryStream, cancellationToken);

        var image = new AquariumImage
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = request.AquariumId,
            FileName = request.Image.FileName,
            Name = string.IsNullOrWhiteSpace(request.Name) ? request.Image.FileName : request.Name.Trim(),
            Description = request.Description?.Trim(),
            ContentType = string.IsNullOrWhiteSpace(request.Image.ContentType)
                ? "application/octet-stream"
                : request.Image.ContentType,
            UploadedAtUtc = DateTime.UtcNow,
            Data = memoryStream.ToArray()
        };

        var result = await _imageService.CreateAsync(image, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return BadRequest(new { result.Errors, result.Warnings });
        }

        _logger.LogInformation("Stored image {ImageId} for aquarium {AquariumId}", result.Data.ID, request.AquariumId);
        return Ok(AquariumImageResponse.FromEntity(result.Data));
    }

    [HttpGet("GetImagesForAquarium/{aquariumId}")]
    public async Task<IActionResult> GetImagesForAquarium(string aquariumId, CancellationToken cancellationToken)
    {
        var images = await _imageService.GetImagesForAquariumAsync(aquariumId, cancellationToken);
        var response = images.Select(AquariumImageResponse.FromEntity).ToList();
        return Ok(response);
    }

    [HttpGet("{imageId}/content")]
    public async Task<IActionResult> GetContent(string imageId, CancellationToken cancellationToken)
    {
        var image = await _imageService.GetAsync(imageId, cancellationToken);
        if (image == null)
        {
            return NotFound();
        }

        return File(image.Data, image.ContentType, image.FileName);
    }

    [HttpDelete("{imageId}")]
    public async Task<IActionResult> Delete(string imageId, CancellationToken cancellationToken)
    {
        var image = await _imageService.GetAsync(imageId, cancellationToken);
        if (image == null)
        {
            return NotFound();
        }

        var result = await _imageService.DeleteAsync(imageId, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new { result.Errors, result.Warnings });
        }

        _logger.LogInformation("Deleted image {ImageId} for aquarium {AquariumId}", imageId, image.AquariumId);
        return Ok();
    }
}
