using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;

namespace JobPortalApp.Repository.UnitOfWorks.Concretes;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}