using AutoMapper;
using JobPortalApp.Model.Categories.Dtos;
using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Categories.Abstracts;
using JobPortalApp.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace JobPortalApp.Service.Categories.Concretes;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryRequest request)
    {
        var existingCategory = await categoryRepository.Where(c => c.Name == request.Name).FirstOrDefaultAsync();
        if (existingCategory is not null)
        {
            return ServiceResult<CategoryDto>.Fail("This category name already exists.", HttpStatusCode.BadRequest);
        }

        var category = mapper.Map<Category>(request);

        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        var categoryAsDto = mapper.Map<CategoryDto>(category);
        return ServiceResult<CategoryDto>.SuccessAsCreated(categoryAsDto, $"categories/{category.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Category deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        var categoryAsDtos = mapper.Map<List<CategoryDto>>(categories);
        return ServiceResult<List<CategoryDto>>.Success(categoryAsDtos);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult<CategoryDto>.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);
        return ServiceResult<CategoryDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            return ServiceResult.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        var isCategoryNameExist = await categoryRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();
        if (isCategoryNameExist)
        {
            return ServiceResult.Fail("This category name already exists.", HttpStatusCode.BadRequest);
        }

        category.Name = request.Name;

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Category updated successfully.", HttpStatusCode.NoContent);
    }

}