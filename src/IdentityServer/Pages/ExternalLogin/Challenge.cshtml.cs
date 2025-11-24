// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using Duende.IdentityServer.Services;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerHost.Pages.ExternalLogin;

[AllowAnonymous]
[SecurityHeaders]
public class Challenge : PageModel
{
    private readonly IIdentityServerInteractionService _interactionService;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly DynamicAuthenticationSchemeService _dynamicSchemeService;

    public Challenge(
        IIdentityServerInteractionService interactionService,
        IAuthenticationSchemeProvider schemeProvider,
        DynamicAuthenticationSchemeService dynamicSchemeService)
    {
        _interactionService = interactionService;
        _schemeProvider = schemeProvider;
        _dynamicSchemeService = dynamicSchemeService;
    }
        
    public async Task<IActionResult> OnGet(string scheme, string? returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (Url.IsLocalUrl(returnUrl) == false && _interactionService.IsValidReturnUrl(returnUrl) == false)
        {
            // user might have clicked on a malicious link - should be logged
            throw new ArgumentException("invalid return URL");
        }

        // Check if the scheme exists, if not try to register it as a dynamic OIDC provider
        var existingScheme = await _schemeProvider.GetSchemeAsync(scheme);
        if (existingScheme == null)
        {
            // Check if this is a custom dynamic OIDC provider
            var oidcProvider = await _dynamicSchemeService.GetOidcProviderConfigurationAsync(scheme);
            if (oidcProvider == null)
            {
                // Provider not found
                throw new ArgumentException($"Authentication scheme '{scheme}' not found");
            }
            // The scheme will be configured dynamically via DynamicOidcOptionsConfiguration
            // when the authentication middleware tries to use it
        }
            
        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Page("/externallogin/callback"),
                
            Items =
            {
                { "returnUrl", returnUrl }, 
                { "scheme", scheme },
            }
        };

        return Challenge(props, scheme);
    }
}
