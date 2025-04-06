using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Service.Companies.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class CompaniesController(ICompanyService companyService) : CustomBaseController
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies() => CreateActionResult(await companyService.GetAllAsync());
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCompanyById([FromRoute] int id) => CreateActionResult(await companyService.GetByIdAsync(id));
    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyRequest request) => CreateActionResult(await companyService.CreateAsync(request));
    [Authorize]
    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCompany([FromRoute] int id, [FromForm] UpdateCompanyRequest request) => CreateActionResult(await companyService.UpdateAsync(id, request));
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompany([FromRoute] int id) => CreateActionResult(await companyService.DeleteAsync(id));
    [Authorize]
    [HttpGet("{id:int}/jobpostings")]
    public async Task<IActionResult> GetCompanyWithJobPostings([FromRoute] int id) => CreateActionResult(await companyService
    .GetCompanyWithJobPostingsAsync(id));
    [Authorize]
    [HttpGet("jobpostings")]
    public async Task<IActionResult> GetCompanyWithJobPostings() => CreateActionResult(await companyService
        .GetCompanyWithJobPostingsAsync());
}