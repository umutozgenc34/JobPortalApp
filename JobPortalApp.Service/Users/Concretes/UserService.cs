using AutoMapper;
using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Users.Abstracts;
using JobPortalApp.Service.Users.Abstracts;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static StackExchange.Redis.Role;

namespace JobPortalApp.Service.Users.Concretes;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IUserProfileRepository _userProfileRepository;

    public UserService(UserManager<User> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager,IUserProfileRepository userProfileRepository)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _userProfileRepository = userProfileRepository;

    }

    public async Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            return ServiceResult<UserDto>.Fail("Email already in use.", HttpStatusCode.BadRequest);
        }

        if (await _userManager.FindByNameAsync(registerDto.UserName) != null)
        {
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
            return ServiceResult<UserDto>.Fail(result.Errors.Select(e => e.Description).ToList(), HttpStatusCode.BadRequest);
        }

        if (!await _roleManager.RoleExistsAsync("user"))
        {
            await _roleManager.CreateAsync(new IdentityRole("user"));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "user");
        if (!roleResult.Succeeded)
        {
            return ServiceResult<UserDto>.Fail(roleResult.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        var userAsDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.Created);
    }

    public async Task<ServiceResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        return ServiceResult.Success("User deleted.", HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<UserDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        if (users.Count == 0)
        {
            return ServiceResult<List<UserDto>>.Fail("Users not found.", HttpStatusCode.NotFound);
        }

        var userDtos = _mapper.Map<List<UserDto>>(users);
        return ServiceResult<List<UserDto>>.Success(userDtos, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("User not found.", HttpStatusCode.NotFound);
        }

        var userDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return ServiceResult<UserDto>.Fail("Username not found.", HttpStatusCode.NotFound);
        }

        var userAsDto = _mapper.Map<UserDto>(user);
        return ServiceResult<UserDto>.Success(userAsDto, HttpStatusCode.OK);
    }

    public async Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return ServiceResult.Fail("User not found.", HttpStatusCode.NotFound);
        }

        user.UserName = request.UserName;
        user.Email = request.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return ServiceResult.Fail(result.Errors.Select(e => e.Description).First(), HttpStatusCode.BadRequest);
        }

        return ServiceResult.Success("User updated.", HttpStatusCode.OK);
    }

    public async Task<ServiceResult<List<UserWithProfileDto>>> GetUsersWithProfilesAsync()
    {
        var users = await _userProfileRepository.GetUsersWithProfilesAsync(); 
        var usersAsDtos = _mapper.Map<List<UserWithProfileDto>>(users);
        return ServiceResult<List<UserWithProfileDto>>.Success(usersAsDtos);
    }
}