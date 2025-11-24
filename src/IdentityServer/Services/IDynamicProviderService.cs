using IdentityServer.Models;

namespace IdentityServer.Services;

/// <summary>
/// Service for managing dynamic authentication providers
/// </summary>
public interface IDynamicProviderService
{
    // OIDC Provider operations
    Task<IEnumerable<OidcProvider>> GetAllOidcProvidersAsync();
    Task<OidcProvider?> GetOidcProviderAsync(int id);
    Task<OidcProvider?> GetOidcProviderBySchemeAsync(string scheme);
    Task<OidcProvider> CreateOidcProviderAsync(OidcProvider provider);
    Task<OidcProvider> UpdateOidcProviderAsync(OidcProvider provider);
    Task DeleteOidcProviderAsync(int id);

    // SAML Provider operations
    Task<IEnumerable<SamlProvider>> GetAllSamlProvidersAsync();
    Task<SamlProvider?> GetSamlProviderAsync(int id);
    Task<SamlProvider?> GetSamlProviderBySchemeAsync(string scheme);
    Task<SamlProvider> CreateSamlProviderAsync(SamlProvider provider);
    Task<SamlProvider> UpdateSamlProviderAsync(SamlProvider provider);
    Task DeleteSamlProviderAsync(int id);

    // Combined operations
    Task<IEnumerable<DynamicProvider>> GetAllEnabledProvidersAsync();
}
