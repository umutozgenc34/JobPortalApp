namespace JobPortalApp.Model.JobPostings.Dtos;

public sealed class JobPostingQuery
{
    public string? Search { get; set; }
    public string? Location { get; set; }
    public int? CategoryId { get; set; }
    public int? CompanyId { get; set; }
    public bool IncludeInactive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
