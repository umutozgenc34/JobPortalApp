using AutoMapper;
using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Repository.JobPostings.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.JobPostings.Abstracts;
using JobPortalApp.Shared.Responses;
using MassTransit;
using System.Net;

namespace JobPortalApp.Service.JobPostings.Concretes;

public class JobPostingService(IJobPostingRepository jobPostingRepository, IUnitOfWork unitOfWork, IMapper mapper) : IJobPostingService
{
    public async Task<ServiceResult<JobPostingDto>> CreateAsync(CreateJobPostingRequest request)
    {
        var jobPosting = mapper.Map<JobPosting>(request);
        jobPosting.Id = NewId.NextSequentialGuid(); // snowflake uyumlu

        await jobPostingRepository.AddAsync(jobPosting);
        await unitOfWork.SaveChangesAsync();

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
        return ServiceResult<JobPostingDto>.SuccessAsCreated(jobPostingAsDto, $"jobpostings/{jobPosting.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            return ServiceResult.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        jobPostingRepository.Delete(jobPosting);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Job posting deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<JobPostingDto>>> GetAllAsync()
    {
        var jobPostings = await jobPostingRepository.GetAllAsync();
        var jobPostingsAsDto = mapper.Map<List<JobPostingDto>>(jobPostings);
        return ServiceResult<List<JobPostingDto>>.Success(jobPostingsAsDto);
    }

    public async Task<ServiceResult<JobPostingDto>> GetByIdAsync(Guid id)
    {
        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            return ServiceResult<JobPostingDto>.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
        return ServiceResult<JobPostingDto>.Success(jobPostingAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(Guid id, UpdateJobPostingRequest request)
    {
        var jobPosting = await jobPostingRepository.GetByIdAsync(id);
        if (jobPosting is null)
        {
            return ServiceResult.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, jobPosting);

        jobPostingRepository.Update(jobPosting);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Job posting updated successfully.", HttpStatusCode.NoContent);
    }
}