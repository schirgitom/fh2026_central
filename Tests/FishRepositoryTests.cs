using DAL.Entities;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class FishRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly IFishRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public FishRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new FishRepository(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistFishWithDefaultDeathDate()
    {
        var aquariumId = Guid.NewGuid().ToString();

        var fish = new Fish
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumId,
            Name = "Blue Fish",
            Inserted = DateTime.UtcNow,
            Amount = 5,
            Description = "Test fish"
        };

        await _repository.InsertAsync(fish);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(fish.ID);

        loaded.Should().NotBeNull();
        loaded!.DeathDate.Should().Be(DateTime.MinValue);
        loaded.Amount.Should().Be(5);
    }
}
