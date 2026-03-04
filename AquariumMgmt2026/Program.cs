using DAL;
using DAL.Repository;
using DAL.UnitOfWork;
using AquariumMgmt2026.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Services.Interfaces;
using Services.Services;
using System.Text;

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
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontendDev", policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });
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

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Paste only the JWT token (without 'Bearer '). Swagger adds the Bearer prefix automatically.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
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
    builder.Services.AddScoped<IAquariumImageService, AquariumImageService>();
    builder.Services.AddScoped<IUserService, UserService>();

    var jwtSection = builder.Configuration.GetSection("Jwt");
    var jwtIssuer = jwtSection["Issuer"]?.Trim();
    var jwtAudience = jwtSection["Audience"]?.Trim();
    var jwtKey = jwtSection["Key"]?.Trim();

    if (string.IsNullOrWhiteSpace(jwtIssuer) ||
        string.IsNullOrWhiteSpace(jwtAudience) ||
        string.IsNullOrWhiteSpace(jwtKey))
    {
        throw new InvalidOperationException(
            "JWT configuration is missing. Please set Jwt:Issuer, Jwt:Audience and Jwt:Key.");
    }

    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddScheme<AuthenticationSchemeOptions, ServiceKeyAuthenticationHandler>(
            ServiceKeyAuthenticationHandler.SchemeName,
            _ => { });

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(
                JwtBearerDefaults.AuthenticationScheme,
                ServiceKeyAuthenticationHandler.SchemeName)
            .RequireAuthenticatedUser()
            .Build();

        options.AddPolicy("UserOrService", policy =>
        {
            policy.AddAuthenticationSchemes(
                JwtBearerDefaults.AuthenticationScheme,
                ServiceKeyAuthenticationHandler.SchemeName);
            policy.RequireAuthenticatedUser();
        });
    });

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

    app.UseCors("FrontendDev");

    app.UseAuthentication();
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
