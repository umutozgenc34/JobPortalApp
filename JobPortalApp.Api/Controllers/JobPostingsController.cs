using JobPortalApp.Api.Extensions;
using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Service.JobPostings.Abstracts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class JobPostingsController(IJobPostingService jobPostingService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllJobPostings([FromQuery] JobPostingQuery query)
    {
        query.IncludeInactive = query.IncludeInactive && User.IsAdministrator();
        return CreateActionResult(await jobPostingService.GetAllAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetJobPostingById([FromRoute] Guid id) => CreateActionResult(await jobPostingService.GetByIdAsync(id));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpPost]
    public async Task<IActionResult> CreateJobPosting([FromBody] CreateJobPostingRequest request) => CreateActionResult(await jobPostingService.CreateAsync(request));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateJobPosting([FromRoute] Guid id, [FromBody] UpdateJobPostingRequest request) => CreateActionResult(await jobPostingService.UpdateAsync(id, request));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobPosting([FromRoute] Guid id) => CreateActionResult(await jobPostingService.DeleteAsync(id));
}
