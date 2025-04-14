using AutoMapper;
using JobPortalApp.Model.Notifications.Dtos;
using JobPortalApp.Model.Notifications.Entities;

namespace JobPortalApp.Service.Notifications.Profiles;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification,NotificationDto>().ReverseMap();
    }
}
