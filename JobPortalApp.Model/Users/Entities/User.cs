using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace JobPortalApp.Model.Users.Entities;

public class User : IdentityUser, IAuditEntity
{
    public DateTime CreatedAt { get ; set ; }
    public DateTime UpdatedAt { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
