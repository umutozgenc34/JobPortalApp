using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Categories.Concretes;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Concretes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobPortalApp.Repository.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtensions(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICategoryRepository,CategoryRepository>();

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });
        return services;
    }
}
