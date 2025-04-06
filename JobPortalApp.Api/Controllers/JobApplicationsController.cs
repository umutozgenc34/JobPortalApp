using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Service.JobApplications.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortalApp.Api.Controllers;

[Authorize]
public class JobApplicationsController(IJobApplicationService jobApplicationService) : CustomBaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ApplyToJob([FromBody] ApplyToJobRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User not authenticated.");
        }

        return CreateActionResult(await jobApplicationService.ApplyAsync(request, userId));
    }
    [Authorize]
    [HttpGet("by-user")]
    public async Task<IActionResult> GetMyApplications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User not authenticated.");
        }

        return CreateActionResult(await jobApplicationService.GetApplicationsByUserIdAsync(userId));
    }

    [AllowAnonymous]
    [HttpGet("by-jobposting/{jobPostingId:guid}")]
    public async Task<IActionResult> GetApplicationsByJobPostingId([FromRoute] Guid jobPostingId) =>
        CreateActionResult(await jobApplicationService.GetApplicationsByJobPostingIdAsync(jobPostingId));
}