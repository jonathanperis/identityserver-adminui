using IdentityServer.Models.Users;

namespace IdentityServer.Services;

/// <summary>
/// Service interface for managing custom user operations
/// </summary>
public interface IUserManagementService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
    Task<IEnumerable<ApplicationUser>> GetInactiveUsersAsync();
    Task<IEnumerable<ApplicationUser>> GetUsersByCompanyAsync(string company);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task DeactivateUserAsync(string userId);
    Task ActivateUserAsync(string userId);
}
