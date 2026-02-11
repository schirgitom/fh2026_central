using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class SeaWaterAquariumServiceTests : IClassFixture<CosmosDbFixture>
{
    private readonly SeaWaterAquariumService _service;

    public SeaWaterAquariumServiceTests(CosmosDbFixture fixture)
    {
        var repo = new Repository<SeaWaterAquarium>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);
        _service = new SeaWaterAquariumService(repo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistSeaWaterAquarium()
    {
        var ownerId = Guid.NewGuid().ToString();
        var aquarium = new SeaWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Service SW",
            OwnerId = ownerId,
            Depth = 60,
            Height = 70,
            Length = 120,
            Liters = 350,
            HasWaveMachine = true
        };

        var result = await _service.CreateAsync(aquarium);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
