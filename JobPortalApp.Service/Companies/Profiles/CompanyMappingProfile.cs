using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;

namespace JobPortalApp.Service.Companies.Profiles;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<CreateCompanyRequest, Company>();
        CreateMap<UpdateCompanyRequest, Company>();
        CreateMap<Company, CompanytDto>().ReverseMap() ;
        CreateMap<Company, CompanyWithJobPostingsDto>();

        CreateMap<CreateCompanyReviewRequest, CompanyReview>();
        CreateMap<UpdateCompanyReviewRequest, CompanyReview>();
        CreateMap<CompanyReview, CompanyReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ReverseMap();
    }
}
