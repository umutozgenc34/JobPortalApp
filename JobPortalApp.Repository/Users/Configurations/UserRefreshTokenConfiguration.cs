using JobPortalApp.Model.Users.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.Users.Configurations;
public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired();
    }
}

