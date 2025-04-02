using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Companies.Abstracts;
using JobPortalApp.Shared.Responses;
using System.Net;

namespace JobPortalApp.Service.Companies.Concretes;

public class CompanyReviewService(ICompanyReviewRepository companyReviewRepository, IUnitOfWork unitOfWork, IMapper mapper)
    : ICompanyReviewService
{
    public async Task<ServiceResult<CompanyReviewDto>> CreateAsync(CreateCompanyReviewRequest request)
    {
        var companyReview = mapper.Map<CompanyReview>(request);

        await companyReviewRepository.AddAsync(companyReview);
        await unitOfWork.SaveChangesAsync();

        var companyReviewDto = mapper.Map<CompanyReviewDto>(companyReview);
        return ServiceResult<CompanyReviewDto>.SuccessAsCreated(companyReviewDto, $"companyreviews/{companyReview.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var companyReview = await companyReviewRepository.GetByIdAsync(id);
        if (companyReview is null)
        {
            return ServiceResult.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        companyReviewRepository.Delete(companyReview);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Company review deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CompanyReviewDto>>> GetAllAsync()
    {
        var companyReviews = await companyReviewRepository.GetAllAsync();
        var companyReviewsAsDto = mapper.Map<List<CompanyReviewDto>>(companyReviews);
        return ServiceResult<List<CompanyReviewDto>>.Success(companyReviewsAsDto);
    }

    public async Task<ServiceResult<CompanyReviewDto>> GetByIdAsync(int id)
    {
        var companyReview = await companyReviewRepository.GetByIdAsync(id);
        if (companyReview is null)
        {
            return ServiceResult<CompanyReviewDto>.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        var companyReviewDto = mapper.Map<CompanyReviewDto>(companyReview);
        return ServiceResult<CompanyReviewDto>.Success(companyReviewDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCompanyReviewRequest request)
    {
        var companyReview = await companyReviewRepository.GetByIdAsync(id);
        if (companyReview is null)
        {
            return ServiceResult.Fail("Company review not found.", HttpStatusCode.NotFound);
        }

        mapper.Map(request, companyReview);

        companyReviewRepository.Update(companyReview);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Company review updated successfully.", HttpStatusCode.NoContent);
    }
}