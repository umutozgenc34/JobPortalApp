using JobPortalApp.Model.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Users.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.FullName)
               .HasMaxLength(100);

        builder.Property(up => up.ProfileImageUrl)
               .HasMaxLength(255);

        builder.Property(up => up.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(up => up.CvFilePath)
               .HasMaxLength(255);

        builder.Navigation(x => x.User).AutoInclude();
    }
}
