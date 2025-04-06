using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Service.Users.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;
[Authorize]
public class UserProfilesController(IUserProfileService userProfileService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllUserProfiles() =>
        CreateActionResult(await userProfileService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserProfileById([FromRoute] int id) =>
        CreateActionResult(await userProfileService.GetByIdAsync(id));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateUserProfile([FromForm] CreateUserProfileRequest request) =>
        CreateActionResult(await userProfileService.CreateAsync(request));

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUserProfile([FromRoute] int id, [FromForm] UpdateUserProfileRequest request) =>
        CreateActionResult(await userProfileService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUserProfile([FromRoute] int id) =>
        CreateActionResult(await userProfileService.DeleteAsync(id));
}
