using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.JobApplications.Abstracts;

public interface IJobApplicationService
{
    Task<ServiceResult<JobApplicationDto>> ApplyAsync(ApplyToJobRequest request, string userId);
    Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByUserIdAsync(string userId);
    Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByJobPostingIdAsync(Guid jobPostingId);
}
