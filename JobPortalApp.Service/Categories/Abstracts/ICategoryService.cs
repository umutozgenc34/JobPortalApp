using JobPortalApp.Model.Categories.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Categories.Abstracts;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<CategoryWithJobPostingsDto>> GetCategoryWithJobPostingsAsync(int categoryId);
    Task<ServiceResult<List<CategoryWithJobPostingsDto>>> GetCategoryWithJobPostingsAsync();
}