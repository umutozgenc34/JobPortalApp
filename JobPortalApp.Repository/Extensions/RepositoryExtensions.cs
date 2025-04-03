using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Categories.Concretes;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.Companies.Concretes;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.Interceptors;
using JobPortalApp.Repository.JobPostings.Abstracts;
using JobPortalApp.Repository.JobPostings.Concretes;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Concretes;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Repository.Users.Concretes;
using Microsoft.AspNetCore.Identity;
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
        services.AddScoped<ICompanyReviewRepository, CompanyReviewRepository>();
        services.Decorate<ICompanyReviewRepository,CompanyReviewRepositoryWithCache>();
        services.AddScoped<IJobPostingRepository, JobPostingRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            opt.AddInterceptors(new AuditDbContextInterceptor());
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
        return services;
    }
}
