using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using JobPortalApp.Service.Categories.Abstracts;
using JobPortalApp.Service.Categories.Concretes;

namespace JobPortalApp.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services,Type assembly)
    {
        services.AddAutoMapper(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(assembly);

        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}
