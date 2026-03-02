using DAL.Entities;
using DAL.Repository;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class AquariumRepositoryTests: IClassFixture<MongoDbFixture>
{
    private readonly IAquariumRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AquariumRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new AquariumRepository(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistFreshWaterAquarium()
    {
        // Arrange
        var ownerId = Guid.NewGuid().ToString();

        var aquarium = new FreshWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Test Aquarium",
            OwnerId = ownerId,
            Depth = 40,
            Height = 50,
            Length = 100,
            Liters = 200,
            HasFreshAir = true,
            HasCo2System = true
        };

        // Act
        await _repository.InsertAsync(aquarium);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(aquarium.ID);

        // Assert
        loaded.Should().NotBeNull();
        loaded.Should().BeOfType<FreshWaterAquarium>();

        var freshwater = loaded as FreshWaterAquarium;
        freshwater!.HasCo2System.Should().BeTrue();
    }
}
