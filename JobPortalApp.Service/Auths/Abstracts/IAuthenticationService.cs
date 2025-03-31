using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Auths.Abstracts;

public interface IAuthenticationService
{
    Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto);
    Task<ServiceResult<TokenDto>> CreateTokenByRefreshToken(string refreshToken);
    Task<ServiceResult> RevokeRefreshToken(string refreshToken);
}