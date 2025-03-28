namespace JobPortalApp.Model.JobPostings.Dtos;

public sealed record CreateJobPostingRequest(
    string Title,
    string Description,
    string Location,
    bool IsActive,
    string? SalaryRange,
    int CompanyId,
    int CategoryId
);
