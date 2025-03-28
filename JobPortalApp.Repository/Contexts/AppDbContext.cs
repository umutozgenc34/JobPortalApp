using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Model.JobPostings.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobPortalApp.Repository.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
