using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Api.Extensions;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));

        if (app.Environment.IsDevelopment())
        {
            await services.GetRequiredService<AppDbContext>().Database.MigrateAsync();
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Role {Role} created.", role);
            }
        }

        var configuration = services.GetRequiredService<IConfiguration>();
        var email = configuration["AdminUser:Email"];
        var userName = configuration["AdminUser:UserName"];
        var password = configuration["AdminUser:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogInformation("AdminUser settings are not configured. Admin seeding skipped.");
            return;
        }

        var userManager = services.GetRequiredService<UserManager<User>>();
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new User { Email = email, UserName = userName, EmailConfirmed = true };
            var createResult = await userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
            {
                logger.LogError("Admin user could not be created: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }
            logger.LogInformation("Admin user {Email} created.", email);
        }

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            logger.LogInformation("Admin role assigned to {Email}.", email);
        }
    }
}
