# Testing Guide: Custom Dynamic Authentication

This guide walks you through testing the custom dynamic authentication implementation.

## Prerequisites

Before testing, ensure you have:
- .NET 8 SDK installed
- PostgreSQL or SQLite database available
- Basic understanding of OIDC authentication

## Setup Steps

### 1. Database Setup

#### Option A: PostgreSQL (Recommended for Production)

```bash
# Update connection string in appsettings.json
cd src/IdentityServer
# Edit appsettings.json and set your PostgreSQL connection
```

#### Option B: SQLite (Quick Testing)

```bash
# Uncomment SQLite lines in Program.cs and appsettings.json
# SQLite will create IdentityServer.db in the project folder
```

### 2. Run Migrations

```bash
cd src/IdentityServer

# Apply all migrations
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext
```

This creates:
- Identity tables (Users, Roles, etc.)
- IdentityServer configuration tables
- **Dynamic provider tables**: `DynamicOidcProviders`, `DynamicSamlProviders`

### 3. Seed Test Data

```bash
# Seed initial users and clients
dotnet run --project src/IdentityServer /seed

# Optionally seed sample providers
dotnet run --project src/IdentityServer /seed /seedproviders
```

The seed command creates:
- Test user: `alice` / `alice` (admin role)
- Test user: `bob` / `bob` (user role)
- Sample clients for testing
- Sample OIDC providers (disabled by default)

## Test Scenarios

### Test 1: View Dynamic Providers Admin Page

**Steps:**
1. Start IdentityServer:
   ```bash
   cd src/IdentityServer
   dotnet run
   ```

2. Navigate to: `https://localhost:5443`

3. Login with: `alice` / `alice`

4. Click "Dynamic Providers" in navigation

**Expected Result:**
- See empty OIDC and SAML provider lists
- OR see seeded sample providers (if /seedproviders was used)

### Test 2: Add OIDC Provider via Database

**Steps:**
1. Connect to your database

2. Insert a test OIDC provider:
   ```sql
   INSERT INTO "DynamicOidcProviders" (
       "Scheme", "DisplayName", "Enabled", "ProviderType",
       "Authority", "ClientId", "ClientSecret", "Scopes",
       "CallbackPath", "GetClaimsFromUserInfoEndpoint", "SaveTokens",
       "RequireHttpsMetadata", "ResponseType", "Created"
   ) VALUES (
       'demo-duende',
       'Demo IdentityServer',
       true,
       'OIDC',
       'https://demo.duendesoftware.com',
       'interactive.public',
       '',
       'openid profile email api',
       '/signin-demo',
       true,
       true,
       true,
       'code',
       NOW()
   );
   ```

3. Refresh the admin page

**Expected Result:**
- New provider appears in the OIDC providers list
- Status shows "Enabled"

### Test 3: Login Using Dynamic Provider

**Steps:**
1. Navigate to: `https://localhost:5443/Account/Login`

2. You should see "Demo IdentityServer" button under "External Account"

3. Click "Demo IdentityServer"

4. You'll be redirected to Duende's demo server

5. Login with demo credentials (typically `alice`/`alice` or `bob`/`bob`)

6. Approve consent if requested

7. You'll be redirected back to IdentityServer

**Expected Result:**
- Successful authentication
- Redirected to home page as authenticated user
- User claims populated from Demo IdentityServer

### Test 4: Toggle Provider Status

**Steps:**
1. Go to admin page: `https://localhost:5443/Admin/Providers`

2. Find your test provider

3. Click "Disable" button

4. Refresh the login page

5. Verify the provider button no longer appears

6. Go back to admin and click "Enable"

7. Verify the provider appears again on login page

**Expected Result:**
- Provider visibility changes based on enabled/disabled status
- Changes take effect immediately

### Test 5: API Access

**Test Get All OIDC Providers:**
```bash
curl -X GET https://localhost:5443/api/DynamicProviders/oidc \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -k
```

**Test Create Provider via API:**
```bash
curl -X POST https://localhost:5443/api/DynamicProviders/oidc \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -k \
  -d '{
    "scheme": "google",
    "displayName": "Google",
    "enabled": true,
    "authority": "https://accounts.google.com",
    "clientId": "YOUR_CLIENT_ID",
    "clientSecret": "YOUR_SECRET",
    "scopes": "openid profile email",
    "callbackPath": "/signin-google"
  }'
```

**Note:** API endpoints require authentication. You'll need to:
1. Login to get an access token
2. Use that token in the Authorization header

### Test 6: Real Provider (Google Example)

To test with a real provider like Google:

**Steps:**

1. **Create Google OAuth Credentials:**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select existing
   - Enable Google+ API
   - Go to Credentials → Create Credentials → OAuth 2.0 Client ID
   - Application type: Web application
   - Authorized redirect URIs: `https://localhost:5443/signin-google`
   - Note the Client ID and Client Secret

2. **Add Google Provider to Database:**
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

3. **Test Login:**
   - Go to login page
   - Click "Google" button
   - Login with your Google account
   - Grant permissions
   - You'll be authenticated with Google identity

**Expected Result:**
- Successful login with Google
- User claims populated from Google account

## Test Data Reference

### Default Test Users (from seed)

| Username | Password | Role  |
|----------|----------|-------|
| alice    | alice    | admin |
| bob      | bob      | user  |

### Sample OIDC Provider Configurations

**Demo Duende (Public Client - No Secret Required):**
- Authority: `https://demo.duendesoftware.com`
- Client ID: `interactive.public`
- Client Secret: (empty)
- Scopes: `openid profile email api`

**Auth0 (Template):**
- Authority: `https://YOUR_DOMAIN.auth0.com`
- Client ID: (from Auth0)
- Client Secret: (from Auth0)
- Scopes: `openid profile email`

**Okta (Template):**
- Authority: `https://YOUR_DOMAIN.okta.com/oauth2/default`
- Client ID: (from Okta)
- Client Secret: (from Okta)
- Scopes: `openid profile email`

## Troubleshooting

### Provider Doesn't Appear on Login Page

**Check:**
1. Is `Enabled` set to `true` in database?
2. Is the IdentityServer application running?
3. Check application logs for errors
4. Verify database connection in appsettings.json

**Solution:**
```sql
-- Verify provider status
SELECT "Scheme", "DisplayName", "Enabled" FROM "DynamicOidcProviders";

-- Enable provider
UPDATE "DynamicOidcProviders" SET "Enabled" = true WHERE "Scheme" = 'your-scheme';
```

### Authentication Fails

**Common Issues:**

1. **Invalid Client ID/Secret**
   - Verify credentials in provider configuration
   - Check if credentials are still valid in external provider

2. **Redirect URI Mismatch**
   - Ensure callback path matches: `/signin-{scheme}`
   - Check external provider's allowed redirect URIs

3. **HTTPS Required**
   - Most providers require HTTPS
   - Use `https://localhost:5443` not `http://localhost:5443`

4. **Authority URL Wrong**
   - Verify authority URL is correct
   - Check for trailing slashes

**Debug Steps:**
```bash
# Check application logs
cd src/IdentityServer
dotnet run --urls "https://localhost:5443"

# Enable detailed authentication logging in appsettings.json:
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

### Database Migration Issues

**Reset Database:**
```bash
cd src/IdentityServer

# Drop and recreate
dotnet ef database drop -c ApplicationDbContext -f
dotnet ef database update -c ApplicationDbContext

# Reseed
dotnet run /seed /seedproviders
```

### API Returns 401 Unauthorized

**Issue:** API endpoints require authentication

**Solution:**
1. Login to IdentityServer first
2. For testing, use Postman or similar tool with cookie auth
3. Or implement API token authentication

## Performance Testing

### Load Test Provider Retrieval

```bash
# Install Apache Bench
sudo apt-get install apache2-utils

# Test API endpoint (after authentication)
ab -n 1000 -c 10 https://localhost:5443/api/DynamicProviders/all
```

### Monitor Database Queries

```sql
-- PostgreSQL: Check slow queries
SELECT pid, now() - pg_stat_activity.query_start AS duration, query 
FROM pg_stat_activity 
WHERE state = 'active';

-- Check provider query performance
EXPLAIN ANALYZE 
SELECT * FROM "DynamicOidcProviders" WHERE "Enabled" = true;
```

## Integration Testing

### Test with WeatherClient

The solution includes a WeatherClient that can test the authentication flow:

```bash
# Terminal 1: Start IdentityServer
cd src/IdentityServer
dotnet run

# Terminal 2: Start WeatherApi
cd src/WeatherApi
dotnet run

# Terminal 3: Start WeatherClient
cd src/WeatherClient
dotnet run
```

Navigate to WeatherClient and test login through IdentityServer with your dynamic provider.

## Cleanup

### Remove Test Data

```sql
-- Remove all dynamic providers
DELETE FROM "DynamicOidcProviders";
DELETE FROM "DynamicSamlProviders";

-- Or remove specific provider
DELETE FROM "DynamicOidcProviders" WHERE "Scheme" = 'demo-duende';
```

### Reset to Clean State

```bash
cd src/IdentityServer

# Drop all databases
dotnet ef database drop -c ApplicationDbContext -f
dotnet ef database drop -c ConfigurationDbContext -f
dotnet ef database drop -c PersistedGrantDbContext -f

# Recreate fresh
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c ConfigurationDbContext
dotnet ef database update -c PersistedGrantDbContext

# Seed basics
dotnet run /seed
```

## Next Steps

After successful testing:

1. **Production Setup:**
   - Use strong connection strings
   - Enable HTTPS everywhere
   - Encrypt client secrets in database
   - Set up proper authentication for API

2. **Add More Providers:**
   - Configure real providers (Google, Microsoft, etc.)
   - Test multi-provider scenarios
   - Document provider-specific setup

3. **Extend Functionality:**
   - Implement SAML support
   - Add provider metadata validation
   - Create provider testing tool
   - Add analytics and monitoring

4. **Security Hardening:**
   - Review and audit code
   - Implement rate limiting
   - Add comprehensive logging
   - Set up alerts for failures

## Additional Resources

- [DYNAMIC_AUTHENTICATION.md](DYNAMIC_AUTHENTICATION.md) - Architecture and API docs
- [Duende IdentityServer Docs](https://docs.duendesoftware.com/identityserver/)
- [OIDC Specification](https://openid.net/specs/openid-connect-core-1_0.html)
