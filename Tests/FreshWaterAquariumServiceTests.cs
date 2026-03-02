using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class FreshWaterAquariumServiceTests : IClassFixture<MongoDbFixture>
{
    private readonly FreshWaterAquariumService _service;
    private readonly SeaWaterAquariumService _seaWaterService;

    public FreshWaterAquariumServiceTests(MongoDbFixture fixture)
    {
        var repo = new Repository<FreshWaterAquarium>(fixture.Context);
        var seaRepo = new Repository<SeaWaterAquarium>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);

        _service = new FreshWaterAquariumService(repo, uow);
        _seaWaterService = new SeaWaterAquariumService(seaRepo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistFreshWaterAquarium()
    {
        var ownerId = Guid.NewGuid().ToString();
        var aquarium = new FreshWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Service FW",
            OwnerId = ownerId,
            Depth = 40,
            Height = 50,
            Length = 100,
            Liters = 200,
            HasFreshAir = true,
            HasCo2System = true
        };

        var result = await _service.CreateAsync(aquarium);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task WhereAsync_WithMixedAquariumTypesForSameOwner_ShouldReturnOnlyFreshWater()
    {
        var ownerId = Guid.NewGuid().ToString();

        var freshWaterAquarium = new FreshWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Mixed FW",
            OwnerId = ownerId,
            Depth = 40,
            Height = 50,
            Length = 100,
            Liters = 200,
            HasFreshAir = true,
            HasCo2System = true
        };

        var seaWaterAquarium = new SeaWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Mixed SW",
            OwnerId = ownerId,
            Depth = 60,
            Height = 70,
            Length = 120,
            Liters = 350,
            HasWaveMachine = true,
            IsReefTank = true
        };

        await _service.CreateAsync(freshWaterAquarium);
        await _seaWaterService.CreateAsync(seaWaterAquarium);

        var result = await _service.WhereAsync(a => a.OwnerId == ownerId);

        result.Should().HaveCount(1);
        result[0].ID.Should().Be(freshWaterAquarium.ID);
    }
}
