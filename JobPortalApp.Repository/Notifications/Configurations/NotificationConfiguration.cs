﻿using JobPortalApp.Model.Notifications.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobPortalApp.Repository.Notifications.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.Title)
              .IsRequired()
              .HasMaxLength(200);

        builder.Property(n => n.Message)
               .IsRequired()
               .HasMaxLength(1000);

        builder.HasOne(n => n.User)
               .WithMany(u => u.Notifications)
               .HasForeignKey(n => n.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
