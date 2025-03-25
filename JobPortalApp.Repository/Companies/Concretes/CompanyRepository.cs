using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Repository.Companies.Concretes;

public class CompanyRepository(AppDbContext context) : GenericRepository<AppDbContext, Company, int>(context), ICompanyRepository;

