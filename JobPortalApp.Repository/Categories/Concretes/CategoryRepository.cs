using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Repositories.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Repository.Categories.Concretes;

public class CategoryRepository(AppDbContext context) : GenericRepository<AppDbContext,Category,int>(context),ICategoryRepository;
