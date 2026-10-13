using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Companies.Abstracts;
using JobPortalApp.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace JobPortalApp.Service.Companies.Concretes;

public class CompanyReviewService(ICompanyReviewRepository companyReviewRepository, ICompanyRepository companyRepository,
                                  IUnitOfWork unitOfWork, IMapper mapper) : ICompanyReviewService
{
    public async Task<ServiceResult<CompanyReviewDto>> CreateAsync(CreateCompanyReviewRequest request, string userId)
    {
        var companyExists = await companyRepository.Where(c => c.Id == request.CompanyId).AnyAsync();
        if (!companyExists)
        {
            return ServiceResult<CompanyReviewDto>.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var alreadyReviewed = await companyReviewRepository
            .Where(r => r.CompanyId == request.CompanyId && r.UserId == userId)
            .AnyAsync();
        if (alreadyReviewed)
        {
            return ServiceResult<CompanyReviewDto>.Fail("You have already reviewed this company.", HttpStatusCode.BadRequest);
        }

        var companyReview = mapper.Map<CompanyReview>(request);
        companyReview.UserId = userId;

        await companyReviewRepository.AddAsync(companyReview);
        await unitOfWork.SaveChangesAsync();

        var createdReview = await companyReviewRepository.Where(r => r.Id == companyReview.Id).FirstAsync();
        var companyReviewDto = mapper.Map<CompanyReviewDto>(createdReview);
        return ServiceResult<CompanyReviewDto>.SuccessAsCreated(companyReviewDto, $"companyreviews/{companyReview.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id, string userId, bool isAdmin)
    {
        var companyReview = await companyReviewRepository.Where(r => r.Id == id).FirstOrDefaultAsync();
        if (companyReview is null)
        {
            return ServiceResult.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        if (companyReview.UserId != userId && !isAdmin)
        {
            return ServiceResult.Fail("You can only delete your own reviews.", HttpStatusCode.Forbidden);
        }

        companyReviewRepository.Delete(companyReview);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Company review deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CompanyReviewDto>>> GetAllAsync()
    {
        var companyReviews = await companyReviewRepository.Where()
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        var companyReviewsAsDto = mapper.Map<List<CompanyReviewDto>>(companyReviews);
        return ServiceResult<List<CompanyReviewDto>>.Success(companyReviewsAsDto);
    }

    public async Task<ServiceResult<List<CompanyReviewDto>>> GetByCompanyIdAsync(int companyId)
    {
        var companyReviews = await companyReviewRepository.Where(r => r.CompanyId == companyId)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        var companyReviewsAsDto = mapper.Map<List<CompanyReviewDto>>(companyReviews);
        return ServiceResult<List<CompanyReviewDto>>.Success(companyReviewsAsDto);
    }

    public async Task<ServiceResult<CompanyReviewDto>> GetByIdAsync(int id)
    {
        var companyReview = await companyReviewRepository.Where(r => r.Id == id).AsNoTracking().FirstOrDefaultAsync();
        if (companyReview is null)
        {
            return ServiceResult<CompanyReviewDto>.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        var companyReviewDto = mapper.Map<CompanyReviewDto>(companyReview);
        return ServiceResult<CompanyReviewDto>.Success(companyReviewDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCompanyReviewRequest request, string userId)
    {
        var companyReview = await companyReviewRepository.GetByIdAsync(id);
        if (companyReview is null)
        {
            return ServiceResult.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        if (companyReview.UserId != userId)
        {
            return ServiceResult.Fail("You can only update your own reviews.", HttpStatusCode.Forbidden);
        }

        companyReview.Rating = request.Rating;
        companyReview.Comment = request.Comment;

        companyReviewRepository.Update(companyReview);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Company review updated successfully.", HttpStatusCode.NoContent);
    }
}
