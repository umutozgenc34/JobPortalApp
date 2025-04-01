using JobPortalApp.Model.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Companies.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(c => c.Industry)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.Email)
               .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(c => c.WebsiteUrl)
               .HasMaxLength(200);

        builder.Property(c => c.Location)
               .HasMaxLength(150);

        builder.Property(c => c.About)
               .HasColumnType("TEXT");

        builder.HasMany(c => c.JobPostings)
               .WithOne(j => j.Company)
               .HasForeignKey(j => j.CompanyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<CompanyReview>()
              .WithOne(cr => cr.Company)
              .HasForeignKey(cr => cr.CompanyId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
