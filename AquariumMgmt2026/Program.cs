using DAL;
using DAL.Repository;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Services;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var cosmosSection = builder.Configuration.GetSection("Cosmos");
var accountEndpoint = cosmosSection["AccountEndpoint"];
var accountKey = cosmosSection["AccountKey"];
var databaseName = cosmosSection["DatabaseName"];
var allowInsecureTlsForEmulator = builder.Environment.IsDevelopment() &&
                                  cosmosSection.GetValue<bool>("AllowInsecureTlsForEmulator");

if (string.IsNullOrWhiteSpace(accountEndpoint) ||
    string.IsNullOrWhiteSpace(accountKey) ||
    string.IsNullOrWhiteSpace(databaseName))
{
    throw new InvalidOperationException(
        "Cosmos configuration is missing. Please set Cosmos:AccountEndpoint, Cosmos:AccountKey and Cosmos:DatabaseName.");
}

builder.Services.AddDbContext<AquariumDBContext>(options =>
{
    options.UseCosmos(
        accountEndpoint,
        accountKey,
        databaseName,
        cosmosOptionsAction: cosmosOptions =>
        {
            if (!allowInsecureTlsForEmulator)
            {
                return;
            }

            // Development-only fallback for local emulator setups with untrusted certificates.
            cosmosOptions.HttpClientFactory(() =>
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(handler);
            });
        });
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICoralService, CoralService>();
builder.Services.AddScoped<IFishService, FishService>();
builder.Services.AddScoped<IFreshWaterAquariumService, FreshWaterAquariumService>();
builder.Services.AddScoped<ISeaWaterAquariumService, SeaWaterAquariumService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
