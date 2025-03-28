using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.JobPostings.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Repository.JobPostings.Concretes;

public class JobPostingRepository(AppDbContext context) : GenericRepository<AppDbContext,JobPosting,Guid>(context),IJobPostingRepository;
