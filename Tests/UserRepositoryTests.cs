using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;

namespace Tests;

public class UserRepositoryTests : IClassFixture<MongoDbFixture>
{
    private readonly Repository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRepositoryTests(MongoDbFixture fixture)
    {
        _repository = new Repository<User>(fixture.Context);
        _unitOfWork = new UnitOfWork(fixture.Context);
    }

    [Fact]
    public async Task InsertAsync_ShouldPersistUser()
    {
        var user = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = "test.user@example.com",
            Firstname = "Test",
            Lastname = "User",
            Password = "pw",
            Active = true
        };

        await _repository.InsertAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var loaded = await _repository.GetAsync(user.ID);

        loaded.Should().NotBeNull();
        loaded!.Email.Should().Be("test.user@example.com");
        loaded.Active.Should().BeTrue();
    }
}
