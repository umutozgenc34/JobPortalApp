using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Service.Companies.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class CompanyReviewsController(ICompanyReviewService companyReviewService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
        => CreateActionResult(await companyReviewService.GetAllAsync());

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int id)
        => CreateActionResult(await companyReviewService.GetByIdAsync(id));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateCompanyReviewRequest request)
        => CreateActionResult(await companyReviewService.CreateAsync(request));

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] UpdateCompanyReviewRequest request)
        => CreateActionResult(await companyReviewService.UpdateAsync(id, request));
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview([FromRoute] int id)
        => CreateActionResult(await companyReviewService.DeleteAsync(id));
}