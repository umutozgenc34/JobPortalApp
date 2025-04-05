using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.JobApplications.Entities;

public class JobApplication : BaseEntity<Guid>, IAuditEntity
{
    public string? CoverLetter { get; set; }

    public string UserId { get; set; } = null!;
    public virtual User User { get; set; }
    public Guid JobPostingId { get; set; }
    public virtual JobPosting JobPosting { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
