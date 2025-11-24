using IdentityServer.Database;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services;

/// <summary>
/// Implementation of dynamic authentication provider management service
/// </summary>
public class DynamicProviderService : IDynamicProviderService
{
    private readonly ApplicationDbContext _context;

    public DynamicProviderService(ApplicationDbContext context)
    {
        _context = context;
    }

    #region OIDC Provider Operations

    public async Task<IEnumerable<OidcProvider>> GetAllOidcProvidersAsync()
    {
        return await _context.OidcProviders.ToListAsync();
    }

    public async Task<OidcProvider?> GetOidcProviderAsync(int id)
    {
        return await _context.OidcProviders.FindAsync(id);
    }

    public async Task<OidcProvider?> GetOidcProviderBySchemeAsync(string scheme)
    {
        return await _context.OidcProviders
            .FirstOrDefaultAsync(p => p.Scheme == scheme);
    }

    public async Task<OidcProvider> CreateOidcProviderAsync(OidcProvider provider)
    {
        provider.Created = DateTime.UtcNow;
        _context.OidcProviders.Add(provider);
        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task<OidcProvider> UpdateOidcProviderAsync(OidcProvider provider)
    {
        provider.Updated = DateTime.UtcNow;
        _context.OidcProviders.Update(provider);
        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task DeleteOidcProviderAsync(int id)
    {
        var provider = await _context.OidcProviders.FindAsync(id);
        if (provider != null)
        {
            _context.OidcProviders.Remove(provider);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region SAML Provider Operations

    public async Task<IEnumerable<SamlProvider>> GetAllSamlProvidersAsync()
    {
        return await _context.SamlProviders.ToListAsync();
    }

    public async Task<SamlProvider?> GetSamlProviderAsync(int id)
    {
        return await _context.SamlProviders.FindAsync(id);
    }

    public async Task<SamlProvider?> GetSamlProviderBySchemeAsync(string scheme)
    {
        return await _context.SamlProviders
            .FirstOrDefaultAsync(p => p.Scheme == scheme);
    }

    public async Task<SamlProvider> CreateSamlProviderAsync(SamlProvider provider)
    {
        provider.Created = DateTime.UtcNow;
        _context.SamlProviders.Add(provider);
        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task<SamlProvider> UpdateSamlProviderAsync(SamlProvider provider)
    {
        provider.Updated = DateTime.UtcNow;
        _context.SamlProviders.Update(provider);
        await _context.SaveChangesAsync();
        return provider;
    }

    public async Task DeleteSamlProviderAsync(int id)
    {
        var provider = await _context.SamlProviders.FindAsync(id);
        if (provider != null)
        {
            _context.SamlProviders.Remove(provider);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Combined Operations

    public async Task<IEnumerable<DynamicProvider>> GetAllEnabledProvidersAsync()
    {
        var oidcProviders = await _context.OidcProviders
            .Where(p => p.Enabled)
            .ToListAsync();

        var samlProviders = await _context.SamlProviders
            .Where(p => p.Enabled)
            .ToListAsync();

        return oidcProviders.Cast<DynamicProvider>()
            .Concat(samlProviders.Cast<DynamicProvider>())
            .OrderBy(p => p.DisplayName);
    }

    #endregion
}
