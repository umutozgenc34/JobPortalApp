using JobPortalApp.Model.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Users.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(u => u.UserName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasOne(u => u.UserProfile)
               .WithOne(up => up.User)
               .HasForeignKey<UserProfile>(up => up.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}
