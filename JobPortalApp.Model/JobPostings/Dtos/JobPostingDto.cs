namespace JobPortalApp.Model.JobPostings.Dtos;

public sealed record JobPostingDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string Location { get; init; }
    public bool IsActive { get; init; }
    public string? SalaryRange { get; init; }

    public int CompanyId { get; init; }
    public string CompanyName { get; init; }

    public int CategoryId { get; init; }
    public string CategoryName { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
