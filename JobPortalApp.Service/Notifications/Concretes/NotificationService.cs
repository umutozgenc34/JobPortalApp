using AutoMapper;
using JobPortalApp.Model.Notifications.Dtos;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Notifications.Abstracts;
using JobPortalApp.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Notifications.Concretes;

public class NotificationService(INotificationRepository notificationRepository, IMapper mapper, IUnitOfWork unitOfWork,ILogger <NotificationService>logger) : INotificationService
{
    public async Task<ServiceResult<List<NotificationDto>>> GetUserNotificationAsync(string userId)
    {
        logger.LogInformation("GetUserNotificationAsync called for UserId: {UserId}", userId);
        var notifications = await notificationRepository.GetUserNotificationsAsync(userId);
        var notificationsAsDto = mapper.Map<List<NotificationDto>>(notifications);

        logger.LogInformation("GetUserNotificationAsync returned {Count} notifications for UserId: {UserId}", notificationsAsDto.Count, userId);
        return ServiceResult<List<NotificationDto>>.Success(notificationsAsDto);
    }

    public async Task<ServiceResult> MarkAsReadAsync(Guid id, string userId)
    {
        var notification = await notificationRepository.Where(n => n.Id == id && n.UserId == userId).FirstOrDefaultAsync();
        if (notification is null)
        {
            return ServiceResult.Fail("Notification not found.", HttpStatusCode.NotFound);
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await unitOfWork.SaveChangesAsync();
        }

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> MarkAllAsReadAsync(string userId)
    {
        var unreadNotifications = await notificationRepository.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await unitOfWork.SaveChangesAsync();
        logger.LogInformation("{Count} notifications marked as read for UserId: {UserId}", unreadNotifications.Count, userId);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
