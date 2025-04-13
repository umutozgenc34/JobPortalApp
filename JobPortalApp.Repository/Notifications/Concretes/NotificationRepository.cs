using JobPortalApp.Model.Notifications.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;
using Microsoft.EntityFrameworkCore;

namespace JobPortalApp.Repository.Notifications.Concretes;

public class NotificationRepository(AppDbContext context) : GenericRepository<AppDbContext, Notification, Guid>(context), INotificationRepository
{
    public async Task<List<Notification>> GetUserNotificationsAsync(string userId) =>
        await context.Notifications.Where(n => n.UserId == userId)
                             .OrderByDescending(n => n.CreatedAt)
                             .ToListAsync();
}


