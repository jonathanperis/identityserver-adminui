using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Admin;

[Authorize]
public class ProvidersModel : PageModel
{
    private readonly IDynamicProviderService _providerService;
    private readonly DynamicAuthenticationSchemeService _schemeService;

    public ProvidersModel(
        IDynamicProviderService providerService,
        DynamicAuthenticationSchemeService schemeService)
    {
        _providerService = providerService;
        _schemeService = schemeService;
    }

    public IEnumerable<OidcProvider> OidcProviders { get; set; } = Enumerable.Empty<OidcProvider>();
    public IEnumerable<SamlProvider> SamlProviders { get; set; } = Enumerable.Empty<SamlProvider>();

    public async Task OnGetAsync()
    {
        OidcProviders = await _providerService.GetAllOidcProvidersAsync();
        SamlProviders = await _providerService.GetAllSamlProvidersAsync();
    }

    public async Task<IActionResult> OnPostToggleOidcAsync(int id)
    {
        var provider = await _providerService.GetOidcProviderAsync(id);
        if (provider != null)
        {
            provider.Enabled = !provider.Enabled;
            await _providerService.UpdateOidcProviderAsync(provider);
            _schemeService.ClearOptionsCache(provider.Scheme);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleSamlAsync(int id)
    {
        var provider = await _providerService.GetSamlProviderAsync(id);
        if (provider != null)
        {
            provider.Enabled = !provider.Enabled;
            await _providerService.UpdateSamlProviderAsync(provider);
        }

        return RedirectToPage();
    }
}
