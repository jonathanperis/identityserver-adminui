using IdentityServer.Models.Users;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

/// <summary>
/// API controller for managing custom user data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserManagementService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get active users only
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetActiveUsers()
    {
        var users = await _userService.GetActiveUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get inactive users
    /// </summary>
    [HttpGet("inactive")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetInactiveUsers()
    {
        var users = await _userService.GetInactiveUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUser>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    /// <summary>
    /// Get users by company
    /// </summary>
    [HttpGet("company/{company}")]
    public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsersByCompany(string company)
    {
        var users = await _userService.GetUsersByCompanyAsync(company);
        return Ok(users);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationUser>> UpdateUser(string id, ApplicationUser user)
    {
        if (id != user.Id)
        {
            return BadRequest("User ID mismatch");
        }

        try
        {
            var updated = await _userService.UpdateUserAsync(user);
            _logger.LogInformation("Updated user {UserId}", id);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deactivate user
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        try
        {
            await _userService.DeactivateUserAsync(id);
            _logger.LogInformation("Deactivated user {UserId}", id);
            return Ok(new { message = "User deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Activate user
    /// </summary>
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        try
        {
            await _userService.ActivateUserAsync(id);
            _logger.LogInformation("Activated user {UserId}", id);
            return Ok(new { message = "User activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating user {UserId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }
}
