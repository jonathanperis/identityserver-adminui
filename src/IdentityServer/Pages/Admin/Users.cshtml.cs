using IdentityServer.Models.Users;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.Pages.Admin;

[Authorize]
public class UsersModel : PageModel
{
    private readonly IUserManagementService _userService;

    public UsersModel(IUserManagementService userService)
    {
        _userService = userService;
    }

    public IEnumerable<ApplicationUser> Users { get; set; } = Enumerable.Empty<ApplicationUser>();

    public async Task OnGetAsync()
    {
        Users = await _userService.GetAllUsersAsync();
    }
}
