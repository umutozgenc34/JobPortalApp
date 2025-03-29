using JobPortalApp.Model.JobPostings.Dtos;

namespace JobPortalApp.Model.Categories.Dtos;

public sealed record CategoryWithJobPostingsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<JobPostingDto> JobPostings { get; set; }
}
