using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Model.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobPortalApp.Repository.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User,IdentityRole,string>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens{ get; set; }
    public DbSet<CompanyReview> CompanyReviews { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
