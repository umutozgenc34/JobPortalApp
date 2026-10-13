using AutoMapper;
using JobPortalApp.Model.JobPostings.Dtos;
using JobPortalApp.Model.JobPostings.Entities;
using JobPortalApp.Repository.Categories.Abstracts;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.JobPostings.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.JobPostings.Abstracts;
using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Service.Rabbit.Publishers;
using JobPortalApp.Shared.Responses;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.JobPostings.Concretes;

public class JobPostingService(IJobPostingRepository jobPostingRepository, ICompanyRepository companyRepository, ICategoryRepository categoryRepository,
                               IUnitOfWork unitOfWork, IMapper mapper, NotificationPublisher notificationPublisher,
                               ILogger<JobPostingService> logger) : IJobPostingService
{
    private const int MaxPageSize = 50;

    public async Task<ServiceResult<JobPostingDto>> CreateAsync(CreateJobPostingRequest request)
    {
        logger.LogInformation("CreateAsync started with Title: {Title}", request.Title);

        var referenceError = await ValidateReferencesAsync(request.CompanyId, request.CategoryId);
        if (referenceError is not null)
        {
            return ServiceResult<JobPostingDto>.Fail(referenceError, HttpStatusCode.BadRequest);
        }

        var jobPosting = mapper.Map<JobPosting>(request);
        jobPosting.Id = NewId.NextSequentialGuid();

        await jobPostingRepository.AddAsync(jobPosting);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("JobPosting created with ID: {JobPostingId}", jobPosting.Id);

        if (jobPosting.IsActive)
        {
            notificationPublisher.Publish(new NotificationMessage
            {
                Title = "New job posting is published!",
                Message = $"A new job posting titled '{jobPosting.Title}' has been added."
            });
        }

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
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

    public async Task<ServiceResult<PagedResult<JobPostingDto>>> GetAllAsync(JobPostingQuery query)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);

        var jobPostings = jobPostingRepository.Where().AsNoTracking();

        if (!query.IncludeInactive)
        {
            jobPostings = jobPostings.Where(x => x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            jobPostings = jobPostings.Where(x =>
                x.Title.Contains(search) ||
                x.Description.Contains(search) ||
                x.Company.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Location))
        {
            var location = query.Location.Trim();
            jobPostings = jobPostings.Where(x => x.Location.Contains(location));
        }

        if (query.CategoryId is not null)
        {
            jobPostings = jobPostings.Where(x => x.CategoryId == query.CategoryId);
        }

        if (query.CompanyId is not null)
        {
            jobPostings = jobPostings.Where(x => x.CompanyId == query.CompanyId);
        }

        var totalCount = await jobPostings.CountAsync();
        var items = await jobPostings
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var itemsAsDto = mapper.Map<List<JobPostingDto>>(items);
        logger.LogInformation("GetAllAsync returned {Count} of {TotalCount} job postings.", itemsAsDto.Count, totalCount);

        return ServiceResult<PagedResult<JobPostingDto>>.Success(new PagedResult<JobPostingDto>(itemsAsDto, page, pageSize, totalCount));
    }

    public async Task<ServiceResult<JobPostingDto>> GetByIdAsync(Guid id)
    {
        logger.LogInformation("GetByIdAsync called for ID: {Id}", id);

        var jobPosting = await jobPostingRepository.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
        if (jobPosting is null)
        {
            logger.LogWarning("GetByIdAsync failed. JobPosting not found with ID: {Id}", id);
            return ServiceResult<JobPostingDto>.Fail("Job posting not found.", HttpStatusCode.NotFound);
        }

        var jobPostingAsDto = mapper.Map<JobPostingDto>(jobPosting);
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

        var referenceError = await ValidateReferencesAsync(request.CompanyId, request.CategoryId);
        if (referenceError is not null)
        {
            return ServiceResult.Fail(referenceError, HttpStatusCode.BadRequest);
        }

        mapper.Map(request, jobPosting);

        jobPostingRepository.Update(jobPosting);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UpdateAsync completed successfully for ID: {Id}", id);
        return ServiceResult.Success("Job posting updated successfully.", HttpStatusCode.NoContent);
    }

    private async Task<string?> ValidateReferencesAsync(int companyId, int categoryId)
    {
        var company = await companyRepository.GetByIdAsync(companyId);
        if (company is null) return "Company not found.";

        var category = await categoryRepository.GetByIdAsync(categoryId);
        if (category is null) return "Category not found.";

        return null;
    }
}
