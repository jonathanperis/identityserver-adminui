# POC Project Documentation

## Overview

This project is a Proof of Concept (POC) using Duende Identity Server with **custom Dynamic Authentication** implementation. The AdminUI dependency has been removed and replaced with a lightweight, custom solution for managing OIDC and SAML authentication providers at runtime.

## Key Features

- **Custom Dynamic Authentication**: Runtime configuration of external authentication providers without code changes
- **OIDC Provider Support**: Full support for OpenID Connect dynamic providers
- **SAML Provider Foundation**: Database schema and models ready for SAML implementation
- **REST API**: Complete API for managing authentication providers
- **Custom Database Tables**: No dependency on commercial IdentityExpress schema
- **Lightweight**: Focused implementation without full AdminUI overhead

## Project Structure

```
IdentityAdminUI.sln
README.md
DYNAMIC_AUTHENTICATION.md (NEW - Detailed documentation)
src/
    IdentityServer/
        Controllers/
            DynamicProvidersController.cs (NEW - API for provider management)
        Configuration/
            DynamicOidcOptionsConfiguration.cs (NEW - OIDC options loader)
        Models/
            DynamicProvider.cs (NEW - Base provider model)
            OidcProvider.cs (NEW - OIDC provider model)
            SamlProvider.cs (NEW - SAML provider model)
        Services/
            IDynamicProviderService.cs (NEW - Service interface)
            DynamicProviderService.cs (NEW - Provider CRUD operations)
            DynamicAuthenticationSchemeService.cs (NEW - Scheme loading)
        Database/
            ApplicationDbContext.cs (UPDATED - Added dynamic provider tables)
        Migrations/
            AddDynamicProviders (NEW - Database migration)
        appsettings.Development.json
        appsettings.json
        Config.cs
        Program.cs (UPDATED - Integrated dynamic authentication)
        Pages/
            Account/Login/ (UPDATED - Shows dynamic providers)
            ExternalLogin/ (UPDATED - Handles dynamic providers)
    WeatherApi/
        WeatherApi.csproj
        Program.cs
        appsettings.Development.json
        appsettings.json
    WeatherClient/
        WeatherClient.csproj
        Program.cs
        appsettings.Development.json
        appsettings.json
        wwwroot/
        Models/
        Services/
        Components/
```

## Prerequisites

- .NET 8 SDK
- Duende Identity Server (included via NuGet)
- PostgreSQL database (or SQLite for development)
- ~~Node.js (for AdminUI)~~ - No longer needed
- ~~AdminUI~~ - Removed in favor of custom implementation

## Quick Start

### 1. Install EF Core Tools

```sh
dotnet tool install --global dotnet-ef --version 8.0.12
```

### 2. Run Database Migrations

Navigate to the `IdentityServer` project directory and run the following commands:

```sh
cd src/IdentityServer

# Apply Identity and IdentityServer migrations
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

This will create:
- ASP.NET Identity tables (Users, Roles, Claims, etc.)
- IdentityServer configuration tables (Clients, Resources, etc.)
- Dynamic authentication provider tables (DynamicOidcProviders, DynamicSamlProviders)

### 3. Running the Projects

You can run the projects individually or use the solution file.

#### IdentityServer

Navigate to the `IdentityServer` project directory and run:

```sh
cd src/IdentityServer
dotnet run
```

Access at: `https://localhost:5443`

### 4. Managing Dynamic Providers

See [DYNAMIC_AUTHENTICATION.md](DYNAMIC_AUTHENTICATION.md) for detailed documentation on:
- Adding OIDC providers via API or database
- Configuring provider settings
- Testing authentication flows
- Troubleshooting common issues

#### Example: Add a Google OIDC Provider

```sql
INSERT INTO "DynamicOidcProviders" (
    "Scheme", "DisplayName", "Enabled", "ProviderType",
    "Authority", "ClientId", "ClientSecret", "Scopes",
    "CallbackPath", "GetClaimsFromUserInfoEndpoint", "SaveTokens",
    "RequireHttpsMetadata", "ResponseType", "Created"
) VALUES (
    'google',
    'Google',
    true,
    'OIDC',
    'https://accounts.google.com',
    'YOUR_GOOGLE_CLIENT_ID',
    'YOUR_GOOGLE_CLIENT_SECRET',
    'openid profile email',
    '/signin-google',
    true,
    true,
    true,
    'code',
    NOW()
);
```

After adding, the Google button will appear on the login page automatically.

### Configuration

#### IdentityServer

Configuration files for IdentityServer are located in the `IdentityServer` directory:

- `appsettings.Development.json`
- `appsettings.json`

Key configuration options:
- `ConnectionStrings:DefaultConnection` - Database connection
- Authentication schemes are configured dynamically from database

### API Endpoints

Dynamic provider management is available at:
- `GET/POST/PUT/DELETE /api/DynamicProviders/oidc` - OIDC providers
- `GET/POST/PUT/DELETE /api/DynamicProviders/saml` - SAML providers
- `GET /api/DynamicProviders/all` - All enabled providers

**Note**: API endpoints require authentication. Use the login flow to obtain access.

### WeatherApi

The `WeatherApi` project provides a simple API to fetch weather data. It includes controllers that handle HTTP requests and return weather information.

#### Running WeatherApi

Navigate to the `WeatherApi` project directory and run:

```sh
dotnet run
```

### WeatherClient

The `WeatherClient` project is a client application that consumes the `WeatherApi` to display weather data. It includes a simple console application that makes HTTP requests to the `WeatherApi` and displays the results.

#### Running WeatherClient

Navigate to the `WeatherClient` project directory and run:

```sh
dotnet run
```

### Documentation

For detailed documentation on the custom Dynamic Authentication implementation, see:

- **[DYNAMIC_AUTHENTICATION.md](DYNAMIC_AUTHENTICATION.md)** - Complete guide to dynamic authentication
  - Architecture overview
  - Database schema details
  - API documentation
  - Setup and configuration
  - Usage examples
  - SAML extension guide
  - Troubleshooting

### Additional Resources

- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7/)
- [Dynamic Providers in Duende IdentityServer](https://docs.duendesoftware.com/identityserver/ui/login/dynamicproviders/)
- [OpenID Connect Core Specification](https://openid.net/specs/openid-connect-core-1_0.html)

## What Changed from Original Implementation

### Removed
- ✗ AdminUI project and all dependencies
- ✗ IdentityExpress packages (`IdentityExpress.Identity`, `Rsk.AdminUI`)
- ✗ Commercial licensing requirements
- ✗ AdminUI web interface and Angular frontend

### Added
- ✓ Custom database tables for dynamic providers
- ✓ REST API for provider management
- ✓ Runtime OIDC scheme configuration
- ✓ Integrated dynamic provider display on login page
- ✓ Comprehensive documentation
- ✓ Foundation for SAML support

### Benefits
1. **No Commercial Dependencies**: Open-source solution
2. **Custom Schema**: Full control over data structure
3. **Lightweight**: Only what you need for dynamic auth
4. **API-Driven**: Easy integration with external systems
5. **Extensible**: Simple to add new provider types

## Testing Dynamic Authentication

### 1. Start IdentityServer
```bash
cd src/IdentityServer
dotnet run
```

### 2. Add a Test OIDC Provider

Use the SQL example above or call the API:

```bash
curl -X POST https://localhost:5443/api/DynamicProviders/oidc \
  -H "Content-Type: application/json" \
  -d '{
    "scheme": "test-oidc",
    "displayName": "Test Provider",
    "enabled": true,
    "authority": "https://demo.duendesoftware.com",
    "clientId": "interactive.public",
    "clientSecret": "",
    "scopes": "openid profile email"
  }'
```

### 3. Navigate to Login Page

Visit `https://localhost:5443/Account/Login` and you should see the "Test Provider" button under "External Account".

## Troubleshooting

### Build Errors
```bash
cd /home/runner/work/identityserver-adminui/identityserver-adminui
dotnet clean
dotnet restore
dotnet build
```

### Database Issues
```bash
cd src/IdentityServer
dotnet ef database drop -c ApplicationDbContext
dotnet ef database update -c ApplicationDbContext
```

### Port Already in Use
Update ports in `Properties/launchSettings.json` for each project.

## Contributing

This is a POC implementation. Contributions and improvements are welcome:
1. SAML provider implementation
2. Admin UI for provider management
3. Provider testing capabilities
4. Enhanced security features
5. Multi-tenancy support