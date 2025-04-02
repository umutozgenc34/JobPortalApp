using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Companies.Abstracts;

public interface ICompanyReviewService
{
    Task<ServiceResult<CompanyReviewDto>> CreateAsync(CreateCompanyReviewRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<List<CompanyReviewDto>>> GetAllAsync();
    Task<ServiceResult<CompanyReviewDto>> GetByIdAsync(int id);
    Task<ServiceResult> UpdateAsync(int id, UpdateCompanyReviewRequest request);
}
