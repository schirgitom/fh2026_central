using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class CoralServiceTests : IClassFixture<MongoDbFixture>
{
    private readonly CoralService _service;

    public CoralServiceTests(MongoDbFixture fixture)
    {
        var repo = new Repository<Coral>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);
        _service = new CoralService(repo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistCoral()
    {
        var coral = new Coral
        {
            ID = Guid.NewGuid().ToString(),
            AquariumId = Guid.NewGuid().ToString(),
            Name = "Service Coral",
            Inserted = DateTime.UtcNow,
            Amount = 2,
            Description = "Test",
            CoralTyp = CoralTyp.SoftCoral
        };

        var result = await _service.CreateAsync(coral);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
