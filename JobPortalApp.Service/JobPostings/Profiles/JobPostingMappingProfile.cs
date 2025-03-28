using AutoMapper;
using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Model.JobPostings.Entities;

namespace JobPortalApp.Service.JobPostings.Profiles;

public class JobPostingMappingProfile : Profile
{
    public JobPostingMappingProfile()
    {
        CreateMap<CreateJobPostingRequest, JobPosting>();
        CreateMap<UpdateJobPostingRequest,JobPosting>();
        CreateMap<JobPosting, JobPostingDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ReverseMap();
    }
}
