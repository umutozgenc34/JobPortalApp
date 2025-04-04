using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Service.Users.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class UsersController(IUserService userService) : CustomBaseController
{

    [HttpGet("username")]
    public async Task<IActionResult> GetUser()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            return NotFound("Kullanıcı adı bulunamadı.");
        }
        return CreateActionResult(await userService.GetUserByNameAsync(userName));
    }


    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
        => CreateActionResult(await userService.GetAllAsync());


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
        => CreateActionResult(await userService.GetByIdAsync(id));


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id)
        => CreateActionResult(await userService.DeleteAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        => CreateActionResult(await userService.UpdateAsync(request.Id, request));

    [HttpGet("users-with-profiles")]
    public async Task<IActionResult> GetUsersWithProfiles() => CreateActionResult(await userService.GetUsersWithProfilesAsync());

}