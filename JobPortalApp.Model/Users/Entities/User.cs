using JobPortalApp.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace JobPortalApp.Model.Users.Entities;

public class User : IdentityUser, IAuditEntity
{
    public DateTime CreatedAt { get ; set ; }
    public DateTime UpdatedAt { get; set; }
}
