using JobPortalApp.Model.Companies.Entities;
using JobPortalApp.Repository.Companies.Abstracts;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Repository.Companies.Concretes;

public class CompanyReviewRepository(AppDbContext context) : GenericRepository<AppDbContext,CompanyReview,int>(context),ICompanyReviewRepository;
