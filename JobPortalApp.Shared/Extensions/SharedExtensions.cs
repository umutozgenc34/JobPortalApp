using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using JobPortalApp.Shared.Services.Cloudinaryy.Concretes;
using JobPortalApp.Shared.Services.Cloudinaryy.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobPortalApp.Shared.Extensions;

public static class SharedExtensions
{
    public static IServiceCollection AddSharedExtension(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.Configure<CloudinarySettings>(opt =>
        {
            configuration.GetSection("CloudinarySettings").Bind(opt);
        });
        return services;

    }
}
