using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class AquariumImageServiceTests : IClassFixture<MongoDbFixture>
{
    private readonly AquariumImageService _service;
    private readonly FreshWaterAquariumService _aquariumService;

    public AquariumImageServiceTests(MongoDbFixture fixture)
    {
        var imageRepo = new Repository<AquariumImage>(fixture.Context);
        var aquariumRepo = new Repository<Aquarium>(fixture.Context);
        var freshRepo = new Repository<FreshWaterAquarium>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);

        _service = new AquariumImageService(imageRepo, aquariumRepo, uow);
        _aquariumService = new FreshWaterAquariumService(freshRepo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistImageWithMetadata()
    {
        var aquariumId = await CreateAquariumAsync();
        var uploadedAt = DateTime.UtcNow;
        var image = new AquariumImage
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumId,
            FileName = "aquarium.jpg",
            Name = "Front View",
            Description = "Main tank shot",
            ContentType = "image/jpeg",
            UploadedAtUtc = uploadedAt,
            Data = [1, 2, 3]
        };

        var result = await _service.CreateAsync(image);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AquariumId.Should().Be(aquariumId);
        result.Data.Name.Should().Be("Front View");
        result.Data.Description.Should().Be("Main tank shot");
        result.Data.UploadedAtUtc.Should().Be(uploadedAt);
    }

    [Fact]
    public async Task GetImagesForAquariumAsync_ShouldReturnOnlyMatchingImages()
    {
        var aquariumA = await CreateAquariumAsync();
        var aquariumB = await CreateAquariumAsync();

        await _service.CreateAsync(new AquariumImage
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumA,
            FileName = "a.jpg",
            Name = "A",
            ContentType = "image/jpeg",
            UploadedAtUtc = DateTime.UtcNow,
            Data = [10]
        });

        await _service.CreateAsync(new AquariumImage
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumB,
            FileName = "b.jpg",
            Name = "B",
            ContentType = "image/jpeg",
            UploadedAtUtc = DateTime.UtcNow,
            Data = [20]
        });

        var images = await _service.GetImagesForAquariumAsync(aquariumA);

        images.Should().HaveCount(1);
        images[0].AquariumId.Should().Be(aquariumA);
        images[0].FileName.Should().Be("a.jpg");
    }

    [Fact]
    public async Task AquariumExistsAsync_ShouldReturnFalseForUnknownAquarium()
    {
        var exists = await _service.AquariumExistsAsync(Guid.NewGuid().ToString());

        exists.Should().BeFalse();
    }

    private async Task<string> CreateAquariumAsync()
    {
        var aquariumId = Guid.NewGuid().ToString();
        await _aquariumService.CreateAsync(new FreshWaterAquarium
        {
            ID = aquariumId,
            Name = $"Aquarium-{aquariumId}",
            OwnerId = Guid.NewGuid().ToString(),
            Depth = 40,
            Height = 50,
            Length = 100,
            Liters = 200,
            HasFreshAir = true,
            HasCo2System = false
        });

        return aquariumId;
    }
}
