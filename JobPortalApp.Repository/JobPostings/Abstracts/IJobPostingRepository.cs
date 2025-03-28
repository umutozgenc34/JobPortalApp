using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.JobPostings.Abstracts;

public interface IJobPostingRepository : IGenericRepository<JobPosting,Guid>;
