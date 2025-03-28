using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using JobPortalApp.Service.Categories.Abstracts;
using JobPortalApp.Service.Categories.Concretes;
using JobPortalApp.Service.Companies.Abstracts;
using JobPortalApp.Service.Companies.Concretes;
using JobPortalApp.Service.JobPostings.Abstracts;
using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Service.JobPostings.Concretes;

namespace JobPortalApp.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services,Type assembly)
    {
        services.AddAutoMapper(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(assembly);

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICompanyService,CompanyService>();
        services.AddScoped<IJobPostingService, JobPostingService>();

        return services;
    }
}
