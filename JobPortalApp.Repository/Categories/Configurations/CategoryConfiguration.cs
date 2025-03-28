using JobPortalApp.Model.Categories.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Categories.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);

        builder.HasMany(c => c.JobPostings)
               .WithOne(j => j.Category)
               .HasForeignKey(j => j.CategoryId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
