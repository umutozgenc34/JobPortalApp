using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Service.Companies.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompanyReviewsController(ICompanyReviewService companyReviewService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllReviews()
        => CreateActionResult(await companyReviewService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int id)
        => CreateActionResult(await companyReviewService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateCompanyReviewRequest request)
        => CreateActionResult(await companyReviewService.CreateAsync(request));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] UpdateCompanyReviewRequest request)
        => CreateActionResult(await companyReviewService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReview([FromRoute] int id)
        => CreateActionResult(await companyReviewService.DeleteAsync(id));
}