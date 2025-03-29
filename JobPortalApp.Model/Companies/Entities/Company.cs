using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Shared.Entities;
using System.Text.Json.Serialization;

namespace JobPortalApp.Model.Companies.Entities;

public class Company : BaseEntity<int>
{
    public string Name { get; set; }
    public string About { get; set; }
    public string Industry { get; set; }
    public string LogoUrl { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Location { get; set; }

    [JsonIgnore]
    public List<JobPosting> JobPostings { get; set; }
}
