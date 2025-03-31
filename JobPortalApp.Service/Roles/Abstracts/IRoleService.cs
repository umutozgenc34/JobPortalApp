using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Roles.Abstracts;

public interface IRoleService
{
    Task<ServiceResult> CreateUserRolesAsync(string userName, List<string> roles);
}