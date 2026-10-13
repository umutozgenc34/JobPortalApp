using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Service.Tokens.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security;
using JobPortalApp.Shared.Security.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace JobPortalApp.Service.Users.Concretes;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRefreshTokenService _userRefreshTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager,IUserProfileRepository userProfileRepository,
                       IUserRefreshTokenService userRefreshTokenService, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _userProfileRepository = userProfileRepository;
        _userRefreshTokenService = userRefreshTokenService;
        _logger = logger;
    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        _logger.LogInformation("RegisterAsync called with Email: {Email}, UserName: {UserName}", registerDto.Email, registerDto.UserName);

        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            _logger.LogWarning("Registration failed: Email already in use - {Email}", registerDto.Email);
            return ServiceResult<UserDto>.Fail("Email already in use.", HttpStatusCode.BadRequest);
        }

        if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
        {
            _logger.LogWarning("Registration failed: Username already taken - {UserName}", registerDto.UserName);
            return ServiceResult<UserDto>.Fail("Username already taken.", HttpStatusCode.BadRequest);
        }

        var user = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return ServiceResult<UserDto>.Fail(result.Errors.Select(e => e.Description).ToList(), HttpStatusCode.BadRequest);
        }

        if (!await _roleManager.RoleExistsAsync(AppRoles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(AppRoles.User));
            _logger.LogInformation("Role '{Role}' created during registration.", AppRoles.User);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, AppRoles.User);
        if (!roleResult.Succeeded)
        {
            _logger.LogWarning("Failed to assign role '{Role}' to {UserName}: {Errors}", AppRoles.User, registerDto.UserName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            await _userManager.DeleteAsync(user);
            return ServiceResult<UserDto>.Fail(roleResult.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        _logger.LogInformation("User registered successfully: {UserName}", registerDto.UserName);
        var userAsDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> DeleteAsync(string id)
    {
        _logger.LogInformation("DeleteAsync called for UserId: {UserId}", id);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("DeleteAsync failed - User not found: {UserId}", id);
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var refreshToken = await _userRefreshTokenService.GetByIdAsync(id);
        if (refreshToken is not null)
        {
            _userRefreshTokenService.Delete(refreshToken);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogWarning("DeleteAsync failed for UserId {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            return ServiceResult.Fail(result.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        _logger.LogInformation("User deleted successfully: {UserId}", id);
        return ServiceResult.Success("User deleted.", HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<UserDto>>> GetAllAsync()
    {
        _logger.LogInformation("GetAllAsync called.");

        var users = await _userManager.Users.AsNoTracking().OrderBy(u => u.UserName).ToListAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);
        _logger.LogInformation("GetAllAsync succeeded - {UserCount} users found.", users.Count);
        return ServiceResult<List<UserDto>>.Success(userDtos, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(string id)
    {
        _logger.LogInformation("GetByIdAsync called for UserId: {UserId}", id);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("GetByIdAsync failed - User not found: {UserId}", id);
            return ServiceResult<UserDto>.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var userDto = _mapper.Map<UserDto>(user);
        _logger.LogInformation("GetByIdAsync succeeded for UserId: {UserId}", id);
        return ServiceResult<UserDto>.Success(userDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<CurrentUserDto>> GetCurrentUserAsync(string userId)
    {
        _logger.LogInformation("GetCurrentUserAsync called for UserId: {UserId}", userId);

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("GetCurrentUserAsync failed - User not found: {UserId}", userId);
            return ServiceResult<CurrentUserDto>.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var currentUser = new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList()
        };

        return ServiceResult<CurrentUserDto>.Success(currentUser);
    }

    public async Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName)
    {
        _logger.LogInformation("GetUserByNameAsync called for UserName: {UserName}", userName);

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            _logger.LogWarning("GetUserByNameAsync failed - Username not found: {UserName}", userName);
            return ServiceResult<UserDto>.Fail("Username not found.", HttpStatusCode.NotFound);
        }

        var userAsDto = _mapper.Map<UserDto>(user);
        _logger.LogInformation("GetUserByNameAsync succeeded for UserName: {UserName}", userName);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request)
    {
        _logger.LogInformation("UpdateAsync called for UserId: {UserId}", id);

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("UpdateAsync failed - User not found: {UserId}", id);
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogWarning("UpdateAsync failed for UserId {UserId}: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
            return ServiceResult.Fail(result.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        _logger.LogInformation("UpdateAsync succeeded for UserId: {UserId}", id);
        return ServiceResult.Success("User updated.", HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<UserWithProfileDto>>> GetUsersWithProfilesAsync()
    {
        _logger.LogInformation("GetUsersWithProfilesAsync called.");

        var users = await _userProfileRepository.GetUsersWithProfilesAsync();
        var usersAsDtos = _mapper.Map<List<UserWithProfileDto>>(users);

        _logger.LogInformation("GetUsersWithProfilesAsync succeeded - {Count} users with profiles found.", usersAsDtos.Count);
        return ServiceResult<List<UserWithProfileDto>>.Success(usersAsDtos);
    }
}