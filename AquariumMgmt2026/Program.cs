using DAL;
using DAL.Repository;
using DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Services.Interfaces;
using Services.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "AquariumMgmt2026");
    });

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Aquarium Management API",
            Version = "v1",
            Description = "API for managing aquariums, fish, corals, and users."
        });
    });

    var mongoSection = builder.Configuration.GetSection("MongoDb");
    var connectionString = mongoSection["ConnectionString"]?.Trim();
    var databaseName = mongoSection["DatabaseName"]?.Trim();

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException(
            "MongoDb configuration is missing. Please set MongoDb:DatabaseName.");
    }

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "MongoDb configuration is missing. Please set MongoDb:ConnectionString.");
    }

    if (builder.Environment.IsDevelopment())
    {
        var mongoUrl = MongoUrl.Create(connectionString);
        var server = mongoUrl.Server?.ToString() ?? "<unknown>";
        var user = string.IsNullOrWhiteSpace(mongoUrl.Username) ? "<none>" : mongoUrl.Username;
        Log.Information("MongoDB configured. Server={Server} User={User} Database={DatabaseName}",
            server, user, databaseName);
    }

    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

    builder.Services.AddDbContext<AquariumDBContext>((serviceProvider, options) =>
    {
        var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
        options.UseMongoDB(mongoClient, databaseName);
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
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString() ?? "<unknown>");
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };
    });

    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aquarium Management API v1");
    });

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Starting AquariumMgmt2026 API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
