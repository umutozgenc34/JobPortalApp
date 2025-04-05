namespace JobPortalApp.Model.JobApplications.Dtos;

public sealed record ApplyToJobRequest(Guid JobPostingId,string? CoverLetter);