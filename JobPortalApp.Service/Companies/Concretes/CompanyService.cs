using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Companies.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Companies.Concretes;

public class CompanyService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IMapper mapper,ICloudinaryService cloudinaryService,
                            ILogger<CompanyService> logger) : ICompanyService
{
    public async Task<ServiceResult<CompanytDto>> CreateAsync(CreateCompanyRequest request)
    {
        logger.LogInformation("CreateAsync started with Name: {Name}", request.Name);

        var existingCompany = await companyRepository.Where(c => c.Name == request.Name).FirstOrDefaultAsync();
        if (existingCompany is not null)
        {
            logger.LogWarning("CreateAsync failed. Company with name '{Name}' already exists.", request.Name);
            return ServiceResult<CompanytDto>.Fail("This company name already exists.", HttpStatusCode.BadRequest);
        }

        var company = mapper.Map<Company>(request);

        if (request.LogoUrl is not null)
        {
            var logoUrl = await cloudinaryService.UploadImage(request.LogoUrl, "logos");
            company.LogoUrl = logoUrl;
        }

        await companyRepository.AddAsync(company);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Company created with ID: {CompanyId}", company.Id);

        var companyAsDto = mapper.Map<CompanytDto>(company);
        logger.LogInformation("CreateAsync finished successfully for ID: {Id}", company.Id);
        return ServiceResult<CompanytDto>.SuccessAsCreated(companyAsDto, $"companies/{company.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        logger.LogInformation("DeleteAsync called for ID: {Id}", id);

        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            logger.LogWarning("DeleteAsync failed. Company not found with ID: {Id}", id);
            return ServiceResult.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        companyRepository.Delete(company);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Company deleted successfully. ID: {Id}", id);
        return ServiceResult.Success("Company deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CompanytDto>>> GetAllAsync()
    {
        logger.LogInformation("GetAllAsync called.");

        var companies = await companyRepository.GetAllAsync();
        var companyAsDtos = mapper.Map<List<CompanytDto>>(companies);

        logger.LogInformation("GetAllAsync returned {Count} companies.", companyAsDtos.Count);
        return ServiceResult<List<CompanytDto>>.Success(companyAsDtos);
    }

    public async Task<ServiceResult<CompanytDto>> GetByIdAsync(int id)
    {
        logger.LogInformation("GetByIdAsync called for ID: {Id}", id);

        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            logger.LogWarning("GetByIdAsync failed. Company not found with ID: {Id}", id);
            return ServiceResult<CompanytDto>.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var companyAsDto = mapper.Map<CompanytDto>(company);
        logger.LogInformation("GetByIdAsync succeeded for ID: {Id}", id);
        return ServiceResult<CompanytDto>.Success(companyAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCompanyRequest request)
    {
        logger.LogInformation("UpdateAsync called for ID: {Id}", id);

        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            logger.LogWarning("UpdateAsync failed. Company not found with ID: {Id}", id);
            return ServiceResult.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var isCompanyNameExist = await companyRepository
            .Where(x => x.Name == request.Name && x.Id != id)
            .AnyAsync();

        if (isCompanyNameExist)
        {
            logger.LogWarning("UpdateAsync failed. Company name '{Name}' already exists.", request.Name);
            return ServiceResult.Fail("This company name already exists.", HttpStatusCode.BadRequest);
        }

        mapper.Map(request, company);

        if (request.LogoUrl is not null)
        {
            var logoUrl = await cloudinaryService.UploadImage(request.LogoUrl, "logos");
            company.LogoUrl = logoUrl;
        }

        companyRepository.Update(company);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("UpdateAsync completed successfully for ID: {Id}", id);
        return ServiceResult.Success("Company updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<CompanyWithJobPostingsDto>> GetCompanyWithJobPostingsAsync(int companyId)
    {
        logger.LogInformation("GetCompanyWithJobPostingsAsync called for ID: {CompanyId}", companyId);

        var company = await companyRepository.GetCompanyWithJobPostingsAsync(companyId);
        if (company is null)
        {
            logger.LogWarning("GetCompanyWithJobPostingsAsync failed. Company not found with ID: {CompanyId}", companyId);
            return ServiceResult<CompanyWithJobPostingsDto>.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var companyAsDto = mapper.Map<CompanyWithJobPostingsDto>(company);
        logger.LogInformation("GetCompanyWithJobPostingsAsync succeeded for ID: {CompanyId}", companyId);
        return ServiceResult<CompanyWithJobPostingsDto>.Success(companyAsDto);
    }

    public async Task<ServiceResult<List<CompanyWithJobPostingsDto>>> GetCompanyWithJobPostingsAsync()
    {
        logger.LogInformation("GetCompanyWithJobPostingsAsync (all) called.");

        var companies = await companyRepository.GetCompanyWithJobPostings().ToListAsync();
        var companiesAsDto = mapper.Map<List<CompanyWithJobPostingsDto>>(companies);

        logger.LogInformation("GetCompanyWithJobPostingsAsync returned {Count} companies.", companiesAsDto.Count);
        return ServiceResult<List<CompanyWithJobPostingsDto>>.Success(companiesAsDto);
    }
}
