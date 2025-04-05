using JobPortalApp.Model.JobApplications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.JobApplications.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("JobApplications");

        builder.HasKey(ja => ja.Id);

        builder.Property(ja => ja.CoverLetter)
               .HasMaxLength(1000);


        builder.Property(ja => ja.UserId)
               .IsRequired();

        builder.HasOne(ja => ja.User)
               .WithMany(u => u.JobApplications)  
               .HasForeignKey(ja => ja.UserId)   
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_JobApplications_Users");

        builder.Property(ja => ja.JobPostingId)
               .IsRequired();

        builder.HasOne(ja => ja.JobPosting)
               .WithMany(jp => jp.JobApplications)
               .HasForeignKey(ja => ja.JobPostingId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_JobApplications_JobPostings");

        builder.Navigation(x => x.User).AutoInclude();

    }
}
