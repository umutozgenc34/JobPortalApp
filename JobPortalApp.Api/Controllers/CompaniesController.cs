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
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request) => CreateActionResult(await companyService.CreateAsync(request));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCompany([FromRoute] int id, [FromBody] UpdateCompanyRequest request) => CreateActionResult(await companyService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCompany([FromRoute] int id) => CreateActionResult(await companyService.DeleteAsync(id));
}