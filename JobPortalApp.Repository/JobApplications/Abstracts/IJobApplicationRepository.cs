using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.JobApplications.Abstracts;

public interface IJobApplicationRepository : IGenericRepository<JobApplication,Guid>
{
    Task<bool> HasUserAppliedAsync(string userId, Guid jobPostingId);
    Task<List<JobApplication>> GetApplicationsByUserIdAsync(string userId);
    Task<List<JobApplication>> GetApplicationsByJobPostingIdAsync(Guid jobPostingId);
}
