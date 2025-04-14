using JobPortalApp.Model.Notifications.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Notifications.Abstracts;

public interface INotificationService
{
    Task<ServiceResult<List<NotificationDto>>> GetUserNotificationAsync(string userId);
}
