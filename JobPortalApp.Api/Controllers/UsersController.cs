using JobPortalApp.Api.Extensions;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

[Authorize]
public class UsersController(IUserService userService) : CustomBaseController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
        => CreateActionResult(await userService.GetCurrentUserAsync(User.GetUserId()));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserRequest request)
        => CreateActionResult(await userService.UpdateAsync(User.GetUserId(), request));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
        => CreateActionResult(await userService.GetAllAsync());

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
        => CreateActionResult(await userService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
        => CreateActionResult(await userService.DeleteAsync(id));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet("users-with-profiles")]
    public async Task<IActionResult> GetUsersWithProfiles() => CreateActionResult(await userService.GetUsersWithProfilesAsync());
}
