using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Repositories.Concretes;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.Companies.Concretes;

public class CompanyRepository(AppDbContext context) : GenericRepository<AppDbContext, Company, int>(context), ICompanyRepository
{
    public IQueryable<Company?> GetCompanyWithJobPostings() => Context.Companies.Include(x => x.JobPostings).AsQueryable();

    public Task<Company?> GetCompanyWithJobPostingsAsync(int id) => Context.Companies.Include(x => x.JobPostings).FirstOrDefaultAsync(x => x.Id == id);
}

