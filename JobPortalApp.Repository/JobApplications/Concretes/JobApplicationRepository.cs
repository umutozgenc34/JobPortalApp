using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.JobApplications.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.JobApplications.Concretes;

public class JobApplicationRepository(AppDbContext context) : GenericRepository<AppDbContext,JobApplication, Guid>(context), IJobApplicationRepository
{
    public async Task<bool> HasUserAppliedAsync(string userId, Guid jobPostingId) => await context.JobApplications
            .AnyAsync(x => x.UserId == userId && x.JobPostingId == jobPostingId);

    public async Task<List<JobApplication>> GetApplicationsByUserIdAsync(string userId) =>  await context.JobApplications
            .Include(x => x.JobPosting)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    public async Task<List<JobApplication>> GetApplicationsByJobPostingIdAsync(Guid jobPostingId) => await context.JobApplications
            .Include(x => x.User)
            .Where(x => x.JobPostingId == jobPostingId)
            .ToListAsync();

}
