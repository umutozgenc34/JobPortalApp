using JobPortalApp.Shared.Security;
using System.Security.Claims;

namespace JobPortalApp.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User id claim is missing.");

    public static bool IsAdministrator(this ClaimsPrincipal user) =>
        user.IsInRole(AppRoles.Admin) || user.IsInRole(AppRoles.SuperAdmin);
}
