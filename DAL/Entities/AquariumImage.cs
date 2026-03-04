using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class AquariumImage : Entity
{
    [Required]
    public string AquariumId { get; set; } = string.Empty;

    [Required]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime UploadedAtUtc { get; set; }

    [Required]
    public string ContentType { get; set; } = string.Empty;

    [Required]
    public byte[] Data { get; set; } = [];
}
