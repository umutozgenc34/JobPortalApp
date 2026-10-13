namespace JobPortalApp.Shared.Security;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string SuperAdmin = "SuperAdmin";
    public const string User = "User";
    public const string Administrators = Admin + "," + SuperAdmin;

    public static readonly IReadOnlyList<string> All = [Admin, SuperAdmin, User];
}
