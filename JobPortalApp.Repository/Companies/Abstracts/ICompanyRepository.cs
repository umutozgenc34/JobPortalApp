using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.Companies.Abstracts;

public interface ICompanyRepository : IGenericRepository<Company, int>
{
    IQueryable<Company?> GetCompanyWithJobPostings();
    Task<Company?> GetCompanyWithJobPostingsAsync(int id);
}
