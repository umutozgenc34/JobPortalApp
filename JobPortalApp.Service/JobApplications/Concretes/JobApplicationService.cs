using AutoMapper;
using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Repository.JobApplications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.JobApplications.Abstracts;
using JobPortalApp.Shared.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.JobApplications.Concretes;

public class JobApplicationService(
    IJobApplicationRepository jobApplicationRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<JobApplicationService> logger
) : IJobApplicationService
{
    public async Task<ServiceResult<JobApplicationDto>> ApplyAsync(ApplyToJobRequest request, string userId)
    {
        logger.LogInformation("ApplyAsync started for UserId: {UserId}, JobPostingId: {JobPostingId}", userId, request.JobPostingId);

        var hasApplied = await jobApplicationRepository.HasUserAppliedAsync(userId, request.JobPostingId);
        if (hasApplied)
        {
            logger.LogWarning("ApplyAsync failed. User {UserId} has already applied to JobPostingId {JobPostingId}", userId, request.JobPostingId);
            return ServiceResult<JobApplicationDto>.Fail("You have already applied to this job.", HttpStatusCode.BadRequest);
        }

        var application = new JobApplication
        {
            Id = NewId.NextSequentialGuid(),
            UserId = userId,
            JobPostingId = request.JobPostingId,
            CoverLetter = request.CoverLetter,
        };

        await jobApplicationRepository.AddAsync(application);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Application created with ID: {ApplicationId} for UserId: {UserId}", application.Id, userId);

        var applicationDto = mapper.Map<JobApplicationDto>(application);
        logger.LogInformation("ApplyAsync finished successfully for ApplicationId: {ApplicationId}", application.Id);
        return ServiceResult<JobApplicationDto>.SuccessAsCreated(applicationDto, $"applications/{application.Id}");
    }

    public async Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByUserIdAsync(string userId)
    {
        logger.LogInformation("GetApplicationsByUserIdAsync called for UserId: {UserId}", userId);

        var applications = await jobApplicationRepository.GetApplicationsByUserIdAsync(userId);
        var applicationDtos = mapper.Map<List<JobApplicationDto>>(applications);

        logger.LogInformation("GetApplicationsByUserIdAsync returned {Count} applications for UserId: {UserId}", applicationDtos.Count, userId);
        return ServiceResult<List<JobApplicationDto>>.Success(applicationDtos);
    }

    public async Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByJobPostingIdAsync(Guid jobPostingId)
    {
        logger.LogInformation("GetApplicationsByJobPostingIdAsync called for JobPostingId: {JobPostingId}", jobPostingId);

        var applications = await jobApplicationRepository.GetApplicationsByJobPostingIdAsync(jobPostingId);
        var applicationDtos = mapper.Map<List<JobApplicationDto>>(applications);

        logger.LogInformation("GetApplicationsByJobPostingIdAsync returned {Count} applications for JobPostingId: {JobPostingId}", applicationDtos.Count, jobPostingId);
        return ServiceResult<List<JobApplicationDto>>.Success(applicationDtos);
    }
}