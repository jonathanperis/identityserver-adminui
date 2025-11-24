# Custom User Schema Implementation

This document describes the custom user schema implementation that extends ASP.NET Identity with additional user properties beyond the standard schema.

## Overview

The default `IdentityUser` class has been replaced with a custom `ApplicationUser` class that includes extended profile information, address fields, work details, preferences, and tracking fields.

## Custom User Model

### ApplicationUser Class

Location: `/src/IdentityServer/Models/Users/ApplicationUser.cs`

The `ApplicationUser` class extends `IdentityUser` with the following additional properties:

#### Personal Information
- `FirstName` - User's first name
- `LastName` - User's last name
- `FullName` - Computed property combining first and last name
- `DateOfBirth` - User's date of birth
- `ProfilePictureUrl` - URL or path to profile picture

#### Address Information
- `AddressLine1` - Primary address
- `AddressLine2` - Secondary address (apartment, suite, etc.)
- `City` - City
- `State` - State or province
- `PostalCode` - Postal/ZIP code
- `Country` - Country

#### Work Information
- `Company` - Company or organization name
- `JobTitle` - Job title or position
- `Department` - Department

#### Preferences
- `PreferredLanguage` - User's preferred language/locale (e.g., "en", "es")
- `TimeZone` - User's timezone (e.g., "America/New_York")

#### Account Management
- `IsActive` - Whether the account is active (default: true)
- `CreatedAt` - Account creation timestamp
- `UpdatedAt` - Last profile update timestamp
- `LastLoginAt` - Last successful login timestamp
- `CustomMetadata` - JSON field for additional custom data

## Database Schema

### Table Structure

The custom fields are added to the `AspNetUsers` table with the following schema:

```sql
-- Personal Information
FirstName VARCHAR(100)
LastName VARCHAR(100)
DateOfBirth DATETIME
ProfilePictureUrl VARCHAR(500)

-- Address
AddressLine1 VARCHAR(200)
AddressLine2 VARCHAR(200)
City VARCHAR(100)
State VARCHAR(100)
PostalCode VARCHAR(20)
Country VARCHAR(100)

-- Work
Company VARCHAR(200)
JobTitle VARCHAR(200)
Department VARCHAR(200)

-- Preferences
PreferredLanguage VARCHAR(10)
TimeZone VARCHAR(100)

-- Account Management
IsActive BIT NOT NULL DEFAULT 1
CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
UpdatedAt DATETIME NULL
LastLoginAt DATETIME NULL
CustomMetadata NVARCHAR(MAX) NULL
```

### Indexes

The following indexes are created for performance:
- `IX_AspNetUsers_IsActive` - For filtering active/inactive users
- `IX_AspNetUsers_Company` - For company-based queries
- `IX_AspNetUsers_LastLoginAt` - For login tracking queries

## Migration

### Creating the Migration

The custom user fields migration was created with:

```bash
cd src/IdentityServer
dotnet ef migrations add AddCustomUserFields -c ApplicationDbContext
```

### Applying the Migration

```bash
cd src/IdentityServer
dotnet ef database update -c ApplicationDbContext
```

## User Management Service

### IUserManagementService Interface

Location: `/src/IdentityServer/Services/IUserManagementService.cs`

Provides operations for:
- Getting all users
- Getting users by ID, email, or username
- Filtering active/inactive users
- Getting users by company
- Updating user profiles
- Activating/deactivating users

### UserManagementService Implementation

Location: `/src/IdentityServer/Services/UserManagementService.cs`

Key features:
- Automatic `UpdatedAt` timestamp on updates
- Active/inactive user filtering
- Company-based user queries
- Error handling with descriptive messages

## API Endpoints

### Users API Controller

Location: `/src/IdentityServer/Controllers/UsersController.cs`

Available endpoints:

```
GET    /api/Users                    - Get all users
GET    /api/Users/active             - Get active users only
GET    /api/Users/inactive           - Get inactive users
GET    /api/Users/{id}               - Get specific user by ID
GET    /api/Users/company/{company}  - Get users by company
PUT    /api/Users/{id}               - Update user profile
POST   /api/Users/{id}/activate      - Activate a user
POST   /api/Users/{id}/deactivate    - Deactivate a user
```

All endpoints require authentication.

### Example API Usage

**Get all active users:**
```bash
curl -X GET https://localhost:5443/api/Users/active \
  -H "Authorization: Bearer <token>" \
  -k
```

**Update user profile:**
```bash
curl -X PUT https://localhost:5443/api/Users/{id} \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "id": "user-id",
    "firstName": "John",
    "lastName": "Doe",
    "company": "Acme Corp",
    "jobTitle": "Senior Developer",
    "isActive": true
  }' \
  -k
```

## Admin UI

### Users Management Page

Location: `/src/IdentityServer/Pages/Admin/Users.cshtml`

Features:
- View all registered users in a table
- See key information: username, full name, email, company, job title
- Filter by status (active/inactive)
- View detailed user information in a modal
- Quick access to API endpoints

**Access:** Navigate to `https://localhost:5443/Admin/Users` after logging in.

### User Details Modal

The details modal shows:
- **Personal Information:** Name, email, phone, date of birth
- **Address:** Complete address information
- **Work Information:** Company, job title, department
- **Preferences:** Language, timezone
- **Account Information:** User ID, status, timestamps

## Seeding Sample Data

### Updated Seed Users

The `SeedData.cs` file has been updated to seed users with custom fields:

**Alice (Admin User):**
```csharp
FirstName = "Alice"
LastName = "Smith"
DateOfBirth = 1985-03-15
City = "Wonderland"
Company = "Adventure Corp"
JobTitle = "Explorer"
Department = "Discovery"
```

**Bob (Standard User):**
```csharp
FirstName = "Bob"
LastName = "Smith"
DateOfBirth = 1990-07-22
City = "Springfield"
State = "IL"
Company = "Tech Solutions Inc"
JobTitle = "Developer"
Department = "Engineering"
```

## Integration with Authentication

### Login Tracking

The login process automatically updates the `LastLoginAt` field:

**Local Login:** `Pages/Account/Login/Index.cshtml.cs`
```csharp
user.LastLoginAt = DateTime.UtcNow;
await _userManager.UpdateAsync(user);
```

**External Login:** `Pages/ExternalLogin/Callback.cshtml.cs`
- New users are created with full profile data from external claims
- Existing users have `LastLoginAt` updated

### Display Name

The system now uses the full name instead of username:
```csharp
DisplayName = user.FullName ?? user.UserName
```

## Configuration Changes

### Program.cs

Updated to use `ApplicationUser`:
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
  .AddAspNetIdentity<ApplicationUser>();

// Register user management service
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
```

### ApplicationDbContext

Updated to use `ApplicationUser` and configure custom properties:
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // DbSets and configuration
}
```

## Best Practices

### Updating User Profiles

Always use the `UserManagementService` to update user profiles:
```csharp
var user = await _userService.GetUserByIdAsync(userId);
user.Company = "New Company";
user.JobTitle = "New Title";
await _userService.UpdateUserAsync(user); // Automatically sets UpdatedAt
```

### Active/Inactive Users

Use the `IsActive` flag instead of deleting users:
```csharp
await _userService.DeactivateUserAsync(userId);
// User is now inactive but data is preserved
```

### Custom Metadata

Use the `CustomMetadata` field for additional JSON data:
```csharp
user.CustomMetadata = JsonSerializer.Serialize(new {
    Theme = "dark",
    NotificationPreferences = new { Email = true, SMS = false }
});
```

## Querying Users

### By Company
```csharp
var users = await _userService.GetUsersByCompanyAsync("Acme Corp");
```

### Active Users Only
```csharp
var activeUsers = await _userService.GetActiveUsersAsync();
```

### With EF Core Directly
```csharp
var users = await _context.Users
    .Where(u => u.IsActive && u.Company == "Acme Corp")
    .OrderBy(u => u.LastName)
    .ToListAsync();
```

## Extending Further

### Adding New Fields

1. Add property to `ApplicationUser` class
2. Create a new migration:
   ```bash
   dotnet ef migrations add AddNewUserFields -c ApplicationDbContext
   ```
3. Apply the migration:
   ```bash
   dotnet ef database update -c ApplicationDbContext
   ```
4. Update UI and API as needed

### Adding Relationships

Example: Add user preferences as a separate table:
```csharp
public class UserPreferences
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    // Additional preference fields
}
```

## Security Considerations

1. **Sensitive Data:** Consider encrypting sensitive fields like phone numbers
2. **PII Compliance:** Implement data retention policies for personal information
3. **API Access:** All user management endpoints require authentication
4. **Soft Deletes:** Use `IsActive` flag instead of hard deletes for audit trails

## Troubleshooting

### Migration Issues

If migration fails, check:
1. Database connection in `appsettings.json`
2. Existing data compatibility
3. Run migrations in correct order

### Seed Data

To reseed with new user data:
```bash
dotnet ef database drop -c ApplicationDbContext -f
dotnet ef database update -c ApplicationDbContext
dotnet run /seed
```

### Display Issues

If user information doesn't appear:
1. Verify migration was applied
2. Check `IsActive` flag
3. Ensure user management service is registered in `Program.cs`

## Summary

The custom user schema implementation provides:
- ✅ Extended user profiles with 20+ additional fields
- ✅ Complete CRUD API for user management
- ✅ Admin UI for viewing and managing users
- ✅ Login tracking and activity monitoring
- ✅ Performance-optimized with proper indexes
- ✅ Integration with external authentication providers
- ✅ Flexible metadata storage for custom data

All custom user data is stored in the standard ASP.NET Identity tables with additional columns, maintaining compatibility with existing Identity features while providing enhanced functionality.
