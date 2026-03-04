using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AquariumMgmt2026.Security;

public class ServiceKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ServiceKey";
    public const string DefaultHeaderName = "X-Service-Key";

    private readonly IConfiguration _configuration;

    public ServiceKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headerName = _configuration["ServiceAuth:HeaderName"];
        if (string.IsNullOrWhiteSpace(headerName))
        {
            headerName = DefaultHeaderName;
        }

        if (!Request.Headers.TryGetValue(headerName, out var providedKeyValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var expectedKey = _configuration["ServiceAuth:ApiKey"];
        var providedKey = providedKeyValues.ToString();

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Service API key is not configured."));
        }

        if (!string.Equals(providedKey, expectedKey, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid service API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "service-client"),
            new Claim(ClaimTypes.Name, "ServiceClient"),
            new Claim("auth_type", "service_key")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
