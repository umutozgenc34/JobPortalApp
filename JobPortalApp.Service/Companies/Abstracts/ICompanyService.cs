using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Shared.Responses;

namespace JobPortalApp.Service.Companies.Abstracts;

public interface ICompanyService
{
    Task<ServiceResult<CompanytDto>> CreateAsync(CreateCompanyRequest request);
    Task<ServiceResult> DeleteAsync(int id);
    Task<ServiceResult<List<CompanytDto>>> GetAllAsync();
    Task<ServiceResult<CompanytDto>> GetByIdAsync(int id);
    Task<ServiceResult> UpdateAsync(int id, UpdateCompanyRequest request);
    Task<ServiceResult<List<CompanyWithJobPostingsDto>>> GetCompanyWithJobPostingsAsync();
    Task<ServiceResult<CompanyWithJobPostingsDto>> GetCompanyWithJobPostingsAsync(int companyId);
}
