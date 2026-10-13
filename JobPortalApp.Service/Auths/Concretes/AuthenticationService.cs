using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Auths.Abstracts;
using JobPortalApp.Service.Tokens.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace JobPortalApp.Service.Auths.Concretes;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRefreshTokenService _userRefreshTokenService;
    public AuthenticationService(ITokenService tokenService, UserManager<User> userManager, IUnitOfWork unitOfWork, IUserRefreshTokenService userRefreshTokenService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _userRefreshTokenService = userRefreshTokenService;
    }
    public async Task<ServiceResult<TokenDto>> CreateTokenAsync(LoginDto loginDto)
    {
        ArgumentNullException.ThrowIfNull(loginDto);

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null) return ServiceResult<TokenDto>.Fail("Email or Password wrong.", HttpStatusCode.BadRequest);

        if (await _userManager.IsLockedOutAsync(user))
        {
            return ServiceResult<TokenDto>.Fail("Too many failed login attempts. Please try again later.", HttpStatusCode.TooManyRequests);
        }

        if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return ServiceResult<TokenDto>.Fail("Email or Password wrong.", HttpStatusCode.BadRequest);
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var token = await _tokenService.CreateTokenAsync(user);
        var userRefreshToken = await _userRefreshTokenService.Where(x => x.Id == user.Id).SingleOrDefaultAsync();
        if (userRefreshToken == null)
        {
            await _userRefreshTokenService.AddAsync(new UserRefreshToken { Id = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
        }
        else
        {
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;
        }
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<TokenDto>.Success(token);
    }
    public async Task<ServiceResult<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
        if (existRefreshToken == null)
        {
            return ServiceResult<TokenDto>.Fail("Refresh token not found.", HttpStatusCode.NotFound);
        }

        if (existRefreshToken.Expiration < DateTime.UtcNow)
        {
            _userRefreshTokenService.Delete(existRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<TokenDto>.Fail("Refresh token expired.", HttpStatusCode.Unauthorized);
        }

        var user = await _userManager.FindByIdAsync(existRefreshToken.Id);
        if (user == null)
        {
            return ServiceResult<TokenDto>.Fail("User Id not found.", HttpStatusCode.NotFound);
        }
        var tokenDto = await _tokenService.CreateTokenAsync(user);
        existRefreshToken.Code = tokenDto.RefreshToken;
        existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<TokenDto>.Success(tokenDto);
    }
    public async Task<ServiceResult> RevokeRefreshToken(string refreshToken)
    {
        var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
        if (existRefreshToken == null)
        {
            return ServiceResult.Fail("Refresh token not found.", HttpStatusCode.NotFound);
        }
        _userRefreshTokenService.Delete(existRefreshToken);
        await _unitOfWork.SaveChangesAsync();
        return ServiceResult.Success("Refresh token silindi.", HttpStatusCode.NoContent);
    }
}
