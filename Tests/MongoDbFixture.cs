using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Tests;

public class 
    MongoDbFixture : IDisposable
{
    public AquariumDBContext Context { get; }

    public MongoDbFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .Build();

        var connectionString = configuration["MongoDb:ConnectionString"]!;
        var databaseName = configuration["MongoDb:DatabaseName"]!;
        var isolatedDatabaseName = $"{databaseName}_{Guid.NewGuid():N}";
        var mongoClient = new MongoClient(connectionString);

        var options = new DbContextOptionsBuilder<AquariumDBContext>()
            .UseMongoDB(mongoClient, isolatedDatabaseName)
            .Options;

        Context = new AquariumDBContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
