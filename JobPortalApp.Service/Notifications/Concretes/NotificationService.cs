using AutoMapper;
using JobPortalApp.Model.Notifications.Dtos;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Notifications.Abstracts;
using JobPortalApp.Shared.Responses;
using Microsoft.Extensions.Logging;

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
}
