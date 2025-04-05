namespace JobPortalApp.Model.JobApplications.Dtos;

public sealed record JobApplicationDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } 
    public string UserEmail { get; init; } 

    public Guid JobPostingId { get; init; }
    public string JobPostingTitle { get; init; } 

    public string? CoverLetter { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}