using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using FluentAssertions;
using Services.Services;

namespace Tests;

public class UserServiceTests : IClassFixture<CosmosDbFixture>
{
    private readonly UserService _service;

    public UserServiceTests(CosmosDbFixture fixture)
    {
        var repo = new Repository<User>(fixture.Context);
        var uow = new UnitOfWork(fixture.Context);
        _service = new UserService(repo, uow);
    }

    [Fact]
    public async Task CreateAsync_ShouldHashPassword()
    {
        var user = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = $"user{Guid.NewGuid()}@example.com",
            Firstname = "Test",
            Lastname = "User",
            Password = "Str0ng!Passw0rd",
            Active = true
        };

        var result = await _service.CreateAsync(user);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.HashedPassword.Should().NotBeNullOrWhiteSpace();
        result.Data.Password.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnUser_ForValidPassword()
    {
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Str0ng!Passw0rd";

        var user = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = email,
            Firstname = "Test",
            Lastname = "User",
            Password = password,
            Active = true
        };

        await _service.CreateAsync(user);

        var loggedIn = await _service.AuthenticateAsync(email, password);

        loggedIn.Should().NotBeNull();
        loggedIn!.Email.Should().Be(email);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_ForInvalidPassword()
    {
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Str0ng!Passw0rd";

        var user = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = email,
            Firstname = "Test",
            Lastname = "User",
            Password = password,
            Active = true
        };

        await _service.CreateAsync(user);

        var loggedIn = await _service.AuthenticateAsync(email, "WrongPassword");

        loggedIn.Should().BeNull();
    }
}
