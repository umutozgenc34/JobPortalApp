using AutoMapper;
using JobPortalApp.Model.JobApplications.Dtos;
using JobPortalApp.Model.JobApplications.Entities;
using JobPortalApp.Repository.JobApplications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.JobApplications.Abstracts;
using JobPortalApp.Shared.Responses;
using MassTransit;
using System.Net;

namespace JobPortalApp.Service.JobApplications.Concretes;

public class JobApplicationService(
    IJobApplicationRepository jobApplicationRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IJobApplicationService
{
    public async Task<ServiceResult<JobApplicationDto>> ApplyAsync(ApplyToJobRequest request, string userId)
    {
        var hasApplied = await jobApplicationRepository.HasUserAppliedAsync(userId, request.JobPostingId);
        if (hasApplied)
        {
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

        var applicationDto = mapper.Map<JobApplicationDto>(application);
        return ServiceResult<JobApplicationDto>.SuccessAsCreated(applicationDto, $"applications/{application.Id}");
    }

    public async Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByUserIdAsync(string userId)
    {
        var applications = await jobApplicationRepository.GetApplicationsByUserIdAsync(userId);
        var applicationDtos = mapper.Map<List<JobApplicationDto>>(applications);
        return ServiceResult<List<JobApplicationDto>>.Success(applicationDtos);
    }

    public async Task<ServiceResult<List<JobApplicationDto>>> GetApplicationsByJobPostingIdAsync(Guid jobPostingId)
    {
        var applications = await jobApplicationRepository.GetApplicationsByJobPostingIdAsync(jobPostingId);
        var applicationDtos = mapper.Map<List<JobApplicationDto>>(applications);
        return ServiceResult<List<JobApplicationDto>>.Success(applicationDtos);
    }
}