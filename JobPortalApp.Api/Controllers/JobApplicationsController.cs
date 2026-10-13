using JobPortalApp.Api.Extensions;
using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Service.JobApplications.Abstracts;
using JobPortalApp.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

[Authorize]
public class JobApplicationsController(IJobApplicationService jobApplicationService) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> ApplyToJob([FromBody] ApplyToJobRequest request)
        => CreateActionResult(await jobApplicationService.ApplyAsync(request, User.GetUserId()));

    [HttpGet("by-user")]
    public async Task<IActionResult> GetMyApplications()
        => CreateActionResult(await jobApplicationService.GetApplicationsByUserIdAsync(User.GetUserId()));

    [Authorize(Roles = AppRoles.Administrators)]
    [HttpGet("by-jobposting/{jobPostingId:guid}")]
    public async Task<IActionResult> GetApplicationsByJobPostingId([FromRoute] Guid jobPostingId) =>
        CreateActionResult(await jobApplicationService.GetApplicationsByJobPostingIdAsync(jobPostingId));
}
