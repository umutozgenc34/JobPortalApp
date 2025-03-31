using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Users.Entities;

public class UserRefreshToken : BaseEntity<string>
{
    public string Code { get; set; } = default!;
    public DateTime Expiration { get; set; }
}