using JobPortalApp.Model.Companies.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Companies.Configurations;

public class CompanyReviewConfiguration : IEntityTypeConfiguration<CompanyReview>
{
    public void Configure(EntityTypeBuilder<CompanyReview> builder)
    {
        builder.ToTable("CompanyReviews");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Rating)
               .IsRequired()
               .HasDefaultValue(1) 
               .HasColumnType("tinyint"); 

        builder.Property(cr => cr.Comment)
               .IsRequired()
               .HasMaxLength(1000); 

        builder.HasOne(cr => cr.Company)
               .WithMany()
               .HasForeignKey(cr => cr.CompanyId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.User)
               .WithMany()
               .HasForeignKey(cr => cr.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
