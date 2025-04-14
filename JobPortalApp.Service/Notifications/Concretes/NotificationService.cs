using AutoMapper;
using JobPortalApp.Model.Notifications.Dtos;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Notifications.Abstracts;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Notifications.Concretes;

public class NotificationService(INotificationRepository notificationRepository, IMapper mapper, IUnitOfWork unitOfWork) : INotificationService
{
    public async Task<ServiceResult<List<NotificationDto>>> GetUserNotificationAsync(string userId)
    {
        var notifications = await notificationRepository.GetUserNotificationsAsync(userId);
        var notificationsAsDto = mapper.Map<List<NotificationDto>>(notifications);

        return ServiceResult<List<NotificationDto>>.Success(notificationsAsDto);
    }
}
