using AutoMapper;
using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Model.JobApplications.Entities;

namespace JobPortalApp.Service.JobApplications.Profiles;

public class JobApplicationMappingProfile : Profile
{
    public JobApplicationMappingProfile()
    {
        CreateMap<JobApplication, JobApplicationDto>()
            .ForMember(dest => dest.JobPostingTitle, opt => opt.MapFrom(src => src.JobPosting.Title))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));
    }
}