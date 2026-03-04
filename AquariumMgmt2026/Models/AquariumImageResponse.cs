using DAL.Entities;

namespace AquariumMgmt2026.Models;

public class AquariumImageResponse
{
    public string Id { get; set; } = string.Empty;
    public string AquariumId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime UploadedAtUtc { get; set; }

    public static AquariumImageResponse FromEntity(AquariumImage image)
    {
        return new AquariumImageResponse
        {
            Id = image.ID,
            AquariumId = image.AquariumId,
            FileName = image.FileName,
            Name = image.Name,
            Description = image.Description,
            ContentType = image.ContentType,
            SizeInBytes = image.Data.LongLength,
            UploadedAtUtc = image.UploadedAtUtc
        };
    }
}
