using JobPortalApp.Api.Extensions;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

[Authorize]
public class UserProfilesController(IUserProfileService userProfileService) : CustomBaseController
{
    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet]
    public async Task<IActionResult> GetAllUserProfiles() =>
        CreateActionResult(await userProfileService.GetAllAsync());

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserProfileById([FromRoute] int id) =>
        CreateActionResult(await userProfileService.GetByIdAsync(id));

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile() =>
        CreateActionResult(await userProfileService.GetByUserIdAsync(User.GetUserId()));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateUserProfile([FromForm] CreateUserProfileRequest request) =>
        CreateActionResult(await userProfileService.CreateAsync(request, User.GetUserId()));

    [HttpPut("me")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateUserProfileRequest request) =>
        CreateActionResult(await userProfileService.UpdateAsync(User.GetUserId(), request));

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyProfile() =>
        CreateActionResult(await userProfileService.DeleteAsync(User.GetUserId()));
}
