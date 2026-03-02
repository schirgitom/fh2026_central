using DAL.Entities;
using DAL.Repository.Impl;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class CoralRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly ICoralRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CoralRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new CoralRepository(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistCoral()
    {
        var aquariumId = Guid.NewGuid().ToString();

        var coral = new Coral
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = aquariumId,
            Name = "Red Coral",
            Inserted = DateTime.UtcNow,
            Amount = 3,
            Description = "Test coral",
            CoralTyp = CoralTyp.HardCoral
        };

        await _repository.InsertAsync(coral);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(coral.ID);

        loaded.Should().NotBeNull();
        loaded!.CoralTyp.Should().Be(CoralTyp.HardCoral);
        loaded.Amount.Should().Be(3);
    }
}
