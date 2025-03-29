using AutoMapper;
using JobPortalApp.Model.Categories.Dtos;
using JobPortalApp.Model.Categories.Entities;

namespace JobPortalApp.Service.Categories.Profiles;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<UpdateCategoryRequest, Category>();
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, CategoryWithJobPostingsDto>();
    }
}