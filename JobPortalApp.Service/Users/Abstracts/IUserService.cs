using JobPortalApp.Model.Users.Dtos;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Security.Dtos;

namespace JobPortalApp.Service.Users.Abstracts;

public interface IUserService
{
    Task<ServiceResult<UserDto>> RegisterAsync(RegisterDto registerDto);
    Task<ServiceResult<UserDto>> GetUserByNameAsync(string userName);
    Task<ServiceResult<List<UserDto>>> GetAllAsync();
    Task<ServiceResult<UserDto>> GetByIdAsync(string id);
    Task<ServiceResult> UpdateAsync(string id, UpdateUserRequest request);
    Task<ServiceResult> DeleteAsync(string id);
}