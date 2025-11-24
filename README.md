# IdentityServer Admin UI - POC

## Overview

This is a Proof of Concept (POC) implementation of Duende IdentityServer with **custom Dynamic Authentication** support. The commercial AdminUI dependency has been removed and replaced with a lightweight, custom solution for managing OIDC and SAML authentication providers at runtime.

## Key Features

- **Custom Dynamic Authentication**: Runtime configuration of external authentication providers without code changes
- **OIDC Provider Support**: Full support for OpenID Connect dynamic providers
- **SAML Provider Foundation**: Database schema and models ready for SAML implementation
- **REST API**: Complete API for managing authentication providers
- **Custom Database Tables**: No dependency on commercial IdentityExpress schema
- **Lightweight**: Focused implementation without full AdminUI overhead
- **Sample Applications**: Includes WeatherAPI (protected API) and WeatherClient (Blazor client)

## Project Structure

```
IdentityAdminUI.sln                                    # Solution file
src/
    IdentityServer/                                    # Main Identity Server
        Controllers/
            DynamicProvidersController.cs              # API for provider management
        Configuration/
            DynamicOidcOptionsConfiguration.cs         # OIDC options loader
        Models/
            DynamicProvider.cs                         # Base provider model
            OidcProvider.cs                            # OIDC provider model
            SamlProvider.cs                            # SAML provider model (ready for future use)
        Services/
            IDynamicProviderService.cs                 # Service interface
            DynamicProviderService.cs                  # Provider CRUD operations
            DynamicAuthenticationSchemeService.cs      # Runtime scheme loading
        Database/
            ApplicationDbContext.cs                    # DB context with dynamic provider tables
        Migrations/                                    # EF Core migrations
        Pages/
            Account/Login/                             # Login page with dynamic providers
        Program.cs                                     # Application startup
        Config.cs                                      # IdentityServer configuration
        appsettings.json                              # Configuration (PostgreSQL connection)
    
    WeatherApi/                                        # Sample protected API
        Program.cs                                     # API with JWT authentication
        
    WeatherClient/                                     # Sample Blazor client
        Program.cs                                     # Blazor app with OIDC authentication
        Components/                                    # Blazor components
        Services/                                      # Token service
```

## Prerequisites

- **.NET 8 SDK** or later
- **PostgreSQL database** (or SQLite for quick testing)
- **Docker** (optional, for running PostgreSQL)
- Duende Identity Server (included via NuGet)

## Getting Started

### 1. Setup PostgreSQL Database

The project uses PostgreSQL by default. You can run PostgreSQL using Docker:

```bash
docker run --name postgres-db -e POSTGRES_PASSWORD=admin -p 5432:5432 -d postgres:16
```

This command will:
- Create a container named `postgres-db`
- Set the password to `admin` (for local development only)
- Expose PostgreSQL on port `5432`
- Run the container in detached mode
- Use PostgreSQL version 16 (pinned for consistency)

> **Security Note**: The password `admin` is used for local development only. For production environments, use strong passwords and secure credential management.

**Alternative: Using SQLite for Quick Testing**

If you prefer SQLite, modify the following files:
1. In `src/IdentityServer/appsettings.json`, uncomment the SQLite connection string:
   ```json
   "DefaultConnection": "DataSource=IdentityServer.db;"
   ```
2. In `src/IdentityServer/Program.cs`, uncomment the SQLite options and comment out PostgreSQL options

### 2. Install EF Core Tools

If you haven't already, install the Entity Framework Core tools:

```bash
dotnet tool install --global dotnet-ef --version 8.0.12
```

### 3. Apply Database Migrations

Navigate to the IdentityServer project directory and run the migrations:

```bash
cd src/IdentityServer

# Apply all database migrations
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

This will create:
- **ASP.NET Identity tables**: Users, Roles, Claims, etc.
- **IdentityServer configuration tables**: Clients, Resources, API Scopes, etc.
- **Dynamic authentication provider tables**: DynamicOidcProviders, DynamicSamlProviders
- **Operational tables**: Persisted grants, device codes, etc.

### 4. Run the Projects

You can run the projects individually or together.

#### Running IdentityServer

```bash
cd src/IdentityServer
dotnet run
```

Access at: **https://localhost:5443** (HTTPS) or **http://localhost:5000** (HTTP)

#### Running WeatherAPI (Optional)

The WeatherAPI is a sample protected API that requires authentication from IdentityServer:

```bash
cd src/WeatherApi
dotnet run
```

Default endpoints:
- Swagger UI: Check console output for the URL
- Weather endpoint: `/weatherforecast` (requires authentication)

#### Running WeatherClient (Optional)

The WeatherClient is a Blazor application that uses IdentityServer for authentication and calls the WeatherAPI:

```bash
cd src/WeatherClient
dotnet run
```

Access at the URL shown in the console output.

## Dynamic Authentication

### Overview

This implementation allows you to add, update, and remove OIDC/SAML authentication providers at runtime without restarting the application. Providers are stored in the database and automatically loaded when the application starts.

### Database Schema

#### DynamicOidcProviders Table

Key columns:
- `Scheme`: Unique identifier (e.g., "google", "microsoft")
- `DisplayName`: User-friendly name shown on login page
- `Enabled`: Whether the provider is active
- `Authority`: OIDC authority URL (e.g., "https://accounts.google.com")
- `ClientId`: OAuth client ID
- `ClientSecret`: OAuth client secret
- `Scopes`: Space-separated scopes (e.g., "openid profile email")
- `CallbackPath`: OAuth callback path
- Additional OIDC options: `GetClaimsFromUserInfoEndpoint`, `SaveTokens`, `RequireHttpsMetadata`, `ResponseType`

### Adding Dynamic Providers

#### Method 1: Direct SQL Insert

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

#### Method 2: Using REST API

The application provides REST API endpoints for managing providers (requires authentication):

**Get all OIDC providers:**
```bash
GET /api/DynamicProviders/oidc
```

**Add a new OIDC provider:**
```bash
POST /api/DynamicProviders/oidc
Content-Type: application/json

{
    "scheme": "google",
    "displayName": "Google",
    "enabled": true,
    "authority": "https://accounts.google.com",
    "clientId": "YOUR_CLIENT_ID",
    "clientSecret": "YOUR_CLIENT_SECRET",
    "scopes": "openid profile email"
}
```

**Update a provider:**
```bash
PUT /api/DynamicProviders/oidc/{scheme}
```

**Delete a provider:**
```bash
DELETE /api/DynamicProviders/oidc/{scheme}
```

**Get all enabled providers:**
```bash
GET /api/DynamicProviders/all
```

### Testing Dynamic Authentication

1. **Start IdentityServer:**
   ```bash
   cd src/IdentityServer
   dotnet run
   ```

2. **Add a test provider** (use SQL or API as shown above)

3. **Navigate to the login page:**
   Visit `https://localhost:5443/Account/Login`
   
   The newly added provider button will appear under "External Account" section automatically.

## Configuration

### IdentityServer Configuration

The main configuration is in `src/IdentityServer/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=postgres;"
  }
}
```

> **Note**: The example uses `Password=admin` for local development. In production, use strong passwords and store credentials securely using environment variables or secret management tools.

Key configuration points:
- **ConnectionStrings:DefaultConnection**: Database connection string
- **Authentication schemes**: Configured dynamically from the database
- **IdentityServer options**: Configured in `Program.cs`
- **Clients and Resources**: Configured in `Config.cs`

### Application URLs

Default URLs (configured in `Properties/launchSettings.json`):
- **IdentityServer**: https://localhost:5443 (HTTPS), http://localhost:5000 (HTTP)
- **WeatherAPI**: Check console output after running
- **WeatherClient**: Check console output after running

## Architecture Details

### Dynamic Authentication Components

1. **Models** (`/src/IdentityServer/Models/`)
   - `DynamicProvider.cs`: Base class for all dynamic providers
   - `OidcProvider.cs`: OIDC provider configuration
   - `SamlProvider.cs`: SAML provider configuration (foundation for future implementation)

2. **Services** (`/src/IdentityServer/Services/`)
   - `IDynamicProviderService.cs`: Interface for CRUD operations
   - `DynamicProviderService.cs`: Provider management implementation
   - `DynamicAuthenticationSchemeService.cs`: Runtime scheme loading

3. **Configuration** (`/src/IdentityServer/Configuration/`)
   - `DynamicOidcOptionsConfiguration.cs`: Configures OIDC options from database

4. **API Controller** (`/src/IdentityServer/Controllers/`)
   - `DynamicProvidersController.cs`: REST API for provider management

5. **Database** (`/src/IdentityServer/Database/`)
   - `ApplicationDbContext.cs`: EF Core context with dynamic provider tables

### How It Works

1. **Application Startup**: 
   - `DynamicAuthenticationSchemeService` loads enabled providers from database
   - OIDC authentication schemes are registered dynamically
   - `DynamicOidcOptionsConfiguration` configures each scheme

2. **Login Page**:
   - Queries database for enabled providers
   - Displays provider buttons dynamically

3. **Authentication Flow**:
   - User clicks external provider button
   - OIDC middleware handles the authentication using configuration from database
   - User is redirected back after successful authentication

## What Changed from Original Implementation

### Removed
- ✗ Commercial AdminUI project and all dependencies
- ✗ IdentityExpress packages (`IdentityExpress.Identity`, `Rsk.AdminUI`)
- ✗ Commercial licensing requirements
- ✗ AdminUI web interface and Angular frontend

### Added
- ✓ Custom database tables for dynamic providers (`DynamicOidcProviders`, `DynamicSamlProviders`)
- ✓ REST API for provider management
- ✓ Runtime OIDC scheme configuration
- ✓ Integrated dynamic provider display on login page
- ✓ Foundation for SAML support
- ✓ Sample WeatherAPI and WeatherClient applications

### Benefits
1. **No Commercial Dependencies**: Fully open-source solution
2. **Custom Schema**: Complete control over data structure
3. **Lightweight**: Only essential features for dynamic authentication
4. **API-Driven**: Easy integration with external systems
5. **Extensible**: Simple to add new provider types
6. **Cost-Effective**: No licensing fees

## Troubleshooting

### Database Connection Issues

If you're having trouble connecting to PostgreSQL:

```bash
# Verify PostgreSQL is running
docker ps

# Check logs
docker logs postgres-db

# Restart the container
docker restart postgres-db
```

### Build Errors

```bash
# Clean and rebuild from solution root
dotnet clean
dotnet restore
dotnet build
```

### Database Migration Issues

```bash
cd src/IdentityServer

# Drop and recreate database (WARNING: This will delete all data)
dotnet ef database drop -c ApplicationDbContext
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

### Port Already in Use

Update ports in `src/IdentityServer/Properties/launchSettings.json`:

```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:5443;http://localhost:5000"
    }
  }
}
```

### Dynamic Providers Not Appearing

1. Verify providers are enabled in the database:
   ```sql
   SELECT "Scheme", "DisplayName", "Enabled" FROM "DynamicOidcProviders";
   ```

2. Check IdentityServer logs for errors

3. Restart the IdentityServer application

## Additional Resources

- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7/)
- [Dynamic Providers in Duende IdentityServer](https://docs.duendesoftware.com/identityserver/ui/login/dynamicproviders/)
- [OpenID Connect Core Specification](https://openid.net/specs/openid-connect-core-1_0.html)
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

## Contributing

This is a POC implementation. Contributions and improvements are welcome:

1. **SAML provider implementation**: Complete the SAML provider support
2. **Admin UI**: Build a web interface for provider management
3. **Provider testing**: Add capabilities to test providers before enabling
4. **Enhanced security**: Additional security features and validations
5. **Multi-tenancy**: Support for multiple tenants with isolated providers
6. **Documentation**: Improve and expand documentation
7. **Tests**: Add unit and integration tests

## License

This project is licensed under the terms specified in the LICENSE file.

## Support

For issues, questions, or contributions, please use the GitHub repository's issue tracker.