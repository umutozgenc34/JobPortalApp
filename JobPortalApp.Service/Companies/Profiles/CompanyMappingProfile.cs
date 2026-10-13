using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;

namespace JobPortalApp.Service.Companies.Profiles;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<CreateCompanyRequest, Company>()
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());
        CreateMap<UpdateCompanyRequest, Company>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LogoUrl, opt => opt.Ignore());
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
