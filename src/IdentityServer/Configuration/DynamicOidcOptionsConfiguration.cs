using IdentityServer.Database;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityServer.Configuration;

/// <summary>
/// Configures OpenID Connect options dynamically from database
/// </summary>
public class DynamicOidcOptionsConfiguration : IConfigureNamedOptions<OpenIdConnectOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DynamicOidcOptionsConfiguration> _logger;

    public DynamicOidcOptionsConfiguration(
        IServiceProvider serviceProvider,
        ILogger<DynamicOidcOptionsConfiguration> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Configure(string? name, OpenIdConnectOptions options)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Use synchronous query to avoid blocking async code
        // This is acceptable here because IConfigureNamedOptions.Configure is synchronous by design
        var provider = context.OidcProviders
            .AsNoTracking()
            .FirstOrDefault(p => p.Scheme == name && p.Enabled);

        if (provider == null)
        {
            _logger.LogWarning("OIDC provider {Scheme} not found in database", name);
            return;
        }

        _logger.LogInformation("Configuring OIDC scheme {Scheme} with authority {Authority}", name, provider.Authority);

        options.Authority = provider.Authority;
        options.ClientId = provider.ClientId;
        options.ClientSecret = provider.ClientSecret;
        options.ResponseType = provider.ResponseType;
        options.CallbackPath = provider.CallbackPath;
        options.GetClaimsFromUserInfoEndpoint = provider.GetClaimsFromUserInfoEndpoint;
        options.SaveTokens = provider.SaveTokens;
        options.RequireHttpsMetadata = provider.RequireHttpsMetadata;

        if (!string.IsNullOrEmpty(provider.MetadataAddress))
        {
            options.MetadataAddress = provider.MetadataAddress;
        }

        // Parse and add scopes
        if (!string.IsNullOrEmpty(provider.Scopes))
        {
            var scopesList = provider.Scopes.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            options.Scope.Clear();
            foreach (var scopeItem in scopesList)
            {
                options.Scope.Add(scopeItem);
            }
        }

        // Set sign-in scheme to IdentityServer's cookie
        options.SignInScheme = "Identity.Application";
    }

    public void Configure(OpenIdConnectOptions options)
    {
        // Default configuration if needed
    }
}
