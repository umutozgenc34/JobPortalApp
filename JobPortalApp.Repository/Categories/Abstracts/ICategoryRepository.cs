using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.Categories.Abstracts;

public interface ICategoryRepository : IGenericRepository<Category, int>
{
    IQueryable<Category?> GetCategoryWithJobPostings();
    Task<Category?> GetCategoryWithJobPostingsAsync(int id);
}
