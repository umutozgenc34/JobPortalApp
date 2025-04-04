using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;

namespace JobPortalApp.Service.Users.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UpdateUserRequest, User>();
        CreateMap<UserDto, User>().ReverseMap();

        CreateMap<CreateUserProfileRequest, UserProfile>();
        CreateMap<UpdateUserProfileRequest, UserProfile>();
        CreateMap<UserProfile,UserProfileDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ReverseMap();
    }
}
