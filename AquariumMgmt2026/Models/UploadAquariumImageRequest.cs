using System.ComponentModel.DataAnnotations;

namespace AquariumMgmt2026.Models;

public class UploadAquariumImageRequest
{
    [Required]
    public string AquariumId { get; set; } = string.Empty;

    [Required]
    public IFormFile Image { get; set; } = default!;

    public string? Name { get; set; }

    public string? Description { get; set; }
}
