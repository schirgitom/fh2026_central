using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tests;

public class CosmosDbFixture : IDisposable
{
    public AquariumDBContext Context { get; }

    public CosmosDbFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build();

        var options = new DbContextOptionsBuilder<AquariumDBContext>()
            .UseCosmos(
                configuration["Cosmos:AccountEndpoint"]!,
                configuration["Cosmos:AccountKey"]!,
                configuration["Cosmos:DatabaseName"]!
            )
            .Options;

        Context = new AquariumDBContext(options);

        // Cosmos erstellt DB & Container beim ersten Zugriff
        Context.Database.EnsureCreatedAsync()
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}