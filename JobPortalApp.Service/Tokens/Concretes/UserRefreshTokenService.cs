using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Contexts;
using JobPortalApp.Service.Tokens.Abstracts;
using JobPortalApp.Shared.Repositories.Concretes;

namespace JobPortalApp.Service.Tokens.Concretes;

public class UserRefreshTokenService(AppDbContext context) : GenericRepository<AppDbContext, UserRefreshToken, string>(context), IUserRefreshTokenService;