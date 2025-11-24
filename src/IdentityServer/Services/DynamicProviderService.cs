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
        return await _context.OidcProviders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<OidcProvider?> GetOidcProviderAsync(int id)
    {
        return await _context.OidcProviders.FindAsync(id);
    }

    public async Task<OidcProvider?> GetOidcProviderBySchemeAsync(string scheme)
    {
        return await _context.OidcProviders
            .AsNoTracking()
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
        // Use ExecuteDelete for better performance (EF Core 7.0+)
        var rowsAffected = await _context.OidcProviders
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        // Note: In production, consider checking if provider is actively used
        // before deletion to prevent breaking active authentication flows
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"OIDC provider with ID {id} not found");
        }
    }

    #endregion

    #region SAML Provider Operations

    public async Task<IEnumerable<SamlProvider>> GetAllSamlProvidersAsync()
    {
        return await _context.SamlProviders
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<SamlProvider?> GetSamlProviderAsync(int id)
    {
        return await _context.SamlProviders.FindAsync(id);
    }

    public async Task<SamlProvider?> GetSamlProviderBySchemeAsync(string scheme)
    {
        return await _context.SamlProviders
            .AsNoTracking()
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
        // Use ExecuteDelete for better performance (EF Core 7.0+)
        var rowsAffected = await _context.SamlProviders
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        // Note: In production, consider checking if provider is actively used
        // before deletion to prevent breaking active authentication flows
        if (rowsAffected == 0)
        {
            throw new InvalidOperationException($"SAML provider with ID {id} not found");
        }
    }

    #endregion

    #region Combined Operations

    public async Task<IEnumerable<DynamicProvider>> GetAllEnabledProvidersAsync()
    {
        // Use parallel queries for better performance
        var oidcTask = _context.OidcProviders
            .AsNoTracking()
            .Where(p => p.Enabled)
            .ToListAsync();

        var samlTask = _context.SamlProviders
            .AsNoTracking()
            .Where(p => p.Enabled)
            .ToListAsync();

        await Task.WhenAll(oidcTask, samlTask);

        return oidcTask.Result.Cast<DynamicProvider>()
            .Concat(samlTask.Result.Cast<DynamicProvider>())
            .OrderBy(p => p.DisplayName);
    }

    #endregion
}
