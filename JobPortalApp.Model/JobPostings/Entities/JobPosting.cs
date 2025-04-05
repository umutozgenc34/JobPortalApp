using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Shared.Entities;
using System.Text.Json.Serialization;

namespace JobPortalApp.Model.JobPostings.Entities;

public class JobPosting : BaseEntity<Guid>, IAuditEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public string? SalaryRange { get; set; }

    public int CompanyId { get; set; }
    [JsonIgnore]
    public Company Company { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
