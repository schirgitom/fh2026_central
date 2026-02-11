using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class FishServiceTests : IClassFixture<CosmosDbFixture>
{
    private readonly FishService _service;

    public FishServiceTests(CosmosDbFixture fixture)
    {
        var repo = new Repository<Fish>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);
        _service = new FishService(repo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistFish()
    {
        var fish = new Fish
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = Guid.NewGuid().ToString(),
            Name = "Service Fish",
            Inserted = DateTime.UtcNow,
            Amount = 4,
            Description = "Test"
        };

        var result = await _service.CreateAsync(fish);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
