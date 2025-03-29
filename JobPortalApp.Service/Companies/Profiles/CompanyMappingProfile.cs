using AutoMapper;
using JobPortalApp.Model.Categories.Dtos;
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
    }
}
