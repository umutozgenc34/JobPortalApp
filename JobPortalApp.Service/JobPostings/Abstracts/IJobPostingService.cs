using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.JobPostings.Abstracts;

public interface IJobPostingService
{
    Task<ServiceResult<JobPostingDto>> CreateAsync(CreateJobPostingRequest request);
    Task<ServiceResult> DeleteAsync(Guid id);
    Task<ServiceResult<PagedResult<JobPostingDto>>> GetAllAsync(JobPostingQuery query);
    Task<ServiceResult<JobPostingDto>> GetByIdAsync(Guid id);
    Task<ServiceResult> UpdateAsync(Guid id, UpdateJobPostingRequest request);
}
