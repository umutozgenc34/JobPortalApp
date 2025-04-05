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
using Microsoft.Extensions.Configuration;
using JobPortalApp.Service.Roles.Abstracts;
using JobPortalApp.Service.Roles.Concretes;
using JobPortalApp.Service.Tokens.Abstracts;
using JobPortalApp.Service.Tokens.Concretes;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Service.Users.Concretes;
using JobPortalApp.Service.Auths.Abstracts;
using JobPortalApp.Service.Auths.Concretes;
using JobPortalApp.Shared.Security.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using JobPortalApp.Shared.Security.Encryption;
using JobPortalApp.Repository.JobApplications.Abstracts;
using JobPortalApp.Service.JobApplications.Abstracts;
using JobPortalApp.Service.JobApplications.Concretes;

namespace JobPortalApp.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services,Type assembly,IConfiguration configuration)
    {
        services.AddAutoMapper(assembly);
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining(assembly);

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICompanyService,CompanyService>();
        services.AddScoped<ICompanyReviewService,CompanyReviewService>();
        services.AddScoped<IJobPostingService, JobPostingService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRefreshTokenService, UserRefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();

        services.Configure<CustomTokenOption>(configuration.GetSection("TokenOption"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
        {
            var tokenOptions = configuration.GetSection("TokenOption").Get<CustomTokenOption>();
            opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                ValidIssuer = tokenOptions.Issuer,
                ValidAudience = tokenOptions.Audience[0],
                IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

        return services;
    }
}
