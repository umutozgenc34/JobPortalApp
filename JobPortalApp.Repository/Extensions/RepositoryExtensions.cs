using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Categories.Concretes;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.Companies.Concretes;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.Interceptors;
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
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.Decorate<ICompanyRepository,CompanyRepositoryWithCache>();

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            opt.AddInterceptors(new AuditDbContextInterceptor());
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        return services;
    }
}
