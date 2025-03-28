using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Shared.Entities;

namespace JobPortalApp.Model.Categories.Entities;

public class Category : BaseEntity<int>
{
    public string Name { get; set; } = default!;
    public List<JobPosting> JobPostings { get; set; }
}
