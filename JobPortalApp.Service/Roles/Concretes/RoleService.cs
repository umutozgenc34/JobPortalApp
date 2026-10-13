using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Service.Roles.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Roles.Concretes;

public class RoleService : IRoleService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,ILogger<RoleService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles)
    {
        _logger.LogInformation("CreateUserRolesAsync called for UserName: {UserName} with Roles: {Roles}", userName, string.Join(", ", roles ?? new List<string>()));

        if (roles == null || !roles.Any())
        {
            _logger.LogWarning("CreateUserRolesAsync failed - roles list is null or empty.");
            return ServiceResult.Fail("The roles to be assigned should be specified.");
        }

        var unknownRoles = roles.Where(role => !AppRoles.All.Contains(role)).ToList();
        if (unknownRoles.Count > 0)
        {
            _logger.LogWarning("CreateUserRolesAsync failed - unknown roles: {Roles}", string.Join(", ", unknownRoles));
            return ServiceResult.Fail($"Unknown role(s): {string.Join(", ", unknownRoles)}. Allowed roles: {string.Join(", ", AppRoles.All)}");
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            _logger.LogWarning("CreateUserRolesAsync failed - user not found: {UserName}", userName);
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }

        foreach (var role in roles.Distinct())
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole { Name = role });
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Error occurred while creating the role: {Role}", role);
                    return ServiceResult.Fail($"An error occurred while creating the role: {role}");
                }
                _logger.LogInformation("Role created: {Role}", role);
            }

            if (await _userManager.IsInRoleAsync(user, role))
            {
                continue;
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
            {
                _logger.LogError("Error occurred while assigning role '{Role}' to user '{UserName}'", role, userName);
                return ServiceResult.Fail($"An error occurred while assigning the role to the user.: {role}");
            }

            _logger.LogInformation("Role '{Role}' assigned to user '{UserName}'", role, userName);
        }

        _logger.LogInformation("CreateUserRolesAsync completed successfully for user: {UserName}", userName);
        return ServiceResult.Success("The roles has been successfully added.", HttpStatusCode.Created);
    }

}
