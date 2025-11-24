using IdentityServer.Database;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services;

/// <summary>
/// Service for configuring dynamic authentication schemes at runtime
/// </summary>
public class DynamicAuthenticationSchemeService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IOptionsMonitorCache<OpenIdConnectOptions> _optionsCache;
    private readonly ILogger<DynamicAuthenticationSchemeService> _logger;

    public DynamicAuthenticationSchemeService(
        ApplicationDbContext context,
        IAuthenticationSchemeProvider schemeProvider,
        IOptionsMonitorCache<OpenIdConnectOptions> optionsCache,
        ILogger<DynamicAuthenticationSchemeService> logger)
    {
        _context = context;
        _schemeProvider = schemeProvider;
        _optionsCache = optionsCache;
        _logger = logger;
    }

    /// <summary>
    /// Load all enabled OIDC providers and register their schemes
    /// </summary>
    public async Task<IEnumerable<AuthenticationScheme>> LoadOidcSchemesAsync()
    {
        var providers = await _context.OidcProviders
            .Where(p => p.Enabled)
            .ToListAsync();

        var schemes = new List<AuthenticationScheme>();

        foreach (var provider in providers)
        {
            try
            {
                var scheme = await _schemeProvider.GetSchemeAsync(provider.Scheme);
                if (scheme == null)
                {
                    _logger.LogInformation("OIDC scheme {Scheme} not found, will be registered on first use", provider.Scheme);
                }
                else
                {
                    schemes.Add(scheme);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading OIDC scheme {Scheme}", provider.Scheme);
            }
        }

        return schemes;
    }

    /// <summary>
    /// Get configuration for an OIDC provider by scheme
    /// </summary>
    public async Task<OidcProvider?> GetOidcProviderConfigurationAsync(string scheme)
    {
        return await _context.OidcProviders
            .FirstOrDefaultAsync(p => p.Scheme == scheme && p.Enabled);
    }

    /// <summary>
    /// Get all enabled OIDC providers
    /// </summary>
    public async Task<IEnumerable<OidcProvider>> GetEnabledOidcProvidersAsync()
    {
        return await _context.OidcProviders
            .Where(p => p.Enabled)
            .ToListAsync();
    }

    /// <summary>
    /// Get all enabled SAML providers
    /// </summary>
    public async Task<IEnumerable<SamlProvider>> GetEnabledSamlProvidersAsync()
    {
        return await _context.SamlProviders
            .Where(p => p.Enabled)
            .ToListAsync();
    }

    /// <summary>
    /// Refresh authentication schemes cache
    /// </summary>
    public void ClearOptionsCache(string scheme)
    {
        _optionsCache.TryRemove(scheme);
    }
}
