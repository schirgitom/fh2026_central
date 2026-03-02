using DAL.Entities;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class SeaWaterAquariumRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly IAquariumRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SeaWaterAquariumRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new AquariumRepository(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistSeaWaterAquarium()
    {
        var ownerId = Guid.NewGuid().ToString();

        var aquarium = new SeaWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Sea Tank",
            OwnerId = ownerId,
            Depth = 60,
            Height = 70,
            Length = 120,
            Liters = 350,
            HasWaveMachine = true
        };

        await _repository.InsertAsync(aquarium);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(aquarium.ID);

        loaded.Should().NotBeNull();
        loaded.Should().BeOfType<SeaWaterAquarium>();

        var seawater = loaded as SeaWaterAquarium;
        seawater!.HasWaveMachine.Should().BeTrue();
    }
}
