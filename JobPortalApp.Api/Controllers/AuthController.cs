using JobPortalApp.Service.Auths.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Security.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalApp.Api.Controllers;

public class AuthController(IAuthenticationService authenticationService, IUserService userService) : CustomBaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto) => CreateActionResult(await authenticationService.CreateTokenAsync(loginDto));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
       => CreateActionResult(await userService.RegisterAsync(registerDto));

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenDto refreshTokenDto) => CreateActionResult(await authenticationService.RevokeRefreshToken(refreshTokenDto.Token));

    [HttpPost("create-token-by-refresh-token")]
    public async Task<IActionResult> CreateTokenByRefreshToken([FromBody] RefreshTokenDto refreshTokenDto) => CreateActionResult(await authenticationService
        .CreateTokenByRefreshToken(refreshTokenDto.Token));
}
