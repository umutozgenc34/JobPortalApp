using JobPortalApp.Model.Notifications.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Repository.Notifications.Abstracts;

public interface INotificationRepository : IGenericRepository<Notification, Guid>
{
    Task<List<Notification>> GetUserNotificationsAsync(string userId);
}