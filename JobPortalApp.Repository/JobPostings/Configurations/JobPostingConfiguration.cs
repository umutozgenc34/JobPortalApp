using JobPortalApp.Model.JobPostings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.JobPostings.Configurations;

public class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder.ToTable("JobPostings");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Title)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(j => j.Description)
               .IsRequired()
               .HasColumnType("TEXT"); 

        builder.Property(j => j.Location)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(j => j.SalaryRange)
               .HasMaxLength(50);

        builder.Property(j => j.IsActive)
               .HasDefaultValue(true);

        builder.Property(j => j.CreatedAt)
               .IsRequired();

        builder.Property(j => j.UpdatedAt)
               .IsRequired();

        builder.HasOne(j => j.Company)
               .WithMany(c => c.JobPostings)
               .HasForeignKey(j => j.CompanyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j=> j.Category)
               .WithMany(c=> c.JobPostings)
               .HasForeignKey(j => j.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);


        builder.Navigation(x => x.Category).AutoInclude();
        builder.Navigation(x => x.Company).AutoInclude();
    }
}
