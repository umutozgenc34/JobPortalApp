using AutoMapper;
using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Repository.JobPostings.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.JobPostings.Abstracts;
using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.JobPostings.Concretes;

public class JobPostingService(IJobPostingRepository jobPostingRepository, IUnitOfWork unitOfWork, IMapper mapper,NotificationPublisher notificationPublisher,
                                ILogger<JobPostingService> logger) : IJobPostingService
{
    public async Task<ServiceResult<JobPostingDto>> CreateAsync(CreateJobPostingRequest request)
    {
        logger.LogInformation("CreateAsync started with Title: {Title}", request.Title);

        var jobPosting = mapper.Map<JobPosting>(request);
        jobPosting.Id = NewId.NextSequentialGuid();  // snowflake uyumlu

        await jobPostingRepository.AddAsync(jobPosting);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("JobPosting created with ID: {JobPostingId}", jobPosting.Id);

        notificationPublisher.Publish(new NotificationMessage
        {
            Title = "New job posting is published!",
            Message = $"A new job posting titled '{jobPosting.Title}' has been added."
        });

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
        logger.LogInformation("CreateAsync finished successfully for ID: {Id}", jobPosting.Id);
        return ServiceResult<JobPostingDto>.SuccessAsCreated(jobPostingAsDto, $"jobpostings/{jobPosting.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        logger.LogInformation("DeleteAsync started for ID: {Id}", id);

        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            logger.LogWarning("DeleteAsync failed. JobPosting not found with ID: {Id}", id);
            return ServiceResult.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        jobPostingRepository.Delete(jobPosting);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("JobPosting deleted successfully. ID: {Id}", id);
        return ServiceResult.Success("Job posting deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<JobPostingDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called.");

        var jobPostings = await jobPostingRepository.GetAllAsync();
        var jobPostingsAsDto = mapper.Map<List<JobPostingDto>>(jobPostings);

        logger.LogInformation("GetAllAsync returned {Count} job postings.", jobPostingsAsDto.Count);
        return ServiceResult<List<JobPostingDto>>.Success(jobPostingsAsDto);
    }

    public async Task<ServiceResult<JobPostingDto>> GetByIdAsync(Guid id)
    {
        logger.LogInformation("GetByIdAsync called for ID: {Id}", id);

        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            logger.LogWarning("GetByIdAsync failed. JobPosting not found with ID: {Id}", id);
            return ServiceResult<JobPostingDto>.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
        logger.LogInformation("GetByIdAsync succeeded for ID: {Id}", id);
        return ServiceResult<JobPostingDto>.Success(jobPostingAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, UpdateJobPostingRequest request)
    {
        logger.LogInformation("UpdateAsync called for ID: {Id}", id);

        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            logger.LogWarning("UpdateAsync failed. JobPosting not found with ID: {Id}", id);
            return ServiceResult.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, jobPosting);

        jobPostingRepository.Update(jobPosting);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UpdateAsync completed successfully for ID: {Id}", id);
        return ServiceResult.Success("Job posting updated successfully.", HttpStatusCode.NoContent);
    }
}