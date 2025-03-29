using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Repositories.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.Categories.Concretes;

public class CategoryRepository(AppDbContext context) : GenericRepository<AppDbContext, Category, int>(context), ICategoryRepository
{
    public IQueryable<Category?> GetCategoryWithJobPostings() => Context.Categories.Include(x => x.JobPostings).AsQueryable();

    public Task<Category?> GetCategoryWithJobPostingsAsync(int id) => Context.Categories.Include(x => x.JobPostings).FirstOrDefaultAsync(x => x.Id == id);
}
