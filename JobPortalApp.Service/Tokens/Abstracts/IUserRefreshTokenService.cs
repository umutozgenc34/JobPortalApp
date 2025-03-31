using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Repositories.Abstracts;

namespace JobPortalApp.Service.Tokens.Abstracts;

public interface IUserRefreshTokenService : IGenericRepository<UserRefreshToken, string>;