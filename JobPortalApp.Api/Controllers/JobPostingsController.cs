using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Service.JobPostings.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class JobPostingsController(IJobPostingService jobPostingService) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAllJobPostings() => CreateActionResult(await jobPostingService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetJobPostingById([FromRoute] Guid id) => CreateActionResult(await jobPostingService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateJobPosting([FromBody] CreateJobPostingRequest request) => CreateActionResult(await jobPostingService.CreateAsync(request));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateJobPosting([FromRoute] Guid id, [FromBody] UpdateJobPostingRequest request) => CreateActionResult(await jobPostingService.UpdateAsync(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteJobPosting([FromRoute] Guid id) => CreateActionResult(await jobPostingService.DeleteAsync(id));
}

