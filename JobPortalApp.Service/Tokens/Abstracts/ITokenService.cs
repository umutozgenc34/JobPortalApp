using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Tokens.Abstracts;

public interface ITokenService
{
    Task<TokenDto> CreateTokenAsync(User user);
}