# Dynamic Authentication Implementation

This solution implements custom Dynamic Authentication support for IdentityServer without relying on the commercial AdminUI product. It provides runtime configuration of OIDC and SAML authentication providers using custom database tables.

## Overview

The implementation allows you to:
- Add, update, and remove OIDC authentication providers at runtime
- Add, update, and remove SAML authentication providers at runtime (foundation for future implementation)
- Store provider configurations in custom database tables
- Manage providers through a REST API
- Automatically display dynamic providers on the login page

## Architecture

### Components

1. **Models** (`/src/IdentityServer/Models/`)
   - `DynamicProvider.cs` - Base class for all dynamic providers
   - `OidcProvider.cs` - OIDC provider configuration
   - `SamlProvider.cs` - SAML provider configuration (ready for future SAML implementation)

2. **Database** (`/src/IdentityServer/Database/`)
   - `ApplicationDbContext.cs` - Extended with dynamic provider tables
   - Tables: `DynamicOidcProviders`, `DynamicSamlProviders`

3. **Services** (`/src/IdentityServer/Services/`)
   - `IDynamicProviderService.cs` - Interface for CRUD operations
   - `DynamicProviderService.cs` - Implementation of provider management
   - `DynamicAuthenticationSchemeService.cs` - Runtime scheme loading

4. **Configuration** (`/src/IdentityServer/Configuration/`)
   - `DynamicOidcOptionsConfiguration.cs` - Configures OIDC options from database

5. **API** (`/src/IdentityServer/Controllers/`)
   - `DynamicProvidersController.cs` - REST API for provider management

## Database Schema

### DynamicOidcProviders Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Scheme | string(200) | Unique scheme identifier |
| DisplayName | string(200) | Display name for users |
| Enabled | bool | Whether provider is active |
| ProviderType | string(50) | Always "OIDC" |
| Authority | string(500) | OIDC authority URL |
| ClientId | string(200) | OAuth client ID |
| ClientSecret | string(500) | OAuth client secret |
| ResponseType | string(100) | OAuth response type (default: "code") |
| Scopes | string(500) | Requested scopes (space/comma separated) |
| CallbackPath | string(200) | Callback path (default: "/signin-oidc") |
| GetClaimsFromUserInfoEndpoint | bool | Fetch claims from userinfo |
| SaveTokens | bool | Save access/refresh tokens |
| MetadataAddress | string(500) | Custom metadata URL (optional) |
| RequireHttpsMetadata | bool | Require HTTPS for metadata |
| Created | datetime | Creation timestamp |
| Updated | datetime | Last update timestamp |

### DynamicSamlProviders Table

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Scheme | string(200) | Unique scheme identifier |
| DisplayName | string(200) | Display name for users |
| Enabled | bool | Whether provider is active |
| ProviderType | string(50) | Always "SAML" |
| SpEntityId | string(500) | Service Provider entity ID |
| IdpEntityId | string(500) | Identity Provider entity ID |
| IdpSingleSignOnUrl | string(500) | IdP SSO URL |
| IdpMetadataUrl | string(500) | IdP metadata URL (optional) |
| AcsPath | string(200) | Assertion Consumer Service path |
| IdpCertificate | text | IdP certificate (Base64) |
| SpCertificate | text | SP certificate (Base64 PFX) |
| SpCertificatePassword | string(200) | SP certificate password |
| SignAuthenticationRequests | bool | Sign auth requests |
| WantAssertionsSigned | bool | Require signed assertions |
| NameIdFormat | string(200) | SAML NameID format |
| BindingType | string(50) | SAML binding (POST/Redirect) |
| Created | datetime | Creation timestamp |
| Updated | datetime | Last update timestamp |

## API Endpoints

All endpoints require authentication and are under `/api/DynamicProviders`.

### OIDC Providers

- `GET /api/DynamicProviders/oidc` - List all OIDC providers
- `GET /api/DynamicProviders/oidc/{id}` - Get specific OIDC provider
- `POST /api/DynamicProviders/oidc` - Create new OIDC provider
- `PUT /api/DynamicProviders/oidc/{id}` - Update OIDC provider
- `DELETE /api/DynamicProviders/oidc/{id}` - Delete OIDC provider

### SAML Providers

- `GET /api/DynamicProviders/saml` - List all SAML providers
- `GET /api/DynamicProviders/saml/{id}` - Get specific SAML provider
- `POST /api/DynamicProviders/saml` - Create new SAML provider
- `PUT /api/DynamicProviders/saml/{id}` - Update SAML provider
- `DELETE /api/DynamicProviders/saml/{id}` - Delete SAML provider

### Combined

- `GET /api/DynamicProviders/all` - Get all enabled providers

## Setup and Configuration

### 1. Database Migration

Run the migration to create dynamic provider tables:

```bash
cd src/IdentityServer
dotnet ef database update -c ApplicationDbContext
```

### 2. Configure Connection String

Update `appsettings.json` with your database connection:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=postgres;"
  }
}
```

### 3. Run the Application

```bash
cd src/IdentityServer
dotnet run
```

## Usage Examples

### Adding an OIDC Provider via API

```bash
curl -X POST https://localhost:5443/api/DynamicProviders/oidc \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "scheme": "google",
    "displayName": "Google",
    "enabled": true,
    "authority": "https://accounts.google.com",
    "clientId": "YOUR_CLIENT_ID",
    "clientSecret": "YOUR_CLIENT_SECRET",
    "scopes": "openid profile email",
    "callbackPath": "/signin-google"
  }'
```

### Adding an OIDC Provider Directly to Database

```sql
INSERT INTO "DynamicOidcProviders" (
    "Scheme", "DisplayName", "Enabled", "ProviderType",
    "Authority", "ClientId", "ClientSecret", "Scopes",
    "CallbackPath", "GetClaimsFromUserInfoEndpoint", "SaveTokens",
    "RequireHttpsMetadata", "Created"
) VALUES (
    'auth0',
    'Auth0',
    true,
    'OIDC',
    'https://YOUR_DOMAIN.auth0.com',
    'YOUR_CLIENT_ID',
    'YOUR_CLIENT_SECRET',
    'openid profile email',
    '/signin-auth0',
    true,
    true,
    true,
    NOW()
);
```

### Viewing Providers on Login Page

Once configured, dynamic providers automatically appear on the IdentityServer login page under "External Account" section. Users can click on the provider button to authenticate.

## How It Works

### Runtime Configuration

1. **Request Flow**:
   - User clicks on dynamic provider button
   - `Challenge.cshtml.cs` receives the scheme name
   - System checks if scheme exists
   - If not found in registered schemes, checks custom database
   - `DynamicOidcOptionsConfiguration` configures the scheme on-demand
   - Authentication proceeds with configured options

2. **Login Page**:
   - `Index.cshtml.cs` loads all enabled providers from database
   - Combines with standard IdentityServer providers
   - Displays all available authentication options

3. **Caching**:
   - Options are cached per scheme
   - Cache is cleared when provider is updated or deleted
   - Ensures configuration changes take effect immediately

## Extending for SAML

The SAML provider model is included but requires additional implementation:

1. **Add SAML NuGet Package**:
   ```bash
   dotnet add package Rsk.Saml.DuendeIdentityServer
   ```

2. **Configure SAML in Program.cs**:
   ```csharp
   builder.Services.AddSaml2PAuthenticationScheme();
   ```

3. **Create SAML Options Configuration** (similar to `DynamicOidcOptionsConfiguration.cs`):
   ```csharp
   public class DynamicSamlOptionsConfiguration : IConfigureNamedOptions<Saml2pAuthenticationOptions>
   {
       // Configure SAML options from database
   }
   ```

## Benefits Over AdminUI

1. **No Licensing Costs**: No need for commercial AdminUI license
2. **Custom Schema**: Full control over database structure
3. **Lightweight**: Only dynamic authentication, no full admin UI overhead
4. **Customizable**: Easy to extend and modify for specific needs
5. **API-First**: RESTful API for integration with other systems

## Security Considerations

1. **API Authentication**: All management endpoints require authentication
2. **HTTPS**: Always use HTTPS in production
3. **Secret Storage**: Consider encrypting client secrets in database
4. **Input Validation**: API validates all provider configurations
5. **Audit Logging**: Consider adding audit trail for provider changes

## Troubleshooting

### Provider Not Appearing on Login Page

- Check that `Enabled` is set to `true` in database
- Verify `Scheme` is unique
- Check application logs for errors

### Authentication Fails

- Verify `Authority` URL is correct
- Check `ClientId` and `ClientSecret`
- Ensure `CallbackPath` matches provider configuration
- Check firewall/network connectivity to authority

### Database Connection Issues

- Verify connection string in `appsettings.json`
- Ensure database exists and is accessible
- Check that migrations have been applied

## Future Enhancements

1. Complete SAML implementation with Rsk.Saml package
2. Add provider metadata validation
3. Implement provider testing before activation
4. Add UI for provider management (admin panel)
5. Support for additional provider types (WS-Federation, etc.)
6. Multi-tenancy support for providers
7. Provider analytics and usage tracking

## References

- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/)
- [Dynamic Providers in Duende IdentityServer](https://docs.duendesoftware.com/identityserver/ui/login/dynamicproviders/)
- [OpenID Connect Specification](https://openid.net/specs/openid-connect-core-1_0.html)
- [SAML 2.0 Specification](http://docs.oasis-open.org/security/saml/Post2.0/sstc-saml-tech-overview-2.0.html)
