using DAL.Entities;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class AnimalRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly IAnimalRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AnimalRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new AnimalRepository(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistDerivedAnimal()
    {
        var aquariumId = Guid.NewGuid().ToString();

        var fish = new Fish
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumId,
            Name = "Goldfish",
            Inserted = DateTime.UtcNow,
            Amount = 2,
            Description = "Derived animal"
        };

        await _repository.InsertAsync(fish);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(fish.ID);

        loaded.Should().NotBeNull();
        loaded.Should().BeOfType<Fish>();
        loaded!.Name.Should().Be("Goldfish");
    }
}
