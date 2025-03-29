using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Service.Companies.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class CompaniesController(ICompanyService companyService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllCompanies() => CreateActionResult(await companyService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCompanyById([FromRoute] int id) => CreateActionResult(await companyService.GetByIdAsync(id));

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyRequest request) => CreateActionResult(await companyService.CreateAsync(request));

    [HttpPut("{id:int}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCompany([FromRoute] int id, [FromForm] UpdateCompanyRequest request) => CreateActionResult(await companyService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompany([FromRoute] int id) => CreateActionResult(await companyService.DeleteAsync(id));

    [HttpGet("{id:int}/jobpostings")]
    public async Task<IActionResult> GetCompanyWithJobPostings([FromRoute] int id) => CreateActionResult(await companyService
    .GetCompanyWithJobPostingsAsync(id));

    [HttpGet("jobpostings")]
    public async Task<IActionResult> GetCompanyWithJobPostings() => CreateActionResult(await companyService
        .GetCompanyWithJobPostingsAsync());
}