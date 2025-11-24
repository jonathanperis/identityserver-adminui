using IdentityServer.Database;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer;

/// <summary>
/// Seeds sample dynamic authentication providers for testing
/// </summary>
public static class DynamicProviderSeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        context.Database.Migrate();

        // Seed OIDC Providers
        SeedOidcProviders(context);
        
        // Seed SAML Providers (examples for future implementation)
        SeedSamlProviders(context);

        context.SaveChanges();
    }

    private static void SeedOidcProviders(ApplicationDbContext context)
    {
        // Google OIDC Provider Example
        if (!context.OidcProviders.Any(p => p.Scheme == "google"))
        {
            var googleProvider = new OidcProvider
            {
                Scheme = "google",
                DisplayName = "Google",
                Enabled = false, // Disabled by default - requires configuration
                Authority = "https://accounts.google.com",
                ClientId = "YOUR_GOOGLE_CLIENT_ID", // Replace with actual client ID
                ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET", // Replace with actual client secret
                Scopes = "openid profile email",
                CallbackPath = "/signin-google",
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                RequireHttpsMetadata = true,
                ResponseType = "code"
            };
            context.OidcProviders.Add(googleProvider);
        }

        // Microsoft OIDC Provider Example
        if (!context.OidcProviders.Any(p => p.Scheme == "microsoft"))
        {
            var microsoftProvider = new OidcProvider
            {
                Scheme = "microsoft",
                DisplayName = "Microsoft",
                Enabled = false, // Disabled by default - requires configuration
                Authority = "https://login.microsoftonline.com/common/v2.0",
                ClientId = "YOUR_MICROSOFT_CLIENT_ID", // Replace with actual client ID
                ClientSecret = "YOUR_MICROSOFT_CLIENT_SECRET", // Replace with actual client secret
                Scopes = "openid profile email",
                CallbackPath = "/signin-microsoft",
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                RequireHttpsMetadata = true,
                ResponseType = "code"
            };
            context.OidcProviders.Add(microsoftProvider);
        }

        // Auth0 OIDC Provider Example
        if (!context.OidcProviders.Any(p => p.Scheme == "auth0"))
        {
            var auth0Provider = new OidcProvider
            {
                Scheme = "auth0",
                DisplayName = "Auth0",
                Enabled = false, // Disabled by default - requires configuration
                Authority = "https://YOUR_DOMAIN.auth0.com", // Replace with your Auth0 domain
                ClientId = "YOUR_AUTH0_CLIENT_ID", // Replace with actual client ID
                ClientSecret = "YOUR_AUTH0_CLIENT_SECRET", // Replace with actual client secret
                Scopes = "openid profile email",
                CallbackPath = "/signin-auth0",
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                RequireHttpsMetadata = true,
                ResponseType = "code"
            };
            context.OidcProviders.Add(auth0Provider);
        }

        // Okta OIDC Provider Example
        if (!context.OidcProviders.Any(p => p.Scheme == "okta"))
        {
            var oktaProvider = new OidcProvider
            {
                Scheme = "okta",
                DisplayName = "Okta",
                Enabled = false, // Disabled by default - requires configuration
                Authority = "https://YOUR_DOMAIN.okta.com/oauth2/default", // Replace with your Okta domain
                ClientId = "YOUR_OKTA_CLIENT_ID", // Replace with actual client ID
                ClientSecret = "YOUR_OKTA_CLIENT_SECRET", // Replace with actual client secret
                Scopes = "openid profile email",
                CallbackPath = "/signin-okta",
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                RequireHttpsMetadata = true,
                ResponseType = "code"
            };
            context.OidcProviders.Add(oktaProvider);
        }

        // Demo Duende IdentityServer (for testing)
        if (!context.OidcProviders.Any(p => p.Scheme == "demo-duende"))
        {
            var demoProvider = new OidcProvider
            {
                Scheme = "demo-duende",
                DisplayName = "Demo IdentityServer",
                Enabled = true, // Enabled for testing
                Authority = "https://demo.duendesoftware.com",
                ClientId = "interactive.public",
                ClientSecret = "", // Public client
                Scopes = "openid profile email api",
                CallbackPath = "/signin-demo",
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                RequireHttpsMetadata = true,
                ResponseType = "code"
            };
            context.OidcProviders.Add(demoProvider);
        }
    }

    private static void SeedSamlProviders(ApplicationDbContext context)
    {
        // SAML Provider Example (for future implementation)
        if (!context.SamlProviders.Any(p => p.Scheme == "saml-example"))
        {
            var samlProvider = new SamlProvider
            {
                Scheme = "saml-example",
                DisplayName = "SAML Provider Example",
                Enabled = false, // Disabled - example only
                SpEntityId = "https://localhost:5443",
                IdpEntityId = "https://idp.example.com",
                IdpSingleSignOnUrl = "https://idp.example.com/sso",
                IdpMetadataUrl = "https://idp.example.com/metadata",
                AcsPath = "/saml/acs",
                SignAuthenticationRequests = false,
                WantAssertionsSigned = true,
                NameIdFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
                BindingType = "POST"
            };
            context.SamlProviders.Add(samlProvider);
        }
    }
}
