using JobPortalApp.Model.Categories.Dtos;
using JobPortalApp.Service.Categories.Abstracts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class CategoriesController(ICategoryService categoryService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories() => CreateActionResult(await categoryService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id) => CreateActionResult(await categoryService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request) => CreateActionResult(await categoryService.CreateAsync(request));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request) => CreateActionResult(await categoryService.UpdateAsync(id, request));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id) => CreateActionResult(await categoryService.DeleteAsync(id));

    [HttpGet("{id:int}/jobpostings")]
    public async Task<IActionResult> GetCategoryWithJobPostings([FromRoute] int id) => CreateActionResult(await categoryService
        .GetCategoryWithJobPostingsAsync(id));

    [HttpGet("jobpostings")]
    public async Task<IActionResult> GetCategoryWithJobPostings() => CreateActionResult(await categoryService
        .GetCategoryWithJobPostingsAsync());
}
