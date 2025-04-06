using JobPortalApp.Service.Roles.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class RolesController(IRoleService roleService) : CustomBaseController
{
    [Authorize(Roles ="Admin,SuperAdmin")]
    [HttpPost("CreateUserRoles/{userName}")]
    public async Task<IActionResult> CreateUserRoles(string userName, [FromBody] List<string> roles) => CreateActionResult(await roleService
       .CreateUserRolesAsync(userName, roles));
}