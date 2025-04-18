﻿using AutoMapper;
using JobPortalApp.Model.Companies.Dtos;
using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Companies.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Services.Cloudinaryy.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace JobPortalApp.Service.Companies.Concretes;

public class CompanyService(ICompanyRepository companyRepository, IUnitOfWork unitOfWork, IMapper mapper,ICloudinaryService cloudinaryService) : ICompanyService
{
    public async Task<ServiceResult<CompanytDto>> CreateAsync(CreateCompanyRequest request)
    {
        var existingCompany = await companyRepository.Where(c => c.Name == request.Name).FirstOrDefaultAsync();
        if (existingCompany is not null)
        {
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

        var companyAsDto = mapper.Map<CompanytDto>(company);
        return ServiceResult<CompanytDto>.SuccessAsCreated(companyAsDto, $"companies/{company.Id}");
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            return ServiceResult.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        companyRepository.Delete(company);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success("Company deleted successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<List<CompanytDto>>> GetAllAsync()
    {
        var companies = await companyRepository.GetAllAsync();
        var companyAsDtos = mapper.Map<List<CompanytDto>>(companies);
        return ServiceResult<List<CompanytDto>>.Success(companyAsDtos);
    }

    public async Task<ServiceResult<CompanytDto>> GetByIdAsync(int id)
    {
        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            return ServiceResult<CompanytDto>.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var companyAsDto = mapper.Map<CompanytDto>(company);
        return ServiceResult<CompanytDto>.Success(companyAsDto);
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateCompanyRequest request)
    {
        var company = await companyRepository.GetByIdAsync(id);
        if (company is null)
        {
            return ServiceResult.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var isCompanyNameExist = await companyRepository
            .Where(x => x.Name == request.Name && x.Id != id)
            .AnyAsync();

        if (isCompanyNameExist)
        {
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

        return ServiceResult.Success("Company updated successfully.", HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult<CompanyWithJobPostingsDto>> GetCompanyWithJobPostingsAsync(int companyId)
    {
        var company = await companyRepository.GetCompanyWithJobPostingsAsync(companyId);
        if (company is null)
        {
            return ServiceResult<CompanyWithJobPostingsDto>.Fail("Company not found.", HttpStatusCode.NotFound);
        }

        var companyAsDto = mapper.Map<CompanyWithJobPostingsDto>(company);
        return ServiceResult<CompanyWithJobPostingsDto>.Success(companyAsDto);
    }

    public async Task<ServiceResult<List<CompanyWithJobPostingsDto>>> GetCompanyWithJobPostingsAsync()
    {
        var companies = await companyRepository.GetCompanyWithJobPostings().ToListAsync();
        var companiesAsDto = mapper.Map<List<CompanyWithJobPostingsDto>>(companies);
        return ServiceResult<List<CompanyWithJobPostingsDto>>.Success(companiesAsDto);
    }
}
