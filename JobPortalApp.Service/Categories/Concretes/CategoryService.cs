using AutoMapper;
using JobPortalApp.Model.Categories.Dtos;
using JobPortalApp.Model.Categories.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Categories.Abstracts;
using JobPortalApp.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Categories.Concretes;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper,
                             ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<ServiceResult<CategoryDto>> CreateAsync(CreateCategoryRequest request)
    {
        logger.LogInformation("CreateAsync started with Name: {Name}", request.Name);

        var existingCategory = await categoryRepository.Where(c => c.Name == request.Name).FirstOrDefaultAsync();
        if (existingCategory is not null)
        {
            logger.LogWarning("CreateAsync failed. Category with name '{Name}' already exists.", request.Name);
            return ServiceResult<CategoryDto>.Fail("This category name already exists.", HttpStatusCode.BadRequest);
        }

        var category = mapper.Map<Category>(request);

        await categoryRepository.AddAsync(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Category created with ID: {CategoryId}", category.Id);

        var categoryAsDto = mapper.Map<CategoryDto>(category);
        return ServiceResult<CategoryDto>.SuccessAsCreated(categoryAsDto, $"categories/{category.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        logger.LogInformation("DeleteAsync called for ID: {Id}", id);

        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("DeleteAsync failed. Category not found with ID: {Id}", id);
            return ServiceResult.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Category deleted successfully. ID: {Id}", id);
        return ServiceResult.Success("Category deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called.");

        var categories = await categoryRepository.GetAllAsync();
        var categoryAsDtos = mapper.Map<List<CategoryDto>>(categories);

        logger.LogInformation("GetAllAsync returned {Count} categories.", categoryAsDtos.Count);
        return ServiceResult<List<CategoryDto>>.Success(categoryAsDtos);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called for ID: {Id}", id);

        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("GetByIdAsync failed. Category not found with ID: {Id}", id);
            return ServiceResult<CategoryDto>.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);
        logger.LogInformation("GetByIdAsync succeeded for ID: {Id}", id);
        return ServiceResult<CategoryDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        logger.LogInformation("UpdateAsync called for ID: {Id}", id);

        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null)
        {
            logger.LogWarning("UpdateAsync failed. Category not found with ID: {Id}", id);
            return ServiceResult.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        var isCategoryNameExist = await categoryRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();
        if (isCategoryNameExist)
        {
            logger.LogWarning("UpdateAsync failed. Category name '{Name}' already exists.", request.Name);
            return ServiceResult.Fail("This category name already exists.", HttpStatusCode.BadRequest);
        }

        category.Name = request.Name;

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UpdateAsync completed successfully for ID: {Id}", id);
        return ServiceResult.Success("Category updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<CategoryWithJobPostingsDto>> GetCategoryWithJobPostingsAsync(int categoryId)
    {
        logger.LogInformation("GetCategoryWithJobPostingsAsync called for ID: {CategoryId}", categoryId);

        var category = await categoryRepository.GetCategoryWithJobPostingsAsync(categoryId);
        if (category is null)
        {
            logger.LogWarning("GetCategoryWithJobPostingsAsync failed. Category not found with ID: {CategoryId}", categoryId);
            return ServiceResult<CategoryWithJobPostingsDto>.Fail("Category not found.", HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryWithJobPostingsDto>(category);
        logger.LogInformation("GetCategoryWithJobPostingsAsync succeeded for ID: {CategoryId}", categoryId);
        return ServiceResult<CategoryWithJobPostingsDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<List<CategoryWithJobPostingsDto>>> GetCategoryWithJobPostingsAsync()
    {
        logger.LogInformation("GetCategoryWithJobPostingsAsync (all) called.");

        var categories = await categoryRepository.GetCategoryWithJobPostings().ToListAsync();
        var categoriesAsDtos = mapper.Map<List<CategoryWithJobPostingsDto>>(categories);

        logger.LogInformation("GetCategoryWithJobPostingsAsync returned {Count} categories.", categoriesAsDtos.Count);
        return ServiceResult<List<CategoryWithJobPostingsDto>>.Success(categoriesAsDtos);
    }

}