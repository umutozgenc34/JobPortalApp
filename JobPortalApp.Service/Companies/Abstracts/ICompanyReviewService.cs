using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Companies.Abstracts;

public interface ICompanyReviewService
{
    Task<ServiceResult<CompanyReviewDto>> CreateAsync(CreateCompanyReviewRequest request, string userId);
    Task<ServiceResult> DeleteAsync(int id, string userId, bool isAdmin);
    Task<ServiceResult<List<CompanyReviewDto>>> GetAllAsync();
    Task<ServiceResult<List<CompanyReviewDto>>> GetByCompanyIdAsync(int companyId);
    Task<ServiceResult<CompanyReviewDto>> GetByIdAsync(int id);
    Task<ServiceResult> UpdateAsync(int id, UpdateCompanyReviewRequest request, string userId);
}
