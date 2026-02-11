using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class FreshWaterAquariumServiceTests : IClassFixture<CosmosDbFixture>
{
    private readonly FreshWaterAquariumService _service;

    public FreshWaterAquariumServiceTests(CosmosDbFixture fixture)
    {
        var repo = new Repository<FreshWaterAquarium>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);
        _service = new FreshWaterAquariumService(repo, uow);
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
}
